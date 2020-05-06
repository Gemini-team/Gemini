//use std::os::raw::c_void;

//use std::path::Path;
use std::sync::mpsc::Receiver;

extern crate gl;

extern crate glfw;
use self::glfw::{Key, Action};

extern crate image;

//use image::GenericImage;
//use image::DynamicImage::*;

//use crate::camera::{Camera, CameraMovement::*};
//use rust_opengl::graphics::camera::{ Camera, CameraMovement::*};
//use graphics::camera::{Camera, CameraMovement::*};

//use crate::graphics::camera::{Camera, CameraMovement::*};

/// Event processing function as introduced in 1.7.4 (Camera Class) and used in
/// most later tutorials
pub fn process_events(events: &Receiver<(f64, glfw::WindowEvent)>,
                  first_mouse: &mut bool,
                  last_x: &mut f32,
                  last_y: &mut f32) {
    for (_, event) in glfw::flush_messages(events) {
        match event {
            glfw::WindowEvent::FramebufferSize(width, height) => {
                // make sure the viewport matches the new window dimensions; note that width and
                // height will be significantly larger than specified on retina displays.
                unsafe { gl::Viewport(0, 0, width, height) }
            }
            glfw::WindowEvent::CursorPos(x_pos, y_pos) => {
                let (x_pos, y_pos) = (x_pos as f32, y_pos as f32);
                if *first_mouse {
                    *last_x = x_pos;
                    *last_y = y_pos;
                    *first_mouse = false;
                }

                let x_offset = x_pos - *last_x;
                let y_offset = *last_y - y_pos; // reversed since y-coordinates go from bottom to top

                *last_x = x_pos;
                *last_y = y_pos;

                //camera.process_mouse_movement(x_offset, y_offset, true);
            }
            /*
            glfw::WindowEvent::Scroll(_xoffset, yoffset) => {
                camera.process_mouse_scroll(yoffset as f32);
            }
            */
            _ => {}
        }
    }
}


pub fn process_input(window: &mut glfw::Window, delta_time: f32) {
    if window.get_key(Key::Escape) == Action::Press {
        window.set_should_close(true)
    }
}
/*
/// Input processing function as introduced in 1.7.4 (Camera Class) and used in
/// most later tutorials
pub fn process_input(window: &mut glfw::Window, delta_time: f32, camera: &mut Camera) {
    if window.get_key(Key::Escape) == Action::Press {
        window.set_should_close(true)
    }


    if window.get_key(Key::W) == Action::Press {
        camera.process_keyboard(FORWARD, delta_time);
    }
    if window.get_key(Key::S) == Action::Press {
        camera.process_keyboard(BACKWARD, delta_time);
    }
    if window.get_key(Key::A) == Action::Press {
        camera.process_keyboard(LEFT, delta_time);
    }
    if window.get_key(Key::D) == Action::Press {
        camera.process_keyboard(RIGHT, delta_time);
    }
    if window.get_key(Key::E) == Action::Press {
        camera.process_keyboard(UP, delta_time);
    }
    if window.get_key(Key::Q) == Action::Press {
        camera.process_keyboard(DOWN, delta_time);
    }
}
*/