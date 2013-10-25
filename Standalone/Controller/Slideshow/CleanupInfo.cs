using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    public class CleanupInfo
    {
        private List<String> files = new List<string>();
        private Dictionary<String, List<Object>> objects = new Dictionary<string, List<object>>();

        public void claimFile(String name)
        {
            files.Add(Path.GetFullPath(name));
        }

        public bool isClaimed(String file)
        {
            return files.Contains(Path.GetFullPath(file));
        }

        public void defineObjectClass(String type)
        {
            objects.Add(type, new List<object>());
        }

        public void claimObject(String type, Object obj)
        {
            List<Object> objs;
            if (objects.TryGetValue(type, out objs))
            {
                objs.Add(obj);
            }
            else
            {
                throw new Exception(String.Format("The object class {0} has not been defined.", type));
            }
        }

        public IEnumerable<T> getObjects<T>(String type)
        {
            List<Object> objs;
            if (objects.TryGetValue(type, out objs))
            {
                return objs.Select(o => (T)o);
            }
            else
            {
                throw new Exception(String.Format("The object class {0} has not been defined.", type));
            }
        }

        public void clearObjects(String type)
        {
            List<Object> objs;
            if (objects.TryGetValue(type, out objs))
            {
                objs.Clear();
            }
            else
            {
                throw new Exception(String.Format("The object class {0} has not been defined.", type));
            }
        }
    }
}
