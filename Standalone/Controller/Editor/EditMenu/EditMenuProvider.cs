using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    public interface EditMenuProvider
    {
        void cut();

        void copy();

        void paste();

        void selectAll();
    }
}
