using System;
using ProyectoMundial.Modelos;

namespace ProyectoMundial.Estructuras
{
    // Max Heap
    // Se usa para: obtener rapidamente el jugador con el valor mas alto en
    // una categoria (el "record"), y para generar el listado completo
    // ordenado de mayor a menor en esa categoria (heap sort).
    public class MaxHeap
    {
        private int[] claves;
        private Jugador[] datos;
        private int cantidad;

        public int Cantidad => cantidad;

        public MaxHeap(int capacidadInicial = 8)
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

        // Como no se permite usar List<T>, cuando el arreglo se llena se crea
        // uno nuevo del doble de tamano y se copian los datos: asi los datos
        // siguen sin estar "quemados" a un tamano fijo.
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
                if (claves[padre] >= claves[i]) break;
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
                int mayor = i;

                if (izq < cantidad && claves[izq] > claves[mayor]) mayor = izq;
                if (der < cantidad && claves[der] > claves[mayor]) mayor = der;

                if (mayor == i) break;
                Intercambiar(i, mayor);
                i = mayor;
            }
        }

        private void Intercambiar(int i, int j)
        {
            (claves[i], claves[j]) = (claves[j], claves[i]);
            (datos[i], datos[j]) = (datos[j], datos[i]);
        }

        // ================= BUSCAR =================
        // En un heap no hay una forma mas rapida de buscar un dato arbitrario
        // que no sea la raiz: solo se garantiza el orden entre padre e hijo,
        // asi que buscar recorre todo el arreglo (O(n)).
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

            // El elemento que se movio al hueco puede necesitar subir o
            // bajar para restaurar la propiedad de heap.
            SubirDesde(idx);
            BajarDesde(idx);
            return true;
        }

        // ================= RECORRER =================
        // Devuelve los elementos tal como estan en el arreglo interno del
        // heap (no es un orden alfabetico ni descendente).
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
            Console.WriteLine("Contenido del Max Heap (orden interno del arreglo):");
            for (int i = 0; i < cantidad; i++)
                Console.WriteLine($"  [{claves[i]}] {datos[i]}");
        }

        public Jugador? VerMaximo() => cantidad == 0 ? null : datos[0];

        public Jugador? ExtraerMaximo()
        {
            if (cantidad == 0) return null;
            var max = datos[0];
            cantidad--;
            claves[0] = claves[cantidad];
            datos[0] = datos[cantidad];
            if (cantidad > 0) BajarDesde(0);
            return max;
        }

        // Devuelve TODOS los jugadores ordenados de mayor a menor segun la
        // clave (heap sort). No modifica este heap: trabaja sobre una copia.
        public Jugador[] ObtenerTodosOrdenadosDescendente()
        {
            var temporal = new MaxHeap(cantidad);
            temporal.cantidad = cantidad;
            Array.Copy(claves, temporal.claves, cantidad);
            Array.Copy(datos, temporal.datos, cantidad);

            var resultado = new Jugador[cantidad];
            for (int i = 0; i < resultado.Length; i++)
                resultado[i] = temporal.ExtraerMaximo()!;
            return resultado;
        }
    }
}
