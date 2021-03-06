﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Anomalous.GuiFramework;

namespace Medical
{
    class UnknownDocumentHandler : DocumentHandler
    {
        private EditorController editorController;

        public UnknownDocumentHandler(EditorController editorController)
        {
            this.editorController = editorController;
        }

        public bool canReadFile(string filename)
        {
            return Directory.Exists(filename);
        }

        public bool processFile(string filename)
        {
            editorController.openProject(filename);
            return true;
        }

        public string getPrettyName(string filename)
        {
            return "Unknown Editor Projects";
        }

        public string getIcon(string filename)
        {
            return "EditorIcons.EditorTools";
        }
    }
}
