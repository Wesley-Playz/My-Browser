namespace CSharpBrowser
{
    partial class Form1
    {
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnBack;
        private System.Windows.Forms.ToolStripButton btnForward;
        private System.Windows.Forms.ToolStripButton btnReload;
        private System.Windows.Forms.ToolStripButton btnNewTab;
        private System.Windows.Forms.ToolStripTextBox urlTextBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolStripButton btnCloseTab;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            toolStrip1 = new ToolStrip();
            btnBack = new ToolStripButton();
            btnForward = new ToolStripButton();
            btnReload = new ToolStripButton();
            btnNewTab = new ToolStripButton();
            btnCloseTab = new ToolStripButton();
            urlTextBox = new ToolStripTextBox();
            tabControl1 = new TabControl();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(28, 28);
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnBack, btnForward, btnReload, btnNewTab, btnCloseTab, urlTextBox });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1000, 40);
            toolStrip1.TabIndex = 1;
            // 
            // btnBack
            // 
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(60, 34);
            btnBack.Text = "Back";
            // 
            // btnForward
            // 
            btnForward.Name = "btnForward";
            btnForward.Size = new Size(91, 34);
            btnForward.Text = "Forward";
            // 
            // btnReload
            // 
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(80, 34);
            btnReload.Text = "Reload";
            // 
            // btnNewTab
            // 
            btnNewTab.Name = "btnNewTab";
            btnNewTab.Size = new Size(97, 34);
            btnNewTab.Text = "New Tab";
            // 
            // btnCloseTab
            // 
            btnCloseTab.Name = "btnCloseTab";
            btnCloseTab.Size = new Size(105, 34);
            btnCloseTab.Text = "Close Tab";
            // 
            // urlTextBox
            // 
            urlTextBox.Dock = DockStyle.Fill;
            urlTextBox.Name = "urlTextBox";
            urlTextBox.Size = new Size(500, 40);
            // 
            // tabControl1
            // 
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 40);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1000, 664);
            tabControl1.TabIndex = 0;
            // 
            // Form1
            // 
            ClientSize = new Size(1000, 704);
            Controls.Add(tabControl1);
            Controls.Add(toolStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "My Browser";
            Load += Form1_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
