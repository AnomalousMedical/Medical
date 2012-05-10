using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    public interface ModelCreationInfo
    {
        Model createModel(String name);

        String DefaultName { get; set; }
    }
}
