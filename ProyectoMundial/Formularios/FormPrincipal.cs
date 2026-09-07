using System;
using System.Drawing;
using System.Windows.Forms;
using ProyectoMundial.Modelos;
using ProyectoMundial.Datos;

namespace ProyectoMundial.Formularios
{
    public partial class FormPrincipal : Form
    {
        private readonly GestorJugadores gestor = new GestorJugadores();

        private Label lblEstado = null!;

        // --- Tab Registro ---
        private TextBox txtId = null!, txtNombre = null!, txtSeleccion = null!;
        private ComboBox cmbPosicion = null!;
        private NumericUpDown numMinutos = null!, numGoles = null!, numAsistencias = null!,
                               numAmarillas = null!, numRojas = null!, numPartidos = null!;

        // --- Tab Busqueda ---
        private TextBox txtBuscarId = null!;
        private Label lblResultadoBusqueda = null!;

        // --- Tab Rankings ---
        private ComboBox cmbCategoria = null!;
        private DataGridView gridRankings = null!;

        // --- Tab Catalogo ---
        private DataGridView gridCatalogo = null!;

        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Tabla de posiciones - Mundial";
            this.Width = 1100;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(950, 650);
            this.Font = new Font("Segoe UI", 9.5F);

            var tabs = ConstruirTabs();
            var panelSuperior = ConstruirPanelSuperior();

            lblEstado = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Listo.",
                Padding = new Padding(8, 0, 0, 0),
                BackColor = Color.WhiteSmoke
            };

