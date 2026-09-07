using System;
using ProyectoMundial.Modelos;
using ProyectoMundial.Estructuras;

namespace ProyectoMundial.Datos
{
    // Punto central del programa. Guarda un unico Arbol B+ con todos los
    // jugadores (esa es la "fuente de la verdad"), y arma los Max/Min heap
    // "al vuelo" cuando se piden rankings. La interfaz grafica solo habla
    // con esta clase: nunca toca el Arbol B+ ni los heaps directamente.
    public class GestorJugadores
    {
        private readonly ArbolBMas indice = new ArbolBMas();

        // Devuelve false si ya existia un jugador con ese Id (no se duplica).
        public bool Registrar(Jugador jugador)
        {
            if (indice.Buscar(jugador.Id) != null)
                return false;
            indice.Insertar(jugador);
            return true;
        }

        public Jugador? Buscar(string id) => indice.Buscar(id);

        public bool Eliminar(string id) => indice.Eliminar(id);

        // Todos los jugadores ordenados por Id (recorrido de hojas del Arbol B+).
        public Jugador[] ObtenerTodos() => indice.Recorrer();

        // Listado completo, ordenado de mayor a menor en la categoria elegida.
        // Se arma un Max Heap temporal con los datos actuales y se extrae todo (heap sort).
        public Jugador[] ObtenerListadoPorCategoria(Func<Jugador, int> categoria)
        {
            var jugadores = indice.Recorrer();
            var heap = new MaxHeap(jugadores.Length == 0 ? 1 : jugadores.Length);
            foreach (var jugador in jugadores)
                heap.Insertar(categoria(jugador), jugador);
            return heap.ObtenerTodosOrdenadosDescendente();
        }

        // Top N de una categoria, usando el Min Heap acotado (mas eficiente
        // que ordenar todo el catalogo cuando solo interesan los primeros N).
        public Jugador[] ObtenerTopN(Func<Jugador, int> categoria, int n)
        {
            return MinHeap.ObtenerTopN(indice.Recorrer(), categoria, n);
        }

        // Carga jugadores desde un .csv. Si un Id ya existe, ese jugador se
        // omite (no se duplica). Devuelve cuantos se agregaron realmente.
        public int CargarDesdeCsv(string ruta)
        {
            int agregados = 0;
            foreach (var jugador in GestorArchivos.Cargar(ruta))
                if (Registrar(jugador))
                    agregados++;
            return agregados;
        }

        public void GuardarEnCsv(string ruta)
        {
            GestorArchivos.Guardar(ruta, ObtenerTodos());
        }
    }
}
