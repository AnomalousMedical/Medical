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
        private Button sizeButton;

        private Button outputBrowse;
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
        private Button renderButton;

        private ResolutionMenu resolutionMenu;
        private SceneViewController sceneViewController;
        private ImageRenderer imageRenderer;

        private int previewMaxWidth;
        private int previewMaxHeight;

        private Bitmap currentImage = null;
        private ImageAtlas imageAtlas = null;

        private ImageLicenseServer imageLicenseServer;
        private NotificationGUIManager notificationManager;
        private GUIManager guiManager;
        private LicensePopup licensePopup = null;

        public RenderPropertiesDialog(SceneViewController sceneViewController, ImageRenderer imageRenderer, ImageLicenseServer imageLicenseServer, GUIManager guiManager)
            : base("Medical.GUI.Render.RenderPropertiesDialog.layout")
        {
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;
            this.imageLicenseServer = imageLicenseServer;
            this.notificationManager = guiManager.NotificationManager;
            this.guiManager = guiManager;

            width = new NumericEdit(window.findWidget("RenderingTab/WidthEdit") as Edit);
            height = new NumericEdit(window.findWidget("RenderingTab/HeightEdit") as Edit);

            width.ValueChanged += new MyGUIEvent(renderSizeChanged);
            height.ValueChanged += new MyGUIEvent(renderSizeChanged);

            renderButton = window.findWidget("RenderingTab/Render") as Button;
            renderButton.MouseButtonClick += new MyGUIEvent(renderButton_MouseButtonClick);

            previewImage = (StaticImage)window.findWidget("PreviewImage");
            previewMaxWidth = previewImage.Width;
            previewMaxHeight = previewImage.Height;

            fullSizeButton = (Button)window.findWidget("FullSize");
            fullSizeButton.MouseButtonClick += new MyGUIEvent(fullSizeButton_MouseButtonClick);

            sizeButton = window.findWidget("RenderingTab/SizeButton") as Button;
            sizeButton.MouseButtonClick += new MyGUIEvent(sizeButton_MouseButtonClick);

            //ResolutionMenu
            resolutionMenu = new ResolutionMenu();
            resolutionMenu.ResolutionChanged += new EventHandler(resolutionMenu_ResolutionChanged);

            //Image save properties
            imageName = (Edit)window.findWidget("ImageName");
            imageName.Caption = "Anomalous Image";

            outputFolder = (Edit)window.findWidget("OutputFolder");
            outputFolder.Caption = MedicalConfig.ImageOutputFolder;
            outputBrowse = (Button)window.findWidget("OutputBrowse");
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
            if (licensePopup != null)
            {
                licensePopup.Dispose();
            }
            resolutionMenu.Dispose();
            base.Dispose();
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            changeAspectRatio();
        }

        private void changeAspectRatio()
        {
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
            RenderWidth = resolutionMenu.ImageWidth;
            RenderHeight = resolutionMenu.ImageHeight;
            changeAspectRatio();
        }

        void outputBrowse_MouseButtonClick(Widget source, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.show("Could not open folder browser.\nReason: " + ex.Message, "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        void viewLicense_MouseButtonClick(Widget source, EventArgs e)
        {
            source.Enabled = false;
            ImageLicenseType type = ImageLicenseType.Personal;
            if (licenseTypeGroup.SelectedButton == commercialButton)
            {
                type = ImageLicenseType.Commercial;
            }
            imageLicenseServer.getLicenseFromServer(type, delegate(bool success, String message)
            {
                source.Enabled = true;
                if (success)
                {
                    if (licensePopup == null)
                    {
                        licensePopup = new LicensePopup(guiManager);
                    }
                    licensePopup.LicenseText = message;
                    licensePopup.show(0, 0);
                }
                else
                {
                    MessageBox.show(message, "Server Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
            });
        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (agreeButton.Checked)
            {
                if (!String.IsNullOrWhiteSpace(imageName.OnlyText))
                {
                    ImageLicenseType type = ImageLicenseType.Personal;
                    if (licenseTypeGroup.SelectedButton == commercialButton)
                    {
                        type = ImageLicenseType.Commercial;
                    }
                    setInterfaceEnabled(false);
                    imageLicenseServer.licenseImage(type, delegate(bool success, String message)
                    {
                        setInterfaceEnabled(true);
                        try
                        {
                            if (success)
                            {
                                saveImage();
                            }
                            else
                            {
                                MessageBox.show(message, "License Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.show(String.Format("Could not save image.\nReason: {0}", ex.Message), "Save Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                        }
                    });
                }
                else
                {
                    MessageBox.show("You must enter a name for your image.", "Save Error", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
                }
            }
            else
            {
                MessageBox.show("You must agree to the license in order to save your image.", "License", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
            }
        }

        void openOutputFolder_MouseButtonClick(Widget source, EventArgs e)
        {
            String folder = outputFolder.OnlyText;
            try
            {
                ensureOutputFolderExists(folder);
                OtherProcessManager.openLocalURL(outputFolder.OnlyText);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Could not open output directory {0}.\nReason: {1}", folder, ex.Message), "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        void toggleRequireImagesWidgets()
        {
            saveButton.Enabled = fullSizeButton.Enabled = currentImage != null;
        }

        void setInterfaceEnabled(bool enabled)
        {
            width.Edit.Enabled = enabled;
            height.Edit.Enabled = enabled;
            sizeButton.Enabled = enabled;
            outputBrowse.Enabled = enabled;
            imageName.Enabled = enabled;
            outputFolder.Enabled = enabled;
            imageFormat.Enabled = enabled;
            fullSizeButton.Enabled = enabled;
            personalButton.Enabled = enabled;
            commercialButton.Enabled = enabled;
            agreeButton.Enabled = enabled;
            saveButton.Enabled = enabled;
            renderButton.Enabled = enabled;
        }

        void closeCurrentImage()
        {
            if (!imageLicenseServer.LicensingImage)
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
            imageRenderer.addLicenseText(bitmap, String.Format("Licensed to {0} for {1} use.", imageLicenseServer.LicenseeName, licenseUse), fontPixels);
        }

        private void saveImage()
        {
            String outputDirectory = outputFolder.OnlyText;
            String imageBaseName = imageName.OnlyText;
            String extension;
            ImageFormat imageOutputFormat;
            getImageFormat(out extension, out imageOutputFormat);
            try
            {
                writeImageToDisk(outputDirectory, imageBaseName, extension, imageOutputFormat);
            }
            catch (Exception e)
            {
                try
                {
                    //Try to do our best to save the image somewhere.
                    writeImageToDisk(MedicalConfig.ImageOutputFolder, imageBaseName, extension, imageOutputFormat);
                    MessageBox.show(String.Format("Error writing the file to the directory {0}.\nYour file has been written to {1} instead.\nReason: {2}", outputDirectory, MedicalConfig.ImageOutputFolder, e.Message), "Save Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
                catch (Exception e1)
                {
                    MessageBox.show(String.Format("Error writing the file to the directory {0}\nReason: {1}.\n\nAlso failed to write to the backup save folder {2}.\nReason: {3}\n\nOur servers will still count this as one of your image credits. Please contact us at info@anomalousmedical.com get your credit back.\nWe reccomend taking a screen shot of this error message and sending it to us.", outputDirectory, e.Message, MedicalConfig.ImageOutputFolder, e1.Message), "Save Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
            }
        }

        private void writeImageToDisk(String outputDirectory, String imageBaseName, String extension, ImageFormat imageOutputFormat)
        {
            String fileName = imageBaseName + extension;
            ensureOutputFolderExists(outputDirectory);
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

        private void ensureOutputFolderExists(String outputFolder)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
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
