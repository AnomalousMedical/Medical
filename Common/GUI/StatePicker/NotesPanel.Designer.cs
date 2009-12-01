﻿namespace Medical.GUI
{
    partial class NotesPanel
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.stateNameTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.datePicker = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.procedureType = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.notes = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.kryptonLabel4);
            this.panel1.Controls.Add(this.stateNameTextBox);
            this.panel1.Controls.Add(this.kryptonLabel3);
            this.panel1.Controls.Add(this.datePicker);
            this.panel1.Controls.Add(this.kryptonLabel2);
            this.panel1.Controls.Add(this.procedureType);
            this.panel1.Controls.Add(this.kryptonLabel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(273, 158);
            this.panel1.TabIndex = 3;
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(3, 0);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(70, 19);
            this.kryptonLabel4.TabIndex = 13;
            this.kryptonLabel4.Values.Text = "State Name";
            // 
            // stateNameTextBox
            // 
            this.stateNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stateNameTextBox.Location = new System.Drawing.Point(6, 21);
            this.stateNameTextBox.Name = "stateNameTextBox";
            this.stateNameTextBox.Size = new System.Drawing.Size(262, 20);
            this.stateNameTextBox.TabIndex = 12;
            this.stateNameTextBox.Text = "Custom State";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(3, 45);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(90, 19);
            this.kryptonLabel3.TabIndex = 11;
            this.kryptonLabel3.Values.Text = "Procedure Date";
            // 
            // datePicker
            // 
            this.datePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.datePicker.Location = new System.Drawing.Point(6, 66);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(262, 21);
            this.datePicker.TabIndex = 10;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(3, 91);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(91, 19);
            this.kryptonLabel2.TabIndex = 9;
            this.kryptonLabel2.Values.Text = "Procedure Type";
            // 
            // procedureType
            // 
            this.procedureType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.procedureType.Location = new System.Drawing.Point(6, 112);
            this.procedureType.Name = "procedureType";
            this.procedureType.Size = new System.Drawing.Size(262, 20);
            this.procedureType.TabIndex = 8;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 135);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(41, 19);
            this.kryptonLabel1.TabIndex = 7;
            this.kryptonLabel1.Values.Text = "Notes";
            // 
            // notes
            // 
            this.notes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notes.Location = new System.Drawing.Point(5, 158);
            this.notes.Name = "notes";
            this.notes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.notes.Size = new System.Drawing.Size(263, 161);
            this.notes.TabIndex = 4;
            this.notes.Text = "";
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(0, 158);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(5, 163);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge1.StateCommon.Color2 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(268, 158);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(5, 163);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge2.StateCommon.Color2 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            // 
            // kryptonBorderEdge3
            // 
            this.kryptonBorderEdge3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge3.Location = new System.Drawing.Point(5, 319);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(263, 2);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge3.StateCommon.Color2 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
            // 
            // NotesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.notes);
            this.Controls.Add(this.kryptonBorderEdge3);
            this.Controls.Add(this.kryptonBorderEdge2);
            this.Controls.Add(this.kryptonBorderEdge1);
            this.Controls.Add(this.panel1);
            this.LargeIcon = global::Medical.Properties.Resources.NotesIcon;
            this.LayerState = "MandibleSizeLayers";
            this.Name = "NotesPanel";
            this.NavigationState = "Midline Anterior";
            this.Size = new System.Drawing.Size(273, 321);
            this.TextLine1 = "Notes";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox notes;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker datePicker;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox procedureType;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox stateNameTextBox;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
    }
}
