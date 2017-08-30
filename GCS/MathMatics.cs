using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GCS
{
    public static class MathMatics
    {
        private const float Upperbound = 1000; 

        public static float [] Solve2Eq(float a, float b, float c)
        {
            float d = b * b - 4 * a * c;
            float result;

            if (d < 0)
                return new float[] { };
            else if (d == 0)
            {
                result = -b / (2 * a);

                return new float[] { result, result };
            }
            else
            {
                result = (-b + (float)Math.Sqrt(d)) / (2 * a);

                return new float[] { result, result };
            }
        }
 
        // 근이 - Upperbound, Upperbound 안에 있다고 가정, 충분히 큰 Upperbound 를 주면 됨
        public static float[] solveNEq(float[] polynorm, int accuracy = 3)
        {
            List<float> answer = new List<float>();
            List<float> coef = polynorm.ToList();
 
            if (coef.Count == 0)
                return new float[] { };
            else
                while (coef.First() == 0)
                    coef.Remove(0);

            if (coef.Count <= 1) return new float[] { };
            else if (coef.Count == 2) return new float[] { -coef[0] / coef[1] };
            else if (coef.Count == 3) return Solve2Eq(coef[0], coef[1], coef[2]);
            else
            {
                float[] diff = new float[coef.Count - 1];
                for (int i = 0; i < coef.Count - 1; i++)
                {
                    diff[i] = (coef.Count - 1 - i) * coef[i]; // 극값을 구한 후
                }
                List<float> extreme_vals = solveNEq(diff, accuracy).ToList();
                extreme_vals.Sort();

                extreme_vals.Insert(0, -Upperbound);
                extreme_vals.Add(Upperbound);

                for (int j = 0; j < extreme_vals.Count - 1; j++) // 극값을 기준으로 구간을 나눠서 이진 탐색 실시
                {
                    if (applyPolynormialFunc(coef, extreme_vals[j]) < Math.Pow(10, -accuracy))
                        answer.Add(extreme_vals[j]);
                    else if (applyPolynormialFunc(coef, extreme_vals[j + 1]) < Math.Pow(10, -accuracy))
                        answer.Add(extreme_vals[j + 1]);
                    // 중근을 먼저 찾고(중근은 극값안에 있음)
                    else if (applyPolynormialFunc(coef, extreme_vals[j]) * applyPolynormialFunc(coef, extreme_vals[j + 1]) < 0)
                    {
                        float[] interval = new float[] { extreme_vals[j], extreme_vals[j + 1] };

                        while (Math.Abs(applyPolynormialFunc(coef, interval[0])) > Math.Pow(10, -accuracy))
                        {
                            int index = (applyPolynormialFunc(coef, interval[0]) * applyPolynormialFunc(coef, (interval[0] + interval[1]) / 2) < 0)
                                ? 1
                                : 0;

                            interval[index] = (interval[1] + interval[0]) / 2;
                        }

                        answer.Add(interval[0]);
                    }
                }
                answer.ForEach(x => Math.Round(x, accuracy));

                return answer.ToArray();
            }
        }

        private static float applyPolynormialFunc(List<float> polynorm, float x)
        {
            float sum = 0;

            for (int i = 0; i < polynorm.Count; i++)
                sum += (float)Math.Pow(x, polynorm.Count - 1 - i) * polynorm[i];

            return sum;
        }
    }
}
