namespace Medical.GUI
{
    partial class NavigationStateSelector
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
            this.navigationStateView = new System.Windows.Forms.ListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.hiddenCheck = new System.Windows.Forms.CheckBox();
            this.nameText = new System.Windows.Forms.TextBox();
            this.translationText = new System.Windows.Forms.TextBox();
            this.lookAtText = new System.Windows.Forms.TextBox();
            this.stateUpdate = new System.Windows.Forms.Button();
            this.gotoButton = new System.Windows.Forms.Button();
            this.linkView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.radiusUpDown = new System.Windows.Forms.NumericUpDown();
            this.linkUpdate = new System.Windows.Forms.Button();
            this.useCurrentButton = new System.Windows.Forms.Button();
            this.showNavigationCheck = new System.Windows.Forms.CheckBox();
            this.multipleStateMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTwoWayLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createStateMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singleStateMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.destroyStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destroySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.radiusUpDown)).BeginInit();
            this.multipleStateMenu.SuspendLayout();
            this.createStateMenu.SuspendLayout();
            this.singleStateMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // navigationStateView
            // 
            this.navigationStateView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.navigationStateView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn});
            this.navigationStateView.ContextMenuStrip = this.createStateMenu;
            this.navigationStateView.HideSelection = false;
            this.navigationStateView.Location = new System.Drawing.Point(0, 32);
            this.navigationStateView.Name = "navigationStateView";
            this.navigationStateView.Size = new System.Drawing.Size(276, 293);
            this.navigationStateView.TabIndex = 9;
            this.navigationStateView.UseCompatibleStateImageBehavior = false;
            this.navigationStateView.View = System.Windows.Forms.View.Details;
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "State";
            this.nameColumn.Width = 270;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 331);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 355);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Translation";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 378);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Look At";
            // 
            // hiddenCheck
            // 
            this.hiddenCheck.AutoSize = true;
            this.hiddenCheck.Location = new System.Drawing.Point(7, 399);
            this.hiddenCheck.Name = "hiddenCheck";
            this.hiddenCheck.Size = new System.Drawing.Size(60, 17);
            this.hiddenCheck.TabIndex = 15;
            this.hiddenCheck.Text = "Hidden";
            this.hiddenCheck.UseVisualStyleBackColor = true;
            // 
            // nameText
            // 
            this.nameText.Location = new System.Drawing.Point(74, 329);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(190, 20);
            this.nameText.TabIndex = 16;
            // 
            // translationText
            // 
            this.translationText.Location = new System.Drawing.Point(74, 352);
            this.translationText.Name = "translationText";
            this.translationText.Size = new System.Drawing.Size(190, 20);
            this.translationText.TabIndex = 17;
            // 
            // lookAtText
            // 
            this.lookAtText.Location = new System.Drawing.Point(74, 375);
            this.lookAtText.Name = "lookAtText";
            this.lookAtText.Size = new System.Drawing.Size(190, 20);
            this.lookAtText.TabIndex = 18;
            // 
            // stateUpdate
            // 
            this.stateUpdate.Enabled = false;
            this.stateUpdate.Location = new System.Drawing.Point(5, 418);
            this.stateUpdate.Name = "stateUpdate";
            this.stateUpdate.Size = new System.Drawing.Size(75, 23);
            this.stateUpdate.TabIndex = 19;
            this.stateUpdate.Text = "Update";
            this.stateUpdate.UseVisualStyleBackColor = true;
            this.stateUpdate.Click += new System.EventHandler(this.stateUpdate_Click);
            // 
            // gotoButton
            // 
            this.gotoButton.Enabled = false;
            this.gotoButton.Location = new System.Drawing.Point(86, 418);
            this.gotoButton.Name = "gotoButton";
            this.gotoButton.Size = new System.Drawing.Size(75, 23);
            this.gotoButton.TabIndex = 20;
            this.gotoButton.Text = "Goto";
            this.gotoButton.UseVisualStyleBackColor = true;
            this.gotoButton.Click += new System.EventHandler(this.gotoButton_Click);
            // 
            // linkView
            // 
            this.linkView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.linkView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.linkView.HideSelection = false;
            this.linkView.Location = new System.Drawing.Point(0, 447);
            this.linkView.MultiSelect = false;
            this.linkView.Name = "linkView";
            this.linkView.Size = new System.Drawing.Size(276, 143);
            this.linkView.TabIndex = 21;
            this.linkView.UseCompatibleStateImageBehavior = false;
            this.linkView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Link";
            this.columnHeader1.Width = 270;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 597);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Button";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 624);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Visual Radius";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(86, 597);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(178, 21);
            this.comboBox1.TabIndex = 24;
            // 
            // radiusUpDown
            // 
            this.radiusUpDown.DecimalPlaces = 2;
            this.radiusUpDown.Location = new System.Drawing.Point(86, 622);
            this.radiusUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.radiusUpDown.Name = "radiusUpDown";
            this.radiusUpDown.Size = new System.Drawing.Size(120, 20);
            this.radiusUpDown.TabIndex = 25;
            // 
            // linkUpdate
            // 
            this.linkUpdate.Location = new System.Drawing.Point(7, 653);
            this.linkUpdate.Name = "linkUpdate";
            this.linkUpdate.Size = new System.Drawing.Size(75, 23);
            this.linkUpdate.TabIndex = 26;
            this.linkUpdate.Text = "Update";
            this.linkUpdate.UseVisualStyleBackColor = true;
            // 
            // useCurrentButton
            // 
            this.useCurrentButton.Enabled = false;
            this.useCurrentButton.Location = new System.Drawing.Point(167, 418);
            this.useCurrentButton.Name = "useCurrentButton";
            this.useCurrentButton.Size = new System.Drawing.Size(75, 23);
            this.useCurrentButton.TabIndex = 27;
            this.useCurrentButton.Text = "Use Current";
            this.useCurrentButton.UseVisualStyleBackColor = true;
            this.useCurrentButton.Click += new System.EventHandler(this.useCurrentButton_Click);
            // 
            // showNavigationCheck
            // 
            this.showNavigationCheck.Appearance = System.Windows.Forms.Appearance.Button;
            this.showNavigationCheck.AutoSize = true;
            this.showNavigationCheck.Location = new System.Drawing.Point(2, 4);
            this.showNavigationCheck.Name = "showNavigationCheck";
            this.showNavigationCheck.Size = new System.Drawing.Size(98, 23);
            this.showNavigationCheck.TabIndex = 28;
            this.showNavigationCheck.Text = "Show Navigation";
            this.showNavigationCheck.UseVisualStyleBackColor = true;
            this.showNavigationCheck.CheckedChanged += new System.EventHandler(this.showNavigationCheck_CheckedChanged);
            // 
            // multipleStateMenu
            // 
            this.multipleStateMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createLinkToolStripMenuItem,
            this.createTwoWayLinkToolStripMenuItem,
            this.destroySelectedToolStripMenuItem});
            this.multipleStateMenu.Name = "multipleStateMenu";
            this.multipleStateMenu.Size = new System.Drawing.Size(186, 92);
            // 
            // createLinkToolStripMenuItem
            // 
            this.createLinkToolStripMenuItem.Name = "createLinkToolStripMenuItem";
            this.createLinkToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.createLinkToolStripMenuItem.Text = "Create Link";
            this.createLinkToolStripMenuItem.Click += new System.EventHandler(this.createLinkToolStripMenuItem_Click);
            // 
            // createTwoWayLinkToolStripMenuItem
            // 
            this.createTwoWayLinkToolStripMenuItem.Name = "createTwoWayLinkToolStripMenuItem";
            this.createTwoWayLinkToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.createTwoWayLinkToolStripMenuItem.Text = "Create Two Way Link";
            this.createTwoWayLinkToolStripMenuItem.Click += new System.EventHandler(this.createTwoWayLinkToolStripMenuItem_Click);
            // 
            // createStateMenu
            // 
            this.createStateMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createStateToolStripMenuItem});
            this.createStateMenu.Name = "createStateMenu";
            this.createStateMenu.Size = new System.Drawing.Size(138, 26);
            // 
            // createStateToolStripMenuItem
            // 
            this.createStateToolStripMenuItem.Name = "createStateToolStripMenuItem";
            this.createStateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.createStateToolStripMenuItem.Text = "Create State";
            this.createStateToolStripMenuItem.Click += new System.EventHandler(this.createStateToolStripMenuItem_Click);
            // 
            // singleStateMenu
            // 
            this.singleStateMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.destroyStateToolStripMenuItem});
            this.singleStateMenu.Name = "singleStateMenu";
            this.singleStateMenu.Size = new System.Drawing.Size(144, 26);
            // 
            // destroyStateToolStripMenuItem
            // 
            this.destroyStateToolStripMenuItem.Name = "destroyStateToolStripMenuItem";
            this.destroyStateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.destroyStateToolStripMenuItem.Text = "Destroy State";
            this.destroyStateToolStripMenuItem.Click += new System.EventHandler(this.destroyStateToolStripMenuItem_Click);
            // 
            // destroySelectedToolStripMenuItem
            // 
            this.destroySelectedToolStripMenuItem.Name = "destroySelectedToolStripMenuItem";
            this.destroySelectedToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.destroySelectedToolStripMenuItem.Text = "Destroy Selected";
            this.destroySelectedToolStripMenuItem.Click += new System.EventHandler(this.destroySelectedToolStripMenuItem_Click);
            // 
            // NavigationStateSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(276, 691);
            this.Controls.Add(this.showNavigationCheck);
            this.Controls.Add(this.useCurrentButton);
            this.Controls.Add(this.linkUpdate);
            this.Controls.Add(this.radiusUpDown);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.linkView);
            this.Controls.Add(this.gotoButton);
            this.Controls.Add(this.stateUpdate);
            this.Controls.Add(this.lookAtText);
            this.Controls.Add(this.translationText);
            this.Controls.Add(this.nameText);
            this.Controls.Add(this.hiddenCheck);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.navigationStateView);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Name = "NavigationStateSelector";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Navigation State Editor";
            this.ToolStripName = "Editing";
            ((System.ComponentModel.ISupportInitialize)(this.radiusUpDown)).EndInit();
            this.multipleStateMenu.ResumeLayout(false);
            this.createStateMenu.ResumeLayout(false);
            this.singleStateMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView navigationStateView;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox hiddenCheck;
        private System.Windows.Forms.TextBox nameText;
        private System.Windows.Forms.TextBox translationText;
        private System.Windows.Forms.TextBox lookAtText;
        private System.Windows.Forms.Button stateUpdate;
        private System.Windows.Forms.Button gotoButton;
        private System.Windows.Forms.ListView linkView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.NumericUpDown radiusUpDown;
        private System.Windows.Forms.Button linkUpdate;
        private System.Windows.Forms.Button useCurrentButton;
        private System.Windows.Forms.CheckBox showNavigationCheck;
        private System.Windows.Forms.ContextMenuStrip multipleStateMenu;
        private System.Windows.Forms.ToolStripMenuItem createLinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createTwoWayLinkToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip createStateMenu;
        private System.Windows.Forms.ToolStripMenuItem createStateToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip singleStateMenu;
        private System.Windows.Forms.ToolStripMenuItem destroyStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem destroySelectedToolStripMenuItem;


    }
}