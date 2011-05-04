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

        private AnatomyContextWindowManager anatomyWindowManager;
        private AnatomyTagManager anatomyTagManager = new AnatomyTagManager();
        private AnatomySearchList anatomySearchList = new AnatomySearchList();
        private List<AnatomyTagGroup> topLevelGroups = new List<AnatomyTagGroup>();

        private SceneViewController sceneViewController;

        private Button groupButton;
        private Button individualButton;
        private ButtonGroup pickingModeGroup;

        public AnatomyFinder(SceneViewController sceneViewController)
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            this.sceneViewController = sceneViewController;
            anatomyWindowManager = new AnatomyContextWindowManager(sceneViewController);

            anatomyList = (MultiList)window.findWidget("AnatomyList");
            anatomyList.addColumn("Anatomy", anatomyList.Width);
            anatomyList.ListChangePosition += new MyGUIEvent(anatomyList_ListChangePosition);
            anatomyList.ListSelectAccept += new MyGUIEvent(anatomyList_ListSelectAccept);

            searchBox = (Edit)window.findWidget("SearchBox");
            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);

            pickAnatomy.FirstFrameUpEvent += new MessageEventCallback(pickAnatomy_FirstFrameUpEvent);

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

        private AnatomyContextWindow changeSelectedAnatomy()
        {
            if (anatomyList.hasItemSelected())
            {
                return anatomyWindowManager.showWindow((Anatomy)anatomyList.getItemDataAt(anatomyList.getIndexSelected()));
            }
            else
            {
                anatomyWindowManager.closeUnpinnedWindow();
                return null;
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
                absMouse.y = (absMouse.y - windowLoc.y) / windowSize.Height;
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
                    uint selectedIndex = 0;
                    if (pickingModeGroup.SelectedButton == groupButton && matches[0].AllowGroupSelection)
                    {
                        AnatomyTag mainTag = matches[0].Tags.First();
                        if (mainTag != null)
                        {
                            anatomyList.findSubItemWith(0, mainTag.Tag, out selectedIndex);
                        }
                    }
                    anatomyList.setIndexSelected(selectedIndex);
                }
                AnatomyContextWindow activeAnatomyWindow = changeSelectedAnatomy();
                if (activeAnatomyWindow != null)
                {
                    activeAnatomyWindow.Position = new Vector2(eventManager.Mouse.getAbsMouse().x, eventManager.Mouse.getAbsMouse().y);
                    activeAnatomyWindow.ensureVisible();
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

        void unhideAll_MouseButtonClick(Widget source, EventArgs e)
        {
            TransparencyController.smoothSetAllAlphas(1.0f, MedicalConfig.TransparencyChangeMultiplier);
        }

        void anatomyList_ListSelectAccept(Widget source, EventArgs e)
        {
            if (anatomyList.hasItemSelected())
            {
                Anatomy selectedAnatomy = (Anatomy)anatomyList.getItemDataAt(anatomyList.getIndexSelected());
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
    }
}
