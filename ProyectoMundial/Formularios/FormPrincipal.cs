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

        // ---- Paleta del tema oscuro (Futstats MV) ----
        private static readonly Color ColorFondo = Color.FromArgb(0x12, 0x15, 0x1f);
        private static readonly Color ColorPanel = Color.FromArgb(0x1c, 0x21, 0x30);
        private static readonly Color ColorBorde = Color.FromArgb(0x3a, 0x41, 0x55);
        private static readonly Color ColorSeleccion = Color.FromArgb(0x23, 0x2a, 0x3d);
        private static readonly Color ColorTexto = Color.White;
        private static readonly Color ColorTextoSecundario = Color.FromArgb(0x9a, 0xa3, 0xb8);
        private static readonly Color ColorAcento = Color.FromArgb(0xe0, 0xa9, 0x4c);

        private Label lblEstado = null!;

        // --- Barra de pestanas propia (reemplaza al TabControl nativo) ---
        private Button[] botonesTab = null!;
        private Panel[] panelesTab = null!;

        // --- Tab Registro ---
        private TextBox txtId = null!, txtNombre = null!, txtSeleccion = null!;
        private ComboBox cmbPosicion = null!;
        private NumericUpDown numMinutos = null!, numGoles = null!, numAsistencias = null!,
                               numAmarillas = null!, numRojas = null!, numPartidos = null!, numPorterias = null!;

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
            this.Text = "Futstats MV - Estadisticas del Mundial";
            this.Width = 1150;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(980, 650);
            this.Font = new Font("Segoe UI", 9.5F);
            this.BackColor = ColorFondo;
            this.ForeColor = ColorTexto;

            var panelSuperior = ConstruirPanelSuperior();
            var panelContenido = ConstruirContenido();

            lblEstado = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Listo.",
                Padding = new Padding(8, 0, 0, 0),
                BackColor = ColorPanel,
                ForeColor = ColorTextoSecundario
            };

            // Orden importante: Fill primero, luego Top, luego Bottom.
            this.Controls.Add(panelContenido);
            this.Controls.Add(panelSuperior);
            this.Controls.Add(lblEstado);

            MostrarPestana(3); // Arranca en "Catalogo completo"
        }

        // ================= ENCABEZADO + BOTONES CSV + BARRA DE PESTANAS =================
        // Todo esto vive en un unico panel Dock=Top, posicionado con Left/Top/Anchor
        // (evita ambiguedades de orden entre varios controles con Dock=Top).
        private Panel ConstruirPanelSuperior()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 132, BackColor = ColorPanel };

            var logo = new Panel
            {
                Left = 16,
                Top = 14,
                Width = 44,
                Height = 44,
                BackColor = ColorFondo
            };
            var lblLogo = new Label
            {
                Dock = DockStyle.Fill,
                Text = "\U0001F3C6", // trofeo
                Font = new Font("Segoe UI Emoji", 18F),
                ForeColor = ColorAcento,
                TextAlign = ContentAlignment.MiddleCenter
            };
            logo.Controls.Add(lblLogo);

            var lblMarca = new Label
            {
                Left = 70, Top = 16, Width = 200, Height = 18,
                Text = "Futstats MV",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTextoSecundario
            };
            var lblTitulo = new Label
            {
                Left = 70, Top = 34, Width = 400, Height = 26,
                Text = "Estadísticas del mundial",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = ColorTexto
            };

            var btnCargar = BotonSecundario("Cargar desde CSV...", 170);
            btnCargar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCargar.Top = 16;
            btnCargar.Left = panel.Width - 16 - 170 - 10 - 170;
            btnCargar.Click += BtnCargarCsv_Click;

            var btnGuardar = BotonSecundario("Guardar en CSV...", 170);
            btnGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGuardar.Top = 16;
            btnGuardar.Left = panel.Width - 16 - 170;
            btnGuardar.Click += BtnGuardarCsv_Click;

            // ---- Barra de pestanas (4 botones a partes iguales) ----
            var tablaTabs = new TableLayoutPanel
            {
                Left = 16,
                Top = 82,
                Height = 40,
                Width = panel.Width - 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = 4,
                RowCount = 1
            };
            for (int i = 0; i < 4; i++)
                tablaTabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            string[] titulosTab = { "Registro", "Busqueda", "Rankings", "Catalogo completo" };
            botonesTab = new Button[4];
            for (int i = 0; i < 4; i++)
            {
                var boton = new Button
                {
                    Text = titulosTab[i],
                    Dock = DockStyle.Fill,
                    Margin = new Padding(i == 0 ? 0 : 4, 0, 0, 0),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = ColorPanel,
                    ForeColor = ColorTexto
                };
                boton.FlatAppearance.BorderColor = ColorBorde;
                boton.FlatAppearance.BorderSize = 1;

                int indice = i; // copia local: evita el error clasico de closures en un for
                boton.Click += (s, e) => MostrarPestana(indice);

                botonesTab[i] = boton;
                tablaTabs.Controls.Add(boton, i, 0);
            }

            panel.Controls.Add(logo);
            panel.Controls.Add(lblMarca);
            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(btnCargar);
            panel.Controls.Add(btnGuardar);
            panel.Controls.Add(tablaTabs);

            return panel;
        }

        private Button BotonSecundario(string texto, int ancho)
        {
            var boton = new Button
            {
                Text = texto,
                Width = ancho,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorPanel,
                ForeColor = ColorTexto
            };
            boton.FlatAppearance.BorderColor = ColorBorde;
            boton.FlatAppearance.BorderSize = 1;
            return boton;
        }

        private void MostrarPestana(int indice)
        {
            for (int i = 0; i < panelesTab.Length; i++)
            {
                bool activo = (i == indice);
                panelesTab[i].Visible = activo;
                botonesTab[i].BackColor = activo ? ColorSeleccion : ColorPanel;
                botonesTab[i].Font = new Font(botonesTab[i].Font, activo ? FontStyle.Bold : FontStyle.Regular);
            }

            if (indice == 3) RefrescarCatalogo();
        }

        // ================= CARGAR / GUARDAR CSV =================
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

        // ================= CONTENIDO (4 paneles intercambiables) =================
        private Control ConstruirContenido()
        {
            var contenedor = new Panel { Dock = DockStyle.Fill, BackColor = ColorFondo };

            var panelRegistro = new Panel { Dock = DockStyle.Fill, BackColor = ColorFondo, Visible = false };
            panelRegistro.Controls.Add(ConstruirPanelRegistro());

            var panelBusqueda = new Panel { Dock = DockStyle.Fill, BackColor = ColorFondo, Visible = false };
            panelBusqueda.Controls.Add(ConstruirPanelBusqueda());

            var panelRankings = new Panel { Dock = DockStyle.Fill, BackColor = ColorFondo, Visible = false };
            panelRankings.Controls.Add(ConstruirPanelRankings());

            var panelCatalogo = new Panel { Dock = DockStyle.Fill, BackColor = ColorFondo, Visible = false };
            panelCatalogo.Controls.Add(ConstruirPanelCatalogo());

            panelesTab = new[] { panelRegistro, panelBusqueda, panelRankings, panelCatalogo };
            foreach (var p in panelesTab)
                contenedor.Controls.Add(p);

            return contenedor;
        }

        // ================= REGISTRO =================
        private Control ConstruirPanelRegistro()
        {
            var contenedor = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 0,
                Padding = new Padding(24),
                BackColor = ColorFondo
            };
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            void AgregarFila(string etiqueta, Control control)
            {
                int fila = contenedor.RowCount;
                contenedor.RowCount++;
                contenedor.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
                contenedor.Controls.Add(new Label { Text = etiqueta, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, ForeColor = ColorTexto }, 0, fila);
                contenedor.Controls.Add(control, 1, fila);
            }

            txtId = CampoTexto();
            txtNombre = CampoTexto();
            txtSeleccion = CampoTexto();
            cmbPosicion = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = ColorPanel, ForeColor = ColorTexto };
            cmbPosicion.Items.AddRange(new object[] { "Portero", "Defensa", "Mediocampista", "Delantero" });
            cmbPosicion.SelectedIndex = 0;

            numMinutos = NuevoNumeric(0, 900);
            numGoles = NuevoNumeric(0, 50);
            numAsistencias = NuevoNumeric(0, 50);
            numAmarillas = NuevoNumeric(0, 10);
            numRojas = NuevoNumeric(0, 5);
            numPartidos = NuevoNumeric(0, 10);
            numPorterias = NuevoNumeric(0, 10);

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
            AgregarFila("Porterias a 0:", numPorterias);

            var panelBotones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                BackColor = ColorFondo
            };

            var btnRegistrar = BotonSecundario("Registrar nuevo", 150);
            btnRegistrar.Height = 34;
            btnRegistrar.Click += BtnRegistrar_Click;

            var btnCargarParaEditar = BotonSecundario("Cargar para editar", 150);
            btnCargarParaEditar.Height = 34;
            btnCargarParaEditar.Click += BtnCargarParaEditar_Click;

            var btnActualizar = BotonSecundario("Guardar cambios", 150);
            btnActualizar.Height = 34;
            btnActualizar.Click += BtnActualizar_Click;

            var btnLimpiar = BotonSecundario("Limpiar campos", 140);
            btnLimpiar.Height = 34;
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

        private TextBox CampoTexto() => new TextBox { Dock = DockStyle.Fill, BackColor = ColorPanel, ForeColor = ColorTexto, BorderStyle = BorderStyle.FixedSingle };

        private NumericUpDown NuevoNumeric(int min, int max) => new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Minimum = min,
            Maximum = max,
            Value = 0,
            BackColor = ColorPanel,
            ForeColor = ColorTexto
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
            numPorterias.Value = jugador.PorteriasACero;

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
            jugador.PorteriasACero = (int)numPorterias.Value;

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
            (int)numPartidos.Value,
            (int)numPorterias.Value
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
            numPorterias.Value = 0;
        }

        // ================= BUSQUEDA =================
        private Control ConstruirPanelBusqueda()
        {
            var contenedor = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = ColorFondo };

            var lblTitulo = new Label { Text = "Id del jugador:", Left = 0, Top = 14, Width = 120, ForeColor = ColorTexto };
            txtBuscarId = CampoTextoPosicionado(130, 11, 160);
            var btnBuscar = BotonSecundario("Buscar", 100);
            btnBuscar.Left = 300; btnBuscar.Top = 8; btnBuscar.Height = 30;
            btnBuscar.Click += BtnBuscar_Click;

            var btnEliminar = BotonSecundario("Eliminar este jugador", 170);
            btnEliminar.Left = 410; btnEliminar.Top = 8; btnEliminar.Height = 30;
            btnEliminar.Click += BtnEliminarDesdeBusqueda_Click;

            lblResultadoBusqueda = new Label
            {
                Left = 0,
                Top = 60,
                Width = 750,
                Height = 280,
                Font = new Font("Consolas", 11.5F),
                Text = "Escribe un Id y presiona Buscar.",
                ForeColor = ColorTexto
            };

            contenedor.Controls.Add(lblTitulo);
            contenedor.Controls.Add(txtBuscarId);
            contenedor.Controls.Add(btnBuscar);
            contenedor.Controls.Add(btnEliminar);
            contenedor.Controls.Add(lblResultadoBusqueda);

            return contenedor;
        }

        private TextBox CampoTextoPosicionado(int left, int top, int width) => new TextBox
        {
            Left = left, Top = top, Width = width,
            BackColor = ColorPanel, ForeColor = ColorTexto, BorderStyle = BorderStyle.FixedSingle
        };

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
                $"Partidos jugados: {jugador.PartidosJugados}\n" +
                $"Porterias a 0:    {jugador.PorteriasACero}";
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
            var contenedor = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = ColorFondo };

            var lblCategoria = new Label { Text = "Categoria:", Left = 0, Top = 14, Width = 90, ForeColor = ColorTexto };
            cmbCategoria = new ComboBox { Left = 90, Top = 11, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = ColorPanel, ForeColor = ColorTexto };
            cmbCategoria.Items.AddRange(new object[]
            {
                "Goles", "Asistencias", "Minutos jugados",
                "Tarjetas amarillas", "Tarjetas rojas", "Partidos jugados", "Porterias a 0"
            });
            cmbCategoria.SelectedIndex = 0;

            var btnTop5 = BotonSecundario("Ver Top 5", 110);
            btnTop5.Left = 300; btnTop5.Top = 8; btnTop5.Height = 30;
            btnTop5.Click += (s, e) => MostrarRanking(soloTop5: true);

            var btnCompleto = BotonSecundario("Listado completo", 150);
            btnCompleto.Left = 420; btnCompleto.Top = 8; btnCompleto.Height = 30;
            btnCompleto.Click += (s, e) => MostrarRanking(soloTop5: false);

            gridRankings = NuevoGrid();
            gridRankings.Left = 0;
            gridRankings.Top = 60;
            gridRankings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
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
                "Porterias a 0" => j => j.PorteriasACero,
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
            var contenedor = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = ColorFondo };

            var btnRefrescar = BotonSecundario("Actualizar catalogo", 170);
            btnRefrescar.Left = 0; btnRefrescar.Top = 10; btnRefrescar.Height = 30;
            btnRefrescar.Click += (s, e) => RefrescarCatalogo();

            gridCatalogo = NuevoGrid();
            gridCatalogo.Left = 0;
            gridCatalogo.Top = 55;
            gridCatalogo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
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
        private DataGridView NuevoGrid() => new DataGridView
        {
            Width = 1000,
            Height = 480,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = ColorFondo,
            GridColor = ColorBorde,
            BorderStyle = BorderStyle.FixedSingle,
            EnableHeadersVisualStyles = false,
            ColumnHeadersDefaultCellStyle =
            {
                BackColor = ColorPanel,
                ForeColor = ColorTextoSecundario,
                SelectionBackColor = ColorPanel,
                SelectionForeColor = ColorTextoSecundario,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            },
            DefaultCellStyle =
            {
                BackColor = ColorPanel,
                ForeColor = ColorTexto,
                SelectionBackColor = ColorSeleccion,
                SelectionForeColor = ColorTexto
            }
        };

        // Cada columna tiene el ancho que realmente necesita su contenido (la
        // mayoria son numeros cortos): asi caben las 11 columnas sin verse
        // separadas artificialmente. Si aun asi no caben, el grid muestra
        // su scroll horizontal normal en vez de deformar las columnas.
        private void ConfigurarColumnasJugador(DataGridView grid)
        {
            grid.Columns.Clear();
            grid.Columns.Add(NuevaColumna("colId", "Id", 100));
            grid.Columns.Add(NuevaColumna("colNombre", "Nombre", 290));
            grid.Columns.Add(NuevaColumna("colSeleccion", "Seleccion", 210));
            grid.Columns.Add(NuevaColumna("colPosicion", "Posicion", 165));
            grid.Columns.Add(NuevaColumna("colMinutos", "Minutos", 165));
            grid.Columns.Add(NuevaColumna("colGoles", "Goles", 165));
            grid.Columns.Add(NuevaColumna("colAsistencias", "Asistencias", 165));
            grid.Columns.Add(NuevaColumna("colAmarillas", "T. Amarillas", 165));
            grid.Columns.Add(NuevaColumna("colRojas", "T. Rojas", 165));
            grid.Columns.Add(NuevaColumna("colPartidos", "Partidos", 165));
            grid.Columns.Add(NuevaColumna("colPorterias", "Porterias a 0", 165));
        }

        private DataGridViewTextBoxColumn NuevaColumna(string nombre, string encabezado, int ancho) => new DataGridViewTextBoxColumn
        {
            Name = nombre,
            HeaderText = encabezado,
            Width = ancho
        };

        private void LlenarGrid(DataGridView grid, Jugador[] jugadores)
        {
            grid.Rows.Clear();
            foreach (var j in jugadores)
            {
                grid.Rows.Add(j.Id, j.Nombre, j.Seleccion, j.Posicion,
                    j.MinutosJugados, j.Goles, j.Asistencias,
                    j.TarjetasAmarillas, j.TarjetasRojas, j.PartidosJugados, j.PorteriasACero);
            }
        }
    }
}
