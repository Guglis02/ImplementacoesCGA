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
	/// Classe respons�vel por gerar e controlar intera��es com o terreno.
	/// </summary>
	/// <param name="window">Janela da aplica��o</param>
	/// <param name="patchSize">Tamanho de cada patch</param>
	/// <param name="patchAmount">Quantidade de linhas/colunas de patches 
	/// (na pr�tica, o terreno ter� este n�mero elevado ao quadrado de patches)</param>
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
	/// Gera os buffers de v�rtices e �ndices.
	/// </summary>
	void genBuffers();

	/// <summary>
	/// Trata as intera��es com o terreno.
	/// </summary>
	void processInput();

	/// <summary>
	/// Calcula a posi��o da luz em fun��o do �ngulo, tomando como centro de rota��o o centro do plano.
	/// </summary>
	vec3 calculateLightPos(float ang);

	/// <summary>
	/// Atualiza a posi��o das fontes de luz no shader.
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