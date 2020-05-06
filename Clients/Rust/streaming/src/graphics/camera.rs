
extern crate nalgebra_glm as glm;
use glm::{Mat4, Vec3};


// Defines several possible options for camera movement.
// Used as abstraction to stay away from window-system specific input methods.
#[derive(PartialEq, Clone, Copy)]
pub enum CameraMovement {
    FORWARD,
    BACKWARD,
    LEFT,
    RIGHT,
    UP,
    DOWN
}

use self::CameraMovement::*;

// Default camera values
const YAW: f32 = -90.0;
const PITCH: f32 = 0.0;
const SPEED: f32 = 2.5;
const SENSITIVITY: f32 = -0.1;
const ZOOM: f32 = 45.0;

pub struct Camera {
    pub position: Vec3,
    pub front: Vec3,
    pub up: Vec3,
    pub right: Vec3,
    pub world_up: Vec3,
    // Euler angles
    pub yaw: f32,
    pub pitch: f32,
    // Camera options
    pub movement_speed: f32,
    pub mouse_sensitivity: f32,
    pub zoom: f32,
}

impl Default for Camera {
    fn default() -> Camera {
        let mut camera = Camera {
            position: glm::vec3(0.0, 0.0, 0.0),
            front: glm::vec3(0.0, 0.0, -1.0),
            up: glm::zero(),
            right: glm::zero(),
            world_up: glm::vec3(0.0, 1.0, 0.0).normalize(),
            yaw: YAW,
            pitch: PITCH,
            movement_speed: SPEED,
            mouse_sensitivity: SENSITIVITY,
            zoom: ZOOM,
        };
        
        camera.update_camera_vectors();
        camera

    }
}

impl Camera {
    /// Returns the view matrix calculated using Euler Angles and the LookAt matrix
    pub fn get_view_matrix(&self) -> Mat4 {
        let center = self.position + self.front;
        glm::look_at(&self.position, &center, &self.up)
    }

    /// Processes input received from any keyboard-like input system.
    /// Accepts input parameter in the form of camera defined ENUM.
    pub fn process_keyboard(&mut self, direction: CameraMovement, delta_time: f32) {
        let velocity = self.movement_speed * delta_time;
        match direction {
            FORWARD => self.position += self.front * velocity,
            BACKWARD => self.position += -(self.front * velocity),
            LEFT => self.position += -(self.right * velocity),
            RIGHT => self.position += self.right * velocity,
            UP => self.position += self.up * velocity,
            DOWN => self.position += -(self.up * velocity)
        }
    }

    pub fn process_mouse_movement(&mut self, mut x_offset: f32, mut y_offset: f32, constrain_pitch: bool) {
        x_offset *= self.mouse_sensitivity;
        y_offset *= self.mouse_sensitivity;
       
        // Need -= because += is inverted
        self.yaw -= x_offset;
        self.pitch -= y_offset;

        // Make sure that when pitch is out of bounds screen does not get flipped
        if constrain_pitch {
            if self.pitch > 89.0 {
                self.pitch = 89.0;
            }
            if self.pitch < -89.0 {
                self.pitch = -89.0;
            }
        }

        // Update, Front, Right and Up Vectors using the updated Euler angles.
        self.update_camera_vectors();

    }

    pub fn process_mouse_scroll(&mut self, y_offset: f32) {
        if self.zoom >= 1.0 && self.zoom <= 45.0 {
            self.zoom -= y_offset;
        }
        if self.zoom <= 1.0 {
            self.zoom = 1.0;
        }
        if self.zoom >= 45.0 {
            self.zoom = 45.0;
        }
    }

    fn update_camera_vectors(&mut self)
    {
        //Calculate the new Front vector
        let front = glm::vec3
        (
            self.yaw.to_radians().cos() * self.pitch.to_radians().cos(),
            self.pitch.to_radians().sin(),
            self.yaw.to_radians().sin() * self.pitch.to_radians().cos(),
        );
        self.front = front.normalize();
        // Also re-calculate the Right and Up vector 
        self.right = self.front.cross(&self.world_up).normalize();
        self.up = self.right.cross(&self.front).normalize();
    }
}
