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
    public partial class Lider : Form
    {
        IMongoDatabase database;
        IMongoCollection<BsonDocument> usuariosCollection;
        public Lider(string nombreRecibido, string rolRecibido)
        {
            InitializeComponent();
            lblNombreUsuario.Text =  nombreRecibido;
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

        private void Lider_Load(object sender, EventArgs e)
        {
            var client = new MongoClient("mongodb://localhost:27017/");
            database = client.GetDatabase("Base_de_datos_Reunion"); // El nombre de tu BD
            usuariosCollection = database.GetCollection<BsonDocument>("Usuario");
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string usuarioLogueado = lblNombreUsuario.Text;

                // 1. Definimos la colección de reuniones para empezar desde ahí
                var reunionesCollection = database.GetCollection<BsonDocument>("Reunion");

                var pipeline = new[]
                {
    // Paso 1: Buscar en la colección de USUARIOS quién tiene el nombre logueado
    // para obtener su ID de reunión.
    new BsonDocument("$lookup", new BsonDocument
    {
        { "from", "Usuario" },
        { "localField", "_id" }, // _id de la Reunión
        { "foreignField", "ID" }, // Campo ID en Usuario que apunta a la reunión
        { "as", "TodosLosAsistentes" }
    }),

    // Paso 2: Filtrar para que solo aparezcan las reuniones donde Jeremías (el logueado) es parte
    new BsonDocument("$match", new BsonDocument("TodosLosAsistentes.Nombres", usuarioLogueado)),

    // Paso 3: Proyectar los datos finales
    new BsonDocument("$project", new BsonDocument
    {
        { "ID_Reunion", "$_id" },
        { "Fecha", "$fechaReunion" },
        { "Lugar", "$Lugar" },
        // Aquí usamos $reduce o una transformación para unir los nombres en un solo string
        { "Asistentes", new BsonDocument("$reduce", new BsonDocument
            {
                { "input", "$TodosLosAsistentes.Nombres" },
                { "initialValue", "" },
                { "in", new BsonDocument("$concat", new BsonArray
                    {
                        "$$value",
                        new BsonDocument("$cond", new BsonArray { new BsonDocument("$eq", new BsonArray { "$$value", "" }), "", ", " }),
                        "$$this"
                    })
                }
            })
        },
        { "_id", 0 }
    })
};

                // Ejecutar en la colección de Reuniones
                var resultados = reunionesCollection.Aggregate<BsonDocument>(pipeline).ToList();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("Lugar");
                dt.Columns.Add("Asistentes");

                foreach (var doc in resultados)
                {
                    dt.Rows.Add(
                        doc["ID_Reunion"].ToString(),
                        doc["Fecha"].ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                        doc["Lugar"].ToString(),
                        doc["Asistentes"].ToString()
                    );
                }

                guna2DataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar: " + ex.Message);
            }
        }

        private void btnsalir_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿ Desea salir ?", "mensaje importante", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                Application.Exit();
            }
        }
    }
    
}
