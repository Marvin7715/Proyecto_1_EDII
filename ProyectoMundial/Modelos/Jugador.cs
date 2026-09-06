namespace ProyectoMundial.Modelos
{
    // Representa un jugador del Mundial con sus datos y estadisticas acumuladas.
    // Esta es la unidad de informacion que viajara dentro del Arbol B+ y de los heaps.
    public class Jugador
    {
        public string Id { get; set; } = "";           // clave unica -> la usara el Arbol B+ para indexar
        public string Nombre { get; set; } = "";
        public string Seleccion { get; set; } = "";     // pais / equipo
        public string Posicion { get; set; } = "";      // Portero, Defensa, Mediocampista, Delantero

        public int MinutosJugados { get; set; }
        public int Goles { get; set; }
        public int Asistencias { get; set; }
        public int TarjetasAmarillas { get; set; }
        public int TarjetasRojas { get; set; }
        public int PartidosJugados { get; set; }

        public Jugador() { }

        public Jugador(string id, string nombre, string seleccion, string posicion,
                        int minutosJugados, int goles, int asistencias,
                        int tarjetasAmarillas, int tarjetasRojas, int partidosJugados)
        {
            Id = id;
            Nombre = nombre;
            Seleccion = seleccion;
            Posicion = posicion;
            MinutosJugados = minutosJugados;
            Goles = goles;
            Asistencias = asistencias;
            TarjetasAmarillas = tarjetasAmarillas;
            TarjetasRojas = tarjetasRojas;
            PartidosJugados = partidosJugados;
        }

        // Util para mostrar rapido en consola, listas o el manual tecnico.
        public override string ToString()
        {
            return $"{Id} | {Nombre} ({Seleccion}, {Posicion}) - Goles: {Goles}, Asist: {Asistencias}, Min: {MinutosJugados}";
        }

        // Convierte el jugador a una linea de texto separada por comas, para guardarlo en el .csv
        public string ALineaCsv()
        {
            return string.Join(",", Id, Nombre, Seleccion, Posicion,
                MinutosJugados, Goles, Asistencias, TarjetasAmarillas, TarjetasRojas, PartidosJugados);
        }

        // Reconstruye un Jugador a partir de una linea leida del .csv
        public static Jugador DesdeLineaCsv(string linea)
        {
            var partes = linea.Split(',');
            return new Jugador(
                id: partes[0].Trim(),
                nombre: partes[1].Trim(),
                seleccion: partes[2].Trim(),
                posicion: partes[3].Trim(),
                minutosJugados: int.Parse(partes[4]),
                goles: int.Parse(partes[5]),
                asistencias: int.Parse(partes[6]),
                tarjetasAmarillas: int.Parse(partes[7]),
                tarjetasRojas: int.Parse(partes[8]),
                partidosJugados: int.Parse(partes[9])
            );
        }
    }
}
