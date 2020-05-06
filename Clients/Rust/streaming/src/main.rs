pub mod graphics;
pub mod input;

extern crate glfw;
use self::glfw::{Context};

extern crate gl;
use self::gl::types::*;

//use std::sync::mpsc::Receiver;
use std::ptr;
use std::mem;
use std::os::raw::c_void;


extern crate byte_strings;
use byte_strings::c_str;

extern crate image;
use image::DynamicImage::*;
use image::GenericImageView;

extern crate nalgebra_glm as glm;
use glm::{Mat4, Vec3};

use graphics::shader::Shader;
use graphics::texture::*;
use graphics::texture::Texture;
use graphics::camera::Camera;
use graphics::model::Model;

use input::control;


// settings
const SCR_WIDTH: u32 = 1024;
const SCR_HEIGHT: u32 = 768;

use tonic::{transport::Server, Request, Response, Status};

pub mod sensordata {
    tonic::include_proto!("sensordata");
}

use sensordata::sensordata_client::SensordataClient;
use sensordata::SensordataRequest;

//fn main()

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {


   let mut client = SensordataClient::connect("http://[::1]:50084").await?;

    let mut camera = Camera {
        position: glm::vec3(0.0, 0.0, 3.0),
        ..Camera::default()
    };

    let mut first_mouse = true;
    let mut last_x: f32 = SCR_WIDTH as f32 / 2.0;
    let mut last_y: f32 = SCR_HEIGHT as f32 / 2.0;

    // timing 
    let mut delta_time: f32 = 0.0;
    let mut last_frame: f32 = 0.0;

    // lighting
    //let light_pos = glm::vec3(1.2, 1.0, 2.0);

    // glfw: initialize and configure
    // ------------------------------
    let mut glfw = glfw::init(glfw::FAIL_ON_ERRORS).unwrap();
    glfw.window_hint(glfw::WindowHint::ContextVersion(3, 3));
    glfw.window_hint(glfw::WindowHint::OpenGlProfile(glfw::OpenGlProfileHint::Core));

    #[cfg(target_os = "macos")]
    glfw.window_hint(glfw::WindowHint::OpenGlForwardCompat(true));

    // glfw window creation
    // --------------------
    let (mut window, events) = glfw.create_window(SCR_WIDTH, SCR_HEIGHT, "Rust Streaming Client", glfw::WindowMode::Windowed)
        .expect("Failed to create GLFW window");

    // Fullscreen
    /*
    glfw.with_primary_monitor_mut(|_: &mut _, m: Option<&glfw::Monitor>| {

        let monitor = m.unwrap();

        let mode = monitor.get_video_mode().unwrap();

        window.set_monitor(glfw::WindowMode::FullScreen(&monitor), 0, 0, mode.width, mode.height, Some(mode.refresh_rate));

        println!("{}x{} fullscreen enabled at {}Hz on monitor {}", mode.width, mode.height, mode.refresh_rate, monitor.get_name().unwrap());
    });
    */


    window.make_current();
    window.set_key_polling(true);
    window.set_framebuffer_size_polling(true);
    window.set_cursor_pos_polling(true);
    window.set_scroll_polling(true);

    // Tell glfw to capture mouse input
    //window.set_cursor_mode(glfw::CursorMode::Enabled);

    // gl: load all OpenGL function pointers
    // ---------------------------------------
    gl::load_with(|symbol| window.get_proc_address(symbol) as *const _);

    let (ourShader, VBO, VAO, EBO, mut texture) = unsafe {
        // build and compile our shader program
        // ------------------------------------
        let ourShader = Shader::new(
            "shaders/streaming_shader.vs",
            "shaders/streaming_shader.fs");

        // set up vertex data (and buffer(s)) and configure vertex attributes
        // ------------------------------------------------------------------
        // HINT: type annotation is crucial since default for float literals is f64
        /*
        let vertices: [f32; 32] = [
            // positions       // colors        // texture coords
             0.5,  0.5, 0.0,   1.0, 0.0, 0.0,   1.0, 1.0, // top right
             0.5, -0.5, 0.0,   0.0, 1.0, 0.0,   1.0, 0.0, // bottom right
            -0.5, -0.5, 0.0,   0.0, 0.0, 1.0,   0.0, 0.0, // bottom left
            -0.5,  0.5, 0.0,   1.0, 1.0, 0.0,   0.0, 1.0  // top left
        ];

        */

        let vertices: [f32; 32] = [
            // positions       // colors        // texture coords
             1.0,  1.0, 0.0,   1.0, 0.0, 0.0,   1.0, 1.0, // top right
             1.0, -1.0, 0.0,   0.0, 1.0, 0.0,   1.0, 0.0, // bottom right
            -1.0, -1.0, 0.0,   0.0, 0.0, 1.0,   0.0, 0.0, // bottom left
            -1.0,  1.0, 0.0,   1.0, 1.0, 0.0,   0.0, 1.0  // top left
        ];

        let indices = [
            0, 1, 3,  // first Triangle
            1, 2, 3   // second Triangle
        ];
        let (mut VBO, mut VAO, mut EBO) = (0, 0, 0);
        gl::GenVertexArrays(1, &mut VAO);
        gl::GenBuffers(1, &mut VBO);
        gl::GenBuffers(1, &mut EBO);

        gl::BindVertexArray(VAO);

        gl::BindBuffer(gl::ARRAY_BUFFER, VBO);
        gl::BufferData(gl::ARRAY_BUFFER,
                       (vertices.len() * mem::size_of::<GLfloat>()) as GLsizeiptr,
                       &vertices[0] as *const f32 as *const c_void,
                       gl::STATIC_DRAW);

        gl::BindBuffer(gl::ELEMENT_ARRAY_BUFFER, EBO);
        gl::BufferData(gl::ELEMENT_ARRAY_BUFFER,
                       (indices.len() * mem::size_of::<GLfloat>()) as GLsizeiptr,
                       &indices[0] as *const i32 as *const c_void,
                       gl::STATIC_DRAW);

        let stride = 8 * mem::size_of::<GLfloat>() as GLsizei;
        // position attribute
        gl::VertexAttribPointer(0, 3, gl::FLOAT, gl::FALSE, stride, ptr::null());
        gl::EnableVertexAttribArray(0);
        // color attribute
        gl::VertexAttribPointer(1, 3, gl::FLOAT, gl::FALSE, stride, (3 * mem::size_of::<GLfloat>()) as *const c_void);
        gl::EnableVertexAttribArray(1);
        // texture coord attribute
        gl::VertexAttribPointer(2, 2, gl::FLOAT, gl::FALSE, stride, (6 * mem::size_of::<GLfloat>()) as *const c_void);
        gl::EnableVertexAttribArray(2);


        // Load and create a texture
        let mut texture = graphics::texture::Texture::new("resources/textures/container.jpg");


        (ourShader, VBO, VAO, EBO, texture)
    };

    let mut img = texture.image.as_mut_rgb8();

    let mut counter = 0;
    // render loop
    // -----------
    while !window.should_close() {
        // events
        // -----
        control::process_events(&events, &mut first_mouse, &mut last_x, &mut last_y);

        // input
        control::process_input(&mut window, delta_time);

        // render
        // ------
        unsafe {
            gl::ClearColor(0.2, 0.3, 0.3, 1.0);
            gl::Clear(gl::COLOR_BUFFER_BIT);

            // bind Texture
            gl::BindTexture(gl::TEXTURE_2D, texture.id);


            let request = tonic::Request::new(SensordataRequest{
                operation: "streaming".to_string()
            });
    
    
            let mut stream = client.stream_sensordata(request)
                .await?
                .into_inner();
    
            
    
            while let Some(data) = stream.message().await? {
                //println!("DATA = {:?}", data);

                let buffer: &[u8] = &data.data;
                texture.data = buffer.to_vec();
                //image::save_buffer("image.png", buffer, 800, 640, image::ColorType::Rgb8).unwrap();
                
                // TODO: Find a way to use gl::TexSubImage2D for updating the texture instead.
                gl::TexImage2D(
                    gl::TEXTURE_2D, 
                    0, 
                    gl::RGB as i32, 
                    800 as i32, 
                    640 as i32,
                    0, 
                    gl::RGB, 
                    gl::UNSIGNED_BYTE, 
                    &buffer[0] as *const u8 as *const c_void
                );
            }


            // render container
            ourShader.use_program();
            gl::BindVertexArray(VAO);
            gl::DrawElements(gl::TRIANGLES, 6, gl::UNSIGNED_INT, ptr::null());

            gl::BindTexture(gl::TEXTURE_2D, 0);

        }

        // per-frame time logic
        let current_frame = glfw.get_time() as f32;
        delta_time = current_frame - last_frame;
        last_frame = current_frame;

        println!("fps: {}", 1.0 / delta_time);
        // events

        // glfw: swap buffers and poll IO events (keys pressed/released, mouse moved etc.)
        // -------------------------------------------------------------------------------
        window.swap_buffers();
        glfw.poll_events();
    }

    // TODO - move this into an own function
    unsafe {
        //gl::DeleteVertexArrays(1, &cube_vao);
        //gl::DeleteVertexArrays(1, &light_vao);
        //gl::DeleteBuffers(1, &vbo);
        //gl::DeleteBuffers(1, &ebo);
    }

    Ok(())
}

unsafe fn draw_coordinates() {


}
