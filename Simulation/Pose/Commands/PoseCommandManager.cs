using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    public static class PoseCommandManager
    {
        private static Dictionary<String, PoseCommand> poseCommands = new Dictionary<string, PoseCommand>();

        internal static void addAction(PoseCommandAction action)
        {
            foreach (String commandName in action.CommandNames)
            {
                PoseCommand command;
                if (!poseCommands.TryGetValue(commandName, out command))
                {
                    command = new PoseCommand();
                    poseCommands.Add(commandName, command);
                }
                command.addAction(action);
            }
        }

        internal static void removeAction(PoseCommandAction action)
        {
            foreach (String commandName in action.CommandNames)
            {
                PoseCommand command;
                if (poseCommands.TryGetValue(commandName, out command))
                {
                    command.removeAction(action);
                    if (command.IsEmpty)
                    {
                        poseCommands.Remove(commandName);
                    }
                }
            }
        }

        public static void runPosingStarted(String commandName)
        {
            PoseCommand command;
            if (commandName != null && poseCommands.TryGetValue(commandName, out command))
            {
                command.posingStarted();
            }
        }

        public static void runPosingEnded(String commandName)
        {
            PoseCommand command;
            if (commandName != null && poseCommands.TryGetValue(commandName, out command))
            {
                command.posingEnded();
            }
        }
    }
}
