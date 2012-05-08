using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class ModelMemory
    {
        private Dictionary<String, Object> models = new Dictionary<String, Object>();

        public ModelMemory()
        {

        }

        public void add(String name, Object model)
        {
            if (models.ContainsKey(name))
            {
                models[name] = model;
            }
            else
            {
                models.Add(name, model);
            }
        }

        public void remove(String name)
        {
            models.Remove(name);
        }

        public void clear()
        {
            models.Clear();
        }

        public TypeName get<TypeName>(String name)
            where TypeName : class
        {
            return this[name] as TypeName;
        }

        Object this[String name]
        {
            get
            {
                Object value;
                models.TryGetValue(name, out value);
                return value;
            }
        }
    }
}
