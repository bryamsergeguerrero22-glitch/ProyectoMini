using MongoDB.Bson;
using MongoDB.Driver;
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
        IMongoDatabase database;
        IMongoCollection<BsonDocument> usuariosCollection;
        public Investigador(string nombreRecibido, string rolRecibido)
        {
            InitializeComponent();
            lblNombreUsuario.Text = nombreRecibido;
            lblRolUsuario.Text = "Rol " + rolRecibido;
        }

        private void btnsalir_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea cerrar sesión?", "Mensaje Importante", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void Investigador_Load(object sender, EventArgs e)
        {
            // Reemplaza con tu cadena de conexión si es distinta
            var client = new MongoClient("mongodb://localhost:27017/");
            database = client.GetDatabase("Base_de_datos_Reunion"); // El nombre de tu BD
            usuariosCollection = database.GetCollection<BsonDocument>("Usuario");
        }

        private void gunabtnConsultarReunion_Click(object sender, EventArgs e)
        {
            try
            {
                string usuarioLogueado = lblNombreUsuario.Text;

                var pipeline = new[]
                {
    // Ahora el filtro es dinámico: buscará al usuario que tenga el nombre del label
    new BsonDocument("$match", new BsonDocument("Nombres", usuarioLogueado)),

    new BsonDocument("$lookup", new BsonDocument
    {
        { "from", "Reunion" },
        { "localField", "ID" },
        { "foreignField", "_id" },
        { "as", "Reunion_info" }
    }),

    new BsonDocument("$unwind", "$Reunion_info"),

    new BsonDocument("$project", new BsonDocument
    {
        { "ID_Reunion", "$Reunion_info._id" },
        { "Fecha", "$Reunion_info.fechaReunion" },
        { "Lugar", "$Reunion_info.Lugar" },
        { "_id", 0 }
    })
};


                // 2. Ejecutamos la agregación
                var resultados = usuariosCollection.Aggregate<BsonDocument>(pipeline).ToList();

                // 3. Pasamos los datos al DataGridView usando un DataTable
                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("Lugar");

                foreach (var doc in resultados)
                {
                    dt.Rows.Add(
                        doc["ID_Reunion"].ToString(),
                        doc["Fecha"].ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                        doc["Lugar"].ToString()
                    );
                }

                // 4. Asignamos el DataTable al DataGridView
                // Cambia 'dgvReuniones' por el nombre real de tu control Guna
                guna2DataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar: " + ex.Message);
            }
        }
    }
}