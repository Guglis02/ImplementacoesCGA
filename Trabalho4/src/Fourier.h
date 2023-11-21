#ifndef FOURIER_H_INCLUDED
#define FOURIER_H_INCLUDED

#include "./gl_canvas2d.h"
#include <math.h>
#include <vector>

using namespace std;

/** \brief
Classe responsavel pela logica das transformacoes de fourier.
*/
class Fourier
{
public:
    Fourier()
    {
    }

    void Render(float xOrigin, float yOrigin, float numberOfEpicycles)
    {
        int timeSinceStart = glutGet(GLUT_ELAPSED_TIME);
        int deltaTime = timeSinceStart - oldTimeSinceStart;
        oldTimeSinceStart = timeSinceStart;

        CV::translate(xOrigin, yOrigin);

        x = 0;
        y = 0;

        for (int i = 0; i < numberOfEpicycles; i++)
        {
            oldX = x;
            oldY = y;

            n = i * 2 + 1;
            radius = 75.0 * (4.0 / (n * PI));

            x += radius * cos(n * time);
            y += radius * sin(n * time);

            CV::color(0.5, 0.5, 0.5);
            CV::circle(oldX, oldY, radius, 64);
            CV::circleFill(x, y, 8, 16);

            CV::color(1, 1, 1);
            CV::line(oldX, oldY, x, y);
        }
        wave.insert(wave.begin(), y);

        CV::translate(xOrigin + 150, yOrigin);
        CV::line(x - 150, y, 0, wave[0]);

        for (int i = 0; i < wave.size(); i++)
        {
            CV::point(i * 0.1, wave[i]);
        }

        time += deltaTime * 0.001f;

        if (wave.size() > 5000)
        {
            wave.pop_back();
        }
    }
private:
    float time = 0;
    int oldTimeSinceStart = 0;
    vector<float> wave;
    float x, y, oldX, oldY, n, radius;
};

#endif // FOURIER_H_INCLUDED
