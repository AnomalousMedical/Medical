using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;
using MyGUIPlugin;
using System.Drawing;

namespace Medical
{
    public class AnatomyController
    {
        private const string TRANSPARENCY_STATE = "AnatomyFinder";

        public event EventHandler AnatomyChanged;

        private AnatomyTagManager anatomyTagManager = new AnatomyTagManager();
        private AnatomySearchList anatomySearchList = new AnatomySearchList();

        private ImageRenderer imageRenderer;
        private ImageAtlas imageAtlas = new ImageAtlas("AntomyThumbnails", new Size2(50, 50), new Size2(512, 512));

        public AnatomyController(ImageRenderer imageRenderer)
        {
            this.imageRenderer = imageRenderer;
            TransparencyController.createTransparencyState(TRANSPARENCY_STATE);
        }

        public void sceneLoaded()
        {
            foreach (AnatomyIdentifier anatomy in AnatomyManager.AnatomyList)
            {
                anatomySearchList.addAnatomy(anatomy);
                anatomyTagManager.addAnatomyIdentifier(anatomy);
            }
            foreach (AnatomyTagGroup tagGroup in anatomyTagManager.Groups)
            {
                anatomySearchList.addAnatomy(tagGroup);
            }
            if (AnatomyChanged != null)
            {
                AnatomyChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public void sceneUnloading()
        {
            anatomyTagManager.clear();
            anatomySearchList.clear();
        }

        public AnatomyTagManager TagManager
        {
            get
            {
                return anatomyTagManager;
            }
        }

        public AnatomySearchList SearchList
        {
            get
            {
                return anatomySearchList;
            }
        }

        public String getThumbnail(Anatomy anatomy, float theta)
        {
            if (imageRenderer == null)
            {
                return "";
            }
            if (!imageAtlas.containsImage(anatomy.AnatomicalName))
            {
                //Generate thumbnail
                AxisAlignedBox boundingBox = anatomy.WorldBoundingBox;
                //SceneViewWindow window = sceneViewController.ActiveWindow;
                Vector3 center = boundingBox.Center;

                //float nearPlane = window.Camera.getNearClipDistance();
                //float theta = window.Camera.getFOVy() * 0.0174532925f;
                float aspectRatio = 1.0f;// window.Camera.getAspectRatio();
                if (aspectRatio < 1.0f)
                {
                    theta *= aspectRatio;
                }

                Vector3 translation = center;
                Vector3 direction = Vector3.Backward;// (window.Translation - window.LookAt).normalized();
                translation += direction * boundingBox.DiagonalDistance / (float)Math.Tan(theta);

                String currentState = TransparencyController.ActiveTransparencyState;
                TransparencyController.ActiveTransparencyState = TRANSPARENCY_STATE;

                TransparencyController.setAllAlphas(0.0f);
                anatomy.TransparencyChanger.CurrentAlpha = 1.0f;

                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = 50;
                imageProperties.Height = 50;
                imageProperties.UseWindowBackgroundColor = false;
                imageProperties.CustomBackgroundColor = Engine.Color.White;
                imageProperties.AntiAliasingMode = 2;
                imageProperties.TransparentBackground = true;
                imageProperties.UseActiveViewportLocation = false;
                imageProperties.UseCustomPosition = true;
                imageProperties.CameraLookAt = center;
                imageProperties.CameraPosition = translation;
                imageProperties.ShowBackground = false;
                imageProperties.ShowWatermark = false;
                imageProperties.ShowUIUpdates = false;

                String imageName;
                using (Bitmap thumb = imageRenderer.renderImage(imageProperties))
                {
                    imageName = imageAtlas.addImage(anatomy.AnatomicalName, thumb);
                }

                TransparencyController.ActiveTransparencyState = currentState;

                return imageName;
            }
            else
            {
                return imageAtlas.getImageId(anatomy.AnatomicalName);
            }
        }
    }
}
