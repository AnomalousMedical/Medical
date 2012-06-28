using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;

namespace Medical.Presentation
{
    class PresentationIndex
    {
        private List<PresentationEntry> entries = new List<PresentationEntry>();

        public event Action<PresentationEntry> EntryAdded;
        public event Action<PresentationEntry> EntryRemoved;

        public void addEntry(PresentationEntry entry)
        {
            entries.Add(entry);
            if (EntryAdded != null)
            {
                EntryAdded.Invoke(entry);
            }
        }

        public void removeEntry(PresentationEntry entry)
        {
            if (EntryRemoved != null)
            {
                EntryRemoved.Invoke(entry);
            }
            entries.Remove(entry);
        }

        public AnomalousMvcContext buildMvcContext()
        {
            AnomalousMvcContext mvcContex = new AnomalousMvcContext();
            if (entries.Count > 0)
            {
                NavigationModel navModel = new NavigationModel(NavigationModel.DefaultName);
                mvcContex.Controllers.add(new MvcController("__PresentationReserved_Common",
                    new RunCommandsAction("Startup",
                        new ShowViewCommand(entries[0].Name),
                        new HideMainInterfaceCommand(),
                        new SaveCameraPositionCommand(),
                        new SaveMedicalStateCommand(),
                        new SaveLayersCommand(),
                        new SaveMusclePositionCommand()),
                    new RunCommandsAction("Shutdown",
                        new ShowMainInterfaceCommand(),
                        new RestoreCameraPositionCommand(),
                        new RestoreMedicalStateCommand(),
                        new RestoreLayersCommand(),
                        new RestoreMusclePositionCommand()),
                    new RunCommandsAction("MoveNext", new NavigateNextCommand()),
                    new RunCommandsAction("MovePrevious", new NavigatePreviousCommand()),
                    new RunCommandsAction("Close", new ShutdownCommand())));

                foreach (PresentationEntry entry in entries)
                {
                    entry.addToContext(mvcContex, navModel);
                }
            }
            return mvcContex;
        }
    }
}
