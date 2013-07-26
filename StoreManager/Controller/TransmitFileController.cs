using Anomalous.Medical.StoreManager.Config;
using Anomalous.Medical.StoreManager.Models;
using Ionic.Zip;
using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Anomalous.Medical.StoreManager.Controller
{
    class TransmitFileController
    {
        private LicenseManager licenseManager;
        private bool allowSend = true;
        private String pluginSourcePath;
        private String archiveFile;

        public TransmitFileController(AnomalousMvcContext context, LicenseManager licenseManager, String pluginSourcePath)
        {
            MvcController controller = context.Controllers["TransmitFile"];
            this.licenseManager = licenseManager;
            this.pluginSourcePath = pluginSourcePath;

            ((RunCommandsAction)controller.Actions["SendFile"]).addCommand(new CallbackCommand((executingContext) =>
            {
                sendFile(executingContext);
            }));
        }

        private void sendFile(AnomalousMvcContext executingContext)
        {
            if (allowSend)
            {
                DataModel model = executingContext.getModel<DataModel>("PluginDetails");
                String storeUniqueName = model.getValue("StoreUniqueName");
                String pluginUniqueName = model.getValue("PluginUniqueName");

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

                ViewHostControl error = executingContext.RunningActionViewHost.findControl("Error");
                error.Visible = false;
                ViewHostControl errorMessage = executingContext.RunningActionViewHost.findControl("ErrorMessage");
                errorMessage.Value = "";

                ThreadPool.QueueUserWorkItem(arg =>
                {
                    try
                    {
                        createArchive(pluginUniqueName);
                        ThreadManager.invokeAndWait(() =>
                        {
                            creatingArchiveDone.Visible = true;
                            gettingUploadUrl.Visible = true;
                        });
                        String uploadUrl = getUploadUrl(storeUniqueName, pluginUniqueName);
                        ThreadManager.invokeAndWait(() =>
                        {
                            gettingUploadUrlDone.Visible = true;
                            sendingFile.Visible = true;
                        });
                        sendFile(uploadUrl);
                        ThreadManager.invokeAndWait(() =>
                        {
                            sendingFileDone.Visible = true;
                            completingUpload.Visible = true;
                        });
                        completeUpload(storeUniqueName, pluginUniqueName);
                        ThreadManager.invokeAndWait(() =>
                        {
                            completingUploadDone.Visible = true;
                            executingContext.runAction("TransmitFile/SendFileCompleted");
                        });
                    }
                    catch (Exception ex)
                    {
                        ThreadManager.invokeAndWait(() =>
                        {
                            error.Visible = true;
                            errorMessage.Value = ex.Message;
                        });
                    }

                    try
                    {
                        cleanupFiles();
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.Error("{0} occured when trying to delete the archive file. Message: {1}", ex.GetType().Name, ex.Message);
                    }

                    allowSend = true;
                });
            }
        }

        private void createArchive(String pluginUniqueName)
        {
            String zipFolder = StoreManagerConfig.UploadArchiveDirectory;
            if (!Directory.Exists(zipFolder))
            {
                Directory.CreateDirectory(zipFolder);
            }
            archiveFile = Path.Combine(zipFolder, Path.ChangeExtension(pluginUniqueName, "zip"));
            cleanupFiles(); //Just make sure the archive doesn't already exist
            using (ZipFile zipFile = new ZipFile(archiveFile))
            {
                zipFile.AddDirectory(pluginSourcePath, Path.Combine("Plugins", pluginUniqueName));
                zipFile.Save();
            }
        }

        private String getUploadUrl(String storeUniqueName, String pluginUniqueName)
        {
            CredentialServerConnection serverConnection = new CredentialServerConnection(StoreManagerConfig.GetBeginUploadUrl(storeUniqueName), licenseManager.User, licenseManager.MachinePassword);
            serverConnection.addArgument("PluginUniqueName", pluginUniqueName);
            ResponseModel response = null;
            try
            {
                response = serverConnection.makeRequestSaveableResponse() as ResponseModel;
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} occured when trying to get the list of user stores. Message: {1}", ex.GetType().Name, ex.Message);
            }
            if (response == null)
            {
                throw new Exception("Response from server was invalid.");
            }
            if (!response.Success)
            {
                throw new Exception(response.Message);
            }
            return response.Message;
        }

        private void sendFile(String uploadUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(uploadUrl));
            request.Timeout = ServerConnection.DefaultTimeout;
            request.Method = "PUT";
            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            request.ContentType = "application/zip";
            using (Stream stream = File.Open(archiveFile, FileMode.Open, FileAccess.Read))
            {
                request.ContentLength = stream.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    stream.CopyTo(dataStream);
                }
            }

            // Get the response.
            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode statusCode = webResponse.StatusCode;
                if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Created)
                {
                    throw new Exception(String.Format("Error uploading file to Anomalous Medical. Http status was {0}.", webResponse.StatusDescription));
                }
            }
        }

        private void completeUpload(String storeUniqueName, String pluginUniqueName)
        {
            CredentialServerConnection serverConnection = new CredentialServerConnection(StoreManagerConfig.GetUploadCompleteUrl(storeUniqueName), licenseManager.User, licenseManager.MachinePassword);
            serverConnection.addArgument("PluginUniqueName", pluginUniqueName);
            ResponseModel response = null;
            try
            {
                response = serverConnection.makeRequestSaveableResponse() as ResponseModel;
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} occured when trying to get the list of user stores. Message: {1}", ex.GetType().Name, ex.Message);
            }
            if (response == null)
            {
                throw new Exception("Response from server was invalid.");
            }
            if (!response.Success)
            {
                throw new Exception(response.Message);
            }
        }

        private void cleanupFiles()
        {
            if (File.Exists(archiveFile))
            {
                File.Delete(archiveFile);
            }
        }
    }
}
