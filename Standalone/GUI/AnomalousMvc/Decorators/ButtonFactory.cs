using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.AnomalousMvc
{
    public interface ButtonFactory
    {
        void addTextButton(ButtonDefinition buttonDefinition, int x, int y, int width, int height);

        void addCloseButton(CloseButtonDefinition buttonDefinition, int x, int y, int width, int height);
    }
}
