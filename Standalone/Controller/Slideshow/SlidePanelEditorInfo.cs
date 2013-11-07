//using Engine;
//using Medical.Controller.AnomalousMvc;
//using Medical.GUI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Medical
//{
//    public class SlidePanelEditorInfo
//    {
//        private RmlWysiwygView view;
//        private RmlWysiwygComponent component;

//        public SlidePanelEditorInfo(Slide slide, RmlSlidePanel panel)
//        {
//            //String editorViewName = panel.createViewName("RmlView");
//            //RawRmlWysiwygView rmlView = new RawRmlWysiwygView(editorViewName, this.uiCallback, this.uiCallback, this.undoBuffer);
//            //rmlView.ViewLocation = panel.ViewLocation;
//            //rmlView.IsWindow = false;
//            //rmlView.EditPreviewContent = true;
//            //rmlView.Size = new IntSize2(panel.Size, panel.Size);
//            //rmlView.WidthSizeStrategy = panel.SizeStrategy;
//            //rmlView.HeightSizeStrategy = panel.SizeStrategy;
//            //rmlView.Rml = panel.Rml;
//            //rmlView.FakePath = slide.UniqueName + "/index.rml";
//            //rmlView.ComponentCreated += (view, component) =>
//            //{
//            //    this.component = component;
//            //    component.RmlEdited += rmlEditor =>
//            //    {
//            //        panel.Rml = rmlEditor.CurrentRml;
//            //    };
//            //    component.ElementDraggedOffDocument += RmlWysiwyg_ElementDraggedOffDocument;
//            //    component.ElementDroppedOffDocument += RmlWysiwyg_ElementDroppedOffDocument;
//            //    component.ElementReturnedToDocument += RmlWysiwyg_ElementReturnedToDocument;
//            //};
//            //rmlView.UndoRedoCallback = (rml) =>
//            //{
//            //    this.wysiwygUndoCallback(editorViewName, rml);
//            //};
//            //rmlView.RequestFocus += (view) =>
//            //{
//            //    currentRmlEditor = view.Name;
//            //};
//            //rmlView.addCustomStrategy(imageStrategy);
//            //rmlView.addCustomStrategy(triggerStrategy);
//            //mvcContext.Views.add(rmlView);
//            //rmlEditors.Add(rmlView.Name, new Pair<RawRmlWysiwygView, RmlWysiwygComponent>(rmlView, null));
//            //showEditorWindowsCommand.addCommand(new ShowViewCommand(rmlView.Name));
//            //closeEditorWindowsCommand.addCommand(new CloseViewIfOpen(rmlView.Name));
//            //if (currentRmlEditor == null)
//            //{
//            //    currentRmlEditor = rmlView.Name;
//            //}
//        }

//        public View View
//        {
//            get
//            {
//                return view;
//            }
//        }
//    }
//}
