using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class SaveableTypeControllerCachedResource<T> : SaveableCachedResource<T>
        where T : Saveable
    {
        SaveableTypeController<T> typeController;

        public SaveableTypeControllerCachedResource(String file, T saveable, SaveableTypeController<T> typeController)
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
