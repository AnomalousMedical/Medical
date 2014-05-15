using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine.Platform;
using Engine;
using System.Drawing;

namespace Medical.GUI
{
    public class AnatomyFinder : MDIDialog
    {
        enum AnatomyFinderEvents
        {
            PickAnatomy,
            ChangeSelectionMode,
            OpenAnatomyFinder,
            ToggleAddMode,
            ToggleRemoveMode,
        }

        enum SelectionMode
        {
            Select,
            Add,
            Remove
        }

        private static readonly int ThumbSize = ScaleHelper.Scaled(50);
        private static readonly int ThumbRenderSize = ThumbSize;
        private static readonly int lockSize = ScaleHelper.Scaled(18);

        private static MessageEvent pickAnatomy;
        private static MessageEvent changeSelectionMode;
        private static MessageEvent openAnatomyFinder;
        private static MessageEvent toggleAdMode;
        private static MessageEvent toggleRemoveMode;
        
        static AnatomyFinder()
        {
            //pickAnatomy = new MessageEvent(AnatomyFinderEvents.PickAnatomy);
            //pickAnatomy.addButton(MouseButtonCode.MB_BUTTON0);
            //DefaultEvents.registerDefaultEvent(pickAnatomy);

            changeSelectionMode = new MessageEvent(AnatomyFinderEvents.ChangeSelectionMode);
            changeSelectionMode.addButton(KeyboardButtonCode.KC_TAB);
            DefaultEvents.registerDefaultEvent(changeSelectionMode);

            openAnatomyFinder = new MessageEvent(AnatomyFinderEvents.OpenAnatomyFinder);
            openAnatomyFinder.addButton(KeyboardButtonCode.KC_LCONTROL);
            openAnatomyFinder.addButton(KeyboardButtonCode.KC_F);
            DefaultEvents.registerDefaultEvent(openAnatomyFinder);

            toggleAdMode = new MessageEvent(AnatomyFinderEvents.ToggleAddMode);
            toggleAdMode.addButton(KeyboardButtonCode.KC_LCONTROL);
            DefaultEvents.registerDefaultEvent(toggleAdMode);

            toggleRemoveMode = new MessageEvent(AnatomyFinderEvents.ToggleRemoveMode);
            toggleRemoveMode.addButton(KeyboardButtonCode.KC_LMENU);
            DefaultEvents.registerDefaultEvent(toggleRemoveMode);
        }

        private HashSetMultiSelectButtonGrid anatomyList;
        private EditBox searchBox;
        private Button clearButton;

        private AnatomyContextWindowManager anatomyWindowManager;

        private SceneViewController sceneViewController;

        private AnatomyController anatomyController;
        private int lastWidth = -1;
        private int lastHeight = -1;

        private Vector3 mouseDownMousePos;
        private const int MOUSE_MOVE_GRACE_PIXELS = 3;

        private ButtonGridLiveThumbnailController<Anatomy> buttonGridThumbs;
        private EventManager eventManager;

        private ButtonGroup<SelectionMode> selectionMode;

        public event Action ShowBuyMessage;

        public AnatomyFinder(AnatomyController anatomyController, SceneViewController sceneViewController, EventManager eventManager)
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            this.eventManager = eventManager;
            eventManager.Mouse.ButtonDown += mouse_ButtonDown;
            eventManager.Mouse.ButtonUp += mouse_ButtonUp;

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

            selectionMode = new ButtonGroup<SelectionMode>();
            selectionMode.addButton(SelectionMode.Select, (Button)window.findWidget("SelectButton"));
            selectionMode.addButton(SelectionMode.Add, (Button)window.findWidget("AddButton"));
            selectionMode.addButton(SelectionMode.Remove, (Button)window.findWidget("RemoveButton"));
            selectionMode.Selection = SelectionMode.Select;

            //pickAnatomy.FirstFrameDownEvent += new MessageEventCallback(pickAnatomy_FirstFrameDownEvent);
            //pickAnatomy.FirstFrameUpEvent += new MessageEventCallback(pickAnatomy_FirstFrameUpEvent);
            changeSelectionMode.FirstFrameUpEvent += new MessageEventCallback(changeSelectionMode_FirstFrameUpEvent);
            openAnatomyFinder.FirstFrameUpEvent += new MessageEventCallback(openAnatomyFinder_FirstFrameUpEvent);
            toggleAdMode.FirstFrameDownEvent += toggleAdMode_FirstFrameDownEvent;
            toggleAdMode.FirstFrameUpEvent += toggleAdMode_FirstFrameUpEvent;
            toggleRemoveMode.FirstFrameDownEvent += toggleRemoveMode_FirstFrameDownEvent;
            toggleRemoveMode.FirstFrameUpEvent += toggleRemoveMode_FirstFrameUpEvent;

            Button unhideAll = window.findWidget("UnhideAll") as Button;
            unhideAll.MouseButtonClick += new MyGUIEvent(unhideAll_MouseButtonClick);

            this.Resized += new EventHandler(AnatomyFinder_Resized);
            fixListItemWidth();

            updateSearch();
        }

