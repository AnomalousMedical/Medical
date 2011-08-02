using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class produces IImageDisplays as needed for the Timeline.
    /// </summary>
    public interface ITextDisplayFactory
    {
        ITextDisplay createTextDisplay(String cameraName);

        List<String> FontNames
        {
            get;
        }
    }
}
