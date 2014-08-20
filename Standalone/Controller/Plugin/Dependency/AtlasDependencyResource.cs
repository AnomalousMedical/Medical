using Engine.Editing;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class AtlasDependencyResource : Saveable
    {
        public AtlasDependencyResource()
        {

        }

        public String Path { get; set; }

        public bool Recursive { get; set; }

        protected AtlasDependencyResource(LoadInfo info)
        {
            Path = info.GetString("Path");
            Recursive = info.GetBoolean("Recursive");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Path", Path);
            info.AddValue("Recursive", Recursive);
        }
    }
}
