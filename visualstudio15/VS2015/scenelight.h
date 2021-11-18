#ifndef SCENE_LIGHT_H
#define SCENE_LIGHT_H

#include "framework.h"

struct sSceneLight {
	Vector3 position;
	Vector4 color = { 1.0f, 1.0f, 1.0f, 1.0f };
	Vector3 diffuse = { 1.0f, 1.0f, 1.0f };
	Vector3 specular = { 1.0f, 1.0f, 1.0f };
	Vector3 ambient = { 0.25f, 0.25f, 0.25f };
};

sSceneLight curr_scene_light;
#endif