        public override void Dispose()
        {
            buttonGridThumbs.Dispose();
            eventManager.Mouse.ButtonDown -= mouse_ButtonDown;
            eventManager.Mouse.ButtonUp -= mouse_ButtonUp;
            anatomyWindowManager.Dispose();
            base.Dispose();
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

        void openAnatomyFinder_FirstFrameUpEvent(EventManager eventManager)
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

        void mouse_ButtonUp(Mouse mouse, MouseButtonCode buttonCode)
        {
            if (buttonCode == MouseButtonCode.MB_BUTTON0)
            {
                pickAnatomy_FirstFrameUpEvent(eventManager);
            }
        }

        void mouse_ButtonDown(Mouse mouse, MouseButtonCode buttonCode)
        {
            if (buttonCode == MouseButtonCode.MB_BUTTON0)
            {
                pickAnatomy_FirstFrameDownEvent(eventManager);
            }
        }

        void pickAnatomy_FirstFrameDownEvent(EventManager eventManager)
        {
            mouseDownMousePos = eventManager.Mouse.getAbsMouse();
        }

        void pickAnatomy_FirstFrameUpEvent(EventManager eventManager)
        {
            Vector3 absMouse = eventManager.Mouse.getAbsMouse();
            Vector3 mouseMovedAmount = mouseDownMousePos - absMouse;
            mouseMovedAmount.x = Math.Abs(mouseMovedAmount.x);
            mouseMovedAmount.y = Math.Abs(mouseMovedAmount.y);
            if (!Gui.Instance.HandledMouseButtons && !InputManager.Instance.isModalAny() && mouseMovedAmount.x < MOUSE_MOVE_GRACE_PIXELS && mouseMovedAmount.y < MOUSE_MOVE_GRACE_PIXELS)
            {
                SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
                Vector2 windowLoc = new Vector2(activeWindow.RenderXLoc, activeWindow.RenderYLoc);
                Size2 windowSize = new Size2(activeWindow.RenderWidth, activeWindow.RenderHeight);
                DisplayHintLocation = new IntVector2((int)absMouse.x, (int)absMouse.y); //Set the hint location before modifying the abs mouse
                absMouse.x = (absMouse.x - windowLoc.x) / windowSize.Width;
                absMouse.y = (absMouse.y - windowLoc.y) / windowSize.Height;
                Ray3 cameraRay = activeWindow.getCameraToViewportRay(absMouse.x, absMouse.y);

                Anatomy bestMatch = anatomyController.findAnatomy(cameraRay);

                processSelection(bestMatch);

                if (anatomyController.SelectedAnatomy.Count > 0)
                {
                    searchBox.Caption = "Clicked";
                    clearButton.Visible = true;
                    if (MedicalConfig.AutoOpenAnatomyFinder && !Visible)
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

        void changeSelectionMode_FirstFrameUpEvent(EventManager eventManager)
        {
            if (!Gui.Instance.HandledKeyboardButtons)
            {
                switch (anatomyController.PickingMode)
                {
                    case AnatomyPickingMode.Group:
                        if (anatomyController.ShowPremiumAnatomy)
                        {
                            anatomyController.PickingMode = AnatomyPickingMode.Individual;
                        }
                        else
                        {
                            anatomyController.PickingMode = AnatomyPickingMode.None;
                        }
                        break;
                    case AnatomyPickingMode.Individual:
                        anatomyController.PickingMode = AnatomyPickingMode.None;
                        break;
                    case AnatomyPickingMode.None:
                        anatomyController.PickingMode = AnatomyPickingMode.Group;
                        break;
                }
            }
        }

        void clearButton_MouseButtonClick(Widget source, EventArgs e)
        {
            searchBox.Caption = "";
            updateSearch();
            InputManager.Instance.setKeyFocusWidget(searchBox);
        }

        void unhideAll_MouseButtonClick(Widget source, EventArgs e)
        {
            TransparencyController.smoothSetAllAlphas(1.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
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
                processSelection(anatomy);
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
                    TransparencyChanger transparencyChanger = selectedAnatomy.TransparencyChanger;
                    if (transparencyChanger.CurrentAlpha >= 0.9999f)
                    {
                        transparencyChanger.smoothBlend(0.7f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
                    }
                    else if (transparencyChanger.CurrentAlpha <= 0.00008f)
                    {
                        transparencyChanger.smoothBlend(1.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
                    }
                    else
                    {
                        transparencyChanger.smoothBlend(0.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
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

            LayerState layers = new LayerState("Temp");
            layers.buildFrom(anatomy.TransparencyChanger.TransparencyInterfaces, 1.0f);

            buttonGridThumbs.itemAdded(arg2, layers, translation, center, anatomy);
        }

        private void processSelection(Anatomy anatomy)
        {
            switch(selectionMode.Selection)
            {
                case SelectionMode.Select:
                    anatomyController.SelectedAnatomy.setSelection(anatomy);
                    break;
                case SelectionMode.Add:
                    anatomyController.SelectedAnatomy.addSelection(anatomy);
                    break;
                case SelectionMode.Remove:
                    anatomyController.SelectedAnatomy.removeSelection(anatomy);
                    break;
            }
        }

        void toggleRemoveMode_FirstFrameUpEvent(EventManager eventManager)
        {
            if(selectionMode.Selection == SelectionMode.Remove)
            {
                selectionMode.Selection = SelectionMode.Select;
            }
        }

        void toggleRemoveMode_FirstFrameDownEvent(EventManager eventManager)
        {
            selectionMode.Selection = SelectionMode.Remove;
        }

        void toggleAdMode_FirstFrameUpEvent(EventManager eventManager)
        {
            if (selectionMode.Selection == SelectionMode.Add)
            {
                selectionMode.Selection = SelectionMode.Select;
            }
        }

        void toggleAdMode_FirstFrameDownEvent(EventManager eventManager)
        {
            selectionMode.Selection = SelectionMode.Add;
        }
    }
}
