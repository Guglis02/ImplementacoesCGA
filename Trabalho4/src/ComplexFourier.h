#ifndef COMPLEXFOURIER_H_INCLUDED
#define COMPLEXFOURIER_H_INCLUDED

#include "./Fourier.h"
#include "./gl_canvas2d.h"
#include <math.h>
#include <vector>
#include <algorithm>

using namespace std;

class ComplexFourier : public Fourier
{
public:
    ComplexFourier(){}

    void Setup(vector<double> points)
    {
        size_t size = points.size();
        int step = size / (size * 0.5);

        for (int i = 0; i < points.size() - 1; i += step)
        {
            Vector2 c = Vector2(points[i], points[i + 1]);
            originalSignal.push_back(c);
        }

        fourierSignal = complexDiscreteFourierTransform(originalSignal);

        std::sort(fourierSignal.begin(), fourierSignal.end(), [](const WavePoint& a, const WavePoint& b) {
            return b.amp < a.amp;
        });    
    }

    void Render(int screenWidth, int screenHeight)
    {
        Vector2 v = epiCycles(screenWidth >> 1, screenHeight >> 1, 0, fourierSignal);

        wave.insert(wave.begin(), v);

        CV::color(1, 1, 1);
        for (int i = 0; i < wave.size() - 1; i++)
        {
            CV::line(wave[i].x, wave[i].y, wave[i + 1].x, wave[i + 1].y);
        }

        time += PI_2 / fourierSignal.size();

        if (time > PI * 2)
        {
            time = 0;
            wave.clear();
        }
    }
protected:
    vector<Vector2> originalSignal;
    vector<WavePoint> fourierSignal;

    vector<WavePoint> complexDiscreteFourierTransform(vector<Vector2> signal)
    {
        vector<WavePoint> X;
        int N = signal.size();

        for (int k = 0; k < N; k++)
        {
            Vector2 sum = Vector2(0, 0);
            
            for (int n = 0; n < N; n++)
            {
                double phi = (PI_2 * k * n) / N;
                Vector2 c = Vector2(cos(phi), -sin(phi));
                sum += (signal[n] * c);
            }

            sum.x /= N;
            sum.y /= N;

            double freq = k;
            double amp = sqrt(sum.x * sum.x + sum.y * sum.y);
            double phase = atan2(sum.y, sum.x);

            WavePoint wavePoint = WavePoint(sum.x, sum.y, freq, amp, phase);

            X.push_back(wavePoint);
        }

        return X;
    }
};

#endif // COMPLEXFOURIER_H_INCLUDED