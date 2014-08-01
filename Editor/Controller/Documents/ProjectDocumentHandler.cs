using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    class ProjectDocumentHandler : DocumentHandler
    {
        private EditorController editorController;

        public ProjectDocumentHandler(EditorController editorController)
        {
            this.editorController = editorController;
        }

        public bool canReadFile(string filename)
        {
            return Directory.Exists(filename) && 
                (Directory.EnumerateFiles(filename, "*.mvc", SearchOption.AllDirectories).FirstOrDefault() != null ||
                Directory.EnumerateFiles(filename, "*.ddd", SearchOption.AllDirectories).FirstOrDefault() != null);
        }

        public bool processFile(string filename)
        {
            editorController.openProject(filename);
            return true;
        }

        public string getPrettyName(string filename)
        {
            return "Editor Projects";
        }

        public string getIcon(string filename)
        {
            return "EditorIcons.EditorTools";
        }
    }
}
