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
    public enum AnatomyPickingMode
    {
        Group,
        Individual,
        None,
    }

    public class AnatomyController : IDisposable
    {
        private const string TRANSPARENCY_STATE = "AnatomyFinder";
        private ImageRendererProperties imageProperties;

        public event EventHandler AnatomyChanged;

        private AnatomyTagManager anatomyTagManager = new AnatomyTagManager();
        private AnatomySearchList anatomySearchList = new AnatomySearchList();
        private AnatomyTree anatomyTree = new AnatomyTree();

        private ImageRenderer imageRenderer;
        private ImageAtlas imageAtlas = new ImageAtlas("AntomyThumbnails", new Size2(50, 50), new Size2(512, 512));

        private AnatomyPickingMode pickingMode;
        public event EventDelegate<AnatomyController, AnatomyPickingMode> PickingModeChanged;
        public event EventDelegate<AnatomyController, bool> ShowPremiumAnatomyChanged;
        private bool showPremiumAnatomy = false;

        public AnatomyController(ImageRenderer imageRenderer)
        {
            this.imageRenderer = imageRenderer;
            TransparencyController.createTransparencyState(TRANSPARENCY_STATE);

            imageProperties = new ImageRendererProperties();
            imageProperties.Width = 50;
            imageProperties.Height = 50;
            imageProperties.UseWindowBackgroundColor = false;
            imageProperties.CustomBackgroundColor = new Engine.Color(.94f, .94f, .94f);
            imageProperties.AntiAliasingMode = 2;
            imageProperties.TransparentBackground = true;
            imageProperties.UseActiveViewportLocation = false;
            imageProperties.ShowBackground = false;
            imageProperties.ShowWatermark = false;
            imageProperties.ShowUIUpdates = false;
        }

        public void Dispose()
        {
            imageAtlas.Dispose();
        }

        public void sceneLoaded()
        {
            AnatomyOrganizer organizer = AnatomyManager.AnatomyOrganizer;
            if (organizer != null)
            {
                anatomyTagManager.setupPropertyGroups(organizer.TagProperties);
            }
            foreach (AnatomyIdentifier anatomy in AnatomyManager.AnatomyList)
            {
                anatomySearchList.addAnatomy(anatomy);
                anatomyTagManager.addAnatomyIdentifier(anatomy);
                anatomyTree.addAnatomy(anatomy);
            }
            foreach (AnatomyTagGroup tagGroup in anatomyTagManager.Groups)
            {
                anatomySearchList.addAnatomy(tagGroup);
                anatomyTree.addAnatomy(tagGroup);
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
            anatomyTree.clear();
            imageAtlas.clear();
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

        public AnatomyTree AnatomyTree
        {
            get
            {
                return anatomyTree;
            }
        }

        /// <summary>
        /// Check to see if a piece of anatomy has a thumbnail.
        /// </summary>
        /// <param name="anatomy">The anatomy to check.</param>
        /// <returns>True if there is a thumbnail, false if not.</returns>
        public bool hasThumbnail(Anatomy anatomy)
        {
            return imageAtlas.containsImage(anatomy.AnatomicalName);
        }

        /// <summary>
        /// Get the string for a thumbnail. Will return true if the thumbnail had to be created.
        /// </summary>
        /// <param name="anatomy">The anatomy to create the thumbnail for.</param>
        /// <param name="theta">The fovy</param>
        /// <param name="imageName">The name of the image will be put here.</param>
        /// <returns></returns>
        public bool getThumbnail(Anatomy anatomy, float theta, out String imageName)
        {
            if (!imageAtlas.containsImage(anatomy.AnatomicalName))
            {
                //Generate thumbnail
                AxisAlignedBox boundingBox = anatomy.WorldBoundingBox;
                Vector3 center = boundingBox.Center;

                float aspectRatio = (float)imageProperties.Width / imageProperties.Height;
                if (aspectRatio < 1.0f)
                {
                    theta *= aspectRatio;
                }

                Vector3 translation = center;
                Vector3 direction = anatomy.PreviewCameraDirection;
                translation += direction * boundingBox.DiagonalDistance / (float)Math.Tan(theta);

                String currentState = TransparencyController.ActiveTransparencyState;
                TransparencyController.ActiveTransparencyState = TRANSPARENCY_STATE;

                TransparencyController.setAllAlphas(0.0f);
                anatomy.TransparencyChanger.CurrentAlpha = 1.0f;

                imageProperties.CameraLookAt = center;
                imageProperties.CameraPosition = translation;

                using (Bitmap thumb = imageRenderer.renderImage(imageProperties))
                {
                    imageName = imageAtlas.addImage(anatomy.AnatomicalName, thumb);
                }

                TransparencyController.ActiveTransparencyState = currentState;

                return true;
            }
            else
            {
                imageName = imageAtlas.getImageId(anatomy.AnatomicalName);
                return false;
            }
        }

        public AnatomyPickingMode PickingMode
        {
            get
            {
                return pickingMode;
            }
            set
            {
                if (value != pickingMode)
                {
                    pickingMode = value;
                    if (PickingModeChanged != null)
                    {
                        PickingModeChanged.Invoke(this, value);
                    }
                }
            }
        }

        public bool ShowPremiumAnatomy
        {
            get
            {
                return showPremiumAnatomy;
            }
            set
            {
                if (showPremiumAnatomy != value)
                {
                    showPremiumAnatomy = value;
                    if (ShowPremiumAnatomyChanged != null)
                    {
                        ShowPremiumAnatomyChanged.Invoke(this, showPremiumAnatomy);
                    }
                }
            }
        }
    }
}
