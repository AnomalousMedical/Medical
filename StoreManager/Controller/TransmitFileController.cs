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
    class TransmitFileController
    {
        private LicenseManager licenseManager;
        private bool allowSend = true;

        public TransmitFileController(AnomalousMvcContext context, LicenseManager licenseManager)
        {
            MvcController controller = context.Controllers["TransmitFile"];
            this.licenseManager = licenseManager;

            ((RunCommandsAction)controller.Actions["SendFile"]).addCommand(new CallbackCommand((executingContext) =>
            {
                sendFile(executingContext);
            }));
        }

        private void sendFile(AnomalousMvcContext executingContext)
        {
            if (allowSend)
            {
                allowSend = false;
                ViewHostControl creatingArchive = executingContext.RunningActionViewHost.findControl("CreatingArchive");
                creatingArchive.Visible = true;
                ViewHostControl creatingArchiveDone = executingContext.RunningActionViewHost.findControl("CreatingArchiveDone");
                creatingArchiveDone.Visible = false;

                ViewHostControl gettingUploadUrl = executingContext.RunningActionViewHost.findControl("GettingUploadUrl");
                gettingUploadUrl.Visible = false;
                ViewHostControl gettingUploadUrlDone = executingContext.RunningActionViewHost.findControl("GettingUploadUrlDone");
                gettingUploadUrlDone.Visible = false;

                ViewHostControl sendingFile = executingContext.RunningActionViewHost.findControl("SendingFile");
                sendingFile.Visible = false;
                ViewHostControl sendingFileDone = executingContext.RunningActionViewHost.findControl("SendingFileDone");
                sendingFileDone.Visible = false;

                ViewHostControl completingUpload = executingContext.RunningActionViewHost.findControl("CompletingUpload");
                completingUpload.Visible = false;
                ViewHostControl completingUploadDone = executingContext.RunningActionViewHost.findControl("CompletingUploadDone");
                completingUploadDone.Visible = false;

                ThreadPool.QueueUserWorkItem(arg =>
                {
                    createArchive();
                    ThreadManager.invokeAndWait(() =>
                    {
                        creatingArchiveDone.Visible = true;
                        gettingUploadUrl.Visible = true;
                    });
                    getUploadUrl();
                    ThreadManager.invokeAndWait(() =>
                    {
                        gettingUploadUrlDone.Visible = true;
                        sendingFile.Visible = true;
                    });
                    sendFile();
                    ThreadManager.invokeAndWait(() =>
                    {
                        sendingFileDone.Visible = true;
                        completingUpload.Visible = true;
                    });
                    completeUpload();
                    ThreadManager.invokeAndWait(() =>
                    {
                        completingUploadDone.Visible = true;
                    });
                });
            }
        }

        private void createArchive()
        {
            Thread.Sleep(1000);
        }

        private void getUploadUrl()
        {
            Thread.Sleep(1000);
        }

        private void sendFile()
        {
            Thread.Sleep(1000);
        }

        private void completeUpload()
        {
            Thread.Sleep(1000);
        }
    }
}
