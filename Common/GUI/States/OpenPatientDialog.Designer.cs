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
            this.panel1 = new System.Windows.Forms.Panel();
            this.exportButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.fileList = new System.Windows.Forms.ListView();
            this.name = new System.Windows.Forms.ColumnHeader();
            this.dateModified = new System.Windows.Forms.ColumnHeader();
            this.cancelButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.exportButton);
            this.panel1.Controls.Add(this.importButton);
            this.panel1.Controls.Add(this.openButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 288);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(451, 37);
            this.panel1.TabIndex = 0;
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(369, 7);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(287, 7);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(75, 23);
            this.importButton.TabIndex = 1;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(6, 7);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 0;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // fileList
            // 
            this.fileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.dateModified});
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.FullRowSelect = true;
            this.fileList.Location = new System.Drawing.Point(0, 0);
            this.fileList.MultiSelect = false;
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(451, 288);
            this.fileList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.fileList.TabIndex = 1;
            this.fileList.UseCompatibleStateImageBehavior = false;
            this.fileList.View = System.Windows.Forms.View.Details;
            // 
            // name
            // 
            this.name.Text = "Name";
            this.name.Width = 262;
            // 
            // dateModified
            // 
            this.dateModified.Text = "DateModified";
            this.dateModified.Width = 184;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(88, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // OpenPatientDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 325);
            this.Controls.Add(this.fileList);
            this.Controls.Add(this.panel1);
            this.Name = "OpenPatientDialog";
            this.ShowInTaskbar = false;
            this.Text = "Open";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.ListView fileList;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader dateModified;
        private System.Windows.Forms.Button cancelButton;
    }
}