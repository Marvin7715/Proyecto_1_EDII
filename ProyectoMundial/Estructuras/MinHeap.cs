using System;
using ProyectoMundial.Modelos;

namespace ProyectoMundial.Estructuras
{
    // Min Heap implementado desde cero (mismo diseno que el MaxHeap, con las
    // comparaciones invertidas).
    //
    // Ademas de sus operaciones basicas, aqui vive el algoritmo de "top-N
    // usando un min-heap acotado": para encontrar, por ejemplo, el top 5 de
    // goleadores entre cientos de jugadores, es mucho mas eficiente mantener
    // un heap de tamano 5 que ordenar la lista completa.
    public class MinHeap
    {
        private int[] claves;
        private Jugador[] datos;
        private int cantidad;

        public int Cantidad => cantidad;

        public MinHeap(int capacidadInicial = 8)
        {
            if (capacidadInicial < 1) capacidadInicial = 1;
            claves = new int[capacidadInicial];
            datos = new Jugador[capacidadInicial];
            cantidad = 0;
        }

        // ================= INSERTAR =================
        public void Insertar(int clave, Jugador dato)
        {
            if (cantidad == claves.Length)
                Redimensionar();

            claves[cantidad] = clave;
            datos[cantidad] = dato;
            SubirDesde(cantidad);
            cantidad++;
        }

        private void Redimensionar()
        {
            int nuevaCapacidad = claves.Length * 2;
            var nuevasClaves = new int[nuevaCapacidad];
            var nuevosDatos = new Jugador[nuevaCapacidad];
            for (int i = 0; i < cantidad; i++)
            {
                nuevasClaves[i] = claves[i];
                nuevosDatos[i] = datos[i];
            }
            claves = nuevasClaves;
            datos = nuevosDatos;
        }

        private void SubirDesde(int i)
        {
            while (i > 0)
            {
                int padre = (i - 1) / 2;
                if (claves[padre] <= claves[i]) break;
                Intercambiar(i, padre);
                i = padre;
            }
        }

        private void BajarDesde(int i)
        {
            while (true)
            {
                int izq = 2 * i + 1;
                int der = 2 * i + 2;
                int menor = i;

                if (izq < cantidad && claves[izq] < claves[menor]) menor = izq;
                if (der < cantidad && claves[der] < claves[menor]) menor = der;

                if (menor == i) break;
                Intercambiar(i, menor);
                i = menor;
            }
        }

        private void Intercambiar(int i, int j)
        {
            (claves[i], claves[j]) = (claves[j], claves[i]);
            (datos[i], datos[j]) = (datos[j], datos[i]);
        }

        // ================= BUSCAR =================
        public Jugador? Buscar(string id)
        {
            for (int i = 0; i < cantidad; i++)
                if (datos[i].Id == id)
                    return datos[i];
            return null;
        }

        // ================= ELIMINAR =================
        public bool Eliminar(string id)
        {
            int idx = -1;
            for (int i = 0; i < cantidad; i++)
                if (datos[i].Id == id) { idx = i; break; }
            if (idx == -1) return false;

            cantidad--;
            claves[idx] = claves[cantidad];
            datos[idx] = datos[cantidad];

            SubirDesde(idx);
            BajarDesde(idx);
            return true;
        }

        // ================= RECORRER =================
        public Jugador[] Recorrer()
        {
            var copia = new Jugador[cantidad];
            for (int i = 0; i < cantidad; i++)
                copia[i] = datos[i];
            return copia;
        }

        // ================= IMPRIMIR =================
        public void Imprimir()
        {
            Console.WriteLine("Contenido del Min Heap (orden interno del arreglo):");
            for (int i = 0; i < cantidad; i++)
                Console.WriteLine($"  [{claves[i]}] {datos[i]}");
        }

        public Jugador? VerMinimo() => cantidad == 0 ? null : datos[0];

        public Jugador? ExtraerMinimo()
        {
            if (cantidad == 0) return null;
            var min = datos[0];
            cantidad--;
            claves[0] = claves[cantidad];
            datos[0] = datos[cantidad];
            if (cantidad > 0) BajarDesde(0);
            return min;
        }

        // ================= TOP N (heap acotado) =================
        // Recorre 'candidatos' una sola vez, manteniendo dentro del heap
        // unicamente los N valores mas altos vistos hasta el momento (por
        // eso el heap es de MINIMOS: la raiz es siempre "el mas debil de
        // los mejores N", el primero en salir si aparece algo mejor).
        // Al final se extrae todo el heap para devolver el top N ordenado
        // de mayor a menor.
        public static Jugador[] ObtenerTopN(Jugador[] candidatos, Func<Jugador, int> obtenerClave, int n)
        {
            if (n < 1) return Array.Empty<Jugador>();

            var heap = new MinHeap(n);

            foreach (var jugador in candidatos)
            {
                int clave = obtenerClave(jugador);
                if (heap.Cantidad < n)
                {
                    heap.Insertar(clave, jugador);
                }
                else if (clave > heap.claves[0])
                {
                    heap.ExtraerMinimo();
                    heap.Insertar(clave, jugador);
                }
            }

            int total = heap.Cantidad;
            var resultado = new Jugador[total];
            for (int i = total - 1; i >= 0; i--)
                resultado[i] = heap.ExtraerMinimo()!; // sale el menor primero -> se llena de atras hacia adelante

            return resultado;
        }
    }
}
