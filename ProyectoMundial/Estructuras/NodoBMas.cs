using ProyectoMundial.Modelos;

namespace ProyectoMundial.Estructuras
{
    // Nodo del Arbol B+. Puede ser un nodo interno (solo sirve para guiar la
    // busqueda, no guarda datos reales) o una hoja (guarda los Jugador de
    // verdad y esta enlazada con la siguiente hoja para poder recorrer todo
    // el catalogo en orden sin volver a subir al nodo interno).
    public class NodoBMas
    {
        public bool EsHoja;
        public int NumClaves;

        // Los arreglos se crean con un espacio EXTRA (tamano = orden) para
        // poder insertar temporalmente antes de dividir el nodo. Nunca se usa
        // List<T> ni ninguna coleccion nativa: son arreglos de tamano fijo.
        public string[] Claves;
        public Jugador[]? Datos;      // solo se usa si EsHoja == true
        public NodoBMas[]? Hijos;     // solo se usa si EsHoja == false
        public NodoBMas? Siguiente;   // solo se usa si EsHoja == true

        public NodoBMas(bool esHoja, int orden)
        {
            EsHoja = esHoja;
            NumClaves = 0;
            Claves = new string[orden];

            if (esHoja)
                Datos = new Jugador[orden];
            else
                Hijos = new NodoBMas[orden + 1];
        }
    }
}
