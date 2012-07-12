using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Medical;

namespace PresentationEditor
{
    class PresentationDocumentHandler : DocumentHandler
    {
        private EditorController editorController;

        public PresentationDocumentHandler(EditorController editorController)
        {
            this.editorController = editorController;
        }

        public bool canReadFile(string filename)
        {
            return File.Exists(filename);
        }

        public bool processFile(string filename)
        {
            editorController.openProject(Path.GetDirectoryName(filename), filename);
            return true;
        }

        public string getPrettyName(string filename)
        {
            return "Presentation";
        }

        public string getIcon(string filename)
        {
            return "StandaloneIcons/NoIcon";
        }
    }
}
