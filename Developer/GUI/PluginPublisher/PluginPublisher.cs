using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using MyGUIPlugin;
using Medical;

namespace Developer.GUI
{
    class PluginPublisher : MDIDialog
    {
        private const String PLUGIN_WILDCARD = "Data Driven Plugin (*.ddp)|*.ddp";
        private const String SIGNATURE_WILDCARD = "Signature File (*.xml)|*.xml";

        private EditBox pluginFileEdit;
        private EditBox signatureFileEdit;
        private EditBox outDirEdit;

        private PluginPublishController pluginPublishController;

        public PluginPublisher(PluginPublishController pluginPublishController)
            :base("Developer.GUI.PluginPublisher.PluginPublisher.layout")
        {
            this.pluginPublishController = pluginPublishController;

            pluginFileEdit = (EditBox)window.findWidget("PluginFileEdit");
            signatureFileEdit = (EditBox)window.findWidget("SignatureFileEdit");
            signatureFileEdit.Caption = DeveloperConfig.LastPluginKey;
            outDirEdit = (EditBox)window.findWidget("OutDirEdit");
            outDirEdit.Caption = DeveloperConfig.LastPluginExportDirectory;

            Button pluginFileBrowser = (Button)window.findWidget("PluginFileBrowser");
            pluginFileBrowser.MouseButtonClick += new MyGUIEvent(pluginFileBrowser_MouseButtonClick);

            Button signatureFileBrowser = (Button)window.findWidget("SignatureFileBrowser");
            signatureFileBrowser.MouseButtonClick += new MyGUIEvent(signatureFileBrowser_MouseButtonClick);

            Button outDirBrowser = (Button)window.findWidget("OutDirBrowser");
            outDirBrowser.MouseButtonClick += new MyGUIEvent(outDirBrowser_MouseButtonClick);

            Button publishButton = (Button)window.findWidget("PublishButton");
            publishButton.MouseButtonClick += new MyGUIEvent(publishButton_MouseButtonClick);
        }

        void publishButton_MouseButtonClick(Widget source, EventArgs e)
        {
            pluginPublishController.publishPlugin(pluginFileEdit.OnlyText, signatureFileEdit.OnlyText, outDirEdit.OnlyText);
            DeveloperConfig.LastPluginExportDirectory = outDirEdit.OnlyText;
            DeveloperConfig.LastPluginKey = signatureFileEdit.OnlyText;
        }

        void outDirBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            using (DirDialog dirDialog = new DirDialog(MainWindow.Instance, "Select an out directory", MedicalConfig.UserDocRoot))
            {
                if (dirDialog.showModal() == NativeDialogResult.OK)
                {
                    outDirEdit.Caption = dirDialog.Path;
                }
            }
        }

        void signatureFileBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            using (FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Select a signature file", MedicalConfig.UserDocRoot, "", SIGNATURE_WILDCARD, false))
            {
                if (openDialog.showModal() == NativeDialogResult.OK)
                {
                    signatureFileEdit.Caption = openDialog.Path;
                }
            }
        }

        void pluginFileBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            using (FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Select a plugin file", MedicalConfig.UserDocRoot, "", PLUGIN_WILDCARD, false))
            {
                if (openDialog.showModal() == NativeDialogResult.OK)
                {
                    pluginFileEdit.Caption = openDialog.Path;
                }
            }
        }
    }
}
