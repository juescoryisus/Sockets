namespace Sockets
{
    partial class Cliente
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
            label1 = new Label();
            txtNombre = new TextBox();
            lblIp = new Label();
            txtIpServidor = new TextBox();
            btnConectar = new Button();
            lblUsuarios = new Label();
            lstUsuarios = new ListBox();
            lblConversacion = new Label();
            lblAviso = new Label();
            txtChat = new TextBox();
            label2 = new Label();
            txtMensaje = new TextBox();
            btnEnviar = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(59, 20);
            label1.TabIndex = 0;
            label1.Text = "Usuario";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(77, 12);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(135, 27);
            txtNombre.TabIndex = 1;
            // 
            // lblIp
            // 
            lblIp.AutoSize = true;
            lblIp.Location = new Point(228, 15);
            lblIp.Name = "lblIp";
            lblIp.Size = new Size(96, 20);
            lblIp.TabIndex = 2;
            lblIp.Text = "IP servidor";
            // 
            // txtIpServidor
            // 
            txtIpServidor.Location = new Point(330, 12);
            txtIpServidor.Name = "txtIpServidor";
            txtIpServidor.Size = new Size(140, 27);
            txtIpServidor.TabIndex = 3;
            // 
            // btnConectar
            // 
            btnConectar.Location = new Point(485, 11);
            btnConectar.Name = "btnConectar";
            btnConectar.Size = new Size(120, 29);
            btnConectar.TabIndex = 4;
            btnConectar.Text = "Conectar";
            btnConectar.UseVisualStyleBackColor = true;
            btnConectar.Click += btnConectar_Click;
            // 
            // lblUsuarios
            // 
            lblUsuarios.AutoSize = true;
            lblUsuarios.Location = new Point(12, 50);
            lblUsuarios.Name = "lblUsuarios";
            lblUsuarios.Size = new Size(161, 20);
            lblUsuarios.TabIndex = 5;
            lblUsuarios.Text = "Usuarios conectados:";
            // 
            // lstUsuarios
            // 
            lstUsuarios.FormattingEnabled = true;
            lstUsuarios.Location = new Point(12, 73);
            lstUsuarios.Name = "lstUsuarios";
            lstUsuarios.Size = new Size(200, 304);
            lstUsuarios.TabIndex = 6;
            lstUsuarios.SelectedIndexChanged += lstUsuarios_SelectedIndexChanged;
            // 
            // lblConversacion
            // 
            lblConversacion.AutoSize = true;
            lblConversacion.Location = new Point(228, 50);
            lblConversacion.Name = "lblConversacion";
            lblConversacion.Size = new Size(131, 20);
            lblConversacion.TabIndex = 7;
            lblConversacion.Text = "Chat con: (nadie)";
            // 
            // lblAviso
            // 
            lblAviso.AutoSize = true;
            lblAviso.ForeColor = Color.Firebrick;
            lblAviso.Location = new Point(480, 50);
            lblAviso.Name = "lblAviso";
            lblAviso.Size = new Size(0, 20);
            lblAviso.TabIndex = 8;
            // 
            // txtChat
            // 
            txtChat.Location = new Point(228, 73);
            txtChat.Multiline = true;
            txtChat.Name = "txtChat";
            txtChat.ReadOnly = true;
            txtChat.ScrollBars = ScrollBars.Vertical;
            txtChat.Size = new Size(560, 304);
            txtChat.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 398);
            label2.Name = "label2";
            label2.Size = new Size(64, 20);
            label2.TabIndex = 10;
            label2.Text = "Mensaje";
            // 
            // txtMensaje
            // 
            txtMensaje.Location = new Point(82, 395);
            txtMensaje.Name = "txtMensaje";
            txtMensaje.Size = new Size(590, 27);
            txtMensaje.TabIndex = 11;
            // 
            // btnEnviar
            // 
            btnEnviar.Location = new Point(685, 394);
            btnEnviar.Name = "btnEnviar";
            btnEnviar.Size = new Size(103, 29);
            btnEnviar.TabIndex = 12;
            btnEnviar.Text = "Enviar";
            btnEnviar.UseVisualStyleBackColor = true;
            btnEnviar.Click += btnEnviar_Click;
            // 
            // Cliente
            // 
            AcceptButton = btnEnviar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnEnviar);
            Controls.Add(txtMensaje);
            Controls.Add(label2);
            Controls.Add(txtChat);
            Controls.Add(lblAviso);
            Controls.Add(lblConversacion);
            Controls.Add(lstUsuarios);
            Controls.Add(lblUsuarios);
            Controls.Add(btnConectar);
            Controls.Add(txtIpServidor);
            Controls.Add(lblIp);
            Controls.Add(txtNombre);
            Controls.Add(label1);
            Name = "Cliente";
            Text = "Cliente";
            Load += Cliente_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtNombre;
        private Label lblIp;
        private TextBox txtIpServidor;
        private Button btnConectar;
        private Label lblUsuarios;
        private ListBox lstUsuarios;
        private Label lblConversacion;
        private Label lblAviso;
        private TextBox txtChat;
        private Label label2;
        private TextBox txtMensaje;
        private Button btnEnviar;
    }
}
