﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller;

namespace Medical.GUI.AnomalousMvc
{
    class MDIDialogDecorator : MDIDialog, ViewHostComponent, ButtonFactory, IDisposable
    {
        private ViewHostComponent child;

        private String closeAction = null;
        private bool fireCloseEvent;

        public MDIDialogDecorator(MDILayoutManager targetLayoutManager, ViewHostComponent child, ButtonCollection buttons)
            : base(buttons.hasItem("Close") ? "Medical.GUI.AnomalousMvc.Decorators.MDIDialogDecoratorCSX.layout" : "Medical.GUI.AnomalousMvc.Decorators.MDIDialogDecoratorCS.layout")
        {
            this.MDIManager = targetLayoutManager;

            if (buttons.Count > 0)
            {
                //Keep button decorator from being made if there is only one button and it is close
                if (buttons.Count == 1 && buttons.hasItem("Close"))
                {
                    buttons["Close"].createButton(this, 0, 0, 0, 0);
                }
                else
                {
                    child = new ButtonDecorator(child, buttons, this);
                }
            }

            fireCloseEvent = closeAction != null;

            window.Visible = true;

            IntCoord clientCoord = window.ClientCoord;
            int widthDifference = window.Width - clientCoord.width;
            int heightDifference = window.Height - clientCoord.height;

            Position = new Vector2(child.Widget.Left, child.Widget.Top);
            Size = new IntSize2(child.Widget.Width + widthDifference, child.Widget.Height + heightDifference);
            dockedSize = Size;

            clientCoord = window.ClientCoord;
            this.child = child;
            child.Widget.attachToWidget(window);
            child.Widget.setCoord(0, 0, clientCoord.width, clientCoord.height);
            child.Widget.Align = Align.Stretch;

            Resized += new EventHandler(MDIDialogDecorator_Resized);
            Closed += new EventHandler(MDIDialogDecorator_Closed);

            child.topLevelResized();
            window.Visible = false;
        }

        public override void Dispose()
        {
            child.Dispose();
        }

        public void opening()
        {
            this.Visible = true;
            child.opening();
        }

        public void closing()
        {
            fireCloseEvent = false;
            child.closing();
            this.Visible = false;
        }

        public void topLevelResized()
        {
            child.topLevelResized();
        }

        public MyGUIViewHost ViewHost
        {
            get
            {
                return child.ViewHost;
            }
        }

        public Widget Widget
        {
            get
            {
                return window;
            }
        }

        public bool _RequestClosed { get; set; }

        public void _animationCallback(LayoutContainer oldChild)
        {
            Dispose();
        }

        public void addTextButton(ButtonDefinition buttonDefinition, int x, int y, int width, int height)
        {
            ((ButtonDecorator)child).addTextButton(buttonDefinition, x, y, width, height);
        }

        public void addCloseButton(CloseButtonDefinition buttonDefinition, int x, int y, int width, int height)
        {
            closeAction = buttonDefinition.Action;
        }

        void MDIDialogDecorator_Resized(object sender, EventArgs e)
        {
            child.topLevelResized();
        }

        void MDIDialogDecorator_Closed(object sender, EventArgs e)
        {
            if (fireCloseEvent)
            {
                ViewHost.Context.runAction(closeAction, ViewHost);
                ((DialogCancelEventArgs)e).Cancel = true;
            }
        }
    }
}