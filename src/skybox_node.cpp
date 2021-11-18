#include "skybox_node.h"


SkyboxNode::SkyboxNode() {
	material = new SkyBoxMaterial();
	mesh = new Mesh();

	mesh->createCube();

	((SkyBoxMaterial*)material)->setCubemapTexture(SKYBOXES[0]);

	this->name = "Skybox";
}
SkyboxNode::~SkyboxNode() {
	delete material;
	delete mesh;
}

void SkyboxNode::render(Camera* camera) {
	// Move the box of the skybox to the current camera position
	model.setTranslation(camera->eye.x, camera->eye.y, camera->eye.z);

	if (material) {
		glDisable(GL_DEPTH_TEST);
		material->render(mesh, model, camera);
		glEnable(GL_DEPTH_TEST);
	}
}

void SkyboxNode::renderInMenu() {
	bool has_changed = false;
	has_changed |= ImGui::Combo("Select", &enviorment_id, "City\0Dragonvale\0Snow\0");

	if (has_changed) {
		((SkyBoxMaterial*)material)->setCubemapTexture(SKYBOXES[enviorment_id]);
	}
}