using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProyectoMundial.Formularios
{
    // Ventana principal del programa. Por ahora solo confirma que el proyecto
    // compila y corre. En la siguiente etapa se convertira en el menu real
    // (registro, busqueda, rankings, etc).
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Tabla de posiciones - Mundial";
            this.Width = 1000;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 600);

            var lblBienvenida = new Label
            {
                Text = "Etapa 1 completa: el proyecto compila y corre correctamente.\n\n" +
                       "Aqui construiremos el menu real: registro de jugadores,\n" +
                       "busqueda, y los rankings usando el Arbol B+, el Max Heap y el Min Heap.",
                AutoSize = true,
                Left = 40,
                Top = 40,
                Font = new Font("Segoe UI", 12F)
            };

            this.Controls.Add(lblBienvenida);
        }
    }
}
