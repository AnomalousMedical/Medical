using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;
using System.IO;
using System.Drawing;
using Medical.Controller;

namespace Medical.GUI
{
    public class ImageBrowserWindow<BrowseType> : Dialog
    {
        private SendResult<BrowseType> SendResult;

        private SingleSelectButtonGrid imageGrid;
        private BrowserImageManager imageManager;

        public ImageBrowserWindow(String message, ResourceProvider resourceProvider)
            :base("Medical.GUI.Editor.ImageBrowserWindow.layout")
        {
            imageGrid = new SingleSelectButtonGrid((ScrollView)window.findWidget("ScrollView"));
            imageGrid.ItemActivated += imageGrid_ItemActivated;
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
            window.Caption = message;

            Button selectButton = (Button)window.findWidget("Select");
            selectButton.MouseButtonClick += new MyGUIEvent(selectButton_MouseButtonClick);
            Button cancelButton = (Button)window.findWidget("Cancel");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Accepted = false;

            imageManager = new BrowserImageManager(resourceProvider);
        }

        public override void Dispose()
        {
            imageManager.Dispose();
            base.Dispose();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            imageGrid.layout();
        }

        public void setBrowser(Browser browser)
        {
            imageGrid.clear();
            addNodes(browser.getTopNode(), browser.DefaultSelection);
        }

        private void addNodes(BrowserNode node, BrowserNode defaultNode)
        {
            if (node.Value != null)
            {
                ButtonGridItem item = imageGrid.addItem("", node.Text);
                item.UserObject = node.Value;
                imageManager.loadThumbnail(node.Value.ToString(), (imageKey, size) =>
                {
                    item.setImage(imageKey);
                    item.setImageSize(size.Width, size.Height);
                });
                if (node == defaultNode)
                {
                    imageGrid.SelectedItem = item;
                }
            }
            foreach (BrowserNode child in node.getChildIterator())
            {
                addNodes(child, defaultNode);
            }
        }

        /// <summary>
        /// The value that is selected on the tree. Can be null.
        /// </summary>
        public BrowseType SelectedValue
        {
            get
            {
                if (imageGrid.SelectedItem != null)
                {
                    return (BrowseType)imageGrid.SelectedItem.UserObject;
                }
                return default(BrowseType);
            }
        }

        public bool Accepted { get; set; }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
        }

        void selectButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (SelectedValue != null)
            {
                Accepted = true;
                close();
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Accepted = false;
            close();
        }

        void imageGrid_ItemActivated(object sender, EventArgs e)
        {
            if (SelectedValue != null)
            {
                Accepted = true;
                close();
            }
        }

        public static void GetInput(Browser browser, bool modal, SendResult<BrowseType> sendResult, ResourceProvider resourceProvider)
        {
            ImageBrowserWindow<BrowseType> inputBox = new ImageBrowserWindow<BrowseType>(browser.Prompt, resourceProvider);
            inputBox.setBrowser(browser);
            inputBox.SendResult = sendResult;
            inputBox.Closing += new EventHandler<DialogCancelEventArgs>(inputBox_Closing);
            inputBox.Closed += new EventHandler(inputBox_Closed);
            inputBox.center();
            inputBox.open(modal);
        }

        static void inputBox_Closing(object sender, DialogCancelEventArgs e)
        {
            ImageBrowserWindow<BrowseType> inputBox = (ImageBrowserWindow<BrowseType>)sender;
            String errorPrompt = null;
            if (inputBox.Accepted && !inputBox.SendResult(inputBox.SelectedValue, ref errorPrompt))
            {
                MessageBox.show(errorPrompt, "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                e.Cancel = true;
            }
        }

        private static void inputBox_Closed(object sender, EventArgs e)
        {
            ImageBrowserWindow<BrowseType> inputBox = (ImageBrowserWindow<BrowseType>)sender;
            inputBox.Dispose();
        }
    }

    class BrowserImageManager : IDisposable
    {
        private static WorkQueue workQueue = new WorkQueue();

        public const int ThumbWidth = 100;
        public const int ThumbHeight = 100;
        private ImageAtlas imageAtlas = new ImageAtlas("ImageBrowserThumbs" + Guid.NewGuid().ToString("D"), new IntSize2(ThumbWidth, ThumbHeight));
        private ResourceProvider resourceProvider;
        bool disposed = false;
        private Dictionary<String, IntSize2> sizes = new Dictionary<string, IntSize2>();

        public BrowserImageManager(ResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
            imageAtlas.ResizeMode = ImageResizeMode.KeepAspect;
        }

        public void Dispose()
        {
            imageAtlas.Dispose();
            disposed = true;
        }

        public void loadThumbnail(String file, Action<String, IntSize2> loadedCallback)
        {
            String id = imageAtlas.getImageId(file);
            if (id != null)
            {
                loadedCallback(id, sizes[file]);
            }
            else
            {
                workQueue.enqueue(() =>
                {
                    String thumbPath = file;
                    try
                    {
                        if (resourceProvider.exists(thumbPath))
                        {
                            using (Stream stream = resourceProvider.openFile(thumbPath))
                            {
                                Image thumb = Bitmap.FromStream(stream);
                                ThreadManager.invoke(new Action(() =>
                                {
                                    try
                                    {
                                        if (!disposed)
                                        {
                                            if (!imageAtlas.containsImage(file))
                                            {
                                                IntSize2 size;
                                                String imageKey = imageAtlas.addImage(file, thumb, out size);
                                                sizes.Add(imageKey, size);
                                                loadedCallback(imageKey, size);
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        thumb.Dispose();
                                    }
                                }));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.Error("Could not load thumbnail because of {0} exception.\nReason: {1}", ex.GetType(), ex.Message);
                    }
                });
            }
        }
    }
}
