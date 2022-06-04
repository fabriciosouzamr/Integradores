using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Integradores.Properties;
using static Integradores._Funcoes;
using System.Diagnostics;
using static BancoDados;
using static BancoDados.clsBancoDados;

namespace Integradores
{
  public partial class frmIntegrador : Form
  {
    private bool mouseDown;
    private Point lastLocation;

    System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();

    public frmIntegrador()
    {
      InitializeComponent();
    }

    private void frmIntegrador_MouseDown(object sender, MouseEventArgs e)
    {
      mouseDown = true;
      lastLocation = e.Location;
    }

    private void frmIntegrador_MouseMove(object sender, MouseEventArgs e)
    {
      if (mouseDown)
      {
        this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);
        this.Update();
      }
    }

    private void frmIntegrador_MouseUp(object sender, MouseEventArgs e)
    {
      mouseDown = false;
    }

    protected void frmIntegrador_Load(object sender, EventArgs e)
    {
      string sSql = "";
      DataTable oData = null;

      try
      {
        Config_App.sDB_Tipo = BancoDados_Constantes.const_TipoBancoDados_MySql;
        Config_App.sDB_BancoDados = "i9ativa";
        Config_App.sNomeSistema = Integradores.Properties.Settings.Default.Processador;
        Config_App.sProcessador = Integradores.Properties.Settings.Default.Processador;
        Config_App.sDB_StringConexao = "Server=sql1.plugthink.com;Database=i9ativa;Uid=i9admin;Pwd=8nbzw4FFrXEzJui";

        Criptografia.Criptografia oCriptografia = new Criptografia.Criptografia();

        oCriptografia.Key = Constantes.const_Senha;
        Config_App.sDB_Tipo = Integradores.Properties.Settings.Default.DB_Tipo;
        Config_App.sDB_StringConexao = oCriptografia.Decrypt(Integradores.Properties.Settings.Default.DB_StringConexao);
        //////Config_App.sDB_TabelaView = Integradores.Properties.Settings.Default.DB_TabelaView;
        Config_App.sDB_BancoDados = Integradores.Properties.Settings.Default.DB_Nome;
        Config_App.sNomeSistema = Integradores.Properties.Settings.Default.Processador;
        Config_App.sProcessador = Integradores.Properties.Settings.Default.Processador;

        oCriptografia = null;
      }
      catch (Exception Ex)
      {
        sSql = Ex.Message;
      }

      try
      {
        appLog = new EventLog();
        appLog.Source = Config_App.sNomeSistema + " [Desktop]";
        appLog.Log = "Application";
        appLog.WriteEntry("Serviço Iniciando", EventLogEntryType.Information);
        Config.appLog = appLog;
      }
      catch (Exception Ex)
      {
        MessageBox.Show(Ex.Message);
        MessageBox.Show("Abra o aplicativo como administrador");
        Application.Exit();
        return;
      }

      try
      {
        Declaracao.oMensagem_log = new Mensagem_log();

        appLog.WriteEntry("Serviço conectando Banco de Dados", EventLogEntryType.Information);
        Config.dbconstring = Config_App.sDB_StringConexao;
        Integrador_Funcoes.oBancoDados = new clsBancoDados();
        Integrador_Funcoes.oBancoDados.DBConectar(Config_App.sDB_Tipo, Config.dbconstring);

        if (Config_App.sProcessador.Trim() != "")
        {
          lblRSistema.Text = lblRSistema.Text + " (" + Config_App.sProcessador.Trim() + ")";
          lblHostName.Text = System.Environment.MachineName.Trim().ToUpper() + " - " + Config_App.sProcessador.Trim();

          Declaracao.processador = Config_App.sProcessador.Trim() + " - " + Application.ProductVersion.ToString();
        }
        else
        {
          lblHostName.Text = System.Environment.MachineName.Trim().ToUpper();
        }
      }
      catch (Exception Ex)
      {
        Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message, "", true);
      }

      lblTarefa_Informacao.Text = "";

      timTarefasPendentes.Enabled = false;

      Integrador_Funcoes.oBancoDados = new clsBancoDados();
      Integrador_Funcoes.oBancoDados.DBConectar(Config_App.sDB_Tipo, Config_App.sDB_StringConexao);

