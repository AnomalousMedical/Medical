using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using Engine;
using OgrePlugin;
using BulletPlugin;
using PCPlatform;
using Engine.Platform;
using Engine.Renderer;
using Medical;
using System.Xml;
using Engine.ObjectManagement;
using System.IO;
using Engine.Saving.XMLSaver;
using OgreWrapper;

namespace Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StandaloneController controller = new StandaloneController())
            {
                //try
                {
                    controller.go();
                }
                //catch (Exception e)
                //{
                //    Log.Default.printException(e);
                //    String errorMessage = e.Message + "\n" + e.StackTrace;
                //    while (e.InnerException != null)
                //    {
                //        e = e.InnerException;
                //        errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
                //    }
                //    //MessageBox.Show(errorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
            }
        }
    }
}
