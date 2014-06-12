using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class SceneErrorWindow : MDIDialog
    {
        ImageBox rmlImage;
        RocketWidget rocketWidget;
        GUIManager guiManager;

        public SceneErrorWindow(GUIManager guiManager, BehaviorErrorManager errorManager)
            : base("Medical.GUI.SceneErrorWindow.SceneErrorWindow.layout")
        {
            rmlImage = (ImageBox)window.findWidget("RmlImage");
            rocketWidget = new RocketWidget(rmlImage, false);

            window.WindowChangedCoord += window_WindowChangedCoord;
            this.guiManager = guiManager;
            guiManager.addManagedDialog(this);
            guiManager.autoDisposeDialog(this);

            rocketWidget.Context.UnloadAllDocuments();

            StringBuilder htmlString = new StringBuilder();

            htmlString.Append(DocumentStart);

            foreach (BehaviorBlacklistEventArgs error in errorManager.BlacklistEvents)
            {
                if (error.Behavior != null)
                {
                    addLine(htmlString, error);
                }
                else
                {
                    htmlString.AppendFormat("<p>Null Behavior blacklisted.  Reason: {0}<br/></p>", error.Message);
                }
            }

            htmlString.Append(DocumentEnd);

            var resourceLoader = new RocketAssemblyResourceLoader(this.GetType().Assembly);
            RocketInterface.Instance.FileInterface.addExtension(resourceLoader);
            using (ElementDocument document = rocketWidget.Context.LoadDocumentFromMemory(htmlString.ToString()))
            {
                if (document != null)
                {
                    document.Show();
                    rocketWidget.removeFocus();
                    rocketWidget.renderOnNextFrame();
                }
            }
            RocketInterface.Instance.FileInterface.removeExtension(resourceLoader);
        }

        public override void Dispose()
        {
            guiManager.removeManagedDialog(this);
            rocketWidget.Dispose();
            base.Dispose();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            rocketWidget.resized();
        }

        void addLine(StringBuilder sb, BehaviorBlacklistEventArgs error)
        {
            sb.AppendFormat(ErrorLine, error.Behavior.Name, error.Behavior.GetType().Name, error.Message, error.Behavior.Owner != null ? error.Behavior.Owner.Name : "NullOwner");
        }

        const String DocumentStart = @"<rml>
	<head>
		<link type=""text/template"" href=""Medical.GUI.SceneErrorWindow.ErrorTemplate.trml"" />
	</head>
	<body template=""ErrorTemplate"">";

        const String DocumentEnd = "</body></rml>";

        const String ErrorLine = "<p><span class=\"Subsystem\">Behavior</span>&nbsp;<span class=\"SimObject\">{3}</span>&nbsp;<span class=\"Type\">{1}</span>&nbsp;<span class=\"ElementName\">{0}</span>&nbsp;<span class=\"Reason\">{2}</span></p>";
    }
}
