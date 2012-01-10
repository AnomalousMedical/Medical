using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    delegate void VariableChosenCallback(DataFieldInfo dataField);

    interface TextVariableTextBody
    {
        void insertVariableString(String variableText);

        void removeVariable(TextVariableEditor variable);

        void findNextInstance(String variableText);

        void openVariableBrowser(VariableChosenCallback variableChosenCallback);
    }
}
