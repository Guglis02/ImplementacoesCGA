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

	glm::mat4 getViewMatrix();
private:
	float const cameraSpeed = 3.0f;
	float const mouseSensitivity = 0.5f;
	vec3 const cameraUp = vec3(0.0f, 1.0f, 0.0f);

	vec3 cameraPos;
	vec3 cameraDir;

	static CameraController* m_inst;

	double lastX;
	double lastY;
	float yaw;
	float pitch;

	GLFWwindow* window;

	void processMouse();
	void processKeyboard();
};

