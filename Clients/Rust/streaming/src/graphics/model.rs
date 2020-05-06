use std::os::raw::c_void;
use std::path::Path;

use nalgebra_glm as glm;
use gl;
use image;
use image::DynamicImage::*;
use image::GenericImageView;
use tobj;

//use crate::mesh::{Mesh, Texture, Vertex };
//use crate::shader::Shader;

use super::mesh::{ Mesh, Texture, Vertex} ;
use super::shader::Shader;

#[derive(Default)]
pub struct Model {
    pub meshes: Vec<Mesh>,
    pub textures_loaded: Vec<Texture>,
    directory: String
}

impl Model {
    pub fn new(path: &str) -> Model {
        let mut model = Model::default();
        model.load_model(path);
        model
    }

    pub fn draw(&self, shader: &Shader) {
        for mesh in &self.meshes {
            unsafe { mesh.draw(shader) }
        }
    }

    fn load_model(&mut self, path: &str) {
        let path = Path::new(path);

        // retrieve the directory path of the filepath
        self.directory = path.parent().unwrap_or_else(|| Path::new("")).to_str().expect("Failed retreiving directory path for model.").into();
        let obj = tobj::load_obj(path);

        //let (models, materials) = obj.unwrap();
        let (models, materials) = obj.expect("Failed parsing models and materials from obj.");
        for model in models {
            let mesh = &model.mesh;
            let num_vertices = mesh.positions.len() / 3;

            // data to fill
            let mut vertices: Vec<Vertex> = Vec::with_capacity(num_vertices);
            let indices: Vec<u32> = mesh.indices.clone(); 

            let(p, n, t) = (&mesh.positions, &mesh.normals, &mesh.texcoords);
            for i in 0..num_vertices {
                vertices.push(
                    Vertex {
                        position: glm::vec3(p[i*3], p[i*3 + 1], p[i*3 + 2]),
                        normal: glm::vec3(n[i*3], n[i*3 + 1], n[i*3 + 2]),
                        tex_coords: glm::vec2(t[i*2], t[i*2 + 1]),
                        ..Vertex::default()
                    }
                )
            }

            // process material
            let mut textures = Vec::<Texture>::new();
            if let Some(material_id) = mesh.material_id {
                let material = &materials[material_id];

                // 1. diffuse map
                if !material.diffuse_texture.is_empty() {
                    let texture = self.load_material_texture(&material.diffuse_texture, "texture_diffuse");
                    textures.push(texture);
                }

                // 2. specular map
                if !material.specular_texture.is_empty() {
                    let texture = self.load_material_texture(&material.specular_texture, "texture_specular");
                    textures.push(texture);
                }

                // 3. normal map
                if !material.normal_texture.is_empty() {
                    let texture = self.load_material_texture(&material.normal_texture, "texture_normal");
                    textures.push(texture);
                }
                // NO height maps
            }
        
            self.meshes.push(Mesh::new(vertices, indices, textures));
        }

    }

    fn load_material_texture(&mut self, path: &str, type_name: &str) -> Texture {
        {
            let texture = self.textures_loaded.iter().find(|t| t.path == path);
            if let Some(texture) = texture {
                return texture.clone();
            }
        }

        let texture = Texture {
            id: unsafe { texture_from_file(path, &self.directory) },
            texture_type: type_name.into(),
            path: path.into()
        };

        self.textures_loaded.push(texture.clone());
        texture

    }
}

unsafe fn texture_from_file(path: &str, directory: &str) -> u32 {
    let file_name = format!("{}/{}", directory, path);

    let mut texture_id = 0;
    gl::GenTextures(1, &mut texture_id);

    let img = image::open(&Path::new(&file_name)).expect("Texture failed to load");
    let format = match img {
        ImageLuma8(_) => gl::RED,
        ImageLumaA8(_) => gl::RG,
        ImageRgb8(_) => gl::RGB,
        ImageRgba8(_) => gl::RGBA,
        ImageBgr8(_) => gl::BGR,
        ImageBgra8(_) => gl::BGRA,
        _ => gl::RED

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

