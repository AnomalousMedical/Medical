using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine.Platform;

namespace Medical.GUI
{
    public class EditInterfaceTreeView : IDisposable
    {
        /// <summary>
        /// The delegate for EditInterfaceViews.
        /// </summary>
        /// <param name="evt">The EditInterfaceViewEvent.</param>
        public delegate void EditInterfaceTreeViewEvent(EditInterfaceViewEvent evt);

        /// <summary>
        /// Called when the selected EditInterface has changed. Cannot be canceled.
        /// </summary>
        public event EditInterfaceTreeViewEvent EditInterfaceSelectionChanged;

        /// <summary>
        /// Called when the selected EditInterface is about to change. Can be canceled.
        /// </summary>
        public event EditInterfaceTreeViewEvent EditInterfaceSelectionChanging;

        /// <summary>
        /// Called when the interface has requested to further edit an object.
        /// Can be ignored if not applicable.
        /// </summary>
        public event EditInterfaceTreeViewEvent EditInterfaceSelectionEdit;

        /// <summary>
        /// Called when an EditInterface has been chosen in some way.
        /// </summary>
        public event EditInterfaceTreeViewEvent EditInterfaceChosen;

        /// <summary>
        /// Called when an EditInterface is added.
        /// </summary>
        public event EditInterfaceTreeViewEvent EditInterfaceAdded;

        /// <summary>
        /// Called when an EditInterface is removed.
        /// </summary>
        public event EditInterfaceTreeViewEvent EditInterfaceRemoved;

        private Tree tree;
        private EditInterface mainEditInterface;
        private EditInterfaceTreeNode parentNode;

        private EditInterface currentMenuInterface;
        private EditUICallback editUICallback;

        public EditInterfaceTreeView(Tree tree, EditUICallback editUICallback)
        {
            this.tree = tree;
            this.editUICallback = editUICallback;
            tree.AfterSelect += new EventHandler<TreeEventArgs>(tree_AfterSelect);
            tree.BeforeSelect += new EventHandler<TreeCancelEventArgs>(tree_BeforeSelect);
            tree.NodeMouseReleased += new EventHandler<TreeMouseEventArgs>(tree_NodeMouseReleased);
            tree.NodeMouseDoubleClick += new EventHandler<TreeEventArgs>(tree_NodeMouseDoubleClick);
        }

        public void Dispose()
        {
            tree.AfterSelect -= tree_AfterSelect;
            tree.BeforeSelect -= tree_BeforeSelect;
            tree.NodeMouseReleased -= tree_NodeMouseReleased;
            tree.NodeMouseDoubleClick -= tree_NodeMouseDoubleClick;

            if (parentNode != null)
            {
                parentNode.Dispose();
            }
        }

        public EditInterface EditInterface
        {
            get
            {
                return mainEditInterface;
            }
            set
            {
                if (parentNode != null)
                {
                    tree.Nodes.remove(parentNode);
                    parentNode.Dispose();
                    parentNode = null;
                }
                mainEditInterface = value;
                if (mainEditInterface != null)
                {
                    parentNode = new EditInterfaceTreeNode(mainEditInterface, this);
                    tree.Nodes.add(parentNode);
                    tree.SelectedNode = parentNode;
                    parentNode.Expanded = true;
                }
            }
        }

        public void expandAll()
        {
            tree.expandAll();
        }

        public void nodeAdded(EditInterfaceTreeNode editInterfaceTreeNode)
        {
            editInterfaceTreeNode.Expanded = true;
            if (EditInterfaceAdded != null)
            {
                EditInterfaceAdded.Invoke(new EditInterfaceViewEvent(editInterfaceTreeNode.EditInterface));
            }
            tree.layout();
        }

        public void nodeRemoved(EditInterfaceTreeNode editInterfaceTreeNode)
        {
            if (EditInterfaceRemoved != null)
            {
                EditInterfaceRemoved.Invoke(new EditInterfaceViewEvent(editInterfaceTreeNode.EditInterface));
            }
            tree.layout();
        }

