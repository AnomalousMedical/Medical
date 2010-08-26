using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Standalone;

namespace Medical.GUI
{
    class AppMenu : IDisposable
    {
        private Layout layout;
        private PopupContainer popupContainer;
        private PiperJBOGUI piperGUI;
        private StandaloneController standaloneController;

        public AppMenu(PiperJBOGUI piperGUI, StandaloneController standaloneController)
        {
            this.piperGUI = piperGUI;
            this.standaloneController = standaloneController;

            layout = LayoutManager.Instance.loadLayout("Medical.Controller.GUIController.AppMenu.layout");
            Widget mainWidget = layout.getWidget(0);
            mainWidget.Visible = false;
            popupContainer = new PopupContainer(mainWidget);

            Button changeSceneButton = mainWidget.findWidget("File/ChangeScene") as Button;
            Button openButton = mainWidget.findWidget("File/Open") as Button;
            Button saveButton = mainWidget.findWidget("File/Save") as Button;
            Button saveAsButton = mainWidget.findWidget("File/SaveAs") as Button;
            Button quitButton = mainWidget.findWidget("File/Quit") as Button;

            changeSceneButton.MouseButtonClick += new MyGUIEvent(changeSceneButton_MouseButtonClick);
            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);
            saveButton.MouseButtonClick += new MyGUIEvent(saveButton_MouseButtonClick);
            saveAsButton.MouseButtonClick += new MyGUIEvent(saveAsButton_MouseButtonClick);
            quitButton.MouseButtonClick += new MyGUIEvent(quitButton_MouseButtonClick);
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void show(int x, int y)
        {
            popupContainer.show(x, y);
        }

        void changeSceneButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.showChooseSceneDialog();
            popupContainer.hide();
        }

        void saveButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.save();
            popupContainer.hide();
        }

        void saveAsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.saveAs();
            popupContainer.hide();
        }

        void openButton_MouseButtonClick(Widget source, EventArgs e)
        {
            piperGUI.open();
            popupContainer.hide();
        }

        void quitButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.shutdown();
            popupContainer.hide();
        }
    }
}
