using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical
{
    class LayerStateTableCellProvider : ICustomCellProvider
    {
        private static LayerStateTableCellProvider instance;

        public static LayerStateTableCellProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LayerStateTableCellProvider();
                }
                return instance;
            }
        }

        public TableCell createCell(Type propType, EditableProperty property)
        {
            if (propType == typeof(LayerState))
            {
                return new LayerStateTableCell((LayerStateEditableProperty)property);
            }
            return null;
        }
    }
}
