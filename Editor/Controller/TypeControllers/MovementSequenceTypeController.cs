using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Xml;
using Medical.Muscles;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    class MovementSequenceTypeController : SaveableTypeController<MovementSequence>
    {
        private EditorController editorController;
        private MovementSequenceEditorContext editorContext;
        private const String Icon = "MovementSequenceEditorIcon";

        public MovementSequenceTypeController(EditorController editorController)
            :base(".seq", editorController)
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
        }

        public override void openFile(string file)
        {
            try
            {
                MovementSequence movementSequence = (MovementSequence)loadObject(file);

                editorContext = new MovementSequenceEditorContext(movementSequence, file, this);
                editorContext.Focus += new Action<MovementSequenceEditorContext>(editorContext_Focus);
                editorContext.Blur += new Action<MovementSequenceEditorContext>(editorContext_Blur);
                editorController.runEditorContext(editorContext.MvcContext);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error loading movement sequence {0}.\nReason: {1}", file, ex.Message), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        public void saveFile(MovementSequence movementSequence, String file)
        {
            saveObject(file, movementSequence);
            editorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Movement Sequence", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Movement Sequence Name", "Enter a name for the movement sequence.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".seq");
                    if (editorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewMovementSequence(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewMovementSequence(filePath);
                    }
                    return true;
                });
            }));
        }

        void createNewMovementSequence(String filePath)
        {
            MovementSequence movementSequence = new MovementSequence();
            movementSequence.Duration = 5.0f;
            creatingNewFile(filePath);
            saveObject(filePath, movementSequence);
            openFile(filePath);
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            close();
        }

        private void close()
        {
            if (editorContext != null)
            {
                editorContext.close();
            }
            closeCurrentCachedResource();
        }

        void editorContext_Focus(MovementSequenceEditorContext obj)
        {
            editorContext = obj;
        }

        void editorContext_Blur(MovementSequenceEditorContext obj)
        {
            if (editorContext == obj)
            {
                editorContext = null;
            }
        }
    }
}
