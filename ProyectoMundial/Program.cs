using System;
using System.Windows.Forms;
using ProyectoMundial.Formularios;

namespace ProyectoMundial
{
    internal static class Program
    {
        // Punto de entrada del programa. Todo arranca aqui.
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormPrincipal());
        }
    }
}
