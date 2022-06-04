using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BancoDados;
using static BancoDados.clsBancoDados;
using static Integradores_OlaPdv.clsGeral;

namespace Integradores_OlaPdv
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
    public class cInterface
    {
      public string CodPuxada;
      public string Nome;
      public string JSon;
      public int Erro;
      public string Mensagem;
      public int nr_ordem_execucao;

      public string BancoDestino_ConectionString;
      public string BancoDestino_Tipo;
    }

    private DateTime Processamento_Inicio;
    ServiceStatus serviceStatus = new ServiceStatus();

    cInterface[] oInterface;

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

    public Service1()
    {
      InitializeComponent();

      try
      {
        Criptografia.Criptografia oCriptografia = new Criptografia.Criptografia();

        oCriptografia.Key = Constantes.const_Senha;
        Config_App.sDB_Tipo = Integradores_OlaPdv.Properties.Settings.Default.DB_Tipo;
        Config_App.sDB_StringConexao = oCriptografia.Decrypt(Integradores_OlaPdv.Properties.Settings.Default.DB_StringConexao);
        Config_App.sNomeSistema = Integradores_OlaPdv.Properties.Settings.Default.Processador;
        Config_App.sProcessador = Integradores_OlaPdv.Properties.Settings.Default.Processador;

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

        Assembly assembly = Assembly.GetExecutingAssembly();
        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        Config_App.ProductVersion = fileVersionInfo.ProductVersion;
      }
      catch (Exception Ex)
      {
        Log_Gravar(0, LogTipo.ErroInicializar, 0, Ex.Message, "Iniciando Serviço");
      }
    }

    protected override void OnStart(string[] args)
    {
      this.EventLog.WriteEntry("Serviço Iniciando", EventLogEntryType.Information);

      try
      {
        ThreadStart Start = new ThreadStart(Processar);
        Thread Thread = new Thread(Start);

        this.EventLog.WriteEntry("Serviço conectando Banco de Dados", EventLogEntryType.Information);
        oBancoDados = new clsBancoDados();
        if (!oBancoDados.DBConectar(Config_App.sDB_Tipo.ToUpper(),
                                    Config_App.sDB_StringConexao))
        {
          this.EventLog.WriteEntry("Serviço não conseguiu conectar Banco de Dados - " + oBancoDados.DBErro(), EventLogEntryType.Information);

          this.Stop();
        }
        else
        {
          Thread.Start();

          this.EventLog.WriteEntry("Serviço Iniciado", EventLogEntryType.Information);
        }
      }
      catch (Exception Ex)
      {
        this.Stop();

        Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message, "On Start");
      }
    }

    protected override void OnStop()
    {
    }

    private void ServiceStatus(ServiceState oStatus)
    {
      serviceStatus.dwCurrentState = oStatus;
      SetServiceStatus(this.ServiceHandle, ref serviceStatus);
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

        sCaminho = sCaminho + "\\Log.txt";

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

    private static string OlaPDV_Produto_SN(string sTexto)
    {
      if (sTexto.Trim() == "S")
      {
        return "S";
      }
      else
      {
        return "N";
      }
    }

    private void Processar()
    {
      DataTable oData;
      string sSqlText = "";
      string sCOD_PUXADA = "";
      bool bFim = false;
      int iCont = 1;
      string ds_parametro = "";
      string[,] oParametro = null;
      OlaPdv_Root OlaPdv_Root;
      bool Erro = false;
      string sIntegracao = "OlaPdv_" + DateTime.Now.ToString("yyyyMMddHHmm");
      clsBancoDados oBancoDados_Destino;

      sSqlText = "select * from vw_tarefas_executar where id_Servico = 9 and HostLeitura = '" + Config_App.sProcessador + "' order by CodPuxada, nr_ordem_execucao";
      oData = oBancoDados.DBQuery(sSqlText);

      if (oData.Rows.Count != 0)
      {
        oInterface = new cInterface[oData.Rows.Count - 1];

        foreach (DataRow oRow in oData.Rows)
        {
          iCont = iCont + 1;
          bFim = false;

          oInterface[iCont - 1] = new cInterface();
          oInterface[iCont - 1].CodPuxada = oRow["CodPuxada"].ToString();
          oInterface[iCont - 1].Nome = oRow["Servico"].ToString();
          oInterface[iCont - 1].nr_ordem_execucao = Convert.ToInt32(oRow["nr_ordem_execucao"].ToString());
          oInterface[iCont - 1].BancoDestino_ConectionString = oRow["ds_stringconexaoDestino"].ToString();
          oInterface[iCont - 1].BancoDestino_Tipo = oRow["tp_bancodadosDestino"].ToString();

          if (sCOD_PUXADA.Trim() != oRow["CodPuxada"].ToString())
          {
            Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, oRow["CodPuxada"].ToString(), "Início", "", Config_App.sProcessador, "0", "", 0, 0);
          }

          Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Carregando Dados - Início", "", Config_App.sProcessador, "0", "", 0, 0);

          //Ler Json - Início
          ServicePointManager.Expect100Continue = true;
          ServicePointManager.DefaultConnectionLimit = 9999;
          ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                                 SecurityProtocolType.Ssl3;

          HttpWebRequest request = (HttpWebRequest)WebRequest.Create(oRow["ds_stringconexaoOrigem"].ToString());

          try
          {
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(oRow["ds_usuario"].ToString() + ":" + oRow["ds_senha"].ToString())));
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            ds_parametro = oRow["ds_parametro"].ToString();
            oParametro = Parametro_Desmontar(ds_parametro);

            string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                           "\"TIPO_REGISTRO\":\"" + Parametro_Valor(oParametro, "TIPO_REGISTRO") + "\"," +
                           "\"TIPO_CONSULTA\":\"" + Parametro_Valor(oParametro, "TIPO_CONSULTA") + "\"}";

            request.ContentLength = json.Length;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
              streamWriter.Write(json);
              streamWriter.Flush();
              streamWriter.Close();
            }

            var content = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
              using (var stream = response.GetResponseStream())
              {
                using (var sr = new StreamReader(stream))
                {
                  content = sr.ReadToEnd();

                  oInterface[iCont - 1].JSon = content;

                  OlaPdv_Root = JsonConvert.DeserializeObject<OlaPdv_Root>(content);

                  oInterface[iCont - 1].Erro = OlaPdv_Root.codigo;
                  oInterface[iCont - 1].Mensagem = OlaPdv_Root.mensagem;

                  if (OlaPdv_Root.codigo != 1)
                  {
                    Erro = true;
                    break;
                  }
                }
              }
            }
          }
          catch (Exception Ex)
          {
            oInterface[iCont - 1].Erro = -1000;
            oInterface[iCont - 1].Mensagem = Ex.Message;

            Erro = true;
            break;
          }
          //Ler Json - Fim

          Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Carregando Dados - Fim", "", Config_App.sProcessador, "0", "", 0, 0);

          if (iCont == oData.Rows.Count)
          {
            bFim = true;
          }
          else
          {
            if (oRow["CodPuxada"].ToString() != oData.Rows[iCont + 1][""].ToString())
            {
              bFim = true;
            }
          }

          if (bFim == true)
          {
            Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Fim", "", Config_App.sProcessador, "0", "", 0, 0);
          }

          Erro = false;
        }

        if (!Erro)
        {
          string sCaminho = "";
          string sEDI_Integracao = "";

          OlaPDV_Configuracao_PDV_Root OlaPDV_CONFIGURACAO_PDV_Root;
          OlaPDV_CondicaoPagamento_Root OlaPDV_CondicaoPagamento_Root;
          OlaPDV_Vendedor_Root OlaPDV_Vendedor_Root;
          OlaPDV_TabelaPrecoProduto_Root OlaPDV_TabelaPrecoProduto_Root;
          OlaPDV_Produto_Root OlaPDV_Produto_Root;
          OlaPDV_EstoqueProduto_Root OlaPDV_EstoqueProduto_Root;
          OlaPDV_TabelaPreco_Root OlaPDV_TabelaPreco_Root;
          OlaPDV_ConfiguracaoEscalonada_Root OlaPDV_ConfiguracaoEscalonada_Root;
          OlaPDV_CategoriaEscalonada_Root OlaPDV_CategoriaEscalonada_Root;
          OlaPDV_EscalonadaFaixaDesconto_Root OlaPDV_EscalonadaFaixaDesconto_Root;
          OlaPDV_Escalonada_Root OlaPDV_Escalonada_Root;

          foreach (cInterface oItem in oInterface)
          {
            StreamWriter x;
            oBancoDados_Destino = new clsBancoDados();

            oBancoDados_Destino.DBConectar(oItem.BancoDestino_Tipo, oItem.BancoDestino_ConectionString);

            sCaminho = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + sIntegracao + "_" + oItem.Nome + ".txt";

            x = File.CreateText(sCaminho);
            x.WriteLine(oItem.Nome);
            x.Close();

            try
            {
              switch (oItem.Nome)
              {
                case "OlaPdvCondicaoPagamento":
                  {
                    OlaPDV_CondicaoPagamento_Root = JsonConvert.DeserializeObject<OlaPDV_CondicaoPagamento_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_olapdv_CondicaoPagamento_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                           new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                           new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_CondicaoPagamento_Root.OlaPDV_CondicaoPagamento.Count > 0)
                    {
                      foreach (OlaPDV_CondicaoPagamento OlaPDV_CondicaoPagamento in OlaPDV_CondicaoPagamento_Root.OlaPDV_CondicaoPagamento)
                      {
                        iCont++;

                        sEDI_Integracao = OlaPDV_CondicaoPagamento.COD_CONDICAO_PAGAMENTO.ToString().Trim().PadLeft(3, '0');

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_CondicaoPagamento_Root.OlaPDV_CondicaoPagamento.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_CondicaoPagamento", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                        new clsCampo { Nome = "sDSC_CONDICAO_PAGAMENTO", Valor = OlaPDV_CondicaoPagamento.DSC_CONDICAO_PAGAMENTO, Tipo = DbType.String },
                                                                                                        new clsCampo { Nome = "iCOD_TIPO_CONDICAO_PAGAMENTO", Valor = OlaPDV_CondicaoPagamento.COD_CONDICAO_PAGAMENTO, Tipo = DbType.Int32 },
                                                                                                        new clsCampo { Nome = "sEDI_Integracao", Valor = sEDI_Integracao, Tipo = DbType.String },
                                                                                                        new clsCampo { Nome = "iIND_ATIVO", Valor = OlaPDV_CondicaoPagamento.IND_ATIVO, Tipo = DbType.Int32 },
                                                                                                        new clsCampo { Nome = "dPRC_ADICIONAL_FINANCEIRO", Valor = OlaPDV_CondicaoPagamento.PRC_ADICIONAL_FINANCEIRO, Tipo = DbType.Double },
                                                                                                        new clsCampo { Nome = "iPRIORIDADE_CONDICAO_PAGAMENTO", Valor = OlaPDV_CondicaoPagamento.PRIORIDADE_CONDICAO_PAGAMENTO, Tipo = DbType.Int32 },
                                                                                                        new clsCampo { Nome = "sTIPO_DOCUMENTO", Valor = OlaPDV_CondicaoPagamento.TIPO_DOCUMENTO, Tipo = DbType.String },
                                                                                                        new clsCampo { Nome = "dVALOR_MINIMO", Valor = OlaPDV_CondicaoPagamento.VLR_MINIMO_PEDIDO, Tipo = DbType.Double },
                                                                                                        new clsCampo { Nome = "iNroParcelas", Valor = OlaPDV_CondicaoPagamento.QUANTIDADE_PARCELAS, Tipo = DbType.Int32 },
                                                                                                        new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                        new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } });
                      }
                    }

                    break;
                  }
                case "OlaPdvVendedor":
                  {
                    OlaPDV_Vendedor_Root = JsonConvert.DeserializeObject<OlaPDV_Vendedor_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_OlaPdv_Vendedor_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                  new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                  new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_Vendedor_Root.OlaPDV_Vendedor.Count > 0)
                    {
                      foreach (OlaPDV_Vendedor OlaPDV_Vendedor in OlaPDV_Vendedor_Root.OlaPDV_Vendedor)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_Vendedor_Root.OlaPDV_Vendedor.Count);

                        oBancoDados_Destino.DBProcedure("sp_OlaPdv_Vendedor", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                               new clsCampo { Nome = "sCOD_VENDEDOR", Valor = OlaPDV_Vendedor.COD_VENDEDOR, Tipo = DbType.String },
                                                                                               new clsCampo { Nome = "sVendedor", Valor = OlaPDV_Vendedor.NOM_VENDEDOR, Tipo = DbType.Int32 },
                                                                                               new clsCampo { Nome = "sTelefoneVendedor", Valor = OlaPDV_Vendedor.NUM_TELEFONE, Tipo = DbType.String },
                                                                                               new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                               new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } });
                      }
                    }

                    break;
                  }
                case "OlaPdvTabelaPreco":
                  {
                    OlaPDV_TabelaPreco_Root = JsonConvert.DeserializeObject<OlaPDV_TabelaPreco_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_OlaPdv_TabelaPreco_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                     new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                     new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_TabelaPreco_Root.OlaPDV_TabelaPreco.Count > 0)
                    {
                      foreach (OlaPDV_TabelaPreco OlaPDV_TabelaPreco in OlaPDV_TabelaPreco_Root.OlaPDV_TabelaPreco)
                      {
                        iCont++;

                        sEDI_Integracao = OlaPDV_TabelaPreco.COD_TABELA_PRECO.ToString().Trim().PadLeft(2, '0');

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_TabelaPreco_Root.OlaPDV_TabelaPreco.Count);

                        oBancoDados_Destino.DBProcedure("sp_OlaPdv_TabelaPreco", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                  new clsCampo { Nome = "sTabelaPreco", Valor = OlaPDV_TabelaPreco.DSC_TABELA_PRECO, Tipo = DbType.String },
                                                                                                  new clsCampo { Nome = "sEDI_Integracao", Valor = sEDI_Integracao, Tipo = DbType.String },
                                                                                                  new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                  new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } }); ;
                      }
                    }

                    break;
                  }
                case "OlaPdvProduto":
                  {
                    OlaPDV_Produto_Root = JsonConvert.DeserializeObject<OlaPDV_Produto_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_olapdv_produto_inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_Produto_Root.OlaPDV_Produto.Count > 0)
                    {
                      foreach (OlaPDV_Produto OlaPDV_Produto in OlaPDV_Produto_Root.OlaPDV_Produto)
                      {
                        iCont++;


                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_Produto_Root.OlaPDV_Produto.Count);

                        oBancoDados_Destino.DBProcedure("OlaPDV_Produto", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sCOD_MARCA", Valor = OlaPDV_Produto.COD_MARCA, Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sDSC_FAMILIA", Valor = OlaPDV_Produto.DSC_FAMILIA, Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sLinhaAtivo", Valor = OlaPDV_Produto_SN(OlaPDV_Produto.B2B_FAMILIA.Trim()).Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sCOD_UNIDADE_MEDIDA", Valor = OlaPDV_Produto.COD_UNIDADE_MEDIDA.Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sCOD_GRUPO_COMERCIALIZACAO", Valor = OlaPDV_Produto.COD_GRUPO_COMERCIALIZACAO.ToString().Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sCOD_PRODUTO_APELIDO", Valor = OlaPDV_Produto.COD_PRODUTO_APELIDO.ToString().Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sCOD_PRODUTO_INTERNO", Valor = OlaPDV_Produto.COD_PRODUTO_INTERNO.ToString().Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sGrupoAtivo", Valor = OlaPDV_Produto_SN(OlaPDV_Produto.DSC_GRUPO.Trim()), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sProdutoAtivo", Valor = OlaPDV_Produto_SN(OlaPDV_Produto.B2B_PRODUTO.Trim()), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sLinhaAtiva", Valor = OlaPDV_Produto_SN(OlaPDV_Produto.B2B_FAMILIA.Trim()), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sCOD_GRUPO", Valor = OlaPDV_Produto.COD_GRUPO.ToString().Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sDSC_GRUPO", Valor = OlaPDV_Produto.DSC_GRUPO.Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sDSC_PRODUTO", Valor = OlaPDV_Produto.DSC_PRODUTO.Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sNUMERO_ORDEM_IMPRESSAO", Valor = OlaPDV_Produto.NUMERO_ORDEM_IMPRESSAO.ToString().Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "sCOD_EAN", Valor = OlaPDV_Produto.COD_EAN.Trim(), Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "iQTD_UNIDADE_MEDIDA", Valor = OlaPDV_Produto.QTD_UNIDADE_MEDIDA, Tipo = DbType.Int32 },
                                                                                           new clsCampo { Nome = "dPCT_MAXIMO_DESCONTO", Valor = OlaPDV_Produto.PCT_MAXIMO_DESCONTO, Tipo = DbType.Double },
                                                                                           new clsCampo { Nome = "dVLR_PESO_PRODUTOo", Valor = OlaPDV_Produto.VLR_PESO_PRODUTO, Tipo = DbType.Double },
                                                                                           new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                           new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } }) ;
                      }
                    }

                    break;
                  }
                case "OlaPdvEstoqueProduto":
                  {
                    OlaPDV_EstoqueProduto_Root = JsonConvert.DeserializeObject<OlaPDV_EstoqueProduto_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_olapdv_EstoqueProduto_inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String }});

                    if (OlaPDV_EstoqueProduto_Root.OlaPDV_EstoqueProduto.Count > 0)
                    {
                      foreach (OlaPDV_EstoqueProduto OlaPDV_EstoqueProduto in OlaPDV_EstoqueProduto_Root.OlaPDV_EstoqueProduto)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_EstoqueProduto_Root.OlaPDV_EstoqueProduto.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_EstoqueProduto", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                     new clsCampo { Nome = "sCOD_PRODUTO_INTERNO", Valor = OlaPDV_EstoqueProduto.COD_PRODUTO_INTERNO.ToString().Trim(), Tipo = DbType.String },
                                                                                                     new clsCampo { Nome = "dQTD_ESTOQUE", Valor = OlaPDV_EstoqueProduto.QTD_ESTOQUE, Tipo = DbType.Double },
                                                                                                     new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                     new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } }); ;
                      }
                    }

                    break;
                  }
                case "OlaPdvTabelaPrecoProduto":
                  {
                    long idProtocolo;

                    OlaPDV_TabelaPrecoProduto_Root = JsonConvert.DeserializeObject<OlaPDV_TabelaPrecoProduto_Root>(oItem.JSon);

                    idProtocolo = Convert.ToInt64(DateTime.Now.Year.ToString() +
                                                  DateTime.Now.Month.ToString().PadLeft(2, '0') +
                                                  DateTime.Now.Day.ToString().PadLeft(2, '0') +
                                                  DateTime.Now.Hour.ToString().PadLeft(2, '0') +
                                                  DateTime.Now.Minute.ToString().PadLeft(2, '0') +
                                                  DateTime.Now.Second.ToString().PadLeft(2, '0'));

                    oBancoDados_Destino.DBProcedure("sp_olapdv_tabelaprecoproduto_inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                            new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                            new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_TabelaPrecoProduto_Root.OlaPDV_TabelaPrecoProduto.Count > 0)
                    {
                      foreach (OlaPDV_TabelaPrecoProduto OlaPDV_TabelaPrecoProduto in OlaPDV_TabelaPrecoProduto_Root.OlaPDV_TabelaPrecoProduto)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_TabelaPrecoProduto_Root.OlaPDV_TabelaPrecoProduto.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_tabelaprecoproduto", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                         new clsCampo { Nome = "sCOD_TABELA_PRECO", Valor = OlaPDV_TabelaPrecoProduto.COD_TABELA_PRECO.ToString().Trim(), Tipo = DbType.String },
                                                                                                         new clsCampo { Nome = "sCOD_PRODUTO_INTERNO", Valor = OlaPDV_TabelaPrecoProduto.COD_PRODUTO_INTERNO.ToString().Trim(), Tipo = DbType.String },
                                                                                                         new clsCampo { Nome = "dVLR_FINAL_PRODUTO", Valor = OlaPDV_TabelaPrecoProduto.VLR_FINAL_PRODUTO, Tipo = DbType.Double },
                                                                                                         new clsCampo { Nome = "iProtocolo", Valor = idProtocolo.ToString(), Tipo = DbType.String },
                                                                                                         new clsCampo { Nome = "dIND_MULTIPLO", Valor = Convert.ToDouble(OlaPDV_TabelaPrecoProduto.IND_MULTIPLO), Tipo = DbType.Double },
                                                                                                         new clsCampo { Nome = "dQTD_MAXIMA_PEDIDA", Valor = OlaPDV_TabelaPrecoProduto.QTD_MAXIMA_PEDIDA, Tipo = DbType.Double },
                                                                                                         new clsCampo { Nome = "dQTD_MINIMA_PEDIDA", Valor = OlaPDV_TabelaPrecoProduto.QTD_MINIMA_PEDIDA, Tipo = DbType.Double },
                                                                                                         new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                         new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } }); ;
                      }

                      oBancoDados_Destino.DBProcedure("sp_olapdv_tabelaprecoproduto_fim", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String }});

                    }

                    break;
                  }
                case "OlaPdvConfiguracaoEscalonada":
                  {
                    OlaPDV_ConfiguracaoEscalonada_Root = JsonConvert.DeserializeObject<OlaPDV_ConfiguracaoEscalonada_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_OlaPdv_ConfiguracaoEscalonada_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                                new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                                new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_ConfiguracaoEscalonada_Root.OlaPdv_ConfiguracaoEscalonada.Count > 0)
                    {
                      foreach (OlaPDV_ConfiguracaoEscalonada OlaPDV_ConfiguracaoEscalonada in OlaPDV_ConfiguracaoEscalonada_Root.OlaPdv_ConfiguracaoEscalonada)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_ConfiguracaoEscalonada_Root.OlaPdv_ConfiguracaoEscalonada.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_ConfiguracaoEscalonada", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                             new clsCampo { Nome = "sCOD_ESCALONADA", Valor = OlaPDV_ConfiguracaoEscalonada.COD_ESCALONADA, Tipo = DbType.String },
                                                                                                             new clsCampo { Nome = "sDSC_ESCALONADA", Valor = OlaPDV_ConfiguracaoEscalonada.DSC_ESCALONADA, Tipo = DbType.String },
                                                                                                             new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                             new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } }); ;
                      }
                    }

                    break;
                  }
                case "OlaPdvCategoriaEscalonada":
                  {
                    OlaPDV_CategoriaEscalonada_Root = JsonConvert.DeserializeObject<OlaPDV_CategoriaEscalonada_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_olapdv_CategoriaEscalonada_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                             new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                             new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_CategoriaEscalonada_Root.OlaPDV_CategoriaEscalonada.Count > 0)
                    {
                      foreach (OlaPDV_CategoriaEscalonada OlaPDV_CategoriaEscalonada in OlaPDV_CategoriaEscalonada_Root.OlaPDV_CategoriaEscalonada)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_CategoriaEscalonada_Root.OlaPDV_CategoriaEscalonada.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_CategoriaEscalonada", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                          new clsCampo { Nome = "sCOD_TABELA_PRECO", Valor = OlaPDV_CategoriaEscalonada.COD_TABELA_PRECO, Tipo = DbType.String },
                                                                                                          new clsCampo { Nome = "iCOD_CTI", Valor = OlaPDV_CategoriaEscalonada.COD_CTI, Tipo = DbType.Int32 },
                                                                                                          new clsCampo { Nome = "iCOD_ESCALONADA", Valor = OlaPDV_CategoriaEscalonada.COD_ESCALONADA, Tipo = DbType.Int32 },
                                                                                                          new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                          new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } }); ;
                      }

                      oBancoDados_Destino.DBProcedure("sp_olapdv_CategoriaEscalonada_Total", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String }});
                    }

                    break;
                  }
                case "OlaPdvEscalonadaFaixaDesconto":
                  {
                    OlaPDV_EscalonadaFaixaDesconto_Root = JsonConvert.DeserializeObject<OlaPDV_EscalonadaFaixaDesconto_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_olapdv_FaixaDescontoEscalonada_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                                 new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                                 new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_EscalonadaFaixaDesconto_Root.OlaPDV_EscalonadaFaixaDesconto.Count > 0)
                    {
                      foreach (OlaPDV_EscalonadaFaixaDesconto OlaPDV_EscalonadaFaixaDesconto in OlaPDV_EscalonadaFaixaDesconto_Root.OlaPDV_EscalonadaFaixaDesconto)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_EscalonadaFaixaDesconto_Root.OlaPDV_EscalonadaFaixaDesconto.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_FaixaDescontoEscalonada", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                              new clsCampo { Nome = "sCOD_PRODUTO", Valor = OlaPDV_EscalonadaFaixaDesconto.COD_PRODUTO, Tipo = DbType.String },
                                                                                                              new clsCampo { Nome = "iCOD_EMPREGADO", Valor = OlaPDV_EscalonadaFaixaDesconto.COD_EMPREGADO, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "iCOD_ESCALONADA", Valor = OlaPDV_EscalonadaFaixaDesconto.COD_ESCALONADA, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "iCOD_FAMILIA", Valor = OlaPDV_EscalonadaFaixaDesconto.COD_FAMILIA, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "iCOD_GRUPO", Valor = OlaPDV_EscalonadaFaixaDesconto.COD_GRUPO, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "iNUM_SEQUENCIA_ESCALONADA", Valor = OlaPDV_EscalonadaFaixaDesconto.NUM_SEQUENCIA_ESCALONADA, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "dPCT_MAXIMO_DESCONTO", Valor = OlaPDV_EscalonadaFaixaDesconto.PCT_MAXIMO_DESCONTO, Tipo = DbType.Double },
                                                                                                              new clsCampo { Nome = "iQTD_FINAL_ESCALONADA", Valor = OlaPDV_EscalonadaFaixaDesconto.QTD_FINAL_ESCALONADA, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "iQTD_INICIAL_ESCALONADA", Valor = OlaPDV_EscalonadaFaixaDesconto.QTD_INICIAL_ESCALONADA, Tipo = DbType.Int32 },
                                                                                                              new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                              new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } }); ;
                      }
                    }

                    break;
                  }
                case "OlaPdvEscalonada":
                  {
                    OlaPDV_Escalonada_Root = JsonConvert.DeserializeObject<OlaPDV_Escalonada_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_OlaPdv_EntidadeEscalonada_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                            new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                            new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_Escalonada_Root.OlaPDV_Escalonada.Count > 0)
                    {
                      foreach (OlaPDV_Escalonada OlaPDV_Escalonada in OlaPDV_Escalonada_Root.OlaPDV_Escalonada)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_Escalonada_Root.OlaPDV_Escalonada.Count);

                        oBancoDados_Destino.DBProcedure("sp_OlaPdv_EntidadeEscalonada", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                         new clsCampo { Nome = "sCOD_PDV", Valor = OlaPDV_Escalonada.COD_PDV.ToString().Trim(), Tipo = DbType.String },
                                                                                                         new clsCampo { Nome = "sCOD_ESCALONADA", Valor = OlaPDV_Escalonada.COD_ESCALONADA.ToString().Trim(), Tipo = DbType.String },
                                                                                                         new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                         new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } }); ;
                      }
                    }

                    break;
                  }
                case "OlaPdvConfiguracaoPdv":
                  {
                    OlaPDV_CONFIGURACAO_PDV_Root = JsonConvert.DeserializeObject<OlaPDV_Configuracao_PDV_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_olapdv_Configuracao_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                      new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                      new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_CONFIGURACAO_PDV_Root.OlaPDV_Configuracao_PDV.Count > 0)
                    {
                      foreach (OlaPDV_Configuracao_PDV OlaPdv_CONFIGURACAO_PDV in OlaPDV_CONFIGURACAO_PDV_Root.OlaPDV_Configuracao_PDV)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_CONFIGURACAO_PDV_Root.OlaPDV_Configuracao_PDV.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_Configuracao", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCOD_PDV", Valor = OlaPdv_CONFIGURACAO_PDV.COD_PDV, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCNPJ_CPF", Valor = OlaPdv_CONFIGURACAO_PDV.CNPJ_CPF, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sRAZAO_SOCIAL", Valor = OlaPdv_CONFIGURACAO_PDV.RAZAO_SOCIAL, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sNOME_FANTASIA", Valor = OlaPdv_CONFIGURACAO_PDV.NOME_FANTASIA, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCOD_CATEGORIA_VAREJO", Valor = OlaPdv_CONFIGURACAO_PDV.COD_CATEGORIA_VAREJO, Tipo = DbType.Int32 },
                                                                                                   new clsCampo { Nome = "sUF", Valor = OlaPdv_CONFIGURACAO_PDV.UF, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sENDERECO", Valor = OlaPdv_CONFIGURACAO_PDV.ENDERECO, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sBairro", Valor = OlaPdv_CONFIGURACAO_PDV.BAIRRO, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCidade", Valor = OlaPdv_CONFIGURACAO_PDV.CIDADE, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCEP", Valor = OlaPdv_CONFIGURACAO_PDV.CEP, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sLATITUDE", Valor = OlaPdv_CONFIGURACAO_PDV.LATITUDE, Tipo = DbType.Double },
                                                                                                   new clsCampo { Nome = "sLONGITUDE", Valor = OlaPdv_CONFIGURACAO_PDV.LONGITUDE, Tipo = DbType.Double },
                                                                                                   new clsCampo { Nome = "sCONDICAO_PAGTO", Valor = OlaPdv_CONFIGURACAO_PDV.CONDICAO_PAGTO, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sVENDEDOR", Valor = OlaPdv_CONFIGURACAO_PDV.VENDEDOR, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "dSALDO_DISPONIVEL", Valor = OlaPdv_CONFIGURACAO_PDV.SALDO_DISPONIVEL, Tipo = DbType.Int32 },
                                                                                                   new clsCampo { Nome = "sTABELA_PRECO", Valor = OlaPdv_CONFIGURACAO_PDV.TABELA_PRECO, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sIND_ATIVO", Valor = OlaPdv_CONFIGURACAO_PDV.CNPJ_CPF, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } });
                      }
                    }
;
                    break;
                  }
              }

            }
            catch (Exception Ex)
            {
            }
          }
        }
        else
        {
          sSqlText = "update tf set DataTarefa = dateadd(hour, 1, getdate())" +
                     " from vw_tarefas te" +
                      " inner join tb_tarefas tf on tf.idTarefa = te.idTarefa" +
                      " where te.id_Servico = 9" +
                        " and te.HostLeitura = 'Dev05.OlaPdv.00000005'";
          oBancoDados.DBExecutar(sSqlText);
        }
      }
    }

    public class OlaPdv_Root
    {
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_Configuracao_PDV
    {
      public string BAIRRO { get; set; }
      public int CEP { get; set; }
      public string CIDADE { get; set; }
      public string CNPJ_CPF { get; set; }
      public int COD_CATEGORIA_VAREJO { get; set; }
      public int COD_FAD { get; set; }
      public int COD_PDV { get; set; }
      public int COD_TOP_OBRIGATORIO { get; set; }
      public string CONDICAO_PAGTO { get; set; }
      public DateTime DATA_CADASTRO { get; set; }
      public string ENDERECO { get; set; }
      public string GRUPO_COMERCIALIZACAO { get; set; }
      public string IND_ATIVO { get; set; }
      public double LATITUDE { get; set; }
      public int LIMITE_CREDITO { get; set; }
      public double LONGITUDE { get; set; }
      public string NOME_FANTASIA { get; set; }
      public string RAZAO_SOCIAL { get; set; }
      public int SALDO_DISPONIVEL { get; set; }
      public string STATUS_FINANCEIRO { get; set; }
      public string TABELA_PRECO { get; set; }
      public string UF { get; set; }
      public string VENDEDOR { get; set; }
    }

    public class OlaPDV_Configuracao_PDV_Root
    {
      public List<OlaPDV_Configuracao_PDV> OlaPDV_Configuracao_PDV { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_CondicaoPagamento
    {
      public int COD_CONDICAO_PAGAMENTO { get; set; }
      public int COD_TIPO_CONDICAO_PAGAMENTO { get; set; }
      public string DSC_CONDICAO_PAGAMENTO { get; set; }
      public int IND_ATIVO { get; set; }
      public double PRC_ADICIONAL_FINANCEIRO { get; set; }
      public int PRIORIDADE_CONDICAO_PAGAMENTO { get; set; }
      public string COD_TIPO_DOCUMENTO_COBRANCA { get; set; }
      public string QUANTIDADE_PARCELAS { get; set; }
      public string TIPO_DOCUMENTO { get; set; }
      public double VLR_MINIMO_PEDIDO { get; set; }
    }

    public class OlaPDV_CondicaoPagamento_Root
    {
      public List<OlaPDV_CondicaoPagamento> OlaPDV_CondicaoPagamento { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_PDV_Escalonada
    {
      public int COD_ESCALONADA { get; set; }
      public int COD_PDV { get; set; }
    }

    public class OlaPDV_PDV_Escalonada_Root
    {
      public List<OlaPDV_PDV_Escalonada> OlaPDV_PDV_Escalonada { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    class OlaPDV_TabelaPrecoProduto
    {
      public int COD_PRODUTO_INTERNO { get; set; }
      public int COD_TABELA_PRECO { get; set; }
      public int IND_MULTIPLO { get; set; }
      public double PCT_MAXIMO_DESCONTO_TABPRE { get; set; }
      public double QTD_MAXIMA_PEDIDA { get; set; }
      public double QTD_MINIMA_PEDIDA { get; set; }
      public double VLR_FINAL_PRODUTO { get; set; }
    }

    class OlaPDV_TabelaPrecoProduto_Root
    {
      public List<OlaPDV_TabelaPrecoProduto> OlaPDV_TabelaPrecoProduto { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    class OlaPDV_TabelaPreco
    {
      public int COD_TABELA_PRECO { get; set; }
      public string DSC_TABELA_PRECO { get; set; }
    }

    class OlaPDV_TabelaPreco_Root
    {
      public List<OlaPDV_TabelaPreco> OlaPDV_TabelaPreco { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_Vendedor
    {
      public string COD_GRUPO_COMERCIALIZACAO { get; set; }
      public int COD_VENDEDOR { get; set; }
      public string IND_STATUS { get; set; }
      public string NOM_VENDEDOR { get; set; }
      public string NUM_TELEFONE { get; set; }
    }

    public class OlaPDV_Vendedor_Root
    {
      public List<OlaPDV_Vendedor> OlaPDV_Vendedor { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_Produto
    {
      public string B2B_FAMILIA { get; set; }
      public string B2B_GRUPO { get; set; }
      public string B2B_PRODUTO { get; set; }
      public string COD_EAN { get; set; }
      public int COD_FAMILIA { get; set; }
      public int COD_GRUPO { get; set; }
      public int COD_GRUPO_COMERCIALIZACAO { get; set; }
      public int COD_MARCA { get; set; }
      public int COD_PRODUTO_APELIDO { get; set; }
      public int COD_PRODUTO_INTERNO { get; set; }
      public string COD_UNIDADE_MEDIDA { get; set; }
      public string DSC_APELIDO_PRODUTO { get; set; }
      public string DSC_FAMILIA { get; set; }
      public string DSC_GRUPO { get; set; }
      public string DSC_GRUPO_COMERCIALIZACAO { get; set; }
      public string DSC_MARCA { get; set; }
      public string DSC_PRODUTO { get; set; }
      public int FATOR_PRECO { get; set; }
      public string IND_ATIVO { get; set; }
      public int NUMERO_ORDEM_IMPRESSAO { get; set; }
      public int PCT_MAXIMO_DESCONTO { get; set; }
      public int QTD_UNIDADE_MEDIDA { get; set; }
      public double VLR_PESO_PRODUTO { get; set; }
    }

    public class OlaPDV_Produto_Root
    {
      public List<OlaPDV_Produto> OlaPDV_Produto { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_EstoqueProduto
    {
      public int COD_PRODUTO_INTERNO { get; set; }
      public int QTD_ESTOQUE { get; set; }
    }

    public class OlaPDV_EstoqueProduto_Root
    {
      public List<OlaPDV_EstoqueProduto> OlaPDV_EstoqueProduto { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_ConfiguracaoEscalonada
    {
      public int COD_ESCALONADA { get; set; }
      public string DSC_ESCALONADA { get; set; }
    }

    public class OlaPDV_ConfiguracaoEscalonada_Root
    {
      public List<OlaPDV_ConfiguracaoEscalonada> OlaPdv_ConfiguracaoEscalonada { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_CategoriaEscalonada
    {
      public int COD_CTI { get; set; }
      public int COD_ESCALONADA { get; set; }
      public int COD_TABELA_PRECO { get; set; }
    }

    public class OlaPDV_CategoriaEscalonada_Root
    {
      public List<OlaPDV_CategoriaEscalonada> OlaPDV_CategoriaEscalonada { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_EscalonadaFaixaDesconto
    {
      public int COD_EMPREGADO { get; set; }
      public int COD_ESCALONADA { get; set; }
      public int COD_FAMILIA { get; set; }
      public int COD_GRUPO { get; set; }
      public int COD_PRODUTO { get; set; }
      public int NUM_SEQUENCIA_ESCALONADA { get; set; }
      public double PCT_MAXIMO_DESCONTO { get; set; }
      public int QTD_FINAL_ESCALONADA { get; set; }
      public int QTD_INICIAL_ESCALONADA { get; set; }
    }

    public class OlaPDV_EscalonadaFaixaDesconto_Root
    {
      public List<OlaPDV_EscalonadaFaixaDesconto> OlaPDV_EscalonadaFaixaDesconto { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_Escalonada
    {
      public int COD_ESCALONADA { get; set; }
      public int COD_PDV { get; set; }
    }

    public class OlaPDV_Escalonada_Root
    {
      public List<OlaPDV_Escalonada> OlaPDV_Escalonada { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }
  }
}
