using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Platform;
using Medical;
using System.IO;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    public class MedicalForm : KryptonForm, OSWindow
    {
        private List<OSWindowListener> listeners = new List<OSWindowListener>();
        protected FileTracker fileTracker = new FileTracker("*.sim.xml|*.sim.xml");
        private String windowDefaultText = null;
        private const String TITLE_FORMAT = "{0} - {1}";

        public MedicalForm()
        {

        }

        #region OSWindow Members

        public String WindowHandle
        {
            get
            {
                return this.Handle.ToString();
            }
        }

        public int WindowWidth
        {
            get
            {
                return this.Width;
            }
        }

        public int WindowHeight
        {
            get
            {
                return this.Height;
            }
        }

        public void addListener(OSWindowListener listener)
        {
            listeners.Add(listener);
        }

        public void removeListener(OSWindowListener listener)
        {
            listeners.Remove(listener);
        }

        #endregion

        protected override void OnResize(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.resized(this);
            }
            base.OnResize(e);
        }

        protected override void OnMove(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.moved(this);
            }
            base.OnMove(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.closing(this);
            }
            base.OnHandleDestroyed(e);
        }
        
        /// <summary>
        /// Update the title of the window to reflect a current filename or other info.
        /// </summary>
        /// <param name="subName">A name to place as a secondary name in the title.</param>
        protected void updateWindowTitle(String subName)
        {
            if (windowDefaultText == null)
            {
                windowDefaultText = this.Text;
            }
            Text = String.Format(TITLE_FORMAT, windowDefaultText, subName);
        }

        /// <summary>
        /// Clear the window title back to the default text.
        /// </summary>
        protected void clearWindowTitle()
        {
            if (windowDefaultText != null)
            {
                Text = windowDefaultText;
            }
        }
    }
}
