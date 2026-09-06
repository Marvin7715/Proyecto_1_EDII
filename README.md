# Proyecto Mundial - Estructura de Datos II

## Estado actual: Etapa 1 - Esqueleto del proyecto

Este es el punto de partida. Por ahora el programa solo abre una ventana de
prueba para confirmar que todo compila correctamente. Las estructuras de
datos (Arbol B+, Max Heap, Min Heap) y el menu real se iran agregando en las
siguientes etapas, una por una.

## Como abrirlo en Visual Studio

1. Descomprime este .zip en una carpeta de tu computadora (por ejemplo en
   Documentos).
2. Abre Visual Studio.
3. Archivo > Abrir > Proyecto o solucion...
4. Selecciona el archivo `ProyectoMundial.sln`.
5. Presiona F5 (o el boton verde de Play) para ejecutar.

Deberias ver una ventana que dice "Etapa 1 completa...". Si eso aparece,
el entorno esta listo y podemos seguir con la siguiente etapa.

## Estructura de carpetas

- `Modelos/` -> la clase `Jugador`, con todos los datos de cada jugador.
- `Estructuras/` -> aqui ira el Arbol B+, el Max Heap y el Min Heap
  (implementados a mano, sin usar `List<T>` ni `Dictionary` como estructura
  principal, tal como lo exige el proyecto).
- `Datos/` -> el archivo `.csv` de ejemplo y, mas adelante, la clase que lo
  lee y lo escribe.
- `Formularios/` -> las pantallas de Windows Forms (registro, busqueda,
  rankings).

## Siguiente paso

Cuando confirmes que esto corre sin errores en tu maquina, seguimos con la
implementacion del Arbol B+.
