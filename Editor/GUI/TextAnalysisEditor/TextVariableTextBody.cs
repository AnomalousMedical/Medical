using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    interface TextVariableTextBody
    {
        void insertVariableString(String variableText);

        void removeVariable(TextVariableEditor variable);

        void findNextInstance(String variableText);
    }
}
