using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoMini
{
    public partial class Investigador : Form
    {
        public Investigador(string nombreRecibido, string rolRecibido)
        {
            InitializeComponent();
            lblNombreUsuario.Text = nombreRecibido;
            lblRolUsuario.Text = "Rol " + rolRecibido;
        }

        private void btnsalir_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿ desea salir ?", "mensaje importante", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                Application.Exit();
            }
        }
    }
}