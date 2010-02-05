using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    public class TextWatermark : Watermark
    {
        private PanelOverlayElement statsPanel;
        private TextAreaOverlayElement fpsTextArea;
        private Overlay overlay;
        private String name;
        float markHeight = 100;
        String text;

        public TextWatermark(String name, String text, float height)
        {
            this.name = name;
            this.markHeight = height;
            this.text = text;
            VerticalAlignment = GuiVerticalAlignment.GVA_BOTTOM;
        }

        public String Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (fpsTextArea != null)
                {
                    fpsTextArea.setCaption(value);
                }
            }
        }

        public override void createOverlays()
        {
            overlay = OverlayManager.getInstance().create(name + "_WatermarkOverlay");
            statsPanel = OverlayManager.getInstance().createOverlayElement(PanelOverlayElement.TypeName, name + "_WatermarkPanel") as PanelOverlayElement;
            fpsTextArea = OverlayManager.getInstance().createOverlayElement(TextAreaOverlayElement.TypeName, name + "_WatermarkText") as TextAreaOverlayElement;
            statsPanel.addChild(fpsTextArea);
            fpsTextArea.setFontName("Watermark");
            fpsTextArea.setMetricsMode(GuiMetricsMode.GMM_PIXELS);
            fpsTextArea.setCharHeight(markHeight);
            fpsTextArea.setCaption(text);
            fpsTextArea.setVerticalAlignment(VerticalAlignment);
            float xPos = 5.0f;
            float yPos = 5.0f;
            if(VerticalAlignment == GuiVerticalAlignment.GVA_BOTTOM)
            {
                yPos = -markHeight;
            }
            fpsTextArea.setPosition(xPos, yPos);

            overlay.add2d(statsPanel);
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
            if (statsPanel != null)
            {
                overlay.remove2d(statsPanel);
                OverlayManager.getInstance().destroyOverlayElement(statsPanel);
                OverlayManager.getInstance().destroyOverlayElement(fpsTextArea);
                OverlayManager.getInstance().destroy(overlay);
                statsPanel = null;
                fpsTextArea = null;
                overlay = null;
            }
        }

        public override Watermark clone(String newName)
        {
            TextWatermark wm = new TextWatermark(newName, text, markHeight);
            wm.VerticalAlignment = VerticalAlignment;
            return wm;
        }

        public GuiVerticalAlignment VerticalAlignment { get; set; }
    }
}
