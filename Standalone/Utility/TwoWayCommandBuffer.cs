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
        public void push(TwoWayCommand command)
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

                if (OnLast)
                {
                    //Did we add a command at the end with nothing to execute
                    currentExecuteCommand = lastCommand;
                }
            }
        }

        /// <summary>
        /// Trim the commands after the current undo command.
        /// </summary>
        public void trim()
        {
            if (currentUndoCommand != null)
            {
                Node<TwoWayCommand> currentTrimCommand = currentUndoCommand.Next;
                while (currentTrimCommand != null)
                {
                    currentTrimCommand.Data.trimmed();
                    currentTrimCommand.Data.removed();
                    currentTrimCommand = currentTrimCommand.Next;
                }
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
        public void popFirst()
        {
            if (firstCommand != null)
            {
                firstCommand.Data.poppedFront();
                firstCommand.Data.removed();
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
            Node<TwoWayCommand> current = firstCommand;
            while (current != null)
            {
                current.Data.cleared();
                current.Data.removed();
                current = current.Next;
            }
            currentUndoCommand = currentExecuteCommand = firstCommand = lastCommand = null;
        }

        /// <summary>
        /// Executes the current command in the buffer and increments to the next command or does nothing if you are already at the last command.
        /// </summary>
        public void execute()
        {
            if (currentExecuteCommand != null)
            {
                //Update the state before running the command, makes the state easier to track if you are updating a ui to match.
                var commandToRun = currentExecuteCommand;
                currentUndoCommand = currentExecuteCommand;
                currentExecuteCommand = currentExecuteCommand.Next;
                commandToRun.Data.execute();
            }
        }

        /// <summary>
        /// Skips the current command in the buffer and increments to the next command or does nothing if you are already at the last command.
        /// </summary>
        public void skipExecute()
        {
            if (currentExecuteCommand != null)
            {
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
                //Update the state before running the command, makes the state easier to track if you are updating a ui to match.
                var commandToRun = currentUndoCommand;
                currentExecuteCommand = currentUndoCommand;
                currentUndoCommand = currentUndoCommand.Previous;
                commandToRun.Data.undo();
            }
        }

        /// <summary>
        /// This is true if we are on the last command.
        /// </summary>
        public bool OnLast
        {
            get
            {
                return currentExecuteCommand == null;
            }
        }

        /// <summary>
        /// This is true if we are on the first command.
        /// </summary>
        public bool OnFirst
        {
            get
            {
                return currentUndoCommand == null;
            }
        }

        /// <summary>
        /// Get the current count of elements, note that this will be computed each time
        /// </summary>
        public int Count
        {
            get
            {
                int i = 0;
                Node<TwoWayCommand> currentNode = firstCommand;
                while (currentNode != null)
                {
                    ++i;
                    currentNode = currentNode.Next;
                }
                return i;
            }
        }
    }
}
