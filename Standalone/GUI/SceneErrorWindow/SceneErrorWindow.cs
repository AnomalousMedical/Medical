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

            StringBuilder htmlString = new StringBuilder();
            foreach (BehaviorBlacklistEventArgs blacklist in errorManager.BlacklistEvents)
            {
                if (blacklist.Behavior != null)
                {
                    htmlString.AppendFormat("<p><span class=\"Subsystem\">Behavior</span>&nbsp;<span class=\"SimObject\">{3}</span>&nbsp;<span class=\"Type\">{1}</span>&nbsp;<span class=\"ElementName\">{0}</span>&nbsp;<span class=\"Reason\">{2}</span></p>", blacklist.Behavior.Name, blacklist.Behavior.GetType().Name, blacklist.Message, blacklist.Behavior.Owner != null ? blacklist.Behavior.Owner.Name : "NullOwner");
                }
                else
                {
                    htmlString.AppendFormat("<p>Null Behavior blacklisted.  Reason: {0}<br/></p>", blacklist.Message);
                }
            }
            setBodyMarkup(htmlString.ToString());
        }

        public override void Dispose()
        {
            guiManager.removeManagedDialog(this);
            rocketWidget.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// You can use this to just do body using a default head.
        /// </summary>
        public void setBodyMarkup(String markup)
        {
            rocketWidget.Context.UnloadAllDocuments();

            StringBuilder htmlString = new StringBuilder();

            htmlString.Append(DocumentStart);
            htmlString.Append(markup);
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

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            rocketWidget.resized();
        }

        const String DocumentStart = @"<rml>
	<head>
		<link type=""text/template"" href=""Medical.GUI.SceneErrorWindow.ErrorTemplate.trml"" />
	</head>
	<body template=""ErrorTemplate"">";

        const String DocumentEnd = "</body></rml>";
    }
}
