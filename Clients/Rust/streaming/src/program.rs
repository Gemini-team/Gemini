
extern crate gl;
use self::gl::types::*;
use std::ptr;
use std::str;


pub struct Program {
    pub id: u32,
}

/**
 * TODO - Should pass this a vec of shaders since the amount of shaders in a
 * program can vary
 */
impl Program {
    pub fn new(vertex_shader: GLuint, fragment_shader: GLuint) -> Program {
        Program{id: link_program(vertex_shader, fragment_shader)}
    }
}

/**
 * TODO - Should do this for a list of shaders, since the program can include a variable
 * amount of shaders.
 */
fn link_program(vertex_shader: GLuint, fragment_shader: GLuint) -> GLuint {

    let mut success = gl::FALSE as GLint;
    let mut info_log = Vec::with_capacity(512);

    unsafe {
        let shader_program = gl::CreateProgram();
        gl::AttachShader(shader_program, vertex_shader);
        gl::AttachShader(shader_program, fragment_shader);
        

        gl::LinkProgram(shader_program);
        // check for linking errors

        gl::GetProgramiv(shader_program, gl::LINK_STATUS, &mut success);

        if success != gl::TRUE as GLint {
            gl::GetProgramInfoLog(shader_program, 512, ptr::null_mut(), info_log.as_mut_ptr() as *mut GLchar);
            println!("ERROR::SHADER::PROGRAM::COMPILATION_FAILED\n{}", str::from_utf8(&info_log).unwrap());
        }

        shader_program
    }

}