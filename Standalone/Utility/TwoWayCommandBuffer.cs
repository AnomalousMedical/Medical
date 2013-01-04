using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TwoWayCommandBuffer
    {
        private Node<TwoWayCommand> firstCommand = null;
        private Node<TwoWayCommand> lastCommand = null;
        private Node<TwoWayCommand> currentUndoCommand = null;
        private Node<TwoWayCommand> currentExecuteCommand = null;

        public TwoWayCommandBuffer()
        {
            
        }

        /// <summary>
        /// Add a command to the end of the list.
        /// </summary>
        /// <param name="command"></param>
        public void pushCommand(TwoWayCommand command)
        {
            if (lastCommand == null)
            {
                currentExecuteCommand = firstCommand = lastCommand = new Node<TwoWayCommand>(command);
            }
            else
            {
                Node<TwoWayCommand> oldLast = lastCommand;
                lastCommand = new Node<TwoWayCommand>(command);
                lastCommand.Previous = oldLast;
                oldLast.Next = lastCommand;

                if (currentExecuteCommand == null)
                {
                    //Did we add a command at the end with nothing to execute
                    currentExecuteCommand = lastCommand;
                }
            }
        }

        /// <summary>
        /// Trim the commands after the current undo command.
        /// </summary>
        public void trimCommands()
        {
            if (currentUndoCommand != null)
            {
                currentExecuteCommand = null;
                currentUndoCommand.Next = null;
                lastCommand = currentUndoCommand;
            }
            else
            {
                clear();
            }
        }

        /// <summary>
        /// Pops the first command off the buffer.
        /// </summary>
        public void popFirstCommand()
        {
            if (firstCommand != null)
            {
                //If there is only one command, clear everything
                if (firstCommand == lastCommand)
                {
                    clear();
                }
                else
                {
                    //Make sure this command wasn't one of the signifigant ones
                    if (firstCommand == currentUndoCommand)
                    {
                        currentUndoCommand = null;
                    }
                    if (firstCommand == currentExecuteCommand)
                    {
                        currentExecuteCommand = firstCommand.Next;
                    }

                    //Remove the first command node
                    firstCommand = firstCommand.Next;
                    if (firstCommand != null)
                    {
                        firstCommand.Previous = null;
                    }
                }
            }
        }

        public void clear()
        {
            currentUndoCommand = currentExecuteCommand = firstCommand = lastCommand = null;
        }

        /// <summary>
        /// Executes the current command in the buffer and increments to the next command or does nothing if you are already at the last command.
        /// </summary>
        public void execute()
        {
            if (currentExecuteCommand != null)
            {
                currentExecuteCommand.Data.execute();
                currentUndoCommand = currentExecuteCommand;
                currentExecuteCommand = currentExecuteCommand.Next;
            }
        }

        /// <summary>
        /// Undoes the current command in the buffer and decrements to the previous command or does nothing if you are already at the first command.
        /// </summary>
        public void undo()
        {
            if (currentUndoCommand != null)
            {
                currentUndoCommand.Data.undo();
                currentExecuteCommand = currentUndoCommand;
                currentUndoCommand = currentUndoCommand.Previous;
            }
        }
    }
}
