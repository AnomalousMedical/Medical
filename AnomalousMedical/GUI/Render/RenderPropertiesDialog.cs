using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using System.Drawing;
using Engine;
using System.IO;
using System.Drawing.Imaging;

namespace Medical.GUI
{
    public class RenderPropertiesDialog : MDIDialog
    {
        private NumericEdit width;
        private NumericEdit height;

        private Edit imageName;
        private Edit outputFolder;
        private ComboBox imageFormat;

        private StaticImage previewImage;
        private Button fullSizeButton;

        private ButtonGroup licenseTypeGroup = new ButtonGroup();
        private Button personalButton;
        private Button commercialButton;
        private CheckButton agreeButton;
        private Button saveButton;

        private ResolutionMenu resolutionMenu;
        private SceneViewController sceneViewController;
        private ImageRenderer imageRenderer;

        private int previewMaxWidth;
        private int previewMaxHeight;

        private Bitmap currentImage = null;
        private ImageAtlas imageAtlas = null;

        private LicenseManager licenseManager;
        private NotificationGUIManager notificationManager;

        public RenderPropertiesDialog(SceneViewController sceneViewController, ImageRenderer imageRenderer, LicenseManager licenseManager, NotificationGUIManager notificationManager)
            : base("Medical.GUI.Render.RenderPropertiesDialog.layout")
        {
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;
            this.licenseManager = licenseManager;
            this.notificationManager = notificationManager;

            width = new NumericEdit(window.findWidget("RenderingTab/WidthEdit") as Edit);
            height = new NumericEdit(window.findWidget("RenderingTab/HeightEdit") as Edit);

            width.ValueChanged += new MyGUIEvent(renderSizeChanged);
            height.ValueChanged += new MyGUIEvent(renderSizeChanged);

            Button renderButton = window.findWidget("RenderingTab/Render") as Button;
            renderButton.MouseButtonClick += new MyGUIEvent(renderButton_MouseButtonClick);

            previewImage = (StaticImage)window.findWidget("PreviewImage");
            previewMaxWidth = previewImage.Width;
            previewMaxHeight = previewImage.Height;

            fullSizeButton = (Button)window.findWidget("FullSize");
            fullSizeButton.MouseButtonClick += new MyGUIEvent(fullSizeButton_MouseButtonClick);

            Button sizeButton = window.findWidget("RenderingTab/SizeButton") as Button;
            sizeButton.MouseButtonClick += new MyGUIEvent(sizeButton_MouseButtonClick);

            //ResolutionMenu
            resolutionMenu = new ResolutionMenu();
            resolutionMenu.ResolutionChanged += new EventHandler(resolutionMenu_ResolutionChanged);

            //Image save properties
            imageName = (Edit)window.findWidget("ImageName");
            imageName.Caption = "Anomalous Image";

            outputFolder = (Edit)window.findWidget("OutputFolder");
            outputFolder.Caption = MedicalConfig.ImageOutputFolder;
            Button outputBrowse = (Button)window.findWidget("OutputBrowse");
            outputBrowse.MouseButtonClick += new MyGUIEvent(outputBrowse_MouseButtonClick);

            imageFormat = (ComboBox)window.findWidget("ImageFormat");
            imageFormat.SelectedIndex = 0;

            //License controls
            personalButton = (Button)window.findWidget("Personal");
            licenseTypeGroup.addButton(personalButton);
            commercialButton = (Button)window.findWidget("Commercial");
            licenseTypeGroup.addButton(commercialButton);

            agreeButton = new CheckButton((Button)window.findWidget("Agree"));
            Button viewLicense = (Button)window.findWidget("ViewLicense");
            viewLicense.MouseButtonClick += new MyGUIEvent(viewLicense_MouseButtonClick);

            saveButton = (Button)window.findWidget("Save");
            saveButton.MouseButtonClick += new MyGUIEvent(saveButton_MouseButtonClick);

            Button openOutputFolder = (Button)window.findWidget("OpenOutputFolder");
            openOutputFolder.MouseButtonClick += new MyGUIEvent(openOutputFolder_MouseButtonClick);

            toggleRequireImagesWidgets();
        }

