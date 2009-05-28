namespace Medical.GUI
{
    partial class MuscleControl
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.clenchButton = new System.Windows.Forms.Button();
            this.neutralButton = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.bothSides = new System.Windows.Forms.ComboBox();
            this.toggleMuscles = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.forceUpDown = new System.Windows.Forms.NumericUpDown();
            this.leftMuscleGroups = new System.Windows.Forms.ComboBox();
            this.rightMuscleGroups = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Right = new System.Windows.Forms.Label();
            this.leftDigastricForce = new System.Windows.Forms.NumericUpDown();
            this.rightDigastricForce = new System.Windows.Forms.NumericUpDown();
            this.leftLatPtForce = new System.Windows.Forms.NumericUpDown();
            this.rightLatPtForce = new System.Windows.Forms.NumericUpDown();
            this.leftMedPtForce = new System.Windows.Forms.NumericUpDown();
            this.rightMedPtForce = new System.Windows.Forms.NumericUpDown();
            this.leftMasseterForce = new System.Windows.Forms.NumericUpDown();
            this.rightMasseterForce = new System.Windows.Forms.NumericUpDown();
            this.leftTemporalisForce = new System.Windows.Forms.NumericUpDown();
            this.rightTemporalisForce = new System.Windows.Forms.NumericUpDown();
            this.leftDigastric = new System.Windows.Forms.CheckBox();
            this.rightDigastric = new System.Windows.Forms.CheckBox();
            this.leftLateralPterygoid = new System.Windows.Forms.CheckBox();
            this.rightLateralPterygoid = new System.Windows.Forms.CheckBox();
            this.leftMedialPterygoid = new System.Windows.Forms.CheckBox();
            this.rightMedialPterygoid = new System.Windows.Forms.CheckBox();
            this.leftTemporalis = new System.Windows.Forms.CheckBox();
            this.rightTemporalis = new System.Windows.Forms.CheckBox();
            this.leftMasseter = new System.Windows.Forms.CheckBox();
            this.rightMasseter = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.forceUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftDigastricForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightDigastricForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftLatPtForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightLatPtForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftMedPtForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightMedPtForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftMasseterForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightMasseterForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftTemporalisForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightTemporalisForce)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(259, 421);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.panel4);
            this.tabPage1.Controls.Add(this.panel3);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.closeButton);
            this.tabPage1.Controls.Add(this.openButton);
            this.tabPage1.Controls.Add(this.clenchButton);
            this.tabPage1.Controls.Add(this.neutralButton);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(251, 395);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Basic";
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::Medical.Properties.Resources.openmuscle;
            this.panel4.Location = new System.Drawing.Point(18, 167);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(96, 115);
            this.panel4.TabIndex = 7;
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = global::Medical.Properties.Resources.clenchedmuscle;
            this.panel3.Location = new System.Drawing.Point(125, 16);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(96, 115);
            this.panel3.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Medical.Properties.Resources.neutralmuscle;
            this.panel2.Location = new System.Drawing.Point(125, 167);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(96, 115);
            this.panel2.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.neutralmuscle;
            this.panel1.Location = new System.Drawing.Point(18, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(96, 115);
            this.panel1.TabIndex = 4;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(135, 288);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(29, 288);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 2;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // clenchButton
            // 
            this.clenchButton.Location = new System.Drawing.Point(135, 137);
            this.clenchButton.Name = "clenchButton";
            this.clenchButton.Size = new System.Drawing.Size(75, 23);
            this.clenchButton.TabIndex = 1;
            this.clenchButton.Text = "Clench";
            this.clenchButton.UseVisualStyleBackColor = true;
            this.clenchButton.Click += new System.EventHandler(this.clenchButton_Click);
            // 
            // neutralButton
            // 
            this.neutralButton.Location = new System.Drawing.Point(29, 137);
            this.neutralButton.Name = "neutralButton";
            this.neutralButton.Size = new System.Drawing.Size(75, 23);
            this.neutralButton.TabIndex = 0;
            this.neutralButton.Text = "Neutral";
            this.neutralButton.UseVisualStyleBackColor = true;
            this.neutralButton.Click += new System.EventHandler(this.neutralButton_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.bothSides);
            this.tabPage2.Controls.Add(this.toggleMuscles);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.forceUpDown);
            this.tabPage2.Controls.Add(this.leftMuscleGroups);
            this.tabPage2.Controls.Add(this.rightMuscleGroups);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.Right);
            this.tabPage2.Controls.Add(this.leftDigastricForce);
            this.tabPage2.Controls.Add(this.rightDigastricForce);
            this.tabPage2.Controls.Add(this.leftLatPtForce);
            this.tabPage2.Controls.Add(this.rightLatPtForce);
            this.tabPage2.Controls.Add(this.leftMedPtForce);
            this.tabPage2.Controls.Add(this.rightMedPtForce);
            this.tabPage2.Controls.Add(this.leftMasseterForce);
            this.tabPage2.Controls.Add(this.rightMasseterForce);
            this.tabPage2.Controls.Add(this.leftTemporalisForce);
            this.tabPage2.Controls.Add(this.rightTemporalisForce);
            this.tabPage2.Controls.Add(this.leftDigastric);
            this.tabPage2.Controls.Add(this.rightDigastric);
            this.tabPage2.Controls.Add(this.leftLateralPterygoid);
            this.tabPage2.Controls.Add(this.rightLateralPterygoid);
            this.tabPage2.Controls.Add(this.leftMedialPterygoid);
            this.tabPage2.Controls.Add(this.rightMedialPterygoid);
            this.tabPage2.Controls.Add(this.leftTemporalis);
            this.tabPage2.Controls.Add(this.rightTemporalis);
            this.tabPage2.Controls.Add(this.leftMasseter);
            this.tabPage2.Controls.Add(this.rightMasseter);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(251, 395);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Advanced";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 82;
            this.label3.Text = "Both Sides";
            // 
            // bothSides
            // 
            this.bothSides.FormattingEnabled = true;
            this.bothSides.Location = new System.Drawing.Point(1, 19);
            this.bothSides.Name = "bothSides";
            this.bothSides.Size = new System.Drawing.Size(117, 21);
            this.bothSides.TabIndex = 81;
            // 
            // toggleMuscles
            // 
            this.toggleMuscles.Location = new System.Drawing.Point(146, 17);
            this.toggleMuscles.Name = "toggleMuscles";
            this.toggleMuscles.Size = new System.Drawing.Size(86, 23);
            this.toggleMuscles.TabIndex = 80;
            this.toggleMuscles.Text = "Show Vectors";
            this.toggleMuscles.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 79;
            this.label2.Text = "Selected Force";
            // 
            // forceUpDown
            // 
            this.forceUpDown.DecimalPlaces = 4;
            this.forceUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.forceUpDown.Location = new System.Drawing.Point(106, 101);
            this.forceUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.forceUpDown.Name = "forceUpDown";
            this.forceUpDown.Size = new System.Drawing.Size(116, 20);
            this.forceUpDown.TabIndex = 78;
            // 
            // leftMuscleGroups
            // 
            this.leftMuscleGroups.FormattingEnabled = true;
            this.leftMuscleGroups.Location = new System.Drawing.Point(129, 70);
            this.leftMuscleGroups.Name = "leftMuscleGroups";
            this.leftMuscleGroups.Size = new System.Drawing.Size(117, 21);
            this.leftMuscleGroups.TabIndex = 77;
            // 
            // rightMuscleGroups
            // 
            this.rightMuscleGroups.FormattingEnabled = true;
            this.rightMuscleGroups.Location = new System.Drawing.Point(3, 70);
            this.rightMuscleGroups.Name = "rightMuscleGroups";
            this.rightMuscleGroups.Size = new System.Drawing.Size(115, 21);
            this.rightMuscleGroups.TabIndex = 76;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(129, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 75;
            this.label1.Text = "Left";
            // 
            // Right
            // 
            this.Right.AutoSize = true;
            this.Right.Location = new System.Drawing.Point(5, 49);
            this.Right.Name = "Right";
            this.Right.Size = new System.Drawing.Size(32, 13);
            this.Right.TabIndex = 74;
            this.Right.Text = "Right";
            // 
            // leftDigastricForce
            // 
            this.leftDigastricForce.DecimalPlaces = 4;
            this.leftDigastricForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.leftDigastricForce.Location = new System.Drawing.Point(130, 366);
            this.leftDigastricForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.leftDigastricForce.Name = "leftDigastricForce";
            this.leftDigastricForce.Size = new System.Drawing.Size(116, 20);
            this.leftDigastricForce.TabIndex = 73;
            this.leftDigastricForce.Tag = "LeftDigastricDynamic";
            // 
            // rightDigastricForce
            // 
            this.rightDigastricForce.DecimalPlaces = 4;
            this.rightDigastricForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.rightDigastricForce.Location = new System.Drawing.Point(2, 366);
            this.rightDigastricForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.rightDigastricForce.Name = "rightDigastricForce";
            this.rightDigastricForce.Size = new System.Drawing.Size(116, 20);
            this.rightDigastricForce.TabIndex = 72;
            this.rightDigastricForce.Tag = "RightDigastricDynamic";
            // 
            // leftLatPtForce
            // 
            this.leftLatPtForce.DecimalPlaces = 4;
            this.leftLatPtForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.leftLatPtForce.Location = new System.Drawing.Point(130, 315);
            this.leftLatPtForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.leftLatPtForce.Name = "leftLatPtForce";
            this.leftLatPtForce.Size = new System.Drawing.Size(116, 20);
            this.leftLatPtForce.TabIndex = 71;
            this.leftLatPtForce.Tag = "LeftLateralPterygoidDynamic";
            // 
            // rightLatPtForce
            // 
            this.rightLatPtForce.DecimalPlaces = 4;
            this.rightLatPtForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.rightLatPtForce.Location = new System.Drawing.Point(2, 315);
            this.rightLatPtForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.rightLatPtForce.Name = "rightLatPtForce";
            this.rightLatPtForce.Size = new System.Drawing.Size(116, 20);
            this.rightLatPtForce.TabIndex = 70;
            this.rightLatPtForce.Tag = "RightLateralPterygoidDynamic";
            // 
            // leftMedPtForce
            // 
            this.leftMedPtForce.DecimalPlaces = 4;
            this.leftMedPtForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.leftMedPtForce.Location = new System.Drawing.Point(130, 263);
            this.leftMedPtForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.leftMedPtForce.Name = "leftMedPtForce";
            this.leftMedPtForce.Size = new System.Drawing.Size(116, 20);
            this.leftMedPtForce.TabIndex = 69;
            this.leftMedPtForce.Tag = "LeftMedialPterygoidDynamic";
            // 
            // rightMedPtForce
            // 
            this.rightMedPtForce.DecimalPlaces = 4;
            this.rightMedPtForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.rightMedPtForce.Location = new System.Drawing.Point(2, 263);
            this.rightMedPtForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.rightMedPtForce.Name = "rightMedPtForce";
            this.rightMedPtForce.Size = new System.Drawing.Size(116, 20);
            this.rightMedPtForce.TabIndex = 68;
            this.rightMedPtForce.Tag = "RightMedialPterygoidDynamic";
            // 
            // leftMasseterForce
            // 
            this.leftMasseterForce.DecimalPlaces = 4;
            this.leftMasseterForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.leftMasseterForce.Location = new System.Drawing.Point(130, 207);
            this.leftMasseterForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.leftMasseterForce.Name = "leftMasseterForce";
            this.leftMasseterForce.Size = new System.Drawing.Size(116, 20);
            this.leftMasseterForce.TabIndex = 67;
            this.leftMasseterForce.Tag = "LeftMasseterDynamic";
            // 
            // rightMasseterForce
            // 
            this.rightMasseterForce.DecimalPlaces = 4;
            this.rightMasseterForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.rightMasseterForce.Location = new System.Drawing.Point(1, 206);
            this.rightMasseterForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.rightMasseterForce.Name = "rightMasseterForce";
            this.rightMasseterForce.Size = new System.Drawing.Size(116, 20);
            this.rightMasseterForce.TabIndex = 66;
            this.rightMasseterForce.Tag = "RightMasseterDynamic";
            // 
            // leftTemporalisForce
            // 
            this.leftTemporalisForce.DecimalPlaces = 4;
            this.leftTemporalisForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.leftTemporalisForce.Location = new System.Drawing.Point(129, 156);
            this.leftTemporalisForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.leftTemporalisForce.Name = "leftTemporalisForce";
            this.leftTemporalisForce.Size = new System.Drawing.Size(116, 20);
            this.leftTemporalisForce.TabIndex = 65;
            this.leftTemporalisForce.Tag = "LeftTemporalisDynamic";
            // 
            // rightTemporalisForce
            // 
            this.rightTemporalisForce.DecimalPlaces = 4;
            this.rightTemporalisForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.rightTemporalisForce.Location = new System.Drawing.Point(1, 156);
            this.rightTemporalisForce.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.rightTemporalisForce.Name = "rightTemporalisForce";
            this.rightTemporalisForce.Size = new System.Drawing.Size(116, 20);
            this.rightTemporalisForce.TabIndex = 62;
            this.rightTemporalisForce.Tag = "RightTemporalisDynamic";
            // 
            // leftDigastric
            // 
            this.leftDigastric.Appearance = System.Windows.Forms.Appearance.Button;
            this.leftDigastric.AutoSize = true;
            this.leftDigastric.Location = new System.Drawing.Point(130, 338);
            this.leftDigastric.Name = "leftDigastric";
            this.leftDigastric.Size = new System.Drawing.Size(58, 23);
            this.leftDigastric.TabIndex = 59;
            this.leftDigastric.Tag = "LeftDigastricDynamic";
            this.leftDigastric.Text = "Digastric";
            this.leftDigastric.UseVisualStyleBackColor = true;
            // 
            // rightDigastric
            // 
            this.rightDigastric.Appearance = System.Windows.Forms.Appearance.Button;
            this.rightDigastric.AutoSize = true;
            this.rightDigastric.Location = new System.Drawing.Point(3, 338);
            this.rightDigastric.Name = "rightDigastric";
            this.rightDigastric.Size = new System.Drawing.Size(58, 23);
            this.rightDigastric.TabIndex = 58;
            this.rightDigastric.Tag = "RightDigastricDynamic";
            this.rightDigastric.Text = "Digastric";
            this.rightDigastric.UseVisualStyleBackColor = true;
            // 
            // leftLateralPterygoid
            // 
            this.leftLateralPterygoid.Appearance = System.Windows.Forms.Appearance.Button;
            this.leftLateralPterygoid.AutoSize = true;
            this.leftLateralPterygoid.Location = new System.Drawing.Point(130, 286);
            this.leftLateralPterygoid.Name = "leftLateralPterygoid";
            this.leftLateralPterygoid.Size = new System.Drawing.Size(96, 23);
            this.leftLateralPterygoid.TabIndex = 57;
            this.leftLateralPterygoid.Tag = "LeftLateralPterygoidDynamic";
            this.leftLateralPterygoid.Text = "Lateral Pterygoid";
            this.leftLateralPterygoid.UseVisualStyleBackColor = true;
            // 
            // rightLateralPterygoid
            // 
            this.rightLateralPterygoid.Appearance = System.Windows.Forms.Appearance.Button;
            this.rightLateralPterygoid.AutoSize = true;
            this.rightLateralPterygoid.Location = new System.Drawing.Point(2, 286);
            this.rightLateralPterygoid.Name = "rightLateralPterygoid";
            this.rightLateralPterygoid.Size = new System.Drawing.Size(96, 23);
            this.rightLateralPterygoid.TabIndex = 56;
            this.rightLateralPterygoid.Tag = "RightLateralPterygoidDynamic";
            this.rightLateralPterygoid.Text = "Lateral Pterygoid";
            this.rightLateralPterygoid.UseVisualStyleBackColor = true;
            // 
            // leftMedialPterygoid
            // 
            this.leftMedialPterygoid.Appearance = System.Windows.Forms.Appearance.Button;
            this.leftMedialPterygoid.AutoSize = true;
            this.leftMedialPterygoid.Location = new System.Drawing.Point(129, 235);
            this.leftMedialPterygoid.Name = "leftMedialPterygoid";
            this.leftMedialPterygoid.Size = new System.Drawing.Size(95, 23);
            this.leftMedialPterygoid.TabIndex = 55;
            this.leftMedialPterygoid.Tag = "LeftMedialPterygoidDynamic";
            this.leftMedialPterygoid.Text = "Medial Pterygoid";
            this.leftMedialPterygoid.UseVisualStyleBackColor = true;
            // 
            // rightMedialPterygoid
            // 
            this.rightMedialPterygoid.Appearance = System.Windows.Forms.Appearance.Button;
            this.rightMedialPterygoid.AutoSize = true;
            this.rightMedialPterygoid.Location = new System.Drawing.Point(1, 234);
            this.rightMedialPterygoid.Name = "rightMedialPterygoid";
            this.rightMedialPterygoid.Size = new System.Drawing.Size(95, 23);
            this.rightMedialPterygoid.TabIndex = 54;
            this.rightMedialPterygoid.Tag = "RightMedialPterygoidDynamic";
            this.rightMedialPterygoid.Text = "Medial Pterygoid";
            this.rightMedialPterygoid.UseVisualStyleBackColor = true;
            // 
            // leftTemporalis
            // 
            this.leftTemporalis.Appearance = System.Windows.Forms.Appearance.Button;
            this.leftTemporalis.AutoSize = true;
            this.leftTemporalis.Location = new System.Drawing.Point(129, 127);
            this.leftTemporalis.Name = "leftTemporalis";
            this.leftTemporalis.Size = new System.Drawing.Size(68, 23);
            this.leftTemporalis.TabIndex = 51;
            this.leftTemporalis.Tag = "LeftTemporalisDynamic";
            this.leftTemporalis.Text = "Temporalis";
            this.leftTemporalis.UseVisualStyleBackColor = true;
            // 
            // rightTemporalis
            // 
            this.rightTemporalis.Appearance = System.Windows.Forms.Appearance.Button;
            this.rightTemporalis.AutoSize = true;
            this.rightTemporalis.Location = new System.Drawing.Point(1, 127);
            this.rightTemporalis.Name = "rightTemporalis";
            this.rightTemporalis.Size = new System.Drawing.Size(68, 23);
            this.rightTemporalis.TabIndex = 48;
            this.rightTemporalis.Tag = "RightTemporalisDynamic";
            this.rightTemporalis.Text = "Temporalis";
            this.rightTemporalis.UseVisualStyleBackColor = true;
            // 
            // leftMasseter
            // 
            this.leftMasseter.Appearance = System.Windows.Forms.Appearance.Button;
            this.leftMasseter.AutoSize = true;
            this.leftMasseter.Location = new System.Drawing.Point(129, 181);
            this.leftMasseter.Name = "leftMasseter";
            this.leftMasseter.Size = new System.Drawing.Size(60, 23);
            this.leftMasseter.TabIndex = 47;
            this.leftMasseter.Tag = "LeftMasseterDynamic";
            this.leftMasseter.Text = "Masseter";
            this.leftMasseter.UseVisualStyleBackColor = true;
            // 
            // rightMasseter
            // 
            this.rightMasseter.Appearance = System.Windows.Forms.Appearance.Button;
            this.rightMasseter.AutoSize = true;
            this.rightMasseter.Location = new System.Drawing.Point(1, 180);
            this.rightMasseter.Name = "rightMasseter";
            this.rightMasseter.Size = new System.Drawing.Size(60, 23);
            this.rightMasseter.TabIndex = 46;
            this.rightMasseter.Tag = "RightMasseterDynamic";
            this.rightMasseter.Text = "Masseter";
            this.rightMasseter.UseVisualStyleBackColor = true;
            // 
            // MuscleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 421);
            this.Controls.Add(this.tabControl1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "MuscleControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Muscles";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.forceUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftDigastricForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightDigastricForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftLatPtForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightLatPtForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftMedPtForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightMedPtForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftMasseterForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightMasseterForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftTemporalisForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightTemporalisForce)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button clenchButton;
        private System.Windows.Forms.Button neutralButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox bothSides;
        private System.Windows.Forms.Button toggleMuscles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown forceUpDown;
        private System.Windows.Forms.ComboBox leftMuscleGroups;
        private System.Windows.Forms.ComboBox rightMuscleGroups;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Right;
        private System.Windows.Forms.NumericUpDown leftDigastricForce;
        private System.Windows.Forms.NumericUpDown rightDigastricForce;
        private System.Windows.Forms.NumericUpDown leftLatPtForce;
        private System.Windows.Forms.NumericUpDown rightLatPtForce;
        private System.Windows.Forms.NumericUpDown leftMedPtForce;
        private System.Windows.Forms.NumericUpDown rightMedPtForce;
        private System.Windows.Forms.NumericUpDown leftMasseterForce;
        private System.Windows.Forms.NumericUpDown rightMasseterForce;
        private System.Windows.Forms.NumericUpDown leftTemporalisForce;
        private System.Windows.Forms.NumericUpDown rightTemporalisForce;
        private System.Windows.Forms.CheckBox leftDigastric;
        private System.Windows.Forms.CheckBox rightDigastric;
        private System.Windows.Forms.CheckBox leftLateralPterygoid;
        private System.Windows.Forms.CheckBox rightLateralPterygoid;
        private System.Windows.Forms.CheckBox leftMedialPterygoid;
        private System.Windows.Forms.CheckBox rightMedialPterygoid;
        private System.Windows.Forms.CheckBox leftTemporalis;
        private System.Windows.Forms.CheckBox rightTemporalis;
        private System.Windows.Forms.CheckBox leftMasseter;
        private System.Windows.Forms.CheckBox rightMasseter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
    }
}
