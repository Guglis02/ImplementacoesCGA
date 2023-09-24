// Trabalho 1 – Shader
// 
// Programa em C++, utilizando a API OpenGL 4.x, para simular um terreno 
// formado por N² patches, dispostos em um grid NxN. Desenvolvido usando
// como base a demo glsl40_tessellation_displacement_mapping.
// 
// Use WASD para mover a camera pelo mapa e mouse para rotacionar a camera.
// Use R/F para aumentar/diminuir o tessellation level máximo.
// Use E para ativar/desativar wireframe.
//
// Setembro 2023 - Gustavo Machado de Freitas - gmfreitas@inf.ufsm.br

//Include GLEW - always first 
#include "GL/glew.h"
#include <GLFW/glfw3.h>

//Include the standard C++ headers 
#include "GLUtils.h"
#include "Scene.h"
#include <cstdlib>
#include <cstdio>
#include <string>
#include <iostream>
#include "TessellatedQuad.h"
#include "FpsController.h"

#define WINDOW_WIDTH	1000
#define WINDOW_HEIGHT	1000

using namespace std;

Scene* tessellatedQuad;
GLFWwindow* window;

void mainLoop()
{
	do
	{
		// Check for OpenGL errors
		GLUtils::checkForOpenGLError(__FILE__, __LINE__);

		FpsController::getInstance().update();
		tessellatedQuad->update(FpsController::getInstance().GetDeltaTime());

		tessellatedQuad->render();

		glfwSwapBuffers(window);
		//Get and organize events, like keyboard and mouse input, window resizing, etc...  
		glfwPollEvents();
	} while (!glfwWindowShouldClose(window));
}

// Define an error callback  
void errorCallback(int error, const char* description)
{
	fputs(description, stderr);
	_fgetchar();
}

// Key callback
void keyCallback(GLFWwindow* window, int key, int scancode, int action, int mods)
{
	if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
		glfwSetWindowShouldClose(window, GL_TRUE);
}

// Initialize GLFW  
void initGLFW()
{
	cout << "Initializing GLFW..." << endl;
	if (!glfwInit())
	{
		exit(EXIT_FAILURE);
	}

	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 0);
	glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_FALSE);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);
	glfwWindowHint(GLFW_OPENGL_DEBUG_CONTEXT, GL_TRUE);

	window = glfwCreateWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "GLSL4.0 - Tessellation Displacement Mapping", NULL, NULL);
	if (!window)
	{
		fprintf(stderr, "Failed to open GLFW window.\n");
		glfwTerminate();
		system("pause");
		exit(EXIT_FAILURE);
	}
}

void initCallbacks()
{
	glfwMakeContextCurrent(window);
	glfwSetKeyCallback(window, keyCallback);
	glfwSetErrorCallback(errorCallback);
}

void initGLEW()
{
	cout << "Initializing GLEW..." << endl;
	// Initialize GLEW
	glewExperimental = GL_TRUE; //ensures that all extensions with valid entry points will be exposed.
	GLenum err = glewInit();
	if (err != GLEW_OK)
	{
		fprintf(stderr, "Error: %s\n", glewGetErrorString(err));
		system("pause");
		exit(EXIT_FAILURE);
	}
	GLUtils::checkForOpenGLError(__FILE__, __LINE__); // Will throw error. Just ignore, glew bug.
	GLUtils::dumpGLInfo();
}

void initializeGL()
{
	cout << "Initializing GL..." << endl;
	glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
	glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
	GLUtils::checkForOpenGLError(__FILE__, __LINE__);
}

// Close OpenGL window and terminate GLFW  
void closeApplication()
{
	glfwDestroyWindow(window);
	glfwTerminate();
}

int main(void)
{
	initGLFW();
	initCallbacks();
	initGLEW();
	initializeGL();

	tessellatedQuad = new TessellatedQuad(window, 5, 9);
	tessellatedQuad->init();

	cout << endl << "WASD: Move camera pelo mapa" << endl;
	cout << "Mouse Click: Rotaciona camera" << endl;
	cout << "R/F: Aumenta/Diminui tessellation level maximo" << endl;
	cout << "E: Ativa/Desativa wireframe" << endl;

	mainLoop();

	exit(EXIT_SUCCESS);
}