using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCS
{
    public class DotNaming
    {
        private int _current = 0;

        public string GetCurrent()
        {
            try
            {
                if (_current < 25) // 'A' ~ 'Z'
                {
                    return I2S(_current + 'A');
                }
                else if (_current < 25 + 25 * 25)
                {
                    return string.Concat(I2S((_current - 25) / 25 + 'A'), I2S(_current % 25));
                }
                else
                    throw new OverflowException("이거 그냥 반복문 돌리면 되는데 그걸 하기엔 너모 귀찮다 꺄륵..");
            }
            finally
            {
                _current++;
            }
        }

        private static string I2S(int i)
            => ((char)i).ToString();
    }
}
