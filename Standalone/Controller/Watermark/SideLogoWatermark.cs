using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    public class SideLogoWatermark : Watermark
    {
        private PanelOverlayElement panel;
        private Overlay overlay;
        private String name;
        float markWidth = 100;
        float markHeight = 100;
        String materialName;
        float leftOffset;
        float bottomOffset;

        public SideLogoWatermark(String name, String materialName, float width, float height, float leftOffset, float bottomOffset)
        {
            this.name = name;
            this.markWidth = width;
            this.markHeight = height;
            this.materialName = materialName;
            this.leftOffset = leftOffset;
            this.bottomOffset = bottomOffset;
            
            //Create overlays
            overlay = OverlayManager.getInstance().create(name + "_WatermarkOverlay");
            panel = OverlayManager.getInstance().createOverlayElement(PanelOverlayElement.TypeName, name + "_WatermarkPanel") as PanelOverlayElement;
            panel.setUV(0, 0, 1, 1);
            panel.setVerticalAlignment(GuiVerticalAlignment.GVA_BOTTOM);
            panel.setMetricsMode(GuiMetricsMode.GMM_PIXELS);
            panel.setMaterialName(materialName);
            overlay.add2d(panel);
            panel.setDimensions(markWidth, markHeight);
            panel.setPosition(leftOffset, -markHeight - bottomOffset);
            Visible = true;
        }

        public void Dispose()
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

        public bool Visible
        {
            get
            {
                return overlay.isVisible();
            }
            set
            {
                if (overlay != null)
                {
                    if (value && !overlay.isVisible())
                    {
                        overlay.show();

                    }
                    else if (!value && overlay.isVisible())
                    {
                        overlay.hide();
                    }
                }
            }
        }
    }
}
