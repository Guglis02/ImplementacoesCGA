#pragma once

#include <GL/glew.h>

#include <GLFW\glfw3.h>
#include "Scene.h"
#include <vector>
#include "glslprogram.h"
#include "../CameraController.h"

class TessellatedQuad : public Scene
{
public:
	TessellatedQuad(GLFWwindow* window, int size = 1, int numberOfPatches = 9);

	// mesh virtual functions
	void init();
	void update(double t);
	void render();
	void resize(int, int);

private:
	struct TessellatedQuad::Patch {
		std::vector<vec3> vertices;
		std::vector<vec2> texcoord;
		std::vector<int> indices;
		vec3 planePos;

		void genPatch(float patchSize, int patchX, int patchY);
	};

	void genTerrain();
	void genBuffers();
	void processInput();

	CameraController* cameraController;

	GLuint bufferHandles[9 * 3];
	GLuint* vaoIDs;
	int size;
	int numberOfPatches;

	vec3 planePos;
	std::vector<Patch> patches;

	GLSLProgram shader;
	GLFWwindow* window;

	glm::mat4 modelMatrix;
	glm::mat4 projectionMatrix;
	glm::mat4 modelViewProjectionMatrix;
	glm::mat4 modelViewMatrix;

	bool wireframe = false;

	int tessLevel = 1;
};