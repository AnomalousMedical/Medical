using Engine.Editing;
using Logging;
using Medical;
using Medical.Controller;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lecture
{
    public class LectureUICallback : EditorUICallback
    {
        private String currentDirectory = "";

        public LectureUICallback(StandaloneController standaloneController, EditorController editorController, PropEditController propEditController)
            : base(standaloneController, editorController, propEditController)
        {

        }

        public override Browser createFileBrowser(String searchPattern, String prompt)
        {
            int dirLength = GetDirLength();
            Browser browser = new Browser("Files", prompt);
            if (editorController.ResourceProvider != null)
            {
                foreach (String timeline in thumbFilter(editorController.ResourceProvider.listFiles(searchPattern, currentDirectory, true)))
                {
                    browser.addNode("", null, new BrowserNode(timeline.Substring(dirLength), timeline));
                }
            }
            else
            {
                Log.Warning("No resources loaded.");
            }
            return browser;
        }

        public override Browser createFileBrowser(IEnumerable<string> searchPatterns, string prompt)
        {
            int dirLength = GetDirLength();
            Browser browser = new Browser("Files", prompt);
            if (editorController.ResourceProvider != null)
            {
                foreach (String searchPattern in searchPatterns)
                {
                    foreach (String file in thumbFilter(editorController.ResourceProvider.listFiles(searchPattern, currentDirectory, true)))
                    {
                        browser.addNode("", null, new BrowserNode(file.Substring(dirLength), file));
                    }
                }
            }
            else
            {
                Log.Warning("No resources loaded.");
            }
            return browser;
        }

        public override Browser createFileBrowser(IEnumerable<string> searchPatterns, String prompt, String leadingPath)
        {
            int dirLength = GetDirLength();
            Browser browser = new Browser("Files", prompt);
            if (editorController.ResourceProvider != null)
            {
                foreach (String searchPattern in searchPatterns)
                {
                    foreach (String file in thumbFilter(editorController.ResourceProvider.listFiles(searchPattern, currentDirectory, true)))
                    {
                        browser.addNode("", null, new BrowserNode(file.Substring(dirLength), Path.Combine(leadingPath, file)));
                    }
                }
            }
            else
            {
                Log.Warning("No resources loaded.");
            }
            return browser;
        }

        public String CurrentDirectory
        {
            get
            {
                return currentDirectory;
            }
            set
            {
                currentDirectory = value;
                if (value == null)
                {
                    currentDirectory = "";
                }
            }
        }

        private int GetDirLength()
        {
            int dirLength = currentDirectory.Length;
            if (currentDirectory != "")
            {
                ++dirLength;
            }
            return dirLength;
        }

        IEnumerable<String> thumbFilter(IEnumerable<String> files)
        {
            foreach (String file in files)
            {
                if (!Path.GetFileName(file).Equals("thumb.png", StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        public override void showBrowser<T>(Browser browser, SendResult<T> resultCallback)
        {
            switch (browser.Hint)
            {
                case Browser.DisplayHint.Images:
                    ImageBrowserWindow<T>.GetInput(browser, true, resultCallback, editorController.ResourceProvider, (importFile, finishedCallback) =>
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem((stateInfo) =>
                        {
                            try
                            {
                                String writePath = Path.Combine(CurrentDirectory, Guid.NewGuid().ToString("D") + Path.GetExtension(importFile));
                                using (Stream writeStream = editorController.ResourceProvider.openWriteStream(writePath))
                                {
                                    using (Stream readStream = File.Open(importFile, FileMode.Open, FileAccess.Read))
                                    {
                                        readStream.CopyTo(writeStream);
                                    }
                                }
                                ThreadManager.invoke(new Action(() =>
                                {
                                    finishedCallback(writePath);
                                }));
                            }
                            catch (Exception ex)
                            {
                                ThreadManager.invoke(() =>
                                {
                                    MessageBox.show(String.Format("Error copying file {0} to your project.\nReason: {1}", importFile, ex.Message), "Image Copy Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                                });
                            }
                        });
                    });
                    break;
                default:
                    base.showBrowser<T>(browser, resultCallback);
                    break;
            }

        }
    }
}
