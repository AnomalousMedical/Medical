using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.Exam;

namespace Medical.GUI
{
    /// <summary>
    /// This class provides information about a data field used in a variable
    /// browser.
    /// </summary>
    class DataFieldInfo
    {
        public DataFieldInfo(String path, String name)
        {
            this.Path = path;
            this.Name = name;
        }

        public DataRetriever createDataRetriever()
        {
            return new DataRetriever(Name, Path.Split('.'));
        }

        public String Path { get; set; }

        public String Name { get; set; }

        public String FullName
        {
            get
            {
                return String.Format("{0}.{1}", Path, Name);
            }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
