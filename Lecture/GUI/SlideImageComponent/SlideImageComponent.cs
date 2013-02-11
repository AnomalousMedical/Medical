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
        Int32NumericEdit sizeEdit;
        IntSize2 currentImageSize;

        const bool Key = false;
        private bool NotDisposed = true;

        public SlideImageComponent(EditorResourceProvider resourceProvider, String subdirectory, String currentImageName)
            : base("Lecture.GUI.SlideImageComponent.SlideImageComponent.layout", "SlideImage")
        {
            this.resourceProvider = resourceProvider;
            this.subdirectory = subdirectory;

            Button browseButton = (Button)widget.findWidget("Browse");
            browseButton.MouseButtonClick += browseButton_MouseButtonClick;

            imagePreview = (ImageBox)widget.findWidget("Image");
            imagePanel = widget.findWidget("ImagePanel");
            imageAtlas = new ImageAtlas("SlideImageComponentAtlas_" + Guid.NewGuid().ToString("D"), new IntSize2(imagePreview.Width, imagePreview.Height));
            sizeEdit = new Int32NumericEdit((EditBox)widget.findWidget("Size"))
            {
                MaxValue = 10000,
                MinValue = 0,
                Increment = 10,
                Value = 100
            };
            sizeEdit.ValueChanged += sizeEdit_ValueChanged;

            keyTimer = new Timer(UPDATE_DELAY);
            keyTimer.SynchronizingObject = new ThreadManagerSynchronizeInvoke();
            keyTimer.AutoReset = false;
            keyTimer.Elapsed += keyTimer_Elapsed;

            if (currentImageName != null)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(o =>
                {
                    openImageBGThread(Path.Combine(subdirectory, currentImageName));
                });
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
                    currentImageSize = new IntSize2(image.Width, image.Height);
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
                                widget.findWidget("LoadLabel").Visible = false;
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

            }
        }

        internal bool applyToElement(Element element)
        {
            element.SetAttribute("src", imageName);
            float scale = sizeEdit.Value / 100f;
            element.SetAttribute("width", currentImageSize.Width * scale + "pf");
            element.SetAttribute("height", currentImageSize.Height * scale + "pf");
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
    }
}
