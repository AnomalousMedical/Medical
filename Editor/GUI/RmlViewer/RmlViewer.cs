using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using MyGUIPlugin;
using Engine;
using libRocketPlugin;
using Medical.Controller;
using Logging;
using Medical;
using OgreWrapper;

namespace Medical.GUI
{
    public class RmlViewer : MDIDialog
    {
        private RocketWidget rocketWidget;
        private String documentName = null;
        private FileSystemWatcher fileWatcher;
        private String windowTitleBase;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();
        private EditorController editorController;

        public RmlViewer(EditorController editorController)
            : base("Medical.GUI.RmlViewer.RmlViewer.layout")
        {
            this.editorController = editorController;

            ImageBox imageBox = (ImageBox)window.findWidget("RocketImage");
            rocketWidget = new RocketWidget(imageBox);
            rocketWidget.Enabled = false;

            this.Resized += new EventHandler(TestRocketWindow_Resized);
            window.RootKeyChangeFocus += new MyGUIEvent(window_RootKeyChangeFocus);

            extensionActions.Add(new ExtensionAction("Open System Editor", "Edit", openInSystemEditor));

            windowTitleBase = window.Caption;
        }

        public override void Dispose()
        {
            if (fileWatcher != null)
            {
                fileWatcher.Dispose();
            }
            rocketWidget.Dispose();
            base.Dispose();
        }

        public void changeDocument(string file)
        {
            if (!File.Exists(file))
            {
                MessageBox.show(String.Format("The file {0} does not exist. Would you like to create it?", file), "Create File", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, 
                delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        using (StreamWriter sw = new StreamWriter(file))
                        {
                            sw.Write(defaultRml);
                        }
                        documentName = file;
                        loadDocument();
                    }
                });
            }
            else
            {
                documentName = file;
                loadDocument();
            }
        }

        public void openInSystemEditor()
        {
            if (documentName != null)
            {
                OtherProcessManager.openLocalURL(documentName);
            }
        }

        public void activateExtensionActions()
        {
            editorController.ExtensionActions = extensionActions;
        }

        protected override void onClosed(EventArgs args)
        {
            rocketWidget.Enabled = false;
            base.onClosed(args);
        }

        protected override void onShown(EventArgs args)
        {
            rocketWidget.Enabled = true;
            base.onShown(args);
        }

        void window_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfea = (RootFocusEventArgs)e;
            if (rfea.Focus)
            {
                activateExtensionActions();
            }
        }

        void TestRocketWindow_Resized(object sender, EventArgs e)
        {
            rocketWidget.resized();
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ThreadManager.invoke(new Action(delegate()
            {
                loadDocument();
            }));
        }

        private void loadDocument()
        {
            try
            {
                RocketOgreTextureManager.refreshTextures();

                window.Caption = String.Format("{0} - '{1}'  {2}", windowTitleBase, Path.GetFileName(documentName), Path.GetDirectoryName(documentName));

                Factory.ClearStyleSheetCache();
                rocketWidget.Context.UnloadAllDocuments();
                using (ElementDocument document = rocketWidget.Context.LoadDocument(documentName))
                {
                    if (document != null)
                    {
                        document.Show();
                        rocketWidget.removeFocus();
                    }
                }

                if (fileWatcher != null)
                {
                    fileWatcher.Dispose();
                    fileWatcher = null;
                }

                String realLocation = documentName;

                if (VirtualFileSystem.Instance.exists(realLocation))
                {
                    VirtualFileInfo fileInfo = VirtualFileSystem.Instance.getFileInfo(documentName);
                    realLocation = fileInfo.RealLocation;
                }
                else
                {

                }
                if (File.Exists(realLocation))
                {
                    fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(realLocation));
                    fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
                    fileWatcher.EnableRaisingEvents = true;
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Could not load file watcher for {0} because {1}", documentName, ex.Message);
            }
        }

        private const String defaultRml = @"<rml>
  <head>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.rkt.rcss""/>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.Anomalous.rcss""/>
  </head>
  <body>
    <div class=""ScrollArea"">
      <h1>Empty Rml View</h1>
      <p>You can start creating your Rml View here. You can erase this text to start.</p>
    </div>
  </body>
</rml>
";
    }
}
