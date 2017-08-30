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
                return (_current < 26)
                    ? I2S(_current + 'A')
                    : string.Concat(I2S( _current % 26 + 'A'), Convert.ToString(_labelnum + 1));
            }
            finally
            {
                _current++;
            }
        }

        private static string I2S(int i) => ((char)i).ToString();
    }
}
