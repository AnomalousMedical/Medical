using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;
using Engine;
using libRocketPlugin;
using Medical;
using Medical.GUI;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using Medical.SlideshowActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture.GUI
{
    class SlideImageStrategy : ElementStrategy
    {
        private SlideImageComponent slideImageEditor;
        private EditInterfaceEditor appearanceEditor;
        private ImageElementStyle appearance;
        private EditorResourceProvider editorResourceProvider;
        private String subdirectory;
        private Slide slide;
        private UndoRedoBuffer undoBuffer;

        public SlideImageStrategy(Slide slide, UndoRedoBuffer undoBuffer, EditorResourceProvider editorResourceProvider, String subdirectory, String previewIconName = "Editor/ImageIcon")
            : base("img", previewIconName, true)
        {
            this.editorResourceProvider = editorResourceProvider;
            this.subdirectory = subdirectory;
            this.slide = slide;
            this.undoBuffer = undoBuffer;
            ResizeHandles = ResizeType.Width | ResizeType.Left | ResizeType.Top;
        }

        public override RmlElementEditor openEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            bool hasFullscreen = element.HasAttribute("onclick");

            appearance = new ImageElementStyle(element);
            appearance.Changed += appearance_Changed;
            appearanceEditor = new EditInterfaceEditor("Appearance", appearance.getEditInterface(), uiCallback);
            slideImageEditor = new SlideImageComponent(editorResourceProvider, subdirectory, element.GetAttributeString("src"), hasFullscreen);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(slideImageEditor);
            editor.addElementEditor(appearanceEditor);
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component, out TwoWayCommand additionalUndoOperations)
        {
            element.ClearLocalStyles();
            StringBuilder styleString = new StringBuilder();
            StringBuilder classString = new StringBuilder();
            bool changesMade = false;
            element.SetAttribute("src", slideImageEditor.ImageName);
            if (slideImageEditor.ShowFullscreen)
            {
                String actionName = element.GetAttributeString("onclick");
                if (actionName == null)
                {
                    actionName = Guid.NewGuid().ToString();
                    element.SetAttribute("onclick", actionName);
                }

                Action<ShowPopupImageAction> undoAction;
                ShowPopupImageAction oldAction = slide.getAction(actionName) as ShowPopupImageAction;
                if (oldAction == null)
                {
                    undoAction = a =>
                    {
                        slide.removeAction(actionName);
                    };
                }
                else
                {
                    undoAction = a =>
                    {
                        slide.replaceAction(a);
                    };
                }

                var action = new ShowPopupImageAction(actionName)
                {
                    ImageName = slideImageEditor.ImageName,
                };

                additionalUndoOperations = new TwoWayDelegateCommand<ShowPopupImageAction, ShowPopupImageAction>(action, oldAction, new TwoWayDelegateCommand<ShowPopupImageAction, ShowPopupImageAction>.Funcs()
                {
                    ExecuteFunc = (exec) =>
                    {
                        slide.replaceAction(exec);
                    },
                    UndoFunc = undoAction,
                });

                additionalUndoOperations.execute(); //This is not called automatically by the classes consuming this, so we make sure to actually apply the changes
            }
            else
            {
                String actionName = element.GetAttributeString("onclick");
                ShowPopupImageAction oldAction = null;
                if (actionName != null)
                {
                    oldAction = slide.getAction(actionName) as ShowPopupImageAction;
                }

                if(oldAction != null)
                {
                    additionalUndoOperations = new TwoWayDelegateCommand<ShowPopupImageAction, ShowPopupImageAction>(null, oldAction, new TwoWayDelegateCommand<ShowPopupImageAction, ShowPopupImageAction>.Funcs()
                    {
                        ExecuteFunc = (exec) =>
                        {
                            slide.removeAction(actionName);
                        },
                        UndoFunc = (undo) =>
                        {
                            slide.replaceAction(undo);
                        }
                    });
                    additionalUndoOperations.execute(); //Make sure changes are applied
                }
                else
                {
                    additionalUndoOperations = null;
                }

                element.RemoveAttribute("onclick");
            }
            changesMade = appearance.buildClassList(classString) | changesMade;
            changesMade = appearance.buildStyleAttribute(styleString) | changesMade;
            if (changesMade)
            {
                if (classString.Length > 0)
                {
                    element.SetAttribute("class", classString.ToString());
                }
                else
                {
                    element.RemoveAttribute("class");
                }
                if (styleString.Length > 0)
                {
                    element.SetAttribute("style", styleString.ToString());
                }
                else
                {
                    element.RemoveAttribute("style");
                }
            }

            return changesMade;
        }

        public override void changeSizePreview(Element element, IntRect newRect, ResizeType resizeType, IntSize2 bounds)
        {
            appearance.changeSize(element, newRect, resizeType, bounds);
            element.ClearLocalStyles();
            StringBuilder styleString = new StringBuilder();
            StringBuilder classString = new StringBuilder();
            appearance.buildClassList(classString);
            appearance.buildStyleAttribute(styleString);
            if (classString.Length > 0)
            {
                element.SetAttribute("class", classString.ToString());
            }
            else
            {
                element.RemoveAttribute("class");
            }
            if (styleString.Length > 0)
            {
                element.SetAttribute("style", styleString.ToString());
            }
            else
            {
                element.RemoveAttribute("style");
            }
        }

        public override Rect getStartingRect(Element selectedElement, out bool leftAnchor)
        {
            return appearance.createCurrentRect(selectedElement, out leftAnchor);
        }

        public override void applySizeChange(Element element)
        {
            appearanceEditor.alertChangesMade();
        }

        public override bool delete(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String src = element.GetAttributeString("src");
            if (String.IsNullOrEmpty(src))
            {
                component.deleteElement(element);
                return true;
            }
            return false;
        }

        void appearance_Changed(StyleDefinition obj)
        {
            appearanceEditor.alertChangesMade();
        }
    }
}
