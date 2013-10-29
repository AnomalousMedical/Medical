using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;

namespace Medical
{
    public class LayerEntry : Saveable
    {
        private String transparencyObject;
        private float alphaValue;

        public LayerEntry(TransparencyInterface trans)
        {
            this.transparencyObject = trans.ObjectName;
            alphaValue = trans.CurrentAlpha;
        }

        public void timedApply(float time, List<TransparencyInterface> unvisitedInterfaces, EasingFunction easingFunction)
        {
            TransparencyInterface obj = TransparencyController.getTransparencyObject(transparencyObject);
            if (obj != null)
            {
                obj.timedBlend(alphaValue, time, easingFunction);
                unvisitedInterfaces.Remove(obj);
            }
        }

        public void instantlyApply(List<TransparencyInterface> unvisitedInterfaces)
        {
            TransparencyInterface obj = TransparencyController.getTransparencyObject(transparencyObject);
            if (obj != null)
            {
                obj.CurrentAlpha = alphaValue;
                unvisitedInterfaces.Remove(obj);
            }
        }

        public void instantlyApplyBlendPercent(List<TransparencyInterface> unvisitedInterfaces, float percent)
        {
            TransparencyInterface obj = TransparencyController.getTransparencyObject(transparencyObject);
            if (obj != null)
            {
                obj.CurrentAlpha += (alphaValue - obj.CurrentAlpha) * percent;
                unvisitedInterfaces.Remove(obj);
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

        private const string TRANSPARENCY_OBJECT = "TransparencyObject";
        private const string ALPHA_VALUE = "AlphaValue";

        protected LayerEntry(LoadInfo info)
        {
            transparencyObject = info.GetString(TRANSPARENCY_OBJECT);
            alphaValue = info.GetFloat(ALPHA_VALUE);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(TRANSPARENCY_OBJECT, transparencyObject);
            info.AddValue(ALPHA_VALUE, alphaValue);
        }

        #endregion
    }
}
