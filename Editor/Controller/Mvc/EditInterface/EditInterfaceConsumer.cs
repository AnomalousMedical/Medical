using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    interface EditInterfaceConsumer
    {
        EditInterface CurrentEditInterface { get; set; }
    }
}
