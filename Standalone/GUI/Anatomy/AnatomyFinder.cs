using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine.Platform;
using Engine;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;

namespace Medical.GUI
{
    public class AnatomyFinder : PinableMDIDialog
    {
        private static readonly int MouseClickWindowOffset = ScaleHelper.Scaled(5);
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
        private Button undoButton;
        private Button redoButton;

        private ButtonGridItemNaturalSort naturalSort = new ButtonGridItemNaturalSort();

        private AnatomyContextWindowManager anatomyWindowManager;
        private AnatomyFilter anatomyFilter;

        private SceneViewController sceneViewController;
        private LayerController layerController;
        private AnatomyController anatomyController;

        private int lastWidth = -1;
        private int lastHeight = -1;
        private TravelTracker travelTracker = new TravelTracker();
        private ClickedAnatomyManager clickedAnatomy = new ClickedAnatomyManager();

        private ButtonGridLiveThumbnailController<Anatomy> buttonGridThumbs;

        public event Action ShowBuyMessage;

        public AnatomyFinder(AnatomyController anatomyController, SceneViewController sceneViewController, LayerController layerController)
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            this.anatomyController = anatomyController;
            anatomyController.AnatomyChanged += anatomyController_AnatomyChanged;
            anatomyController.ShowPremiumAnatomyChanged += anatomyController_ShowPremiumAnatomyChanged;
            anatomyController.ClearDisplayedAnatomy += anatomyController_ClearDisplayedAnatomy;
            if (PlatformConfig.UnrestrictedEnvironment)
            {
                anatomyController.DisplayAnatomy += anatomyController_DisplayAnatomy;
            }
            else
            {
                anatomyController.DisplayAnatomy += anatomyController_DisplayAnatomy_Restricted;
            }
            anatomyController.SelectedAnatomy.SelectedAnatomyChanged += anatomyController_SelectedAnatomyChanged;
            anatomyController.SearchStarted += anatomyController_SearchStarted;
            anatomyController.SearchEnded += anatomyController_SearchEnded;
            anatomyController.SuggestSearchCaption += anatomyController_SuggestSearchCaption;
            
            this.sceneViewController = sceneViewController;

            this.layerController = layerController;
            this.layerController = layerController;
            layerController.OnRedo += updateUndoRedo;
            layerController.OnUndo += updateUndoRedo;
            layerController.OnUndoRedoChanged += updateUndoRedo;
            layerController.OnActiveTransparencyStateChanged += updateUndoRedo;

            anatomyWindowManager = new AnatomyContextWindowManager(sceneViewController, anatomyController, layerController, this);
            anatomyFilter = new AnatomyFilter(anatomyController);
            anatomyFilter.refreshCategories();
            anatomyFilter.FilterChanged += anatomyFilter_FilterChanged;
            anatomyFilter.TopLevelAnatomyChanged += anatomyFilter_TopLevelAnatomyChanged;

            Button filter = window.findWidget("Filter") as Button;
            filter.MouseButtonClick += filter_MouseButtonClick;

            ScrollView anatomyScroll = (ScrollView)window.findWidget("AnatomyList");
            anatomyList = new HashSetMultiSelectButtonGrid(anatomyScroll, new ButtonGridListLayout(), naturalSort);
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

            Button unhideAll = window.findWidget("UnhideAll") as Button;
            unhideAll.MouseButtonClick += (s, e) =>
            {
                LayerState undo = LayerState.CreateAndCapture();
                this.layerController.unhideAll();
                this.layerController.pushUndoState(undo);
            };

            undoButton = window.findWidget("Undo") as Button;
            undoButton.MouseButtonClick += (s, e) => layerController.undo();

            redoButton = window.findWidget("Redo") as Button;
            redoButton.MouseButtonClick += (s, e) => layerController.redo();