        void tree_NodeMouseDoubleClick(object sender, TreeEventArgs e)
        {
            if (EditInterfaceSelectionEdit != null)
            {
                EditInterfaceViewEvent evt = new EditInterfaceViewEvent((e.Node as EditInterfaceTreeNode).EditInterface);
                EditInterfaceSelectionEdit.Invoke(evt);
            }
        }

        void tree_NodeMouseReleased(object sender, TreeMouseEventArgs e)
        {
            if (e.Button == MouseButtonCode.MB_BUTTON1)
            {
                EditInterfaceTreeNode node = e.Node as EditInterfaceTreeNode;
                tree.SelectedNode = e.Node;
                currentMenuInterface = node.EditInterface;
                if (currentMenuInterface.hasCommands())
                {
                    PopupMenu menu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
                    menu.ItemAccept += new MyGUIEvent(menu_ItemAccept);
                    menu.Closed += new MyGUIEvent(menu_Closed);
                    menu.Visible = false;
                    foreach (EditInterfaceCommand command in currentMenuInterface.getCommands())
                    {
                        MenuItem item = menu.addItem(command.Name);
                        item.UserObject = command;
                    }
                    LayerManager.Instance.upLayerItem(menu);
                    menu.setPosition(e.MousePosition.x, e.MousePosition.y);
                    menu.ensureVisible();
                    menu.setVisibleSmooth(true);
                }
            }
        }

        void menu_Closed(Widget source, EventArgs e)
        {
            Gui.Instance.destroyWidget(source);
        }

        void menu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs ae = (MenuCtrlAcceptEventArgs)e;
            ((EditInterfaceCommand)ae.Item.UserObject).execute(editUICallback);
        }

        void tree_BeforeSelect(object sender, TreeCancelEventArgs e)
        {
            if (EditInterfaceSelectionChanging != null && e.Node != null)
            {
                EditInterfaceViewEvent evt = new EditInterfaceViewEvent((e.Node as EditInterfaceTreeNode).EditInterface);
                EditInterfaceSelectionChanging.Invoke(evt);
                e.Cancel = evt.Cancel;
            }
        }

        void tree_AfterSelect(object sender, TreeEventArgs e)
        {
            if (EditInterfaceSelectionChanged != null)
            {
                EditInterfaceViewEvent evt = new EditInterfaceViewEvent((e.Node as EditInterfaceTreeNode).EditInterface);
                EditInterfaceSelectionChanged.Invoke(evt);
            }
        }

        /// <summary>
        /// Validate all EditInterfaces in this view. If one has an error it
        /// will be highlighted and an errorMessage will be returned.
        /// </summary>
        /// <param name="errorMessage">An error message on an error.</param>
        /// <returns>True if all interfaces are valid.</returns>
        public bool validateAllInterfaces(out String errorMessage)
        {
            errorMessage = "";
            bool selectedOk = true;
            //Start with the selected node. This way if it has an error the change will be less jarring.
            if (tree.SelectedNode != null)
            {
                EditInterface currentInterface = ((EditInterfaceTreeNode)tree.SelectedNode).EditInterface;
                selectedOk = currentInterface.validate(out errorMessage);
            }
            if (selectedOk)
            {
                return scanForErrors(out errorMessage, tree.Nodes);
            }
            //There is an error with the selected node.
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Recursive helper function to scan the whole tree.
        /// </summary>
        /// <param name="errorMessage">An error message on an error.</param>
        /// <returns>True if all interfaces are valid.</returns>
        private bool scanForErrors(out String errorMessage, TreeNodeCollection parent)
        {
            foreach (EditInterfaceTreeNode node in parent)
            {
                if (!node.EditInterface.validate(out errorMessage))
                {
                    tree.SelectedNode = node;
                    return false;
                }
                if (!scanForErrors(out errorMessage, node.Children))
                {
                    return false;
                }
            }
            errorMessage = null;
            return true;
        }
    }
}
