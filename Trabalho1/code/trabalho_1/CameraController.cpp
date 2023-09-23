#include "CameraController.h"
#include <glm/gtc/matrix_transform.hpp>
#include <GLFW/glfw3.h>
#include <iostream>
#include <FpsController.h>

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
    lastX = 0.0f;
    lastY = 0.0f;
    yaw = 0.0f;
    pitch = 0.0f;
}

void CameraController::init(GLFWwindow* window)
{
	this->window = window;
    this->cameraPos = vec3(0.0f, 5.0f, 0.0f);
    this->cameraDir = vec3(1.0f, 0.0f, 0.0f);
}

glm::mat4 CameraController::getViewMatrix()
{
    //std::cout << "Camera pos: " << cameraPos.x << " " << cameraPos.y << " " << cameraPos.z << std::endl;
    //std::cout << "Camera dir: " << cameraDir.x << " " << cameraDir.y << " " << cameraDir.z << std::endl;
    return glm::lookAt(
        cameraPos,
        cameraPos + cameraDir,
        cameraUp);
}

glm::vec3 CameraController::getCameraPos()
{
    return cameraPos;
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
        float xOffset = mouseX - lastX;
        float yOffset = lastY - mouseY;

        xOffset *= mouseSensitivity;
        yOffset *= mouseSensitivity;

        yaw += xOffset;
        pitch += yOffset;

        cameraDir.x = cos(glm::radians(yaw)) * cos(glm::radians(pitch));
        cameraDir.y = sin(glm::radians(pitch));
        cameraDir.z = sin(glm::radians(yaw)) * cos(glm::radians(pitch));
        cameraDir = glm::normalize(cameraDir);
    }

    lastX = mouseX;
    lastY = mouseY;
}

void CameraController::processKeyboard()
{
    float normSpeed = FpsController::getInstance().normalize(cameraSpeed);

    if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
    {
        cameraPos.x += normSpeed * cameraDir.x;
        cameraPos.z += normSpeed * cameraDir.z;
    }
    if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
    {
        cameraPos.x -= normSpeed * cameraDir.x;
        cameraPos.z -= normSpeed * cameraDir.z;
    }
    if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
    {
        cameraPos -= normSpeed * glm::normalize(glm::cross(cameraDir, cameraUp));
    }
    if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
    {
        cameraPos += normSpeed * glm::normalize(glm::cross(cameraDir, cameraUp));
    }
}


