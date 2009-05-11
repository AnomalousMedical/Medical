using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class MedicalConfig
    {
        private static String docRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Atlas";

        public static String DocRoot
        {
            get
            {
                return docRoot;
            }
        }
    }
}
