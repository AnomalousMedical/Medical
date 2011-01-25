using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using Engine;
using OgrePlugin;
using BulletPlugin;
using Engine.Platform;
using Engine.Renderer;
using Medical;
using System.Xml;
using Engine.ObjectManagement;
using System.IO;
using Engine.Saving.XMLSaver;
using OgreWrapper;
using Medical.Controller;
using System.Drawing;
using Medical.GUI;

namespace Medical
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            PiperJBOController piperJBO = null;
            try
            {
                piperJBO = new PiperJBOController();
                piperJBO.run();
            }
            catch (Exception e)
            {
                Logging.Log.Default.printException(e);
                if (piperJBO != null)
                {
                    piperJBO.saveCrashLog();
                }
                String errorMessage = e.Message + "\n" + e.StackTrace;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
                }
                MessageDialog.showErrorDialog(errorMessage, "Exception");
            }
        }
    }
}
