namespace Sockets
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            btnIniciarServidor = new Button();
            btnClienteNuevo = new Button();
            txtMensajes = new TextBox();
            lstClientes = new ListBox();
            txtMensajeServidor = new TextBox();
            btnEnviarACliente = new Button();
            lblClientes = new Label();
            lblIp = new Label();
            txtIpServidor = new TextBox();
            SuspendLayout();
            // 
            // btnIniciarServidor
            // 
            btnIniciarServidor.Location = new Point(12, 12);
            btnIniciarServidor.Name = "btnIniciarServidor";
            btnIniciarServidor.Size = new Size(175, 29);
            btnIniciarServidor.TabIndex = 0;
            btnIniciarServidor.Text = "Iniciar Servidor";
            btnIniciarServidor.UseVisualStyleBackColor = true;
            btnIniciarServidor.Click += btnIniciarServidor_Click;
            // 
            // btnClienteNuevo
            // 
            btnClienteNuevo.Location = new Point(202, 12);
            btnClienteNuevo.Name = "btnClienteNuevo";
            btnClienteNuevo.Size = new Size(175, 29);
            btnClienteNuevo.TabIndex = 1;
            btnClienteNuevo.Text = "Cliente Nuevo";
            btnClienteNuevo.UseVisualStyleBackColor = true;
            btnClienteNuevo.Click += btnClienteNuevo_Click;
            // 
            // lblIp
            // 
            lblIp.AutoSize = true;
            lblIp.Location = new Point(395, 16);
            lblIp.Name = "lblIp";
            lblIp.Size = new Size(116, 20);
            lblIp.TabIndex = 7;
            lblIp.Text = "IP del servidor:";
            // 
            // txtIpServidor
            // 
            txtIpServidor.Location = new Point(517, 12);
            txtIpServidor.Name = "txtIpServidor";
            txtIpServidor.Size = new Size(160, 27);
            txtIpServidor.TabIndex = 8;
            // 
            // lblClientes
            // 
            lblClientes.AutoSize = true;
            lblClientes.Location = new Point(12, 44);
            lblClientes.Name = "lblClientes";
            lblClientes.Size = new Size(148, 20);
            lblClientes.TabIndex = 6;
            lblClientes.Text = "Clientes conectados:";
            // 
            // lstClientes
            // 
            lstClientes.FormattingEnabled = true;
            lstClientes.Location = new Point(12, 67);
            lstClientes.Name = "lstClientes";
            lstClientes.Size = new Size(200, 304);
            lstClientes.TabIndex = 3;
            // 
            // txtMensajes
            // 
            txtMensajes.Location = new Point(228, 67);
            txtMensajes.Multiline = true;
            txtMensajes.Name = "txtMensajes";
            txtMensajes.ReadOnly = true;
            txtMensajes.ScrollBars = ScrollBars.Vertical;
            txtMensajes.Size = new Size(560, 304);
            txtMensajes.TabIndex = 2;
            // 
            // txtMensajeServidor
            // 
            txtMensajeServidor.Location = new Point(12, 395);
            txtMensajeServidor.Name = "txtMensajeServidor";
            txtMensajeServidor.Size = new Size(640, 27);
            txtMensajeServidor.TabIndex = 4;
            // 
            // btnEnviarACliente
            // 
            btnEnviarACliente.Location = new Point(665, 394);
            btnEnviarACliente.Name = "btnEnviarACliente";
            btnEnviarACliente.Size = new Size(123, 29);
            btnEnviarACliente.TabIndex = 5;
            btnEnviarACliente.Text = "Enviar a cliente";
            btnEnviarACliente.UseVisualStyleBackColor = true;
            btnEnviarACliente.Click += btnEnviarACliente_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtIpServidor);
            Controls.Add(lblIp);
            Controls.Add(lblClientes);
            Controls.Add(btnEnviarACliente);
            Controls.Add(txtMensajeServidor);
            Controls.Add(lstClientes);
            Controls.Add(txtMensajes);
            Controls.Add(btnClienteNuevo);
            Controls.Add(btnIniciarServidor);
            Name = "Form1";
            Text = "Servidor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnIniciarServidor;
        private Button btnClienteNuevo;
        private TextBox txtMensajes;
        private ListBox lstClientes;
        private TextBox txtMensajeServidor;
        private Button btnEnviarACliente;
        private Label lblClientes;
        private Label lblIp;
        private TextBox txtIpServidor;
    }
}
