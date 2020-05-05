
extern crate gl;
use self::gl::types::*;

extern crate image;
use image::GenericImageView;
use image::DynamicImage::*;

use std::path::Path;
use std::os::raw::c_void;

pub struct Texture {
    pub id: GLuint,  
    pub image: image::DynamicImage,
    pub data: Vec<u8>,
}

impl Texture {

    // TODO - This should take all the parameters one would want
    // for the texture, i.e texture wrapping
    pub fn new(filepath: &str) -> Texture {
        let (image, data) = load_texture_from_file(filepath);

        let mut texture_id: GLuint = 0;

        unsafe {

            gl::GenTextures(1, &mut texture_id);
            gl::BindTexture(gl::TEXTURE_2D, texture_id);

            // set the texture wrapping parameters
            gl::TexParameteri(gl::TEXTURE_2D, gl::TEXTURE_WRAP_S, gl::REPEAT as i32);
            gl::TexParameteri(gl::TEXTURE_2D, gl::TEXTURE_WRAP_T, gl::REPEAT as i32);

            // set texture filtering parameteres
            gl::TexParameteri(gl::TEXTURE_2D, gl::TEXTURE_MIN_FILTER, gl::LINEAR as i32);
            gl::TexParameteri(gl::TEXTURE_2D, gl::TEXTURE_MAG_FILTER, gl::LINEAR as i32);

            // create texture and generate mipmaps
            gl::TexImage2D(
                gl::TEXTURE_2D,
                0,
                gl::RGB as i32,
                image.width() as i32,
                image.height() as i32,
                0,
                gl::RGB,
                gl::UNSIGNED_BYTE,
                &data[0] as *const u8 as *const c_void
            );

        }

        Texture{id: texture_id, image: image, data: data}
    }
}

// TODO -  This should return a result of whether the loading were
// successful or not
fn load_texture_from_file(filepath: &str) -> (image::DynamicImage, Vec<u8>) {

    let img = image::open(&Path::new(filepath)).expect("Failed to load texture");
    //let data = img.raw_pixels();
    let data = img.to_bytes();

    (img, data)

}

pub unsafe fn load_texture(path: &str) -> u32 {
    let mut texture_id = 0;

    gl::GenTextures(1, &mut texture_id);
    let img = image::open(&Path::new(path)).expect("Texture failed to load");
    let format = match img {
        // TODO: These two should be handled properly
        ImageLuma8(_) => gl::RED,
        ImageLumaA8(_) => gl::RG,

        ImageRgb8(_) => gl::RGB,
        ImageRgba8(_) => gl::RGBA,
        ImageBgr8(_) => gl::BGR,
        ImageBgra8(_) => gl::BGRA,
        // TODO: This should be taken care of in a proper way
        _ => gl::RED,
    };

    //let data = img.raw_pixels();
    let data = img.to_bytes();

    gl::BindTexture(gl::TEXTURE_2D, texture_id);
    gl::TexImage2D(gl::TEXTURE_2D, 0, format as i32, img.width() as i32, img.height() as i32,
        0, format, gl::UNSIGNED_BYTE, &data[0] as *const u8 as *const c_void);
    gl::GenerateMipmap(gl::TEXTURE_2D);

    gl::TexParameteri(gl::TEXTURE_2D, gl::TEXTURE_WRAP_S, gl::REPEAT as i32);
    gl::TexParameteri(gl::TEXTURE_2D, gl::TEXTURE_WRAP_T, gl::REPEAT as i32);
    gl::TexParameteri(gl::TEXTURE_2D, gl::TEXTURE_MIN_FILTER, gl::LINEAR_MIPMAP_LINEAR as i32);
    gl::TexParameteri(gl::TEXTURE_2D, gl::TEXTURE_MAG_FILTER, gl::LINEAR as i32);

    texture_id
}