using Engine;
using MyGUIPlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class SceneStatsDisplay : Component
    {
        private EventLayoutContainer layoutContainer = new EventLayoutContainer();

        private static readonly int YOffset = ScaleHelper.Scaled(50);

        private RenderTarget displayStatsTarget;

        private TextBox fpsDisplay;
        private TextBox trianglesDisplay;
        private TextBox batchesDisplay;

        private float lastFps;
        private uint lastTriangles;
        private uint lastBatches;
        private bool preferVisible = false;

        public SceneStatsDisplay(RenderTarget displayStatsTarget)
            : base("Medical.GUI.SceneStats.SceneStatsDisplay.layout")
        {
            fpsDisplay = (TextBox)widget.findWidget("FPSDisplay");
            trianglesDisplay = (TextBox)widget.findWidget("TrianglesDisplay");
            batchesDisplay = (TextBox)widget.findWidget("BatchesDisplay");

            layoutContainer.LayoutChanged += layoutContainer_LayoutChanged;

            this.displayStatsTarget = displayStatsTarget;
        }

        public override void Dispose()
        {
            displayStatsTarget.PostRenderTargetUpdate -= displayStatsTarget_PostRenderTargetUpdate;
            base.Dispose();
        }

        public bool Visible
        {
            get
            {
                return preferVisible;
            }
            set
            {
                if (preferVisible != value)
                {
                    if (value)
                    {
                        displayStatsTarget.PostRenderTargetUpdate += displayStatsTarget_PostRenderTargetUpdate;
                    }
                    else
                    {
                        displayStatsTarget.PostRenderTargetUpdate -= displayStatsTarget_PostRenderTargetUpdate;
                    }
                    widget.Visible = value;
                    preferVisible = value;
                }
            }
        }

        void displayStatsTarget_PostRenderTargetUpdate()
        {
            float fps = displayStatsTarget.getAverageFPS();
            if (fps != lastFps)
            {
                fpsDisplay.Caption = fps.ToString();
                lastFps = fps;
            }

            uint triangles = displayStatsTarget.getTriangleCount();
            if (triangles != lastTriangles)
            {
                trianglesDisplay.Caption = triangles.ToString();
                lastTriangles = triangles;
            }

            uint batches = displayStatsTarget.getBatchCount();
            if (batches != lastBatches)
            {
                batchesDisplay.Caption = batches.ToString();
                lastBatches = batches;
            }
        }

        void layoutContainer_LayoutChanged(EventLayoutContainer obj)
        {
            if (widget.Height + YOffset < layoutContainer.WorkingSize.Height)
            {
                widget.setCoord(layoutContainer.Location.x, layoutContainer.Location.y + YOffset, layoutContainer.WorkingSize.Width, widget.Height);
                widget.Visible = preferVisible;
            }
            else
            {
                widget.Visible = false;
            }
        }

        public SingleChildLayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }
    }
}
