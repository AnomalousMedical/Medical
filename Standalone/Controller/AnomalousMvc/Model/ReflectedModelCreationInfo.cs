using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Medical.Controller.AnomalousMvc
{
    public class ReflectedModelCreationInfo : ModelCreationInfo
    {
        private static readonly Type[] constructorArgs = { typeof(String) };

        public ReflectedModelCreationInfo(String defaultName, Type modelType)
        {
            this.DefaultName = defaultName;
            this.ModelType = modelType;
        }

        public Model createModel(String name)
        {
            ConstructorInfo constructor = ModelType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, constructorArgs, null);
            if (constructor != null)
            {
                return (Model)constructor.Invoke(new Object[] { name });
            }
            return null;
        }

        public String DefaultName { get; set; }

        public Type ModelType { get; set; }
    }
}
