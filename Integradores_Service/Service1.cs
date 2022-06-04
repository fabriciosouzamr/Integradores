using Integradores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using static BancoDados;
using System.Net;
using static BancoDados.clsBancoDados;

namespace Integradores_Service
{
  public enum ServiceState
  {
    SERVICE_START_PENDING = 0x00000002,
    SERVICE_RUNNING = 0x00000004,

    SERVICE_STOPPED = 0x00000001,
    SERVICE_STOP_PENDING = 0x00000003,

    SERVICE_CONTINUE_PENDING = 0x00000005,
    SERVICE_PAUSE_PENDING = 0x00000006,
    SERVICE_PAUSED = 0x00000007,
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct ServiceStatus
  {
    public int dwServiceType;
    public ServiceState dwCurrentState;
    public int dwControlsAccepted;
    public int dwWin32ExitCode;
    public int dwServiceSpecificExitCode;
    public int dwCheckPoint;
    public int dwWaitHint;
  };

  public partial class Service1 : ServiceBase
  {
    private DateTime Processamento_Inicio;
    ServiceStatus serviceStatus = new ServiceStatus();

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

    public Service1(string[] args)
    {
      InitializeComponent();

      string sErro = "";

      try
      {
        Criptografia.Criptografia oCriptografia = new Criptografia.Criptografia();

        oCriptografia.Key = Constantes.const_Senha;
        Config_App.sDB_Tipo = Integradores_Service.Properties.Settings.Default.DB_Tipo;
        Config_App.sDB_StringConexao = oCriptografia.Decrypt(Integradores_Service.Properties.Settings.Default.DB_StringConexao);
        Config_App.sDB_TabelaView = Integradores_Service.Properties.Settings.Default.DB_TabelaView;
        Config_App.sNomeSistema = Integradores_Service.Properties.Settings.Default.Processador;
        Config_App.sProcessador = Integradores_Service.Properties.Settings.Default.Processador;
        Config_App.sWebHook_Url = oCriptografia.Decrypt(Integradores_Service.Properties.Settings.Default.WebHook_Url);

        oCriptografia = null;
      }
      catch (Exception Ex)
      {
        Log_Gravar(0, LogTipo.ErroInicializar, 0, Ex.Message, "Iniciando Serviço");
      }

      try
      {
        this.ServiceName = Config_App.sNomeSistema;
        this.CanStop = true;
        this.CanPauseAndContinue = true;
        this.AutoLog = false;
      }
      catch (Exception Ex)
      {
        Log_Gravar(0, LogTipo.ErroInicializar, 0, Ex.Message, "Iniciando Serviço");
      }

      try
      {
        //Status de pendente
        serviceStatus.dwWaitHint = 100000;
        ServiceStatus(ServiceState.SERVICE_START_PENDING);

        ((ISupportInitialize)this.EventLog).BeginInit();
        if (!EventLog.SourceExists(this.ServiceName))
        {
          EventLog.CreateEventSource(this.ServiceName, "Application");
        }
          ((ISupportInitialize)this.EventLog).EndInit();

        this.EventLog.Source = Config_App.sNomeSistema + " [Serviço]";
        this.EventLog.Log = "Application";
        Config.appLog = this.EventLog;

        //Status de rodando
        ServiceStatus(ServiceState.SERVICE_RUNNING);

        if (sErro != "")
        {
          Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, sErro, "Iniciando Serviço");
        }

        Assembly assembly = Assembly.GetExecutingAssembly();
        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        Integrador_Funcoes.ProductVersion = fileVersionInfo.ProductVersion;
      }
      catch (Exception Ex)
      {
        Log_Gravar(0, LogTipo.ErroInicializar, 0, Ex.Message, "Iniciando Serviço");
      }

      Declaracao.oMensagem_log = new Mensagem_log();
    }

    private void ServiceStatus(ServiceState oStatus)
    {
      serviceStatus.dwCurrentState = oStatus;
      SetServiceStatus(this.ServiceHandle, ref serviceStatus);
    }

    protected override void OnStart(string[] args)
    {
      this.EventLog.WriteEntry("Serviço Iniciando", EventLogEntryType.Information);

      try
      {
        ThreadStart Start = new ThreadStart(Processar);
        Thread Thread = new Thread(Start);

        this.EventLog.WriteEntry("Serviço conectando Banco de Dados", EventLogEntryType.Information);
        Integrador_Funcoes.oBancoDados = new clsBancoDados();
        if (!Integrador_Funcoes.oBancoDados.DBConectar(Config_App.sDB_Tipo.ToUpper(),
                                                       Config_App.sDB_StringConexao))
        {
          this.EventLog.WriteEntry("Serviço não conseguiu conectar Banco de Dados - " + Integrador_Funcoes.oBancoDados.DBErro(), EventLogEntryType.Information);
        }

        Thread.Start();

        this.EventLog.WriteEntry("Serviço Iniciado", EventLogEntryType.Information);
      }
      catch (Exception Ex)
      {
        this.Stop();

        Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message, "On Start");
      }
    }