        public override void Dispose()
        {
            resolutionMenu.Dispose();
            base.Dispose();
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            sceneViewController.AutoAspectRatio = false;
            sceneViewController.AspectRatio = width.FloatValue / height.FloatValue;
        }

        protected override void onClosed(EventArgs args)
        {
            base.onClosed(args);
            sceneViewController.AutoAspectRatio = true;
            closeCurrentImage();
        }

        void renderSizeChanged(Widget source, EventArgs e)
        {
            sceneViewController.AspectRatio = width.FloatValue / height.FloatValue;
        }

        void fullSizeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (currentImage != null)
            {
                Bitmap bitmap = (Bitmap)currentImage.Clone();
                writeLicenseToImage(bitmap);
                imageRenderer.makeSampleImage(bitmap);
                String windowName = imageName.OnlyText;
                if (windowName == null)
                {
                    windowName = "";
                }

                String extension;
                ImageFormat imageOutputFormat;
                getImageFormat(out extension, out imageOutputFormat);
                ImageWindow window = new ImageWindow(MainWindow.Instance, windowName, bitmap, false, extension, imageOutputFormat, true);
            }
        }

        void renderButton_MouseButtonClick(Widget source, EventArgs e)
        {
            closeCurrentImage();
            SceneViewWindow drawingWindow = sceneViewController.ActiveWindow;
            if (drawingWindow != null)
            {
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = RenderWidth;
                imageProperties.Height = RenderHeight;
                imageProperties.UseWindowBackgroundColor = true;
                imageProperties.AntiAliasingMode = 8;
                currentImage = imageRenderer.renderImage(imageProperties);
                if (currentImage != null)
                {
                    int previewWidth = previewMaxWidth;
                    int previewHeight = previewMaxHeight;
                    if (currentImage.Width > currentImage.Height)
                    {
                        float ratio = (float)currentImage.Height / currentImage.Width;
                        previewHeight = (int)(previewWidth * ratio);
                    }
                    else
                    {
                        float ratio = (float)currentImage.Width / currentImage.Height;
                        previewWidth = (int)(previewHeight * ratio);
                    }
                    if (previewWidth > currentImage.Width || previewHeight > currentImage.Height)
                    {
                        previewWidth = currentImage.Width;
                        previewHeight = currentImage.Height;
                    }
                    imageAtlas = new ImageAtlas("RendererPreview", new Size2(previewWidth, previewHeight), new Size2(previewWidth, previewHeight));
                    String imageKey = imageAtlas.addImage("PreviewImage", currentImage);
                    previewImage.setSize(previewWidth, previewHeight);
                    previewImage.setItemResource(imageKey);

                }
                toggleRequireImagesWidgets();
            }
        }

        void sizeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            resolutionMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        void resolutionMenu_ResolutionChanged(object sender, EventArgs e)
        {
            width.Edit.Enabled = height.Edit.Enabled = resolutionMenu.IsCustom;
            RenderWidth = resolutionMenu.ImageWidth;
            RenderHeight = resolutionMenu.ImageHeight;
        }

        void outputBrowse_MouseButtonClick(Widget source, EventArgs e)
        {
            String startingFolder = outputFolder.OnlyText;
            if (!Directory.Exists(startingFolder))
            {
                startingFolder = MedicalConfig.ImageOutputFolder;
                ensureOutputFolderExists(startingFolder);
            }
            using (DirDialog dirDialog = new DirDialog(MainWindow.Instance, "Select a render output folder", startingFolder))
            {
                if (dirDialog.showModal() == NativeDialogResult.OK)
                {
                    outputFolder.OnlyText = dirDialog.Path;
                }
            }
        }

