namespace Medical.GUI
{
    partial class StatePickerModeList
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
            this.navigatorList = new System.Windows.Forms.ListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // navigatorList
            // 
            this.navigatorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn});
            this.navigatorList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigatorList.HideSelection = false;
            this.navigatorList.LabelWrap = false;
            this.navigatorList.Location = new System.Drawing.Point(0, 0);
            this.navigatorList.MultiSelect = false;
            this.navigatorList.Name = "navigatorList";
            this.navigatorList.Size = new System.Drawing.Size(733, 130);
            this.navigatorList.TabIndex = 1;
            this.navigatorList.UseCompatibleStateImageBehavior = false;
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 100;
            // 
            // StatePickerModeList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 130);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.navigatorList);
            this.Name = "StatePickerModeList";
            this.Text = "Distortion Wizard";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView navigatorList;
        private System.Windows.Forms.ColumnHeader nameColumn;
    }
}