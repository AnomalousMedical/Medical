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

        private Dictionary<MenuItem, Action> menuActions = new Dictionary<MenuItem, Action>();

        //File Menu
        MenuBar menuBar;

        private Slideshow slideshow;
        private SlideshowEditController slideEditController;
        private SlideImageManager slideImageManager;

        private MultiSelectButtonGrid slideGrid;
        private ScrollView scroll;

        //Drag Drop
        private ImageBox dragIconPreview;
        private Widget dropLocationPreview;
        private IntVector2 dragMouseStartPosition;
        private bool firstDrag;
        private int dragHoverIndex;
        private ButtonGridItem dragHoverItem;
        private ButtonGridItem dragItem;
        private bool dropAfter = false;
        bool closeProjectOnClose = true;

        public SlideshowExplorer(SlideshowEditController slideEditController)
            : base("Lecture.GUI.SlideshowExplorer.SlideshowExplorer.layout")
        {
            this.slideEditController = slideEditController;
            slideImageManager = slideEditController.SlideImageManager;

            slideEditController.SlideshowLoaded += slideEditController_SlideshowLoaded;
            slideEditController.SlideshowClosed += slideEditController_SlideshowClosed;
            slideEditController.SlideAdded += slideEditController_SlideAdded;
            slideEditController.SlideRemoved += slideEditController_SlideRemoved;
            slideEditController.SlideSelected += slideEditController_SlideSelected;
            slideEditController.RequestRemoveSelected += removeSelected;
            slideEditController.SlideshowContextStarting += slideEditController_SlideshowContextStarting;
            slideEditController.SlideshowContextBlurred += slideEditController_SlideshowContextBlurred;

            slideImageManager.ThumbUpdating += slideImageManager_ThumbUpdating;
            slideImageManager.ThumbUpdated += slideImageManager_ThumbUpdated;

            windowTitle = window.Caption;
            menuBar = window.findWidget("MenuBar") as MenuBar;
            menuBar.ItemAccept += menuItemAccept;

            scroll = (ScrollView)window.findWidget("Scroll");
            slideGrid = new MultiSelectButtonGrid(scroll, new ButtonGridListLayout());
            slideGrid.SelectedValueChanged += slideGrid_SelectedValueChanged;

            //File Menu
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuControl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            menuActions.Add(fileMenu.addItem("New"), createNewProject);
            menuActions.Add(fileMenu.addItem("Open"), openProject);
            menuActions.Add(fileMenu.addItem("Close"), slideEditController.closeProject);
            menuActions.Add(fileMenu.addItem("Save"), slideEditController.save);
            fileMenu.addItem("Sep", MenuItemType.Separator);
            menuActions.Add(fileMenu.addItem("Present"), play);
            menuActions.Add(fileMenu.addItem("Present from Beginning"), playFromBeginning);
            fileMenu.addItem("Sep2", MenuItemType.Separator);
            menuActions.Add(fileMenu.addItem("Cleanup"), cleanup);

            MenuItem editMenuItem = menuBar.addItem("Edit", MenuItemType.Popup);
            MenuControl editMenu = menuBar.createItemPopupMenuChild(editMenuItem);
            menuActions.Add(editMenu.addItem("Undo"), slideEditController.UndoBuffer.undo);
            menuActions.Add(editMenu.addItem("Redo"), slideEditController.UndoBuffer.execute);

            MenuItem slideMenuItem = menuBar.addItem("Slide", MenuItemType.Popup);
            MenuControl slideMenu = menuBar.createItemPopupMenuChild(slideMenuItem);
            menuActions.Add(slideMenu.addItem("Add"), addSlide);
            menuActions.Add(slideMenu.addItem("Remove"), removeSelected);
            menuActions.Add(slideMenu.addItem("Capture"), slideEditController.capture);

            this.Resized += new EventHandler(ProjectExplorer_Resized);

            dragIconPreview = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, 32, 32, Align.Default, "Info", "SlideDragAndDropPreview");
            dragIconPreview.Visible = false;
            dropLocationPreview = Gui.Instance.createWidgetT("Widget", "2dBorderPanelSkin", 0, 0, 100, 10, Align.Default, "Info", "SlideDropPreview");
            dropLocationPreview.Visible = false;

            this.Closed += SlideshowExplorer_Closed;
            this.Shown += SlideshowExplorer_Shown;
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(dragIconPreview);
            slideGrid.Dispose();
            slideImageManager.Dispose();
            base.Dispose();
        }

        void createNewProject()
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

            Browser browse = new Browser("Smart Lectures", "Create Smart Lecture");
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

        void openProject()
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

        void cleanup()
        {
            if (slideshow != null)
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

        void menuItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs menuEventArgs = (MenuCtrlAcceptEventArgs)e;
            menuActions[menuEventArgs.Item].Invoke();
        }

        void addSlide()
        {
            if (slideshow != null)
            {
                slideEditController.createSlide();
            }
        }

        private void removeSelected()
        {
            if (slideshow != null && slideGrid.SelectedItem != null)
            {
                slideEditController.removeSlides(from item in slideGrid.SelectedItems select (Slide)item.UserObject, (Slide)slideGrid.SelectedItem.UserObject);
            }
        }

        void play()
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
                slideEditController.runSlideshow(startIndex);
            }
        }

        void playFromBeginning()
        {
            if (slideshow != null)
            {
                slideEditController.runSlideshow(0);
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
            slideImageManager.loadThumbnail(slide, (loadedThumbSlide, id) =>
                {
                    //Ensure that the item still exists
                    ButtonGridItem imageUpdateItem = slideGrid.findItemByUserObject(loadedThumbSlide);
                    if (imageUpdateItem != null)
                    {
                        imageUpdateItem.setImage(id);
                    }
                });
            item.MouseButtonPressed += item_MouseButtonPressed;
            item.MouseButtonReleased += item_MouseButtonReleased;
            item.MouseDrag += item_MouseDrag;
        }

        void removeSlideFromGrid(Slide slide)
        {
            ButtonGridItem item = slideGrid.findItemByUserObject(slide);
            if (item != null)
            {
                item.MouseButtonPressed -= item_MouseButtonPressed;
                item.MouseButtonReleased -= item_MouseButtonReleased;
                item.MouseDrag -= item_MouseDrag;

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

        void slideEditController_SlideSelected(Slide primary, IEnumerable<Slide> secondary)
        {
            slideGrid.setSelection(slideGrid.findItemByUserObject(primary), secondarySelectedButtonGridItems(secondary));
        }

        private IEnumerable<ButtonGridItem> secondarySelectedButtonGridItems(IEnumerable<Slide> secondary)
        {
            foreach (Slide slide in secondary)
            {
                yield return slideGrid.findItemByUserObject(slide);
            }
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

        void item_MouseDrag(ButtonGridItem source, MouseEventArgs arg)
        {
            if (firstDrag)
            {
                dragHoverIndex = slideshow.indexOf((Slide)source.UserObject);
                dragHoverItem = source;
                dragItem = source;
                firstDrag = false;
            }
            dragIconPreview.setPosition(arg.Position.x - (dragIconPreview.Width / 2), arg.Position.y - (int)(dragIconPreview.Height * .75f));
            if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - arg.Position.x) > 5 || Math.Abs(dragMouseStartPosition.y - arg.Position.y) > 5))
            {
                dropLocationPreview.Visible = true;
                dropLocationPreview.setSize(dragHoverItem.Width, dropLocationPreview.Height);
                LayerManager.Instance.upLayerItem(dropLocationPreview);
                dragIconPreview.Visible = true;
                dragIconPreview.setItemResource(CommonResources.NoIcon);
                LayerManager.Instance.upLayerItem(dragIconPreview);
            }

            IntVector2 point = arg.Position;
            if (point.y > dragHoverItem.AbsoluteTop && point.y < dragHoverItem.AbsoluteTop + dragHoverItem.Height)
            {
                if (point.y < dragHoverItem.AbsoluteTop + (dragHoverItem.Height / 2))
                {
                    dropLocationPreview.setPosition(dragHoverItem.AbsoluteLeft, dragHoverItem.AbsoluteTop);
                    dropAfter = false;
                }
                else
                {
                    dropLocationPreview.setPosition(dragHoverItem.AbsoluteLeft, dragHoverItem.AbsoluteTop + dragHoverItem.Height);
                    dropAfter = true;
                }
            }
            else
            {
                if (point.y < dragHoverItem.AbsoluteTop + (dragHoverItem.Height / 2))
                {
                    //Look behind
                    int newIndex = dragHoverIndex - 1;
                    bool keepSearching = true;
                    ButtonGridItem checkItem = null;
                    while (newIndex >= 0 && keepSearching)
                    {
                        checkItem = slideGrid.getItem(newIndex);
                        if (checkItem.AbsoluteTop < point.y)
                        {
                            keepSearching = false;
                        }
                        else
                        {
                            --newIndex;
                        }
                    }
                    dragHoverItem = checkItem;
                    dragHoverIndex = newIndex;
                    if (dragHoverIndex < 0)
                    {
                        dragHoverIndex = 0;
                        dragHoverItem = slideGrid.getItem(dragHoverIndex);
                    }
                }
                else
                {
                    //Look ahead
                    int newIndex = dragHoverIndex + 1;
                    bool keepSearching = true;
                    ButtonGridItem checkItem = null;
                    while (newIndex < slideshow.Count && keepSearching)
                    {
                        checkItem = slideGrid.getItem(newIndex);
                        if (checkItem.AbsoluteTop > point.y)
                        {
                            keepSearching = false;
                        }
                        else
                        {
                            ++newIndex;
                        }
                    }
                    dragHoverItem = checkItem;
                    dragHoverIndex = newIndex;
                    if (dragHoverIndex > slideshow.Count - 1)
                    {
                        dragHoverIndex = slideshow.Count - 1;
                        dragHoverItem = slideGrid.getItem(dragHoverIndex);
                    }
                }
            }
        }

        void item_MouseButtonReleased(ButtonGridItem source, MouseEventArgs arg)
        {
            if (dragIconPreview.Visible)
            {
                dragIconPreview.Visible = false;
                dropLocationPreview.Visible = false;
                if (dropAfter)
                {
                    ++dragHoverIndex;
                }
                slideEditController.moveSlides(SelectedSlides, dragHoverIndex);
                dragItem = null;
            }
        }

        void item_MouseButtonPressed(ButtonGridItem source, MouseEventArgs arg)
        {
            dragMouseStartPosition = arg.Position;
            firstDrag = true;
            dragHoverIndex = -1;
            dragHoverItem = null;
        }

        private IEnumerable<Slide> SelectedSlides
        {
            get
            {
                if (dragItem != null && !dragItem.StateCheck)
                {
                    yield return (Slide)dragItem.UserObject;
                }
                foreach (ButtonGridItem item in slideGrid.SelectedItems)
                {
                    yield return (Slide)item.UserObject;
                }
            }
        }

        void SlideshowExplorer_Closed(object sender, EventArgs e)
        {
            if (closeProjectOnClose)
            {
                slideEditController.closeEditors();
            }
        }

        /// <summary>
        /// This is kind of dumb, basically the SlideshowExplorer does not know if it is 
        /// being opened from a showMainInterface or because the user clicked it. So we set
        /// safeToEditSlide to false in the blurred function for playing slideshows, this blocks it
        /// from trying to open its own editor when the current context is shutting down.
        /// We set this back to true every time the slide opens. If you have trouble with a context
        /// loading while another one is open check this little hack first.
        /// </summary>
        bool safeToEditSlide = true;

        void SlideshowExplorer_Shown(object sender, EventArgs e)
        {
            if (closeProjectOnClose && safeToEditSlide && slideGrid.SelectedItem != null)
            {
                slideEditController.editSlide((Slide)slideGrid.SelectedItem.UserObject);
            }
            safeToEditSlide = true;
        }

        void slideEditController_SlideshowContextBlurred(AnomalousMvcContext obj)
        {
            safeToEditSlide = false;
            closeProjectOnClose = true;
        }

        void slideEditController_SlideshowContextStarting(AnomalousMvcContext obj)
        {
            closeProjectOnClose = false;
        }
    }
}
