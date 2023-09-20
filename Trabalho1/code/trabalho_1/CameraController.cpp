#include "CameraController.h"
#include <glm/gtc/matrix_transform.hpp>
#include <GLFW/glfw3.h>

CameraController* CameraController::m_inst = nullptr;
CameraController* CameraController::Inst()
{
	if (!m_inst)
		m_inst = new CameraController();

	return m_inst;
}

CameraController::CameraController()
{
	window = nullptr;
}

void CameraController::init(GLFWwindow* window)
{
	this->window = window;
	viewMatrix = glm::lookAt(
		vec3(0.0f, 0.0f, -1.0f), //eye
		vec3(0.0f, 0.0f, 0.0f), //center
		vec3(0.0f, 1.0f, 0.0f)); //up
}

void CameraController::processInput()
{
	if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
	{
		// Move the camera forward
		viewMatrix = glm::translate(viewMatrix, vec3(0.0f, 0.0f, -cameraSpeed));
	}
	if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
	{
		// Move the camera backward
		viewMatrix = glm::translate(viewMatrix, vec3(0.0f, 0.0f, cameraSpeed));
	}
	if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
	{
		// Move the camera left
		viewMatrix = glm::translate(viewMatrix, vec3(-cameraSpeed, 0.0f, 0.0f));
	}
	if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
	{
		// Move the camera right
		viewMatrix = glm::translate(viewMatrix, vec3(cameraSpeed, 0.0f, 0.0f));
	}
}

