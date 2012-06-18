using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical
{
    public interface ICustomCellProvider
    {
        /// <summary>
        /// Create a cell for the given type. If null is returned it is assumed
        /// this interface does not provide a cell for the type and the search
        /// will continue.
        /// </summary>
        /// <param name="propType">The type to create a cell for.</param>
        /// <returns>The TableCell for the type.</returns>
        TableCell createCell(Type propType, bool hasBrowser, EditableProperty property);
    }
}
