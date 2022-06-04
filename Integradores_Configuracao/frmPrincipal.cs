using Integradores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static BancoDados;
using static BancoDados.clsBancoDados;

namespace Integradores_Configuracao
{
    public partial class frmPrincipal : Form
    {
        private const string const_Senha = "gltuWmM5Nr";

        private const string const_TipoBancoDados_MySql = "MYSQL";
        private const string const_TipoBancoDados_SqlServer = "SQLSRV";
        private const string const_TipoBancoDados_Firebird = "FIREBIRD";

        private const string const_Senha_Administrador = "Admin";
        private const string const_Senha_Usuario = "Usuário";

        private bool mouseDown;
        private Point lastLocation;

        Criptografia.Criptografia oCriptografia = new Criptografia.Criptografia();

        string sArquivoConfiguracao_Windows = "";
        string sArquivoConfiguracao_Servico = "";

        string sSenha_Administrador = "";
        string sWebHook_Url = "";
        string sNomeBanco = "";
        string sStringConexao_Descricao_Windows = "";
        string sStringConexao_Descricao_Servico = "";
        string sStringConexao_Tipo_Windows = "";
        string sStringConexao_Tipo_Servico = "";

        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void cmdGravar_Click(object sender, EventArgs e)
        {
            string sAux = "";

            if (txtSenha_Administrador_Atual.Text.Trim() != sSenha_Administrador)
            {
                MessageBox.Show("A senha de administrador atual não é válida");
                return;
            }
            if (txtSenha_Administrador_Nova.Text.Trim() != txtSenha_Administrador_Validar.Text.Trim())
            {
                MessageBox.Show("A nova senha de administrador não está igual a senha de validação");
                return;
            }

            string[] ArquivoConfiguracao = new string[] { sArquivoConfiguracao_Windows, sArquivoConfiguracao_Servico };
            string sSenha = "";

            if (txtSenha_Administrador_Nova.Text != "")
            {
                sSenha = txtSenha_Administrador_Nova.Text.Trim();
            }
            else
            {
                sSenha = txtSenha_Administrador_Atual.Text.Trim();
            }

            {
                try
                {
                    foreach (string sArquivoConfiguracao in ArquivoConfiguracao)
                    {
                        if (System.IO.File.Exists(sArquivoConfiguracao))
                        {
                            XElement xml = XElement.Load(sArquivoConfiguracao);
                            foreach (XElement x in xml.Elements())
                            {
                                if (x.Name.ToString() == "applicationSettings")
                                {
                                    foreach (XElement x1 in x.Elements())
                                    {
                                        foreach (XElement x2 in x1.Elements())
                                        {
                                            switch (x2.FirstAttribute.Value)
                                            {
                                                case "Processador":
                                                    {
                                                        if (sArquivoConfiguracao == sArquivoConfiguracao_Servico)
                                                        {
                                                            x2.Elements().ElementAt(0).SetValue(txtNomeProcessamento.Text.Trim());
                                                            break;
                                                        }
                                                        else if (sArquivoConfiguracao == sArquivoConfiguracao_Windows)
                                                        {
                                                            x2.Elements().ElementAt(0).SetValue(txtNomeProcessamento.Text.Trim());
                                                            break;
                                                        }

                                                        break;
                                                    }
                                                case "WebHook_Url":
                                                    {
                                                        sAux = oCriptografia.Encrypt(txtWebHook_Url.Text);
                                                        x2.Elements().ElementAt(0).SetValue(sAux);
                                                        break;
                                                    }
                                                case "NomeSistema":
                                                    {
                                                        break;
                                                    }
                                                case "DB_TabelaView":
                                                    {
                                                        break;
                                                    }
                                                case "DB_Tipo":
                                                    {
                                                        x2.Elements().ElementAt(0).SetValue(cboStringConexao_Tipo_Servico.SelectedValue);

                                                        break;
                                                    }
                                                case "DB_StringConexao":
                                                    {
                                                        sAux = oCriptografia.Encrypt(txtStringConexao_Descricao_Servico.Text);
                                                        x2.Elements().ElementAt(0).SetValue(sAux);
                                                        break;
                                                    }
                                                case "DB_Nome":
                                                    {
                                                        x2.Elements().ElementAt(0).SetValue(txtBancoDados.Text);
                                                        break;
                                                    }
                                                case "Senha_Administrador":
                                                    {
                                                        sAux = oCriptografia.Encrypt(sSenha);
                                                        x2.Elements().ElementAt(0).SetValue(sAux);
                                                        break;
                                                    }
                                            }
                                        }
                                    }

                                    xml.Save(sArquivoConfiguracao);
                                }
                            }
                        }
                    }

                    txtSenha_Administrador_Atual.Text = "";

                    ArquivoConfiguracao_Carregar();
                    
                    MessageBox.Show("Configuração Gravada");
                }
                catch (Exception Ex)
                {
                }
            }
        }