      if (!Integrador_Funcoes.oBancoDados.DBConectado())
      {
        pnlControles.Enabled = false;
        appLog.WriteEntry("Serviço não conseguiu conectar Banco de Dados - " +
                          Config_App.sDB_StringConexao, EventLogEntryType.Information);
      }
      else
      {
        sSql = "select idIntegrador from tb_Integrador where NomeIntegrador = '" + Config_App.sProcessador + "'";
        oData = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

        if (oData.Rows.Count == 0)
        {

        }
        else
        {
          Config_App.idIntegrador = Convert.ToInt32(oData.Rows[0]["idIntegrador"]);

          sSql = "update tb_Integrador set hostname = '" + Dns.GetHostName().Trim() + "' where idIntegrador = " + Config_App.idIntegrador.ToString();
          Integrador_Funcoes.oBancoDados.DBExecutar(sSql);

          Tarefa_Carregar();

          timTarefasPendentes.Enabled = true;
          appLog.WriteEntry("Serviço Iniciado", EventLogEntryType.Information);
        }
      }
    }

    private void btnSair_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Deseja realmente sair do integrador?", Constantes.const_Sistema_Nome, MessageBoxButtons.YesNo) == DialogResult.Yes)
      {
        Application.Exit();
      }
    }

    private void frmIntegrador_FormClosing(object sender, FormClosingEventArgs e)
    {
      appLog.WriteEntry("Serviço Parado", EventLogEntryType.Information);
      Integrador_Funcoes.oBancoDados.DBDesconectar();
    }

    public static Boolean SofiaWhatsapp_Processar(int iIdTarefa,
                                                  string sConexaoOrigem,
                                                  string sConexaoDestino)
    {
      Boolean bOk = false;

      DataTable oData;
      string sSql = "";
      string[] sMensagem = new string[1];

      Config.FlexXTools_Usuario = "flagwhats4280en";
      Config.FlexXTools_Senha = "Odfofot59flag2344959";

      Bot.Terms_Carregar();

      sSql = "select bot.idBot, mes.idMessage, mes.message_body, mes.contact_name, mes.contact_uid, mes.Agente, bot.Token, bot.nroOrigem" +
             " from tbmessage mes" +
              " inner join tbbot bot on trim(bot.Apelido) = trim(mes.Agente)" +
             " where mes.idStatusMensagem <> 1 and mes.message_body is not null and upper(mes.message_diretion) = 'I'" +
               " and EventoWZ = 'message'" +
               " and  ifnull(upper(trim(bot.HostLeitura)), 'X') in ('X', '" + System.Environment.MachineName.Trim().ToUpper() + "', '" +
                                                                              Config_App.sProcessador.ToUpper() + "')";
      oData = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

      if (Declaracao.otbmessageterms != null)
      {
        foreach (DataRow Row in oData.Rows)
        {
          Integradores.WhatsApp.EnviarMensagem("", "", "", Row["message_body"].ToString());
        }
      }

      return bOk;
    }

    private void Tarefa_Processar(Boolean bProcessarAgora = false)
    {
      int iIdEmpresa = -1;
      string[,] oParametro = null;
      string ds_parametro = "";
      int iSegundosParaLeituras = 0;

      if (!Integrador_Funcoes.oBancoDados.DBConectado()) { return; }
      Integrador_Funcoes.Processamento_Inicio = Integrador_Funcoes.oBancoDados.DBData();

      Integrador_Funcoes.Propriedade_Carregar();

      timTarefasPendentes.Enabled = false;

      //SofiaWhatsapp_Processar(0,
      //                        "Server=bd2.pedidodireto.com;Port=3306;Database=botservicehomogation;Uid=botsofia;Pwd=4gcCeuMV99URvZb;SslMode=none",
      //                        txtSofia_Conexao.Text);

      Tarefa_Carregar();

      Application.DoEvents();

      Integrador_Funcoes.ProductVersion = Application.ProductVersion;

      Integrador_Funcoes.oBancoDados.DBReconectar(Config_App.sDB_Tipo, Config_App.sDB_StringConexao);
      Integrador_Funcoes.oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_Integrador_Ping_upd", new clsCampo[] { new clsCampo { Nome = "_idIntegrador", Tipo = DbType.Double, Valor = Config_App.idIntegrador } });
      Integrador_Funcoes.oBancoDados.DBReconectar(Config_App.sDB_Tipo, Config_App.sDB_StringConexao);

      foreach (DataGridViewRow oRow_Tarefas in dgwTarefas.Rows)
      {
        iIdEmpresa = Convert.ToInt32(oRow_Tarefas.Cells["idEmpresa"].Value);

        if (FNC_NuloString(oRow_Tarefas.Cells["ds_parametro"].Value).Trim() != "")
        {
          ds_parametro = FNC_NuloString(oRow_Tarefas.Cells["ds_parametro"].Value).Trim();

          ds_parametro = ds_parametro.Replace("[TELEFONE_EMPRESA]", FNC_NuloString(oRow_Tarefas.Cells["CodTelefone"].Value).Trim());
          ds_parametro = ds_parametro.Replace("[COD_PUXADA]", FNC_NuloString(oRow_Tarefas.Cells["CodPuxada"].Value).Trim());
          oParametro = Parametro_Desmontar(ds_parametro);
        }

        if (Convert.ToDateTime(oRow_Tarefas.Cells["dtExecucao"].Value) <= Integrador_Funcoes.Processamento_Inicio || bProcessarAgora)
        {
          Application.DoEvents();

          try
          {
            Integrador_Funcoes.Processar(iIdEmpresa,
                                         Convert.ToInt32(oRow_Tarefas.Cells["idEmpresasServicos_GrupoTarefas"].Value),
                                         Convert.ToInt32(oRow_Tarefas.Cells["idTipoIntegracao"].Value),
                                         Convert.ToInt32(oRow_Tarefas.Cells["idEmpresaIntegracao"].Value),
                                         Convert.ToInt32(oRow_Tarefas.Cells["idTarefa"].Value),
                                         FNC_NuloString(oRow_Tarefas.Cells["Partner"].Value).ToString(),
                                         FNC_NuloString(oRow_Tarefas.Cells["cd_Aplicativo"].Value).ToString(),
                                         FNC_NuloString(oRow_Tarefas.Cells["cd_Servico"].Value).ToString(),
                                         FNC_NuloString(oRow_Tarefas.Cells["Tarefa"].Value).ToString(),
                                         FNC_NuloString(oRow_Tarefas.Cells["NomeArquivo"].Value).ToString(),
                                         oRow_Tarefas.Cells["cdBancoConexaoOrigem"].Value.ToString(),
                                         oRow_Tarefas.Cells["ds_stringconexaoOrigem"].Value.ToString(),
                                         oRow_Tarefas.Cells["ds_stringconexaoDestino"].Value.ToString(),
                                         oRow_Tarefas.Cells["tp_bancodadosOrigem"].Value.ToString(),
                                         oRow_Tarefas.Cells["tp_bancodadosDestino"].Value.ToString(),
                                         oRow_Tarefas.Cells["ds_usuario"].Value.ToString(),
                                         oRow_Tarefas.Cells["ds_senha"].Value.ToString(),
                                         Parametro_Valor(oParametro, "COD_PUXADA"),
                                         Parametro_Valor(oParametro, "TIPO_REGISTRO"),
                                         Parametro_Valor(oParametro, "TEL_CELULAR"),
                                         Parametro_Valor(oParametro, "TIPO_CONSULTA"),
                                         Parametro_Valor(oParametro, "VISAO_FATURAMENTO"),
                                         Parametro_Valor(oParametro, "FILTRO_EXCLUSAO"),
                                         Parametro_Valor(oParametro, "FILTRO_SELECAO"),
                                         Parametro_Valor(oParametro, "QUERY_ATUALIZACAO"),
                                         FNC_NuloString(oRow_Tarefas.Cells["ds_origem"].Value).ToString(),
                                         FNC_NuloString(oRow_Tarefas.Cells["ds_destino"].Value).ToString(),
                                         FNC_NuloString(oRow_Tarefas.Cells["ds_destino_executar"].Value).ToString(),
                                         FNC_NuloString(oRow_Tarefas.Cells["tp_leituradados"].Value).ToString(),
                                         Convert.ToInt32(oRow_Tarefas.Cells["nr_ordem_execucao"].Value),
                                         FNC_NuloString(oRow_Tarefas.Cells["Log_Tools_Integrador"].Value).ToString() == "S",
                                         _Funcoes.FNC_NuloString(oRow_Tarefas.Cells["Api_Log"].Value));
          }
          catch (Exception Ex)
          {
            Log_Gravar(0, LogTipo.ErroNaRotina_ProcessarTarefas, 0, Ex.Message, "");

            iSegundosParaLeituras = Properties.Settings.Default.SegundosParaLeituras;
          }
        }
      }

      Tarefa_Carregar();

      timTarefasPendentes.Enabled = true;
    }

    private void btnTarefa_ProcessarAgora_Click(object sender, EventArgs e)
    {
      Tarefa_Processar(true);
    }

    private void Tarefa_Carregar()
    {
      try
      {
        string sSql = "SELECT *" +
                      " FROM " + Config_App.sDB_BancoDados.Trim() + ".vw_tarefas_executar" +
                      " WHERE HostLeitura in ('X', '" + Config_App.sProcessador.ToUpper().Trim() + "')" +
                      " ORDER BY idEmpresa,nr_ordem_execucao";
        dgwTarefas.DataSource = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

        dgwTarefas.ScrollBars = ScrollBars.Horizontal;
        dgwTarefas.Refresh();
      }
      catch (Exception Ex)
      {
        Log_Gravar(0, LogTipo.ErroLeituraTarefas, 0, Ex.Message, "");
      }
    }

    private void Tarefa_Excluir(int iId_Tarefa)
    {
      if (Funcoes.FNC_Perguntar("Você deseja realmente excluir essa tarefa?"))
      {
        string sSql = "delete from gertarefas where idTarefa = " + iId_Tarefa.ToString();

        Integrador_Funcoes.oBancoDados.DBExecutar(sSql);
      }
    }

    private void timTarefasPendentes_Tick(object sender, EventArgs e)
    {
      Tarefa_Processar();
    }

    private void btnTarefa_Excluir_Click(object sender, EventArgs e)
    {
      if (dgwTarefas.SelectedRows.Count == 0)
      {
        Funcoes.FNC_Mensagem("Selecione a tarefa a ser excluida", Funcoes.Mensasagem_Tipo.Informacao);
        return;
      }

      foreach (DataGridViewRow oRow in dgwTarefas.SelectedRows)
      {
        Tarefa_Excluir(Convert.ToInt32(oRow.Cells["idTarefa"].Value));
      }

      Tarefa_Carregar();

      Funcoes.FNC_Mensagem("Exclusão Efetuada", Funcoes.Mensasagem_Tipo.Informacao);
    }

    private void cmdSofia_Enviar_Click(object sender, EventArgs e)
    {
      //SofiaWhatsapp_EnviarTexto(0, txtSofia_Conexao.Text, txtSofia_Token.Text, txtSofia_UId.Text, txtSofia_To.Text, txtSofia_Texto.Text);
    }

    private void lblHostName_Click(object sender, EventArgs e)
    {
      Clipboard.SetText(lblHostName.Text);
    }

    public Boolean DBSQL_Integrador_Gravar(string sDs_servidor_entrada,
                                           string sDs_servidor_saida,
                                           string sDs_usuario,
                                           string sDs_senha,
                                           string sCd_codigo_copia_arquivo,
                                           string sCd_codigo_empresa,
                                           string sDs_pasta_tabelas,
                                           string sDs_pasta_imagens,
                                           string sDs_pasta_importacao,
                                           string sDs_pasta_exportacao,
                                           string sDs_pasta_lokalizei,
                                           string sDs_separador_campo,
                                           string sDs_separador_colunas,
                                           string sIc_igorar_erro_saldo_estoque,
                                           string sIc_desativar_execucao_automatica,
                                           string sIc_gerar_tabela_preco,
                                           string sIc_re_enviar_imagens,
                                           string sIc_gerar_log_imagens,
                                           string sIc_enviar_pedido_direto,
                                           string sIc_enviar_estoque)
    {
      Boolean bOk = false;

      try
      {
        Integrador_Funcoes.oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_integrador_cad", new clsCampo[] {
                                                                                        new clsCampo {Nome = "p_ds_servidor_entrada", Tipo = DbType.String, Valor = sDs_servidor_entrada},
                                                                                        new clsCampo {Nome = "p_ds_servidor_saida", Tipo = DbType.String, Valor = sDs_servidor_saida },
                                                                                        new clsCampo {Nome = "p_ds_usuario", Tipo = DbType.String, Valor = sDs_usuario },
                                                                                        new clsCampo {Nome = "p_ds_senha", Tipo = DbType.String, Valor = sDs_senha },
                                                                                        new clsCampo {Nome = "p_cd_codigo_copia_arquivo", Tipo = DbType.String, Valor = sCd_codigo_copia_arquivo },
                                                                                        new clsCampo {Nome = "p_cd_codigo_empresa", Tipo = DbType.String, Valor = sCd_codigo_empresa },
                                                                                        new clsCampo {Nome = "p_ds_pasta_tabelas", Tipo = DbType.String, Valor = sDs_pasta_tabelas },
                                                                                        new clsCampo {Nome = "p_ds_pasta_imagens", Tipo = DbType.String, Valor = sDs_pasta_imagens },
                                                                                        new clsCampo {Nome = "p_ds_pasta_importacao", Tipo = DbType.String, Valor = sDs_pasta_importacao },
                                                                                        new clsCampo {Nome = "p_ds_pasta_exportacao", Tipo = DbType.String, Valor = sDs_pasta_exportacao },
                                                                                        new clsCampo {Nome = "p_ds_pasta_lokalizei", Tipo = DbType.String, Valor = sDs_pasta_lokalizei },
                                                                                        new clsCampo {Nome = "p_ds_separador_campo", Tipo = DbType.String, Valor = sDs_separador_campo },
                                                                                        new clsCampo {Nome = "p_ds_separador_colunas", Tipo = DbType.String, Valor = sDs_separador_colunas },
                                                                                        new clsCampo {Nome = "p_ic_igorar_erro_saldo_estoque", Tipo = DbType.String, Valor = sIc_igorar_erro_saldo_estoque },
                                                                                        new clsCampo {Nome = "p_ic_desativar_execucao_automatica", Tipo = DbType.String, Valor = sIc_desativar_execucao_automatica },
                                                                                        new clsCampo {Nome = "p_ic_gerar_tabela_preco", Tipo = DbType.String, Valor = sIc_gerar_tabela_preco },
                                                                                        new clsCampo {Nome = "p_ic_re_enviar_imagens", Tipo = DbType.String, Valor = sIc_re_enviar_imagens },
                                                                                        new clsCampo {Nome = "p_ic_gerar_log_imagens", Tipo = DbType.String, Valor = sIc_gerar_log_imagens },
                                                                                        new clsCampo {Nome = "p_ic_enviar_pedido_direto", Tipo = DbType.String, Valor = sIc_enviar_pedido_direto },
                                                                                        new clsCampo {Nome = "p_ic_enviar_estoque", Tipo = DbType.String, Valor = sIc_enviar_estoque }});

        bOk = true;
      }
      catch (Exception Ex)
      {
        Log_Gravar(0, LogTipo.Integrador_Configuracao, 0, Ex.Message, "");
      }

      return bOk;
    }

    private static void Pausar()
    {

    }

    public Boolean Log_Gravar(int iId_Empresa,
                              LogTipo idOperacao,
                              long iIdRegistro,
                              string sDs_log,
                              string sNomeArquivo,
                              bool ExibirMensagem = false)
    {
      bool bOk = false;

      bOk = Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar("", "", "", "", "", "", "",
                                                            iId_Empresa, idOperacao, iIdRegistro, sNomeArquivo + " >> " + sDs_log);

      if (ExibirMensagem)
      {
        MessageBox.Show(sDs_log);
      }

      return bOk;
    }
  }
}
