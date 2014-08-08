using Engine;
using Engine.Editing;
using Medical;
using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture
{
    class SlideshowStyleManager
    {
        public static readonly int Width = ScaleHelper.Scaled(280);
        public static readonly int Height = ScaleHelper.Scaled(350);

        private SlideshowEditController editorController;
        private EditInterface editInterface;
        private List<Pair<String, String>> unprocessedFiles = new List<Pair<String, String>>();
        private LectureUICallback uiCallback;

        public SlideshowStyleManager(SlideshowEditController editorController, LectureUICallback uiCallback)
        {
            this.editorController = editorController;
            this.uiCallback = uiCallback;
            editInterface = new EditInterface("Theme");
            ReflectedEditInterface.expandEditInterface(this, ReflectedEditInterface.DefaultScanner, editInterface);
        }

        public void showEditor(int left, int top)
        {
            foreach (var file in unprocessedFiles)
            {
                readFile(file.First, file.Second);
            }
            unprocessedFiles.Clear();
            
            PopupGenericEditor.openEditor(editInterface, uiCallback, left, top, Width, Height);
        }

        public void addStyleFile(String file, String name)
        {
            unprocessedFiles.Add(new Pair<string, string>(file, name));
        }

        private void readFile(String file, String name)
        {
            String css = editorController.ResourceProvider.readFileAsString(file);
            SlideshowStyle style = new SlideshowStyle(name, css);
            style.Changed += (arg) =>
            {
                StringBuilder styleString = new StringBuilder(500);
                ((SlideshowStyle)arg).buildStyleSheet(styleString);
                editorController.ResourceProvider.ResourceCache.add(new ResourceProviderTextCachedResource(file, Encoding.UTF8, styleString.ToString(), editorController.ResourceProvider));
                editorController.refreshRmlAndThumbnail();
            };
            editInterface.addSubInterface(style.getEditInterface());
        }

        [Editable(Advanced=true)]
        public bool VectorMode
        {
            get
            {
                return editorController.VectorMode;
            }
            set
            {
                editorController.VectorMode = value;
            }
        }
    }
}
