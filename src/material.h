#ifndef MATERIAL_H
#define MATERIAL_H

#include "framework.h"
#include "shader.h"
#include "camera.h"
#include "mesh.h"
#include "extra/hdre.h"
#include "volume.h"

#include "scene_data.h"

extern sSceneData scene_data;

class Material {
public:

	Shader* shader = NULL;
	Texture* texture = NULL;
	vec4 color;

	virtual void setUniforms(Camera* camera, Matrix44 model) = 0;
	virtual void render(Mesh* mesh, Matrix44 model, Camera * camera) = 0;
	virtual void renderInMenu() = 0;
};

class StandardMaterial : public Material {
public:

	StandardMaterial();
	~StandardMaterial();

	void setUniforms(Camera* camera, Matrix44 model);
	void render(Mesh* mesh, Matrix44 model, Camera * camera);
	void renderInMenu();
};

class WireframeMaterial : public StandardMaterial {
public:

	WireframeMaterial();
	~WireframeMaterial();

	void render(Mesh* mesh, Matrix44 model, Camera * camera);
};


// =================
// CUSTOM MATERIALS
// =================

class TexturedMaterial : public StandardMaterial {
public:
	TexturedMaterial(const char* texture_name);
	~TexturedMaterial();

	void renderInMenu();
};

class PhongMaterial : public StandardMaterial {
public:
	float ambient_value = 0.5f;
	float diffuse_value = 0.6f;
	float specular_value = 0.5f;
	float shiniess = 32.0f;

	Vector4 light_color = {1.0f, 1.0f, 1.0f, 1.0f};
	Vector3 light_position = {-14.0f, -14.0f, 14.0f};

	PhongMaterial();
	~PhongMaterial();

	void setUniforms(Camera* camera, Matrix44 model);
	void renderInMenu();
};

class SkyBoxMaterial : public StandardMaterial {
public:
	SkyBoxMaterial();
	~SkyBoxMaterial();

	void setCubemapTexture(const char* texture);

	void renderInMenu();
};

class ReflectiveMaterial : public StandardMaterial {
public:
	float reflectiveness = 0.6f;

	ReflectiveMaterial();

	void setUniforms(Camera* camera, Matrix44 model);
	void renderInMenu();
};


class VolumetricMaterial : public StandardMaterial {
public:
	Texture *noise_tex = NULL;
	float brightness = 1.0f;
	float ray_step_size = 0.06f;

	float isosurf_threhold = 0.5f;
	float gradient_delta = 0.001f;

	VolumetricMaterial();

	void setVolume(const char* vol_dir);

	void setUniforms(Camera* camera, Matrix44 model);

	void renderInMenu();
};


class SDFMaterial : public StandardMaterial {
public:
	Texture* noise_func;

	SDFMaterial();

	float cam_rotation = 10.0f;

	void setUniforms(Camera* camera, Matrix44 model);

	void renderInMenu();
};
#endif