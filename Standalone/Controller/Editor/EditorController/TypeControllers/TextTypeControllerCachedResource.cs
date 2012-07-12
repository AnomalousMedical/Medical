using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TextTypeControllerCachedResource : TextCachedResource
    {
        TextTypeController typeController;

        public TextTypeControllerCachedResource(String file, String text, TextTypeController typeController)
            :base(file, text)
        {
            this.typeController = typeController;
        }

        public override void save()
        {
            typeController.saveText(File, CachedString);
        }
    }
}
