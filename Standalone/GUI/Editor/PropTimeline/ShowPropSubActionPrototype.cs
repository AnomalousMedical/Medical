using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    sealed class ShowPropSubActionPrototype
    {
        private Type type;

        public ShowPropSubActionPrototype(Type type, String typeName)
        {
            this.type = type;
            this.TypeName = typeName;
        }

        public ShowPropSubAction createSubAction()
        {
            return (ShowPropSubAction)Activator.CreateInstance(type);
        }

        public PropTimelineData createData(ShowPropSubAction action, PropEditController propEditController)
        {
            return new PropTimelineData(action, propEditController);
        }

        public String TypeName { get; private set; }
    }
}
