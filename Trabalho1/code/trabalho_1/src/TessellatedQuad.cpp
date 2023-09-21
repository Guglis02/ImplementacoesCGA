#include "TessellatedQuad.h"
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtx/transform.hpp>
#include <iostream>
#include "TextureManager.h"
#include "GLUtils.h"

using namespace std;

//add to glfwGetKey that gets the pressed key only once (not several times)
char keyOnce[GLFW_KEY_LAST + 1];
#define glfwGetKeyOnce(WINDOW, KEY)             \
    (glfwGetKey(WINDOW, KEY) ?              \
     (keyOnce[KEY] ? false : (keyOnce[KEY] = true)) :   \
     (keyOnce[KEY] = false))

TextureManager* texManager;

TessellatedQuad::TessellatedQuad(GLFWwindow* window, int size, int numberOfPatches)
{
	this->window = window;
	this->size = size;
	this->numberOfPatches = numberOfPatches;
	planePos = vec3(0.0f, 0.0f, 0.0f);
	vaoIDs = new GLuint[patches.size()];
}

void TessellatedQuad::init()
{
	cameraController = CameraController::Inst();
	cameraController->init(window);

	genTerrain();
	genBuffers();

	// init matrices
	modelMatrix = glm::mat4(1.0f);
	
	int w, h;
	glfwGetFramebufferSize(window, &w, &h);

	projectionMatrix = glm::perspective(glm::radians(75.0f), (float)w/(float)h, 0.1f, 100.0f);

	// load shaders
	try {
		shader.compileShader("shader/glsl40_tess_disp_mapp.vert", GLSLShader::VERTEX);
		shader.compileShader("shader/glsl40_tess_disp_mapp.tcs", GLSLShader::TESS_CONTROL);
		shader.compileShader("shader/glsl40_tess_disp_mapp.tes", GLSLShader::TESS_EVALUATION);
		shader.compileShader("shader/glsl40_tess_disp_mapp.frag", GLSLShader::FRAGMENT);

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

void TessellatedQuad::update(double t)
{
	processInput();
	cameraController->processInput();

	//// matrices setup
	modelMatrix = mat4(); // identity
	modelMatrix = glm::translate(modelMatrix, planePos); // translate back
	
	modelViewMatrix = cameraController->getViewMatrix() * modelMatrix;
	modelViewProjectionMatrix = projectionMatrix * modelViewMatrix;

	// set var MVP on the shader
	shader.setUniform("MVP", modelViewProjectionMatrix);
	shader.setUniform("TessLevel", tessLevel);
	shader.setUniform("displacementmapSampler", 1);
	shader.setUniform("colorTextureSampler", 0);
}

void TessellatedQuad::processInput()
{
	// Tessellation Level
	if (glfwGetKeyOnce(window, 'R'))
	{
		tessLevel+=10;
		if (tessLevel > 64)
			tessLevel = 64;
	}
	if (glfwGetKeyOnce(window, 'F'))
	{
		tessLevel-=10;
		if (tessLevel < 1)
			tessLevel = 1;
	}

	// toggle wireframe
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
	
	for (size_t i = 0; i < patches.size(); i++) {
		glBindVertexArray(vaoIDs[i]);
		glPatchParameteri(GL_PATCH_VERTICES, 4);
		glDrawElements(GL_PATCHES, patches[i].indices.size(), GL_UNSIGNED_INT, (GLubyte*)NULL);
		glBindVertexArray(0);
	}
}

void TessellatedQuad::genBuffers()
{
	glGenVertexArrays(patches.size(), vaoIDs);
			
	glGenBuffers(patches.size() * 3, bufferHandles);

	for (size_t i = 0; i < patches.size(); i++) {
		glBindVertexArray(vaoIDs[i]);

		// Vertex buffer
		glBindBuffer(GL_ARRAY_BUFFER, bufferHandles[i * 3]);
		glBufferData(GL_ARRAY_BUFFER, patches[i].vertices.size() * sizeof(vec3), (GLvoid*)&patches[i].vertices[0], GL_STATIC_DRAW);
		glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, (GLubyte*)NULL);
		glEnableVertexAttribArray(0);  // VertexPosition -> layout 0 in the VS

		// Texcoord buffer
		glBindBuffer(GL_ARRAY_BUFFER, bufferHandles[i * 3 + 1]);
		glBufferData(GL_ARRAY_BUFFER, patches[i].texcoord.size() * sizeof(vec2), (GLvoid*)&patches[i].texcoord[0], GL_STATIC_DRAW);
		glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 0, (GLubyte*)NULL);
		glEnableVertexAttribArray(1);  // TexCoord -> layout 1 in the VS

		// Index buffer
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, bufferHandles[i * 3 + 2]);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, patches[i].indices.size() * sizeof(int), (GLvoid*)&patches[i].indices[0], GL_STATIC_DRAW);

		glBindVertexArray(0);
	}
}


void TessellatedQuad::resize(int x, int y)
{

}

void TessellatedQuad::genTerrain()
{
	float patchSize = static_cast<float>(size) / static_cast<float>(numberOfPatches);

	// Generate terrain patches
	for (int i = 0; i < numberOfPatches; i++) {
		for (int j = 0; j < numberOfPatches; j++) {
			Patch patch;
			patch.genPatch(patchSize, i, j);
			patches.push_back(patch);
		}
	}
}


void TessellatedQuad::Patch::genPatch(float patchSize, int patchX, int patchY)
{
	// Calculate patch position
	float xOffset = patchX * patchSize;
	float yOffset = patchY * patchSize;
	planePos = vec3(xOffset, 0.0f, yOffset + 10);

	for (int i = 0; i < 4; i++) {
		float x = (i % 2) * patchSize;
		float y = (i / 2) * patchSize;
		vertices.push_back(vec3(x, 0.0f, y) + planePos);
		texcoord.push_back(vec2(x / (2.0f * patchSize), y / (2.0f * patchSize)));
	}
		
	indices.push_back(0);
	indices.push_back(1);
	indices.push_back(2);
	indices.push_back(3);
}
