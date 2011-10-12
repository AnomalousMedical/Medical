﻿using System;
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

        public void process()
        {
            //Manage existing plugins based on the config file.
            String pluginDirectory = Path.GetFullPath(MedicalConfig.PluginConfig.PluginsFolder);
            foreach(String file in deleteFiles)
            {
                if (file.StartsWith(pluginDirectory))
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
                    Log.Error("Could not delete plugin {0} because it is not in the correct directory {1}.", file, pluginDirectory);
                }
            }

            foreach (String file in moveFiles)
            {
                String fileName = Path.GetFileName(file);
                try
                {
                    File.Move(file, Path.Combine(pluginDirectory, fileName));
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
