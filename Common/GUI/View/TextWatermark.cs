using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    class TextWatermark : Watermark
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
        }

        public override void createOverlays()
        {
            overlay = OverlayManager.getInstance().create(name + "Overlay__");
            statsPanel = OverlayManager.getInstance().createOverlayElement(PanelOverlayElement.TypeName, name + "StatsOverlayPanel__") as PanelOverlayElement;
            fpsTextArea = OverlayManager.getInstance().createOverlayElement(TextAreaOverlayElement.TypeName, name + "StatsFpsText__") as TextAreaOverlayElement;
            statsPanel.addChild(fpsTextArea);
            fpsTextArea.setFontName("Watermark");
            fpsTextArea.setMetricsMode(GuiMetricsMode.GMM_PIXELS);
            fpsTextArea.setCharHeight(markHeight);
            fpsTextArea.setCaption(text);
            fpsTextArea.setVerticalAlignment(GuiVerticalAlignment.GVA_BOTTOM);
            fpsTextArea.setPosition(5.0f, -markHeight);

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
    }
}
