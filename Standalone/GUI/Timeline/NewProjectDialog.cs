using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Logging;

namespace Medical.GUI
{
    class NewProjectDialog : Dialog
    {
        public event EventHandler ProjectCreated;

        private Edit projectName;
        private Edit projectLocation;

        public NewProjectDialog()
            :base("Medical.GUI.Timeline.NewProjectDialog.layout")
        {
            projectName = window.findWidget("ProjectName") as Edit;
            projectLocation = window.findWidget("ProjectLocation") as Edit;
            projectLocation.Caption = MedicalConfig.DocRoot + "/Timeline Projects";
            if (!Directory.Exists(projectLocation.Caption))
            {
                try
                {
                    Directory.CreateDirectory(projectLocation.Caption);
                }
                catch (Exception e)
                {
                    Log.Error("Could not create Timeline Project directory {0} because {1}", projectLocation.Caption, e.Message);
                }
            }

            Button browseButton = window.findWidget("BrowseButton") as Button;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
            Button createButton = window.findWidget("CreateButton") as Button;
            createButton.MouseButtonClick += new MyGUIEvent(createButton_MouseButtonClick);
            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        public String ProjectLocation
        {
            get
            {
                return projectLocation.Caption;
            }
            set
            {
                projectLocation.Caption = value;
            }
        }

        public String FullProjectName
        {
            get
            {
                return Path.Combine(projectLocation.Caption, projectName.Caption);
            }
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void createButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (Directory.Exists(projectLocation.Caption))
            {
                if (Directory.Exists(FullProjectName))
                {
                    MessageBox.show(String.Format("The project {0} already exists. Would you like to delete it and create a new one?", FullProjectName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, overwriteCallback);
                }
                else
                {
                    createProject();
                }
            }
            else
            {
                MessageBox.show(String.Format("Could not create project {0}.\nReason: The Project Location does not exist.", FullProjectName), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            using (wx.DirDialog dirDialog = new wx.DirDialog(MainWindow.Instance, "Choose the path to load files from.", projectLocation.Caption))
            {
                if (dirDialog.ShowModal() == wx.ShowModalResult.OK)
                {
                    projectLocation.Caption = dirDialog.Path;
                }
            }
        }

        private void createProject()
        {
            try
            {
                Directory.CreateDirectory(FullProjectName);
                if (ProjectCreated != null)
                {
                    ProjectCreated.Invoke(this, EventArgs.Empty);
                }
                this.close();
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Could not create project {0}.\nReason: {1}", FullProjectName, ex.Message), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        private void overwriteCallback(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                try
                {
                    Directory.Delete(FullProjectName);
                    createProject();
                }
                catch (Exception ex)
                {
                    MessageBox.show(String.Format("Could not delete old project {0}. No changes have been made.\nReason: {1}", FullProjectName, ex.Message), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
            }
        }
    }
}
