using System;
using ProyectoMundial.Modelos;

namespace ProyectoMundial.Estructuras
{
    // Arbol B+ implementado desde cero, sin usar List<T>, Dictionary,
    // SortedDictionary ni ninguna coleccion nativa de .NET para su logica.
    // Se usa para indexar a los jugadores por su Id: permite buscar un
    // jugador especifico rapidamente, y ademas recorrer TODO el catalogo
    // ya ordenado (gracias al enlace entre hojas), sin tener que ordenar
    // nada por separado.
    public class ArbolBMas
    {
        // Orden del arbol: cada nodo interno tiene como maximo ORDEN hijos
        // y ORDEN-1 claves. Cada hoja guarda como maximo ORDEN-1 datos.
        // Con ORDEN=4 el arbol crece rapido con pocos datos, lo cual es
        // ideal para poder mostrar varios niveles en la demostracion.
        private const int ORDEN = 4;

        private NodoBMas raiz;

        public ArbolBMas()
        {
            raiz = new NodoBMas(esHoja: true, orden: ORDEN);
        }

        // ================= INSERTAR =================
        public void Insertar(Jugador jugador)
        {
            var resultado = InsertarEnNodo(raiz, jugador.Id, jugador);
            if (resultado != null)
            {
                // La raiz se dividio: se crea una raiz nueva con un hijo a cada lado.
                var nuevaRaiz = new NodoBMas(esHoja: false, orden: ORDEN);
                nuevaRaiz.Claves[0] = resultado.Value.clave;
                nuevaRaiz.Hijos![0] = raiz;
                nuevaRaiz.Hijos![1] = resultado.Value.nodo;
                nuevaRaiz.NumClaves = 1;
                raiz = nuevaRaiz;
            }
        }

        // Cuando un nodo se divide, devuelve la clave que "sube" un nivel
        // y el nuevo nodo hermano creado. Si no hubo division, devuelve null.
        private (string clave, NodoBMas nodo)? InsertarEnNodo(NodoBMas nodo, string clave, Jugador dato)
        {
            if (nodo.EsHoja)
            {
                InsertarEnHoja(nodo, clave, dato);
                if (nodo.NumClaves < ORDEN)
                    return null;
                return DividirHoja(nodo);
            }

            int pos = PosicionHijo(nodo, clave);
            var resultado = InsertarEnNodo(nodo.Hijos![pos], clave, dato);
            if (resultado == null)
                return null;

            InsertarEnInterno(nodo, resultado.Value.clave, resultado.Value.nodo, pos);
            if (nodo.NumClaves < ORDEN)
                return null;
            return DividirInterno(nodo);
        }

        // Decide por cual hijo continuar segun la clave (regla: si la clave
        // es mayor o igual que la clave guia, se va por la derecha).
        private int PosicionHijo(NodoBMas nodo, string clave)
        {
            int i = 0;
            while (i < nodo.NumClaves && string.CompareOrdinal(clave, nodo.Claves[i]) >= 0)
                i++;
            return i;
        }

        private void InsertarEnHoja(NodoBMas hoja, string clave, Jugador dato)
        {
            int i = hoja.NumClaves - 1;
            while (i >= 0 && string.CompareOrdinal(hoja.Claves[i], clave) > 0)
            {
                hoja.Claves[i + 1] = hoja.Claves[i];
                hoja.Datos![i + 1] = hoja.Datos![i];
                i--;
            }
            hoja.Claves[i + 1] = clave;
            hoja.Datos![i + 1] = dato;
            hoja.NumClaves++;
        }

