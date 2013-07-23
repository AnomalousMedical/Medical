using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Models
{
    public class StoreModel : Saveable
    {
        public StoreModel()
        {

        }

        public String Name { get; set; }

        public String UniqueName { get; set; }

        protected StoreModel(LoadInfo info)
        {
            Name = info.GetString("Name");
            UniqueName = info.GetString("UniqueName");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", Name);
            info.AddValue("UniqueName", UniqueName);
        }
    }
}
