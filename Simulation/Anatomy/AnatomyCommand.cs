using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.ObjectManagement;

namespace Medical
{
    public enum AnatomyCommandUIType
    {
        Numeric, //Will present a UI where numbers are changed with a slider. Uses the NumericValue property.
        Executable, //Will present a UI with a push button that executes an action. Calls the execute function.
        Boolean, //Will present a UI that turns on and off. Calls the BooleanValue property.
    }

    /// <summary>
    /// This delegate is used to alert subscribers that the numeric value has changed for this command.
    /// </summary>
    /// <param name="command">The command issuing the update.</param>
    /// <param name="value">The new value.</param>
    public delegate void AnatomyNumericValueChanged(AnatomyCommand command, float value);

    public delegate void AnatomyBooleanValueChanged(AnatomyCommand command, bool value);

    /// <summary>
    /// AnatomyCommands are associated with a piece of anatomy and are designed
    /// to change it in some way. They do this by presenting a UI with one of
    /// the AnatomyCommandUITypes and then respond to the input from these UIs.
    /// </summary>
    public interface AnatomyCommand : Saveable, IDisposable
    {
        /// <summary>
        /// Called when the Numeric value changes.
        /// </summary>
        event AnatomyNumericValueChanged NumericValueChanged;

        /// <summary>
        /// Called when the boolean value changes.
        /// </summary>
        event AnatomyBooleanValueChanged BooleanValueChanged;

        /// <summary>
        /// This method will be called during the AnatomyIdentifier's link
        /// phase. At this point it is valid to find any dependent objects
        /// through owner. If there is an error return false and set a message
        /// in errorMessage. This will cause the AnatomyIdentifier linking this
        /// object to be blacklisted.
        /// </summary>
        bool link(SimObject owner, AnatomyIdentifier parentAnatomy, ref String errorMessage);

        /// <summary>
        /// Get the type of command UI that should be created for this command.
        /// </summary>
        AnatomyCommandUIType UIType { get; }

        /// <summary>
        /// Get/Set the numeric value of this command. Used when the UI type is Sliding.
        /// </summary>
        float NumericValue { get; set; }

        /// <summary>
        /// The minimum numeric value for this command. Used when the UI type is Sliding.
        /// </summary>
        float NumericValueMin { get; }

        /// <summary>
        /// The maximum numeric value for this command. Used when the UI type is Sliding.
        /// </summary>
        float NumericValueMax { get; }

        /// <summary>
        /// Get/Set the boolean value of this command. Used when the UI type is Toggle.
        /// </summary>
        bool BooleanValue { get; set; }

        /// <summary>
        /// The text to display on the user interface.
        /// </summary>
        String UIText { get; }

        /// <summary>
        /// Called to execute something. Used when the UI type is Button.
        /// </summary>
        void execute();

        /// <summary>
        /// Create a command suitable for a tag group view of this command. This
        /// can return null to not support grouping commands. This method is
        /// called the first time a new type of command is encountered. If the
        /// command this function is being called on needs to add itself it
        /// should do this.
        /// </summary>
        /// <returns>An AnatomyCommand that can be used for a tag group.</returns>
        AnatomyCommand createTagGroupCommand();

        /// <summary>
        /// Process this command and possibly add it to the given
        /// tagGroupCommand in the manner that is appropriate. The value passed
        /// in will be whatever the subclass returns from createTagGroupCommand.
        /// This method will be called every time except the first time a
        /// certain command type is encountered.
        /// </summary>
        /// <param name="tagGroupCommand">Tha AnatomyCommand created by the subclass's createTagGroupCommand function.</param>
        void addToTagGroupCommand(AnatomyCommand tagGroupCommand);

        /// <summary>
        /// Create an EditInterface for this command.
        /// </summary>
        /// <returns>The newly created EditInterface.</returns>
        EditInterface createEditInterface();
    }
}
