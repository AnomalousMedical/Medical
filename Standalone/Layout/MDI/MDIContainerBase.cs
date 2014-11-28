﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    /// <summary>
    /// The base class for items that go into the MDILayoutContainer.
    /// </summary>
    public abstract class MDIContainerBase : LayoutContainer
    {
        private DockLocation currentDockLocation;

        public MDIContainerBase(DockLocation currentLocation)
        {
            currentDockLocation = currentLocation;
            Scale = 100.0f;
        }

        /// <summary>
        /// The scale of this window in its parent container. The scaling can be
        /// any arbitray number, which will be added up by LayoutContainers. So
        /// the final size of the MDI element will be the percentage this number
        /// is of the whole row.
        /// </summary>
        public float Scale { get; set; }

        public abstract IntSize2 ActualSize { get; set; }

        /// <summary>
        /// The container this window is currently inside of.
        /// Do not touch unless you are MDILayoutManager or MDILayoutContainer.
        /// </summary>
        internal MDIChildContainerBase _ParentContainer { get; set; }

        public abstract MDIWindow findWindowAtPosition(float mouseX, float mouseY);

        /// <summary>
        /// Find out if there is a control widget at the given position, these are things like MDISeparators.
        /// </summary>
        /// <param name="x">The X coord</param>
        /// <param name="y">The Y coord</param>
        /// <returns>True if there is a control widget at this position.</returns>
        protected internal abstract bool isControlWidgetAtPosition(int x, int y);

        public DockLocation CurrentDockLocation
        {
            get
            {
                return currentDockLocation;
            }
            internal set
            {
                if (currentDockLocation != value)
                {
                    DockLocation oldLocaiton = currentDockLocation;
                    currentDockLocation = value;
                    onDockLocationChanged(oldLocaiton, currentDockLocation);
                }
            }
        }

        protected virtual void onDockLocationChanged(DockLocation oldLocation, DockLocation newLocation)
        {

        }
    }
}
