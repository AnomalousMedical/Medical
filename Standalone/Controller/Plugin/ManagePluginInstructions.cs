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

        //This is not saved.
        private String targetDirectory;

        public ManagePluginInstructions(String targetDirectory)
        {
            this.targetDirectory = Path.GetFullPath(targetDirectory);
        }

        /// <summary>
        /// Add a file to delete when process is run.
        /// </summary>
        /// <param name="file"></param>
        public void addFileToDelete(String file)
        {
            deleteFiles.Add(file);
        }

        /// <summary>
        /// Add a file to move when process is run.
        /// </summary>
        /// <param name="file"></param>
        public void addFileToMove(String file)
        {
            moveFiles.Add(file);
        }

        /// <summary>
        /// Move and delete the files.
        /// </summary>
        public void process()
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

        /// <summary>
        /// Save the instructions to a file.
        /// </summary>
        public void savePersistantFile()
        {
            String file = Path.Combine(targetDirectory, "ManagePlugins.xml");
            try
            {
                using (Stream stream = File.Open(file, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite, FileShare.None))
                {
                    SharedXmlSaver.Save(this, stream);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Could not write dependency management instructions to '{0}' because {1}.", file, ex.Message);
            }
        }

        /// <summary>
        /// Delete the persistant file for these instructions.
        /// </summary>
        public void deletePersistantFile()
        {
            String file = Path.Combine(targetDirectory, "ManagePlugins.xml");
            try
            {
                File.Delete(file);
            }
            catch (Exception e)
            {
                Log.Error("Could not delete plugin management file {0} because {1}.", file, e.Message);
            }
        }

        /// <summary>
        /// Restore the instructions in a given directory.
        /// </summary>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        public static ManagePluginInstructions restore(String targetDirectory)
        {
            String file = Path.Combine(targetDirectory, "ManagePlugins.xml");
            if (File.Exists(file))
            {
                using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var loadedInstructions = SharedXmlSaver.Load<ManagePluginInstructions>(stream);
                    loadedInstructions.targetDirectory = Path.GetFullPath(targetDirectory);
                    return loadedInstructions;
                }
            }
            return null;
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
