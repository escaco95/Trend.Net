using System.Drawing;
using Trend;

namespace TrendDemo
{
    class MainWindow : Trend.Window
    {

        public MainWindow()
        {
            InitializeComponent();
            //
            //
            //
            this.WindowAuras.Add(new WindowAuras.Line(Color.Red));
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Name = "MainWindow";
            this.ResumeLayout(false);

        }
    }
}
