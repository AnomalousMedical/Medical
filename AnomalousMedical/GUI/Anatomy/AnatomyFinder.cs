using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine.Platform;
using Engine;

namespace Medical.GUI
{
    public class AnatomyFinder : MDIDialog
    {
        private static readonly int ThumbSize = ScaleHelper.Scaled(50);
        private static readonly int ThumbRenderSize = ScaleHelper.Scaled(200);
        private static readonly int lockSize = ScaleHelper.Scaled(18);

        private static ButtonEvent PickAnatomy;
        private static ButtonEvent OpenAnatomyFinder;
        
        static AnatomyFinder()
        {
            PickAnatomy = new ButtonEvent(EventLayers.Selection);
            PickAnatomy.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(PickAnatomy);

            OpenAnatomyFinder = new ButtonEvent(EventLayers.Gui);
            OpenAnatomyFinder.addButton(KeyboardButtonCode.KC_LCONTROL);
            OpenAnatomyFinder.addButton(KeyboardButtonCode.KC_F);
            DefaultEvents.registerDefaultEvent(OpenAnatomyFinder);
        }

        private HashSetMultiSelectButtonGrid anatomyList;
        private EditBox searchBox;
        private Button clearButton;

        private AnatomyContextWindowManager anatomyWindowManager;

        private SceneViewController sceneViewController;

        private AnatomyController anatomyController;
        private int lastWidth = -1;
        private int lastHeight = -1;

        private ButtonGridLiveThumbnailController<Anatomy> buttonGridThumbs;

        public event Action ShowBuyMessage;

        public AnatomyFinder(AnatomyController anatomyController, SceneViewController sceneViewController)
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            this.anatomyController = anatomyController;
            anatomyController.AnatomyChanged += new EventHandler(anatomyController_AnatomyChanged);
            anatomyController.ShowPremiumAnatomyChanged += anatomyController_ShowPremiumAnatomyChanged;
            anatomyController.ClearDisplayedAnatomy += anatomyController_ClearDisplayedAnatomy;
            anatomyController.DisplayAnatomy += anatomyController_DisplayAnatomy;
            anatomyController.SelectedAnatomy.SelectedAnatomyChanged += anatomyController_SelectedAnatomyChanged;
            anatomyController.SearchStarted += anatomyController_SearchStarted;
            anatomyController.SearchEnded += anatomyController_SearchEnded;
            this.sceneViewController = sceneViewController;
            anatomyWindowManager = new AnatomyContextWindowManager(sceneViewController, anatomyController, this);

            ScrollView anatomyScroll = (ScrollView)window.findWidget("AnatomyList");
            anatomyList = new HashSetMultiSelectButtonGrid(anatomyScroll, new ButtonGridListLayout(), new ButtonGridItemNaturalSort());
            anatomyList.ItemActivated += anatomyList_ItemActivated;
            anatomyList.ItemAdded += anatomyList_ItemAdded;
            anatomyList.ItemRemoved += anatomyList_ItemRemoved;
            anatomyList.ItemChosen += anatomyList_ItemChosen;

            buttonGridThumbs = new ButtonGridLiveThumbnailController<Anatomy>("AnatomyFinder_", new IntSize2(ThumbRenderSize, ThumbRenderSize), sceneViewController, anatomyList, anatomyScroll);

            searchBox = (EditBox)window.findWidget("SearchBox");
            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);
            clearButton = (Button)searchBox.findWidgetChildSkin("Clear");
            clearButton.MouseButtonClick += new MyGUIEvent(clearButton_MouseButtonClick);
            clearButton.NeedToolTip = true;
            clearButton.EventToolTip += button_UserObject_EventToolTip;
            clearButton.UserObject = "Clear Search";

            PickAnatomy.FirstFrameUpEvent += pickAnatomy_FirstFrameUpEvent;
            OpenAnatomyFinder.FirstFrameUpEvent += openAnatomyFinder_FirstFrameUpEvent;

            this.Resized += new EventHandler(AnatomyFinder_Resized);
            fixListItemWidth();

