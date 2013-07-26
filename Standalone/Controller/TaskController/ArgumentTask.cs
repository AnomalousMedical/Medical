using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class ArgumentTask<T> : Task
    {
        public ArgumentTask(String uniqueName, String name, String iconName, String category)
            :base(uniqueName, name, iconName, category)
        {

        }

        public T Argument { get; set; }
    }
}
