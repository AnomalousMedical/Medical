using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Logging;
using Engine.Editing;

namespace Medical.GUI
{
    public class NewProjectDialog : InputBrowserWindow<ProjectTemplate>
    {
        public static NewProjectDialog ShowDialog(Browser browse, Action<ProjectTemplate, String> resultCallback)
        {
            NewProjectDialog projectDialog = new NewProjectDialog(browse.Prompt);
            projectDialog.setBrowser(browse);
            projectDialog.Closing += (sender, e) =>
            {
                if (projectDialog.Accepted)
                {
                    resultCallback(projectDialog.SelectedValue, projectDialog.FullProjectName);
                }
            };

            projectDialog.Closed += (sender, e) =>
            {
                projectDialog.Dispose();
            };

            projectDialog.center();
            projectDialog.ensureVisible();
            projectDialog.open(true);
            return projectDialog;
        }

        private EditBox projectLocation;

        protected NewProjectDialog(String caption)
            :base("Create Project", "", "Medical.GUI.Editor.NewProjectDialog.NewProjectDialog.layout")
        {
            window.Caption = caption;
            projectLocation = window.findWidget("ProjectLocation") as EditBox;
            projectLocation.Caption = EditorConfig.ProjectDirectory;
            if (!Directory.Exists(projectLocation.Caption))
            {
                try
                {
                    Directory.CreateDirectory(projectLocation.Caption);
                }
                catch (Exception e)
                {
                    Log.Error("Could not create project directory {0} because {1}", projectLocation.Caption, e.Message);
                }
            }

            Button browseButton = window.findWidget("BrowseButton") as Button;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
        }

        public override void Dispose()
        {
            base.Dispose();
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
                String projName = Input;
                return Path.Combine(projectLocation.Caption, projName);
            }
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            DirDialog dirDialog = new DirDialog(MainWindow.Instance, "Choose the path to load files from.", projectLocation.Caption);
            dirDialog.showModal((result, path) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    projectLocation.Caption = path;
                }
            });
        }
    }
}
