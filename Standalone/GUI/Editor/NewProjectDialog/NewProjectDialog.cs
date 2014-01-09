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
        public static NewProjectDialog ShowDialog(Browser browse, Action<NewProjectDialog> resultCallback)
        {
            NewProjectDialog projectDialog = new NewProjectDialog(browse.Prompt);
            projectDialog.setBrowser(browse);
            projectDialog.Closing += (sender, e) =>
            {
                if (projectDialog.Accepted)
                {
                    resultCallback(projectDialog);
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
        private CheckButton singleFile;

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

            singleFile = new CheckButton((Button)window.findWidget("SingleFile"));
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

        public bool MakeSingleFile
        {
            get
            {
                return singleFile.Checked;
            }
            set
            {
                singleFile.Checked = value;
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
