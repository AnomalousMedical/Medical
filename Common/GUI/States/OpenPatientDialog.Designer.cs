namespace Medical.GUI
{
    partial class OpenPatientDialog
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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.loadingProgress = new System.Windows.Forms.ProgressBar();
            this.searchBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.warningLabel = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.browseButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.locationTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.cancelButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.openButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.fileDataGrid = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.lastNameColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.firstNameColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.dateModifiedColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.fileListWorker = new System.ComponentModel.BackgroundWorker();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.loadingProgress);
            this.kryptonPanel1.Controls.Add(this.searchBox);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Controls.Add(this.warningLabel);
            this.kryptonPanel1.Controls.Add(this.browseButton);
            this.kryptonPanel1.Controls.Add(this.locationTextBox);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.cancelButton);
            this.kryptonPanel1.Controls.Add(this.openButton);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 360);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(708, 124);
            this.kryptonPanel1.TabIndex = 2;
            // 
            // loadingProgress
            // 
            this.loadingProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadingProgress.Location = new System.Drawing.Point(605, 98);
            this.loadingProgress.Name = "loadingProgress";
            this.loadingProgress.Size = new System.Drawing.Size(100, 23);
            this.loadingProgress.TabIndex = 16;
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(5, 26);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(185, 20);
            this.searchBox.TabIndex = 15;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 4);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(102, 19);
            this.kryptonLabel1.TabIndex = 14;
            this.kryptonLabel1.Values.Text = "Search Last Name";
            // 
            // warningLabel
            // 
            this.warningLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.warningLabel.Location = new System.Drawing.Point(504, 60);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(201, 42);
            this.warningLabel.TabIndex = 13;
            this.warningLabel.Values.Image = global::Medical.Properties.Resources.Warning;
            this.warningLabel.Values.Text = "This directory does not exist.";
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(408, 67);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(90, 25);
            this.browseButton.TabIndex = 12;
            this.browseButton.Values.Text = "Browse";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // locationTextBox
            // 
            this.locationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationTextBox.Location = new System.Drawing.Point(5, 70);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(397, 20);
            this.locationTextBox.TabIndex = 11;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(3, 48);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(83, 19);
            this.kryptonLabel3.TabIndex = 10;
            this.kryptonLabel3.Values.Text = "Load Location";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(100, 94);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 25);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Values.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // openButton
            // 
            this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.openButton.Location = new System.Drawing.Point(3, 94);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(90, 25);
            this.openButton.TabIndex = 0;
            this.openButton.Values.Text = "Open";
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // fileDataGrid
            // 
            this.fileDataGrid.AllowUserToAddRows = false;
            this.fileDataGrid.AllowUserToDeleteRows = false;
            this.fileDataGrid.AllowUserToOrderColumns = true;
            this.fileDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.lastNameColumn,
            this.firstNameColumn,
            this.dateModifiedColumn});
            this.fileDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileDataGrid.Location = new System.Drawing.Point(0, 0);
            this.fileDataGrid.MultiSelect = false;
            this.fileDataGrid.Name = "fileDataGrid";
            this.fileDataGrid.ReadOnly = true;
            this.fileDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fileDataGrid.Size = new System.Drawing.Size(708, 360);
            this.fileDataGrid.TabIndex = 3;
            // 
            // lastNameColumn
            // 
            this.lastNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.lastNameColumn.DataPropertyName = "LastName";
            this.lastNameColumn.HeaderText = "Last Name";
            this.lastNameColumn.Name = "lastNameColumn";
            this.lastNameColumn.ReadOnly = true;
            this.lastNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // firstNameColumn
            // 
            this.firstNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.firstNameColumn.DataPropertyName = "FirstName";
            this.firstNameColumn.HeaderText = "FirstName";
            this.firstNameColumn.Name = "firstNameColumn";
            this.firstNameColumn.ReadOnly = true;
            this.firstNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // dateModifiedColumn
            // 
            this.dateModifiedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dateModifiedColumn.DataPropertyName = "DateModified";
            this.dateModifiedColumn.HeaderText = "Date Modified";
            this.dateModifiedColumn.Name = "dateModifiedColumn";
            this.dateModifiedColumn.ReadOnly = true;
            this.dateModifiedColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // fileListWorker
            // 
            this.fileListWorker.WorkerReportsProgress = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "LastName";
            this.dataGridViewTextBoxColumn1.HeaderText = "Last Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "FirstName";
            this.dataGridViewTextBoxColumn2.HeaderText = "First Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "DateModified";
            this.dataGridViewTextBoxColumn3.HeaderText = "DateModified";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // OpenPatientDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 484);
            this.Controls.Add(this.fileDataGrid);
            this.Controls.Add(this.kryptonPanel1);
            this.MinimumSize = new System.Drawing.Size(416, 200);
            this.Name = "OpenPatientDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Open";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton cancelButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton openButton;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView fileDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn lastNameColumn;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn firstNameColumn;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn dateModifiedColumn;
        private ComponentFactory.Krypton.Toolkit.KryptonButton browseButton;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox locationTextBox;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel warningLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox searchBox;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.ComponentModel.BackgroundWorker fileListWorker;
        private System.Windows.Forms.ProgressBar loadingProgress;
    }
}