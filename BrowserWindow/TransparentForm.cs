using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TCDefectIntegration.BrowserWindow
{

    public class TransparentForm : Form
    {

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int WM_NCHITTEST = 0x84;
        public const int HTTRANSPARENT = -1;
        private const int SW_SHOWNOACTIVATE = 4;

        public TransparentForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Color.Green;
            TransparencyKey = Color.Green;
            Opacity = 0.2f;
        }

        public void ShowWithoutActivate()
        {
            // Show the window without activating it (i.e. do not take focus)
            ShowWindow(this.Handle, (short)SW_SHOWNOACTIVATE);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            // Do your painting here be exclude the area you want to be brighter
            pe.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, SystemColors.Control)), this.ClientRectangle);
        }        


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WM_NCHITTEST)
                m.Result = (IntPtr)HTTRANSPARENT;
            else
                base.WndProc(ref m);
        }
    }

    
}
