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
using System.Timers;

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
        Widget loadingLabel;

        const bool Key = false;
        private bool NotDisposed = true;

        public SlideImageComponent(EditorResourceProvider resourceProvider, String subdirectory, String currentImageName)
            : base("Lecture.GUI.SlideImageComponent.SlideImageComponent.layout", "Image")
        {
            this.resourceProvider = resourceProvider;
            this.subdirectory = subdirectory;

            Button browseButton = (Button)widget.findWidget("Browse");
            browseButton.MouseButtonClick += browseButton_MouseButtonClick;

            imagePreview = (ImageBox)widget.findWidget("Image");
            imagePanel = widget.findWidget("ImagePanel");
            imageAtlas = new ImageAtlas("SlideImageComponentAtlas_" + Guid.NewGuid().ToString("D"), new IntSize2(imagePreview.Width, imagePreview.Height));

            loadingLabel = widget.findWidget("LoadLabel");

            if (currentImageName != null)
            {
                String loadPath = Path.Combine(subdirectory, currentImageName);
                if (resourceProvider.exists(loadPath))
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(o =>
                    {
                        openImageBGThread(loadPath, false);
                    });
                }
                else
                {
                    loadingLabel.Visible = false;
                }
            }
            else
            {
                loadingLabel.Visible = false;
            }
            imageName = currentImageName;
        }

        public override void Dispose()
        {
            NotDisposed = false;
            base.Dispose();
            imageAtlas.Dispose();
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Choose an image", wildcard: "Images|*.jpg;*.jpeg;*.png");
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
                    String extension = Path.GetExtension(path);
                    if (extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase) || extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) || extension.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase))
                    {
                        imageName = Guid.NewGuid().ToString("D") + extension;
                        String filename = Path.Combine(subdirectory, imageName);

                        System.Threading.ThreadPool.QueueUserWorkItem((stateInfo) =>
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
                                openImageBGThread(filename, true);
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
                    else
                    {
                        MessageBox.show(String.Format("Cannot open a file with extension '{0}'. Please choose a file that is a Png Image (.png) or a Jpeg (.jpg or .jpeg).", extension), "Can't Load Image", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                    }
                }
            });
        }

        private void openImageBGThread(String filename, bool applyUpdate)
        {
            try
            {
                using (Stream imageStream = resourceProvider.openFile(filename))
                {
                    Image image = Bitmap.FromStream(imageStream);

                    int left = 0;
                    int top = 0;
                    int width = 8;
                    float aspect = (float)image.Height / image.Width;
                    int height = (int)((float)imagePanel.Width * aspect);
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
                    ThreadManager.invoke(() =>
                    {
                        try
                        {
                            if (NotDisposed)
                            {
                                imagePreview.setPosition(left, top);
                                imagePreview.setSize(width, height);
                                imageAtlas.ImageSize = new IntSize2(width, height);
                                String imageKey = imageAtlas.addImage(Key, image);
                                imagePreview.setItemResource(imageKey);
                                loadingLabel.Visible = false;
                                if (applyUpdate)
                                {
                                    this.fireChangesMade();
                                    this.fireApplyChanges();
                                }
                            }
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
                Logging.Log.Error("Could not load image '{0}'. Reason: {1}", filename, ex.Message);
            }
        }

        internal bool applyToElement(Element element)
        {
            element.SetAttribute("src", imageName);
            return true;
        }

        public void applyChanges()
        {
            fireApplyChanges();
        }
    }
}
