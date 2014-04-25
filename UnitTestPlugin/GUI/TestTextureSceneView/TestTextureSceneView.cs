using Engine;
using Medical;
using Medical.Controller;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestPlugin.GUI
{
    class TestTextureSceneView : MDIDialog
    {
        private SingleSelectButtonGrid buttonGrid;
        private ScrollView scrollView;

        private EditBox secondsToSleepEdit;
        private EditBox numToUpdateEdit;

        private LiveThumbnailController liveThumbnailController;
        private int count = 0;

        public TestTextureSceneView(SceneViewController sceneViewController)
            : base("UnitTestPlugin.GUI.TestTextureSceneView.TestTextureSceneView.layout")
        {
            liveThumbnailController = new LiveThumbnailController("TestRTT_", new IntSize2(200, 200), sceneViewController);
            liveThumbnailController.ThumbnailDestroyed += liveThumbnailController_ThumbnailDestroyed;

            Button addButton = (Button)window.findWidget("AddButton");
            addButton.MouseButtonClick += addButton_MouseButtonClick;

            Button removeButton = (Button)window.findWidget("RemoveButton");
            removeButton.MouseButtonClick +=removeButton_MouseButtonClick;

            scrollView = (ScrollView)window.findWidget("ScrollView");
            buttonGrid = new SingleSelectButtonGrid(scrollView);
            scrollView.CanvasPositionChanged += scrollView_CanvasPositionChanged;

            window.WindowChangedCoord += window_WindowChangedCoord;

            numToUpdateEdit = (EditBox)window.findWidget("NumToUpdate");
            numToUpdateEdit.Caption = liveThumbnailController.NumImagesToUpdate.ToString();
            secondsToSleepEdit = (EditBox)window.findWidget("SecondsToSleep");
            secondsToSleepEdit.Caption = liveThumbnailController.SecondsToSleep.ToString();
            Button applyButton = (Button)window.findWidget("ApplyButton");
            applyButton.MouseButtonClick += applyButton_MouseButtonClick;
        }

        public override void Dispose()
        {
            liveThumbnailController.Dispose();
            buttonGrid.Dispose();
            base.Dispose();
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            int numImagesToUpdate;
            if (int.TryParse(numToUpdateEdit.Caption, out numImagesToUpdate))
            {
                liveThumbnailController.NumImagesToUpdate = numImagesToUpdate;
            }
            double secondsToSleep;
            if (double.TryParse(secondsToSleepEdit.Caption, out secondsToSleep))
            {
                liveThumbnailController.SecondsToSleep = secondsToSleep;
            }
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ButtonGridItem item = buttonGrid.addItem("Main", count++.ToString());
            LiveThumbnailHost host = new ButtonGridItemLiveThumbnailHost(item);
            item.UserObject = host;
            liveThumbnailController.addThumbnailHost(host);
            buttonGrid.resizeAndLayout(window.ClientWidget.Width);
            liveThumbnailController.determineVisibleHosts(VisibleArea);
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            var selectedItem = buttonGrid.SelectedItem;
            if (selectedItem != null)
            {
                liveThumbnailController.removeThumbnailHost((LiveThumbnailHost)selectedItem.UserObject);
                buttonGrid.removeItem(selectedItem);
                liveThumbnailController.determineVisibleHosts(VisibleArea);
            }
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            buttonGrid.resizeAndLayout(scrollView.ViewCoord.width);
            liveThumbnailController.determineVisibleHosts(VisibleArea);
        }

        void liveThumbnailController_ThumbnailDestroyed(LiveThumbnailController thumbController, PooledSceneView sceneView)
        {
            RenderManager.Instance.destroyTexture(sceneView.SceneView.Name);
        }

        void scrollView_CanvasPositionChanged(Widget source, EventArgs e)
        {
            liveThumbnailController.determineVisibleHosts(VisibleArea);
        }

        private IntCoord VisibleArea
        {
            get
            {
                IntCoord viewArea = scrollView.ViewCoord;
                var canvasPos = scrollView.CanvasPosition;
                viewArea.left = (int)canvasPos.x;
                viewArea.top = (int)canvasPos.y;

                return viewArea;
            }
        }

        class ButtonGridItemLiveThumbnailHost : LiveThumbnailHost
        {
            private ButtonGridItem item;

            public ButtonGridItemLiveThumbnailHost(ButtonGridItem item)
            {
                this.item = item;
            }

            public override IntCoord Coord
            {
                get
                {
                    return item.Coord;
                }
            }

            public override void setTextureInfo(string name, IntCoord coord)
            {
                item.ImageBox.setImageTexture(name);
                item.ImageBox.setImageCoord(coord);
            }
        }
    }
}
