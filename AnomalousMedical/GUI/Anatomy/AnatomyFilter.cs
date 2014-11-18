using Engine;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class AnatomyFilter : PopupContainer
    {
        private FlowLayoutContainer flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, ScaleHelper.Scaled(2), new IntVector2());
        private ScrollView scrollView;

        private List<Widget> childWidgets = new List<Widget>();
        private List<AnatomyFacet> activeFacets = new List<AnatomyFacet>();

        public AnatomyFilter()
            : base("Medical.GUI.Anatomy.AnatomyFilter.layout")
        {
            scrollView = widget as ScrollView;
        }

        public override void Dispose()
        {
            clear();
            base.Dispose();
        }

        public void refreshCategories(AnatomyController anatomyController)
        {
            clear();

            NaturalSort<String> sort = new NaturalSort<string>();

            createGroup("Systems", "System", anatomyController.Systems.Select(i => i.AnatomicalName).OrderBy(i => i, sort));
            createGroup("Regions", "Region", anatomyController.Regions.Select(i => i.AnatomicalName).OrderBy(i => i, sort));
            createGroup("Classificatons", "Classification", anatomyController.Classifications.Select(i => i.AnatomicalName).OrderBy(i => i, sort));

            var size = flowLayout.DesiredSize;
            size.Width = scrollView.Width;
            scrollView.CanvasSize = size;
            flowLayout.WorkingSize = size;
            flowLayout.layout();
        }

        public void clear()
        {
            foreach (var widget in childWidgets)
            {
                Gui.Instance.destroyWidget(widget);
            }
            childWidgets.Clear();
            flowLayout.clearChildren();
        }

        public IEnumerable<AnatomyFacet> ActiveFacets
        {
            get
            {
                return activeFacets;
            }
        }

        private void createGroup(String caption, String facetName, IEnumerable<String> items)
        {
            List<CheckButton> groupCheckButtons = new List<CheckButton>();
            List<String> activeFacetValues = new List<string>();
            AnatomyFacet facet = new AnatomyFacet(facetName, activeFacetValues);

            TextBox label = scrollView.createWidgetT("TextBox", "TextBox", 0, 0, widget.Width, ScaleHelper.Scaled(20), Align.Left | Align.Top, "") as TextBox;
            label.TextAlign = Align.Left | Align.VCenter;
            label.Caption = caption;
            label.ForwardMouseWheelToParent = true;
            flowLayout.addChild(new MyGUILayoutContainer(label));
            childWidgets.Add(label);

            Button button = scrollView.createWidgetT("Button", "CheckBox", 0, 0, widget.Width, ScaleHelper.Scaled(20), Align.Left | Align.Top, "") as Button;
            button.Caption = "Include All";
            button.ForwardMouseWheelToParent = true;
            CheckButton allCheckButton = new CheckButton(button);
            allCheckButton.Checked = true;
            allCheckButton.CheckedChanged += (sender, e) =>
                {
                    if (allCheckButton.Checked)
                    {
                        foreach (var groupButton in groupCheckButtons)
                        {
                            groupButton.Checked = false;
                        }
                        activeFacets.Remove(facet);
                    }
                };
            flowLayout.addChild(new MyGUILayoutContainer(button));
            childWidgets.Add(button);

            foreach(String item in items)
            {
                button = scrollView.createWidgetT("Button", "CheckBox", 0, 0, widget.Width, ScaleHelper.Scaled(20), Align.Left | Align.Top, "") as Button;
                button.Caption = item;
                button.ForwardMouseWheelToParent = true;
                CheckButton checkButton = new CheckButton(button);
                checkButton.CheckedChanged += (sender, e) =>
                    {
                        if(checkButton.Checked)
                        {
                            allCheckButton.Checked = false;
                            if(!activeFacets.Contains(facet))
                            {
                                activeFacets.Add(facet);
                            }
                            activeFacetValues.Add(item);
                        }
                        else
                        {
                            activeFacetValues.Remove(item);
                            if(activeFacetValues.Count == 0)
                            {
                                allCheckButton.Checked = true;
                                //The allCheckButton.CheckedChanged is triggered here
                            }
                        }
                    };
                flowLayout.addChild(new MyGUILayoutContainer(button));
                childWidgets.Add(button);
                groupCheckButtons.Add(checkButton);
            }
        }
    }
}
