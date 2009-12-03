namespace Medical.GUI
{
    partial class NavigationEditor
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
            this.createStateMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.linkMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTwoWayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonCombo = new System.Windows.Forms.ComboBox();
            this.radiusUpDown = new System.Windows.Forms.NumericUpDown();
            this.linkUpdate = new System.Windows.Forms.Button();
            this.useCurrentButton = new System.Windows.Forms.Button();
            this.multipleStateMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTwoWayLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destroySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singleStateMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.destroyStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.navigationArrowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.translationGoButton = new System.Windows.Forms.Button();
            this.lookAtGoButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.keyCombo = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.createButton = new System.Windows.Forms.Button();
            this.navigationStateView = new DragNDrop.DragAndDropListView();
            this.State = new System.Windows.Forms.ColumnHeader();
            this.createStateMenu.SuspendLayout();
            this.linkMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radiusUpDown)).BeginInit();
            this.multipleStateMenu.SuspendLayout();
            this.singleStateMenu.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            this.createStateToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.createStateToolStripMenuItem.Text = "Create State";
            this.createStateToolStripMenuItem.Click += new System.EventHandler(this.createStateToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Translation";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Look At";
            // 
            // hiddenCheck
            // 
            this.hiddenCheck.AutoSize = true;
            this.hiddenCheck.Location = new System.Drawing.Point(7, 99);
            this.hiddenCheck.Name = "hiddenCheck";
            this.hiddenCheck.Size = new System.Drawing.Size(60, 17);
            this.hiddenCheck.TabIndex = 15;
            this.hiddenCheck.Text = "Hidden";
            this.hiddenCheck.UseVisualStyleBackColor = true;
            // 
            // nameText
            // 
            this.nameText.Location = new System.Drawing.Point(74, 29);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(190, 20);
            this.nameText.TabIndex = 16;
            // 
            // translationText
            // 
            this.translationText.Location = new System.Drawing.Point(74, 52);
            this.translationText.Name = "translationText";
            this.translationText.Size = new System.Drawing.Size(168, 20);
            this.translationText.TabIndex = 17;
            // 
            // lookAtText
            // 
            this.lookAtText.Location = new System.Drawing.Point(74, 75);
            this.lookAtText.Name = "lookAtText";
            this.lookAtText.Size = new System.Drawing.Size(168, 20);
            this.lookAtText.TabIndex = 18;
            // 
            // stateUpdate
            // 
            this.stateUpdate.Enabled = false;
            this.stateUpdate.Location = new System.Drawing.Point(5, 3);
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
            this.gotoButton.Location = new System.Drawing.Point(86, 3);
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
            this.linkView.ContextMenuStrip = this.linkMenu;
            this.linkView.HideSelection = false;
            this.linkView.Location = new System.Drawing.Point(0, 154);
            this.linkView.MultiSelect = false;
            this.linkView.Name = "linkView";
            this.linkView.Size = new System.Drawing.Size(286, 143);
            this.linkView.TabIndex = 21;
            this.linkView.UseCompatibleStateImageBehavior = false;
            this.linkView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Link";
            this.columnHeader1.Width = 270;
            // 
            // linkMenu
            // 
            this.linkMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteLinkToolStripMenuItem,
            this.deleteTwoWayToolStripMenuItem});
            this.linkMenu.Name = "linkMenu";
            this.linkMenu.Size = new System.Drawing.Size(160, 48);
            // 
            // deleteLinkToolStripMenuItem
            // 
            this.deleteLinkToolStripMenuItem.Name = "deleteLinkToolStripMenuItem";
            this.deleteLinkToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.deleteLinkToolStripMenuItem.Text = "Delete";
            this.deleteLinkToolStripMenuItem.Click += new System.EventHandler(this.deleteLinkToolStripMenuItem_Click);
            // 
            // deleteTwoWayToolStripMenuItem
            // 
            this.deleteTwoWayToolStripMenuItem.Name = "deleteTwoWayToolStripMenuItem";
            this.deleteTwoWayToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.deleteTwoWayToolStripMenuItem.Text = "Delete Two Way";
            this.deleteTwoWayToolStripMenuItem.Click += new System.EventHandler(this.deleteTwoWayToolStripMenuItem_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 304);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Button";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 331);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Visual Radius";
            // 
            // buttonCombo
            // 
            this.buttonCombo.FormattingEnabled = true;
            this.buttonCombo.Location = new System.Drawing.Point(86, 304);
            this.buttonCombo.Name = "buttonCombo";
            this.buttonCombo.Size = new System.Drawing.Size(178, 21);
            this.buttonCombo.TabIndex = 24;
            // 
            // radiusUpDown
            // 
            this.radiusUpDown.DecimalPlaces = 2;
            this.radiusUpDown.Location = new System.Drawing.Point(86, 329);
            this.radiusUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.radiusUpDown.Name = "radiusUpDown";
            this.radiusUpDown.Size = new System.Drawing.Size(120, 20);
            this.radiusUpDown.TabIndex = 25;
            this.radiusUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // linkUpdate
            // 
            this.linkUpdate.Location = new System.Drawing.Point(7, 360);
            this.linkUpdate.Name = "linkUpdate";
            this.linkUpdate.Size = new System.Drawing.Size(75, 23);
            this.linkUpdate.TabIndex = 26;
            this.linkUpdate.Text = "Update";
            this.linkUpdate.UseVisualStyleBackColor = true;
            this.linkUpdate.Click += new System.EventHandler(this.linkUpdate_Click);
            // 
            // useCurrentButton
            // 
            this.useCurrentButton.Enabled = false;
            this.useCurrentButton.Location = new System.Drawing.Point(73, 97);
            this.useCurrentButton.Name = "useCurrentButton";
            this.useCurrentButton.Size = new System.Drawing.Size(75, 23);
            this.useCurrentButton.TabIndex = 27;
            this.useCurrentButton.Text = "Use Current";
            this.useCurrentButton.UseVisualStyleBackColor = true;
            this.useCurrentButton.Click += new System.EventHandler(this.useCurrentButton_Click);
            // 
            // multipleStateMenu
            // 
            this.multipleStateMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createLinkToolStripMenuItem,
            this.createTwoWayLinkToolStripMenuItem,
            this.destroySelectedToolStripMenuItem});
            this.multipleStateMenu.Name = "multipleStateMenu";
            this.multipleStateMenu.Size = new System.Drawing.Size(186, 70);
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
            // destroySelectedToolStripMenuItem
            // 
            this.destroySelectedToolStripMenuItem.Name = "destroySelectedToolStripMenuItem";
            this.destroySelectedToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.destroySelectedToolStripMenuItem.Text = "Destroy Selected";
            this.destroySelectedToolStripMenuItem.Click += new System.EventHandler(this.destroySelectedToolStripMenuItem_Click);
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
            this.destroyStateToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.destroyStateToolStripMenuItem.Text = "Destroy State";
            this.destroyStateToolStripMenuItem.Click += new System.EventHandler(this.destroyStateToolStripMenuItem_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.showToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(274, 24);
            this.mainMenu.TabIndex = 28;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigationArrowsToolStripMenuItem});
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.showToolStripMenuItem.Text = "Show";
            // 
            // navigationArrowsToolStripMenuItem
            // 
            this.navigationArrowsToolStripMenuItem.CheckOnClick = true;
            this.navigationArrowsToolStripMenuItem.Name = "navigationArrowsToolStripMenuItem";
            this.navigationArrowsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.navigationArrowsToolStripMenuItem.Text = "Navigation Arrows";
            this.navigationArrowsToolStripMenuItem.Click += new System.EventHandler(this.navigationArrowsToolStripMenuItem_Click);
            // 
            // translationGoButton
            // 
            this.translationGoButton.Location = new System.Drawing.Point(244, 50);
            this.translationGoButton.Name = "translationGoButton";
            this.translationGoButton.Size = new System.Drawing.Size(29, 23);
            this.translationGoButton.TabIndex = 29;
            this.translationGoButton.Text = "Go";
            this.translationGoButton.UseVisualStyleBackColor = true;
            this.translationGoButton.Click += new System.EventHandler(this.translationGoButton_Click);
            // 
            // lookAtGoButton
            // 
            this.lookAtGoButton.Location = new System.Drawing.Point(244, 74);
            this.lookAtGoButton.Name = "lookAtGoButton";
            this.lookAtGoButton.Size = new System.Drawing.Size(29, 23);
            this.lookAtGoButton.TabIndex = 30;
            this.lookAtGoButton.Text = "Go";
            this.lookAtGoButton.UseVisualStyleBackColor = true;
            this.lookAtGoButton.Click += new System.EventHandler(this.lookAtGoButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 126);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Shortcut Key Ctrl + ";
            // 
            // keyCombo
            // 
            this.keyCombo.FormattingEnabled = true;
            this.keyCombo.Location = new System.Drawing.Point(103, 123);
            this.keyCombo.Name = "keyCombo";
            this.keyCombo.Size = new System.Drawing.Size(166, 21);
            this.keyCombo.TabIndex = 32;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.createButton);
            this.panel1.Controls.Add(this.stateUpdate);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.hiddenCheck);
            this.panel1.Controls.Add(this.keyCombo);
            this.panel1.Controls.Add(this.nameText);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.translationText);
            this.panel1.Controls.Add(this.lookAtGoButton);
            this.panel1.Controls.Add(this.lookAtText);
            this.panel1.Controls.Add(this.translationGoButton);
            this.panel1.Controls.Add(this.gotoButton);
            this.panel1.Controls.Add(this.linkView);
            this.panel1.Controls.Add(this.useCurrentButton);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.linkUpdate);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.radiusUpDown);
            this.panel1.Controls.Add(this.buttonCombo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 324);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(274, 387);
            this.panel1.TabIndex = 33;
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(167, 3);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(75, 23);
            this.createButton.TabIndex = 33;
            this.createButton.Text = "Create";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // navigationStateView
            // 
            this.navigationStateView.AllowDrop = true;
            this.navigationStateView.AllowReorder = true;
            this.navigationStateView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.State});
            this.navigationStateView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationStateView.HideSelection = false;
            this.navigationStateView.LineColor = System.Drawing.Color.Red;
            this.navigationStateView.Location = new System.Drawing.Point(0, 24);
            this.navigationStateView.Name = "navigationStateView";
            this.navigationStateView.Size = new System.Drawing.Size(274, 300);
            this.navigationStateView.TabIndex = 34;
            this.navigationStateView.UseCompatibleStateImageBehavior = false;
            this.navigationStateView.View = System.Windows.Forms.View.Details;
            // 
            // State
            // 
            this.State.Text = "State";
            this.State.Width = 0;
            // 
            // NavigationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ButtonText = "Navigation State Editor";
            this.Controls.Add(this.navigationStateView);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mainMenu);
            this.DockAreas = ((Medical.GUI.DockLocations)((Medical.GUI.DockLocations.Right | Medical.GUI.DockLocations.Float)));
            this.Name = "NavigationEditor";
            this.ShowHint = Medical.GUI.DockLocations.Right;
            this.Size = new System.Drawing.Size(274, 711);
            this.ToolStripName = "Editing";
            this.createStateMenu.ResumeLayout(false);
            this.linkMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radiusUpDown)).EndInit();
            this.multipleStateMenu.ResumeLayout(false);
            this.singleStateMenu.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.ComboBox buttonCombo;
        private System.Windows.Forms.NumericUpDown radiusUpDown;
        private System.Windows.Forms.Button linkUpdate;
        private System.Windows.Forms.Button useCurrentButton;
        private System.Windows.Forms.ContextMenuStrip multipleStateMenu;
        private System.Windows.Forms.ToolStripMenuItem createLinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createTwoWayLinkToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip createStateMenu;
        private System.Windows.Forms.ToolStripMenuItem createStateToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip singleStateMenu;
        private System.Windows.Forms.ToolStripMenuItem destroyStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem destroySelectedToolStripMenuItem;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem navigationArrowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip linkMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteLinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTwoWayToolStripMenuItem;
        private System.Windows.Forms.Button translationGoButton;
        private System.Windows.Forms.Button lookAtGoButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox keyCombo;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private DragNDrop.DragAndDropListView navigationStateView;
        private System.Windows.Forms.ColumnHeader State;
        private System.Windows.Forms.Button createButton;


    }
}