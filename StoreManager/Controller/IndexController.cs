using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Anomalous.Medical.StoreManager.Controller
{
    class IndexController
    {
        private bool allowLogin = true;

        public IndexController(AnomalousMvcContext context, LicenseManager licenseManager)
        {
            ((RunCommandsAction)context.Controllers["Index"].Actions["Opening"]).addCommand(new CallbackCommand((executingContext) =>
                {
                    ViewHostControl passwordTextBox = executingContext.RunningActionViewHost.findControl("Password");
                    passwordTextBox.focus();
                }));

            ((RunCommandsAction)context.Controllers["Index"].Actions["CheckPassword"]).addCommand(new CallbackCommand((executingContext) =>
            {
                if (allowLogin)
                {
                    allowLogin = false;
                    ViewHostControl passwordTextBox = executingContext.RunningActionViewHost.findControl("Password");
                    String pass = passwordTextBox.Value;
                    ThreadPool.QueueUserWorkItem(arg =>
                    {
                        try
                        {
                            bool loginSucessful = licenseManager.login(licenseManager.User, pass);
                            ThreadManager.invoke(() =>
                            {
                                if (loginSucessful)
                                {
                                    executingContext.runAction("Index/Next");
                                }
                                else
                                {
                                    ViewHostControl error = executingContext.RunningActionViewHost.findControl("Error");
                                    error.Visible = true;
                                    error.Value = "The password you entered is not correct.";
                                    passwordTextBox.Value = "";
                                    passwordTextBox.focus();
                                }
                                allowLogin = true;
                            });
                        }
                        catch (AnomalousLicenseServerException ex)
                        {
                            ThreadManager.invoke(() =>
                            {
                                ViewHostControl error = executingContext.RunningActionViewHost.findControl("Error");
                                error.Visible = true;
                                error.Value = ex.Message;
                                passwordTextBox.Value = "";
                                passwordTextBox.focus();
                                allowLogin = true;
                            });
                        }
                    });
                }
            }));
        }
    }
}
