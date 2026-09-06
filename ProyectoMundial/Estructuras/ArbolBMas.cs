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

        // ================= ELIMINAR =================
        // Cantidad minima de claves que debe conservar un nodo (que no sea la
        // raiz) despues de eliminar. Si un nodo queda por debajo de este
        // minimo, se le "pide prestada" una clave a un hermano, o si ningun
        // hermano puede prestar sin quedar tambien corto, se fusiona con uno.
        private static readonly int MIN_HOJA = (int)System.Math.Ceiling((ORDEN - 1) / 2.0);
        private static readonly int MIN_INTERNO = (int)System.Math.Ceiling(ORDEN / 2.0) - 1;

        public bool Eliminar(string id)
        {
            bool eliminado = EliminarRec(raiz, id);

            // Si la raiz es un nodo interno que se quedo sin claves, su unico
            // hijo restante pasa a ser la nueva raiz (el arbol "encoge").
            if (eliminado && !raiz.EsHoja && raiz.NumClaves == 0)
                raiz = raiz.Hijos![0];

            return eliminado;
        }

        private bool EliminarRec(NodoBMas nodo, string clave)
        {
            if (nodo.EsHoja)
                return EliminarDeHoja(nodo, clave);

            int pos = PosicionHijo(nodo, clave);
            bool eliminado = EliminarRec(nodo.Hijos![pos], clave);
            if (!eliminado)
                return false;

            var hijo = nodo.Hijos![pos];
            int minimo = hijo.EsHoja ? MIN_HOJA : MIN_INTERNO;
            if (hijo.NumClaves < minimo)
                Rebalancear(nodo, pos);

            return true;
        }

        private bool EliminarDeHoja(NodoBMas hoja, string clave)
        {
            int idx = -1;
            for (int i = 0; i < hoja.NumClaves; i++)
                if (hoja.Claves[i] == clave) { idx = i; break; }
            if (idx == -1)
                return false; // no existia esa clave

            for (int i = idx; i < hoja.NumClaves - 1; i++)
            {
                hoja.Claves[i] = hoja.Claves[i + 1];
                hoja.Datos![i] = hoja.Datos![i + 1];
            }
            hoja.NumClaves--;
            return true;
        }

        // Decide si conviene pedir prestado a un hermano o fusionarse con uno.
        private void Rebalancear(NodoBMas nodo, int pos)
        {
            var hijo = nodo.Hijos![pos];
            int minimo = hijo.EsHoja ? MIN_HOJA : MIN_INTERNO;

            bool hayIzquierdo = pos > 0;
            bool hayDerecho = pos < nodo.NumClaves;

            if (hayIzquierdo && nodo.Hijos![pos - 1].NumClaves > minimo)
                PrestarDeIzquierda(nodo, pos);
            else if (hayDerecho && nodo.Hijos![pos + 1].NumClaves > minimo)
                PrestarDeDerecha(nodo, pos);
            else if (hayIzquierdo)
                Fusionar(nodo, pos - 1);
            else if (hayDerecho)
                Fusionar(nodo, pos);
        }

        private void PrestarDeIzquierda(NodoBMas nodo, int pos)
        {
            var hijo = nodo.Hijos![pos];
            var izq = nodo.Hijos![pos - 1];

            if (hijo.EsHoja)
            {
                for (int i = hijo.NumClaves; i > 0; i--)
                {
                    hijo.Claves[i] = hijo.Claves[i - 1];
                    hijo.Datos![i] = hijo.Datos![i - 1];
                }
                hijo.Claves[0] = izq.Claves[izq.NumClaves - 1];
                hijo.Datos![0] = izq.Datos![izq.NumClaves - 1];
                hijo.NumClaves++;
                izq.NumClaves--;

                // El separador debe reflejar siempre la primera clave real de la hoja derecha.
                nodo.Claves[pos - 1] = hijo.Claves[0];
            }
            else
            {
                for (int i = hijo.NumClaves; i > 0; i--)
                    hijo.Claves[i] = hijo.Claves[i - 1];
                for (int i = hijo.NumClaves + 1; i > 0; i--)
                    hijo.Hijos![i] = hijo.Hijos![i - 1];

                hijo.Claves[0] = nodo.Claves[pos - 1];
                hijo.Hijos![0] = izq.Hijos![izq.NumClaves];
                hijo.NumClaves++;

                nodo.Claves[pos - 1] = izq.Claves[izq.NumClaves - 1];
                izq.NumClaves--;
            }
        }

        private void PrestarDeDerecha(NodoBMas nodo, int pos)
        {
            var hijo = nodo.Hijos![pos];
            var der = nodo.Hijos![pos + 1];

            if (hijo.EsHoja)
            {
                hijo.Claves[hijo.NumClaves] = der.Claves[0];
                hijo.Datos![hijo.NumClaves] = der.Datos![0];
                hijo.NumClaves++;

                for (int i = 0; i < der.NumClaves - 1; i++)
                {
                    der.Claves[i] = der.Claves[i + 1];
                    der.Datos![i] = der.Datos![i + 1];
                }
                der.NumClaves--;

                nodo.Claves[pos] = der.Claves[0];
            }
            else
            {
                hijo.Claves[hijo.NumClaves] = nodo.Claves[pos];
                hijo.Hijos![hijo.NumClaves + 1] = der.Hijos![0];
                hijo.NumClaves++;

                nodo.Claves[pos] = der.Claves[0];

                for (int i = 0; i < der.NumClaves - 1; i++)
                    der.Claves[i] = der.Claves[i + 1];
                for (int i = 0; i < der.NumClaves; i++)
                    der.Hijos![i] = der.Hijos![i + 1];
                der.NumClaves--;
            }
        }

        // Fusiona hijo(posIzq) con hijo(posIzq+1) en un solo nodo (queda en posIzq).
        private void Fusionar(NodoBMas nodo, int posIzq)
        {
            var izq = nodo.Hijos![posIzq];
            var der = nodo.Hijos![posIzq + 1];

            if (izq.EsHoja)
            {
                for (int i = 0; i < der.NumClaves; i++)
                {
                    izq.Claves[izq.NumClaves + i] = der.Claves[i];
                    izq.Datos![izq.NumClaves + i] = der.Datos![i];
                }
                izq.NumClaves += der.NumClaves;
                izq.Siguiente = der.Siguiente;
            }
            else
            {
                izq.Claves[izq.NumClaves] = nodo.Claves[posIzq];
                izq.Hijos![izq.NumClaves + 1] = der.Hijos![0];
                for (int i = 0; i < der.NumClaves; i++)
                {
                    izq.Claves[izq.NumClaves + 1 + i] = der.Claves[i];
                    izq.Hijos![izq.NumClaves + 2 + i] = der.Hijos![i + 1];
                }
                izq.NumClaves += der.NumClaves + 1;
            }

            for (int i = posIzq; i < nodo.NumClaves - 1; i++)
                nodo.Claves[i] = nodo.Claves[i + 1];
            for (int i = posIzq + 1; i < nodo.NumClaves; i++)
                nodo.Hijos![i] = nodo.Hijos![i + 1];
            nodo.NumClaves--;
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
