#pragma once
#include <GL/glew.h>

#include <GLFW\glfw3.h>
#include "Scene.h"
#include <vector>
#include "glslprogram.h"

class CameraController
{
public:
	static CameraController* Inst();
	CameraController();
	
	void processInput();
	void init(GLFWwindow* window);

	glm::mat4 getViewMatrix() { return viewMatrix; }
private:
	float const cameraSpeed = 0.05f;
	static CameraController* m_inst;
	
	GLFWwindow* window;
	glm::mat4 viewMatrix;
};

