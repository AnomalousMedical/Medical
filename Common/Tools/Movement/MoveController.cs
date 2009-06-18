using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public delegate void TranslationChanged(Vector3 newTranslation, Object sender);

    public class MoveController
    {
        public event TranslationChanged OnTranslationChanged;

        private Vector3 currentTranslation;
        private bool dispatchingTranslation = false;

        public MoveController()
        {

        }

        public void setTranslation(ref Vector3 translation, Object sender)
        {
            if (!dispatchingTranslation)
            {
                dispatchingTranslation = true;
                currentTranslation = translation;
                if (OnTranslationChanged != null)
                {
                    OnTranslationChanged.Invoke(translation, sender);
                }
                dispatchingTranslation = false;
            }
        }

        public Vector3 Translation
        {
            get
            {
                return currentTranslation;
            }
        }
    }
}
