using Medical;
using Medical.GUI;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Developer
{
    class SaveMicrocodeCacheTask : Task
    {
        public SaveMicrocodeCacheTask()
            :base("SaveMicrocodeCache", "Save Microcode Cache", CommonResources.NoIcon, "Developer")
        {

        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Save Microcode Cache", Environment.CurrentDirectory, Root.getSingleton().getRenderSystem().Name + ".mcc", "Microcode Cache (*.mcc)|*.mcc");
            saveDialog.showModal((result, path) => 
            {
                if (result == NativeDialogResult.OK)
                {
                    using (Stream stream = File.OpenWrite(path))
                    {
                        GpuProgramManager.Instance.saveMicrocodeCache(stream);
                    }
                }
            });
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }
    }
}
