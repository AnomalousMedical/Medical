using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface TwoWayCommand
    {
        /// <summary>
        /// Called to execute the command.
        /// </summary>
        void execute();

        /// <summary>
        /// Called to undo the command.
        /// </summary>
        void undo();

        /// <summary>
        /// Called when the command has been popped off the front of the buffer. This means that the buffer was full and the
        /// command is no longer needed.
        /// </summary>
        void poppedFront();

        /// <summary>
        /// Called when the command has been removed from the buffer because clear was called on it.
        /// </summary>
        void cleared();

        /// <summary>
        /// Called when the command has been removed from the buffer because it was after the current command and another command
        /// was added.
        /// </summary>
        void trimmed();

        /// <summary>
        /// Called when a command has been removed from the buffer. It does not matter if it was poppedFront, cleared or trimmed this
        /// function will be called. It will also be called after the event that corresponds to why it was removed.
        /// </summary>
        void removed();
    }
}
