namespace Ornamental
{
    partial class OrnamentalForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            corePanel = new Panel();
            SuspendLayout();
            // 
            // corePanel
            // 
            corePanel.Dock = DockStyle.Fill;
            corePanel.Location = new Point(0, 0);
            corePanel.Margin = new Padding(2);
            corePanel.Name = "corePanel";
            corePanel.Size = new Size(867, 554);
            corePanel.TabIndex = 0;
            // 
            // OrnamentalForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(867, 554);
            Controls.Add(corePanel);
            DoubleBuffered = true;
            Margin = new Padding(2, 2, 2, 2);
            Name = "OrnamentalForm";
            Text = "Ornamental";
            ResumeLayout(false);
        }

        #endregion

        private Panel corePanel;
    }
}
