using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class LayerEntry : Saveable
    {
        private RenderGroup renderGroup;
        private String transparencyObject;
        private float alphaValue;

        public LayerEntry(TransparencyInterface trans)
        {
            this.renderGroup = trans.RenderGroup;
            this.transparencyObject = trans.ObjectName;
            alphaValue = trans.CurrentAlpha;
        }

        public void apply()
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(renderGroup);
            if(group != null)
            {
                TransparencyInterface obj = group.getTransparencyObject(transparencyObject);
                if (obj != null)
                {
                    obj.smoothBlend(alphaValue);
                }
            }
        }

        #region Saveable Members

        private const string RENDER_GROUP = "RenderGroup";
        private const string TRANSPARENCY_OBJECT = "TransparencyObject";
        private const string ALPHA_VALUE = "AlphaValue";

        protected LayerEntry(LoadInfo info)
        {
            renderGroup = info.GetValue<RenderGroup>(RENDER_GROUP);
            transparencyObject = info.GetString(TRANSPARENCY_OBJECT);
            alphaValue = info.GetFloat(ALPHA_VALUE);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(RENDER_GROUP, renderGroup);
            info.AddValue(TRANSPARENCY_OBJECT, transparencyObject);
            info.AddValue(ALPHA_VALUE, alphaValue);
        }

        #endregion
    }
}
