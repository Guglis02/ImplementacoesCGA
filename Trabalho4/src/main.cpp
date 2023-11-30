/*
Trabalho 4 - Transformada de Fourier
CGA - 2023/2
Gustavo Machado de Freitas
*/

#include <GL/glut.h>
#include <GL/freeglut_ext.h> //callback da wheel do mouse.

#include <math.h>
#include <stdio.h>
#include <stdlib.h>

#include "gl_canvas2d.h"
#include "Fourier.h"
#include "ComplexFourier.h"
#include "Points.h"

using namespace std;

int screenWidth = 1500, screenHeight = 800;

Fourier* fourier = NULL;
ComplexFourier* complexFourier = NULL;

void render()
{
    CV::clear(0, 0, 0);
    fourier->Render(screenWidth, screenHeight);
    complexFourier->Render(screenWidth, screenHeight);
}

void mouse(int button, int state, int wheel, int direction, int x, int y)
{
    //printf("\nmouse %d %d %d %d %d %d", button, state, wheel, direction,  x, y);
}

//funcao chamada toda vez que uma tecla for pressionada.
void keyboard(int key)
{
   printf("\nTecla: %d" , key);
}

//funcao chamada toda vez que uma tecla for liberada
void keyboardUp(int key)
{
   printf("\nLiberou: %d" , key);
}

int main(void)
{
    CV::init(&screenWidth, &screenHeight, "Trabalho 4 - Fourier");

    //vector<double> points = generateSpiralPoints(); // Espiral
    //vector<double> points = generateGearPoints(); // Engrenagem
    //vector<double> points = getCodingTrainPoints(); // Trenzinho do Coding Train
    //vector<double> points = generateWheelPoints(); // Roda de bicicleta
    vector<double> points = generateSquarePoints(10); // Quadrado

    fourier = new Fourier();
    fourier->Setup(points);
    complexFourier = new ComplexFourier();
    complexFourier->Setup(points);

    CV::run();
}
