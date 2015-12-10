using System;
using System.Drawing;
using System.Windows.Forms;

namespace TCDefectIntegration {
    public partial class frmLogin : Form {
        public frmLogin() {
            InitializeComponent();
        }

        private void frmLogin_Load( object sender, EventArgs e ) {
            this.BringToFront();
            this.TopMost = true;
            this.TopMost = false;
            this.Activate();
            this.MaximumSize = new Size(int.MaxValue, this.Height);
        }

        private void cmdOK_Click( object sender, EventArgs e ) {
            this.DialogResult = DialogResult.OK;
        }

        private void tbLogin_TextChanged( object sender, EventArgs e ) {
        }

        private void textBox2_TextChanged( object sender, EventArgs e ) {
        }

        private void label2_Click( object sender, EventArgs e ) {
        }

        private void textBox1_TextChanged( object sender, EventArgs e ) {
        }

        private void label1_Click( object sender, EventArgs e ) {
        }

        private void lblLogin_Click( object sender, EventArgs e ) {
        }
    }
}