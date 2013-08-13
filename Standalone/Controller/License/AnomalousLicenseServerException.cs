using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class AnomalousLicenseServerException : Exception
    {
        public AnomalousLicenseServerException(String message)
            : base(message)
        {

        }
    }
}
