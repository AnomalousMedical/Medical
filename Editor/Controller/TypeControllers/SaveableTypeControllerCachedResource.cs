using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class SaveableTypeControllerCachedResource : SaveableCachedResource
    {
        SaveableTypeController typeController;

        public SaveableTypeControllerCachedResource(String file, Saveable saveable, SaveableTypeController typeController)
            :base(file, saveable)
        {
            this.typeController = typeController;
        }

        public override void save()
        {
            typeController.saveObject(File, Saveable);
        }
    }
}
