using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public delegate void MenuEvent(MenuEntry item);

    public class MenuEntry : IDisposable
    {
        public void Dispose()
        {

        }

        public event MenuEvent Select;

        public bool Enabled { get; set; }

        public int ID { get; set; }

        public Menu SubMenu { get; set; }

        public String Help { get; set; }
    }
}
