using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    class CenteredWatermark : Watermark
    {
        private PanelOverlayElement panel;
        private Overlay overlay;
        private String name;
        float markWidth = 100;
        float markHeight = 100;
        String materialName;

        public CenteredWatermark(String name, String materialName, float width, float height)
        {
            this.name = name;
            this.markWidth = width;
            this.markHeight = height;
            this.materialName = materialName;
        }

        public override void createOverlays()
        {
            overlay = OverlayManager.getInstance().create(name + "Overlay__");
            panel = OverlayManager.getInstance().createOverlayElement(PanelOverlayElement.TypeName, name + "StatsOverlayPanel__") as PanelOverlayElement;
            panel.setUV(0, 0, 1, 1);
            panel.setVerticalAlignment(GuiVerticalAlignment.GVA_CENTER);
            panel.setHorizontalAlignment(GuiHorizontalAlignment.GHA_CENTER);
            panel.setMetricsMode(GuiMetricsMode.GMM_RELATIVE);
            panel.setMaterialName(materialName);
            overlay.add2d(panel);
            panel.setDimensions(markWidth, markHeight);
            panel.setPosition(-markWidth / 2.0f, -markHeight / 2.0f);
        }

        public override void sizeChanged(float width, float height)
        {
            if(panel != null)
            {
                float aspect = width / height;
                panel.setDimensions(markWidth, markHeight * aspect);
                panel.setPosition(-panel.getWidth() / 2.0f, -panel.getHeight() / 2.0f);
            }
        }

        public override void setVisible(bool visible)
        {
            if (overlay != null)
            {
                if (visible && !overlay.isVisible())
                {
                    overlay.show();

                }
                else if (!visible && overlay.isVisible())
                {
                    overlay.hide();
                }
            }
        }

        public override void destroyOverlays()
        {
            if (panel != null)
            {
                overlay.remove2d(panel);
                OverlayManager.getInstance().destroyOverlayElement(panel);
                OverlayManager.getInstance().destroy(overlay);
                panel = null;
                overlay = null;
            }
        }
    }
}
