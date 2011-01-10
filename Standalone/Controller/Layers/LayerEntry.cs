using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class LayerEntry : Saveable
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

        public void apply(float multiplier, List<TransparencyInterface> unvisitedInterfaces)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(renderGroup);
            if(group != null)
            {
                TransparencyInterface obj = group.getTransparencyObject(transparencyObject);
                if (obj != null)
                {
                    obj.smoothBlend(alphaValue, multiplier);
                    unvisitedInterfaces.Remove(obj);
                }
            }
        }

        public void timedApply(float time, List<TransparencyInterface> unvisitedInterfaces)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(renderGroup);
            if (group != null)
            {
                TransparencyInterface obj = group.getTransparencyObject(transparencyObject);
                if (obj != null)
                {
                    obj.timedBlend(alphaValue, time);
                    unvisitedInterfaces.Remove(obj);
                }
            }
        }

        public void instantlyApply(List<TransparencyInterface> unvisitedInterfaces)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(renderGroup);
            if (group != null)
            {
                TransparencyInterface obj = group.getTransparencyObject(transparencyObject);
                if (obj != null)
                {
                    obj.CurrentAlpha = alphaValue;
                    unvisitedInterfaces.Remove(obj);
                }
            }
        }

        public RenderGroup RenderGroup
        {
            get
            {
                return renderGroup;
            }
        }

        public String TransparencyObject
        {
            get
            {
                return transparencyObject;
            }
        }

        public float AlphaValue
        {
            get
            {
                return alphaValue;
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
