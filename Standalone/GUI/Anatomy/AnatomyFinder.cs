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
    enum AnatomyFinderEvents
    {
        PickAnatomy,
        ChangeSelectionMode,
    }

    public class AnatomyFinder : Dialog
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
        private ButtonGroup pickingModeGroup;

        private AnatomyController anatomyController;

        public AnatomyFinder(AnatomyController anatomyController, SceneViewController sceneViewController)
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            this.anatomyController = anatomyController;
            anatomyController.AnatomyChanged += new EventHandler(anatomyController_AnatomyChanged);
            this.sceneViewController = sceneViewController;
            anatomyWindowManager = new AnatomyContextWindowManager(sceneViewController);

            anatomyList = new ButtonGrid((ScrollView)window.findWidget("AnatomyList"), new ButtonGridItemNaturalSort());
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

            pickingModeGroup = new ButtonGroup();
            groupButton = (Button)window.findWidget("GroupButton");
            pickingModeGroup.addButton(groupButton);
            individualButton = (Button)window.findWidget("IndividualButton");
            pickingModeGroup.addButton(individualButton);
            pickingModeGroup.SelectedButton = groupButton;
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

        private AnatomyContextWindow changeSelectedAnatomy()
        {
            ButtonGridItem selectedItem = anatomyList.SelectedItem;
            if (selectedItem != null)
            {
                return anatomyWindowManager.showWindow((Anatomy)selectedItem.UserObject);
            }
            else
            {
                anatomyWindowManager.closeUnpinnedWindow();
                return null;
            }
        }

        void pickAnatomy_FirstFrameUpEvent(EventManager eventManager)
        {
            if (!Gui.Instance.HandledMouseButtons)
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
                AnatomyContextWindow activeAnatomyWindow = changeSelectedAnatomy();
                if (activeAnatomyWindow != null)
                {
                    activeAnatomyWindow.Position = new Vector2(eventManager.Mouse.getAbsMouse().x, eventManager.Mouse.getAbsMouse().y);
                    activeAnatomyWindow.ensureVisible();
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
            AnatomyContextWindow contextWindow = changeSelectedAnatomy();
            if (contextWindow != null)
            {
                float x = window.Right;
                float y = anatomyList.SelectedItem.AbsoluteTop;
                if (x + contextWindow.Width > Gui.Instance.getViewWidth())
                {
                    x = window.Left - contextWindow.Width;
                }
                contextWindow.Position = new Vector2(x, y);
                contextWindow.ensureVisible();
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

        private ButtonGridItem addAnatomyToList(Anatomy anatomy)
        {
            ButtonGridItem anatomyItem = anatomyList.addItem("", anatomy.AnatomicalName);
            anatomyItem.UserObject = anatomy;
            return anatomyItem;
        }
    }
}
