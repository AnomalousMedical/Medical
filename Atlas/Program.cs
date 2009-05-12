using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Medical;
using Medical.Controller;

namespace Atlas
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (MedicalController controller = new MedicalController())
            {
                controller.intialize();
                controller.setMedicalInterface(new HeadController());
                controller.start();
            }
        }
    }
}
