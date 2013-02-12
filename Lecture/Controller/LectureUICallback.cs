using Engine.Editing;
using Logging;
using Medical;
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
                foreach (String timeline in editorController.ResourceProvider.listFiles(searchPattern, currentDirectory, true))
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
                    foreach (String file in editorController.ResourceProvider.listFiles(searchPattern, currentDirectory, true))
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
                    foreach (String file in editorController.ResourceProvider.listFiles(searchPattern, currentDirectory, true))
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
    }
}