            updateSearch();
        }

        public override void Dispose()
        {
            PickAnatomy.FirstFrameUpEvent -= pickAnatomy_FirstFrameUpEvent;
            OpenAnatomyFinder.FirstFrameUpEvent -= openAnatomyFinder_FirstFrameUpEvent;

            buttonGridThumbs.Dispose();
            anatomyWindowManager.Dispose();
            base.Dispose();
        }

        void button_UserObject_EventToolTip(Widget source, EventArgs e)
        {
            TooltipManager.Instance.processTooltip(source, source.UserObject.ToString(), (ToolTipEventArgs)e);
        }

        public void sceneUnloading()
        {
            anatomyWindowManager.sceneUnloading();
        }

        public void displayAnatomy(String caption, IEnumerable<Anatomy> anatomyToDisplay)
        {
            if (!Visible)
            {
                Visible = true;
            }
            clearButton.Visible = true;
            searchBox.Caption = caption;
            anatomyController.displayAnatomy(anatomyToDisplay);
        }

        public IntCoord DeadZone
        {
            get
            {
                if (window.Visible)
                {
                    return new IntCoord(window.Left, window.Top, window.Width, window.Height);
                }
                else
                {
                    return new IntCoord(int.MinValue, int.MinValue, int.MinValue, int.MinValue);
                }
            }
        }

        public IntVector2 DisplayHintLocation { get; private set; }

        void openAnatomyFinder_FirstFrameUpEvent(EventLayer eventLayer)
        {
            if (!Gui.Instance.HandledKeyboardButtons || InputManager.Instance.getKeyFocusWidget().RootWidget == window)
            {
                this.Visible = !this.Visible;
                InputManager.Instance.setKeyFocusWidget(searchBox);
            }
        }

        void AnatomyFinder_Resized(object sender, EventArgs e)
        {
            fixListItemWidth();
        }

        private void fixListItemWidth()
        {
            //Layout only if size changes
            if (window.Width != lastWidth || window.Height != lastHeight)
            {
                lastWidth = window.Width;
                lastHeight = window.Height;
                anatomyList.layout();
                buttonGridThumbs.determineVisibleHosts();
            }
        }

        void anatomyController_AnatomyChanged(object sender, EventArgs e)
        {
            updateSearch();
        }

        void searchBox_EventEditTextChange(Widget source, EventArgs e)
        {
            updateSearch();
        }

        private void updateSearch()
        {
            String search = searchBox.Caption;
            anatomyController.findAnatomy(search);
            clearButton.Visible = !String.IsNullOrEmpty(search);
        }

        void pickAnatomy_FirstFrameUpEvent(EventLayer eventLayer)
        {
            IntVector3 absMouse = eventLayer.Mouse.AbsolutePosition;
            if (eventLayer.EventProcessingAllowed)
            {
                SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
                DisplayHintLocation = new IntVector2(absMouse.x, absMouse.y);
                Ray3 cameraRay = activeWindow.getCameraToViewportRayScreen(absMouse.x, absMouse.y);

                Anatomy bestMatch = anatomyController.findAnatomy(cameraRay);

                if (anatomyController.PickingMode != AnatomyPickingMode.None)
                {
                    anatomyController.processSelection(bestMatch);
                }

                if (bestMatch != null)
                {
                    searchBox.Caption = "Clicked";
                    clearButton.Visible = true;
                    if (MedicalConfig.AutoOpenAnatomyFinder && !Visible && anatomyController.SelectionOperator != SelectionOperator.Remove && anatomyController.PickingMode != AnatomyPickingMode.None)
                    {
                        Visible = true;
                    }
                }
                else
                {
                    clearButton.Visible = false;
                    searchBox.Caption = "";
                }
            }
        }

        void clearButton_MouseButtonClick(Widget source, EventArgs e)
        {
            searchBox.Caption = "";
            updateSearch();
            InputManager.Instance.setKeyFocusWidget(searchBox);
        }

        void anatomyList_ItemActivated(ButtonGridItem item)
        {
            toggleAnatomyTransparency(item);
        }

        void anatomyList_ItemChosen(ButtonGridItem item)
        {
            Anatomy anatomy = buttonGridThumbs.getUserObject(item);
            if (anatomyController.ShowPremiumAnatomy || anatomy.ShowInBasicVersion)
            {
                DisplayHintLocation = new IntVector2(window.Right, item.AbsoluteTop);
                anatomyController.processSelection(anatomy);
            }
            else
            {
                showBuyMessage();
            }
        }

        private void toggleAnatomyTransparency(ButtonGridItem item)
        {
            if (item != null)
            {
                Anatomy selectedAnatomy = buttonGridThumbs.getUserObject(item);
                if (anatomyController.ShowPremiumAnatomy || selectedAnatomy.ShowInBasicVersion)
                {
                    if (selectedAnatomy.CurrentAlpha >= 0.9999f)
                    {
                        selectedAnatomy.smoothBlend(0.7f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
                    }
                    else if (selectedAnatomy.CurrentAlpha <= 0.00008f)
                    {
                        selectedAnatomy.smoothBlend(1.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
                    }
                    else
                    {
                        selectedAnatomy.smoothBlend(0.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
                    }
                }
                else
                {
                    showBuyMessage();
                }
            }
        }

        private ButtonGridItem addAnatomyToList(Anatomy anatomy)
        {
            //Add item
            ButtonGridItem anatomyItem = anatomyList.addItem("", anatomy.AnatomicalName, "", anatomy);
            if(!anatomyController.ShowPremiumAnatomy && !anatomy.ShowInBasicVersion)
            {
                IntCoord itemCoord = anatomyItem.Coord;
                ImageBox lockedFeatureImage = (ImageBox)anatomyItem.createWidgetT("ImageBox", "ImageBox", 0, itemCoord.Bottom - lockSize, lockSize, lockSize, Align.Left | Align.Top, "LockedFeatureImage");
                lockedFeatureImage.NeedMouseFocus = false;
                lockedFeatureImage.setItemResource("LockedFeature");
            }
            if(anatomyController.SelectedAnatomy.isSelected(anatomy))
            {
                anatomyList.addSelected(anatomyItem);
            }
            return anatomyItem;
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            int itemCount = anatomyList.Count;
            float fovy = sceneViewController.ActiveWindow.Camera.getFOVy();
        }

        private void showBuyMessage()
        {
            if(ShowBuyMessage != null)
            {
                ShowBuyMessage.Invoke();
            }
        }

        void anatomyController_ShowPremiumAnatomyChanged(AnatomyController source, bool arg)
        {
            //This gets rid of the locks, clear thumbs and update the search.
            updateSearch();
        }

        void anatomyController_SelectedAnatomyChanged(AnatomySelection obj)
        {
            anatomyList.setSelection(selectedButtons(obj));
        }

        IEnumerable<ButtonGridItem> selectedButtons(AnatomySelection selection)
        {
            foreach(var selected in selection.SelectedAnatomy)
            {
                ButtonGridItem item = buttonGridThumbs.findItemByUserObject(selected);
                if(item != null)
                {
                    yield return item;
                }
            }
        }

        void anatomyController_DisplayAnatomy(Anatomy obj)
        {
            addAnatomyToList(obj);
        }

        void anatomyController_ClearDisplayedAnatomy()
        {
            anatomyList.clear();
        }

        void anatomyList_ItemRemoved(ButtonGrid arg1, ButtonGridItem arg2)
        {
            buttonGridThumbs.itemRemoved(arg2);
        }

        void anatomyController_SearchStarted()
        {
            anatomyList.SuppressLayout = true;
        }

        void anatomyController_SearchEnded()
        {
            anatomyList.SuppressLayout = false;
            anatomyList.layout();
            buttonGridThumbs.determineVisibleHosts();
        }

        void anatomyList_ItemAdded(ButtonGrid arg1, ButtonGridItem arg2)
        {
            Anatomy anatomy = (Anatomy)arg2.UserObject; //Ok to access user object here since it is set during the add.
            Radian theta = sceneViewController.ActiveWindow.Camera.getFOVy();

            //Generate thumbnail
            AxisAlignedBox boundingBox = anatomy.WorldBoundingBox;
            Vector3 center = boundingBox.Center;

            //PROBABLY DON'T NEED THIS, ASPECT IS A SQUARE
            float aspectRatio = (float)ThumbSize / ThumbSize;
            if (aspectRatio < 1.0f)
            {
                theta *= aspectRatio;
            }

            Vector3 translation = center;
            Vector3 direction = anatomy.PreviewCameraDirection;
            translation += direction * boundingBox.DiagonalDistance / (float)Math.Tan(theta);

            LayerState layers = new LayerState(anatomy.TransparencyNames, 1.0f);

            buttonGridThumbs.itemAdded(arg2, layers, translation, center, anatomy);
        }
    }
}