            PickAnatomy.FirstFrameDownEvent += PickAnatomy_FirstFrameDownEvent;
            PickAnatomy.OnHeldDown += PickAnatomy_OnHeldDown;
            PickAnatomy.FirstFrameUpEvent += pickAnatomy_FirstFrameUpEvent;
            OpenAnatomyFinder.FirstFrameUpEvent += openAnatomyFinder_FirstFrameUpEvent;

            if(!PlatformConfig.UnrestrictedEnvironment && !anatomyController.ShowPremiumAnatomy)
            {
                filter.Visible = false;
                searchBox.Width = filter.Right - searchBox.Left;
            }

            this.Resized += new EventHandler(AnatomyFinder_Resized);
            fixListItemWidth();

            updateSearch();
        }

        public override void Dispose()
        {
            PickAnatomy.FirstFrameDownEvent -= PickAnatomy_FirstFrameDownEvent;
            PickAnatomy.OnHeldDown -= PickAnatomy_OnHeldDown;
            PickAnatomy.FirstFrameUpEvent -= pickAnatomy_FirstFrameUpEvent;
            OpenAnatomyFinder.FirstFrameUpEvent -= openAnatomyFinder_FirstFrameUpEvent;

            buttonGridThumbs.Dispose();
            anatomyFilter.Dispose();
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

        public IntCoord DeadZone
        {
            get
            {
                if (window.Visible && window.Alpha >= 0.999f)
                {
                    return new IntCoord(window.Left, window.Top, window.Width, window.Height);
                }
                else
                {
                    return new IntCoord(int.MinValue, int.MinValue, int.MinValue, int.MinValue);
                }
            }
        }

        /// <summary>
        /// A hint of where to put the context window.
        /// </summary>
        public IntVector2 DisplayHintLocation { get; private set; }

        /// <summary>
        /// True if the anatomy finder triggered the selection.
        /// </summary>
        public bool TriggeredSelection { get; private set; }

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

        void anatomyController_AnatomyChanged(AnatomyController anatomyController)
        {
            updateSearch();
            anatomyFilter.refreshCategories();
        }

        void searchBox_EventEditTextChange(Widget source, EventArgs e)
        {
            updateSearch();
        }

        private void updateSearch()
        {
            String search = searchBox.Caption;
            anatomyController.findAnatomy(search, anatomyFilter.ActiveFacets);
            clearButton.Visible = !String.IsNullOrEmpty(search);
        }

        void PickAnatomy_FirstFrameDownEvent(EventLayer eventLayer)
        {
            travelTracker.reset();
        }

        void PickAnatomy_OnHeldDown(EventLayer eventLayer)
        {
            travelTracker.traveled(eventLayer.Mouse.RelativePosition);
        }

        void pickAnatomy_FirstFrameUpEvent(EventLayer eventLayer)
        {
            IntVector3 absMouse = eventLayer.Mouse.AbsolutePosition;
            if (eventLayer.EventProcessingAllowed && !travelTracker.TraveledOverLimit)
            {
                if (clickedAnatomy.DoNewClickSearch)
                {
                    SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
                    DisplayHintLocation = new IntVector2(absMouse.x + MouseClickWindowOffset, absMouse.y + MouseClickWindowOffset);
                    TriggeredSelection = false;
                    Ray3 cameraRay = activeWindow.getCameraToViewportRayScreen(absMouse.x, absMouse.y);

                    IEnumerable<Anatomy> matches = anatomyController.findAnatomy(cameraRay);

                    if (matches != null)
                    {
                        clickedAnatomy.setNewResults(matches, eventLayer);
                        anatomyController.processSelection(clickedAnatomy.CurrentMatch, clickedAnatomy.PreviousMatch);
                        clickedAnatomy.moveNext();

                        searchBox.Caption = "Picked";
                        clearButton.Visible = true;
                    }
                    else
                    {
                        clickedAnatomy.clear();
                        anatomyController.processSelection(null, null);
                        clearButton.Visible = false;
                        searchBox.Caption = "";
                    }
                }
                else
                {
                    anatomyController.processSelection(clickedAnatomy.CurrentMatch, clickedAnatomy.PreviousMatch);
                    clickedAnatomy.moveNext();
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
                DisplayHintLocation = new IntVector2(anatomyWindowManager.determineContextWindowX(window.AbsoluteLeft, window.AbsoluteLeft + window.Width), item.AbsoluteTop);
                TriggeredSelection = true;
                anatomyController.processSelection(anatomy, null);
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

        /// <summary>
        /// Shows all anatomy including locked anatomy
        /// </summary>
        /// <param name="anatomy"></param>
        void anatomyController_DisplayAnatomy(Anatomy anatomy)
        {
            //Add item
            ButtonGridItem anatomyItem = anatomyList.addItem("", anatomy.AnatomicalName, "", anatomy);
            if (!anatomyController.ShowPremiumAnatomy && !anatomy.ShowInBasicVersion)
            {
                IntCoord itemCoord = anatomyItem.Coord;
                ImageBox lockedFeatureImage = (ImageBox)anatomyItem.createWidgetT("ImageBox", "ImageBox", 0, itemCoord.Bottom - lockSize, lockSize, lockSize, Align.Left | Align.Top, "LockedFeatureImage");
                lockedFeatureImage.NeedMouseFocus = false;
                lockedFeatureImage.setItemResource("LockedFeature");
            }
            if (anatomyController.SelectedAnatomy.isSelected(anatomy))
            {
                anatomyList.addSelected(anatomyItem);
            }
        }

        /// <summary>
        /// Don't show locked anatomy in the restricted version.
        /// </summary>
        /// <param name="anatomy"></param>
        void anatomyController_DisplayAnatomy_Restricted(Anatomy anatomy)
        {
            if(anatomyController.ShowPremiumAnatomy || anatomy.ShowInBasicVersion)
            {
                //Add item
                ButtonGridItem anatomyItem = anatomyList.addItem("", anatomy.AnatomicalName, "", anatomy);
                if (anatomyController.SelectedAnatomy.isSelected(anatomy))
                {
                    anatomyList.addSelected(anatomyItem);
                }
            }
        }

        void anatomyController_ClearDisplayedAnatomy()
        {
            anatomyList.clear();
        }

        void anatomyList_ItemRemoved(ButtonGrid arg1, ButtonGridItem arg2)
        {
            buttonGridThumbs.itemRemoved(arg2);
        }

        void anatomyController_SearchStarted(SuggestedDisplaySortMode sortMode)
        {
            anatomyList.SuppressLayout = true;
            if (sortMode == SuggestedDisplaySortMode.Alphabetical)
            {
                anatomyList.ItemComprarer = naturalSort;
            }
            else
            {
                anatomyList.ItemComprarer = null;
            }
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

        void filter_MouseButtonClick(Widget source, EventArgs e)
        {
            anatomyFilter.show(source.AbsoluteLeft + source.Width, source.AbsoluteTop);
        }

        void anatomyController_SuggestSearchCaption(String caption)
        {
            clearButton.Visible = true;
            searchBox.Caption = caption;
        }

        void anatomyFilter_FilterChanged(AnatomyFilter source)
        {
            updateSearch();
        }

        void anatomyFilter_TopLevelAnatomyChanged(AnatomyFilter source)
        {
            if (String.IsNullOrEmpty(searchBox.Caption))
            {
                updateSearch();
            }
        }

        protected override bool keepOpenFromPoint(int x, int y)
        {
            return (anatomyFilter.Visible && anatomyFilter.contains(x, y)) || anatomyWindowManager.isContextWindowAtPoint(x, y) || base.keepOpenFromPoint(x, y);
        }

        void updateUndoRedo(LayerController obj)
        {
            undoButton.Enabled = layerController.HasUndo;
            redoButton.Enabled = layerController.HasRedo;
        }
    }
}
