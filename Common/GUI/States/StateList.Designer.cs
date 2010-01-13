namespace Medical.GUI
{
    partial class StateList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stateListBox = new ComponentFactory.Krypton.Toolkit.KryptonListBox();
            this.contextMenu = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.deleteCommand = new ComponentFactory.Krypton.Toolkit.KryptonCommand();
            this.SuspendLayout();
            // 
            // stateListBox
            // 
            this.stateListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateListBox.Location = new System.Drawing.Point(0, 0);
            this.stateListBox.Name = "stateListBox";
            this.stateListBox.Size = new System.Drawing.Size(200, 466);
            this.stateListBox.TabIndex = 0;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItems1});
            // 
            // kryptonContextMenuItems1
            // 
            this.kryptonContextMenuItems1.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItem1});
            // 
            // kryptonContextMenuItem1
            // 
            this.kryptonContextMenuItem1.KryptonCommand = this.deleteCommand;
            this.kryptonContextMenuItem1.Text = "Delete";
            // 
            // deleteCommand
            // 
            this.deleteCommand.Text = "Delete";
            this.deleteCommand.TextLine1 = "Delete";
            // 
            // StateList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.stateListBox);
            this.MinimumSize = new System.Drawing.Size(200, 466);
            this.Name = "StateList";
            this.Size = new System.Drawing.Size(200, 466);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonListBox stateListBox;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu contextMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem1;
        private ComponentFactory.Krypton.Toolkit.KryptonCommand deleteCommand;

    }
}
