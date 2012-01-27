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
        }

        private static MessageEvent pickAnatomy;
        private static MessageEvent changeSelectionMode;
        private static MessageEvent openAnatomyFinder;

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
        }

        private ButtonGrid anatomyList;
        private Edit searchBox;

        private AnatomyContextWindowManager anatomyWindowManager;

        private SceneViewController sceneViewController;

        private AnatomyController anatomyController;
        private int lastWidth = -1;
        private int lastHeight = -1;

        private Vector3 mouseDownMousePos;
        private const int MOUSE_MOVE_GRACE_PIXELS = 3;

        private bool runningThumbnailCoroutine = false;
        private int currentThumbnailIndex = 0;
        private bool allowAnatomySelectionChanges = true;

        private EventManager eventManager;

        public AnatomyFinder(AnatomyController anatomyController, SceneViewController sceneViewController, EventManager eventManager)
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            this.eventManager = eventManager;
            eventManager.Mouse.ButtonDown += mouse_ButtonDown;
            eventManager.Mouse.ButtonUp += mouse_ButtonUp;

            this.anatomyController = anatomyController;
            anatomyController.AnatomyChanged += new EventHandler(anatomyController_AnatomyChanged);
            this.sceneViewController = sceneViewController;
            anatomyWindowManager = new AnatomyContextWindowManager(sceneViewController, anatomyController, this);

            anatomyList = new ButtonGrid((ScrollView)window.findWidget("AnatomyList"), new ButtonGridListLayout(), new ButtonGridItemNaturalSort());
            anatomyList.ItemActivated += new EventHandler(anatomyList_ItemActivated);
            anatomyList.SelectedValueChanged += new EventHandler(anatomyList_SelectedValueChanged);

            searchBox = (Edit)window.findWidget("SearchBox");
            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);
            searchBox.KeyButtonReleased += new MyGUIEvent(searchBox_KeyButtonReleased);

            //pickAnatomy.FirstFrameDownEvent += new MessageEventCallback(pickAnatomy_FirstFrameDownEvent);
            //pickAnatomy.FirstFrameUpEvent += new MessageEventCallback(pickAnatomy_FirstFrameUpEvent);
            changeSelectionMode.FirstFrameUpEvent += new MessageEventCallback(changeSelectionMode_FirstFrameUpEvent);
            openAnatomyFinder.FirstFrameUpEvent += new MessageEventCallback(openAnatomyFinder_FirstFrameUpEvent);

            Button clearButton = window.findWidget("ClearButton") as Button;
            clearButton.MouseButtonClick += new MyGUIEvent(clearButton_MouseButtonClick);

            Button unhideAll = window.findWidget("UnhideAll") as Button;
            unhideAll.MouseButtonClick += new MyGUIEvent(unhideAll_MouseButtonClick);

            this.Resized += new EventHandler(AnatomyFinder_Resized);
            fixListItemWidth();

            updateSearch();
        }

        public override void Dispose()
        {
            eventManager.Mouse.ButtonDown -= mouse_ButtonDown;
            eventManager.Mouse.ButtonUp -= mouse_ButtonUp;
            anatomyWindowManager.Dispose();
            base.Dispose();
        }

        public void sceneUnloading()
        {
            anatomyWindowManager.sceneUnloading();
        }

        public void showRelatedAnatomy(Anatomy anatomy)
        {
            if (!Visible)
            {
                Visible = true;
            }
            anatomyList.SuppressLayout = true;
            anatomyList.clear();
            searchBox.Caption = String.Format("Related to {0}", anatomy.AnatomicalName);
            ButtonGridItem itemToSelect = null;
            bool showPremium = anatomyController.ShowPremiumAnatomy;
            foreach (Anatomy relatedAnatomy in anatomy.RelatedAnatomy)
            {
                if (showPremium || relatedAnatomy.ShowInBasicVersion)
                {
                    ButtonGridItem newItem = addAnatomyToList(relatedAnatomy);
                    if (itemToSelect == null)
                    {
                        itemToSelect = newItem;
                    }
                }
            }
            anatomyList.SuppressLayout = false;
            anatomyList.layout();
        }

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
            }
        }

        void anatomyController_AnatomyChanged(object sender, EventArgs e)
        {
            updateSearch();
        }

        void searchBox_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            if (ke.Key == KeyboardButtonCode.KC_RETURN)
            {
                ButtonGridItem selectedItem = anatomyList.SelectedItem;
                if (selectedItem == null && anatomyList.Count > 0)
                {
                    anatomyList.SelectedItem = anatomyList.getItem(0);
                }
                toggleAnatomyTransparency();
            }
        }

        void searchBox_EventEditTextChange(Widget source, EventArgs e)
        {
            updateSearch();
        }

        private void updateSearch()
        {
            String searchTerm = searchBox.Caption;
            anatomyList.SuppressLayout = true;
            anatomyList.clear();
            if (searchTerm.Length == 0)
            {
                foreach (Anatomy anatomy in anatomyController.AnatomyTree.TopLevelAnatomy)
                {
                    addAnatomyToList(anatomy);
                }
            }
            else
            {
                foreach (Anatomy anatomy in anatomyController.SearchList.findMatchingAnatomy(searchTerm, 35, anatomyController.ShowPremiumAnatomy))
                {
                    addAnatomyToList(anatomy);
                }
            }
            anatomyList.SuppressLayout = false;
            anatomyList.layout();
        }

        private AnatomyContextWindow changeSelectedAnatomy(int left, int top)
        {
            ButtonGridItem selectedItem = anatomyList.SelectedItem;
            if (selectedItem != null)
            {
                return anatomyWindowManager.showWindow((Anatomy)selectedItem.UserObject, left, top, window.Left, window.Right, window.Top, window.Bottom);
            }
            else
            {
                anatomyWindowManager.closeUnpinnedWindow();
                return null;
            }
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
            allowAnatomySelectionChanges = false;
            Vector3 absMouse = eventManager.Mouse.getAbsMouse();
            Vector3 mouseMovedAmount = mouseDownMousePos - absMouse;
            mouseMovedAmount.x = Math.Abs(mouseMovedAmount.x);
            mouseMovedAmount.y = Math.Abs(mouseMovedAmount.y);
            if (!Gui.Instance.HandledMouseButtons && !InputManager.Instance.isModalAny() && mouseMovedAmount.x < MOUSE_MOVE_GRACE_PIXELS && mouseMovedAmount.y < MOUSE_MOVE_GRACE_PIXELS)
            {
                if (anatomyController.PickingMode != AnatomyPickingMode.None)
                {
                    anatomyList.SuppressLayout = true;
                    anatomyList.clear();

                    SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
                    Vector2 windowLoc = new Vector2(activeWindow.RenderXLoc, activeWindow.RenderYLoc);
                    Size2 windowSize = new Size2(activeWindow.RenderWidth, activeWindow.RenderHeight);
                    absMouse.x = (absMouse.x - windowLoc.x) / windowSize.Width;
                    absMouse.y = (absMouse.y - windowLoc.y) / windowSize.Height;
                    Ray3 cameraRay = activeWindow.getCameraToViewportRay(absMouse.x, absMouse.y);
                    List<AnatomyIdentifier> matches = AnatomyManager.findAnatomy(cameraRay);

                    HashSet<String> anatomyTags = new HashSet<String>();
                    ButtonGridItem itemToSelect = null;
                    bool showPremium = anatomyController.ShowPremiumAnatomy;
                    foreach (AnatomyIdentifier anatomy in matches)
                    {
                        if (showPremium || anatomy.ShowInBasicVersion)
                        {
                            ButtonGridItem newItem = addAnatomyToList(anatomy);
                            if (itemToSelect == null)
                            {
                                itemToSelect = newItem;
                            }
                        }
                        foreach (AnatomyTag tag in anatomy.Tags)
                        {
                            anatomyTags.Add(tag.Tag);
                        }
                    }
                    foreach (AnatomyTagGroup tagGroup in anatomyController.TagManager.Groups)
                    {
                        if (tagGroup.ShowInClickSearch && (showPremium || tagGroup.ShowInBasicVersion) && anatomyTags.Contains(tagGroup.AnatomicalName))
                        {
                            addAnatomyToList(tagGroup);
                        }
                    }
                    if (matches.Count > 0)
                    {
                        searchBox.Caption = "Clicked";
                        if (anatomyController.PickingMode == AnatomyPickingMode.Group && matches[0].AllowGroupSelection)
                        {
                            AnatomyTag mainTag = matches[0].Tags.First();
                            if (mainTag != null)
                            {
                                itemToSelect = anatomyList.findItemByCaption(mainTag.Tag);
                            }
                        }
                        anatomyList.SelectedItem = itemToSelect;
                    }
                    else
                    {
                        searchBox.Caption = "";
                        updateSearch();
                    }
                    AnatomyContextWindow activeAnatomyWindow = changeSelectedAnatomy((int)eventManager.Mouse.getAbsMouse().x, (int)eventManager.Mouse.getAbsMouse().y);

                    anatomyList.SuppressLayout = false;
                    anatomyList.layout();
                }
                else
                {
                    anatomyWindowManager.closeUnpinnedWindow();
                }
            }
            allowAnatomySelectionChanges = true;
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
            TransparencyController.smoothSetAllAlphas(1.0f, MedicalConfig.CameraTransitionTime);
        }

        void anatomyList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (allowAnatomySelectionChanges)
            {
                AnatomyContextWindow contextWindow = changeSelectedAnatomy(window.Right, anatomyList.SelectedItem.AbsoluteTop);
                if (contextWindow != null)
                {
                    //float x = window.Right;
                    //float y = anatomyList.SelectedItem.AbsoluteTop;
                    //if (x + contextWindow.Width > Gui.Instance.getViewWidth())
                    //{
                    //    x = window.Left - contextWindow.Width;
                    //}
                    //contextWindow.Position = new Vector2(x, y);
                    //contextWindow.ensureVisible();
                }
            }
        }

        void anatomyList_ItemActivated(object sender, EventArgs e)
        {
            toggleAnatomyTransparency();
        }

        private void toggleAnatomyTransparency()
        {
            ButtonGridItem selectedItem = anatomyList.SelectedItem;
            if (selectedItem != null)
            {
                Anatomy selectedAnatomy = (Anatomy)selectedItem.UserObject;
                TransparencyChanger transparencyChanger = selectedAnatomy.TransparencyChanger;
                if (transparencyChanger.CurrentAlpha == 1.0f)
                {
                    transparencyChanger.smoothBlend(0.7f, MedicalConfig.CameraTransitionTime);
                }
                else if (transparencyChanger.CurrentAlpha == 0.0f)
                {
                    transparencyChanger.smoothBlend(1.0f, MedicalConfig.CameraTransitionTime);
                }
                else
                {
                    transparencyChanger.smoothBlend(0.0f, MedicalConfig.CameraTransitionTime);
                }
            }
        }

        private ButtonGridItem addAnatomyToList(Anatomy anatomy)
        {
            //Add item
            ButtonGridItem anatomyItem = anatomyList.addItem("", anatomy.AnatomicalName, "");
            if (this.Visible)
            {
                startThumbnailCoroutine();
            }
            anatomyItem.UserObject = anatomy;
            return anatomyItem;
        }

        private void startThumbnailCoroutine()
        {
            if (!runningThumbnailCoroutine)
            {
                runningThumbnailCoroutine = true;
                Coroutine.Start(cogenerateThumbnails());
            }
            currentThumbnailIndex = 0;
        }

        IEnumerator<YieldAction> cogenerateThumbnails()
        {
            while (currentThumbnailIndex < anatomyList.Count)
            {
                ButtonGridItem anatomyItem = anatomyList.getItem(currentThumbnailIndex);
                String imageName;
                bool generatedThumb = anatomyController.getThumbnail((Anatomy)anatomyItem.UserObject, sceneViewController.ActiveWindow.Camera.getFOVy(), out imageName);
                anatomyItem.setImage(imageName);
                
                ++currentThumbnailIndex;

                if (generatedThumb)
                {
                    //Only delay if the thumbnail had to be generated. Otherwise using the existing thumbnail is fast.
                    yield return Coroutine.Wait(0);
                }
            }
            runningThumbnailCoroutine = false;
            yield break;
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            int itemCount = anatomyList.Count;
            float fovy = sceneViewController.ActiveWindow.Camera.getFOVy();
            startThumbnailCoroutine();
        }
    }
}