    protected override void OnStop()
    {
      ServiceStatus(ServiceState.SERVICE_STOP_PENDING);
      this.EventLog.WriteEntry("Serviço Parado", EventLogEntryType.Information);
      Integrador_Funcoes.oBancoDados.DBDesconectar();
      Integrador_Funcoes.oBancoDados = null;
      ServiceStatus(ServiceState.SERVICE_STOPPED);
    }

    private void Processar()
    {
      string sSql;
      DataTable oData;
      int iSegundosParaLeituras = 0;
      bool bAtivado = true;

      //System.Timers.Timer timer = new System.Timers.Timer();
      //timer.Interval = 60000; // 60 seconds
      //timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
      //timer.Start();

      Processamento_Inicio = DateTime.Now;

      while (true)
      {
        try
        {
          if (!Integrador_Funcoes.oBancoDados.DBConectado())
          {
            Integrador_Funcoes.oBancoDados.DBConectar(Config_App.sDB_Tipo.ToUpper(),
                                                      Config_App.sDB_StringConexao);
          }

          sSql = "select * from tb_Integrador where NomeIntegrador = '" + Config_App.sProcessador.Trim() + "'";
          oData = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

          if (oData.Rows.Count == 0)
          {
            Config_App.idIntegrador = 1;
            Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, "Integrador não encontrado", "On Start");
          }
          else
          {
            Config_App.idIntegrador = Convert.ToInt32(oData.Rows[0]["idIntegrador"]);
            iSegundosParaLeituras = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData.Rows[0]["SegundosParaLeituras"]));
            bAtivado = (_Funcoes.FNC_NuloZero(oData.Rows[0]["TpAtivo"]) == 1);

            try
            {
              sSql = "update tb_Integrador set hostname = '" + Dns.GetHostName().Trim() + "' where idIntegrador = " + Config_App.idIntegrador.ToString();
              Integrador_Funcoes.oBancoDados.DBExecutar(sSql);
            }
            catch (Exception)
            {
              sSql = "update tb_Integrador set hostname = 'HOST NÃO IDENTIFICADO' where idIntegrador = " + Config_App.idIntegrador.ToString();
              Integrador_Funcoes.oBancoDados.DBExecutar(sSql);
            }
          }

