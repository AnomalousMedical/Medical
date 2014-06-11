using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class AnatomyClickResult
    {
        public AnatomyClickResult(AnatomyIdentifier anatomyIdentifier, float distance)
        {
            this.AnatomyIdentifier = anatomyIdentifier;
            this.Distance = distance;
        }

        public AnatomyIdentifier AnatomyIdentifier { get; private set; }

        public float Distance { get; private set; }
    }
}
