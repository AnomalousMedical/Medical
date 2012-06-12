﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    interface EditMenuProvider
    {
        void cut();

        void copy();

        void paste();

        void selectAll();
    }
}
