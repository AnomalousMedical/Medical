﻿using Engine.Editing;
using Engine.Threads;
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
    public class LectureUICallback : CommonUICallback
    {
        private String currentDirectory = "";

        public LectureUICallback(StandaloneController standaloneController, EditorController editorController, PropEditController propEditController)
            : base(standaloneController, editorController, propEditController)
        {
            addCustomQuery<String, String>(PlaySoundAction.CustomQueries.Record, (queryResult, soundFile) =>
            {
                String finalSoundFile = Path.Combine(CurrentDirectory, Guid.NewGuid().ToString("D") + ".ogg");
                String error = null;
                QuickSoundRecorder.ShowDialog(standaloneController.MedicalController, finalSoundFile, editorController.ResourceProvider.openWriteStream,
                newSoundFile =>
                {
                    queryResult.Invoke(newSoundFile, ref error);
                });
            });
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
                        browser.addNode(null, null, new BrowserNode(file.Substring(dirLength), file));
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
                    ImageBrowserWindow<T>.GetInput(browser, true, resultCallback, editorController.ResourceProvider, importFile);
                    break;
                case Browser.DisplayHint.Sounds:
                    BrowserWindow<T>.GetInput(browser, true, resultCallback, importFile, "Choose Sound", "Ogg Vorbis Files (.ogg)|*.ogg", ".ogg");
                    break;
                default:
                    base.showBrowser<T>(browser, resultCallback);
                    break;
            }

        }

        private void importFile(String importFile, Action<String> finishedCallback)
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
                    finishedCallback(writePath);
                }
                catch (Exception ex)
                {
                    ThreadManager.invoke(() =>
                    {
                        MessageBox.show(String.Format("Error copying file {0} to your project.\nReason: {1}", importFile, ex.Message), "Import Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                    });
                }
            });
        }
    }
}
