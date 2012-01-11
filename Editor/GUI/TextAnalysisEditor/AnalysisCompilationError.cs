using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class AnalysisCompilationError : Exception
    {
        public AnalysisCompilationError(String message)
            :base("Compilation Error: " + message)
        {
            
        }
    }
}
