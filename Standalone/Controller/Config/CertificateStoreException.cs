using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class CertificateStoreException : Exception
    {
        public CertificateStoreException(String message)
            :base(message)
        {

        }
    }
}
