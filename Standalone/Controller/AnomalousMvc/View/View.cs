using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using Engine;

namespace Medical.Controller.AnomalousMvc
{
    public enum ViewSizeStrategy
    {
        Auto = 0,
        Percentage = 1,
        Fixed = 2,
    }

    public enum ViewSizeLimitStrategy
    {
        None = 0,
        Percentage = 1,
        Pixels = 2
    }

    public abstract class View : SaveableEditableItem
    {
        [DoNotSave]
        private bool editPreviewContent = false;

        public View(String name)
            :base(name)
        {
            ViewLocation = ViewLocations.Left;
            IsWindow = false;
            Transparent = false;
            FillScreen = false;
            WidthSizeStrategy = ViewSizeStrategy.Auto;
            HeightSizeStrategy = ViewSizeStrategy.Auto;
            Size = new IntSize2();
            WidthSizeLimitStrategy = ViewSizeLimitStrategy.None;
            HeightSizeLimitStrategy = ViewSizeLimitStrategy.None;
            SizeLimit = new IntSize2();
        }

        [Editable]
        public ViewLocations ViewLocation { get; set; }

        [Editable]
        public bool IsWindow { get; set; }

        [Editable]
        public bool Transparent { get; set; }

        [Editable]
        public bool FillScreen { get; set; }

        [EditableAction]
        public String OpeningAction { get; set; }

        [EditableAction]
        public String ClosingAction { get; set; }

        [Editable]
        public ViewSizeStrategy WidthSizeStrategy { get; set; }

        [Editable]
        public ViewSizeStrategy HeightSizeStrategy { get; set; }

        [Editable]
        public IntSize2 Size { get; set; }

        [Editable]
        public ViewSizeLimitStrategy WidthSizeLimitStrategy { get; set; }

        [Editable]
        public ViewSizeLimitStrategy HeightSizeLimitStrategy { get; set; }

        [Editable]
        public IntSize2 SizeLimit { get; set; }

        public bool EditPreviewContent
        {
            get
            {
                return editPreviewContent;
            }
            set
            {
                editPreviewContent = value;
            }
        }

        protected View(LoadInfo info)
            :base (info)
        {

        }

        public enum CustomQueries
        {
            AddControllerForView
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.IconReferenceTag = "MvcContextEditor/IndividualViewIcon";
            editInterface.addCommand(new EditInterfaceCommand("Create Controller", (uiCallback, caller) =>
            {
                uiCallback.runOneWayCustomQuery(CustomQueries.AddControllerForView, this);
            }));
            base.customizeEditInterface(editInterface);
        }

        /// <summary>
        /// Computes a width based off the current width, working area width and current width strategy.
        /// </summary>
        /// <param name="currentWidth">The current size of the item to compute width for.</param>
        /// <param name="workingAreaWidth">The width of the area that the item will go into, used for percentage calculations.</param>
        /// <returns>The width appropriate for this view's settings</returns>
        public int computeWidth(int currentWidth, int workingAreaWidth)
        {
            if (ViewLocation == ViewLocations.Top || ViewLocation == ViewLocations.Bottom)
            {
                return currentWidth;
            }
            return computeSize(currentWidth, Size.Width, workingAreaWidth, WidthSizeStrategy, WidthSizeLimitStrategy);
        }

        /// <summary>
        /// Computes a height based off the current height, working area height and current height strategy.
        /// </summary>
        /// <param name="currentWidth">The current size of the item to compute height for.</param>
        /// <param name="workingAreaWidth">The height of the area that the item will go into, used for percentage calculations.</param>
        /// <returns>The height appropriate for this view's settings</returns>
        public int computeHeight(int currentHeight, int workingAreaHeight)
        {
            if (ViewLocation == ViewLocations.Left || ViewLocation == ViewLocations.Right)
            {
                return currentHeight;
            }
            return computeSize(currentHeight, Size.Height, workingAreaHeight, HeightSizeStrategy, HeightSizeLimitStrategy);
        }

        private static int computeSize(int componentSize, int userSize, int workingSize, ViewSizeStrategy sizeStrategy, ViewSizeLimitStrategy sizeLimitStrategy)
        {
            int size;
            switch (sizeStrategy)
            {
                case ViewSizeStrategy.Auto:
                    size = componentSize;
                    break;
                case ViewSizeStrategy.Percentage:
                    size = (int)(userSize * 0.01f * workingSize);
                    break;
                case ViewSizeStrategy.Fixed:
                    size = userSize;
                    break;
                default:
                    throw new Exception("Unsupported size strategy");
            }

            switch (sizeLimitStrategy)
            {
                case ViewSizeLimitStrategy.Percentage:
                    int percentageSize = (int)(userSize * 0.01f * workingSize);
                    if (size > percentageSize)
                    {
                        size = percentageSize;
                    }
                    break;
                case ViewSizeLimitStrategy.Pixels:
                    if (size > userSize)
                    {
                        size = userSize;
                    }
                    break;
            }

            return size;
        }
    }
}
