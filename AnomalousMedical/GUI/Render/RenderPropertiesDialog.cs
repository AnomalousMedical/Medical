using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using System.Drawing;
using Engine;

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

        public RenderPropertiesDialog(SceneViewController sceneViewController, ImageRenderer imageRenderer, LicenseManager licenseManager)
            :base("Medical.GUI.Render.RenderPropertiesDialog.layout")
        {
            this.sceneViewController = sceneViewController;
            this.imageRenderer = imageRenderer;
            this.licenseManager = licenseManager;

            width = new NumericEdit(window.findWidget("RenderingTab/WidthEdit") as Edit);
            height = new NumericEdit(window.findWidget("RenderingTab/HeightEdit") as Edit);

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

            toggleRequireImagesWidgets();
        }

        public override void Dispose()
        {
            resolutionMenu.Dispose();
            base.Dispose();
        }

        protected override void onClosed(EventArgs args)
        {
            base.onClosed(args);
            closeCurrentImage();
        }

        void fullSizeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (currentImage != null)
            {
                Bitmap bitmap = (Bitmap)currentImage.Clone();
                writeLicenseToImage(bitmap);
                imageRenderer.makeSampleImage(bitmap);
                ImageWindow window = new ImageWindow(MainWindow.Instance, sceneViewController.ActiveWindow.Name, bitmap, false);
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

        }

        void viewLicense_MouseButtonClick(Widget source, EventArgs e)
        {

        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
        {
            writeImageToDisk();
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
