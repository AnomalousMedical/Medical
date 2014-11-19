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
        private StretchLayoutContainer flowLayout = new StretchLayoutContainer(StretchLayoutContainer.LayoutType.Vertical, ScaleHelper.Scaled(2), new IntVector2());
        private ScrollView scrollView;

        private List<Widget> childWidgets = new List<Widget>();
        private List<AnatomyFacet> activeFacets = new List<AnatomyFacet>();
        private AnatomyController anatomyController;
        private ButtonGroup<TopLevelMode> topLevelButtons = new ButtonGroup<TopLevelMode>();

        /// <summary>
        /// Fired when the filter settings change.
        /// </summary>
        public event EventDelegate<AnatomyFilter> FilterChanged;

        /// <summary>
        /// Fired when the top level anatomy is changed.
        /// </summary>
        public event EventDelegate<AnatomyFilter> TopLevelAnatomyChanged;

        public AnatomyFilter(AnatomyController anatomyController)
            : base("Medical.GUI.Anatomy.AnatomyFilter.layout")
        {
            scrollView = widget as ScrollView;
            this.anatomyController = anatomyController;
            topLevelButtons.clear();
        }

        public override void Dispose()
        {
            clear();
            base.Dispose();
        }

        public void refreshCategories()
        {
            clear();

            NaturalSort<String> sort = new NaturalSort<string>();

            createGroup("Systems", "System", TopLevelMode.System, anatomyController.Systems.Select(i => i.AnatomicalName).OrderBy(i => i, sort));
            createGroup("Regions", "Region", TopLevelMode.Region, anatomyController.Regions.Select(i => i.AnatomicalName).OrderBy(i => i, sort));
            createGroup("Classificatons", "Classification", TopLevelMode.Classification, anatomyController.Classifications.Select(i => i.AnatomicalName).OrderBy(i => i, sort));
            createGroup("Structures", "Structure", TopLevelMode.Structure, anatomyController.Structures.Select(i => i.AnatomicalName).OrderBy(i => i, sort));
            topLevelButtons.Selection = anatomyController.TopLevelMode;

            var size = flowLayout.DesiredSize;
            size.Width = scrollView.Width;
            scrollView.CanvasSize = size;
            size.Width = scrollView.ViewCoord.width - scrollView.ViewCoord.left;
            flowLayout.WorkingSize = size;
            flowLayout.layout();
        }

        public void clear()
        {
            topLevelButtons.clear();
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

        private void createGroup(String caption, String facetName, TopLevelMode topLevelMode, IEnumerable<String> items)
        {
            List<CheckButton> groupCheckButtons = new List<CheckButton>();
            List<String> activeFacetValues = new List<string>();
            AnatomyFacet facet = new AnatomyFacet(facetName, activeFacetValues);

            Button labelButton = scrollView.createWidgetT("Button", "Medical.AnatomyFilterButton", 0, 0, widget.Width, ScaleHelper.Scaled(25), Align.Left | Align.Top, "") as Button;
            labelButton.Caption = caption;
            labelButton.ForwardMouseWheelToParent = true;
            labelButton.MouseButtonClick += (sender, e) =>
                {
                    anatomyController.TopLevelMode = topLevelMode;
                    fireTopLevelAnatomyChanged();
                };
            flowLayout.addChild(new MyGUILayoutContainer(labelButton));
            childWidgets.Add(labelButton);
            topLevelButtons.addButton(topLevelMode, labelButton);

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
                        fireFilterChanged();
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
                            fireFilterChanged();
                        }
                        else
                        {
                            activeFacetValues.Remove(item);
                            if(activeFacetValues.Count == 0)
                            {
                                allCheckButton.Checked = true;
                                //The allCheckButton.CheckedChanged is triggered here
                            }
                            else
                            {
                                fireFilterChanged();
                            }
                        }
                    };
                flowLayout.addChild(new MyGUILayoutContainer(button));
                childWidgets.Add(button);
                groupCheckButtons.Add(checkButton);
            }
        }

        private void fireFilterChanged()
        {
            if(FilterChanged != null)
            {
                FilterChanged.Invoke(this);
            }
        }

        private void fireTopLevelAnatomyChanged()
        {
            if (TopLevelAnatomyChanged != null)
            {
                TopLevelAnatomyChanged.Invoke(this);
            }
        }
    }
}
