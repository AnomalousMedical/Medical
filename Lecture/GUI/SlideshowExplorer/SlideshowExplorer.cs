using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using Engine.Platform;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;
using Medical.GUI;
using Medical;
using System.Drawing;
using System.Drawing.Imaging;

namespace Lecture.GUI
{
    public class SlideshowExplorer : MDIDialog
    {
        private String windowTitle;
        private const String windowTitleFormat = "{0} - {1}";

        public Action<AnomalousMvcContext> RunContext;

        //File Menu
        MenuBar menuBar;
        MenuItem newProject;
        MenuItem openProject;
        MenuItem closeProject;
        MenuItem saveAll;
        MenuItem cleanup;

        private Slideshow slideshow;
        private SlideshowEditController slideEditController;
        private SlideImageManager slideImageManager;

        //Buttons
        Button addButton;
        Button removeButton;
        Button playButton;
        Button captureButton;

        private ButtonGrid slideGrid;
        private ScrollView scroll;

        public SlideshowExplorer(SlideshowEditController slideEditController)
            : base("Lecture.GUI.SlideshowExplorer.SlideshowExplorer.layout")
        {
            this.slideEditController = slideEditController;
            slideImageManager = slideEditController.SlideImageManager;

            addButton = (Button)window.findWidget("Add");
            addButton.MouseButtonClick += addButton_MouseButtonClick;
            removeButton = (Button)window.findWidget("Remove");
            removeButton.MouseButtonClick += removeButton_MouseButtonClick;
            playButton = (Button)window.findWidget("Play");
            playButton.MouseButtonClick += playButton_MouseButtonClick;
            captureButton = (Button)window.findWidget("Capture");
            captureButton.MouseButtonClick += captureButton_MouseButtonClick;

            slideEditController.SlideshowLoaded += slideEditController_SlideshowLoaded;
            slideEditController.SlideshowClosed += slideEditController_SlideshowClosed;
            slideEditController.SlideAdded += slideEditController_SlideAdded;
            slideEditController.SlideRemoved += slideEditController_SlideRemoved;
            slideEditController.SlideSelected += slideEditController_SlideSelected;

            slideImageManager.ThumbUpdating += slideImageManager_ThumbUpdating;
            slideImageManager.ThumbUpdated += slideImageManager_ThumbUpdated;

            windowTitle = window.Caption;
            menuBar = window.findWidget("MenuBar") as MenuBar;

            scroll = (ScrollView)window.findWidget("Scroll");
            slideGrid = new ButtonGrid(scroll, new ButtonGridListLayout());
            slideGrid.SelectedValueChanged += slideGrid_SelectedValueChanged;

            //File Menu
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuControl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            newProject = fileMenu.addItem("New Project");
            openProject = fileMenu.addItem("Open Project");
            closeProject = fileMenu.addItem("Close Project");
            saveAll = fileMenu.addItem("Save");
            cleanup = fileMenu.addItem("Cleanup");

            this.Resized += new EventHandler(ProjectExplorer_Resized);
        }

        public override void Dispose()
        {
            slideGrid.Dispose();
            slideImageManager.Dispose();
            base.Dispose();
        }

