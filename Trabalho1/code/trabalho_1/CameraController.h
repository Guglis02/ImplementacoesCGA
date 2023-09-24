#pragma once
#include <GL/glew.h>

#include <GLFW\glfw3.h>
#include "Scene.h"
#include <vector>
#include "glslprogram.h"

/// <summary>
/// Classe responsável por controlar a câmera.
/// </summary>
class CameraController
{
public:
	/// <summary>
	/// Singleton.
	/// </summary>
	static CameraController* Inst();
	CameraController();

	/// <summary>
	/// Inicializa variáveis da camera.
	/// </summary>
	void init(GLFWwindow* window);

	/// <summary>
	/// Processa input do teclado e mouse.
	/// </summary>
	void processInput();

	glm::mat4 getViewMatrix();
	glm::vec3 getCameraPos();
private:
	float const cameraSpeed = 3.0f;
	float const mouseSensitivity = 0.5f;
	vec3 const cameraUp = vec3(0.0f, -1.0f, 0.0f);

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

