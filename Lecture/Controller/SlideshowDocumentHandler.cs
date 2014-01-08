using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Medical;

namespace Lecture
{
    class SlideshowDocumentHandler : DocumentHandler
    {
        private EditorController editorController;

        public SlideshowDocumentHandler(EditorController editorController)
        {
            this.editorController = editorController;
        }

        public bool canReadFile(string filename)
        {
            try
            {
                if (Directory.Exists(filename) && Directory.EnumerateFiles(filename, "*.show", SearchOption.TopDirectoryOnly).FirstOrDefault() != null)
                {
                    return true;
                }
                String ext = Path.GetExtension(filename);
                return (ext.Equals(".sl", StringComparison.InvariantCultureIgnoreCase) || ext.Equals(".show", StringComparison.InvariantCultureIgnoreCase)) && File.Exists(filename);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool processFile(string filename)
        {
            if (filename.EndsWith(".show", StringComparison.InvariantCultureIgnoreCase)) //Legacy show file loading
            {
                filename = Path.GetDirectoryName(filename);
            }
            editorController.openProject(filename);
            return true;
        }

        public string getPrettyName(string filename)
        {
            return "Smart Lectures";
        }

        public string getIcon(string filename)
        {
            return "Lecture.Icon.SmartLectureIcon";
        }
    }
}
