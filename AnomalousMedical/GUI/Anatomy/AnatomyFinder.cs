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

        private static readonly int ThumbSize = ScaleHelper.Scaled(50);
        private static readonly int ThumbRenderSize = ThumbSize * 4;

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

        private SingleSelectButtonGrid anatomyList;
        private EditBox searchBox;

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
        private ButtonGridLiveThumbnailController<Anatomy> buttonGridThumbs;

        private EventManager eventManager;

        public AnatomyFinder(AnatomyController anatomyController, SceneViewController sceneViewController, EventManager eventManager)
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            this.eventManager = eventManager;
            eventManager.Mouse.ButtonDown += mouse_ButtonDown;
            eventManager.Mouse.ButtonUp += mouse_ButtonUp;

            this.anatomyController = anatomyController;
            anatomyController.AnatomyChanged += new EventHandler(anatomyController_AnatomyChanged);
            anatomyController.ShowPremiumAnatomyChanged += anatomyController_ShowPremiumAnatomyChanged;
            this.sceneViewController = sceneViewController;
            anatomyWindowManager = new AnatomyContextWindowManager(sceneViewController, anatomyController, this);

            ScrollView anatomyScroll = (ScrollView)window.findWidget("AnatomyList");
            anatomyList = new SingleSelectButtonGrid(anatomyScroll, new ButtonGridListLayout(), new ButtonGridItemNaturalSort());
            anatomyList.ItemActivated += new EventHandler(anatomyList_ItemActivated);
            anatomyList.SelectedValueChanged += new EventHandler(anatomyList_SelectedValueChanged);
            anatomyList.ItemAdded += anatomyList_ItemAdded;
            anatomyList.ItemRemoved += anatomyList_ItemRemoved;

            buttonGridThumbs = new ButtonGridLiveThumbnailController<Anatomy>("AnatomyFinder_", new IntSize2(ThumbRenderSize, ThumbRenderSize), sceneViewController, anatomyList, anatomyScroll);
            buttonGridThumbs.AllowThumbUpdate = false;

            searchBox = (EditBox)window.findWidget("SearchBox");
            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);

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
            foreach (Anatomy relatedAnatomy in anatomy.RelatedAnatomy)
            {
                ButtonGridItem newItem = addAnatomyToList(relatedAnatomy);
                if (itemToSelect == null)
                {
                    itemToSelect = newItem;
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
            String searchTerm = searchBox.Caption;
            anatomyList.SuppressLayout = true;
            anatomyList.clear();
            if (searchTerm.Length == 0)
            {
                foreach (Anatomy anatomy in anatomyController.SearchList.TopLevelAnatomy)
                {
                    addAnatomyToList(anatomy);
                }
            }
            else
            {
                foreach (Anatomy anatomy in anatomyController.SearchList.findMatchingAnatomy(searchTerm, 35))
                {
                    addAnatomyToList(anatomy);
                }
            }
            anatomyList.SuppressLayout = false;
            anatomyList.layout();
        }

        private void changeSelectedAnatomy(int left, int top)
        {
            ButtonGridItem selectedItem = anatomyList.SelectedItem;
            if (selectedItem != null)
            {
                Anatomy anatomy = buttonGridThumbs.getUserObject(selectedItem);
                if (anatomyController.ShowPremiumAnatomy || anatomy.ShowInBasicVersion)
                {
                    int deadLeft = int.MinValue;
                    int deadRight = int.MinValue;
                    int deadBottom = int.MinValue;
                    int deadTop = int.MinValue;

                    if (window.Visible)
                    {
                        deadLeft = window.Left;
                        deadRight = window.Right;
                        deadBottom = window.Bottom;
                        deadTop = window.Top;
                    }

                    anatomyWindowManager.showWindow(anatomy, left, top, deadLeft, deadRight, deadTop, deadBottom);
                }
                else
                {
                    showNagMessage();
                }
            }
            else
            {
                anatomyWindowManager.closeUnpinnedWindow();
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
                    foreach (AnatomyIdentifier anatomy in matches)
                    {
                        ButtonGridItem newItem = addAnatomyToList(anatomy);
                        if (itemToSelect == null)
                        {
                            itemToSelect = newItem;
                        }
                        foreach (AnatomyTag tag in anatomy.Tags)
                        {
                            anatomyTags.Add(tag.Tag);
                        }
                    }
                    foreach (AnatomyTagGroup tagGroup in anatomyController.TagManager.Groups)
                    {
                        if (tagGroup.ShowInClickSearch && anatomyTags.Contains(tagGroup.AnatomicalName))
                        {
                            addAnatomyToList(tagGroup);
                        }
                    }
                    if (matches.Count > 0)
                    {
                        if (!this.Visible)
                        {
                            this.Visible = true;
                        }

                        bool showPremium = anatomyController.ShowPremiumAnatomy;
                        searchBox.Caption = "Clicked";
                        if (anatomyController.PickingMode == AnatomyPickingMode.Group && matches[0].AllowGroupSelection || !showPremium)
                        {
                            //Search all tags, not just the main one in case the main one doesn't exist.
                            foreach (AnatomyTag tag in matches[0].Tags)
                            {
                                itemToSelect = anatomyList.findItemByCaption(tag.Tag);
                                if (itemToSelect != null && (showPremium || buttonGridThumbs.getUserObject(itemToSelect).ShowInBasicVersion))
                                {
                                    break;
                                }
                                else
                                {
                                    itemToSelect = null;
                                }
                            }
                        }
                        anatomyList.SelectedItem = itemToSelect;
                    }
                    else
                    {
                        searchBox.Caption = "";
                        updateSearch();
                    }
                    changeSelectedAnatomy((int)eventManager.Mouse.getAbsMouse().x, (int)eventManager.Mouse.getAbsMouse().y);

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
            TransparencyController.smoothSetAllAlphas(1.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
        }

        void anatomyList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (allowAnatomySelectionChanges)
            {
                int top = 0;
                if (anatomyList.SelectedItem != null)
                {
                    top = anatomyList.SelectedItem.AbsoluteTop;
                }
                changeSelectedAnatomy(window.Right, top);
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
                Anatomy selectedAnatomy = buttonGridThumbs.getUserObject(selectedItem);
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
                    showNagMessage();
                }
            }
        }

        private ButtonGridItem addAnatomyToList(Anatomy anatomy)
        {
            //Add item
            ButtonGridItem anatomyItem = anatomyList.addItem("", anatomy.AnatomicalName, "", anatomy);
            if (this.Visible)
            {
                startThumbnailCoroutine();
            }
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
            //while (currentThumbnailIndex < anatomyList.Count)
            //{
            //    ButtonGridItem anatomyItem = anatomyList.getItem(currentThumbnailIndex);
            //    String imageName;
            //    bool generatedThumb = anatomyController.getThumbnail((Anatomy)anatomyItem.UserObject, sceneViewController.ActiveWindow.Camera.getFOVy(), out imageName);
            //    anatomyItem.setImage(imageName);
                
            //    ++currentThumbnailIndex;

            //    if (generatedThumb)
            //    {
            //        //Only delay if the thumbnail had to be generated. Otherwise using the existing thumbnail is fast.
            //        yield return Coroutine.Wait(0);
            //    }
            //}
            runningThumbnailCoroutine = false;
            yield break;
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            int itemCount = anatomyList.Count;
            float fovy = sceneViewController.ActiveWindow.Camera.getFOVy();
            startThumbnailCoroutine();
            buttonGridThumbs.AllowThumbUpdate = true;
        }

        protected override void onClosed(EventArgs args)
        {
            buttonGridThumbs.AllowThumbUpdate = false;
            base.onClosed(args);
        }

        private static void showNagMessage()
        {
            MessageBox.show("Placeholder for nag message", "Placeholder", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
        }

        void anatomyController_ShowPremiumAnatomyChanged(AnatomyController source, bool arg)
        {
            //This gets rid of the locks, clear thumbs and update the search.
            anatomyController.clearThumbs();
            updateSearch();
        }

        void anatomyList_ItemRemoved(ButtonGrid arg1, ButtonGridItem arg2)
        {
            buttonGridThumbs.itemRemoved(arg2);
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

            //using (Bitmap thumb = imageRenderer.renderImage(imageProperties))
            //{
            //    if (!ShowPremiumAnatomy && !anatomy.ShowInBasicVersion)
            //    {
            //        if (lockImage == null)
            //        {
            //            Assembly assembly = this.GetType().Assembly;
            //            lockImage = (Bitmap)Bitmap.FromStream(assembly.GetManifestResourceStream("Medical.Resources.LockedFeature.png"));

            //            lockImageDest = new Rectangle(0, 0, imageProperties.Width / 3, imageProperties.Height / 3);
            //            lockImageDest.Y = imageProperties.Height - lockImageDest.Height;
            //        }
            //        using (Graphics g = Graphics.FromImage(thumb))
            //        {
            //            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //            g.DrawImage(lockImage, lockImageDest);
            //        }
            //    }
            //    imageName = imageAtlas.addImage(anatomy.AnatomicalName, thumb);
            //}
        }
    }
}