        private (string clave, NodoBMas nodo) DividirHoja(NodoBMas hoja)
        {
            var nuevaHoja = new NodoBMas(esHoja: true, orden: ORDEN);
            int mitad = hoja.NumClaves / 2;

            nuevaHoja.NumClaves = hoja.NumClaves - mitad;
            for (int i = 0; i < nuevaHoja.NumClaves; i++)
            {
                nuevaHoja.Claves[i] = hoja.Claves[mitad + i];
                nuevaHoja.Datos![i] = hoja.Datos![mitad + i];
            }
            hoja.NumClaves = mitad;

            nuevaHoja.Siguiente = hoja.Siguiente;
            hoja.Siguiente = nuevaHoja;

            // La clave que sube es una COPIA de la primera clave de la hoja
            // nueva. El dato real se queda unicamente en la hoja.
            return (nuevaHoja.Claves[0], nuevaHoja);
        }

        private void InsertarEnInterno(NodoBMas nodo, string clave, NodoBMas hijoNuevo, int posHijo)
        {
            for (int i = nodo.NumClaves - 1; i >= posHijo; i--)
                nodo.Claves[i + 1] = nodo.Claves[i];
            for (int i = nodo.NumClaves; i >= posHijo + 1; i--)
                nodo.Hijos![i + 1] = nodo.Hijos![i];

            nodo.Claves[posHijo] = clave;
            nodo.Hijos![posHijo + 1] = hijoNuevo;
            nodo.NumClaves++;
        }

        private (string clave, NodoBMas nodo) DividirInterno(NodoBMas nodo)
        {
            var nuevoNodo = new NodoBMas(esHoja: false, orden: ORDEN);
            int mitad = nodo.NumClaves / 2;
            string clavePromovida = nodo.Claves[mitad];

            nuevoNodo.NumClaves = nodo.NumClaves - mitad - 1;
            for (int i = 0; i < nuevoNodo.NumClaves; i++)
                nuevoNodo.Claves[i] = nodo.Claves[mitad + 1 + i];
            for (int i = 0; i <= nuevoNodo.NumClaves; i++)
                nuevoNodo.Hijos![i] = nodo.Hijos![mitad + 1 + i];

            nodo.NumClaves = mitad;

            // Esta clave NO se duplica hacia abajo: en un nodo interno solo
            // sirve para guiar, no representa un dato real.
            return (clavePromovida, nuevoNodo);
        }

        // ================= BUSCAR =================
        public Jugador? Buscar(string id)
        {
            var actual = raiz;
            while (!actual.EsHoja)
                actual = actual.Hijos![PosicionHijo(actual, id)];

            for (int i = 0; i < actual.NumClaves; i++)
                if (actual.Claves[i] == id)
                    return actual.Datos![i];

            return null;
        }

        // ================= RECORRER =================
        // Recorre todas las hojas de izquierda a derecha (gracias al enlace
        // entre ellas), devolviendo los jugadores ya ordenados por Id.
        public Jugador[] Recorrer()
        {
            var hoja = raiz;
            while (!hoja.EsHoja)
                hoja = hoja.Hijos![0];

            int total = 0;
            for (var h = hoja; h != null; h = h.Siguiente)
                total += h.NumClaves;

            var resultado = new Jugador[total];
            int idx = 0;
            for (var h = hoja; h != null; h = h.Siguiente)
                for (int i = 0; i < h.NumClaves; i++)
                    resultado[idx++] = h.Datos![i];

            return resultado;
        }

        // ================= IMPRIMIR =================
        public void Imprimir()
        {
            Console.WriteLine("Catalogo ordenado por Id:");
            foreach (var jugador in Recorrer())
                Console.WriteLine("  " + jugador);
        }

        // Util para ver visualmente los niveles del arbol (solo para depurar / defender).
        public void ImprimirEstructura()
        {
            ImprimirNodo(raiz, 0);
        }

        private void ImprimirNodo(NodoBMas nodo, int nivel)
        {
            string sangria = new string(' ', nivel * 4);
            string tipo = nodo.EsHoja ? "HOJA" : "INTERNO";
            Console.WriteLine($"{sangria}[{tipo}] claves: {string.Join(",", nodo.Claves[..nodo.NumClaves])}");

            if (!nodo.EsHoja)
                for (int i = 0; i <= nodo.NumClaves; i++)
                    ImprimirNodo(nodo.Hijos![i], nivel + 1);
        }
    }
}
