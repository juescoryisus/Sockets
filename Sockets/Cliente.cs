using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Sockets
{
    public partial class Cliente : Form
    {
        private const int PUERTO = 6400;
        private const string SERVIDOR = "[Servidor]";

        private Socket clienteSocket;
        private StreamWriter escritor;
        private readonly object lockEscritura = new();

        private string miNombre;
        private bool conectado;

        // Historial de cada conversación: nombre del otro usuario -> texto
        private readonly Dictionary<string, StringBuilder> conversaciones = new();
        private string chatActual; // con quién estoy chateando ahora

        public Cliente() : this("127.0.0.1") { }

        public Cliente(string ipServidor)
        {
            InitializeComponent();
            txtIpServidor.Text = ipServidor;
        }

        private void Cliente_Load(object sender, EventArgs e)
        {
            ActualizarEstado(false);
        }

        // ---------- Conexión ----------
        private void btnConectar_Click(object sender, EventArgs e)
        {
            if (conectado)
            {
                Desconectar();
                return;
            }

            string nombre = txtNombre.Text.Trim();

            if (nombre == "")
            {
                MessageBox.Show("Escribe tu nombre de usuario.");
                return;
            }
            if (nombre.Length > 20 || nombre.IndexOfAny(new[] { '|', ',', '[', ']' }) >= 0)
            {
                MessageBox.Show("El nombre debe tener máx. 20 caracteres y no puede contener | , [ ]");
                return;
            }
            if (!IPAddress.TryParse(txtIpServidor.Text.Trim(), out IPAddress ip))
            {
                MessageBox.Show("La IP del servidor no es válida.");
                return;
            }

            try
            {
                clienteSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clienteSocket.Connect(new IPEndPoint(ip, PUERTO));

                var stream = new NetworkStream(clienteSocket);
                var lector = new StreamReader(stream, Encoding.UTF8);
                escritor = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

                Enviar($"NICK|{nombre}");

                miNombre = nombre;
                conectado = true;
                conversaciones.Clear();
                chatActual = null;
                txtChat.Clear();
                lblConversacion.Text = "Chat con: (nadie)";
                ActualizarEstado(true);
                ActualizarListaUsuarios(Array.Empty<string>());

                Task.Run(() => RecibirMensajes(lector));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo conectar al servidor: {ex.Message}");
                try { clienteSocket?.Close(); } catch { }
            }
        }

        private void Desconectar()
        {
            conectado = false;
            try { clienteSocket?.Close(); } catch { }
            lstUsuarios.Items.Clear();
            lblAviso.Text = "";
            ActualizarEstado(false);
        }

        private void ActualizarEstado(bool con)
        {
            txtNombre.Enabled = !con;
            txtIpServidor.Enabled = !con;
            btnConectar.Text = con ? "Desconectar" : "Conectar";
            lstUsuarios.Enabled = con;
            txtMensaje.Enabled = con;
            btnEnviar.Enabled = con;
            Text = con ? $"Cliente - {miNombre}" : "Cliente";
        }

        // ---------- Recepción ----------
        private void RecibirMensajes(StreamReader lector)
        {
            try
            {
                string linea;
                while ((linea = lector.ReadLine()) != null)
                    ProcesarLinea(linea);
            }
            catch (Exception) { /* conexión cerrada */ }

            EnUI(() =>
            {
                if (conectado) // el servidor se cayó (no fue una desconexión voluntaria)
                {
                    Desconectar();
                    MessageBox.Show("Se perdió la conexión con el servidor.");
                }
            });
        }

        private void ProcesarLinea(string linea)
        {
            string[] partes = linea.Split('|', 3);

            switch (partes[0])
            {
                case "USERS" when partes.Length >= 2:
                    var nombres = partes[1]
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Where(n => n != miNombre)
                        .OrderBy(n => n)
                        .ToArray();
                    EnUI(() => ActualizarListaUsuarios(nombres));
                    break;

                case "MSG" when partes.Length == 3:
                    string origen = partes[1], texto = partes[2];
                    EnUI(() => RegistrarMensaje(origen, $"{origen}: {texto}"));
                    break;

                case "ERR" when partes.Length >= 2:
                    EnUI(() =>
                    {
                        if (chatActual != null) RegistrarMensaje(chatActual, $"[Aviso] {partes[1]}");
                        else MessageBox.Show(partes[1]);
                    });
                    break;

                case "NICKERR" when partes.Length >= 2:
                    EnUI(() => MessageBox.Show(partes[1], "Nombre rechazado"));
                    break;
            }
        }

        // ---------- Lista de usuarios y conversaciones ----------
        private void ActualizarListaUsuarios(IEnumerable<string> nombres)
        {
            string seleccionado = chatActual;

            lstUsuarios.BeginUpdate();
            lstUsuarios.Items.Clear();
            lstUsuarios.Items.Add(SERVIDOR);
            foreach (string n in nombres) lstUsuarios.Items.Add(n);
            lstUsuarios.EndUpdate();

            if (seleccionado != null)
            {
                int i = lstUsuarios.Items.IndexOf(seleccionado);
                if (i >= 0)
                {
                    lstUsuarios.SelectedIndex = i;
                }
                else
                {
                    lblConversacion.Text = $"Chat con: {seleccionado} (desconectado)";
                    RegistrarMensaje(seleccionado, $"[{seleccionado} se desconectó]");
                }
            }
        }

        private void lstUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsuarios.SelectedItem is not string elegido) return;

            chatActual = elegido;
            lblConversacion.Text = $"Chat con: {elegido}";

            txtChat.Text = conversaciones.TryGetValue(elegido, out var sb) ? sb.ToString() : "";
            txtChat.SelectionStart = txtChat.TextLength;
            txtChat.ScrollToCaret();

            if (lblAviso.Text.Contains(elegido)) lblAviso.Text = "";
            txtMensaje.Focus();
        }

        // Guarda la línea en el historial de esa conversación y la muestra si está abierta
        private void RegistrarMensaje(string conQuien, string linea)
        {
            if (!conversaciones.TryGetValue(conQuien, out var sb))
                conversaciones[conQuien] = sb = new StringBuilder();

            string texto = $"[{DateTime.Now:HH:mm:ss}] {linea}{Environment.NewLine}";
            sb.Append(texto);

            if (conQuien == chatActual) txtChat.AppendText(texto);
            else lblAviso.Text = $"Nuevo mensaje de {conQuien}";
        }

        // ---------- Envío ----------
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (!conectado)
            {
                MessageBox.Show("No hay conexión con el servidor.");
                return;
            }
            if (chatActual == null)
            {
                MessageBox.Show("Selecciona de la lista con quién quieres chatear.");
                return;
            }
            if (!lstUsuarios.Items.Contains(chatActual))
            {
                MessageBox.Show($"{chatActual} ya no está conectado.");
                return;
            }

            string texto = txtMensaje.Text.Trim();
            if (texto == "") return;

            try
            {
                Enviar($"MSG|{chatActual}|{texto}");
                RegistrarMensaje(chatActual, $"Yo: {texto}");
                txtMensaje.Clear();
                txtMensaje.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar: {ex.Message}");
            }
        }

        private void Enviar(string linea)
        {
            lock (lockEscritura) { escritor.WriteLine(linea); }
        }

        // Ejecuta una acción en el hilo de la interfaz
        private void EnUI(Action accion)
        {
            if (IsDisposed) return;
            try
            {
                if (InvokeRequired) Invoke(accion);
                else accion();
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            conectado = false;
            try { clienteSocket?.Close(); } catch { }
            base.OnFormClosed(e);
        }
    }
}
