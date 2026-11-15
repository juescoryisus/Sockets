using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Sockets
{
    public partial class Form1 : Form
    {
        private const int PUERTO = 6400;
        private const string SERVIDOR = "[Servidor]";

        private Socket server;

        // Protocolo (una línea de texto por mensaje, campos separados por '|'):
        //   Cliente -> Servidor:  NICK|nombre          (registro, primer mensaje)
        //                         MSG|destino|texto    (mensaje para otro usuario o para [Servidor])
        //   Servidor -> Cliente:  USERS|a,b,c          (lista de conectados)
        //                         MSG|origen|texto     (mensaje recibido)
        //                         NICKERR|motivo       (nombre rechazado)
        //                         ERR|motivo           (error al enviar)

        private class Conexion
        {
            public Socket Socket;
            public StreamWriter Writer;
            public string Nombre;
            public string Endpoint;

            public void Enviar(string linea)
            {
                try { lock (Writer) { Writer.WriteLine(linea); } }
                catch { /* el cliente ya se fue */ }
            }
        }

        // nombre de usuario -> su conexión
        private readonly Dictionary<string, Conexion> clientes = new(StringComparer.OrdinalIgnoreCase);
        private readonly object lockClientes = new();

        public Form1()
        {
            InitializeComponent();
            txtIpServidor.Text = ObtenerIpLocal();
        }

        private static string ObtenerIpLocal()
        {
            try
            {
                foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip.ToString();
                }
            }
            catch { }
            return "127.0.0.1";
        }

        // ---------- Inicio del servidor ----------
        private void btnIniciarServidor_Click(object sender, EventArgs e)
        {
            if (!IPAddress.TryParse(txtIpServidor.Text.Trim(), out IPAddress ip))
            {
                MessageBox.Show("La IP no es válida. Ejemplo: 192.168.1.10");
                return;
            }

            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(ip, PUERTO));
                server.Listen(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo levantar el servidor en {ip}:{PUERTO}\n{ex.Message}");
                server?.Close();
                server = null;
                return;
            }

            btnIniciarServidor.Enabled = false;
            txtIpServidor.Enabled = false;
            AgregarLog($"[SERVIDOR] Escuchando en {ip}:{PUERTO}...");

            Task.Run(AceptarClientes);
        }

        private void AceptarClientes()
        {
            try
            {
                while (true)
                {
                    Socket conexion = server.Accept();
                    Task.Run(() => AtenderCliente(conexion));
                }
            }
            catch (Exception ex)
            {
                AgregarLog($"[ERROR] Servidor: {ex.Message}");
            }
        }

        // ---------- Atención de cada cliente ----------
        private void AtenderCliente(Socket socket)
        {
            string endpoint = socket.RemoteEndPoint?.ToString() ?? "?";
            Conexion conn = null;
            bool registrado = false;

            try
            {
                var stream = new NetworkStream(socket);
                var lector = new StreamReader(stream, Encoding.UTF8);
                var escritor = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

                // 1) El primer mensaje debe ser el registro del nombre
                string primera = lector.ReadLine();
                if (primera == null || !primera.StartsWith("NICK|")) return;

                string nombre = primera.Substring(5).Trim();
                string error = ValidarNombre(nombre);

                conn = new Conexion { Socket = socket, Writer = escritor, Nombre = nombre, Endpoint = endpoint };

                if (error == null)
                {
                    lock (lockClientes)
                    {
                        if (clientes.ContainsKey(nombre)) error = "Ese nombre ya está en uso.";
                        else { clientes[nombre] = conn; registrado = true; }
                    }
                }

                if (!registrado)
                {
                    conn.Enviar($"NICKERR|{error}");
                    return;
                }

                AgregarClienteALista(nombre);
                AgregarLog($"[CONEXIÓN] {nombre} conectado desde {endpoint}");
                EnviarListaATodos();

                // 2) Ciclo de mensajes
                string linea;
                while ((linea = lector.ReadLine()) != null)
                    ProcesarLinea(conn, linea);
            }
            catch (Exception) { /* el cliente cerró bruscamente */ }
            finally
            {
                if (registrado)
                {
                    lock (lockClientes) { clientes.Remove(conn.Nombre); }
                    QuitarClienteDeLista(conn.Nombre);
                    AgregarLog($"[DESCONEXIÓN] {conn.Nombre} ({endpoint}) se desconectó");
                    EnviarListaATodos();
                }
                try { socket.Close(); } catch { }
            }
        }

        private static string ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "El nombre no puede estar vacío.";
            if (nombre.Length > 20) return "El nombre es demasiado largo (máx. 20).";
            if (nombre.IndexOfAny(new[] { '|', ',', '[', ']' }) >= 0)
                return "El nombre no puede contener | , [ ]";
            return null;
        }

        private void ProcesarLinea(Conexion origen, string linea)
        {
            string[] partes = linea.Split('|', 3);

            if (partes[0] == "MSG" && partes.Length == 3)
            {
                string destinoNombre = partes[1];
                string texto = partes[2];

                // Mensaje dirigido al servidor
                if (destinoNombre == SERVIDOR)
                {
                    AgregarLog($"(de {origen.Nombre} para el servidor) {texto}");
                    return;
                }

                Conexion destino;
                lock (lockClientes) { clientes.TryGetValue(destinoNombre, out destino); }

                if (destino == null)
                {
                    origen.Enviar($"ERR|{destinoNombre} ya no está conectado.");
                    return;
                }

                destino.Enviar($"MSG|{origen.Nombre}|{texto}");
                AgregarLog($"(de {origen.Nombre} para {destino.Nombre}) {texto}");
            }
        }

        // Manda a todos la lista actualizada de usuarios conectados
        private void EnviarListaATodos()
        {
            List<Conexion> destinos;
            string lista;

            lock (lockClientes)
            {
                destinos = clientes.Values.ToList();
                lista = string.Join(",", clientes.Keys);
            }

            foreach (Conexion c in destinos)
                c.Enviar($"USERS|{lista}");
        }

        // ---------- Envío del servidor al cliente seleccionado ----------
        private void btnEnviarACliente_Click(object sender, EventArgs e)
        {
            if (lstClientes.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un cliente de la lista.");
                return;
            }

            string texto = txtMensajeServidor.Text.Trim();
            if (texto == "")
            {
                MessageBox.Show("Escribe un mensaje.");
                return;
            }

            string nombre = lstClientes.SelectedItem.ToString();
            Conexion destino;

            lock (lockClientes) { clientes.TryGetValue(nombre, out destino); }

            if (destino == null)
            {
                MessageBox.Show("Ese cliente ya no está conectado.");
                return;
            }

            destino.Enviar($"MSG|{SERVIDOR}|{texto}");
            AgregarLog($"(para {nombre}) {texto}");
            txtMensajeServidor.Clear();
        }

        // ---------- Utilidades de UI (seguras entre hilos) ----------
        private void AgregarClienteALista(string nombre)
        {
            if (IsDisposed) return;
            try
            {
                if (lstClientes.InvokeRequired)
                {
                    lstClientes.Invoke(new Action(() => AgregarClienteALista(nombre)));
                    return;
                }
                lstClientes.Items.Add(nombre);
            }
            catch (ObjectDisposedException) { }
        }

        private void QuitarClienteDeLista(string nombre)
        {
            if (IsDisposed) return;
            try
            {
                if (lstClientes.InvokeRequired)
                {
                    lstClientes.Invoke(new Action(() => QuitarClienteDeLista(nombre)));
                    return;
                }
                lstClientes.Items.Remove(nombre);
            }
            catch (ObjectDisposedException) { }
        }

        private void AgregarLog(string texto)
        {
            if (IsDisposed) return;
            try
            {
                if (txtMensajes.InvokeRequired)
                {
                    txtMensajes.Invoke(new Action(() => AgregarLog(texto)));
                }
                else
                {
                    txtMensajes.AppendText($"[{DateTime.Now:HH:mm:ss}] {texto}{Environment.NewLine}");
                }
            }
            catch (ObjectDisposedException) { }
        }

        private void btnClienteNuevo_Click(object sender, EventArgs e)
        {
            Cliente cliente = new Cliente(txtIpServidor.Text.Trim());
            cliente.Show();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try { server?.Close(); } catch { }
            base.OnFormClosed(e);
        }
    }
}
