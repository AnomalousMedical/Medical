using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public sealed class ShowPropSubActionPrototype
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

        public String TypeName { get; private set; }
    }
}
