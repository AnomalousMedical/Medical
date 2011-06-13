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
    enum AnatomyFinderEvents
    {
        PickAnatomy,
        ChangeSelectionMode,
    }

    public class AnatomyFinder : MDIDialog
    {
        private static MessageEvent pickAnatomy;
        private static MessageEvent changeSelectionMode;

        static AnatomyFinder()
        {
            pickAnatomy = new MessageEvent(AnatomyFinderEvents.PickAnatomy);
            pickAnatomy.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(pickAnatomy);

            changeSelectionMode = new MessageEvent(AnatomyFinderEvents.ChangeSelectionMode);
            changeSelectionMode.addButton(KeyboardButtonCode.KC_TAB);
            DefaultEvents.registerDefaultEvent(changeSelectionMode);
        }

        private ButtonGrid anatomyList;
        private Edit searchBox;

        private AnatomyContextWindowManager anatomyWindowManager;

        private SceneViewController sceneViewController;

        private Button groupButton;
        private Button individualButton;
        private Button noneButton;
        private ButtonGroup pickingModeGroup;

        private AnatomyController anatomyController;
        private int lastWidth = -1;
        private int lastHeight = -1;

        public AnatomyFinder(AnatomyController anatomyController, SceneViewController sceneViewController)
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            this.anatomyController = anatomyController;
            anatomyController.AnatomyChanged += new EventHandler(anatomyController_AnatomyChanged);
            this.sceneViewController = sceneViewController;
            anatomyWindowManager = new AnatomyContextWindowManager(sceneViewController, anatomyController);

            anatomyList = new ButtonGrid((ScrollView)window.findWidget("AnatomyList"), new ButtonGridListLayout(), new ButtonGridItemNaturalSort());
            anatomyList.ItemActivated += new EventHandler(anatomyList_ItemActivated);
            anatomyList.SelectedValueChanged += new EventHandler(anatomyList_SelectedValueChanged);

            searchBox = (Edit)window.findWidget("SearchBox");
            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);

            pickAnatomy.FirstFrameUpEvent += new MessageEventCallback(pickAnatomy_FirstFrameUpEvent);
            changeSelectionMode.FirstFrameUpEvent += new MessageEventCallback(changeSelectionMode_FirstFrameUpEvent);

            Button clearButton = window.findWidget("ClearButton") as Button;
            clearButton.MouseButtonClick += new MyGUIEvent(clearButton_MouseButtonClick);

            Button unhideAll = window.findWidget("UnhideAll") as Button;
            unhideAll.MouseButtonClick += new MyGUIEvent(unhideAll_MouseButtonClick);

            Button openAll = (Button)window.findWidget("OpenAll");
            openAll.MouseButtonClick += new MyGUIEvent(openAll_MouseButtonClick);

            pickingModeGroup = new ButtonGroup();
            groupButton = (Button)window.findWidget("GroupButton");
            pickingModeGroup.addButton(groupButton);
            individualButton = (Button)window.findWidget("IndividualButton");
            pickingModeGroup.addButton(individualButton);
            noneButton = (Button)window.findWidget("NoneButton");
            pickingModeGroup.addButton(noneButton);
            pickingModeGroup.SelectedButton = groupButton;

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
        }

        public override void deserialize(ConfigFile configFile)
        {
            base.deserialize(configFile);
            fixListItemWidth();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
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
                foreach (AnatomyTagGroup tagGroup in anatomyController.TagManager.Groups)
                {
                    addAnatomyToList(tagGroup);
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

        private AnatomyContextWindow changeSelectedAnatomy(int left, int top)
        {
            ButtonGridItem selectedItem = anatomyList.SelectedItem;
            if (selectedItem != null)
            {
                return anatomyWindowManager.showWindow((Anatomy)selectedItem.UserObject, left, top);
            }
            else
            {
                anatomyWindowManager.closeUnpinnedWindow();
                return null;
            }
        }

        void pickAnatomy_FirstFrameUpEvent(EventManager eventManager)
        {
            if (!Gui.Instance.HandledMouseButtons && !InputManager.Instance.isModalAny() && pickingModeGroup.SelectedButton != noneButton)
            {
                anatomyList.SuppressLayout = true;
                anatomyList.clear();

                Vector3 absMouse = eventManager.Mouse.getAbsMouse();
                SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
                Vector2 windowLoc = activeWindow.Location;
                Size2 windowSize = activeWindow.WorkingSize;
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
                    if (anatomyTags.Contains(tagGroup.AnatomicalName))
                    {
                        addAnatomyToList(tagGroup);
                    }
                }
                if (matches.Count > 0)
                {
                    searchBox.Caption = "Clicked";
                    if (pickingModeGroup.SelectedButton == groupButton && matches[0].AllowGroupSelection)
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
                if (activeAnatomyWindow != null)
                {
                    //activeAnatomyWindow.Position = new Vector2(eventManager.Mouse.getAbsMouse().x, eventManager.Mouse.getAbsMouse().y);
                    //activeAnatomyWindow.ensureVisible();
                }

                anatomyList.SuppressLayout = false;
                anatomyList.layout();
            }
        }

        void changeSelectionMode_FirstFrameUpEvent(EventManager eventManager)
        {
            if (!Gui.Instance.HandledKeyboardButtons)
            {
                if (pickingModeGroup.SelectedButton == groupButton)
                {
                    pickingModeGroup.SelectedButton = individualButton;
                }
                else if (pickingModeGroup.SelectedButton == individualButton)
                {
                    pickingModeGroup.SelectedButton = noneButton;
                }
                else
                {
                    pickingModeGroup.SelectedButton = groupButton;
                }
            }
        }

        void clearButton_MouseButtonClick(Widget source, EventArgs e)
        {
            searchBox.Caption = "";
            updateSearch();
        }

        void unhideAll_MouseButtonClick(Widget source, EventArgs e)
        {
            TransparencyController.smoothSetAllAlphas(1.0f, MedicalConfig.TransparencyChangeMultiplier);
        }

        void anatomyList_SelectedValueChanged(object sender, EventArgs e)
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

        void anatomyList_ItemActivated(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = anatomyList.SelectedItem;
            if (selectedItem != null)
            {
                Anatomy selectedAnatomy = (Anatomy)selectedItem.UserObject;
                TransparencyChanger transparencyChanger = selectedAnatomy.TransparencyChanger;
                if (transparencyChanger.CurrentAlpha == 1.0f)
                {
                    transparencyChanger.smoothBlend(0.7f, MedicalConfig.TransparencyChangeMultiplier);
                }
                else if (transparencyChanger.CurrentAlpha == 0.0f)
                {
                    transparencyChanger.smoothBlend(1.0f, MedicalConfig.TransparencyChangeMultiplier);
                }
                else
                {
                    transparencyChanger.smoothBlend(0.0f, MedicalConfig.TransparencyChangeMultiplier);
                }
            }
        }

        void openAll_MouseButtonClick(Widget source, EventArgs e)
        {
            int viewHeight = Gui.Instance.getViewHeight();
            int itemCount = anatomyList.Count;
            int left = window.AbsoluteLeft + window.Width;
            int top = window.Top;
            int widest = 0;
            for (int i = 0; i < itemCount; ++i)
            {
                AnatomyContextWindow contextWindow = anatomyWindowManager.showWindow((Anatomy)anatomyList.getItem(i).UserObject, left, top);
                if (contextWindow.Width > widest)
                {
                    widest = contextWindow.Width;
                }
                top += contextWindow.Height;
                if (top > viewHeight)
                {
                    left += widest;
                    widest = 0;
                    top = window.Top;
                    contextWindow.show(left, top);
                    top += contextWindow.Height;
                }
                contextWindow.pinOpen();
            }
        }

        private ButtonGridItem addAnatomyToList(Anatomy anatomy)
        {
            //Add item
            ButtonGridItem anatomyItem = anatomyList.addItem("", anatomy.AnatomicalName, "");
            if (this.Visible)
            {
                String imageName = anatomyController.getThumbnail(anatomy, sceneViewController.ActiveWindow.Camera.getFOVy() * 0.0174532925f);
                anatomyItem.setImage(imageName);
            }
            anatomyItem.UserObject = anatomy;
            return anatomyItem;
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);
            int itemCount = anatomyList.Count;
            float fovy = sceneViewController.ActiveWindow.Camera.getFOVy() * 0.0174532925f;
            for (int i = 0; i < itemCount; ++i)
            {
                ButtonGridItem item = anatomyList.getItem(i);
                item.setImage(anatomyController.getThumbnail((Anatomy)item.UserObject, fovy));
            }
        }
    }
}