            this.Controls.Add(tabs);
            this.Controls.Add(panelSuperior);
            this.Controls.Add(lblEstado);
        }

        // ================= PANEL SUPERIOR (cargar / guardar csv) =================
        private Panel ConstruirPanelSuperior()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 50 };

            var btnCargar = new Button { Text = "Cargar desde CSV...", Left = 10, Top = 10, Width = 170, Height = 30 };
            btnCargar.Click += BtnCargarCsv_Click;

            var btnGuardar = new Button { Text = "Guardar en CSV...", Left = 190, Top = 10, Width = 170, Height = 30 };
            btnGuardar.Click += BtnGuardarCsv_Click;

            panel.Controls.Add(btnCargar);
            panel.Controls.Add(btnGuardar);
            return panel;
        }

        private void BtnCargarCsv_Click(object? sender, EventArgs e)
        {
            using var dialogo = new OpenFileDialog { Filter = "Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*" };
            if (dialogo.ShowDialog() != DialogResult.OK) return;

            try
            {
                int cargados = gestor.CargarDesdeCsv(dialogo.FileName);
                MostrarEstado($"Se cargaron {cargados} jugadores nuevos desde el archivo (los que ya existian se ignoraron).");
                RefrescarCatalogo();
            }
            catch (Exception ex)
            {
                MostrarEstado("Error al cargar el archivo: " + ex.Message);
            }
        }

        private void BtnGuardarCsv_Click(object? sender, EventArgs e)
        {
            using var dialogo = new SaveFileDialog { Filter = "Archivos CSV (*.csv)|*.csv", FileName = "jugadores.csv" };
            if (dialogo.ShowDialog() != DialogResult.OK) return;

            try
            {
                gestor.GuardarEnCsv(dialogo.FileName);
                MostrarEstado("Catalogo guardado correctamente en: " + dialogo.FileName);
            }
            catch (Exception ex)
            {
                MostrarEstado("Error al guardar el archivo: " + ex.Message);
            }
        }

        private void MostrarEstado(string mensaje) => lblEstado.Text = mensaje;

        // ================= TABS =================
        private TabControl ConstruirTabs()
        {
            var tabs = new TabControl { Dock = DockStyle.Fill };

            var tabRegistro = new TabPage("Registro");
            tabRegistro.Controls.Add(ConstruirPanelRegistro());

            var tabBusqueda = new TabPage("Busqueda");
            tabBusqueda.Controls.Add(ConstruirPanelBusqueda());

            var tabRankings = new TabPage("Rankings");
            tabRankings.Controls.Add(ConstruirPanelRankings());

            var tabCatalogo = new TabPage("Catalogo completo");
            tabCatalogo.Controls.Add(ConstruirPanelCatalogo());

            tabs.TabPages.Add(tabRegistro);
            tabs.TabPages.Add(tabBusqueda);
            tabs.TabPages.Add(tabRankings);
            tabs.TabPages.Add(tabCatalogo);

            return tabs;
        }

        // ================= REGISTRO =================
        private Control ConstruirPanelRegistro()
        {
            var contenedor = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 0,
                Padding = new Padding(24)
            };
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            void AgregarFila(string etiqueta, Control control)
            {
                int fila = contenedor.RowCount;
                contenedor.RowCount++;
                contenedor.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
                contenedor.Controls.Add(new Label { Text = etiqueta, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, fila);
                contenedor.Controls.Add(control, 1, fila);
            }

            txtId = new TextBox { Dock = DockStyle.Fill };
            txtNombre = new TextBox { Dock = DockStyle.Fill };
            txtSeleccion = new TextBox { Dock = DockStyle.Fill };
            cmbPosicion = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPosicion.Items.AddRange(new object[] { "Portero", "Defensa", "Mediocampista", "Delantero" });
            cmbPosicion.SelectedIndex = 0;

            numMinutos = NuevoNumeric(0, 700);
            numGoles = NuevoNumeric(0, 50);
            numAsistencias = NuevoNumeric(0, 50);
            numAmarillas = NuevoNumeric(0, 10);
            numRojas = NuevoNumeric(0, 5);
            numPartidos = NuevoNumeric(0, 10);

            AgregarFila("Id (unico):", txtId);
            AgregarFila("Nombre:", txtNombre);
            AgregarFila("Seleccion:", txtSeleccion);
            AgregarFila("Posicion:", cmbPosicion);
            AgregarFila("Minutos jugados:", numMinutos);
            AgregarFila("Goles:", numGoles);
            AgregarFila("Asistencias:", numAsistencias);
            AgregarFila("Tarjetas amarillas:", numAmarillas);
            AgregarFila("Tarjetas rojas:", numRojas);
            AgregarFila("Partidos jugados:", numPartidos);

            var panelBotones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true
            };

            var btnRegistrar = new Button { Text = "Registrar nuevo", Width = 150, Height = 34 };
            btnRegistrar.Click += BtnRegistrar_Click;

            var btnCargarParaEditar = new Button { Text = "Cargar para editar", Width = 150, Height = 34 };
            btnCargarParaEditar.Click += BtnCargarParaEditar_Click;

            var btnActualizar = new Button { Text = "Guardar cambios", Width = 150, Height = 34 };
            btnActualizar.Click += BtnActualizar_Click;

            var btnLimpiar = new Button { Text = "Limpiar campos", Width = 140, Height = 34 };
            btnLimpiar.Click += (s, e) => LimpiarCamposRegistro();

            panelBotones.Controls.Add(btnRegistrar);
            panelBotones.Controls.Add(btnCargarParaEditar);
            panelBotones.Controls.Add(btnActualizar);
            panelBotones.Controls.Add(btnLimpiar);

            contenedor.RowCount++;
            contenedor.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            contenedor.SetColumnSpan(panelBotones, 2);
            contenedor.Controls.Add(panelBotones, 0, contenedor.RowCount - 1);

            return contenedor;
        }

        private NumericUpDown NuevoNumeric(int min, int max) => new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Minimum = min,
            Maximum = max,
            Value = 0
        };

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            string id = txtId.Text.Trim();
            if (id.Length == 0)
            {
                MostrarEstado("El Id no puede estar vacio.");
                return;
            }
            if (gestor.Buscar(id) != null)
            {
                MostrarEstado("Ya existe un jugador con ese Id. Usa 'Cargar para editar' + 'Guardar cambios'.");
                return;
            }

            var jugador = LeerJugadorDesdeFormulario(id);
            gestor.Registrar(jugador);
            MostrarEstado($"Jugador {id} registrado correctamente.");
            LimpiarCamposRegistro();
            RefrescarCatalogo();
        }

        private void BtnCargarParaEditar_Click(object? sender, EventArgs e)
        {
            string id = txtId.Text.Trim();
            var jugador = gestor.Buscar(id);
            if (jugador == null)
            {
                MostrarEstado("No existe un jugador con ese Id.");
                return;
            }

            txtNombre.Text = jugador.Nombre;
            txtSeleccion.Text = jugador.Seleccion;
            cmbPosicion.SelectedItem = jugador.Posicion;
            numMinutos.Value = jugador.MinutosJugados;
            numGoles.Value = jugador.Goles;
            numAsistencias.Value = jugador.Asistencias;
            numAmarillas.Value = jugador.TarjetasAmarillas;
            numRojas.Value = jugador.TarjetasRojas;
            numPartidos.Value = jugador.PartidosJugados;

            MostrarEstado($"Datos de {id} cargados. Modifica lo necesario y presiona 'Guardar cambios'.");
        }

        private void BtnActualizar_Click(object? sender, EventArgs e)
        {
            string id = txtId.Text.Trim();
            var jugador = gestor.Buscar(id);
            if (jugador == null)
            {
                MostrarEstado("No existe un jugador con ese Id. Usa 'Registrar nuevo' primero.");
                return;
            }

            jugador.Nombre = txtNombre.Text.Trim();
            jugador.Seleccion = txtSeleccion.Text.Trim();
            jugador.Posicion = cmbPosicion.SelectedItem?.ToString() ?? jugador.Posicion;
            jugador.MinutosJugados = (int)numMinutos.Value;
            jugador.Goles = (int)numGoles.Value;
            jugador.Asistencias = (int)numAsistencias.Value;
            jugador.TarjetasAmarillas = (int)numAmarillas.Value;
            jugador.TarjetasRojas = (int)numRojas.Value;
            jugador.PartidosJugados = (int)numPartidos.Value;

            MostrarEstado($"Datos de {id} actualizados.");
            RefrescarCatalogo();
        }

        private Jugador LeerJugadorDesdeFormulario(string id) => new Jugador(
            id,
            txtNombre.Text.Trim(),
            txtSeleccion.Text.Trim(),
            cmbPosicion.SelectedItem?.ToString() ?? "Delantero",
            (int)numMinutos.Value,
            (int)numGoles.Value,
            (int)numAsistencias.Value,
            (int)numAmarillas.Value,
            (int)numRojas.Value,
            (int)numPartidos.Value
        );

        private void LimpiarCamposRegistro()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtSeleccion.Clear();
            cmbPosicion.SelectedIndex = 0;
            numMinutos.Value = 0;
            numGoles.Value = 0;
            numAsistencias.Value = 0;
            numAmarillas.Value = 0;
            numRojas.Value = 0;
            numPartidos.Value = 0;
        }

        // ================= BUSQUEDA =================
        private Control ConstruirPanelBusqueda()
        {
            var contenedor = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };

            var lblTitulo = new Label { Text = "Id del jugador:", Left = 0, Top = 14, Width = 120 };
            txtBuscarId = new TextBox { Left = 130, Top = 11, Width = 160 };
            var btnBuscar = new Button { Text = "Buscar", Left = 300, Top = 8, Width = 100, Height = 30 };
            btnBuscar.Click += BtnBuscar_Click;

            var btnEliminar = new Button { Text = "Eliminar este jugador", Left = 410, Top = 8, Width = 170, Height = 30 };
            btnEliminar.Click += BtnEliminarDesdeBusqueda_Click;

            lblResultadoBusqueda = new Label
            {
                Left = 0,
                Top = 60,
                Width = 750,
                Height = 280,
                Font = new Font("Consolas", 11.5F),
                Text = "Escribe un Id y presiona Buscar."
            };

            contenedor.Controls.Add(lblTitulo);
            contenedor.Controls.Add(txtBuscarId);
            contenedor.Controls.Add(btnBuscar);
            contenedor.Controls.Add(btnEliminar);
            contenedor.Controls.Add(lblResultadoBusqueda);

            return contenedor;
        }

        private void BtnBuscar_Click(object? sender, EventArgs e)
        {
            string id = txtBuscarId.Text.Trim();
            var jugador = gestor.Buscar(id);

            if (jugador == null)
            {
                lblResultadoBusqueda.Text = $"No se encontro ningun jugador con Id '{id}'.";
                return;
            }

            lblResultadoBusqueda.Text =
                $"Id:               {jugador.Id}\n" +
                $"Nombre:           {jugador.Nombre}\n" +
                $"Seleccion:        {jugador.Seleccion}\n" +
                $"Posicion:         {jugador.Posicion}\n" +
                $"Minutos jugados:  {jugador.MinutosJugados}\n" +
                $"Goles:            {jugador.Goles}\n" +
                $"Asistencias:      {jugador.Asistencias}\n" +
                $"Tarjetas amar.:   {jugador.TarjetasAmarillas}\n" +
                $"Tarjetas rojas:   {jugador.TarjetasRojas}\n" +
                $"Partidos jugados: {jugador.PartidosJugados}";
        }

        private void BtnEliminarDesdeBusqueda_Click(object? sender, EventArgs e)
        {
            string id = txtBuscarId.Text.Trim();
            if (id.Length == 0)
            {
                MostrarEstado("Escribe un Id antes de eliminar.");
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Seguro que quieres eliminar al jugador '{id}'? Esta accion no se puede deshacer.",
                "Confirmar eliminacion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
                return;

            bool eliminado = gestor.Eliminar(id);
            if (eliminado)
            {
                lblResultadoBusqueda.Text = $"El jugador '{id}' fue eliminado del catalogo.";
                MostrarEstado($"Jugador {id} eliminado.");
                RefrescarCatalogo();
            }
            else
            {
                MostrarEstado($"No existe ningun jugador con Id '{id}' para eliminar.");
            }
        }

        // ================= RANKINGS =================
        private Control ConstruirPanelRankings()
        {
            var contenedor = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };

            var lblCategoria = new Label { Text = "Categoria:", Left = 0, Top = 14, Width = 90 };
            cmbCategoria = new ComboBox { Left = 90, Top = 11, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategoria.Items.AddRange(new object[]
            {
                "Goles", "Asistencias", "Minutos jugados",
                "Tarjetas amarillas", "Tarjetas rojas", "Partidos jugados"
            });
            cmbCategoria.SelectedIndex = 0;

            var btnTop5 = new Button { Text = "Ver Top 5", Left = 300, Top = 8, Width = 110, Height = 30 };
            btnTop5.Click += (s, e) => MostrarRanking(soloTop5: true);

            var btnCompleto = new Button { Text = "Listado completo", Left = 420, Top = 8, Width = 150, Height = 30 };
            btnCompleto.Click += (s, e) => MostrarRanking(soloTop5: false);

            gridRankings = new DataGridView
            {
                Left = 0,
                Top = 60,
                Width = 950,
                Height = 480,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            ConfigurarColumnasJugador(gridRankings);

            contenedor.Controls.Add(lblCategoria);
            contenedor.Controls.Add(cmbCategoria);
            contenedor.Controls.Add(btnTop5);
            contenedor.Controls.Add(btnCompleto);
            contenedor.Controls.Add(gridRankings);

            return contenedor;
        }

        private Func<Jugador, int> ObtenerSelectorCategoria()
        {
            return cmbCategoria.SelectedItem?.ToString() switch
            {
                "Goles" => j => j.Goles,
                "Asistencias" => j => j.Asistencias,
                "Minutos jugados" => j => j.MinutosJugados,
                "Tarjetas amarillas" => j => j.TarjetasAmarillas,
                "Tarjetas rojas" => j => j.TarjetasRojas,
                "Partidos jugados" => j => j.PartidosJugados,
                _ => j => j.Goles
            };
        }

        private void MostrarRanking(bool soloTop5)
        {
            var selector = ObtenerSelectorCategoria();
            var resultado = soloTop5 ? gestor.ObtenerTopN(selector, 5) : gestor.ObtenerListadoPorCategoria(selector);

            LlenarGrid(gridRankings, resultado);
            MostrarEstado(soloTop5
                ? "Mostrando Top 5 (usando el Min Heap acotado)."
                : "Mostrando el listado completo ordenado (usando el Max Heap / heap sort).");
        }

        // ================= CATALOGO =================
        private Control ConstruirPanelCatalogo()
        {
            var contenedor = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };

            var btnRefrescar = new Button { Text = "Actualizar catalogo", Left = 0, Top = 10, Width = 170, Height = 30 };
            btnRefrescar.Click += (s, e) => RefrescarCatalogo();

            gridCatalogo = new DataGridView
            {
                Left = 0,
                Top = 55,
                Width = 950,
                Height = 480,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            ConfigurarColumnasJugador(gridCatalogo);

            contenedor.Controls.Add(btnRefrescar);
            contenedor.Controls.Add(gridCatalogo);

            return contenedor;
        }

        private void RefrescarCatalogo()
        {
            LlenarGrid(gridCatalogo, gestor.ObtenerTodos());
        }

        // ================= AYUDANTES COMPARTIDOS =================
        private void ConfigurarColumnasJugador(DataGridView grid)
        {
            grid.Columns.Clear();
            grid.Columns.Add("colId", "Id");
            grid.Columns.Add("colNombre", "Nombre");
            grid.Columns.Add("colSeleccion", "Seleccion");
            grid.Columns.Add("colPosicion", "Posicion");
            grid.Columns.Add("colMinutos", "Minutos");
            grid.Columns.Add("colGoles", "Goles");
            grid.Columns.Add("colAsistencias", "Asistencias");
            grid.Columns.Add("colAmarillas", "T. Amarillas");
            grid.Columns.Add("colRojas", "T. Rojas");
            grid.Columns.Add("colPartidos", "Partidos");
        }

        private void LlenarGrid(DataGridView grid, Jugador[] jugadores)
        {
            grid.Rows.Clear();
            foreach (var j in jugadores)
            {
                grid.Rows.Add(j.Id, j.Nombre, j.Seleccion, j.Posicion,
                    j.MinutosJugados, j.Goles, j.Asistencias,
                    j.TarjetasAmarillas, j.TarjetasRojas, j.PartidosJugados);
            }
        }
    }
}
