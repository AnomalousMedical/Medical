using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface TwoWayCommand
    {
        void execute();

        void undo();

        void poppedFront();

        void cleared();

        void trimmed();
    }
}
