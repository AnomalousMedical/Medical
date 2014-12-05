using Engine;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class BookmarksTreeNodeWidget : TreeNodeWidget
    {
        private static readonly int PrimaryWidgetWidth = ScaleHelper.Scaled(26);
        private static readonly int PrimaryWidgetHeight = ScaleHelper.Scaled(16);
        private static readonly int PlusMinusButtonSize = ScaleHelper.Scaled(14);
        private static readonly int PlusMinusButtonY = ScaleHelper.Scaled(1);
        private static readonly int MainButtonX = ScaleHelper.Scaled(17);
        private static readonly int MainButtonWidth = ScaleHelper.Scaled(10);
        private static readonly int MainButtonHeight = ScaleHelper.Scaled(16);

        private Widget primaryWidget;
        private Button plusMinusButton;
        private Button mainButton;

        public BookmarksTreeNodeWidget()
        {

        }

        public override void Dispose()
        {
            destroyWidget();
        }

        public override void createWidget(Widget parent, String caption, String imageResource)
        {
            primaryWidget = parent.createWidgetT("Widget", "PanelEmpty", 0, 0, PrimaryWidgetWidth, PrimaryWidgetHeight, Align.Default, "") as Widget;
            primaryWidget.ForwardMouseWheelToParent = true;

            plusMinusButton = primaryWidget.createWidgetT("Button", "ButtonExpandSkin", 0, PlusMinusButtonY, PlusMinusButtonSize, PlusMinusButtonSize, Align.Left | Align.HCenter, "") as Button;
            plusMinusButton.MouseButtonClick += new MyGUIEvent(plusMinusButton_MouseButtonClick);
            plusMinusButton.Visible = treeNode.HasChildren;
            plusMinusButton.ForwardMouseWheelToParent = true;

            mainButton = primaryWidget.createWidgetT("Button", "TreeTextButton", MainButtonX, 0, MainButtonWidth, MainButtonHeight, Align.Stretch, "") as Button;
            mainButton.Caption = caption;
            mainButton.MouseButtonClick += new MyGUIEvent(mainButton_MouseButtonClick);
            mainButton.MouseButtonDoubleClick += new MyGUIEvent(mainButton_MouseButtonDoubleClick);
            mainButton.MouseButtonReleased += new MyGUIEvent(mainButton_MouseButtonReleased);
            mainButton.MouseButtonPressed += new MyGUIEvent(mainButton_MouseButtonPressed);
            mainButton.Selected = treeNode.Selected;
            mainButton.ForwardMouseWheelToParent = true;
            ImageBox image = mainButton.ImageBox;
            if (image != null)
            {
                image.setItemResource(imageResource);
            }
        }

        public override void destroyWidget()
        {
            if (primaryWidget != null)
            {
                Gui.Instance.destroyWidget(primaryWidget);
                primaryWidget = null;
                plusMinusButton = null;
                mainButton = null;
            }
        }

        public override void setCoord(int left, int top, int width, int height)
        {
            primaryWidget.setCoord(left, top, width, height);
        }

        public override void updateExpandedStatus(bool expanded)
        {
            if (plusMinusButton != null)
            {
                plusMinusButton.Selected = !expanded;
                plusMinusButton.Visible = treeNode.HasChildren;
            }
        }

        public override void updateSelectionStatus(bool selected)
        {
            if (mainButton != null)
            {
                mainButton.Selected = selected;
            }
        }

        protected override void updateText()
        {
            if (mainButton != null)
            {
                mainButton.Caption = treeNode.Text;
            }
        }

        protected override void updateImageResource()
        {
            if (mainButton != null)
            {
                ImageBox image = mainButton.ImageBox;
                if (image != null)
                {
                    image.setItemResource(treeNode.ImageResource);
                }
            }
        }

        void plusMinusButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fireExpandToggled();
        }

        void mainButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fireNodeSelected();
        }

        void mainButton_MouseButtonPressed(Widget source, EventArgs e)
        {
            fireNodeMousePressed((MouseEventArgs)e);
        }

        void mainButton_MouseButtonReleased(Widget source, EventArgs e)
        {
            fireNodeMouseReleased((MouseEventArgs)e);
        }

        void mainButton_MouseButtonDoubleClick(Widget source, EventArgs e)
        {
            fireNodeMouseDoubleClicked();
        }
    }
}
