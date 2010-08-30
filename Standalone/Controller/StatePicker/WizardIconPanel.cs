using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Logging;

namespace Medical.GUI
{
    class WizardIconPanel : IDisposable
    {
        public delegate void ModeChangedDelegate(int modeIndex);

        public event ModeChangedDelegate ModeChanged;

        class WizardButtonContainer
        {
            private Button button;

            public WizardButtonContainer(Button button)
            {
                this.button = button;
                button.UserObject = this;
                Layout = new MyGUILayoutContainer(button);
                ModeIndex = 0;
            }

            public LayoutContainer Layout { get; set; }

            public int ModeIndex { get; set; }

            public bool Visible
            {
                get
                {
                    return button.Visible;
                }
                set
                {
                    button.Visible = value;
                }
            }

            public bool StateCheck
            {
                get
                {
                    return button.StateCheck;
                }
                set
                {
                    button.StateCheck = value;
                }
            }
        }

        private Layout layout;
        private Widget mainWidget;
        private MyGUILayoutContainer layoutContainer;
        private FlowLayoutContainer flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Horizontal, 10.0f, new Vector2(0.0f, 10.0f));
        private Dictionary<StateWizardPanel, WizardButtonContainer> panels = new Dictionary<StateWizardPanel, WizardButtonContainer>();
        private WizardButtonContainer selectedButton;
        private List<WizardButtonContainer> currentButtons = new List<WizardButtonContainer>();

        public WizardIconPanel()
        {
            layout = LayoutManager.Instance.loadLayout("Medical.Controller.StatePicker.WizardIconPanel.layout");
            mainWidget = layout.getWidget(0);
            mainWidget.Visible = false;
            layoutContainer = new MyGUILayoutContainer(mainWidget);
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public MyGUILayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }

        public void addPanel(StateWizardPanel panel, int modeIndex)
        {
            WizardButtonContainer container;
            if (!panels.TryGetValue(panel, out container))
            {
                Button button = mainWidget.createWidgetT("Button", "RibbonButton", 0, 0, 78, 64, Align.Default, "") as Button;
                String caption = panel.TextLine1;
                if (panel.TextLine2 != null)
                {
                    caption += '\n' + panel.TextLine2;
                }
                button.Caption = caption;
                button.MouseButtonClick += iconClicked;
                int captionWidth = (int)button.getTextSize().Width;
                button.setSize(captionWidth + 45, button.Height);
                button.StaticImage.setItemResource(panel.ImageKey);
                container = new WizardButtonContainer(button);
            }
            container.ModeIndex = modeIndex;
            container.Visible = true;
            flowLayout.addChild(container.Layout);
            currentButtons.Add(container);
        }

        public void clearPanels()
        {
            flowLayout.clearChildren();
            foreach (WizardButtonContainer wizardButton in currentButtons)
            {
                wizardButton.Visible = false;
                wizardButton.StateCheck = false;
            }
            currentButtons.Clear();
            selectedButton = null;
        }

        public bool SuppressLayout
        {
            get
            {
                return flowLayout.SuppressLayout;
            }
            set
            {
                flowLayout.SuppressLayout = value;
            }
        }

        public void invalidate()
        {
            flowLayout.invalidate();
        }

        public void indexChanged(int index)
        {
            if (selectedButton != null)
            {
                selectedButton.StateCheck = false;
            }
            selectedButton = currentButtons[index];
            selectedButton.StateCheck = true;
        }

        void iconClicked(Widget source, EventArgs e)
        {
            WizardButtonContainer buttonContainer = source.UserObject as WizardButtonContainer;
            if (buttonContainer != null)
            {
                if (ModeChanged != null)
                {
                    ModeChanged.Invoke(buttonContainer.ModeIndex);
                }
            }
            else
            {
                Log.Error("Somehow got a bad WizardButtonContainer. This error should never occur.");
            }
        }
    }
}
