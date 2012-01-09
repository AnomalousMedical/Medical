using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class RenderPreset : Saveable
    {
        public RenderPreset()
        {

        }

        public RenderPreset(String name, int width, int height)
        {
            this.Name = name;
            this.Width = width;
            this.Height = height;
        }

        public String Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        protected RenderPreset(LoadInfo info)
        {
            Name = info.GetString("Name");
            Width = info.GetInt32("Width");
            Height = info.GetInt32("Height");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", Name);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
        }
    }
}
