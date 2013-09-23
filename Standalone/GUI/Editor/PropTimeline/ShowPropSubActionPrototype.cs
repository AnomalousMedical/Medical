﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    class ShowPropSubActionPrototype
    {
        private Type type;

        public ShowPropSubActionPrototype(Type type, String typeName)
        {
            this.type = type;
            this.TypeName = typeName;
        }

        public virtual ShowPropSubAction createSubAction()
        {
            return (ShowPropSubAction)Activator.CreateInstance(type);
        }

        public virtual PropTimelineData createData(ShowPropSubAction action)
        {
            return new PropTimelineData(action);
        }

        public String TypeName { get; private set; }
    }
}