        void viewLicense_MouseButtonClick(Widget source, EventArgs e)
        {

        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
        {
            try
            {
                if (agreeButton.Checked)
                {
                    writeImageToDisk();
                }
                else
                {
                    MessageBox.show("You must agree to the license in order to save your image.", "License", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Could not save image.\nReason: {0}", ex.Message), "Save Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        void openOutputFolder_MouseButtonClick(Widget source, EventArgs e)
        {
            String folder = outputFolder.OnlyText;
            if (ensureOutputFolderExists(folder))
            {
                OtherProcessManager.openLocalURL(outputFolder.OnlyText);
            }
        }

        void toggleRequireImagesWidgets()
        {
            saveButton.Enabled = fullSizeButton.Enabled = currentImage != null;
        }

        void closeCurrentImage()
        {
            if (currentImage != null)
            {
                currentImage.Dispose();
                currentImage = null;
                toggleRequireImagesWidgets();
            }
            if (imageAtlas != null)
            {
                previewImage.setItemResource("");
                imageAtlas.Dispose();
                imageAtlas = null;
            }
        }

        private void writeLicenseToImage(Bitmap bitmap)
        {
            int fontPixels = (int)(currentImage.Height * 0.0097f);
            if (fontPixels < 6)
            {
                fontPixels = 6;
            }
            String licenseUse = "personal";
            if (licenseTypeGroup.SelectedButton == commercialButton)
            {
                licenseUse = "commercial";
            }
            imageRenderer.addLicenseText(bitmap, String.Format("Licensed to {0} for {1} use.", licenseManager.LicenseeName, licenseUse), fontPixels);
        }

        private void writeImageToDisk()
        {
            String outputDirectory = outputFolder.OnlyText;
            String imageBaseName = imageName.OnlyText;
            if (String.IsNullOrWhiteSpace(imageBaseName))
            {
                throw new Exception("Must specify a name for the image.");
            }

            String extension;
            ImageFormat imageOutputFormat;
            getImageFormat(out extension, out imageOutputFormat);

            String fileName = imageBaseName + extension;
            if (ensureOutputFolderExists(outputDirectory))
            {
                String outputFile = Path.Combine(outputDirectory, fileName);
                if (File.Exists(outputFile))
                {
                    String[] sameNameFiles = Directory.GetFiles(outputDirectory, String.Format("{0}*{1}", imageBaseName, extension), SearchOption.TopDirectoryOnly);
                    int fileIndex = sameNameFiles.Length;
                    fileName = String.Format("{0}{1}{2}", imageBaseName, fileIndex, extension);
                    outputFile = Path.Combine(outputDirectory, fileName);
                    //Not as likely to hit this part so just loop for the filename, this will only happen if they skipped one name
                    while (File.Exists(outputFile))
                    {
                        fileName = String.Format("{0}{1}{2}", imageBaseName, ++fileIndex, extension);
                        outputFile = Path.Combine(outputDirectory, fileName);
                    }
                }
                writeLicenseToImage(currentImage);
                currentImage.Save(outputFile, imageOutputFormat);
                notificationManager.showNotification(new OpenImageNotification(outputFile));
                closeCurrentImage();
            }
        }

        private void getImageFormat(out String extension, out ImageFormat imageOutputFormat)
        {
            extension = ".png";
            imageOutputFormat = ImageFormat.Png;

            switch (imageFormat.SelectedIndex)
            {
                case 1: //Bitmap
                    extension = ".bmp";
                    imageOutputFormat = ImageFormat.Bmp;
                    break;
                case 2: //JPEG
                    extension = ".jpg";
                    imageOutputFormat = ImageFormat.Jpeg;
                    break;
            }
        }

        private bool ensureOutputFolderExists(String outputFolder)
        {
            if (!Directory.Exists(outputFolder))
            {
                try
                {
                    Directory.CreateDirectory(outputFolder);
                }
                catch (Exception e)
                {
                    MessageBox.show(String.Format("Could not create output folder {0}.\nReason: {1}", outputFolder, e.Message), "Directory Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    return false;
                }
            }
            return true;
        }

        public int RenderWidth
        {
            get
            {
                return width.IntValue;
            }
            set
            {
                width.IntValue = value;
            }
        }

        public int RenderHeight
        {
            get
            {
                return height.IntValue;
            }
            set
            {
                height.IntValue = value;
            }
        }
    }
}
