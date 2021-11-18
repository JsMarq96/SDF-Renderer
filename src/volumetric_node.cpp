#include "volumetric_node.h"

VolumetricNode::VolumetricNode() {
	material = new VolumetricMaterial();
	((VolumetricMaterial*) material)->setVolume(VOLUMES_DIR[volume_id]);

	mesh = new Mesh();
	mesh->createCube();

	name = "Volumetric Node";
}


void VolumetricNode::renderInMenu() {
	SceneNode::renderInMenu();
}


SDFQuadNode::SDFQuadNode() {
	mesh = new Mesh();
	mesh->createQuad(0.0f, 0.0f, 2.0f, 2.0f, false);

	material = new SDFMaterial();
}

void SDFQuadNode::renderInMenu() {}