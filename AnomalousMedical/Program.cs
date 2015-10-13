using System;
using System.Collections.Generic;
using System.Net;

namespace Medical
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12; //Enable TLS 1.1 and 1.2

            Medical.Main.Run();
        }
    }
}
