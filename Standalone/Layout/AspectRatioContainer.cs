using Engine;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class AspectRatioContainer : SingleChildConcreteLayoutContainer, IDisposable
    {
        private float inverseAspectRatio = 1.0f;

        private bool autoAspectRatio = true;
        private Widget borderPanel0;
        private Widget borderPanel1;

        public AspectRatioContainer()
        {
            
        }

        public void Dispose()
        {
            destroyBorderPanels();
        }

        public override void layout()
        {
            if (Child != null)
            {
                IntVector2 loc = Location;
                IntSize2 size = WorkingSize;

                if (!autoAspectRatio)
                {
                    size.Height = (int)(size.Width * inverseAspectRatio);
                    if (size.Height > WorkingSize.Height) //Letterbox width
                    {
                        size.Height = WorkingSize.Height;
                        size.Width = (int)(size.Height * (1 / inverseAspectRatio));
                        loc.x += (WorkingSize.Width - size.Width) / 2;

                        borderPanel0.setCoord(Location.x, Location.y, loc.x - Location.x, WorkingSize.Height);
                        borderPanel1.setCoord(loc.x + size.Width, Location.y, loc.x - Location.x + 1, WorkingSize.Height);
                    }
                    else
                    {
                        loc.y += (WorkingSize.Height - size.Height) / 2;

                        borderPanel0.setCoord(Location.x, Location.y, WorkingSize.Width, loc.y - Location.y);
                        borderPanel1.setCoord(Location.x, loc.y + size.Height, WorkingSize.Width, loc.y - Location.y + 1);
                    }
                }

                Child.Location = loc;
                Child.WorkingSize = size;
                Child.layout();
            }
        }

        public bool AutoAspectRatio
        {
            get
            {
                return autoAspectRatio;
            }
            set
            {
                if (autoAspectRatio != value)
                {
                    autoAspectRatio = value;
                    if (autoAspectRatio)
                    {
                        destroyBorderPanels();
                    }
                    else
                    {
                        createBorderPanels();
                    }
                }
            }
        }

        public float AspectRatio
        {
            get
            {
                if (inverseAspectRatio != 0.0f)
                {
                    return 1.0f / inverseAspectRatio;
                }
                else
                {
                    return 1.0f;
                }
            }
            set
            {
                if (value != 0.0f)
                {
                    inverseAspectRatio = 1.0f / value;
                }
                else
                {
                    inverseAspectRatio = 1.0f;
                }
            }
        }

        void createBorderPanels()
        {
            if (borderPanel0 == null)
            {
                borderPanel0 = Gui.Instance.createWidgetT("Widget", "Medical.SceneViewBorder", 0, 0, 1, 1, Align.Default, "Back", "");
                borderPanel1 = Gui.Instance.createWidgetT("Widget", "Medical.SceneViewBorder", 0, 0, 1, 1, Align.Default, "Back", "");
            }
        }

        void destroyBorderPanels()
        {
            if (borderPanel0 != null)
            {
                Gui.Instance.destroyWidget(borderPanel0);
                Gui.Instance.destroyWidget(borderPanel1);
                borderPanel0 = null;
                borderPanel1 = null;
            }
        }
    }
}
