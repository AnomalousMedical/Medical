using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Medical.Controller.AnomalousMvc;
using System.Threading;
using Engine;
using Logging;
using MyGUIPlugin;
using System.Net;
using libRocketPlugin;

namespace Medical.Controller
{
    class MvcLoginController
    {
        StandaloneController controller;
        AnomalousMvcContext context;
        LicenseManager licenseManager;
        bool loggingIn = false;
        private ViewHostControl passwordControl;
        private ViewHostControl messageControl;
        private ViewHostControl errorControl;
        public event Action LoginSucessful;

        public MvcLoginController(StandaloneController controller, LicenseManager licenseManager)
        {
            this.controller = controller;
            this.licenseManager = licenseManager;
        }

        public void showContext()
        {
            EmbeddedResourceProvider embeddedResourceProvider = new EmbeddedResourceProvider(Assembly.GetExecutingAssembly(), "Medical.MvcContexts.Login.");
            controller.TimelineController.setResourceProvider(embeddedResourceProvider);

            ResourceProviderRocketFSExtension resourceProviderRocketFSExtension = new ResourceProviderRocketFSExtension(embeddedResourceProvider);

            //Load and run the mvc context
            context = controller.MvcCore.loadContext(embeddedResourceProvider.openFile("MvcContext.mvc"));

            context.Started += (ctx) =>
            {
                RocketInterface.Instance.FileInterface.addExtension(resourceProviderRocketFSExtension);
            };

            context.RemovedFromStack += (ctx) =>
            {
                RocketInterface.Instance.FileInterface.removeExtension(resourceProviderRocketFSExtension);
            };

            context.RuntimeName = "LogIn";
            context.setResourceProvider(embeddedResourceProvider);
            DataModel credentialsModel = (DataModel)context.Models["Credentials"];
            credentialsModel.setValue("ConnectionURL", MedicalConfig.LicenseServerURL);
            bool focusPassword = false;
            if (MedicalConfig.StoreCredentials)
            {
                credentialsModel.setValue("Remember", "True");
            }
            if (licenseManager.IdentifiedUserName != null)
            {
                credentialsModel.setValue("User", licenseManager.IdentifiedUserName);
                focusPassword = true;
            }
            if (licenseManager.KeyDialogMessage != null)
            {
                credentialsModel.setValue("Message", licenseManager.KeyDialogMessage);
            }
            else
            {
                credentialsModel.setValue("Message", "Enter your Anomalous Medical user name and password.");
            }

            ((RunCommandsAction)context.Controllers["Index"].Actions["LogIn"]).addCommand(new CallbackCommand((executingContext) =>
            {
                if (!loggingIn)
                {
                    loggingIn = true;
                    DataModel model = context.getModel<DataModel>("Credentials");
                    MedicalConfig.StoreCredentials = model.getValue("Remember") == "True";
                    messageControl.Value = "Connecting to server.";
                    errorControl.Value = "";
                    ThreadPool.QueueUserWorkItem(new WaitCallback(getLicense), new Pair<String, String>(model.getValue("User"), model.getValue("Pass")));
                }
            }));

            ((RunCommandsAction)context.Controllers["Index"].Actions["Cancel"]).addCommand(new CallbackCommand((executingContext) =>
            {
                controller.exit();
            }));

            ((RunCommandsAction)context.Controllers["Index"].Actions["ForgotPassword"]).addCommand(new CallbackCommand((executingContext) =>
            {
                OtherProcessManager.openUrlInBrowser(MedicalConfig.ForgotPasswordURL);
            }));

            ((RunCommandsAction)context.Controllers["Index"].Actions["Register"]).addCommand(new CallbackCommand((executingContext) =>
            {
                OtherProcessManager.openUrlInBrowser(MedicalConfig.RegisterURL);
            }));

            ((RunCommandsAction)context.Controllers["Index"].Actions["Opening"]).addCommand(new CallbackCommand((executingContext) =>
            {
                passwordControl = executingContext.RunningActionViewHost.findControl("Pass");
                messageControl = executingContext.RunningActionViewHost.findControl("Message");
                errorControl = executingContext.RunningActionViewHost.findControl("Error");
                if (focusPassword)
                {
                    passwordControl.focus();
                }
                else
                {
                    executingContext.RunningActionViewHost.findControl("User").focus();
                }
            }));

            controller.MvcCore.startRunningContext(context);
        }

        public void close()
        {
            context.runAction("Index/Shutdown");
        }

        void getLicense(Object credentials)
        {
            try
            {
                Pair<String, String> cred = (Pair<String, String>)credentials;
                if (licenseManager.login(cred.First, cred.Second))
                {
                    ThreadManager.invoke(new Callback(licenseCaptured), null);
                }
                else
                {
                    ThreadManager.invoke(new Callback(licenseLoginFail), null);
                }
            }
            catch (AnomalousLicenseServerException alse)
            {
                ThreadManager.invoke(new CallbackString(licenseServerFail), alse.Message);
            }
        }

        private delegate void Callback();
        private delegate void CallbackString(String message);

        void licenseCaptured()
        {
            messageControl.Value = "Login Successful, Loading Plugins.";
            if(LoginSucessful != null)
            {
                LoginSucessful.Invoke();
            }
        }

        void licenseLoginFail()
        {
            messageControl.Value = "Please try again.";
            errorControl.Value = "Username or password is invalid.";
            passwordControl.Value = "";
            passwordControl.focus();
            loggingIn = false;
        }

        void licenseServerFail(String message)
        {
            loggingIn = false;
            messageControl.Value = "Please try again.";
            errorControl.Value = message;
            passwordControl.focus();
        }
    }
}
