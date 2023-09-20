#include "CameraController.h"
#include <glm/gtc/matrix_transform.hpp>
#include <GLFW/glfw3.h>
#include <iostream>

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
    this->cameraPos = vec3(0.0f, 2.0f, 0.0f);
    this->front = vec3(1.0f, 0.0f, 0.0f);

    lastX = 0.0f;
    lastY = 0.0f;
    yaw = 0.0f;
    pitch = 0.0f;
}

glm::mat4 CameraController::getViewMatrix()
{
    std::cout << "Camera pos: " << cameraPos.x << " " << cameraPos.y << " " << cameraPos.z << std::endl;
    std::cout << "Camera front towards: " << front.x << " " << front.y << " " << front.z << std::endl;
    return glm::lookAt(
        cameraPos,
        cameraPos + front,
        cameraUp);
}

void CameraController::processInput()
{
    processKeyboard();
    processMouse();
}

void CameraController::processMouse()
{

    double mouseX, mouseY;
    glfwGetCursorPos(window, &mouseX, &mouseY);

    if (glfwGetMouseButton(window, 0) == GLFW_PRESS)
    {
        float xOffset = static_cast<float>(mouseX) - lastX;
        float yOffset = lastY - static_cast<float>(mouseY);

        lastX = static_cast<float>(mouseX);
        lastY = static_cast<float>(mouseY);

        xOffset *= mouseSensitivity;
        yOffset *= mouseSensitivity;

        yaw += xOffset;
        pitch += yOffset;

        //yaw = glm::clamp(yaw, -179.0f, 179.0f);
        //pitch = glm::clamp(pitch, -90.0f, 90.0f);

        front.x = cos(glm::radians(yaw)) * cos(glm::radians(pitch));
        front.y = sin(glm::radians(pitch));
        front.z = sin(glm::radians(yaw)) * cos(glm::radians(pitch));
        front = glm::normalize(front);
    }
    else
    {
        lastX = static_cast<float>(mouseX);
        lastY = static_cast<float>(mouseY);
    }
}

void CameraController::processKeyboard()
{
    if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
    {
        cameraPos.z += cameraSpeed;
    }
    if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
    {
        cameraPos.z -= cameraSpeed;
    }
    if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
    {
        cameraPos.x -= cameraSpeed;
    }
    if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
    {
        cameraPos.x += cameraSpeed;
    }
}


