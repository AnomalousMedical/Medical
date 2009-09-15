using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    class TiledWatermark : Watermark
    {
        private PanelOverlayElement panel;
        private Overlay overlay;
        private String name;
        float markWidth = 100;
        float markHeight = 100;
        float screenWidth = 100;
        float screenHeight = 100;
        String materialName;

        public TiledWatermark(String name, String materialName, float width, float height)
        {
            this.name = name;
            this.markWidth = width;
            this.markHeight = height;
            this.materialName = materialName;
        }

        public override void createOverlays()
        {
            overlay = OverlayManager.getInstance().create(name + "_WatermarkOverlay");
            panel = OverlayManager.getInstance().createOverlayElement(PanelOverlayElement.TypeName, name + "_WatermarkPanel") as PanelOverlayElement;
            panel.setUV(0, 0, screenWidth / markWidth, screenHeight / markHeight);
            panel.setMaterialName(materialName);
            overlay.add2d(panel);
        }

        public override void sizeChanged(float width, float height)
        {
            screenWidth = width;
            screenHeight = height;
            if (panel != null)
            {
                panel.setUV(0, 0, width / markWidth, height / markHeight);
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
