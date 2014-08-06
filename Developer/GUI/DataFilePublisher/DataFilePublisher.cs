using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using MyGUIPlugin;
using Medical;
using System.IO;

namespace Developer.GUI
{
    class DataFilePublisher : MDIDialog
    {
        private const String PLUGIN_WILDCARD = "Data Driven Definition|*.ddp;*.ddd";
        private const String SIGNATURE_WILDCARD = "Personal Information Exchange (*.p12)|*.p12|All Files|*.*";

        private EditBox pluginFileEdit;
        private EditBox signatureFileEdit;
        private EditBox outDirEdit;
        private EditBox certPasswordEdit;
        private EditBox counterSignatureFileEdit;
        private EditBox counterSignaturePassword;

        private DataPublishController dataPublishController;

        public DataFilePublisher(DataPublishController dataPublishController)
            : base("Developer.GUI.DataFilePublisher.DataFilePublisher.layout")
        {
            this.dataPublishController = dataPublishController;

            pluginFileEdit = (EditBox)window.findWidget("PluginFileEdit");
            signatureFileEdit = (EditBox)window.findWidget("SignatureFileEdit");
            signatureFileEdit.Caption = DeveloperConfig.LastPluginKey;
            outDirEdit = (EditBox)window.findWidget("OutDirEdit");
            outDirEdit.Caption = DeveloperConfig.LastPluginExportDirectory;
            certPasswordEdit = (EditBox)window.findWidget("CertPassword");
            counterSignatureFileEdit = (EditBox)window.findWidget("CounterSignatureFileEdit");
            counterSignatureFileEdit.Caption = DeveloperConfig.LastPluginCounterSignatureKey;
            counterSignaturePassword = (EditBox)window.findWidget("CounterSignaturePassword");

            Button pluginFileBrowser = (Button)window.findWidget("PluginFileBrowser");
            pluginFileBrowser.MouseButtonClick += new MyGUIEvent(pluginFileBrowser_MouseButtonClick);

            Button signatureFileBrowser = (Button)window.findWidget("SignatureFileBrowser");
            signatureFileBrowser.MouseButtonClick += new MyGUIEvent(signatureFileBrowser_MouseButtonClick);

            Button outDirBrowser = (Button)window.findWidget("OutDirBrowser");
            outDirBrowser.MouseButtonClick += new MyGUIEvent(outDirBrowser_MouseButtonClick);

            Button publishButton = (Button)window.findWidget("PublishButton");
            publishButton.MouseButtonClick += new MyGUIEvent(publishButton_MouseButtonClick);

            Button counterSignatureFileBrowser = (Button)window.findWidget("CounterSignatureFileBrowser");
            counterSignatureFileBrowser.MouseButtonClick += counterSignatureFileBrowser_MouseButtonClick;
        }

        void publishButton_MouseButtonClick(Widget source, EventArgs e)
        {
            try
            {
                String dataDefinitionName = pluginFileEdit.OnlyText;
                if (File.Exists(dataDefinitionName))
                {
                    doPublish(dataDefinitionName);
                }
                else if(Directory.Exists(dataDefinitionName))
                {
                    foreach (String directory in Directory.GetDirectories(dataDefinitionName))
                    {
                        String defPath = Path.Combine(directory, "Plugin.ddp");
                        if (File.Exists(defPath))
                        {
                            doPublish(defPath);
                        }
                        else
                        {
                            defPath = Path.Combine(directory, "Dependency.ddd");
                            if (File.Exists(defPath))
                            {
                                doPublish(defPath);
                            }
                        }
                    }
                }
                DeveloperConfig.LastPluginExportDirectory = outDirEdit.OnlyText;
                DeveloperConfig.LastPluginKey = signatureFileEdit.OnlyText;
                DeveloperConfig.LastPluginCounterSignatureKey = counterSignatureFileEdit.OnlyText;
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error publishing plugin {0}.\nReason: {1}", pluginFileEdit.OnlyText, ex.Message), "Publish Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        void doPublish(String dataDefinitionPath)
        {
            try
            {
                dataPublishController.publishDataFile(dataDefinitionPath, signatureFileEdit.OnlyText, certPasswordEdit.OnlyText, counterSignatureFileEdit.OnlyText, counterSignaturePassword.OnlyText, outDirEdit.OnlyText);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error publishing plugin {0}.\nReason: {1}", pluginFileEdit.OnlyText, ex.Message), "Publish Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        void outDirBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            DirDialog dirDialog = new DirDialog(MainWindow.Instance, "Select an out directory", MedicalConfig.UserDocRoot);
            dirDialog.showModal((result, path) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    outDirEdit.Caption = path;
                }
            });
        }

        void signatureFileBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Select a signature file", MedicalConfig.UserDocRoot, "", SIGNATURE_WILDCARD, false);
            openDialog.showModal((result, paths) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    signatureFileEdit.Caption = paths.First();
                }
            });
        }

        void counterSignatureFileBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Select a counter signature file", MedicalConfig.UserDocRoot, "", SIGNATURE_WILDCARD, false);
            openDialog.showModal((result, paths) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    counterSignatureFileEdit.Caption = paths.First();
                }
            });
        }

        void pluginFileBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Select a plugin file", MedicalConfig.UserDocRoot, "", PLUGIN_WILDCARD, false);
            openDialog.showModal((result, dlg) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    pluginFileEdit.Caption = dlg.First();
                }
            });
        }
    }
}
