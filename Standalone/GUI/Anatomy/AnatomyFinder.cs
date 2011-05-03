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
        PickAnatomy
    }

    public class AnatomyFinder : Dialog
    {
        private static MessageEvent pickAnatomy;

        static AnatomyFinder()
        {
            pickAnatomy = new MessageEvent(AnatomyFinderEvents.PickAnatomy);
            pickAnatomy.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(pickAnatomy);
        }

        private MultiList anatomyList;
        private Edit searchBox;

        private AnatomyContextWindowManager anatomyWindowManager = new AnatomyContextWindowManager();
        private AnatomyTagManager anatomyTagManager = new AnatomyTagManager();
        private AnatomySearchList anatomySearchList = new AnatomySearchList();
        private List<AnatomyTagGroup> topLevelGroups = new List<AnatomyTagGroup>();

        private SceneViewController sceneViewController;

        private ButtonGroup showModeGroup = new ButtonGroup();
        private Button focusButton;
        private Button toggleButton;

        public AnatomyFinder(SceneViewController sceneViewController)
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            this.sceneViewController = sceneViewController;

            anatomyList = (MultiList)window.findWidget("AnatomyList");
            anatomyList.addColumn("Anatomy", anatomyList.Width);
            anatomyList.ListChangePosition += new MyGUIEvent(anatomyList_ListChangePosition);
            anatomyList.ListSelectAccept += new MyGUIEvent(anatomyList_ListSelectAccept);

            searchBox = (Edit)window.findWidget("SearchBox");
            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);

            focusButton = (Button)window.findWidget("FocusButton");
            showModeGroup.addButton(focusButton);
            toggleButton = (Button)window.findWidget("ToggleButton");
            showModeGroup.addButton(toggleButton);
            showModeGroup.SelectedButton = toggleButton;

            pickAnatomy.FirstFrameUpEvent += new MessageEventCallback(pickAnatomy_FirstFrameUpEvent);

            Button clearButton = window.findWidget("ClearButton") as Button;
            clearButton.MouseButtonClick += new MyGUIEvent(clearButton_MouseButtonClick);
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
                anatomyList.addItem(tagGroup.AnatomicalName, tagGroup);
                topLevelGroups.Add(tagGroup);
                anatomySearchList.addAnatomy(tagGroup);
            }
        }

        public void sceneUnloading()
        {
            anatomyList.removeAllItems();
            anatomyTagManager.clear();
            anatomySearchList.clear();
            topLevelGroups.Clear();
        }

        void searchBox_EventEditTextChange(Widget source, EventArgs e)
        {
            updateSearch();
        }

        private void updateSearch()
        {
            String searchTerm = searchBox.Caption;
            anatomyList.removeAllItems();
            if (searchTerm.Length == 0)
            {
                foreach (AnatomyTagGroup tagGroup in topLevelGroups)
                {
                    anatomyList.addItem(tagGroup.AnatomicalName, tagGroup);
                }
            }
            else
            {
                foreach (Anatomy anatomy in anatomySearchList.findMatchingAnatomy(searchTerm, 35))
                {
                    anatomyList.addItem(anatomy.AnatomicalName, anatomy);
                }
            }
        }

        private void changeSelectedAnatomy()
        {
            if (anatomyList.hasItemSelected())
            {
                anatomyWindowManager.showWindow((Anatomy)anatomyList.getItemDataAt(anatomyList.getIndexSelected()));
            }
            else
            {
                anatomyWindowManager.closeUnpinnedWindow();
            }
        }

        void anatomyList_ListChangePosition(Widget source, EventArgs e)
        {
            changeSelectedAnatomy();
        }

        void pickAnatomy_FirstFrameUpEvent(EventManager eventManager)
        {
            if (!Gui.Instance.HandledMouseButtons)
            {
                anatomyList.removeAllItems();

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                Vector3 absMouse = eventManager.Mouse.getAbsMouse();
                SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
                Vector2 windowLoc = activeWindow.Location;
                Size2 windowSize = activeWindow.WorkingSize;
                absMouse.x = (absMouse.x - windowLoc.x) / windowSize.Width;
                absMouse.y = (absMouse.y - windowLoc.y) / windowSize.Width;
                Ray3 cameraRay = activeWindow.getCameraToViewportRay(absMouse.x, absMouse.y);
                List<AnatomyIdentifier> matches = AnatomyManager.findAnatomy(cameraRay);
                HashSet<String> anatomyTags = new HashSet<String>();
                foreach (AnatomyIdentifier anatomy in matches)
                {
                    anatomyList.addItem(anatomy.AnatomicalName, anatomy);
                    foreach (AnatomyTag tag in anatomy.Tags)
                    {
                        anatomyTags.Add(tag.Tag);
                    }
                }
                foreach (AnatomyTagGroup tagGroup in topLevelGroups)
                {
                    if (anatomyTags.Contains(tagGroup.AnatomicalName))
                    {
                        anatomyList.addItem(tagGroup.AnatomicalName, tagGroup);
                    }
                }
                searchBox.Caption = "Clicked";
                if (matches.Count > 0)
                {
                    anatomyList.setIndexSelected(0);
                    changeSelectedAnatomy();
                }

                sw.Stop();
                Logging.Log.Debug("Picking took {0} ms", sw.ElapsedMilliseconds);
            }
        }

        void clearButton_MouseButtonClick(Widget source, EventArgs e)
        {
            searchBox.Caption = "";
            updateSearch();
        }

        void anatomyList_ListSelectAccept(Widget source, EventArgs e)
        {
            if (anatomyList.hasItemSelected())
            {
                if (showModeGroup.SelectedButton == focusButton)
                {
                    TransparencyController.smoothSetAllAlphas(0.0f, MedicalConfig.TransparencyChangeMultiplier);
                    Anatomy selectedAnatomy = (Anatomy)anatomyList.getItemDataAt(anatomyList.getIndexSelected());
                    foreach (AnatomyCommand command in selectedAnatomy.Commands)
                    {
                        if (command is TransparencyChanger)
                        {
                            ((TransparencyChanger)command).smoothBlend(1.0f, MedicalConfig.TransparencyChangeMultiplier);
                            break;
                        }
                    }
                    SceneViewWindow window = sceneViewController.ActiveWindow;
                    window.setPosition(window.Translation, selectedAnatomy.Center, MedicalConfig.CameraTransitionTime);
                }
                else
                {
                    Anatomy selectedAnatomy = (Anatomy)anatomyList.getItemDataAt(anatomyList.getIndexSelected());
                    foreach (AnatomyCommand command in selectedAnatomy.Commands)
                    {
                        if (command is TransparencyChanger)
                        {
                            if (command.NumericValue == 1.0f)
                            {
                                ((TransparencyChanger)command).smoothBlend(0.7f, MedicalConfig.TransparencyChangeMultiplier);
                            }
                            else if (command.NumericValue == 0.0f)
                            {
                                ((TransparencyChanger)command).smoothBlend(1.0f, MedicalConfig.TransparencyChangeMultiplier);
                            }
                            else
                            {
                                ((TransparencyChanger)command).smoothBlend(0.0f, MedicalConfig.TransparencyChangeMultiplier);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
