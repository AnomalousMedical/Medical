using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Transparency
{
    class TransparencyGroup
    {
        private Dictionary<String, TransparencyInterface> alphaObjects = new Dictionary<string, TransparencyInterface>();

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

        public void setAlphaValue(float alpha)
        {
            foreach (TransparencyInterface alphaObject in alphaObjects.Values)
            {
                alphaObject.setAlpha(alpha);
            }
        }

        public void setAlphaValue(String objName, float alpha)
        {
            if (alphaObjects.ContainsKey(objName))
            {
                alphaObjects[objName].setAlpha(alpha);
            }
        }

        public float getAlphaValue(String objName)
        {
            if (alphaObjects.ContainsKey(objName))
            {
                return alphaObjects[objName].CurrentAlpha;
            }
            return 0.0f;
        }

        public bool isEmpty()
        {
            return alphaObjects.Count == 0;
        }

        public IEnumerable<TransparencyInterface> getAlphaObjectIter()
        {
            return alphaObjects.Values;
        }

        public RenderGroup Name { get; private set; }
    }
}
