﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    public interface ViewHostFactory
    {
        ViewHost createViewHost(View view, AnomalousMvcContext context);
    }
}