using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class SaveableLinkedList<Type> : LinkedList<Type>, Saveable
        where Type : Saveable
    {
        public SaveableLinkedList()
        {

        }

        protected SaveableLinkedList(LoadInfo info)
        {
            info.RebuildLinkedList<Type>("Item", this);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractLinkedList<Type>("Item", this);
        }
    }
}
