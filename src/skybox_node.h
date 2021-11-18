#pragma once
#include "scenenode.h"
#include "material.h"
#include "texture.h"

static const char* SKYBOXES[3] = { 
	"data/environments/city",
	"data/environments/dragonvale",
	"data/environments/snow"
};

class SkyboxNode : public SceneNode {
public:	
	int enviorment_id = 0;

	SkyboxNode();
	~SkyboxNode();
	
	void render(Camera* camera);
	void renderInMenu();
};