        void createNewProjectClicked(Widget source, EventArgs e)
        {
            if (slideEditController.ResourceProvider == null || slideEditController.ResourceProvider.ResourceCache.Count == 0)
            {
                showNewProjectDialog();
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before creating a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        slideEditController.save();
                    }
                    showNewProjectDialog();
                });
            }
        }

        void showNewProjectDialog()
        {
            slideEditController.stopPlayingTimelines();

            Browser browse = new Browser("Slideshows", "Create Slideshow");
            BrowserNode defaultNode = new BrowserNode("Slideshow", new SlideshowProjectTemplate());
            browse.addNode("", null, defaultNode);
            browse.DefaultSelection = defaultNode;

            NewProjectDialog.ShowDialog(browse, (template, fullProjectName) =>
            {
                if (Directory.Exists(fullProjectName))
                {
                    MessageBox.show(String.Format("The project {0} already exists. Would you like to delete it and create a new one?", fullProjectName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, result =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            slideEditController.createNewProject(fullProjectName, true, template);
                        }
                    });
                }
                else
                {
                    slideEditController.createNewProject(fullProjectName, false, template);
                }
            });
        }

        void openProjectClicked(Widget source, EventArgs e)
        {
            if (slideEditController.ResourceProvider == null || slideEditController.ResourceProvider.ResourceCache.Count == 0)
            {
                showOpenProjectDialog();
            }
            else
            {
                MessageBox.show("You have open files, would you like to save them before opening a new project?", "Save", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Ok)
                    {
                        slideEditController.save();
                    }
                    showOpenProjectDialog();
                });
            }
        }

        void showOpenProjectDialog()
        {
            slideEditController.stopPlayingTimelines();
            FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a project.", "", "", "", false);
            fileDialog.showModal((result, paths) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    String path = paths.First();
                    slideEditController.openProject(Path.GetDirectoryName(path), path);
                }
            });
        }

        void ProjectExplorer_Resized(object sender, EventArgs e)
        {
            slideGrid.resizeAndLayout(scroll.ClientCoord.width);
        }

        void fileMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs menuEventArgs = (MenuCtrlAcceptEventArgs)e;
            if (menuEventArgs.Item == newProject)
            {
                createNewProjectClicked(source, e);
            }
            else if (menuEventArgs.Item == openProject)
            {
                openProjectClicked(source, e);
            }
            else if (menuEventArgs.Item == saveAll)
            {
                slideEditController.save();
            }
            else if (menuEventArgs.Item == closeProject)
            {
                slideEditController.closeProject();
            }
            else if (menuEventArgs.Item == cleanup)
            {
                MessageBox.show("Cleaning up your slideshow will remove unneeded files, however, your project will be saved and all of your undo history will be lost.\nAre you sure you wish to continue?", "Cleanup", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, (result) =>
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        slideEditController.save();
                        slideEditController.cleanup();
                    }
                });
            }
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (slideshow != null)
            {
                AddItemDialog.AddItem(slideEditController.ItemTemplates, slideEditController.createItem);
            }
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (slideshow != null)
            {
                ButtonGridItem selectedItem = slideGrid.SelectedItem;
                if (selectedItem != null)
                {
                    slideEditController.removeSlide((Slide)selectedItem.UserObject);
                }
            }
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (slideshow != null)
            {
                int startIndex = 0;
                ButtonGridItem selectedItem = slideGrid.SelectedItem;
                if (selectedItem != null)
                {
                    //LAME! No other real way to do it for right now.
                    startIndex = slideshow.indexOf((Slide)selectedItem.UserObject);
                    if (startIndex < 0)
                    {
                        startIndex = 0;
                    }
                }
                AnomalousMvcContext context = slideshow.createContext(slideEditController.ResourceProvider, startIndex);
                context.RuntimeName = "Editor.PreviewMvcContext";
                context.setResourceProvider(slideEditController.ResourceProvider);
                if (RunContext != null)
                {
                    RunContext.Invoke(context);
                }
            }
        }

        void slideEditController_SlideshowLoaded(Slideshow show)
        {
            slideGrid.SuppressLayout = true;
            slideGrid.clear();
            slideImageManager.clear(); //We want to be sure to clear out the slidegrid first.
            foreach (Slide slide in show.Slides)
            {
                addSlideToGrid(slide, -1);
            }
            slideGrid.SuppressLayout = false;
            slideGrid.resizeAndLayout(scroll.ClientCoord.width);
            this.slideshow = show;
            window.Caption = String.Format(windowTitleFormat, windowTitle, slideEditController.ResourceProvider.BackingLocation);
            if (!Visible)
            {
                Visible = true;
            }
        }

        void slideEditController_SlideAdded(Slide slide, int index)
        {
            addSlideToGrid(slide, index);
        }

        void slideEditController_SlideRemoved(Slide slide)
        {
            removeSlideFromGrid(slide);
        }

        void slideGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonGridItem item = slideGrid.SelectedItem;
            if (item != null)
            {
                slideEditController.editSlide((Slide)item.UserObject);
            }
        }

        void slideEditController_SlideshowClosed()
        {
            slideGrid.clear();
            slideshow = null;
            window.Caption = windowTitle;
        }

        void addSlideToGrid(Slide slide, int index)
        {
            String thumbName = slideImageManager.loadThumbnail(slide);

            ButtonGridItem item;
            if (index == -1)
            {
                item = slideGrid.addItem("", "Slide " + (slideGrid.Count + 1));
            }
            else
            {
                slideGrid.SuppressLayout = true;
                item = slideGrid.insertItem(index, "", "Slide " + (index + 1));
                foreach (ButtonGridItem laterItem in slideGrid.Items.Skip(++index))
                {
                    laterItem.Caption = "Slide " + ++index;
                }
                slideGrid.SuppressLayout = false;
                slideGrid.layout();
            }
            item.UserObject = slide;
            item.setImage(thumbName);
        }

        void removeSlideFromGrid(Slide slide)
        {
            ButtonGridItem item = slideGrid.findItemByUserObject(slide);
            if (item != null)
            {
                slideGrid.SuppressLayout = true;
                slideGrid.removeItem(item);
                int index = 1;
                foreach (ButtonGridItem button in slideGrid.Items)
                {
                    button.Caption = "Slide " + index++;
                }
                slideGrid.SuppressLayout = false;
                slideGrid.layout();
            }
        }

        void slideEditController_SlideSelected(Slide slide)
        {
            slideGrid.SelectedItem = slideGrid.findItemByUserObject(slide);
        }

        void slideImageManager_ThumbUpdated(Slide slide, String key)
        {
            ButtonGridItem item = slideGrid.findItemByUserObject(slide);
            if (item != null)
            {
                item.setImage(key);
            }
        }

        void slideImageManager_ThumbUpdating(Slide slide)
        {
            ButtonGridItem item = slideGrid.findItemByUserObject(slide);
            if (item != null)
            {
                item.setImage(null); //Null the image or else the program gets crashy
            }
        }

        void captureButton_MouseButtonClick(Widget source, EventArgs e)
        {
            slideEditController.capture();
        }
    }
}
