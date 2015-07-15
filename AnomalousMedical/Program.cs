using Engine.Platform;
using System;
using System.Collections.Generic;

namespace Medical
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //OgrePlugin.OgreInterface.CompressedTextureSupport = OgrePlugin.CompressedTextureSupport.None; //Temp, disable dds

            Medical.Main.Run();
        }
    }
}