        private void cmdFechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            DirectoryInfo diretorio = new DirectoryInfo(System.IO.Path.GetDirectoryName(Application.ExecutablePath));

            string ArquivoLog = String.Empty;

            oCriptografia.Key = const_Senha;

            ComboBox_Carregar(cboStringConexao_Tipo_Servico, new string[] { "Firebird", "My Sql", "Sql Server" },
                                                             new object[] { const_TipoBancoDados_Firebird, const_TipoBancoDados_MySql, const_TipoBancoDados_SqlServer });          

            foreach (FileInfo f in diretorio.GetFiles())
            {
                if (f.Extension == ".config")
                {
                    switch ( f.Name)
                    {
                        case "Integradores_Service.exe.config":
                            {
                                sArquivoConfiguracao_Servico = f.FullName;
                                break;
                            }
                        case "Integradores.exe.config":
                            {
                                sArquivoConfiguracao_Windows = f.FullName;
                                break;
                            }
                    }
                }
            }

            ArquivoConfiguracao_Carregar();

            tbcGeral.TabPages.Remove(tbpArquivos);
            tbcGeral.TabPages.Remove(tbpTemplates);

            Integrador_Funcoes.oBancoDados = new clsBancoDados();

            try
            {
                Integrador_Funcoes.oBancoDados.DBConectar(sStringConexao_Tipo_Windows, sStringConexao_Descricao_Windows);

                ComboBox_Carregar(cboTemplate_Descricao, "select idMessageTerms, trim(Agente) + ' - ' + trim(Command) dsMessageTerms from tb_messageterms where Agente is not null");
            }
            catch (Exception)
            {
            }
        }

        private void ComboBox_Carregar(System.Windows.Forms.ComboBox oCombo, string[] Texto, object[] Codigo = null, bool bAdicionarDados = false, bool bInserirInicio = false)
        {
            DataTable oData = new DataTable();
            DataRowCollection RC;
            DataRow oRow;
            object[] oRowVal = new object[2];
            int iCont;

            if (bAdicionarDados)
                oData = (DataTable)oCombo.DataSource;
            else
            {
                oData.Columns.Add("Codigo");
                oData.Columns.Add("Descricao");
            }

            RC = oData.Rows;

            for (iCont = 0; iCont <= Texto.Length - 1; iCont++)
            {
                if (Codigo != null)
                    oRowVal[0] = Codigo[iCont];
                oRowVal[1] = Texto[iCont];

                if (bInserirInicio)
                {
                    oRow = oData.NewRow();
                    oRow[0] = oRowVal[0];
                    oRow[1] = oRowVal[1];
                    RC.InsertAt(oRow, 0);
                }
                else
                    oRow = RC.Add(oRowVal);
            }

            oCombo.AutoCompleteSource = AutoCompleteSource.ListItems;

            oCombo.DisplayMember = oData.Columns[1].ColumnName;
            oCombo.ValueMember = oData.Columns[0].ColumnName;
            oCombo.DataSource = oData;
        }

