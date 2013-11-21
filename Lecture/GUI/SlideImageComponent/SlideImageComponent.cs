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
        private const int UPDATE_DELAY = 250;

        private Timer keyTimer;

        ImageBox imagePreview;
        ImageAtlas imageAtlas;
        EditorResourceProvider resourceProvider;
        String subdirectory;
        String imageName;
        Widget imagePanel;
        Widget loadingLabel;
        Int32NumericEdit widthEdit;
        Int32NumericEdit heightEdit;
        IntSize2 realImageSize;

        const bool Key = false;
        private bool NotDisposed = true;

        IntSize2 currentSize;

        public SlideImageComponent(EditorResourceProvider resourceProvider, String subdirectory, String currentImageName, IntSize2 currentSize)
            : base("Lecture.GUI.SlideImageComponent.SlideImageComponent.layout", "SlideImage")
        {
            this.resourceProvider = resourceProvider;
            this.subdirectory = subdirectory;
            this.currentSize = currentSize;

            Button browseButton = (Button)widget.findWidget("Browse");
            browseButton.MouseButtonClick += browseButton_MouseButtonClick;

            imagePreview = (ImageBox)widget.findWidget("Image");
            imagePanel = widget.findWidget("ImagePanel");
            imageAtlas = new ImageAtlas("SlideImageComponentAtlas_" + Guid.NewGuid().ToString("D"), new IntSize2(imagePreview.Width, imagePreview.Height));
            widthEdit = new Int32NumericEdit((EditBox)widget.findWidget("Width"))
            {
                MaxValue = 10000,
                MinValue = 0,
                Increment = 10,
                Value = 100
            };
            widthEdit.ValueChanged += sizeEdit_ValueChanged;

            heightEdit = new Int32NumericEdit((EditBox)widget.findWidget("Height"))
            {
                MaxValue = 10000,
                MinValue = 0,
                Increment = 10,
                Value = 100
            };
            heightEdit.ValueChanged += sizeEdit_ValueChanged;

            keyTimer = new Timer(UPDATE_DELAY);
            keyTimer.SynchronizingObject = new ThreadManagerSynchronizeInvoke();
            keyTimer.AutoReset = false;
            keyTimer.Elapsed += keyTimer_Elapsed;

            loadingLabel = widget.findWidget("LoadLabel");

            if (currentImageName != null)
            {
                String loadPath = Path.Combine(subdirectory, currentImageName);
                if (resourceProvider.exists(loadPath))
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(o =>
                    {
                        openImageBGThread(loadPath, false, false);
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
            keyTimer.Dispose();
            base.Dispose();
            imageAtlas.Dispose();
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Choose an image", wildcard: "Images|*");
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
                                openImageBGThread(filename, true, true);
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

        private void openImageBGThread(String filename, bool applyUpdate, bool isNewImage)
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
                    realImageSize = new IntSize2(image.Width, image.Height);
                    ThreadManager.invoke(() =>
                    {
                        try
                        {
                            if (NotDisposed)
                            {
                                widthEdit.Value = currentSize.Width > 0 ? currentSize.Width : width;
                                heightEdit.Value = currentSize.Height > 0 ? currentSize.Height : height;
                                if (isNewImage)
                                {
                                    float ratio = height / (float)width;
                                    heightEdit.Value = (int)(widthEdit.Value * ratio);
                                }
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
            element.SetAttribute("width", widthEdit.Value.ToString());
            element.SetAttribute("height", heightEdit.Value.ToString());
            element.SetAttribute("scale", "true");
            return true;
        }

        void sizeEdit_ValueChanged(Widget source, EventArgs e)
        {
            this.fireChangesMade();
            keyTimer.Stop();
            keyTimer.Start();
        }

        void keyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            fireApplyChanges();
        }

        public void changeSize(IntSize2 newSize)
        {
            widthEdit.Value = newSize.Width;
            heightEdit.Value = newSize.Height;
            this.fireChangesMade();
        }

        public void applyChanges()
        {
            fireApplyChanges();
        }
    }
}
