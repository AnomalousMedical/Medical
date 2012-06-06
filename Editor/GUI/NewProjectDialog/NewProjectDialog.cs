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
    class NewProjectDialog : InputBrowserWindow<ProjectTemplate>
    {
        public static void ShowDialog(Action<ProjectTemplate, String> resultCallback)
        {
            Browser browse = new Browser("Project Templates", "Create Project");
            browse.addNode("", null, new BrowserNode("Empty", new ProjectTemplate()));

            NewProjectDialog projectDialog = new NewProjectDialog();
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
        }

        private EditBox projectLocation;

        protected NewProjectDialog()
            :base("Create Project", "", "Medical.GUI.NewProjectDialog.NewProjectDialog.layout")
        {
            projectLocation = window.findWidget("ProjectLocation") as EditBox;
            projectLocation.Caption = EditorConfig.TimelineProjectDirectory;
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
            using (DirDialog dirDialog = new DirDialog(MainWindow.Instance, "Choose the path to load files from.", projectLocation.Caption))
            {
                if (dirDialog.showModal() == NativeDialogResult.OK)
                {
                    projectLocation.Caption = dirDialog.Path;
                }
            }
        }
    }
}
