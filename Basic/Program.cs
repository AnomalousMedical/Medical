using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Logging;
using Medical.GUI;

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
                using (MedicalForm medicalForm = new MedicalForm())
                {
                    try
                    {
                        controller.intialize(medicalForm);
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
}
