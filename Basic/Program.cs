using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Logging;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (MedicalController controller = new MedicalController())
            {
                try
                {
                    controller.intialize();
                    //controller.setMedicalInterface(new HeadController());
                    controller.start();
                }
                catch (Exception e)
                {
                    Log.Default.printException(e);
                    MessageBox.Show(e.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
