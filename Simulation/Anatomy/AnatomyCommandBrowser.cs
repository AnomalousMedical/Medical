using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using System.Reflection;

namespace Medical
{
    class AnatomyCommandBrowser : Browser
    {
        static String[] delimiter = { "." };

        public AnatomyCommandBrowser()
            : base("AnatomyCommands", "Add Anatomy Command")
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (typeof(AnatomyCommand).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        this.addNode(type.Namespace, delimiter, new BrowserNode(type.Name, type));
                    }
                }
            }
        }
    }
}
