using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCS
{
    public enum Actions
    {
        CREATE,
        DELETE,
        MOVE
    }

    public class ImportantAction
    {
        public Actions action;
        
        public ImportantAction()
        {
               
        }
    }
 
}
