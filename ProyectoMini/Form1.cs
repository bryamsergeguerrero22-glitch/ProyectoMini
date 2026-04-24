using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Windows.Forms;

namespace ProyectoMini
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Configurar la cadena de conexión y conectar a MongoDB
                string connectionString = "mongodb://localhost:27017/";// Asegúrate de que esta cadena de conexión sea correcta para tu entorno
                var client = new MongoClient(connectionString);// Crea una instancia del cliente de MongoDB
                var database = client.GetDatabase("Base_de_datos_Reunion");// Asegúrate de que el nombre de la base de datos sea correcto
                var collection = database.GetCollection<BsonDocument>("Usuario");// Asegúrate de que el nombre de la colección sea correcto

                // 2. Obtener los valores de los cuadros de texto
                // IMPORTANTE: Cambia "txtID" y "txtPassword" por los nombres reales de tus TextBox en el diseño
                int idIngresado = int.Parse(txtID.Text);
                int passIngresada = int.Parse(txtPassword.Text);

                // 3. Crear el filtro para buscar coincidencias exactas de ID y Contraseña
                // Usamos "_id.ID" porque en tu base de datos el ID está dentro del objeto _id
                var filtro = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("_id.ID", idIngresado),
                    Builders<BsonDocument>.Filter.Eq("Contraseña", passIngresada)
                );

                // 4. Ejecutar la búsqueda en la colección
                var usuarioEncontrado = collection.Find(filtro).FirstOrDefault();

                // 5. Validar el resultado del inicio de sesión
                if (usuarioEncontrado != null)
                {
                    // Si entra aquí, las credenciales son correctas
                    string nombres = usuarioEncontrado["Nombres"].AsString;
                    string tipoUsuario = usuarioEncontrado["Tipo_Usuario"].AsString;

                    MessageBox.Show($"¡Bienvenido {nombres}!\nHas ingresado como: {tipoUsuario}",
                                    "Acceso Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (tipoUsuario == "Lider")
                    {
                        // Pasamos nombre y rol al formulario Lider
                        Lider ventanaLider = new Lider(nombres, tipoUsuario);
                        ventanaLider.Show();
                    }
                    else if (tipoUsuario == "Investigador")
                    {
                        // Pasamos nombre y rol al formulario Investigador
                        Investigador ventanaInv = new Investigador(nombres, tipoUsuario);
                        ventanaInv.Show();
                    }

                    this.Hide();             // Oculta el formulario de login actual
                }
                else
                {
                    // Si entra aquí, no encontró a nadie con ese ID y Contraseña
                    MessageBox.Show("El ID o la contraseña son incorrectos.",
                                    "Error de Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                // Este error salta si el usuario escribe letras en lugar de números
                MessageBox.Show("Por favor, ingresa únicamente números en los campos de ID y Contraseña.",
                                "Datos inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // Este error captura problemas de conexión, base de datos caída, etc.
                MessageBox.Show("Error al intentar conectar con la base de datos:\n" + ex.Message,
                                "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Bloquea si NO es número Y NO es una tecla de control (como borrar)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Por favor solo ingrese números", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Bloquea si NO es número Y NO es una tecla de control (como borrar)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Por favor solo ingrese números", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}