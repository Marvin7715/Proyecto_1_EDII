using System;
using System.IO;
using ProyectoMundial.Modelos;

namespace ProyectoMundial.Datos
{
    // Unica responsabilidad: convertir el archivo .csv en Jugador[] y
    // viceversa. No sabe nada de arboles ni heaps.
    public static class GestorArchivos
    {
        public static Jugador[] Cargar(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
                return Array.Empty<Jugador>();

            var lineas = File.ReadAllLines(rutaArchivo);
            var resultado = new Jugador[lineas.Length]; // como maximo, una por linea
            int idx = 0;

            // La linea 0 es el encabezado (Id,Nombre,Seleccion,...), se salta.
            for (int i = 1; i < lineas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lineas[i])) continue;
                resultado[idx++] = Jugador.DesdeLineaCsv(lineas[i]);
            }

            if (idx != resultado.Length)
                Array.Resize(ref resultado, idx);

            return resultado;
        }

        public static void Guardar(string rutaArchivo, Jugador[] jugadores)
        {
            using var escritor = new StreamWriter(rutaArchivo, append: false);
            escritor.WriteLine("Id,Nombre,Seleccion,Posicion,MinutosJugados,Goles,Asistencias,TarjetasAmarillas,TarjetasRojas,PartidosJugados");
            foreach (var jugador in jugadores)
                escritor.WriteLine(jugador.ALineaCsv());
        }
    }
}
