using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This interface abstracts the way data controls are made from the data.
    /// The data will call these factory methods and the factory implementation
    /// will handle binding that data to a control.
    /// </summary>
    public interface DataControlFactory
    {
        void pushColumnLayout();

        void popColumnLayout();

        void addField(BooleanDataField field);

        void addField(MenuItemField field);

        void addField(MultipleChoiceField field);

        void addField(NotesDataField field);

        void addField(NumericDataField field);

        void addField(PlayExampleDataField field);

        void addField(StaticTextDataField field);

        void addField(MoveCameraDataField field);

        void addField(ChangeLayersDataField field);

        void addField(MoveCameraChangeLayersDataField field);

        void addField(CloseGUIPlayTimelineField closeGUIPlayTimelineField);
    }
}