          if (bAtivado) { LerInterfaces(); }
        }
        catch (Exception Ex)
        {
          Log_Gravar(0, LogTipo.Integrador_LerInterface, 0, Ex.Message, "ProcessarXX");

          Integrador_Funcoes.Processar_Sincronizacao("Integrador", Config_App.sProcessador, "ERR", "0000000", "ErroExecucao", Ex.Message, "IFP",
                                                     Integrador_Funcoes.TempoExecucaoAPI.ToString(),
                                                     Integrador_Funcoes.TempoExecucaoIntegrador.ToString(),
                                                     "", "", "", "");

          iSegundosParaLeituras = Properties.Settings.Default.SegundosParaLeituras;
        }

        Thread.Sleep(iSegundosParaLeituras);
      }
    }

    private void LerInterfaces()
    {
      DataTable oData;
      string[,] oParametro = null;
      string ds_parametro;

      string sCOD_PUXADA = "";
      string sTIPO_REGISTRO = "";
      string sTEL_CELULAR = "";
      string sTIPO_CONSULTA = "";
      string sVISAO_FATURAMENTO = "";
      string sQUERY_ATUALIZACAO = "";
      string sFILTRO_EXCLUSAO = "";
      string sFILTRO_SELECAO = "";

      Integrador_Funcoes.Processamento_Inicio = Integrador_Funcoes.oBancoDados.DBData();

      try
      {
        Integrador_Funcoes.oBancoDados.DBReconectar(Config_App.sDB_Tipo, Config_App.sDB_StringConexao);
        Integrador_Funcoes.oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_Integrador_Ping_upd", new clsCampo[] { new clsCampo { Nome = "_idIntegrador", Tipo = DbType.Double, Valor = Config_App.idIntegrador } });
        Integrador_Funcoes.oBancoDados.DBReconectar(Config_App.sDB_Tipo, Config_App.sDB_StringConexao);
      }
      catch (Exception)
      {
      }

      try
      {
        string sSql = "SELECT *" +
                      " FROM " + Config_App.sDB_TabelaView.Trim() +
                      " WHERE HostLeitura in ('X', '" + Config_App.sProcessador.ToUpper().Trim() + "')" +
                      " ORDER BY idEmpresa,nr_ordem_execucao";
        oData = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

        foreach (DataRow oRow_Tarefas in oData.Rows)
        {
          try
          {
            if (_Funcoes.FNC_NuloString(oRow_Tarefas["ds_parametro"]).Trim() != "")
            {
              ds_parametro = oRow_Tarefas["ds_parametro"].ToString();
              ds_parametro = ds_parametro.Replace("[TELEFONE_EMPRESA]", _Funcoes.FNC_NuloString(oRow_Tarefas["CodTelefone"].ToString()).Trim());
              ds_parametro = ds_parametro.Replace("[COD_PUXADA]", _Funcoes.FNC_NuloString(oRow_Tarefas["CodPuxada"].ToString()).Trim());
              oParametro = _Funcoes.Parametro_Desmontar(ds_parametro);
              sCOD_PUXADA = _Funcoes.Parametro_Valor(oParametro, "COD_PUXADA");
              sTIPO_REGISTRO = _Funcoes.Parametro_Valor(oParametro, "TIPO_REGISTRO");
              sTEL_CELULAR = _Funcoes.Parametro_Valor(oParametro, "TEL_CELULAR");
              sTIPO_CONSULTA = _Funcoes.Parametro_Valor(oParametro, "TIPO_CONSULTA");
              sVISAO_FATURAMENTO = _Funcoes.Parametro_Valor(oParametro, "VISAO_FATURAMENTO");
              sQUERY_ATUALIZACAO = _Funcoes.Parametro_Valor(oParametro, "QUERY_ATUALIZACAO");
              sFILTRO_EXCLUSAO = _Funcoes.Parametro_Valor(oParametro, "FILTRO_EXCLUSAO");
              sFILTRO_SELECAO = _Funcoes.Parametro_Valor(oParametro, "FILTRO_SELECAO");
            }

            Integrador_Funcoes.Processar(Convert.ToInt32(_Funcoes.FNC_NuloZero(oRow_Tarefas["idEmpresa"])),
                                         Convert.ToInt32(_Funcoes.FNC_NuloZero(oRow_Tarefas["idEmpresasServicos_GrupoTarefas"])),
                                         Convert.ToInt32(_Funcoes.FNC_NuloZero(oRow_Tarefas["idTipoIntegracao"])),
                                         Convert.ToInt32(_Funcoes.FNC_NuloZero(oRow_Tarefas["idEmpresaIntegracao"])),
                                         Convert.ToInt32(_Funcoes.FNC_NuloZero(oRow_Tarefas["idTarefa"])),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["Partner"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["cd_Aplicativo"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["cd_Servico"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["Tarefa"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["NomeArquivo"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["cdBancoConexaoOrigem"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["ds_stringconexaoOrigem"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["ds_stringconexaoDestino"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["tp_bancodadosOrigem"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["tp_bancodadosDestino"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["ds_usuario"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["ds_senha"]).ToString(),
                                         sCOD_PUXADA,
                                         sTIPO_REGISTRO,
                                         sTEL_CELULAR,
                                         sTIPO_CONSULTA,
                                         sVISAO_FATURAMENTO,
                                         sFILTRO_EXCLUSAO,
                                         sFILTRO_SELECAO,
                                         sQUERY_ATUALIZACAO,
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["ds_origem"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["ds_destino"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["ds_destino_executar"]).ToString(),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["tp_leituradados"]).ToString(),
                                         Convert.ToInt32(_Funcoes.FNC_NuloZero(oRow_Tarefas["nr_ordem_execucao"])),
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["Log_Tools_Integrador"]).ToString() == "S",
                                         _Funcoes.FNC_NuloString(oRow_Tarefas["Api_Log"]));
          }
          catch (Exception Ex)
          {
            Log_Gravar(0, LogTipo.Integrador_LerInterface, 0, Ex.Message,
                       "LerInterfaces - " + _Funcoes.FNC_NuloString(oRow_Tarefas["Tarefa"]).ToString() + " " +
                                            Integrador_Funcoes.UltimoErro + " -" + Integrador_Funcoes.UltimoLocalErro);
          }
        }
      }
      catch (Exception Ex)
      {
        Log_Gravar(0, LogTipo.Integrador_LerInterface, 0, Ex.Message, "LerInterfaces");
      }
    }

    private void Erro(object source, string Local, string Erro, EventArgs e)
    {
      //Log_Gravar()
    }

    public Boolean Log_Gravar(int iId_Empresa,
                              LogTipo idOperacao,
                              long iIdRegistro,
                              string sDs_log,
                              string sTitulo,
                              bool ExibirMensagem = false)
    {
      bool bOk = false;

      try
      {
        this.EventLog.WriteEntry(sTitulo + " - " + sDs_log, EventLogEntryType.Information);
      }
      catch (Exception)
      {
      }

      try
      {
        string sCaminho = System.AppDomain.CurrentDomain.BaseDirectory.ToString();

        sCaminho = "C:\\Sistemas\\Integradores\\Log.txt";

        //bOk = Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar("", "", "", "", "", "", "", iId_Empresa, LogTipo.Integrador_Eventos,
        //                                                      0, idOperacao, iIdRegistro, sTitulo + " >> " + sDs_log);

        StreamWriter vWriter = new StreamWriter(sCaminho, true);

        vWriter.WriteLine("  ");
        vWriter.WriteLine(sTitulo + ": " + DateTime.Now.ToString());
        vWriter.WriteLine(sDs_log);
        vWriter.Flush();
        vWriter.Close();
      }
      catch (Exception)
      {
      }

      return bOk;
    }
  }
}
