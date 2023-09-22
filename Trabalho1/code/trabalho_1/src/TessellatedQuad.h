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
	TessellatedQuad(GLFWwindow* window, int patchSize = 1, int patchAmount = 3);

	// mesh virtual functions
	void init();
	void update(double t);
	void render();
	void resize(int, int);

private:
	void genPlane();
	void genBuffers();
	void processInput();

	CameraController* cameraController;

	GLuint vaoID;
	int size;
	int patchAmount;
	std::vector<vec3> vertices;
	std::vector<vec2> texcoord;
	std::vector<unsigned int> indices;

	GLSLProgram shader;
	GLFWwindow* window;

	glm::mat4 modelMatrix;
	glm::mat4 projectionMatrix;
	glm::mat4 modelViewProjectionMatrix;
	glm::mat4 modelViewMatrix;

	vec3 planePos;
	bool wireframe = false;

	int tessLevel = 1;
	int cameraRange;
};