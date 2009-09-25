using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TransparencyGroup
    {
        static NaturalSort<String> sorter = new NaturalSort<string>();
        private SortedList<String, TransparencyInterface> alphaObjects = new SortedList<string, TransparencyInterface>(sorter);

        public TransparencyGroup(RenderGroup name)
        {
            this.Name = name;
        }

        public void addTransparencyObject(TransparencyInterface alphaObject)
        {
            alphaObjects.Add(alphaObject.ObjectName, alphaObject);
        }

        public void removeTransparencyObject(TransparencyInterface alphaObject)
        {
            alphaObjects.Remove(alphaObject.ObjectName);
        }

        public bool isEmpty()
        {
            return alphaObjects.Count == 0;
        }

        public IEnumerable<TransparencyInterface> getTransparencyObjectIter()
        {
            return alphaObjects.Values;
        }

        public TransparencyInterface getTransparencyObject(String name)
        {
            TransparencyInterface ret = null;
            alphaObjects.TryGetValue(name, out ret);
            return ret;
        }

        public RenderGroup Name { get; private set; }
    }
}
