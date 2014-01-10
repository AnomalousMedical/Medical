using Anomalous.Medical.StoreManager.Config;
using Anomalous.Medical.StoreManager.Models;
using Ionic.Zip;
using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
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
        private ResourceProvider resourceProvider;
        private String archiveFile;
        private bool continueUpload = true;
        private bool processedLastProgressUpdate = true;

        public TransmitFileController(AnomalousMvcContext context, LicenseManager licenseManager, ResourceProvider resourceProvider)
        {
            MvcController controller = context.Controllers["TransmitFile"];
            this.licenseManager = licenseManager;
            this.resourceProvider = resourceProvider;

            ((RunCommandsAction)controller.Actions["SendFile"]).addCommand(new CallbackCommand((executingContext) =>
            {
                sendFile(executingContext);
            }));
        }

        private void sendFile(AnomalousMvcContext executingContext)
        {
            if (allowSend)
            {
                continueUpload = true;
                DataModel model = executingContext.getModel<DataModel>("PluginDetails");
                String storeUniqueName = model.getValue("StoreUniqueName");
                String pluginUniqueName = model.getValue("PluginUniqueName");

                allowSend = false;
                ViewHost viewHost = executingContext.RunningActionViewHost;
                ViewHostControl creatingArchive = viewHost.findControl("CreatingArchive");
                creatingArchive.Visible = true;
                ViewHostControl creatingArchiveDone = viewHost.findControl("CreatingArchiveDone");
                creatingArchiveDone.Visible = false;

                ViewHostControl gettingUploadUrl = viewHost.findControl("GettingUploadUrl");
                gettingUploadUrl.Visible = false;
                ViewHostControl gettingUploadUrlDone = viewHost.findControl("GettingUploadUrlDone");
                gettingUploadUrlDone.Visible = false;

                ViewHostControl sendingFile = viewHost.findControl("SendingFile");
                sendingFile.Visible = false;
                ViewHostControl sendingFileDone = viewHost.findControl("SendingFileDone");
                sendingFileDone.Visible = false;

                ViewHostControl completingUpload = viewHost.findControl("CompletingUpload");
                completingUpload.Visible = false;
                ViewHostControl completingUploadDone = viewHost.findControl("CompletingUploadDone");
                completingUploadDone.Visible = false;

                ViewHostControl error = viewHost.findControl("Error");
                error.Visible = false;
                ViewHostControl errorMessage = viewHost.findControl("ErrorMessage");
                errorMessage.Value = "";
                RmlViewHostControl progressBarInner = (RmlViewHostControl)viewHost.findControl("ProgressBarInner");
                progressBarInner.Value = "0%";
                progressBarInner.Element.SetProperty("width", "0%");
                ViewHostControl transferRate = viewHost.findControl("TransferRate");
                transferRate.Value = "Calculating Speed";

                ThreadPool.QueueUserWorkItem(arg =>
                {
                    try
                    {
                        createArchive(pluginUniqueName);
                        ThreadManager.invokeAndWait(() =>
                        {
                            if (viewHost.Open)
                            {
                                creatingArchiveDone.Visible = true;
                                gettingUploadUrl.Visible = true;
                            }
                            else
                            {
                                continueUpload = false;
                            }
                        });
                        shouldContinueUpload();
                        String uploadUrl = getUploadUrl(storeUniqueName, pluginUniqueName);
                        ThreadManager.invokeAndWait(() =>
                        {
                            if (viewHost.Open)
                            {
                                gettingUploadUrlDone.Visible = true;
                                sendingFile.Visible = true;
                            }
                            else
                            {
                                continueUpload = false;
                            }
                        });
                        shouldContinueUpload();
                        processedLastProgressUpdate = true;
                        sendFile(uploadUrl, (progress, kbPerSec, finished) =>
                            {
                                if (processedLastProgressUpdate || finished)
                                {
                                    processedLastProgressUpdate = false;
                                    ThreadManager.invoke(() =>
                                        {
                                            if (viewHost.Open)
                                            {
                                                String width = progress + "%";
                                                progressBarInner.Element.SetProperty("width", width);
                                                progressBarInner.Value = width;
                                                if (Double.IsInfinity(kbPerSec))
                                                {
                                                    transferRate.Value = "Calculating Speed";
                                                }
                                                else
                                                {
                                                    transferRate.Value = String.Format("{0:N2} Kb per second", kbPerSec);
                                                }
                                            }
                                            else
                                            {
                                                continueUpload = false;
                                            }
                                            processedLastProgressUpdate = true;
                                        });
                                    //Thread.Sleep(100);
                                    shouldContinueUpload(); //Even though we don't wait here its possible that continueUpload will get set to false, so this will stop the upload process.
                                }
                            });
                        ThreadManager.invokeAndWait(() =>
                        {
                            if (viewHost.Open)
                            {
                                sendingFileDone.Visible = true;
                                completingUpload.Visible = true;
                            }
                            else
                            {
                                continueUpload = false;
                            }
                        });
                        shouldContinueUpload();
                        completeUpload(storeUniqueName, pluginUniqueName);
                        ThreadManager.invokeAndWait(() =>
                        {
                            if (viewHost.Open)
                            {
                                completingUploadDone.Visible = true;
                                executingContext.runAction("TransmitFile/SendFileCompleted");
                            }
                            else
                            {
                                continueUpload = false;
                            }
                        });
                        //Got to the end, dont need to check continue upload, add that if you add another step.
                    }
                    catch (CancelUploadException)
                    {
                        cancelUpload(storeUniqueName, pluginUniqueName);
                    }
                    catch (Exception ex)
                    {
                        String message = ex.Message;
                        try
                        {
                            cancelUpload(storeUniqueName, pluginUniqueName);
                        }
                        catch (Exception cancelException)
                        {
                            String.Format("{0} Additional {1} canceling the upload request. Message: {2}.", message, cancelException.GetType().Name, cancelException.Message);
                        }
                        ThreadManager.invokeAndWait(() =>
                        {
                            if (viewHost.Open)
                            {
                                error.Visible = true;
                                errorMessage.Value = message;
                            }
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
            String inArchivePath = Path.Combine("Plugins", pluginUniqueName);
            if (File.Exists(resourceProvider.BackingLocation)) //If the file exists for the resource provider backing store, we assume it is a zip for now
            {
                File.Copy(resourceProvider.BackingLocation, archiveFile);
                using (ZipFile zipFile = new ZipFile(archiveFile))
                {
                    int fileCount = zipFile.Count;
                    for (int i = 0; i < fileCount; ++i)
                    {
                        var entry = zipFile[i];
                        entry.FileName = Path.Combine(inArchivePath, entry.FileName);
                    }
                    zipFile.Save();
                }
            }
            else
            {
                using (ZipFile zipFile = new ZipFile(archiveFile))
                {
                    zipFile.AddDirectory(resourceProvider.BackingLocation, inArchivePath);
                    zipFile.Save();
                }
            }
        }

        private String getUploadUrl(String storeUniqueName, String pluginUniqueName)
        {
            CredentialServerConnection serverConnection = new CredentialServerConnection(StoreManagerConfig.GetBeginUploadUrl(storeUniqueName), licenseManager.User, licenseManager.MachinePassword);
            serverConnection.addArgument("PluginUniqueName", pluginUniqueName);
            ResponseModel response = null;
            try
            {
                response = serverConnection.makeRequestSaveableResponse(StoreManagerTypeFinder.Instance) as ResponseModel;
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

        private void sendFile(String uploadUrl, Action<int, double, bool> progressCallback)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(uploadUrl));
            request.Timeout = ServerConnection.DefaultTimeout;
            request.Method = "PUT";
            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            request.ContentType = "application/zip";
            request.AllowWriteStreamBuffering = false;
            byte[] buffer = new byte[4096];
            using (Stream stream = File.Open(archiveFile, FileMode.Open, FileAccess.Read))
            {
                request.ContentLength = stream.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    try
                    {
                        int read;
                        float totalRead = 0;
                        DateTime startTime = DateTime.Now;
                        TimeSpan totalTime;
                        double kbPerSec = 0.0;
                        while((read = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            dataStream.Write(buffer, 0, read);
                            totalRead += read;
                            totalTime = DateTime.Now - startTime;
                            kbPerSec = (totalRead * 1000.0f) / (totalTime.TotalMilliseconds * 1024.0f);
                            progressCallback((int)(totalRead / stream.Length * 100), kbPerSec, false);
                        }
                        progressCallback(100, kbPerSec, true);
                    }
                    catch (CancelUploadException ex)
                    {
                        //Can't come up with anything to fix the uploading problems, it will
                        //cancel, but the endpoint in storage will be screwed up for a little while
                        //before the user can upload again.
                        throw ex;
                    }
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
                response = serverConnection.makeRequestSaveableResponse(StoreManagerTypeFinder.Instance) as ResponseModel;
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} occured when trying to complete upload. Message: {1}", ex.GetType().Name, ex.Message);
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

        private void cancelUpload(String storeUniqueName, String pluginUniqueName)
        {
            CredentialServerConnection serverConnection = new CredentialServerConnection(StoreManagerConfig.GetCancelUploadUrl(storeUniqueName), licenseManager.User, licenseManager.MachinePassword);
            serverConnection.addArgument("PluginUniqueName", pluginUniqueName);
            ResponseModel response = null;
            try
            {
                response = serverConnection.makeRequestSaveableResponse(StoreManagerTypeFinder.Instance) as ResponseModel;
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} occured when trying to cancel upload. Message: {1}", ex.GetType().Name, ex.Message);
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

        private void shouldContinueUpload()
        {
            if (!continueUpload)
            {
                throw new CancelUploadException();
            }
        }
    }
}
