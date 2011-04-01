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
        private TextAreaOverlayElement textOverlay = null;
        private PanelOverlayElement repeaterPanel = null;
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
                if (textOverlay != null)
                {
                    panel.removeChild(textOverlay.getName());
                    OverlayManager.getInstance().destroyOverlayElement(textOverlay);
                }
                if (repeaterPanel != null)
                {
                    overlay.remove2d(repeaterPanel);
                    OverlayManager.getInstance().destroyOverlayElement(repeaterPanel);
                }
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

        public void addText(String text)
        {
            textOverlay = OverlayManager.getInstance().createOverlayElement(TextAreaOverlayElement.TypeName, name + "TextArea__") as TextAreaOverlayElement;
            panel.addChild(textOverlay);
            textOverlay.setFontName("StatsFont");
            textOverlay.setVerticalAlignment(GuiVerticalAlignment.GVA_TOP);
            textOverlay.setMetricsMode(GuiMetricsMode.GMM_PIXELS);
            textOverlay.setCharHeight(15.0f);
            textOverlay.setPosition(textOverlay.getLeft(), -15);
            textOverlay.setCaption(text);
        }

        public void addRepeatingOverlayElement(String materialName, float imageWidth, float imageHeight, float repeat, float overlaySizeX, float overlaySizeY)
        {
            float aspectRatio = imageHeight / imageWidth;
            float imageRepeatWidth = overlaySizeX / repeat;
            float imageRepeatHeight = imageRepeatWidth * aspectRatio;
            imageRepeatWidth = overlaySizeX / imageRepeatWidth;
            imageRepeatHeight = overlaySizeY / imageRepeatHeight;

            repeaterPanel = OverlayManager.getInstance().createOverlayElement(PanelOverlayElement.TypeName, name + "_WatermarkRepeaterPanel") as PanelOverlayElement;
            repeaterPanel.setUV(0, 0, imageRepeatWidth, imageRepeatHeight);
            //panel.setVerticalAlignment(GuiVerticalAlignment.GVA_BOTTOM);
            repeaterPanel.setMetricsMode(GuiMetricsMode.GMM_PIXELS);
            repeaterPanel.setMaterialName(materialName);
            overlay.remove2d(panel);
            overlay.add2d(repeaterPanel);
            overlay.add2d(panel);
            repeaterPanel.setDimensions(overlaySizeX, overlaySizeY);
            repeaterPanel.setPosition(0, 0);
        }
    }
}
