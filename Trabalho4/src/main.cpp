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
#include "Slider.h"
#include "MouseHandler.h"
#include "Fourier.h"
#include "Points.h"

using namespace std;

int screenWidth = 500, screenHeight = 500;

MouseHandler* mouseHandler = NULL;
Slider* slider = NULL;
Fourier* fourier = NULL;

void render()
{
    CV::clear(0, 0, 0);
    slider->Render();
    CV::translate(screenWidth / 3, screenHeight >> 1);
    fourier->Render();

    CV::color(1, 1, 1);
    CV::line(0, 0, points[0], points[1]);

    for (int i = 0; i < points.size(); i += 4)
    {
        CV::color(1, 1, 1);
        CV::line(points[i], points[i + 1], points[i + 2], points[i + 3]);
    }
}

void mouse(int button, int state, int wheel, int direction, int x, int y)
{
    mouseHandler->Update(button, state, wheel, direction, x, y);

    if (mouseHandler->GetState() == 0)
    {
        slider->OnMouseClick(mouseHandler->GetX(), mouseHandler->GetY());
    }
    else if (mouseHandler->IsDragging())
    {
        slider->OnMouseDrag(mouseHandler->GetX());
    }
    else if (mouseHandler->GetState() == 1)
    {
        slider->OnMouseRelease();
    }

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

   slider = new Slider(15, 15, 100, 15, 0.01, 1, 0.5);
   mouseHandler = new MouseHandler();
   fourier = new Fourier();

   CV::run();
}
