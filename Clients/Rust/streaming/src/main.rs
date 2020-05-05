pub mod input;

extern crate glfw;
use self::glfw::{Context};

use input::control;

extern crate image;

// settings
const SCR_WIDTH: u32 = 1024;
const SCR_HEIGHT: u32 = 768;

fn main() {
    let mut first_mouse = true;
    let mut last_x: f32 = SCR_WIDTH as f32 / 2.0;
    let mut last_y: f32 = SCR_HEIGHT as f32 / 2.0;

    // timing 
    let mut delta_time: f32 = 0.0;
    let mut last_frame: f32 = 0.0;

    let mut glfw = glfw::init(glfw::FAIL_ON_ERRORS).unwrap();
    glfw.window_hint(glfw::WindowHint::ContextVersion(3, 3));
    glfw.window_hint(glfw::WindowHint::OpenGlProfile(glfw::OpenGlProfileHint::Core));

    #[cfg(target_os = "macos")]
    glfw.window_hint(glfw::WindowHint::OpenGlForwardCompat(true));

    let (mut window, events) = glfw.create_window(SCR_WIDTH, SCR_HEIGHT, "Streaming", glfw::WindowMode::Windowed)
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
    window.set_cursor_mode(glfw::CursorMode::Disabled);

    // gl: load all OpenGL function pointers
    // ---------------------------------------
    gl::load_with(|symbol| window.get_proc_address(symbol) as *const _);

    while !window.should_close() {
        // events
        // -----
        control::process_events(&events, &mut first_mouse, &mut last_x, &mut last_y);

        // input
        control::process_input(&mut window, delta_time);

        // render
        // ------

        // per-frame time logic
        let current_frame = glfw.get_time() as f32;
        delta_time = current_frame - last_frame;
        last_frame = current_frame;

        //println!("fps: {}", 1.0 / delta_time);
        // events

        unsafe {
            gl::ClearColor(0.2, 0.3, 0.3, 1.0);
            gl::Clear(gl::COLOR_BUFFER_BIT | gl::DEPTH_BUFFER_BIT);

        }

        // glfw: swap buffers and poll IO events (keys pressed/released, mouse moved etc.)
        // -------------------------------------------------------------------------------
        window.swap_buffers();
        glfw.poll_events();
    }

}
