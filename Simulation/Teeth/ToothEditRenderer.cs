using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Renderer;

namespace Medical
{
    class ToothEditRenderer : EditInterfaceRenderer
    {
        private List<EditInterfaceRenderer> subRenderers = new List<EditInterfaceRenderer>();

        public void addSubRenderer(EditInterfaceRenderer renderer)
        {
            subRenderers.Add(renderer);
        }

        public void removeSubRenderer(EditInterfaceRenderer renderer)
        {
            subRenderers.Remove(renderer);
        }

        public void frameUpdate(DebugDrawingSurface drawingSurface)
        {

        }

        public void interfaceDeselected(DebugDrawingSurface drawingSurface)
        {

        }

        public void interfaceSelected(DebugDrawingSurface drawingSurface)
        {
            foreach (EditInterfaceRenderer renderer in subRenderers)
            {
                renderer.interfaceSelected(drawingSurface);
            }
        }

        public void propertiesChanged(DebugDrawingSurface drawingSurface)
        {

        }
    }
}
