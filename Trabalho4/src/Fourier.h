#ifndef FOURIER_H_INCLUDED
#define FOURIER_H_INCLUDED

#include "./gl_canvas2d.h"
#include <math.h>
#include <vector>

#include "Points.h"

using namespace std;

/** \brief
Classe responsavel pela logica das transformacoes de fourier.
*/
class Fourier
{
public:
    Fourier()
    {
        for (int i = 0; i < points.size() - 1; i += 8)
        {
            xValues.push_back(points[i]);
            yValues.push_back(points[i + 1]);
        }

        fourierX = discreteFourierTransform(xValues);
        fourierY = discreteFourierTransform(yValues);

        sortFourierVectors();
    }

    void Render(int screenWidth, int screenHeight)
    {
        Vector2 vx = epiCycles(screenWidth >> 1, 100, 0, fourierX);
        Vector2 vy = epiCycles(300, screenHeight >> 1, PI * 0.5, fourierY);
        Vector2 v = Vector2(vx.x, vy.y);

        wave.insert(wave.begin(), v);
        CV::line(vx.x, vx.y, v.x, v.y);
        CV::line(vy.x, vy.y, v.x, v.y);

        CV::color(1, 1, 1);
        for (int i = 0; i < wave.size() - 1; i++)
        {
            CV::line(wave[i].x, wave[i].y, wave[i + 1].x, wave[i + 1].y);
        }

        time += PI_2 / fourierY.size();

        if (time > PI * 2)
        {
            time = 0;
            wave.clear();
        }
        
        // Demonstração do desenho
        Vector2 offset = Vector2((screenWidth >> 1) + 500, screenHeight >> 1);

        for (int i = 0; i < xValues.size() - 1; i += 1)
        {
            CV::color(1, 1, 1);
            CV::line(xValues[i] + offset.x,
                    yValues[i] + offset.y,
                    xValues[i + 1] + offset.x,
                    yValues[i + 1] + offset.y);
        }
    }
private:
    float time = 0;
    int oldTimeSinceStart = 0;
    vector<Vector2> wave;
    vector<double> xValues;
    vector<double> yValues;
    float x, y, oldX, oldY, n, radius;

    struct WavePoint
    {
        double re;
        double im;
        double freq;
        double amp;
        double phase;

        WavePoint(double re, double im, double freq, double amp, double phase)
        {
            this->re = re;
            this->im = im;
            this->freq = freq;
            this->amp = amp;
            this->phase = phase;
        }
    };

    vector<WavePoint> fourierX;
    vector<WavePoint> fourierY;

    vector<WavePoint> discreteFourierTransform(vector<double> v)
    {
        vector<WavePoint> X;
        int N = v.size();

        for (int k = 0; k < N; k++)
        {
            double re = 0;
            double im = 0;

            for (int n = 0; n < N; n++)
            {
                double phi = (PI_2 * k * n) / N;
                re += v[n] * cos(phi);
                im -= v[n] * sin(phi);
            }

            re /= N;
            im /= N;

            double freq = k;
            double amp = sqrt(re * re + im * im);
            double phase = atan2(im, re);

            WavePoint wavePoint = WavePoint(re, im, freq, amp, phase);

            X.push_back(wavePoint);
        }

        return X;
    }

    Vector2 epiCycles(double x, double y, double rotation, vector<WavePoint> fourier)
    {
        for (int i = 0; i < fourier.size(); i++)
        {
            double oldX = x;
            double oldY = y;

            double freq = fourier[i].freq;
            double radius = fourier[i].amp;
            double phase = fourier[i].phase;

            x += radius * cos(freq * time + phase + rotation);
            y += radius * sin(freq * time + phase + rotation);

            CV::color(0.5, 0.5, 0.5);
            CV::circle(oldX, oldY, radius, 64);
            CV::circleFill(x, y, 8, 16);

            CV::color(1, 1, 1);
            CV::line(oldX, oldY, x, y);
        }

        return Vector2(x, y);
    }

    void sortFourierVectors() {
        std::sort(fourierX.begin(), fourierX.end(), [](const WavePoint& a, const WavePoint& b) {
            return b.amp < a.amp;
        });

        std::sort(fourierY.begin(), fourierY.end(), [](const WavePoint& a, const WavePoint& b) {
            return b.amp < a.amp;
        });
    }
};

#endif // FOURIER_H_INCLUDED