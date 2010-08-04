using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Platform;

namespace Medical.GUI
{
    public partial class MainForm : Form, OSWindow
    {
        private List<OSWindowListener> listeners = new List<OSWindowListener>();

        public MainForm()
        {
            InitializeComponent();
        }

        public string WindowHandle
        {
            get { return this.Handle.ToString(); }
        }

        public int WindowHeight
        {
            get { return Height; }
        }

        public int WindowWidth
        {
            get { return Width; }
        }

        public void addListener(OSWindowListener listener)
        {
            listeners.Add(listener);
        }

        public void removeListener(OSWindowListener listener)
        {
            listeners.Remove(listener);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            foreach (OSWindowListener listener in listeners)
            {
                listener.closing(this);
            }
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            foreach (OSWindowListener listener in listeners)
            {
                listener.moved(this);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (OSWindowListener listener in listeners)
            {
                listener.resized(this);
            }
        }
    }
}
