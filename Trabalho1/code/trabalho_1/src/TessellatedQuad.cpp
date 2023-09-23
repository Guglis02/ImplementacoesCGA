#include "TessellatedQuad.h"
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtx/transform.hpp>
#include <iostream>
#include "TextureManager.h"
#include "GLUtils.h"
#include <FpsController.h>

using namespace std;

//add to glfwGetKey that gets the pressed key only once (not several times)
char keyOnce[GLFW_KEY_LAST + 1];
#define glfwGetKeyOnce(WINDOW, KEY)             \
    (glfwGetKey(WINDOW, KEY) ?              \
     (keyOnce[KEY] ? false : (keyOnce[KEY] = true)) :   \
     (keyOnce[KEY] = false))

TextureManager* texManager;

TessellatedQuad::TessellatedQuad(GLFWwindow* window, int size, int patchAmount)
{
	this->window = window;
	this->size = size;
	this->cameraRange = size * 4;
	this->patchAmount = patchAmount;
	planePos = vec3(0.0f, 0.0f, 0.0f);
}

void TessellatedQuad::init()
{
	cameraController = CameraController::Inst();
	cameraController->init(window);

	genPlane();
	genBuffers();

	// init matrices
	modelMatrix = glm::mat4(1.0f);
	
	int w, h;
	glfwGetFramebufferSize(window, &w, &h);

	projectionMatrix = glm::perspective(glm::radians(75.0f), (float)w/(float)h, 0.1f, 100.0f);

	// load shaders
	try {
		shader.compileShader("shader/trabalho_1.vert", GLSLShader::VERTEX);
		shader.compileShader("shader/trabalho_1.tcs", GLSLShader::TESS_CONTROL);
		shader.compileShader("shader/trabalho_1.tes", GLSLShader::TESS_EVALUATION);
		shader.compileShader("shader/trabalho_1.frag", GLSLShader::FRAGMENT);
		shader.compileShader("shader/trabalho_1.geom", GLSLShader::GEOMETRY);

		shader.link();
		shader.use();
	}
	catch (GLSLProgramException &e) {
		cerr << e.what() << endl;
		system("pause");
		exit(EXIT_FAILURE);
	}
	shader.printActiveAttribs();
	
	// Get a TextureManager's instance
	texManager = TextureManager::Inst();

	// Load our color texture with Id 0
	glActiveTexture(GL_TEXTURE0);
	//if (!texManager->LoadTexture("..\\..\\resources\\old_bricks_sharp_diff_COLOR.png", 0))
	if (!texManager->LoadTexture("..\\..\\resources\\inter_color.png", 0))
		cout << "Failed to load texture." << endl;
	
	// Load our displacement texture with Id 1
	glActiveTexture(GL_TEXTURE1);	
	//if (!texManager->LoadTexture("..\\..\\resources\\old_bricks_sharp_diff_DISP.png", 1))
	if (!texManager->LoadTexture("..\\..\\resources\\inter_disp.png", 1))
		cout << "Failed to load texture." << endl;
}

float ang = 0;

void TessellatedQuad::updateLight()
{
	float x, z;
	x = size * patchAmount * cos(ang);
	z = size * patchAmount * sin(ang);

	//shader.setUniform("LightPosition1", vec3(x, 1.0f, z));
	shader.setUniform("LightPosition1", cameraController->getCameraPos());

	shader.setUniform("LightDir", normalize(vec3(x, 1.0f, z) - vec3(0, 0, 0)));
	ang += FpsController::getInstance().normalize(0.01);
}

void TessellatedQuad::update(double t)
{
	processInput();
	cameraController->processInput();
	updateLight();

	//// matrices setup
	modelMatrix = mat4(); // identity
	modelMatrix = glm::translate(modelMatrix, planePos); // translate back
	
	modelViewMatrix = cameraController->getViewMatrix() * modelMatrix;
	modelViewProjectionMatrix = projectionMatrix * modelViewMatrix;

	// set var MVP on the shader
	shader.setUniform("MVP", modelViewProjectionMatrix); //ModelViewProjection

	shader.setUniform("MAX_DISTANCE", cameraRange);
	shader.setUniform("MAX_TESS_LEVEL", tessLevel);
	shader.setUniform("CameraPosition", cameraController->getCameraPos());

	shader.setUniform("displacementmapSampler", 1);
	shader.setUniform("colorTextureSampler", 0);
}

