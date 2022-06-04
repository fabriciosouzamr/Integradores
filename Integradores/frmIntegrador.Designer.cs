namespace Integradores
{
    partial class frmIntegrador
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmIntegrador));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlLogo = new System.Windows.Forms.Panel();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblRVersao = new System.Windows.Forms.Label();
            this.lblRSistema_Sub = new System.Windows.Forms.Label();
            this.lblRSistema = new System.Windows.Forms.Label();
            this.pnlControles = new System.Windows.Forms.Panel();
            this.tbcGeral = new System.Windows.Forms.TabControl();
            this.tbpPrincipal = new System.Windows.Forms.TabPage();
            this.lblHostName = new System.Windows.Forms.Label();
            this.lblLicenca_CNPJ = new System.Windows.Forms.Label();
            this.lblLicenca_Endereco = new System.Windows.Forms.Label();
            this.lblLicenca_RazaoSocial = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.label25 = new System.Windows.Forms.Label();
            this.tbpTarefas = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTarefa_Informacao = new System.Windows.Forms.Label();
            this.btnTarefa_ProcessarAgora = new System.Windows.Forms.Button();
            this.btnTarefa_Excluir = new System.Windows.Forms.Button();
            this.pnlTarefas_Painel = new System.Windows.Forms.Panel();
            this.dgwTarefas = new System.Windows.Forms.DataGridView();
            this.pnlTarefas_Titulo = new System.Windows.Forms.Panel();
            this.label19 = new System.Windows.Forms.Label();
            this.timTarefasPendentes = new System.Windows.Forms.Timer(this.components);
            this.idTarefa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idEmpresaIntegracao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idTipoIntegracao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tp_bancodadosOrigem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tp_bancodadosDestino = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tarefa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NomeArquivo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataTarefa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ds_stringconexaoOrigem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ds_stringconexaoDestino = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ds_parametro = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ds_token = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ds_usuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ds_senha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodTelefone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idEmpresa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodPuxada = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tp_leituradados = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlLogo.SuspendLayout();
            this.pnlControles.SuspendLayout();
            this.tbcGeral.SuspendLayout();
            this.tbpPrincipal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.tbpTarefas.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlTarefas_Painel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwTarefas)).BeginInit();
            this.pnlTarefas_Titulo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLogo
            // 
            this.pnlLogo.BackColor = System.Drawing.Color.DodgerBlue;
            this.pnlLogo.Controls.Add(this.btnSair);
            this.pnlLogo.Controls.Add(this.lblRVersao);
            this.pnlLogo.Controls.Add(this.lblRSistema_Sub);
            this.pnlLogo.Controls.Add(this.lblRSistema);
            this.pnlLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogo.Location = new System.Drawing.Point(0, 0);
            this.pnlLogo.Name = "pnlLogo";
            this.pnlLogo.Size = new System.Drawing.Size(1258, 66);
            this.pnlLogo.TabIndex = 0;
            this.pnlLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmIntegrador_MouseDown);
            this.pnlLogo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmIntegrador_MouseMove);
            this.pnlLogo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmIntegrador_MouseUp);
            // 
            // btnSair
            // 
            this.btnSair.AutoSize = true;
            this.btnSair.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Image = ((System.Drawing.Image)(resources.GetObject("btnSair.Image")));
            this.btnSair.Location = new System.Drawing.Point(1201, 5);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(51, 55);
            this.btnSair.TabIndex = 4;
            this.btnSair.TabStop = false;
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // lblRVersao
            // 
            this.lblRVersao.AutoSize = true;
            this.lblRVersao.ForeColor = System.Drawing.Color.Transparent;
            this.lblRVersao.Location = new System.Drawing.Point(1125, 46);
            this.lblRVersao.Name = "lblRVersao";
            this.lblRVersao.Size = new System.Drawing.Size(79, 13);
            this.lblRVersao.TabIndex = 2;
            this.lblRVersao.Text = "Versão 5.8.543";
            // 
            // lblRSistema_Sub
            // 
            this.lblRSistema_Sub.AutoSize = true;
            this.lblRSistema_Sub.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRSistema_Sub.ForeColor = System.Drawing.Color.White;
            this.lblRSistema_Sub.Location = new System.Drawing.Point(12, 34);
            this.lblRSistema_Sub.Name = "lblRSistema_Sub";
            this.lblRSistema_Sub.Size = new System.Drawing.Size(214, 25);
            this.lblRSistema_Sub.TabIndex = 1;
            this.lblRSistema_Sub.Text = "Integrador Trade2UP";
            // 
            // lblRSistema
            // 
            this.lblRSistema.AutoSize = true;
            this.lblRSistema.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRSistema.ForeColor = System.Drawing.Color.White;
            this.lblRSistema.Location = new System.Drawing.Point(56, 9);
            this.lblRSistema.Name = "lblRSistema";
            this.lblRSistema.Size = new System.Drawing.Size(126, 13);
            this.lblRSistema.TabIndex = 0;
            this.lblRSistema.Text = "Trade2UP Integrador";
            // 
            // pnlControles
            // 
            this.pnlControles.Controls.Add(this.tbcGeral);
            this.pnlControles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControles.Location = new System.Drawing.Point(0, 66);
            this.pnlControles.Name = "pnlControles";
            this.pnlControles.Size = new System.Drawing.Size(1258, 417);
            this.pnlControles.TabIndex = 1;
            // 
            // tbcGeral
            // 
            this.tbcGeral.Controls.Add(this.tbpPrincipal);
            this.tbcGeral.Controls.Add(this.tbpTarefas);
            this.tbcGeral.Location = new System.Drawing.Point(3, 6);
            this.tbcGeral.Name = "tbcGeral";
            this.tbcGeral.SelectedIndex = 0;
            this.tbcGeral.Size = new System.Drawing.Size(1254, 409);
            this.tbcGeral.TabIndex = 1;
            // 
            // tbpPrincipal
            // 
            this.tbpPrincipal.Controls.Add(this.lblHostName);
            this.tbpPrincipal.Controls.Add(this.lblLicenca_CNPJ);
            this.tbpPrincipal.Controls.Add(this.lblLicenca_Endereco);
            this.tbpPrincipal.Controls.Add(this.lblLicenca_RazaoSocial);
            this.tbpPrincipal.Controls.Add(this.label21);
            this.tbpPrincipal.Controls.Add(this.label20);
            this.tbpPrincipal.Controls.Add(this.picLogo);
            this.tbpPrincipal.Controls.Add(this.label25);
            this.tbpPrincipal.Location = new System.Drawing.Point(4, 22);
            this.tbpPrincipal.Name = "tbpPrincipal";
            this.tbpPrincipal.Padding = new System.Windows.Forms.Padding(3);
            this.tbpPrincipal.Size = new System.Drawing.Size(1246, 383);
            this.tbpPrincipal.TabIndex = 0;
            this.tbpPrincipal.Text = "1. Principal";
            this.tbpPrincipal.UseVisualStyleBackColor = true;
            // 
            // lblHostName
            // 
            this.lblHostName.AutoSize = true;
            this.lblHostName.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblHostName.Location = new System.Drawing.Point(5, 93);
            this.lblHostName.Name = "lblHostName";
            this.lblHostName.Size = new System.Drawing.Size(105, 13);
            this.lblHostName.TabIndex = 8;
            this.lblHostName.Text = "00.000.000/0000-00";
            this.lblHostName.Click += new System.EventHandler(this.lblHostName_Click);
            // 
            // lblLicenca_CNPJ
            // 
            this.lblLicenca_CNPJ.AutoSize = true;
            this.lblLicenca_CNPJ.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblLicenca_CNPJ.Location = new System.Drawing.Point(5, 75);
            this.lblLicenca_CNPJ.Name = "lblLicenca_CNPJ";
            this.lblLicenca_CNPJ.Size = new System.Drawing.Size(105, 13);
            this.lblLicenca_CNPJ.TabIndex = 6;
            this.lblLicenca_CNPJ.Text = "00.000.000/0000-00";
            // 
            // lblLicenca_Endereco
            // 
            this.lblLicenca_Endereco.AutoSize = true;
            this.lblLicenca_Endereco.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblLicenca_Endereco.Location = new System.Drawing.Point(5, 58);
            this.lblLicenca_Endereco.Name = "lblLicenca_Endereco";
            this.lblLicenca_Endereco.Size = new System.Drawing.Size(136, 13);
            this.lblLicenca_Endereco.TabIndex = 5;
            this.lblLicenca_Endereco.Text = "Belo Horizonte - MG - Brasil";
            // 
            // lblLicenca_RazaoSocial
            // 
            this.lblLicenca_RazaoSocial.AutoSize = true;
            this.lblLicenca_RazaoSocial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicenca_RazaoSocial.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblLicenca_RazaoSocial.Location = new System.Drawing.Point(5, 41);
            this.lblLicenca_RazaoSocial.Name = "lblLicenca_RazaoSocial";
            this.lblLicenca_RazaoSocial.Size = new System.Drawing.Size(152, 13);
            this.lblLicenca_RazaoSocial.TabIndex = 4;
            this.lblLicenca_RazaoSocial.Text = "em Liberação no Servidor";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label21.Location = new System.Drawing.Point(5, 24);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(84, 13);
            this.label21.TabIndex = 3;
            this.label21.Text = "Licenciado Para";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label20.Location = new System.Drawing.Point(5, 5);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(385, 13);
            this.label20.TabIndex = 2;
            this.label20.Text = "_______________________________________________________________";
            // 
            // picLogo
            // 
            this.picLogo.Image = ((System.Drawing.Image)(resources.GetObject("picLogo.Image")));
            this.picLogo.Location = new System.Drawing.Point(961, 298);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(278, 79);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picLogo.TabIndex = 1;
            this.picLogo.TabStop = false;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label25.Location = new System.Drawing.Point(5, 97);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(385, 13);
            this.label25.TabIndex = 7;
            this.label25.Text = "_______________________________________________________________";
            // 
            // tbpTarefas
            // 
            this.tbpTarefas.Controls.Add(this.panel1);
            this.tbpTarefas.Controls.Add(this.pnlTarefas_Painel);
            this.tbpTarefas.Controls.Add(this.pnlTarefas_Titulo);
            this.tbpTarefas.Location = new System.Drawing.Point(4, 22);
            this.tbpTarefas.Name = "tbpTarefas";
            this.tbpTarefas.Padding = new System.Windows.Forms.Padding(3);
            this.tbpTarefas.Size = new System.Drawing.Size(1246, 383);
            this.tbpTarefas.TabIndex = 1;
            this.tbpTarefas.Text = "2. Tarefas";
            this.tbpTarefas.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblTarefa_Informacao);
            this.panel1.Controls.Add(this.btnTarefa_ProcessarAgora);
            this.panel1.Controls.Add(this.btnTarefa_Excluir);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 311);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1240, 69);
            this.panel1.TabIndex = 2;
            // 
            // lblTarefa_Informacao
            // 
            this.lblTarefa_Informacao.AutoSize = true;
            this.lblTarefa_Informacao.Location = new System.Drawing.Point(5, 5);
            this.lblTarefa_Informacao.Name = "lblTarefa_Informacao";
            this.lblTarefa_Informacao.Size = new System.Drawing.Size(107, 13);
            this.lblTarefa_Informacao.TabIndex = 825;
            this.lblTarefa_Informacao.Text = "lblTarefa_Informacao";
            // 
            // btnTarefa_ProcessarAgora
            // 
            this.btnTarefa_ProcessarAgora.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnTarefa_ProcessarAgora.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTarefa_ProcessarAgora.ForeColor = System.Drawing.Color.White;
            this.btnTarefa_ProcessarAgora.Location = new System.Drawing.Point(1057, 20);
            this.btnTarefa_ProcessarAgora.Name = "btnTarefa_ProcessarAgora";
            this.btnTarefa_ProcessarAgora.Size = new System.Drawing.Size(183, 43);
            this.btnTarefa_ProcessarAgora.TabIndex = 824;
            this.btnTarefa_ProcessarAgora.Text = "Processar Agora";
            this.btnTarefa_ProcessarAgora.UseVisualStyleBackColor = false;
            this.btnTarefa_ProcessarAgora.Click += new System.EventHandler(this.btnTarefa_ProcessarAgora_Click);
            // 
            // btnTarefa_Excluir
            // 
            this.btnTarefa_Excluir.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnTarefa_Excluir.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTarefa_Excluir.ForeColor = System.Drawing.Color.White;
            this.btnTarefa_Excluir.Location = new System.Drawing.Point(5, 20);
            this.btnTarefa_Excluir.Name = "btnTarefa_Excluir";
            this.btnTarefa_Excluir.Size = new System.Drawing.Size(183, 43);
            this.btnTarefa_Excluir.TabIndex = 823;
            this.btnTarefa_Excluir.Text = "Excluir Tarefa";
            this.btnTarefa_Excluir.UseVisualStyleBackColor = false;
            this.btnTarefa_Excluir.Click += new System.EventHandler(this.btnTarefa_Excluir_Click);
            // 
            // pnlTarefas_Painel
            // 
            this.pnlTarefas_Painel.Controls.Add(this.dgwTarefas);
            this.pnlTarefas_Painel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTarefas_Painel.Location = new System.Drawing.Point(3, 38);
            this.pnlTarefas_Painel.Name = "pnlTarefas_Painel";
            this.pnlTarefas_Painel.Size = new System.Drawing.Size(1240, 342);
            this.pnlTarefas_Painel.TabIndex = 1;
            // 
            // dgwTarefas
            // 
            this.dgwTarefas.AllowUserToAddRows = false;
            this.dgwTarefas.AllowUserToDeleteRows = false;
            this.dgwTarefas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idTarefa,
            this.idEmpresaIntegracao,
            this.idTipoIntegracao,
            this.tp_bancodadosOrigem,
            this.tp_bancodadosDestino,
            this.Tarefa,
            this.NomeArquivo,
            this.DataTarefa,
            this.ds_stringconexaoOrigem,
            this.ds_stringconexaoDestino,
            this.ds_parametro,
            this.ds_token,
            this.ds_usuario,
            this.ds_senha,
            this.CodTelefone,
            this.idEmpresa,
            this.CodPuxada,
            this.tp_leituradados});
            this.dgwTarefas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgwTarefas.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgwTarefas.Location = new System.Drawing.Point(0, 0);
            this.dgwTarefas.Name = "dgwTarefas";
            this.dgwTarefas.Size = new System.Drawing.Size(1240, 342);
            this.dgwTarefas.TabIndex = 0;
            // 
            // pnlTarefas_Titulo
            // 
            this.pnlTarefas_Titulo.BackColor = System.Drawing.Color.DodgerBlue;
            this.pnlTarefas_Titulo.Controls.Add(this.label19);
            this.pnlTarefas_Titulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTarefas_Titulo.Location = new System.Drawing.Point(3, 3);
            this.pnlTarefas_Titulo.Name = "pnlTarefas_Titulo";
            this.pnlTarefas_Titulo.Size = new System.Drawing.Size(1240, 35);
            this.pnlTarefas_Titulo.TabIndex = 0;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.White;
            this.label19.Location = new System.Drawing.Point(5, 5);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(195, 25);
            this.label19.TabIndex = 2;
            this.label19.Text = "Tarefas Pendentes";
            // 
            // timTarefasPendentes
            // 
            this.timTarefasPendentes.Interval = 1000;
            this.timTarefasPendentes.Tick += new System.EventHandler(this.timTarefasPendentes_Tick);
            // 
            // idTarefa
            // 
            this.idTarefa.DataPropertyName = "idTarefa";
            this.idTarefa.HeaderText = "#";
            this.idTarefa.Name = "idTarefa";
            this.idTarefa.Width = 40;
            // 
            // idEmpresaIntegracao
            // 
            this.idEmpresaIntegracao.DataPropertyName = "idEmpresaIntegracao";
            this.idEmpresaIntegracao.HeaderText = "idEmpresaIntegracao";
            this.idEmpresaIntegracao.Name = "idEmpresaIntegracao";
            this.idEmpresaIntegracao.Visible = false;
            // 
            // idTipoIntegracao
            // 
            this.idTipoIntegracao.DataPropertyName = "idTipoIntegracao";
            this.idTipoIntegracao.HeaderText = "idTipoIntegracao";
            this.idTipoIntegracao.Name = "idTipoIntegracao";
            this.idTipoIntegracao.Visible = false;
            // 
            // tp_bancodadosOrigem
            // 
            this.tp_bancodadosOrigem.DataPropertyName = "tp_bancodadosOrigem";
            this.tp_bancodadosOrigem.HeaderText = "tp_bancodadosOrigem";
            this.tp_bancodadosOrigem.Name = "tp_bancodadosOrigem";
            this.tp_bancodadosOrigem.Visible = false;
            // 
            // tp_bancodadosDestino
            // 
            this.tp_bancodadosDestino.DataPropertyName = "tp_bancodadosDestino";
            this.tp_bancodadosDestino.HeaderText = "tp_bancodadosDestino";
            this.tp_bancodadosDestino.Name = "tp_bancodadosDestino";
            this.tp_bancodadosDestino.Visible = false;
            // 
            // Tarefa
            // 
            this.Tarefa.DataPropertyName = "Tarefa";
            this.Tarefa.HeaderText = "Tarefa";
            this.Tarefa.Name = "Tarefa";
            this.Tarefa.ReadOnly = true;
            // 
            // NomeArquivo
            // 
            this.NomeArquivo.DataPropertyName = "NomeArquivo";
            this.NomeArquivo.HeaderText = "NomeArquivo";
            this.NomeArquivo.Name = "NomeArquivo";
            this.NomeArquivo.ReadOnly = true;
            // 
            // DataTarefa
            // 
            this.DataTarefa.DataPropertyName = "DataTarefa";
            dataGridViewCellStyle1.Format = "G";
            dataGridViewCellStyle1.NullValue = null;
            this.DataTarefa.DefaultCellStyle = dataGridViewCellStyle1;
            this.DataTarefa.HeaderText = "DataTarefa";
            this.DataTarefa.Name = "DataTarefa";
            this.DataTarefa.ReadOnly = true;
            // 
            // ds_stringconexaoOrigem
            // 
            this.ds_stringconexaoOrigem.DataPropertyName = "ds_stringconexaoOrigem";
            this.ds_stringconexaoOrigem.HeaderText = "Conexão de Origem";
            this.ds_stringconexaoOrigem.Name = "ds_stringconexaoOrigem";
            this.ds_stringconexaoOrigem.Width = 200;
            // 
            // ds_stringconexaoDestino
            // 
            this.ds_stringconexaoDestino.DataPropertyName = "ds_stringconexaoDestino";
            this.ds_stringconexaoDestino.HeaderText = "Conexão de Destino";
            this.ds_stringconexaoDestino.Name = "ds_stringconexaoDestino";
            this.ds_stringconexaoDestino.Width = 200;
            // 
            // ds_parametro
            // 
            this.ds_parametro.DataPropertyName = "ds_parametro";
            this.ds_parametro.HeaderText = "ds_parametro";
            this.ds_parametro.Name = "ds_parametro";
            this.ds_parametro.Visible = false;
            // 
            // ds_token
            // 
            this.ds_token.DataPropertyName = "ds_token";
            this.ds_token.HeaderText = "Token";
            this.ds_token.Name = "ds_token";
            // 
            // ds_usuario
            // 
            this.ds_usuario.DataPropertyName = "ds_usuario";
            this.ds_usuario.HeaderText = "Usuário";
            this.ds_usuario.Name = "ds_usuario";
            // 
            // ds_senha
            // 
            this.ds_senha.DataPropertyName = "ds_senha";
            this.ds_senha.HeaderText = "Senha";
            this.ds_senha.Name = "ds_senha";
            // 
            // CodTelefone
            // 
            this.CodTelefone.DataPropertyName = "CodTelefone";
            this.CodTelefone.HeaderText = "CodTelefone";
            this.CodTelefone.Name = "CodTelefone";
            this.CodTelefone.Visible = false;
            // 
            // idEmpresa
            // 
            this.idEmpresa.DataPropertyName = "idEmpresa";
            this.idEmpresa.HeaderText = "idEmpresa";
            this.idEmpresa.Name = "idEmpresa";
            this.idEmpresa.Visible = false;
            // 
            // CodPuxada
            // 
            this.CodPuxada.DataPropertyName = "CodPuxada";
            this.CodPuxada.HeaderText = "CodPuxada";
            this.CodPuxada.Name = "CodPuxada";
            this.CodPuxada.Visible = false;
            // 
            // tp_leituradados
            // 
            this.tp_leituradados.DataPropertyName = "tp_leituradados";
            this.tp_leituradados.HeaderText = "tp_leituradados";
            this.tp_leituradados.Name = "tp_leituradados";
            this.tp_leituradados.ReadOnly = true;
            this.tp_leituradados.Visible = false;
            // 
            // frmIntegrador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1258, 483);
            this.Controls.Add(this.pnlControles);
            this.Controls.Add(this.pnlLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmIntegrador";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Integrador";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmIntegrador_FormClosing);
            this.Load += new System.EventHandler(this.frmIntegrador_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmIntegrador_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmIntegrador_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmIntegrador_MouseUp);
            this.pnlLogo.ResumeLayout(false);
            this.pnlLogo.PerformLayout();
            this.pnlControles.ResumeLayout(false);
            this.tbcGeral.ResumeLayout(false);
            this.tbpPrincipal.ResumeLayout(false);
            this.tbpPrincipal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.tbpTarefas.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlTarefas_Painel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgwTarefas)).EndInit();
            this.pnlTarefas_Titulo.ResumeLayout(false);
            this.pnlTarefas_Titulo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlLogo;
        private System.Windows.Forms.Panel pnlControles;
        private System.Windows.Forms.Label lblRSistema;
        private System.Windows.Forms.Label lblRSistema_Sub;
        private System.Windows.Forms.Label lblRVersao;
        private System.Windows.Forms.TabControl tbcGeral;
        private System.Windows.Forms.TabPage tbpPrincipal;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.TabPage tbpTarefas;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Panel pnlTarefas_Painel;
        private System.Windows.Forms.Panel pnlTarefas_Titulo;
        private System.Windows.Forms.DataGridView dgwTarefas;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnTarefa_ProcessarAgora;
        private System.Windows.Forms.Button btnTarefa_Excluir;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lblLicenca_RazaoSocial;
        private System.Windows.Forms.Label lblLicenca_CNPJ;
        private System.Windows.Forms.Label lblLicenca_Endereco;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label lblTarefa_Informacao;
        private System.Windows.Forms.Timer timTarefasPendentes;
        private System.Windows.Forms.Label lblHostName;
        private System.Windows.Forms.DataGridViewTextBoxColumn idTarefa;
        private System.Windows.Forms.DataGridViewTextBoxColumn idEmpresaIntegracao;
        private System.Windows.Forms.DataGridViewTextBoxColumn idTipoIntegracao;
        private System.Windows.Forms.DataGridViewTextBoxColumn tp_bancodadosOrigem;
        private System.Windows.Forms.DataGridViewTextBoxColumn tp_bancodadosDestino;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tarefa;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeArquivo;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataTarefa;
        private System.Windows.Forms.DataGridViewTextBoxColumn ds_stringconexaoOrigem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ds_stringconexaoDestino;
        private System.Windows.Forms.DataGridViewTextBoxColumn ds_parametro;
        private System.Windows.Forms.DataGridViewTextBoxColumn ds_token;
        private System.Windows.Forms.DataGridViewTextBoxColumn ds_usuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn ds_senha;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodTelefone;
        private System.Windows.Forms.DataGridViewTextBoxColumn idEmpresa;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodPuxada;
        private System.Windows.Forms.DataGridViewTextBoxColumn tp_leituradados;
    }
}