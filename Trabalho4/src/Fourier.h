#ifndef FOURIER_H_INCLUDED
#define FOURIER_H_INCLUDED

#include "./gl_canvas2d.h"
#include <math.h>
#include <vector>
#include <algorithm>

using namespace std;

/** \brief
Classe responsavel pela logica das transformacoes de fourier.
*/
class Fourier
{
public:
    Fourier(){}

    void Setup(vector<double> points)
    {
        size_t size = points.size();
        int step = size / (size * 0.5);

        for (int i = 0; i < points.size() - 1; i += step)
        {
            xValues.push_back(points[i]);
            yValues.push_back(points[i + 1]);
        }

        fourierX = discreteFourierTransform(xValues);
        fourierY = discreteFourierTransform(yValues);

        xRestoredValues = invertedDisceteFourierTransform(fourierX);
        yRestoredValues = invertedDisceteFourierTransform(fourierY);

        sortFourierVectors();
    }

    void Render(int screenWidth, int screenHeight)
    {
        Vector2 vx = epiCycles(400, 100, 0, fourierX);
        Vector2 vy = epiCycles(100, screenHeight >> 1, PI * 0.5, fourierY);
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
        
        // Sinal original
        Vector2 offset = Vector2((screenWidth >> 1) + 500, 100);
        drawSignal(xValues, yValues, offset);

        // Sinal reconstruído
        offset = Vector2((screenWidth >> 1) + 500, screenHeight >> 1);
        drawSignal(xValues, yValues, offset);        
    }
protected:
    float time = 0;
    int oldTimeSinceStart = 0;
    vector<Vector2> wave;
    vector<double> xValues;
    vector<double> yValues;
    vector<double> xRestoredValues;
    vector<double> yRestoredValues;
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

    vector<double> invertedDisceteFourierTransform(vector<WavePoint> fourier)
    {
        vector<double> x;

        for (int n = 0; n < fourier.size(); n++)
        {
            double sum = 0;

            for (int k = 0; k < fourier.size(); k++)
            {
                double phi = (PI_2 * k * n) / fourier.size();
                sum += fourier[k].amp * cos(phi + fourier[k].phase);
            }

            sum /= fourier.size();
            x.push_back(sum);
        }

        return x;
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

    void drawSignal(vector<double> xS, vector<double> yS, Vector2 offset)
    {
        for (int i = 0; i < xS.size() - 1; i += 1)
        {
            CV::color(1, 1, 1);
            CV::line(xS[i] + offset.x,
                    yS[i] + offset.y,
                    xS[i + 1] + offset.x,
                    yS[i + 1] + offset.y);
            CV::color(0, 0, 1);
            CV::circleFill(xS[i] + offset.x, yS[i] + offset.y, 1, 16);
        }
    }
};

#endif // FOURIER_H_INCLUDED
