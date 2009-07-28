namespace Medical.GUI
{
    partial class StatePicker
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
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.finishButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.previousButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.statePickerPanelHost = new System.Windows.Forms.Panel();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // navigatorList
            // 
            this.navigatorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn});
            this.navigatorList.Dock = System.Windows.Forms.DockStyle.Left;
            this.navigatorList.HideSelection = false;
            this.navigatorList.Location = new System.Drawing.Point(0, 0);
            this.navigatorList.MultiSelect = false;
            this.navigatorList.Name = "navigatorList";
            this.navigatorList.Size = new System.Drawing.Size(136, 414);
            this.navigatorList.TabIndex = 0;
            this.navigatorList.UseCompatibleStateImageBehavior = false;
            this.navigatorList.View = System.Windows.Forms.View.Details;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.finishButton);
            this.buttonPanel.Controls.Add(this.nextButton);
            this.buttonPanel.Controls.Add(this.previousButton);
            this.buttonPanel.Controls.Add(this.cancelButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonPanel.Location = new System.Drawing.Point(0, 414);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(624, 30);
            this.buttonPanel.TabIndex = 1;
            // 
            // finishButton
            // 
            this.finishButton.Location = new System.Drawing.Point(546, 3);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 2;
            this.finishButton.Text = "Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(465, 3);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 0;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // previousButton
            // 
            this.previousButton.Location = new System.Drawing.Point(384, 3);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(75, 23);
            this.previousButton.TabIndex = 1;
            this.previousButton.Text = "Previous";
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(303, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // statePickerPanelHost
            // 
            this.statePickerPanelHost.AutoScroll = true;
            this.statePickerPanelHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statePickerPanelHost.Location = new System.Drawing.Point(136, 0);
            this.statePickerPanelHost.Name = "statePickerPanelHost";
            this.statePickerPanelHost.Size = new System.Drawing.Size(488, 414);
            this.statePickerPanelHost.TabIndex = 2;
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 100;
            // 
            // StatePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 444);
            this.Controls.Add(this.statePickerPanelHost);
            this.Controls.Add(this.navigatorList);
            this.Controls.Add(this.buttonPanel);
            this.Name = "StatePicker";
            this.Text = "StatePicker";
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView navigatorList;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button previousButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Panel statePickerPanelHost;
        private System.Windows.Forms.ColumnHeader nameColumn;
    }
}