#ifndef VOLNODE_H
#define VOLNODE_H

#include "material.h"
#include "volume.h"
#include "scenenode.h"

static const char* VOLUMES_DIR[3] = {
	"data/volumes/bonsai_16_16.png",
	"data/volumes/teapot_16_16.png",
	"data/volumes/foot_16_16.png"
};

static const char* VOLUMES_NAME[3] = {
	"BONSAI",
	"TEAPOT",
	"FOOT"
};

class VolumetricNode : public SceneNode {
public:
	int volume_id = 1;

	VolumetricNode();

	void renderInMenu();
	
};

#endif