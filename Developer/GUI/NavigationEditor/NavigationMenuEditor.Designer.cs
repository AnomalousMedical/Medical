namespace Medical.GUI
{
    partial class NavigationMenuEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuTree = new System.Windows.Forms.TreeView();
            this.emptySpaceMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addParentMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.layerStateText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textText = new System.Windows.Forms.TextBox();
            this.thumbnailPanel = new System.Windows.Forms.Panel();
            this.updateButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.renderButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.nodeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addStatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSubEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.navigationStateTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.emptySpaceMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.nodeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuTree
            // 
            this.menuTree.ContextMenuStrip = this.emptySpaceMenu;
            this.menuTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuTree.HideSelection = false;
            this.menuTree.Location = new System.Drawing.Point(0, 0);
            this.menuTree.Name = "menuTree";
            this.menuTree.Size = new System.Drawing.Size(284, 248);
            this.menuTree.TabIndex = 0;
            // 
            // emptySpaceMenu
            // 
            this.emptySpaceMenu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.emptySpaceMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addParentMenuToolStripMenuItem});
            this.emptySpaceMenu.Name = "emptySpaceMenu";
            this.emptySpaceMenu.Size = new System.Drawing.Size(168, 26);
            // 
            // addParentMenuToolStripMenuItem
            // 
            this.addParentMenuToolStripMenuItem.Name = "addParentMenuToolStripMenuItem";
            this.addParentMenuToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.addParentMenuToolStripMenuItem.Text = "Add Parent Menu";
            this.addParentMenuToolStripMenuItem.Click += new System.EventHandler(this.addParentMenuToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.navigationStateTextBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.layerStateText);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textText);
            this.panel1.Controls.Add(this.thumbnailPanel);
            this.panel1.Controls.Add(this.updateButton);
            this.panel1.Controls.Add(this.importButton);
            this.panel1.Controls.Add(this.renderButton);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 248);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 311);
            this.panel1.TabIndex = 1;
            // 
            // layerStateText
            // 
            this.layerStateText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.layerStateText.Location = new System.Drawing.Point(5, 133);
            this.layerStateText.Name = "layerStateText";
            this.layerStateText.Size = new System.Drawing.Size(275, 20);
            this.layerStateText.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Layer State";
            // 
            // textText
            // 
            this.textText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textText.Location = new System.Drawing.Point(6, 49);
            this.textText.Name = "textText";
            this.textText.Size = new System.Drawing.Size(275, 20);
            this.textText.TabIndex = 1;
            // 
            // thumbnailPanel
            // 
            this.thumbnailPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.thumbnailPanel.Location = new System.Drawing.Point(10, 176);
            this.thumbnailPanel.Name = "thumbnailPanel";
            this.thumbnailPanel.Size = new System.Drawing.Size(262, 92);
            this.thumbnailPanel.TabIndex = 18;
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(0, 6);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 17;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(92, 274);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(75, 23);
            this.importButton.TabIndex = 16;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // renderButton
            // 
            this.renderButton.Location = new System.Drawing.Point(10, 274);
            this.renderButton.Name = "renderButton";
            this.renderButton.Size = new System.Drawing.Size(75, 23);
            this.renderButton.TabIndex = 15;
            this.renderButton.Text = "Render";
            this.renderButton.UseVisualStyleBackColor = true;
            this.renderButton.Click += new System.EventHandler(this.renderButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 160);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Thumbnail";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Text";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "png";
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "PNG Files|*.png";
            // 
            // nodeMenu
            // 
            this.nodeMenu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.nodeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addStatesToolStripMenuItem,
            this.addSubEntryToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.nodeMenu.Name = "nodeMenu";
            this.nodeMenu.Size = new System.Drawing.Size(150, 70);
            // 
            // addStatesToolStripMenuItem
            // 
            this.addStatesToolStripMenuItem.Name = "addStatesToolStripMenuItem";
            this.addStatesToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.addStatesToolStripMenuItem.Text = "Add States";
            this.addStatesToolStripMenuItem.Click += new System.EventHandler(this.addStatesToolStripMenuItem_Click);
            // 
            // addSubEntryToolStripMenuItem
            // 
            this.addSubEntryToolStripMenuItem.Name = "addSubEntryToolStripMenuItem";
            this.addSubEntryToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.addSubEntryToolStripMenuItem.Text = "Add Sub Entry";
            this.addSubEntryToolStripMenuItem.Click += new System.EventHandler(this.addSubEntryToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // navigationStateTextBox
            // 
            this.navigationStateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.navigationStateTextBox.Location = new System.Drawing.Point(6, 92);
            this.navigationStateTextBox.Name = "navigationStateTextBox";
            this.navigationStateTextBox.Size = new System.Drawing.Size(275, 20);
            this.navigationStateTextBox.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Navigation State";
            // 
            // NavigationMenuEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonText = "Navigation Menu Editor";
            this.Controls.Add(this.menuTree);
            this.Controls.Add(this.panel1);
            this.DockAreas = ((Medical.GUI.DockLocations)((Medical.GUI.DockLocations.Right | Medical.GUI.DockLocations.Float)));
            this.Name = "NavigationMenuEditor";
            this.ShowHint = Medical.GUI.DockLocations.Right;
            this.Size = new System.Drawing.Size(284, 559);
            this.ToolStripName = "Editing";
            this.emptySpaceMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.nodeMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView menuTree;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button renderButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel thumbnailPanel;
        private System.Windows.Forms.ContextMenuStrip emptySpaceMenu;
        private System.Windows.Forms.ToolStripMenuItem addParentMenuToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip nodeMenu;
        private System.Windows.Forms.ToolStripMenuItem addStatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSubEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.TextBox layerStateText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox navigationStateTextBox;
        private System.Windows.Forms.Label label4;
    }
}