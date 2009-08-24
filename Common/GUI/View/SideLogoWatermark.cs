using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    class SideLogoWatermark : Watermark
    {
        private PanelOverlayElement panel;
        private Overlay overlay;
        private String name;
        float markWidth = 100;
        float markHeight = 100;
        String materialName;

        public SideLogoWatermark(String name, String materialName, float width, float height)
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
            panel.setVerticalAlignment(GuiVerticalAlignment.GVA_BOTTOM);
            panel.setMetricsMode(GuiMetricsMode.GMM_PIXELS);
            panel.setMaterialName(materialName);
            overlay.add2d(panel);
            panel.setDimensions(markWidth, markHeight);
            panel.setPosition(0, -markHeight);
        }

        public override void sizeChanged(float width, float height)
        {
            
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
