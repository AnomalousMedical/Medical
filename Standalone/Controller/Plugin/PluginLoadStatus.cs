using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class PluginLoadStatus
    {
        public int Total { get; set; }

        public int Current { get; set; }

        public float PercentComplete
        {
            get
            {
                if(Total != 0)
                {
                    return Current / (float)Total;
                }
                return 0.0f;
            }
        }
    }
}