void TessellatedQuad::processInput()
{
	// Max Tessellation Level
	if (glfwGetKeyOnce(window, 'R'))
	{
		tessLevel+=10;
		//if (tessLevel > 64)
		//	tessLevel = 64;
	}
	if (glfwGetKeyOnce(window, 'F'))
	{
		tessLevel-=10;
		if (tessLevel < 1)
			tessLevel = 1;
	}

	// Ativa/Desativa Wireframe
	if (glfwGetKeyOnce(window, 'E')) {
		wireframe = !wireframe;
		if (wireframe) {
			glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
		}
		else {
			glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
		}
	}
}

void TessellatedQuad::render()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	
	//// Bind both textures
	//// Color
	glActiveTexture(GL_TEXTURE0);
	TextureManager::Inst()->BindTexture(0);
	//// Displacement
	glActiveTexture(GL_TEXTURE1);
	TextureManager::Inst()->BindTexture(1);
	
	glBindVertexArray(vaoID);
	glPatchParameteri(GL_PATCH_VERTICES, 4);
	glDrawElements(GL_PATCHES, indices.size(), GL_UNSIGNED_INT, (GLubyte *)NULL);
	glBindVertexArray(0);
}

void TessellatedQuad::genBuffers()
{
	glGenVertexArrays(1, &vaoID);
	glBindVertexArray(vaoID);

	unsigned int handle[3];
	glGenBuffers(3, handle);

	glBindBuffer(GL_ARRAY_BUFFER, handle[0]);
	glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(vec3), (GLvoid*)&vertices[0], GL_STATIC_DRAW);
	glVertexAttribPointer((GLuint)0, 3, GL_FLOAT, GL_FALSE, 0, (GLubyte *)NULL);
	glEnableVertexAttribArray(0);  // VertexPosition -> layout 0 in the VS

	glBindBuffer(GL_ARRAY_BUFFER, handle[1]);
	glBufferData(GL_ARRAY_BUFFER, texcoord.size() * sizeof(vec2), (GLvoid*)&texcoord[0], GL_STATIC_DRAW);
	glVertexAttribPointer((GLuint)1, 2, GL_FLOAT, GL_FALSE, 0, (GLubyte *)NULL);
	glEnableVertexAttribArray(1);  // TexCoord -> layout 1 in the VS

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, handle[2]);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(int), (GLvoid*)&indices[0], GL_STATIC_DRAW);

	glBindVertexArray(0);
}

void TessellatedQuad::resize(int x, int y)
{

}

void TessellatedQuad::genPlane()
{
	for (int i = 0; i < patchAmount; i++)
	{
		for (int j = 0; j < patchAmount; j++)
		{
			float texSlice = 1.0f / patchAmount;

			// Inferior esquerdo
			vertices.push_back(vec3(i * size, 0.0f, j * size));
			texcoord.push_back(vec2(i * texSlice, j * texSlice));

			// Superior esquerdo
			vertices.push_back(vec3((i + 1) * size, 0.0f, j * size));
			texcoord.push_back(vec2((i + 1) * texSlice, j * texSlice));

			// Superior direito
			vertices.push_back(vec3((i + 1) * size, 0.0f, (j + 1) * size));
			texcoord.push_back(vec2((i + 1) * texSlice, (j + 1) * texSlice));

			// Inferior direito
			vertices.push_back(vec3(i * size, 0.0f, (j + 1) * size));
			texcoord.push_back(vec2(i * texSlice, (j + 1) * texSlice));
		}
	}
	
	for (int i = 0; i < patchAmount * patchAmount; i++)
	{
		indices.push_back(i * 4);
		indices.push_back(i * 4 + 1);
		indices.push_back(i * 4 + 2);
		indices.push_back(i * 4 + 3);
	}
}
