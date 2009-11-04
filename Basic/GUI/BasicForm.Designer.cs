namespace Medical.GUI
{
    partial class BasicForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
            WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin1 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient1 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient2 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient3 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient4 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient5 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient6 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient7 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasicForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mainStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.clinicalRibbon = new ComponentFactory.Krypton.Ribbon.KryptonRibbon();
            this.kryptonManager = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            this.navigationTab = new ComponentFactory.Krypton.Ribbon.KryptonRibbonTab();
            this.layersTab = new ComponentFactory.Krypton.Ribbon.KryptonRibbonTab();
            this.changeSceneMenuItem = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.exitMenuItem = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonRibbonGroup1 = new ComponentFactory.Krypton.Ribbon.KryptonRibbonGroup();
            this.kryptonRibbonGroupTriple1 = new ComponentFactory.Krypton.Ribbon.KryptonRibbonGroupTriple();
            this.showNavigationButton = new ComponentFactory.Krypton.Ribbon.KryptonRibbonGroupButton();
            this.saveMenuItem = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.openMenuItem = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clinicalRibbon)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 492);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(711, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mainStatusLabel
            // 
            this.mainStatusLabel.Name = "mainStatusLabel";
            this.mainStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.mainStatusLabel.Text = "Ready";
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.SystemColors.Control;
            this.dockPanel.DockBottomPortion = 100;
            this.dockPanel.DockLeftPortion = 275;
            this.dockPanel.DockRightPortion = 225;
            this.dockPanel.DockTopPortion = 100;
            this.dockPanel.Location = new System.Drawing.Point(0, 143);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(711, 349);
            dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
            autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
            tabGradient1.EndColor = System.Drawing.SystemColors.Control;
            tabGradient1.StartColor = System.Drawing.SystemColors.Control;
            tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            autoHideStripSkin1.TabGradient = tabGradient1;
            dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
            tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
            dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
            dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
            dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
            tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
            dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
            tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
            tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
            tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
            dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
            tabGradient5.EndColor = System.Drawing.SystemColors.Control;
            tabGradient5.StartColor = System.Drawing.SystemColors.Control;
            tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
            dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
            tabGradient6.EndColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
            tabGradient7.EndColor = System.Drawing.Color.Transparent;
            tabGradient7.StartColor = System.Drawing.Color.Transparent;
            tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
            dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
            dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
            this.dockPanel.Skin = dockPanelSkin1;
            this.dockPanel.TabIndex = 12;
            // 
            // clinicalRibbon
            // 
            this.clinicalRibbon.Name = "clinicalRibbon";
            this.clinicalRibbon.RibbonAppButton.AppButtonImage = global::Medical.Properties.Resources.skull;
            this.clinicalRibbon.RibbonAppButton.AppButtonMenuItems.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.changeSceneMenuItem,
            this.openMenuItem,
            this.saveMenuItem,
            this.exitMenuItem});
            this.clinicalRibbon.RibbonAppButton.AppButtonShowRecentDocs = false;
            this.clinicalRibbon.RibbonTabs.AddRange(new ComponentFactory.Krypton.Ribbon.KryptonRibbonTab[] {
            this.navigationTab,
            this.layersTab});
            this.clinicalRibbon.SelectedTab = this.layersTab;
            this.clinicalRibbon.Size = new System.Drawing.Size(711, 143);
            this.clinicalRibbon.TabIndex = 15;
            // 
            // kryptonManager
            // 
            this.kryptonManager.GlobalPaletteMode = ComponentFactory.Krypton.Toolkit.PaletteModeManager.ProfessionalSystem;
            // 
            // navigationTab
            // 
            this.navigationTab.Groups.AddRange(new ComponentFactory.Krypton.Ribbon.KryptonRibbonGroup[] {
            this.kryptonRibbonGroup1});
            this.navigationTab.Text = "Navigation";
            // 
            // layersTab
            // 
            this.layersTab.Text = "Layers";
            // 
            // changeSceneMenuItem
            // 
            this.changeSceneMenuItem.Text = "Change Scene";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Text = "Exit";
            // 
            // kryptonRibbonGroup1
            // 
            this.kryptonRibbonGroup1.Items.AddRange(new ComponentFactory.Krypton.Ribbon.KryptonRibbonGroupContainer[] {
            this.kryptonRibbonGroupTriple1});
            this.kryptonRibbonGroup1.TextLine1 = "On Screen";
            // 
            // kryptonRibbonGroupTriple1
            // 
            this.kryptonRibbonGroupTriple1.Items.AddRange(new ComponentFactory.Krypton.Ribbon.KryptonRibbonGroupItem[] {
            this.showNavigationButton});
            // 
            // showNavigationButton
            // 
            this.showNavigationButton.ButtonType = ComponentFactory.Krypton.Ribbon.GroupButtonType.Check;
            this.showNavigationButton.ImageLarge = global::Medical.Properties.Resources.NavIcon;
            this.showNavigationButton.ImageSmall = global::Medical.Properties.Resources.NavIcon;
            this.showNavigationButton.TextLine1 = "Show Navigation";
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Text = "Save";
            // 
            // openMenuItem
            // 
            this.openMenuItem.Text = "Open";
            // 
            // BasicForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 514);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.clinicalRibbon);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "BasicForm";
            this.Text = "Articulometics Clinical";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clinicalRibbon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStripStatusLabel mainStatusLabel;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbon clinicalRibbon;
        private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonTab navigationTab;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonTab layersTab;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem changeSceneMenuItem;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem exitMenuItem;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonGroup kryptonRibbonGroup1;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonGroupTriple kryptonRibbonGroupTriple1;
        private ComponentFactory.Krypton.Ribbon.KryptonRibbonGroupButton showNavigationButton;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem openMenuItem;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem saveMenuItem;
    }
}