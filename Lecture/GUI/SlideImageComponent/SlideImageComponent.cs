using Engine;
using libRocketPlugin;
using Medical;
using Medical.Controller;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lecture.GUI
{
    class SlideImageComponent : ElementEditorComponent
    {
        ImageBox imagePreview;
        ImageAtlas imageAtlas;
        EditorResourceProvider resourceProvider;
        String subdirectory;
        String imageName;
        Widget imagePanel;

        const bool Key = false;

        public SlideImageComponent(EditorResourceProvider resourceProvider, String subdirectory, String currentImageName)
            : base("Lecture.GUI.SlideImageComponent.SlideImageComponent.layout", "SlideImage")
        {
            this.resourceProvider = resourceProvider;
            this.subdirectory = subdirectory;

            Button browseButton = (Button)widget.findWidget("Browse");
            browseButton.MouseButtonClick += browseButton_MouseButtonClick;

            imagePreview = (ImageBox)widget.findWidget("Image");
            imagePanel = widget.findWidget("ImagePanel");
            imageAtlas = new ImageAtlas("SlideImageComponentAtlas", new IntSize2(imagePreview.Width, imagePreview.Height));

            if (currentImageName != null)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    openImageBGThread(Path.Combine(subdirectory, currentImageName));
                });
            }
            imageName = currentImageName;
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
                        imagePreview.setItemResource(null);
                        imageAtlas.removeImage(Key);
                    }
                    String path = paths.First();
                    imageName = Guid.NewGuid().ToString("D") + Path.GetExtension(path);
                    String filename = Path.Combine(subdirectory, imageName);

                    ThreadPool.QueueUserWorkItem((stateInfo) =>
                        {
                            try
                            {
                                using (Stream writeStream = resourceProvider.openWriteStream(filename))
                                {
                                    using (Stream readStream = File.Open(path, FileMode.Open, FileAccess.Read))
                                    {
                                        readStream.CopyTo(writeStream);
                                    }
                                }
                                openImageBGThread(filename);
                            }
                            catch (Exception ex)
                            {
                                ThreadManager.invoke(() =>
                                {
                                    MessageBox.show(String.Format("Error copying file {0} to your project.\nReason: {1}", path, ex.Message), "Image Copy Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                                });
                            }
                        });
                }
            });
        }

        private void openImageBGThread(String filename)
        {
            try
            {
                using (Stream imageStream = resourceProvider.openFile(filename))
                {
                    Image image = Bitmap.FromStream(imageStream);
                    int width = 34;
                    int height = 10;
                    float aspect = 4f / 3f;
                    int left = 0;
                    int top = 0;
                    if (image.Width != 0 && image.Width > image.Height)
                    {
                        aspect = (float)image.Height / image.Width;
                        height = (int)((float)imagePanel.Width * aspect);
                        if (height < imagePanel.Height)
                        {
                            width = imagePanel.Width;
                            top = (imagePanel.Height - height) / 2;
                        }
                        else
                        {
                            aspect = (float)image.Width / image.Height;
                            height = imagePanel.Height;
                            width = (int)((float)imagePanel.Height * aspect);
                            left = (imagePanel.Width - width) / 2;
                        }
                    }
                    ThreadManager.invoke(() =>
                    {
                        try
                        {
                            Logging.Log.Debug("Size {0}, {1}, {2}, {3}", left, top, width, height);
                            imagePreview.setPosition(left, top);
                            imagePreview.setSize(width, height);
                            imageAtlas.ImageSize = new IntSize2(width, height);
                            String imageKey = imageAtlas.addImage(Key, image);
                            imagePreview.setItemResource(imageKey);
                        }
                        finally
                        {
                            image.Dispose();
                        }
                        this.fireChangesMade();
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }

        internal bool applyToElement(Element element)
        {
            element.SetAttribute("src", imageName);
            return true;
        }
    }
}
