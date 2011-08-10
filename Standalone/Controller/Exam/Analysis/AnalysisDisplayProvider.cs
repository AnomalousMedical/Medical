using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface AnalysisDisplayProvider
    {
        void showRawData(Exam exam);

        void showTextData(String title, String text);
    }
}
