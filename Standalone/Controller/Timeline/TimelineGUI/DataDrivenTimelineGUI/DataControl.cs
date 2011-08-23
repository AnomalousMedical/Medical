using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class DataControl : LayoutContainer, IDisposable
    {
        public abstract void Dispose();

        public abstract void captureData(DataDrivenExamSection examSection);

        public abstract void displayData(DataDrivenExamSection examSection);
    }
}