        private void ComboBox_Carregar(System.Windows.Forms.ComboBox oCombo, string sSql)
        {
            if (Integrador_Funcoes.oBancoDados.DBConectado())
            {
                DataTable oData = null;

                oData = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

                oCombo.AutoCompleteSource = AutoCompleteSource.ListItems;

                oCombo.DisplayMember = oData.Columns[1].ColumnName;
                oCombo.ValueMember = oData.Columns[0].ColumnName;
                oCombo.DataSource = oData;
            }
        }

        private void ComboBox_Posicionar(System.Windows.Forms.ComboBox oCombo, object vValor, int CdColuna = 0, bool bSetarTAG = false)
        {
            oCombo.SelectedIndex = ComboBox_VerificarIndice(oCombo, vValor, CdColuna);

            if (bSetarTAG)
                oCombo.Tag = oCombo.SelectedIndex;
        }
        public int ComboBox_VerificarIndice(System.Windows.Forms.ComboBox oCombo, object vValor, int CdColuna = 0)
        {
            int iIndice = 0;
            bool bAchou = false;

            foreach (DataRowView Item in oCombo.Items)
            {
                if (Item[0].ToString() == vValor.ToString())
                {
                    bAchou = true;
                    break;
                }

                iIndice = iIndice + 1;
            }

            if (bAchou == true)
                return iIndice;
            else
                return -1;
        }

        private string Arquivo_Dialogo_Abrir()
        {
            OpenFileDialog oDialogoAbrir = new OpenFileDialog();
            string sArquivo = "";

            oDialogoAbrir.ShowDialog();
            oDialogoAbrir.DefaultExt = ".xml|*.xml";

            if (oDialogoAbrir.FileName != "")
            {
                try
                {
                    sArquivo = oDialogoAbrir.FileName;
                }
                catch (Exception ex)
                {
                }
            }

            return sArquivo;
        }

        private void pnlLogo_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void pnlLogo_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        private void pnlLogo_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private string oCriptografia_Decrypt(string sTexo)
        {
            string sRet = "";

            try
            {
                sRet = oCriptografia.Decrypt(sTexo);
            }
            catch (Exception)
            {
            }

            return sRet;
        }

