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
	/// <summary>
	/// Classe responsável por gerar e controlar interações com o terreno.
	/// </summary>
	/// <param name="window">Janela da aplicação</param>
	/// <param name="patchSize">Tamanho de cada patch</param>
	/// <param name="patchAmount">Quantidade de linhas/colunas de patches 
	/// (na prática, o terreno terá este número elevado ao quadrado de patches)</param>
	TessellatedQuad(GLFWwindow* window, int patchSize = 1, int patchAmount = 3);

	// Mesh virtual functions
	void init();
	void update(double t);
	void render();
	void resize(int, int);
private:
	/// <summary>
	/// Gera os patches do terreno.
	/// </summary>
	void genPlane();

	/// <summary>
	/// Gera os buffers de vértices e índices.
	/// </summary>
	void genBuffers();

	/// <summary>
	/// Trata as interações com o terreno.
	/// </summary>
	void processInput();

	/// <summary>
	/// Calcula a posição da luz em função do ângulo, tomando como centro de rotação o centro do plano.
	/// </summary>
	vec3 calculateLightPos(float ang);

	/// <summary>
	/// Atualiza a posição das fontes de luz no shader.
	/// </summary>
	void updateLight();

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
	vec3 planeCenter;
	bool wireframe = false;
	bool tessellateMiddle = false;
	bool tessellateByDistance = false;

	int maxTessLevel = 5;
	int cameraRange;
};