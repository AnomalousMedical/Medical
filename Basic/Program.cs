using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Logging;
using Medical.GUI;
using Medical.Controller;

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
            bool connectionLoop = true;
            using (UserPermissions permissions = new UserPermissions())
            {
                while (connectionLoop)
                {
                    if (UserPermissions.Instance.checkConnection())
                    {
                        connectionLoop = false;
                        using (BasicController controller = new BasicController())
                        {
                            try
                            {
                                controller.go();
                            }
                            catch (Exception e)
                            {
                                Log.Default.printException(e);
                                MessageBox.Show(e.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        connectionLoop = MessageBox.Show("Please connect your dongle.", "Dongle Connection Failure", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Retry;
                    }
                }
            }
        }
    }
}
