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
        private int _labelnum = 0;

        public string GetCurrent()
        {
            try
            {
                if (_current < 26) // 'A' ~ 'Z'
                {
                    return I2S(_current + 'A');
                }
                else 
                {
                    return string.Concat(I2S( _current % 26 + 'A'), Convert.ToString(_labelnum + 1));
                    if (_current % 26 == 0) _labelnum++;
                }
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
