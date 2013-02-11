using Engine;
using libRocketPlugin;
using Medical;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lecture.GUI
{
    class SlideImageComponent : ElementEditorComponent
    {
        ImageBox imagePreview;
        ImageAtlas imageAtlas;
        EditorResourceProvider resourceProvider;
        String subdirectory;

        const bool Key = false;

        public SlideImageComponent(EditorResourceProvider resourceProvider, String subdirectory)
            : base("Lecture.GUI.SlideImageComponent.SlideImageComponent.layout", "SlideImage")
        {
            this.resourceProvider = resourceProvider;
            this.subdirectory = subdirectory;

            Button browseButton = (Button)widget.findWidget("Browse");
            browseButton.MouseButtonClick += browseButton_MouseButtonClick;

            imagePreview = (ImageBox)widget.findWidget("Image");
            imageAtlas = new ImageAtlas("SlideImageComponentAtlas", new Size2(imagePreview.Width, imagePreview.Height));
        }

        public override void Dispose()
        {
            base.Dispose();
            imageAtlas.Dispose();
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Choose an image", wildcard: "Portable Network Graphics (*.png)|*.png");
            openDialog.showModal((result, paths) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    if (imageAtlas.containsImage(Key))
                    {
                        imagePreview.setImageTexture(null);
                        imageAtlas.removeImage(Key);
                    }
                    String path = paths.First();
                    String filename = Guid.NewGuid().ToString("D") + Path.GetExtension(path);
                    File.Copy(path, Path.Combine(resourceProvider.BackingLocation, subdirectory, filename));
                }
            });
        }

        internal bool applyToElement(Element element)
        {
            return false;
        }
    }
}
