using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.IO;
using Logging;

namespace Medical
{
    class ManagePluginInstructions : Saveable
    {
        private List<String> deleteFiles = new List<string>();
        private List<String> moveFiles = new List<string>();

        public ManagePluginInstructions()
        {

        }

        public void addFileToDelete(String file)
        {
            deleteFiles.Add(file);
        }

        public void addFileToMove(String file)
        {
            moveFiles.Add(file);
        }

        public void process(String targetDirectory)
        {
            //Manage existing plugins based on the config file.
            foreach(String file in deleteFiles)
            {
                if (file.StartsWith(targetDirectory))
                {
                    try
                    {
                        File.Delete(file);
                        Log.ImportantInfo("Deleted plugin {0}.", file);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Could not delete plugin {0} because {1}.", file, e.Message);
                    }
                }
                else
                {
                    Log.Error("Could not delete plugin {0} because it is not in the correct directory {1}.", file, targetDirectory);
                }
            }

            foreach (String file in moveFiles)
            {
                String fileName = Path.GetFileName(file);
                try
                {
                    String destinationFile = Path.Combine(targetDirectory, fileName);
                    if (File.Exists(destinationFile))
                    {
                        File.Delete(destinationFile);
                    }
                    File.Move(file, destinationFile);
                }
                catch (Exception e)
                {
                    Log.Error("Could not move plugin {0} because {1}.", file, e.Message);
                }
            }
        }

        protected ManagePluginInstructions(LoadInfo info)
        {
            info.RebuildList<String>("Delete", deleteFiles);
            info.RebuildList<String>("Move", moveFiles);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<String>("Delete", deleteFiles);
            info.ExtractList<String>("Move", moveFiles);
        }
    }
}
