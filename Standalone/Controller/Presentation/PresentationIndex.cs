using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine;

namespace Medical.Presentation
{
    public class PresentationIndex : Saveable
    {
        private List<PresentationEntry> entries = new List<PresentationEntry>();
        private ulong uniqueNameIndex = 0;

        public PresentationIndex()
        {

        }

        public void addEntry(PresentationEntry entry)
        {
            entry.UniqueName = "Entry" + uniqueNameIndex++;
            entries.Add(entry);
        }

        public void removeEntry(PresentationEntry entry)
        {
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
                        new ShowViewCommand(entries[0].UniqueName),
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
                mvcContex.addModel(NavigationModel.DefaultName, navModel);
                mvcContex.StartupAction = "__PresentationReserved_Common/Startup";
                mvcContex.ShutdownAction = "__PresentationReserved_Common/Shutdown";
            }
            return mvcContex;
        }

        public IEnumerable<PresentationEntry> Entries
        {
            get
            {
                return entries;
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("UniqueNameIndex", uniqueNameIndex);
            info.ExtractList<PresentationEntry>("Entry", entries);
        }

        protected PresentationIndex(LoadInfo info)
        {
            uniqueNameIndex = info.GetUInt64("UniqueNameIndex");
            info.RebuildList<PresentationEntry>("Entry", entries);
        }
    }
}