        private void ArquivoConfiguracao_Carregar()
        { 
            try
            {
                string[] ArquivoConfiguracao = new string[] { sArquivoConfiguracao_Windows, sArquivoConfiguracao_Servico };

                Configuracao_Limpar();

                foreach (string sArquivoConfiguracao in ArquivoConfiguracao)
                {
                    if (System.IO.File.Exists(sArquivoConfiguracao))
                    {
                        XElement xml = XElement.Load(sArquivoConfiguracao);
                        foreach (XElement x in xml.Elements())
                        {
                            if (x.Name.ToString() == "applicationSettings")
                            {
                                foreach (XElement x1 in x.Elements())
                                {
                                    foreach (XElement x2 in x1.Elements())
                                    {
                                        switch (x2.FirstAttribute.Value)
                                        {
                                            case "Processador":
                                                {
                                                    if (x2.Value.Trim() != "")
                                                    {
                                                        txtNomeProcessamento.Text = x2.Value.Trim();
                                                    }

                                                    break;
                                                }
                                            case "WebHook_Url":
                                                {
                                                    sWebHook_Url = oCriptografia_Decrypt(x2.Value);
                                                    break;
                                                }
                                            case "NomeSistema":
                                                {
                                                    break;
                                                }
                                            case "DB_TabelaView":
                                                {
                                                    break;
                                                }
                                            case "DB_Tipo":
                                                {
                                                    if (sArquivoConfiguracao == sArquivoConfiguracao_Windows)
                                                    {
                                                        sStringConexao_Tipo_Windows = x2.Value;
                                                    }
                                                    else if (sArquivoConfiguracao == sArquivoConfiguracao_Servico)
                                                    {
                                                        sStringConexao_Tipo_Servico = x2.Value;
                                                    }

                                                    break;
                                                }
                                            case "DB_StringConexao":
                                                {
                                                    if (sArquivoConfiguracao == sArquivoConfiguracao_Windows)
                                                    {                                                       
                                                        sStringConexao_Descricao_Windows = oCriptografia_Decrypt(x2.Value);
                                                    }
                                                    else if (sArquivoConfiguracao == sArquivoConfiguracao_Servico)
                                                    {
                                                        sStringConexao_Descricao_Servico = oCriptografia_Decrypt(x2.Value);
                                                    }

                                                    break;
                                                }
                                            case "DB_Nome":
                                                {
                                                    sNomeBanco = oCriptografia_Decrypt(x2.Value);
                                                    break;
                                                }
                                            case "Senha_Administrador":
                                                {
                                                    if (x2.Value != "")
                                                    {
                                                        sSenha_Administrador = oCriptografia_Decrypt(x2.Value);

                                                        if (sSenha_Administrador == null) { sSenha_Administrador = ""; }
                                                    }

                                                    break;
                                                }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Senha_Administrador_Validar();
            }
            catch (Exception Ex)
            {
                sSenha_Administrador = Ex.Message;
            }
        }

        private void txtSenha_Administrador_Leave(object sender, EventArgs e)
        {
            Senha_Administrador_Validar();
        }

        private void Configuracao_Limpar()
        {
            cboStringConexao_Tipo_Servico.SelectedIndex = -1;
            txtStringConexao_Descricao_Servico.Text = "";
            txtWebHook_Url.Text = "";
            txtBancoDados.Text = "";

            txtSenha_Administrador_Nova.Text = "";
            txtSenha_Administrador_Validar.Text = "";

            grpStringConexaoServidorTarefas_Servico.Enabled = false;
            grpConfiguacoesGerais.Enabled = false;
            cmdGravar.Enabled = false;
        }

        private void Senha_Administrador_Validar()
        {
            txtSenha_Administrador_Nova.Text = "";
            txtSenha_Administrador_Validar.Text = "";

            Configuracao_Limpar();

            if (txtSenha_Administrador_Atual.Text == sSenha_Administrador || sSenha_Administrador == "")
            {
                txtSenha_Administrador_Atual.BackColor = SystemColors.Window;
                txtSenha_Administrador_Nova.BackColor = SystemColors.Window;
                txtSenha_Administrador_Validar.BackColor = SystemColors.Window;
                txtSenha_Administrador_Nova.ReadOnly = false;
                txtSenha_Administrador_Validar.ReadOnly = false;

                if (sSenha_Administrador != "")
                {
                    ComboBox_Posicionar(cboStringConexao_Tipo_Servico, sStringConexao_Tipo_Servico);
                    txtStringConexao_Descricao_Servico.Text = sStringConexao_Descricao_Servico;
                    txtWebHook_Url.Text = sWebHook_Url;
                    txtBancoDados.Text = sNomeBanco;
                }
            }
            else
            {
                txtSenha_Administrador_Atual.BackColor = System.Drawing.Color.Red;
                txtSenha_Administrador_Nova.BackColor = System.Drawing.Color.LightGray;
                txtSenha_Administrador_Validar.BackColor = System.Drawing.Color.LightGray;
                txtSenha_Administrador_Nova.ReadOnly = true;
                txtSenha_Administrador_Validar.ReadOnly = true;
            }

            grpStringConexaoServidorTarefas_Servico.Enabled = (txtSenha_Administrador_Atual.Text == sSenha_Administrador || sSenha_Administrador == "");
            grpConfiguacoesGerais.Enabled = (txtSenha_Administrador_Atual.Text == sSenha_Administrador || sSenha_Administrador == "");
            cmdGravar.Enabled = (txtSenha_Administrador_Atual.Text == sSenha_Administrador || sSenha_Administrador == "");
        }

        private void btnSalvarConfiguracao_Click(object sender, EventArgs e)
        {
            //if (DBSQL_Integrador_Gravar(txtServidorEntrada.Text,
            //                            txtServidorSaida.Text,
            //                            txtUsuario.Text,
            //                            txtSenha.Text,
            //                            txtCodigoCopiarArquivo.Text,
            //                            txtCodigoEmpresa.Text,
            //                            txtPastaTabelas.Text,
            //                            txtPastaImagens.Text,
            //                            txtPastaImportacao.Text,
            //                            txtPastaExportacao.Text,
            //                            txtPastaLokalizei.Text,
            //                            txtSeparadorCampo.Text,
            //                            txtSeparadorColunas.Text,
            //                            ((chkIgonarErroSaldoEstoque.Checked) ? "S" : "N"),
            //                            ((chkDesativarExecucaoAutomatica.Checked) ? "S" : "N"),
            //                            ((chkGerarTabelaPrecos.Checked) ? "S" : "N"),
            //                            ((chkReEnviarImagens.Checked) ? "S" : "N"),
            //                            ((chkGerarLogImagens.Checked) ? "S" : "N"),
            //                            ((chkEnviarParaPedidoDireto.Checked) ? "S" : "N"),
            //                            ((chkEnviarEstoque.Checked) ? "S" : "N")))
            {
                //_Funcoes.FNC_Mensagem("Configuração Salva", Funcoes.Mensasagem_Tipo.Informacao);
            }
        }

        private void CarregarConfiguracao()
        {
            DataTable oData;

            oData = Integrador_Funcoes.oBancoDados.DBQuery("select * from tb_integrador");

            if (oData.Rows.Count != 0)
            {
                txtServidorEntrada.Text = oData.Rows[0]["ds_servidor_entrada"].ToString();
                txtServidorSaida.Text = oData.Rows[0]["ds_servidor_saida"].ToString();
                txtUsuario.Text = oData.Rows[0]["ds_usuario"].ToString();
                txtSenha.Text = oData.Rows[0]["ds_senha"].ToString();
                txtCodigoCopiarArquivo.Text = oData.Rows[0]["cd_codigo_copia_arquivo"].ToString();
                txtCodigoEmpresa.Text = oData.Rows[0]["cd_codigo_empresa"].ToString();
                txtPastaTabelas.Text = oData.Rows[0]["ds_pasta_tabelas"].ToString();
                txtPastaImagens.Text = oData.Rows[0]["ds_pasta_imagens"].ToString();
                txtPastaImportacao.Text = oData.Rows[0]["ds_pasta_importacao"].ToString();
                txtPastaExportacao.Text = oData.Rows[0]["ds_pasta_exportacao"].ToString();
                txtPastaLokalizei.Text = oData.Rows[0]["ds_pasta_lokalizei"].ToString();
                txtSeparadorCampo.Text = oData.Rows[0]["ds_separador_campo"].ToString();
                txtSeparadorCampo.Text = oData.Rows[0]["ds_separador_colunas"].ToString();
                chkIgonarErroSaldoEstoque.Checked = (oData.Rows[0]["ic_igorar_erro_saldo_estoque"].ToString() == "S");
                chkDesativarExecucaoAutomatica.Checked = (oData.Rows[0]["ic_desativar_execucao_automatica"].ToString() == "S");
                chkGerarTabelaPrecos.Checked = (oData.Rows[0]["ic_gerar_tabela_preco"].ToString() == "S");
                chkReEnviarImagens.Checked = (oData.Rows[0]["ic_re_enviar_imagens"].ToString() == "S");
                chkGerarLogImagens.Checked = (oData.Rows[0]["ic_gerar_log_imagens"].ToString() == "S");
                chkEnviarParaPedidoDireto.Checked = (oData.Rows[0]["ic_enviar_pedido_direto"].ToString() == "S");
                chkEnviarEstoque.Checked = (oData.Rows[0]["ic_enviar_estoque"].ToString() == "S");

                if (System.Windows.Forms.SystemInformation.ComputerName.Trim() == "DESKTOP-KIENLH1")
                {
                    txtPastaImportacao.Text = "P:\\Trabalho\\Github\\T2U_Integrador2018\\Rotinas";
                }
            }
        }

        private void cmdTemplate_Gravar_Click(object sender, EventArgs e)
        {
            if (cboTemplate_Descricao.SelectedIndex != -1)
            {
                string sSql;

                sSql = "update tb_messageterms" +
                       " set dsTerms=#dsTerms," +
                            "dsTemplateHeader=#dsTemplateHeader," +
                            "dsTemplate=#dsTemplate," +
                            "dsTemplateFooter=#dsTemplateFooter" +
                       " where idMessageTerms = " + cboTemplate_Descricao.SelectedValue.ToString();
                Integrador_Funcoes.oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "dsTerms", Tipo = DbType.String, Valor = txtTemplate_Terms.Text },
                                                                                 new clsCampo { Nome = "dsTemplateHeader", Tipo = DbType.String, Valor = txtTemplate_Cabecalho.Text },
                                                                                 new clsCampo { Nome = "dsTemplate", Tipo = DbType.String, Valor = txtTemplate_Detalhe.Text },
                                                                                 new clsCampo { Nome = "dsTemplateFooter", Tipo = DbType.String, Valor = txtTemplate_Rodape.Text } });

                MessageBox.Show("Gravação Efetuada");
            }
        }

        private void cboTemplate_Descricao_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable oData;
                string sSql;

                sSql = "select * from tb_messageterms where idMessageTerms = " + cboTemplate_Descricao.SelectedValue.ToString();

                oData = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

                if (oData != null)
                {
                    txtTemplate_Terms.Text = _Funcoes.FNC_NuloString(oData.Rows[0]["dsTerms"]);
                    txtTemplate_Cabecalho.Text = _Funcoes.FNC_NuloString(oData.Rows[0]["dsTemplateHeader"]);
                    txtTemplate_Detalhe.Text = _Funcoes.FNC_NuloString(oData.Rows[0]["dsTemplate"]);
                    txtTemplate_Rodape.Text = _Funcoes.FNC_NuloString(oData.Rows[0]["dsTemplateFooter"]);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmdServico_Instalar_Click(object sender, EventArgs e)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(Application.ExecutablePath);

                string nomeArquivo = fileInfo.DirectoryName + "\\Install.bat";

                if (System.IO.File.Exists(nomeArquivo))
                {
                    System.IO.File.Delete(nomeArquivo);
                }

                // Cria um novo arquivo e devolve um StreamWriter para ele
                StreamWriter writer = new StreamWriter(nomeArquivo);

                // Agora é só sair escrevendo
                writer.WriteLine("sc create \"" +  txtNomeProcessamento.Text +  "\" binpath= \"" + fileInfo.DirectoryName + "\\Integradores_Service.exe" + "\"");

                // Não esqueça de fechar o arquivo ao terminar

                writer.Close();

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine("sc create \"" + txtNomeProcessamento.Text + "\" binpath= \"" + fileInfo.DirectoryName + "\\Integradores_Service.exe" + "\"");
                cmd.StandardInput.Flush();
                cmd.StandardInput.WriteLine("sc description \"" + txtNomeProcessamento.Text + "\" \"" + txtNomeProcessamento.Text + "\"");
                cmd.StandardInput.Flush();
                cmd.StandardInput.WriteLine("sc start \"" + txtNomeProcessamento.Text + "\"");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                Console.WriteLine(cmd.StandardOutput.ReadToEnd());

                MessageBox.Show("Serviço instalado");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void cmdServico_Desinstalar_Click(object sender, EventArgs e)
        {

            try
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine("sc stop \"" + txtNomeProcessamento.Text + "\"");
                cmd.StandardInput.Flush();
                cmd.StandardInput.WriteLine("sc delete \"" + txtNomeProcessamento.Text + "\"");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                Console.WriteLine(cmd.StandardOutput.ReadToEnd());

                MessageBox.Show("Serviço desinstalado");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
    }
}
