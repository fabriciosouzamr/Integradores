using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using static BancoDados;
using static BancoDados.clsBancoDados;
using static Integradores._Funcoes;

namespace Integradores
{
  public enum LogTipo
  {
    ArquivoImportadoComSucesso = 50000,
    ArquivoNaoEncontrado = 50001,

    ErroInicializar = 50001,
    ErroImportarQuantidadeRegistrosInvalida = 50002,
    ErroNoBancoDados = 50500,
    ErroNaRotina_ImportaEstoque = 50501,
    ErroNaRotina_ImportaPreco = 50502,
    ErroLeituraTarefas = 50503,
    ErroNaRotina_Flag = 50505,
    ErroNaRotina_Bot = 50506,
    ErroNaRotina_ProcessarTarefas = 50507,

    Integrador_Configuracao = 50508,
    Integrador_Eventos = 50509,
    Integrador_LerInterface = 50510
  }

  public class Integrador_Funcoes
  {
    public static string PastaImportacao;
    public static string PastaLokalizei;
    public static clsBancoDados oBancoDados;
    public static string ProductVersion;
    public static DateTime Processamento_Inicio;
    public static string UltimoLocalErro;
    public static string UltimoErro;

    public static string sStatus;
    public static string sErro;
    public static double TempoExecucaoAPI;
    public static double TempoExecucaoIntegrador;

    public const string const_Propriedade_Notificacao_EMail_Ativacao = "0\\0\\0\\0\\0\\0\\#Notificacao\\#EMail_Ativacao";
    public const string const_Propriedade_Notificacao_WebServiceAtivacao = "0\\0\\0\\0\\0\\0\\#Notificacao\\#WebServiceAtivacao";
    public const string const_Propriedade_EMail_Host = "0\\0\\0\\0\\0\\0\\#EMail\\#Host";
    public const string const_Propriedade_EMail_Port = "0\\0\\0\\0\\0\\0\\#EMail\\#Port";
    public const string const_Propriedade_EMail_EnableSsl = "0\\0\\0\\0\\0\\0\\#EMail\\#EnableSsl";
    public const string const_Propriedade_EMail_UseDefaultCredentials = "0\\0\\0\\0\\0\\0\\#EMail\\#UseDefaultCredentials";
    public const string const_Propriedade_EMail_UserName = "0\\0\\0\\0\\0\\0\\#EMail\\#UserName";
    public const string const_Propriedade_EMail_Password = "0\\0\\0\\0\\0\\0\\#EMail\\#Password";

    public static string sApiLog = "";

    public class Propriedade
    {
      public string Nome = "";
      public string Secao = "";
      public string Campo = "";
      public object Valor = null;
    }

    public static Propriedade[] Propriedades;

    public static void Propriedade_Carregar()
    {
      DataTable oData;
      string sSql = "";

      sSql = "select Propriedade," +
                    "Secao," +
                    "Campo," +
                    "Valor" +
             " from " + Config_App.sDB_BancoDados + ".tb_propriedade" +
             " where idEmpresa = 0" +
               " and idParceiro = 0" +
               " and idProduto = 0" +
               " and idServico = 0" +
               " and idLicenca = 0" +
               " and idUsuario = 0";
      oData = oBancoDados.DBQuery(sSql);

      foreach (DataRow oRow in oData.Rows)
      {
        if (Propriedades == null)
        {
          Array.Resize(ref Propriedades, 1);
        }
        else
        {
          Array.Resize(ref Propriedades, Propriedades.Length + 1);
        }

        Propriedades[Propriedades.Length - 1] = new Propriedade();
        Propriedades[Propriedades.Length - 1].Nome = oRow["Propriedade"].ToString();
        Propriedades[Propriedades.Length - 1].Secao = oRow["Secao"].ToString();
        Propriedades[Propriedades.Length - 1].Campo = oRow["Campo"].ToString();
        Propriedades[Propriedades.Length - 1].Valor = oRow["Valor"];
      }
    }

    public static object Propriedade_Ler(string Nome)
    {
      object Ret = null;

      try
      {
        foreach (Propriedade Item in Propriedades)
        {
          if (Item.Nome == Nome)
          {
            Ret = Item.Valor;
            break;
          }
        }
      }
      catch (Exception)
      {
      }

      return Ret;
    }

    public static void Processar_Sincronizacao(string sAPP,
                                               string sPartner,
                                               string sTipoLog,
                                               string sCliente,
                                               string sLog,
                                               string sComplemento,
                                               string sOrigem,
                                               string itempo1,
                                               string iTempo2,
                                               string sParametro1,
                                               string sParametro2,
                                               string sWebService,
                                               string sServico)
    {
      string sAPI;

      sAPI = "http://api02.plugthink.com/api/v2/pluglog/_proc/sp_log?sAPP=" + sAPP +
                                                                   "&sPartner=" + sPartner +
                                                                   "&sTipoLog=" + sTipoLog +
                                                                   "&sCliente=" + sCliente +
                                                                   "&sLog=" + sLog +
                                                                   "&sComplemento=" + sComplemento +
                                                                   "&sOrigem=" + sOrigem +
                                                                   "&itempo1=" + itempo1 +
                                                                   "&iTempo2=" + iTempo2 +
                                                                   "&sParametro1=" + sParametro1 +
                                                                   "&sParametro2" + sParametro2 +
                                                                   "&sWebService=" + sWebService +
                                                                   "&sServico=" + sServico +
                                                                   "&sIntegrador=" + Config_App.sProcessador +
                                                                   "&api_key=b74c56ef26c5efff96d55cabb3273569d5b87520014b541961c01343b7bdbfa0";

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sAPI);

      try
      {
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        var content = string.Empty;

        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
            }
          }
        }
      }
      catch (Exception Ex)
      {
      }
      finally
      {
        request = null;
      }
    }

    public static bool Processar(int iIdEmpresa,
                                 long iGrupoTarefas,
                                 int iIdTipoIntegracao,
                                 int idEmpresaIntegracao,
                                 int iIdTarefa,
                                 string sPartner,
                                 string sAplicativo,
                                 string sCdServico,
                                 string sTarefa,
                                 string sNomeArquivo,
                                 string sCodigoConexaoOrigem,
                                 string sStringConexaoOrigem,
                                 string sStringConexaoDestino,
                                 string sTpBancoDadosOrigem,
                                 string sTpBancoDadosDestino,
                                 string sUsuario,
                                 string sSenha,
                                 string sCOD_PUXADA,
                                 string sTIPO_REGISTRO,
                                 string sTEL_CELULAR,
                                 string sTIPO_CONSULTA,
                                 string sVISAO_FATURAMENTO,
                                 string sFILTRO_EXCLUSAO,
                                 string sFILTRO_SELECAO,
                                 string sQUERY_ATUALIZACAO,
                                 string sDS_Origem,
                                 string sDS_Destino,
                                 string sDS_Destino_Executar,
                                 string sTP_LeituraDados,
                                 int nr_ordem_execucao,
                                 bool Log_Tools_Integrador,
                                 string sApiLog)
    {
      bool bOk = false;
      int iEmpresa = 0;
      string sPonto = "";
      string sSqlText = "";
      string sJson = "";
      string sJsonRet = "";

      bOk = false;

      try
      {
        //UltimoLocalErro = "Processar";
        //sPonto = "Processar";

        Integrador_Funcoes.sApiLog = sApiLog;

        sSqlText = "select count(*) from vw_Integrador where idIntegrador = " + Config_App.idIntegrador.ToString() + " and ic_fazer_ping = 'S'";
        if (Convert.ToInt32(Integrador_Funcoes.oBancoDados.DBQuery_ValorUnico(sSqlText)) != 0)
        {
          Processar_Sincronizacao(sAplicativo, sPartner, "PNG", sCOD_PUXADA, "Ativo", "", "IFP",
                                  Integrador_Funcoes.TempoExecucaoAPI.ToString(),
                                  Integrador_Funcoes.TempoExecucaoIntegrador.ToString(), sTIPO_REGISTRO, sTIPO_CONSULTA, sCodigoConexaoOrigem, sCdServico);
        }

        oBancoDados.DBReconectar(Config_App.sDB_Tipo, Config_App.sDB_StringConexao);
        oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_Integrador_DH_upd", new clsCampo[] { new clsCampo { Nome = "_idIntegrador", Tipo = DbType.Double, Valor = Config_App.idIntegrador } });
        oBancoDados.DBReconectar(Config_App.sDB_Tipo, Config_App.sDB_StringConexao);

        Processar_Sincronizacao(sAplicativo, sPartner, "IIN", sCOD_PUXADA, "InicioIntegracao", "", "IFP",
                                Integrador_Funcoes.TempoExecucaoAPI.ToString(),
                                Integrador_Funcoes.TempoExecucaoIntegrador.ToString(), sTIPO_REGISTRO, sTIPO_CONSULTA, sCodigoConexaoOrigem, sCdServico);

        Tarefa_Iniciar(iIdTarefa, iGrupoTarefas, Config_App.idIntegrador, sAplicativo, sPartner, sCdServico, sCOD_PUXADA, sTarefa, nr_ordem_execucao, Log_Tools_Integrador);

        if (FNC_NuloString(sTP_LeituraDados.ToUpper()) == Constantes.const_TipoLeituraDados_DBQuery.ToUpper() ||
            FNC_NuloString(sTP_LeituraDados.ToUpper()) == Constantes.const_TipoLeituraDados_DBProc.ToUpper() ||
            FNC_NuloString(sTP_LeituraDados.ToUpper()) == Constantes.const_TipoLeituraDados_DBTabela.ToUpper() ||
            FNC_NuloString(sTP_LeituraDados.ToUpper()) == Constantes.const_TipoLeituraDados_WSJson.ToUpper())
        {
          sPonto = "BancoDados_Migracao - Início";
          bOk = BancoDados_Migracao(sAplicativo,
                                    sPartner,
                                    sCdServico,
                                    iIdEmpresa,
                                    iIdTipoIntegracao,
                                    idEmpresaIntegracao,
                                    iIdTarefa,
                                    sCOD_PUXADA,
                                    sTIPO_REGISTRO,
                                    sTIPO_CONSULTA,
                                    sVISAO_FATURAMENTO,
                                    sFILTRO_EXCLUSAO,
                                    sFILTRO_SELECAO,
                                    sQUERY_ATUALIZACAO,
                                    sStringConexaoOrigem,
                                    sTpBancoDadosOrigem,
                                    sStringConexaoDestino,
                                    sTpBancoDadosDestino,
                                    sDS_Origem,
                                    sDS_Destino,
                                    sTarefa,
                                    sTP_LeituraDados);
          sPonto = "BancoDados_Migracao - Fim";
        }
        else
        {
          switch (FNC_NuloZero(iIdTipoIntegracao))
          {
            case 9:         //FlagWSWhatsapp_DisDevolucao
              UltimoLocalErro = UltimoLocalErro + " - FlagWSWhatsapp_DisDevolucao";
              bOk = FlagWZ_DisDevolucao(sAplicativo,
                                        sPartner,
                                        sCdServico,
                                        iIdEmpresa,
                                        iIdTarefa,
                                        sStringConexaoOrigem,
                                        sUsuario,
                                        sSenha,
                                        sCOD_PUXADA,
                                        sTIPO_REGISTRO,
                                        sTEL_CELULAR,
                                        sTIPO_CONSULTA,
                                        sVISAO_FATURAMENTO,
                                        sStringConexaoDestino,
                                        sTpBancoDadosDestino);
              break;
            case 10:         //FlagWSWhatsapp_DisTroca
              UltimoLocalErro = UltimoLocalErro + " - FlagWSWhatsapp_DisTroca";
              bOk = FlagWZ_DisTroca(sAplicativo,
                                    sPartner,
                                    sCdServico,
                                    iIdEmpresa,
                                    iIdTarefa,
                                    sStringConexaoOrigem,
                                    sUsuario,
                                    sSenha,
                                    sCOD_PUXADA,
                                    sTIPO_REGISTRO,
                                    sTEL_CELULAR,
                                    sTIPO_CONSULTA,
                                    sVISAO_FATURAMENTO,
                                    sStringConexaoDestino,
                                    sTpBancoDadosDestino);
              break;
            case 11:         //FlagWSWhatsapp_DisTIav_iv
              UltimoLocalErro = UltimoLocalErro + " - FlagWSWhatsapp_DisTIav_iv";
              bOk = FlagWZ_DisIav_iv(sAplicativo,
                                     sPartner,
                                     sCdServico,
                                     iIdEmpresa,
                                     iIdTarefa,
                                     sStringConexaoOrigem,
                                     sUsuario,
                                     sSenha,
                                     sCOD_PUXADA,
                                     sTIPO_REGISTRO,
                                     sTEL_CELULAR,
                                     sTIPO_CONSULTA,
                                     sVISAO_FATURAMENTO,
                                     sStringConexaoDestino,
                                     sTpBancoDadosDestino);
              break;
            case 12:         //FlagWSWhatsapp_DisInadimplencia
              UltimoLocalErro = UltimoLocalErro + " - FlagWSWhatsapp_DisInadimplencia";
              bOk = FlagWZ_DisInadimplencia(sAplicativo,
                                            sPartner,
                                            sCdServico,
                                            iIdEmpresa,
                                            iIdTarefa,
                                            sStringConexaoOrigem,
                                            sUsuario,
                                            sSenha,
                                            sCOD_PUXADA,
                                            sTIPO_REGISTRO,
                                            sTEL_CELULAR,
                                            sTIPO_CONSULTA,
                                            sVISAO_FATURAMENTO,
                                            sStringConexaoDestino,
                                            sTpBancoDadosDestino);
              break;
            case 13:         //FlagWSWhatsapp_DisLogDevolucao
              UltimoLocalErro = UltimoLocalErro + " - FlagWSWhatsapp_DisLogDevolucao";
              bOk = FlagWZ_DisLogDevolucao(sAplicativo,
                                           sPartner,
                                           sCdServico,
                                           iIdEmpresa,
                                           iIdTarefa,
                                           sStringConexaoOrigem,
                                           sUsuario,
                                           sSenha,
                                           sCOD_PUXADA,
                                           sTIPO_REGISTRO,
                                           sTEL_CELULAR,
                                           sTIPO_CONSULTA,
                                           sVISAO_FATURAMENTO,
                                           sStringConexaoDestino,
                                           sTpBancoDadosDestino);
              break;
            case 14:         //FlagWSWhatsapp_DisLogDespersaoRota
              UltimoLocalErro = UltimoLocalErro + " - FlagWSWhatsapp_DisLogDespersaoRota";
              bOk = FlagWZ_DisLog_Taxa_Ocupacao(sAplicativo,
                                                sPartner,
                                                sCdServico,
                                                iIdEmpresa,
                                                iIdTarefa,
                                                sStringConexaoOrigem,
                                                sUsuario,
                                                sSenha,
                                                sCOD_PUXADA,
                                                sTIPO_REGISTRO,
                                                sTEL_CELULAR,
                                                sTIPO_CONSULTA,
                                                sVISAO_FATURAMENTO,
                                                sStringConexaoDestino,
                                                sTpBancoDadosDestino);
              break;
            case 16:         //FlagWSWhatsapp_DisLogDevolucaoMotorista
              UltimoLocalErro = UltimoLocalErro + " - FlagWSWhatsapp_DisLogDevolucaoMotorista";
              bOk = FlagWZ_DisLogDevolucaoMotorista(sAplicativo,
                                                    sPartner,
                                                    sCdServico,
                                                    iIdEmpresa,
                                                    iIdTarefa,
                                                    sStringConexaoOrigem,
                                                    sUsuario,
                                                    sSenha,
                                                    sCOD_PUXADA,
                                                    sTIPO_REGISTRO,
                                                    sTEL_CELULAR,
                                                    sTIPO_CONSULTA,
                                                    sVISAO_FATURAMENTO,
                                                    sStringConexaoDestino,
                                                    sTpBancoDadosDestino);
              break;
            case 17:         //FlagWSWhatsapp_DisLogDevolucaoCarro
              UltimoLocalErro = UltimoLocalErro + " - FlagWSWhatsapp_DisLogDevolucaoCarro";
              bOk = DisLogDevolucaoCarro(sAplicativo,
                                         sPartner,
                                         sCdServico,
                                         iIdEmpresa,
                                         iIdTarefa,
                                         sStringConexaoOrigem,
                                         sUsuario,
                                         sSenha,
                                         sCOD_PUXADA,
                                         sTIPO_REGISTRO,
                                         sTEL_CELULAR,
                                         sTIPO_CONSULTA,
                                         sVISAO_FATURAMENTO,
                                         sStringConexaoDestino,
                                         sTpBancoDadosDestino);
              break;
            case 18:        //DashPlus - DadosInformados
              UltimoLocalErro = UltimoLocalErro + " - DashPlus - DadosInformados";
              bOk = rotImportaDadosDisInformados(sAplicativo,
                                                 sPartner,
                                                 sCOD_PUXADA,
                                                 sTIPO_REGISTRO,
                                                 sTIPO_CONSULTA,
                                                 sStringConexaoOrigem,
                                                 sCdServico,
                                                 iEmpresa,
                                                 iIdTarefa,
                                                 sNomeArquivo.ToString(),
                                                 sStringConexaoDestino,
                                                 sTpBancoDadosDestino);
              break;
            case 19:        //DashPlus - DadosUsuario
              UltimoLocalErro = UltimoLocalErro + " - DashPlus - DadosUsuario";
              bOk = FlagWZ_DisUsuario(sAplicativo,
                                      sPartner,
                                      sCdServico,
                                      iEmpresa,
                                      iIdTarefa,
                                      sStringConexaoOrigem,
                                      sUsuario,
                                      sSenha,
                                      sCOD_PUXADA,
                                      sStringConexaoDestino,
                                      sTpBancoDadosDestino);
              break;
            case 20:        //P&P - USP_ACOMP_D_VENDA_SOFIA
              UltimoLocalErro = UltimoLocalErro + " - P&P - USP_ACOMP_D_VENDA_SOFIA";
              bOk = PeP_USP_ACOMP_D_VENDA_SOFIA(iIdTarefa,
                                                sStringConexaoOrigem,
                                                sTpBancoDadosOrigem,
                                                sStringConexaoDestino,
                                                sTpBancoDadosDestino);
              break;
            case 21:        //Banco de Dados -Query
              break;
            case 23:        //23 - Enviar Mensagem Bot
              {
                bOk = Integrador_ProcessarMensagem(iIdEmpresa);
                break;
              }
            case 28:        //app_botautocadastro - LeadExport
              {
                bOk = app_botautocadastro_LeadExport(sStringConexaoOrigem,
                                                     sTpBancoDadosOrigem,
                                                     sStringConexaoDestino,
                                                     sTpBancoDadosDestino);
                break;
              }
            case 30:        //Flag.WS.Whatsapp - OlaPdv - Condição de Pagamento
              {
                bOk = OlaPDV_CondicaoPagamento(sAplicativo,
                                               sPartner,
                                               sCdServico,
                                               sTarefa,
                                               nr_ordem_execucao,
                                               iEmpresa,
                                               iIdTarefa,
                                               iGrupoTarefas,
                                               sStringConexaoOrigem,
                                               sUsuario,
                                               sSenha,
                                               sCOD_PUXADA,
                                               sTIPO_REGISTRO,
                                               sTIPO_CONSULTA,
                                               sStringConexaoDestino,
                                               sTpBancoDadosDestino,
                                               sDS_Origem,
                                               sDS_Destino,
                                               ref sJsonRet);
                break;
              }
            case 34:        //Flag.WS.Whatsapp - OlaPdv - Tracking Devoluções
              {
                bOk = OlaPDV_TrackingDevolucoes(sAplicativo,
                                                sPartner,
                                                sCdServico,
                                                iEmpresa,
                                                iIdTarefa,
                                                sStringConexaoOrigem,
                                                sUsuario,
                                                sSenha,
                                                sCOD_PUXADA,
                                                sTIPO_REGISTRO,
                                                sTIPO_CONSULTA,
                                                sStringConexaoDestino,
                                                sTpBancoDadosDestino,
                                                sDS_Origem,
                                                sDS_Destino,
                                                ref sJsonRet);
                break;
              }
            case 35:        //Flag.WS.Whatsapp - OlaPdv - Tabela de Preço
              {
                bOk = OlaPDV_TabelaPreco(sAplicativo,
                                         sPartner,
                                         sCdServico,
                                         sTarefa,
                                         nr_ordem_execucao,
                                         iEmpresa,
                                         iIdTarefa,
                                         iGrupoTarefas,
                                         sStringConexaoOrigem,
                                         sUsuario,
                                         sSenha,
                                         sCOD_PUXADA,
                                         sTIPO_REGISTRO,
                                         sTIPO_CONSULTA,
                                         sStringConexaoDestino,
                                         sTpBancoDadosDestino,
                                         sDS_Origem,
                                         sDS_Destino,
                                         ref sJsonRet);
                break;
              }
            case 36:        //Flag.WS.Whatsapp - OlaPdv - Produtos
              {
                bOk = OlaPDV_Produto(sAplicativo,
                                     sPartner,
                                     sCdServico,
                                     sTarefa,
                                     nr_ordem_execucao,
                                     iEmpresa,
                                     iIdTarefa,
                                     iGrupoTarefas,
                                     sStringConexaoOrigem,
                                     sUsuario,
                                     sSenha,
                                     sCOD_PUXADA,
                                     sTIPO_REGISTRO,
                                     sTIPO_CONSULTA,
                                     sStringConexaoDestino,
                                     sTpBancoDadosDestino,
                                     sDS_Origem,
                                     sDS_Destino,
                                     ref sJsonRet);
                break;
              }
            case 123:        //Flag.WS.Whatsapp - OlaPdv - Escalonada
              {
                bOk = OlaPDV_Escalonada(sAplicativo,
                                        sPartner,
                                        sCdServico,
                                        sTarefa,
                                        nr_ordem_execucao,
                                        iEmpresa,
                                        iIdTarefa,
                                        iGrupoTarefas,
                                        sStringConexaoOrigem,
                                        sUsuario,
                                        sSenha,
                                        sCOD_PUXADA,
                                        sTIPO_REGISTRO,
                                        sTIPO_CONSULTA,
                                        sStringConexaoDestino,
                                        sTpBancoDadosDestino,
                                        sDS_Origem,
                                        sDS_Destino,
                                        ref sJsonRet);
                break;
              }
            case 37:        //Flag.WS.Whatsapp - OlaPdv - Estoque
              {
                bOk = OlaPDV_Estoque(sAplicativo,
                                     sPartner,
                                     sCdServico,
                                     sTarefa,
                                     nr_ordem_execucao,
                                     iEmpresa,
                                     iIdTarefa,
                                     iGrupoTarefas,
                                     sStringConexaoOrigem,
                                     sUsuario,
                                     sSenha,
                                     sCOD_PUXADA,
                                     sTIPO_REGISTRO,
                                     sTIPO_CONSULTA,
                                     sStringConexaoDestino,
                                     sTpBancoDadosDestino,
                                     sDS_Origem,
                                     sDS_Destino,
                                     ref sJsonRet);
                break;
              }
            case 38:        //Flag.WS.Whatsapp - OlaPdv - Preço Produto
              {
                bOk = OlaPDV_PrecoProduto(sAplicativo,
                                          sPartner,
                                          sCdServico,
                                          sTarefa,
                                          nr_ordem_execucao,
                                          iEmpresa,
                                          iIdTarefa,
                                          iGrupoTarefas,
                                          sStringConexaoOrigem,
                                          sUsuario,
                                          sSenha,
                                          sCOD_PUXADA,
                                          sTIPO_REGISTRO,
                                          sTIPO_CONSULTA,
                                          sStringConexaoDestino,
                                          sTpBancoDadosDestino,
                                          sDS_Origem,
                                          sDS_Destino,
                                          ref sJsonRet);
                break;
              }
            case 39:        //Flag.WS.Whatsapp - OlaPdv - Vendedor
              {
                bOk = OlaPDV_Vendedor(sAplicativo,
                                      sPartner,
                                      sCdServico,
                                      sTarefa,
                                      nr_ordem_execucao,
                                      iEmpresa,
                                      iIdTarefa,
                                      iGrupoTarefas,
                                      sStringConexaoOrigem,
                                      sUsuario,
                                      sSenha,
                                      sCOD_PUXADA,
                                      sTIPO_REGISTRO,
                                      sTIPO_CONSULTA,
                                      sStringConexaoDestino,
                                      sTpBancoDadosDestino,
                                      sDS_Origem,
                                      sDS_Destino,
                                      ref sJsonRet);
                break;
              }
            case 40:        //Flag.WS.Whatsapp - OlaPdv - Configuração
              {
                bOk = OlaPDV_Configuracao(sAplicativo,
                                          sPartner,
                                          sCdServico,
                                          sTarefa,
                                          nr_ordem_execucao,
                                          iEmpresa,
                                          iIdTarefa,
                                          iGrupoTarefas,
                                          sStringConexaoOrigem,
                                          sUsuario,
                                          sSenha,
                                          sCOD_PUXADA,
                                          sTIPO_REGISTRO,
                                          sTIPO_CONSULTA,
                                          sStringConexaoDestino,
                                          sTpBancoDadosDestino,
                                          sDS_Origem,
                                          sDS_Destino,
                                          ref sJsonRet);
                break;
              }
            case 41:        //Flag.WS.Whatsapp - OlaPdv - Configuração Escalonada
              {
                bOk = OlaPDV_ConfiguracaoEscalonada(sAplicativo,
                                                    sPartner,
                                                    sCdServico,
                                                    sTarefa,
                                                    nr_ordem_execucao,
                                                    iEmpresa,
                                                    iIdTarefa,
                                                    iGrupoTarefas,
                                                    sStringConexaoOrigem,
                                                    sUsuario,
                                                    sSenha,
                                                    sCOD_PUXADA,
                                                    sTIPO_REGISTRO,
                                                    sTIPO_CONSULTA,
                                                    sStringConexaoDestino,
                                                    sTpBancoDadosDestino,
                                                    sDS_Origem,
                                                    sDS_Destino,
                                                    ref sJsonRet);
                break;
              }
            case 42:        //Flag.WS.Whatsapp - OlaPdv - Configuração Escalonada
              {
                bOk = OlaPDV_CategoriaEscalonada(sAplicativo,
                                                 sPartner,
                                                 sCdServico,
                                                 sTarefa,
                                                 nr_ordem_execucao,
                                                 iEmpresa,
                                                 iIdTarefa,
                                                 iGrupoTarefas,
                                                 sStringConexaoOrigem,
                                                 sUsuario,
                                                 sSenha,
                                                 sCOD_PUXADA,
                                                 sTIPO_REGISTRO,
                                                 sTIPO_CONSULTA,
                                                 sStringConexaoDestino,
                                                 sTpBancoDadosDestino,
                                                 sDS_Origem,
                                                 sDS_Destino,
                                                 ref sJsonRet);
                break;
              }
            case 43:        //Flag.WS.Whatsapp - OlaPdv - Faixa de Preço Escalonada
              {
                bOk = OlaPDV_FaixaDescontoEscalonada(sAplicativo,
                                                     sPartner,
                                                     sCdServico,
                                                     sTarefa,
                                                     nr_ordem_execucao,
                                                     iEmpresa,
                                                     iIdTarefa,
                                                     iGrupoTarefas,
                                                     sStringConexaoOrigem,
                                                     sUsuario,
                                                     sSenha,
                                                     sCOD_PUXADA,
                                                     sTIPO_REGISTRO,
                                                     sTIPO_CONSULTA,
                                                     sStringConexaoDestino,
                                                     sTpBancoDadosDestino,
                                                     sDS_Origem,
                                                     sDS_Destino,
                                                     ref sJsonRet);
                break;
              }
            case 121:        //Flag.WS.Whatsapp - OlaPdv - Faixa de Preço Escalonada
              {
                bOk = FlagWZ_DocumentosaVencer(idEmpresaIntegracao,
                                               sAplicativo,
                                               sPartner,
                                               sCdServico,
                                               iEmpresa,
                                               iIdTarefa,
                                               sStringConexaoOrigem,
                                               sUsuario,
                                               sSenha,
                                               sCOD_PUXADA,
                                               sTIPO_REGISTRO,
                                               sTEL_CELULAR,
                                               sTIPO_CONSULTA,
                                               sStringConexaoDestino,
                                               sTpBancoDadosDestino);
                break;
              }
            case 122:        //Flag.WS.Whatsapp - OlaPdv - Faixa de Preço Escalonada
              {
                bOk = FlagWZ_DisSfDocumentosVencidos(idEmpresaIntegracao,
                                                     sAplicativo,
                                                     sPartner,
                                                     sCdServico,
                                                     iEmpresa,
                                                     iIdTarefa,
                                                     sStringConexaoOrigem,
                                                     sUsuario,
                                                     sSenha,
                                                     sCOD_PUXADA,
                                                     sTIPO_REGISTRO,
                                                     sTEL_CELULAR,
                                                     sTIPO_CONSULTA,
                                                     sStringConexaoDestino,
                                                     sTpBancoDadosDestino);
                break;
              }
          }
        }

        if (!bOk)
        {
          Config.appLog_Escrever("ERRO AO PROCESSAR A TAREFA " + sTarefa + "(" + sPonto + ")" + "[" + UltimoErro + "]");
        }
        else
        {
          if (sDS_Destino_Executar.Trim() != "")
          {
            try
            {
              clsBancoDados oBancoDados = new clsBancoDados();
              oBancoDados.DBConectar(sTpBancoDadosDestino, sStringConexaoDestino);

              oBancoDados.DBExecutar(sDS_Destino_Executar);

              oBancoDados.DBDesconectar();
              oBancoDados = null;
            }
            catch (Exception Ex)
            {
              Config.appLog_Escrever("ERRO AO PROCESSAR A TAREFA " + sTarefa + "(Executar Destino)[" + Ex.Message + "]");
            }
          }
        }

        Tarefa_Concluir(iIdTarefa, iGrupoTarefas, sAplicativo, sPartner, sCdServico, sCOD_PUXADA, sTarefa, sJsonRet, nr_ordem_execucao, Log_Tools_Integrador);
        //DBSQL_Tarefas_AutoAgendar_Gravar(sAplicativo, sPartner, sCdServico, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sStringConexaoOrigem, iIdTarefa);

        BancoDados_Migracao_Log(idEmpresaIntegracao, iIdEmpresa, sCOD_PUXADA, sStringConexaoOrigem, sStatus, sErro, TempoExecucaoAPI, TempoExecucaoIntegrador, "COD_PUXADA=" + sCOD_PUXADA,
                                                                                                                                                               "TIPO_REGISTRO=" + sTIPO_REGISTRO,
                                                                                                                                                               "TIPO_CONSULTA=" + sTIPO_CONSULTA,
                                                                                                                                                               "VISAO_FATURAMENTO=" + sVISAO_FATURAMENTO);
      }
      catch (Exception Ex)
      {
        UltimoErro = Ex.Message;
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sStringConexaoOrigem, sCdServico,
                                     iIdEmpresa, LogTipo.ErroLeituraTarefas, iIdTarefa, UltimoLocalErro + " > " + sTarefa + "(" + sPonto + ")" + "[" + UltimoErro + "]");
      }
      finally
      {
        Processar_Sincronizacao(sAplicativo, sPartner, "FIN", sCOD_PUXADA, "FinalIntegracao", UltimoErro, "IFP",
                                Integrador_Funcoes.TempoExecucaoAPI.ToString(),
                                Integrador_Funcoes.TempoExecucaoIntegrador.ToString(), sTIPO_REGISTRO, sTIPO_CONSULTA, sCodigoConexaoOrigem, sCdServico);
      }

      return bOk;
    }

    private static bool rotImportaDadosDisInformados(string sAplicativo,
                                                     string sPartner,
                                                     string sCOD_PUXADA,
                                                     string sTIPO_REGISTRO,
                                                     string sTIPO_CONSULTA,
                                                     string sConexao,
                                                     string sCdServico,
                                                     int iIdEmpresa, int iIdTarefa, string sArquivo, string sBancoDestino, string sTipoBanco)
    {
      string sFile;
      string sDestino;

      try
      {
        sFile = FNC_Diretorio_Tratar(PastaImportacao) + sArquivo;
        clsBancoDados oBancoDados = new clsBancoDados();

        oBancoDados.DBConectar(sTipoBanco, sBancoDestino);

        if (!System.IO.File.Exists(sFile))
        {
          Log_Registar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                       iIdEmpresa, LogTipo.ArquivoNaoEncontrado, iIdTarefa, "Arquivo " + sFile + " não localizado", sArquivo);
          return false;
        }

        rotImportaCSV(sAplicativo, sPartner, sCdServico, iIdEmpresa, iIdTarefa, sFile, oBancoDados, "DadosInformados", new clsCampo[] {new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                                            new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});

        Log_Registar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                     iIdEmpresa, LogTipo.ArquivoImportadoComSucesso, iIdTarefa, "Importação Concluída", sArquivo);

        sDestino = sFile.Substring(0, sFile.Trim().Length - 3) + "pro";

        if (System.IO.File.Exists(sDestino))
          System.IO.File.Delete(sDestino);

        Log_Integrador_Registar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                iIdEmpresa, iIdTarefa, "Renomeado > " + sDestino);
        System.IO.File.Move(sFile, sDestino);

        return true;
      }
      catch (Exception Ex)
      {
        Log_Registar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                     iIdEmpresa, LogTipo.ErroNaRotina_ImportaEstoque, iIdTarefa, Ex.Message, sArquivo);
        return false;
      }
    }

    private static bool rotImportaCSV(string sAplicativo,
                                      string sPartner,
                                      string sCdServico,
                                      int iIdEmpresa,
                                      int iIdTarefa,
                                      string sArquivo,
                                      clsBancoDados oBancoDestino,
                                      string sTabelaDestino,
                                      clsCampo[] CamposAdicionais)
    {
      Boolean bOk = false;
      int iColunasValidas = 0;
      int iCont = 0;

      try
      {
        string sSql = "";
        StreamReader rd = new StreamReader(sArquivo);

        string linha = null;
        string[] linha_Cabecalho = null;
        string[] linha_Detalhe = null;
        clsCampo[] Campos = null;

        while ((linha = rd.ReadLine()) != null)
        {
          if (linha_Cabecalho == null)
          {
            linha_Cabecalho = linha.Split(';');
            linha_Cabecalho = oBancoDestino.DBValidarCampos(sTabelaDestino, linha_Cabecalho, ref iColunasValidas);

            for (int intI = 0; intI < linha_Cabecalho.Length; intI++)
            {
              if (linha_Cabecalho[intI] != "?")
                sSql = sSql + ",#" + linha_Cabecalho[intI];
            }

            if (CamposAdicionais != null)
            {
              for (int intI = 0; intI < CamposAdicionais.Length; intI++)
              {
                sSql = sSql + ",#" + CamposAdicionais[intI].Nome;
              }
            }

            sSql = sSql.Substring(1);

            sSql = "insert into " + sTabelaDestino + "(" + sSql.Replace("#", "") + ") values (" + sSql + ")";
          }
          else
          {
            linha_Detalhe = linha.Split(';');

            if (CamposAdicionais != null)
            {
              Campos = new clsCampo[iColunasValidas + (CamposAdicionais.Length)];
            }
            else
            {
              Campos = new clsCampo[iColunasValidas];
            }

            iCont = 0;

            for (int intI = 0; intI < linha_Cabecalho.Length; intI++)
            {
              if (linha_Cabecalho[intI] != "?")
              {
                Campos[iCont] = new clsCampo { Nome = linha_Cabecalho[intI], Valor = linha_Detalhe[intI], Tipo = DbType.String };
                iCont++;
              }
            }

            if (CamposAdicionais != null)
              for (int intI = 0; intI < CamposAdicionais.Length; intI++)
              {
                Campos[iColunasValidas + intI] = CamposAdicionais[intI];
              }

            bOk = oBancoDestino.DBExecutar(sSql, Campos);
          }
        }

        rd.Close();
        rd = null;
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, "", "", "", "", sCdServico,
                                     iIdEmpresa, LogTipo.ErroNaRotina_ImportaPreco, iIdTarefa, sArquivo + " >> " + Ex.Message);

        bOk = false;
      }

      return bOk;
    }

    private static bool rotCriarDatasEntrega(string sAplicativo,
                                             string sPartner,
                                             string sCdServico,
                                             int iIdTarefa)
    {
      String[] sidCaracteristica = new String[200];

      bool bNovo;

      string sSql;
      DataTable rcoEmpresa;
      DataTable rcoRotas;
      DataTable rcoRotasEntrega;

      DateTime datCalculada;

      DateTime dataInicial;
      DateTime datLancamento;

      int iIdEmpresa;
      string sRotina = "CriarDatasEntrega";

      clsCampo rcoRotasEntrega_idEmpresa;
      clsCampo rcoRotasEntrega_idRotaVenda;
      clsCampo rcoRotasEntrega_Legenda;
      clsCampo rcoRotasEntrega_LegendaCombo;

      clsCampo rcoRotasEntrega_dataInicial;
      clsCampo rcoRotasEntrega_DataFinal;
      clsCampo rcoRotasEntrega_DataLimite;
      clsCampo rcoRotasEntrega_Disponivel;
      clsCampo rcoRotasEntrega_ValorFrete;
      clsCampo rcoRotasEntrega_CompraMinima;
      clsCampo rcoRotasEntrega_idEntidade;
      clsCampo rcoRotasEntrega_idVendedor;

      try
      {
        if (!oBancoDados.DBConectado())
        {
          Log_Registar(sAplicativo, sPartner, "", "", "", "", sCdServico,
                       0, LogTipo.ErroNoBancoDados, iIdTarefa, "Erro ao conectar banco de dados!", sRotina);
          return false;
        }

        sSql = "select idEmpresa from tb_empresas";
        rcoEmpresa = oBancoDados.DBQuery(sSql);

        foreach (DataRow oRow_Empresa in rcoEmpresa.Rows)
        {
          iIdEmpresa = Convert.ToInt32(oRow_Empresa["idEmpresa"]);

          dataInicial = DateTime.Now.AddYears(1);

          sSql = "select concat(idempresa,'-',idRotaVenda,'-',idEntidade,'-',idRotaVendaEntrega) as chave" +
                 " from tb_rotavenda_entrega" +
                 " where Disponivel=1" +
                   " and idEmpresa= " + iIdEmpresa +
                   " and DataInicial <= '" + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "'";
          rcoRotas = oBancoDados.DBQuery(sSql);

          foreach (DataRow oRow_Rotas in rcoRotas.Rows)
          {
            oBancoDados.DBSQL_Integrador_Log_Gravar(sAplicativo, sPartner, "", "", "", "", sCdServico,
                                                    iIdEmpresa, iIdTarefa, "Removendo RotaEntrega " + oRow_Rotas["chave"]);
            //sSql = Inet1.OpenURL("http://fb.pedidodireto.com/api.php?table=RotaVendaEntrega&op=delete&id=" & rcoRotas!chave)
          }

          //Desativa datas Antigas
          sSql = "update tb_rotavenda_entrega" +
                 " set Disponivel=0" +
                 " where DataInicial <= '" + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "'" +
                    "and idEmpresa = " + iIdEmpresa;
          oBancoDados.DBExecutar(sSql);

          //Buscando Rotas
          sSql = "select * from tb_rotavenda where idEmpresa=" + iIdEmpresa;
          rcoRotas = oBancoDados.DBQuery(sSql);

          foreach (DataRow oRow_Rotas in rcoRotas.Rows)
          {
            oBancoDados.DBSQL_Integrador_Log_Gravar(sAplicativo, sPartner, "", "", "", "", sCdServico,
                                                    iIdEmpresa, iIdTarefa, "Importando " + oRow_Rotas["RotaVenda"]);

            datCalculada = dataInicial;

            //DataCalculada
            for (int intI = 0; intI < 7; intI++)
            {

              if (Convert.ToInt32(dataInicial.AddYears(intI).DayOfWeek) == Convert.ToInt32(oRow_Rotas["diaEntrega"]) ||
                  (Convert.ToInt32(dataInicial.AddYears(intI).DayOfWeek) == 6 && Convert.ToInt32(oRow_Rotas["diaEntrega"]) == 0))
              {
                datCalculada = dataInicial.AddYears(intI);
                break;
              }
            }

            for (int intSemana = 0; intSemana < 5; intSemana++)
            {
              //Ajusta semanas
              datLancamento = datCalculada.AddYears(intSemana * 7);

              //Adiciona campo no produto
              sSql = "select * " +
                     " from tb_rotavenda_entrega" +
                     " where idEmpresa=" + iIdEmpresa +
                       " and idRotaVenda='" + oRow_Rotas["idRotaVenda"] + "'" +
                       " and DataInicial='" + String.Format("{0:yyyy-MM-dd}", datLancamento) + "'";
              rcoRotasEntrega = oBancoDados.DBQuery(sSql);

              bNovo = false;

              if (!FNC_Data_Vazio(rcoRotasEntrega)) { bNovo = true; };

              rcoRotasEntrega_idEmpresa = new clsCampo { Nome = "idEmpresa", Tipo = DbType.Int32, Valor = 5 };
              rcoRotasEntrega_idRotaVenda = new clsCampo { Nome = "idRotaVenda", Tipo = DbType.Int32, Valor = oRow_Rotas["idRotaVenda"] };
              rcoRotasEntrega_Legenda = new clsCampo { Nome = "Legenda", Tipo = DbType.String, Valor = "Frete Gratis " + String.Format("{0:dd/MM/yy}", datLancamento) };
              rcoRotasEntrega_LegendaCombo = new clsCampo { Nome = "LegendaCombo", Tipo = DbType.String, Valor = String.Format("{0:dd/MM/yy}", datLancamento) + " (Gratis)" };
              rcoRotasEntrega_dataInicial = new clsCampo { Nome = "dataInicial", Tipo = DbType.DateTime, Valor = datLancamento };
              rcoRotasEntrega_DataFinal = new clsCampo { Nome = "DataFinal", Tipo = DbType.DateTime, Valor = datLancamento };
              rcoRotasEntrega_DataLimite = new clsCampo { Nome = "DataLimite", Tipo = DbType.String, Valor = String.Format("{0:dd/MM/yy}", datLancamento) + " 17:00:00" };
              rcoRotasEntrega_Disponivel = new clsCampo { Nome = "Disponivel", Tipo = DbType.Int32, Valor = 1 };
              rcoRotasEntrega_ValorFrete = new clsCampo { Nome = "ValorFrete", Tipo = DbType.Int32, Valor = 0 };
              rcoRotasEntrega_CompraMinima = new clsCampo { Nome = "CompraMinima", Tipo = DbType.Int32, Valor = 0 };
              rcoRotasEntrega_idEntidade = new clsCampo { Nome = "idEntidade", Tipo = DbType.Int32, Valor = 0 };
              rcoRotasEntrega_idVendedor = new clsCampo { Nome = "idVendedor", Tipo = DbType.Int32, Valor = 0 };

              sSql = "update tb_rotavenda_entrega" +
                     " set idEmpresa=?idEmpresa," +
                          "idRotaVenda=?idRotaVenda," +
                          "Legenda=?Legenda," +
                          "LegendaCombo=?LegendaCombo," +
                          "dataInicial=?dataInicial," +
                          "DataFinal=?DataFinal," +
                          "DataLimite=?DataLimite," +
                          "Disponivel=?Disponivel," +
                          "ValorFrete=?ValorFrete," +
                          "CompraMinima=?CompraMinima," +
                          "idEntidade=?idEntidade," +
                          "idVendedor=?idVendedor" +
                     " where idEmpresa=" + iIdEmpresa +
                       " and idRotaVenda='" + oRow_Rotas["idRotaVenda"] + "'" +
                       " and DataInicial='" + String.Format("{0:yyyy-MM-dd}", datLancamento) + "'";
              oBancoDados.DBExecutar(sSql, new clsCampo[] {rcoRotasEntrega_idEmpresa,
                                                                                       rcoRotasEntrega_idRotaVenda,
                                                                                       rcoRotasEntrega_Legenda,
                                                                                       rcoRotasEntrega_LegendaCombo,
                                                                                       rcoRotasEntrega_dataInicial,
                                                                                       rcoRotasEntrega_DataFinal,
                                                                                       rcoRotasEntrega_DataLimite,
                                                                                       rcoRotasEntrega_Disponivel,
                                                                                       rcoRotasEntrega_ValorFrete,
                                                                                       rcoRotasEntrega_CompraMinima,
                                                                                       rcoRotasEntrega_idEntidade,
                                                                                       rcoRotasEntrega_idVendedor});

              if (bNovo)
              {
                //Busca para salvar no firebase
                sSql = "select concat(rte.idempresa,'-',rte.idRotaVenda,'-',rte.idEntidade,'-',rte.idRotaVendaEntrega) as chave" +
                            ",cast(rte.idRotaVendaEntrega as char) as idRotaVendaEntrega" +
                            ",rte.idempresa" +
                            ",rte.identidade" +
                            ",rte.idRotaVenda" +
                            ",rte.Disponivel" +
                            ",rte.idVendedor" +
                            ",rte.DataFinal" +
                            ",rte.DataLimite" +
                            ",rte.ValorFrete" +
                            ",(case when rte.ValorFrete=0 then 'Gratis' else 'Pago' end) as tpFrete" +
                            ",rte.Legenda as FreteExibicao" +
                            ",rte.LegendaCombo as TextoCombo" +
                    " from tb_rotavenda_entrega rte" +
                    " where idEmpresa=5" +
                        " and idRotaVenda='" + oRow_Rotas["idRotaVenda"] + "'" +
                        " and DataInicial='" + String.Format("{0:yyyy-MM-dd}", datLancamento) + "'";
                rcoRotasEntrega = oBancoDados.DBQuery(sSql);
              }
            }
          }
        }

        Log_Integrador_Registar(sAplicativo, sPartner, "", "", "", "", sCdServico,
                                0, iIdTarefa, "Processamento finalizado - " + sRotina);

        return true;
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, "", "", "", "", sCdServico,
                                     0, LogTipo.ErroNaRotina_ImportaPreco, iIdTarefa, sRotina + " >> " + Ex.Message);

        return false;
      }
    }

    private static bool rotImportaPrecosPPBH(int iIdTarefa,
                                            string sConexao,
                                            string sUsuario,
                                            string sSenha,
                                            string sCOD_PUXADA,
                                            string sTIPO_REGISTRO,
                                            string sTIPO_CONSULTA,
                                            string sDS_STRINGCONEXAODESTINO,
                                            string sTP_BANCODADOSDESTINO)
    {
      bool bRet = false;
      DataTable oData = null;
      clsBancoDados oBancoDados_Destino = null;
      DataTable oData_Aux;

      string sSql;
      int intLinha;

      string vTemp;
      string sLinha;
      object vCampos;

      int iCampo;
      string sCampo;
      string[] sidCaracteristica = new string[201];
      int iCampoCodigo;

      object vFaixaInicialNova;
      object vFaixaInicialFinalNova;

      DataTable rcoProduto;
      DataTable rcoPreco;
      DataTable rcoCaracteristica;
      DataTable rcoPreco2;
      object vProtocolo;

      object iCodigo;
      int iFaixas;
      object iFaixa1;
      object iFaixa2;
      object iFaixa3;

      int iErro;
      int iNovo;
      int iEdita;

      string sDestino;

      double dProduto;

      double parProduto = 0;
      double parPrecoMinimoVenda = 0;
      int parFaixa = 0;
      int parFaixaInicialNova = 0;
      int parFaixaInicialFinalNova = 0;

      DataTable oData_Produto;
      DataTable oData_ProdutoPreco;

      DataRow oRow_Produto;
      DataRow oRow_ProdutoPreco;

      clsCampo oParam_ProdutoPreco_idTabelaPreco;
      clsCampo oParam_ProdutoPreco_idProduto;
      clsCampo oParam_ProdutoPreco_idUnidadeMedida;
      clsCampo oParam_ProdutoPreco_idProtocolo;
      clsCampo oParam_ProdutoPreco_idTabelaPrecoFaixa;
      clsCampo oParam_ProdutoPreco_nroFaixa;
      clsCampo oParam_ProdutoPreco_AgrupadorPreco;
      clsCampo oParam_ProdutoPreco_PrecoCusto;
      clsCampo oParam_ProdutoPreco_PrecoMinimo;
      clsCampo oParam_ProdutoPreco_PrecoVenda;
      clsCampo oParam_ProdutoPreco_PrecoExibicao;
      clsCampo oParam_ProdutoPreco_PrecoVendaUnitario;
      clsCampo oParam_ProdutoPreco_PrecoExibicaoUnitario;
      clsCampo oParam_ProdutoPreco_ValorPromocional;
      clsCampo oParam_ProdutoPreco_PrecoFaixaInicial;
      clsCampo oParam_ProdutoPreco_PrecoFaixaFinal;
      clsCampo oParam_ProdutoPreco_TextoFaixa;
      clsCampo oParam_ProdutoPreco_StatusPreco;

      //oData = FlexXTools.FlexXTools_DataTable("CONDICAO_PAGAMENTO", sConexao, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, "");

      oBancoDados_Destino = new clsBancoDados();
      oBancoDados_Destino.DBConectar(sDS_STRINGCONEXAODESTINO, sTP_BANCODADOSDESTINO);

      try
      {
        if ((oData != null) && (oBancoDados.DBConectado()))
        {
          try
          {
            //inicia laco de busca de caracteristicas
            intLinha = -1;
            iCampoCodigo = -1;
            dProduto = -1;

            //Gera Procoloco
            vProtocolo = System.DateTime.Parse(DateTime.Now.ToString()).ToString("yyyymmddhhnnss");

            //Zerando tabela de precos nao alteradas
            sSql = "Update tb_tabela_precos_produtos set TextoFaixa=nroFaixa, PrecoVenda=0, PrecoExibicao='0.00', PrecoVendaUnitario=0, PrecoExibicaoUnitario='0.00' where idTabelaPreco=4";
            oData_Aux = oBancoDados_Destino.DBQuery(sSql);

            foreach (DataRow oRow in oData.Rows)
            {
              //Adiciona campo no produto
              sSql = "SELECT * FROM tb_produtos WHERE idEmpresa=5 and Codigo='" + parProduto.ToString() + "'";
              oData_Produto = oBancoDados_Destino.DBQuery(sSql);

              if (oData_Produto.Rows.Count != 0)
              {
                oRow_Produto = oData_Produto.Rows[0];

                //Busca Precos Escalonados
                sSql = "SELECT * FROM tb_tabela_precos_produtos " +
                       " WHERE idTabelaPreco=4 and idProduto=" + oRow_Produto["idProduto"].ToString() +
                         " and nroFaixa=" + parFaixa.ToString() +
                         " and idUnidadeMedida=1";
                oData_ProdutoPreco = oBancoDados_Destino.DBQuery(sSql);

                if (oData_ProdutoPreco.Rows.Count == 0)
                {
                  oParam_ProdutoPreco_idTabelaPreco = new clsCampo { Nome = "idTabelaPreco", Valor = 4, Tipo = DbType.Int32 };
                  oParam_ProdutoPreco_idProduto = new clsCampo { Nome = "idProduto", Valor = Convert.ToInt32(oRow_Produto["idProduto"]), Tipo = DbType.Int32 };
                  oParam_ProdutoPreco_idUnidadeMedida = new clsCampo { Nome = "idUnidadeMedida", Valor = 1, Tipo = DbType.Int32 };
                }
                else
                {
                  oRow_ProdutoPreco = oData_ProdutoPreco.Rows[0];
                  oParam_ProdutoPreco_idTabelaPreco = new clsCampo { Nome = "idTabelaPreco", Valor = Convert.ToInt32(oRow_ProdutoPreco["idTabelaPreco"]), Tipo = DbType.Int32 };
                  oParam_ProdutoPreco_idProduto = new clsCampo { Nome = "idProduto", Valor = Convert.ToInt32(oRow_ProdutoPreco["idProduto"]), Tipo = DbType.Int32 };
                  oParam_ProdutoPreco_idUnidadeMedida = new clsCampo { Nome = "idUnidadeMedida", Valor = Convert.ToInt32(oRow_ProdutoPreco["idUnidadeMedida"]), Tipo = DbType.Int32 };
                }

                oParam_ProdutoPreco_idProtocolo = new clsCampo { Nome = "idProtocolo", Valor = Convert.ToInt64(vProtocolo), Tipo = DbType.Int64 };
                oParam_ProdutoPreco_nroFaixa = new clsCampo { Nome = "nroFaixa", Valor = Convert.ToInt32(parFaixa), Tipo = DbType.Int32 };
                oParam_ProdutoPreco_idProtocolo = new clsCampo { Nome = "idProtocolo", Valor = Convert.ToInt64(vProtocolo), Tipo = DbType.Int64 };
                oParam_ProdutoPreco_idTabelaPrecoFaixa = new clsCampo { Nome = "idTabelaPrecoFaixa", Valor = Convert.ToInt32(parFaixa), Tipo = DbType.Int32 };
                oParam_ProdutoPreco_AgrupadorPreco = new clsCampo { Nome = "AgrupadorPreco", Valor = oRow_Produto["idProduto"].ToString(), Tipo = DbType.String };
                oParam_ProdutoPreco_PrecoCusto = new clsCampo { Nome = "PrecoCusto", Valor = 0, Tipo = DbType.Int32 };
                oParam_ProdutoPreco_PrecoMinimo = new clsCampo { Nome = "PrecoMinimo", Valor = parPrecoMinimoVenda / 100, Tipo = DbType.Double };
                oParam_ProdutoPreco_PrecoVenda = new clsCampo { Nome = "PrecoVenda", Valor = parPrecoMinimoVenda / 100, Tipo = DbType.Double };
                oParam_ProdutoPreco_PrecoExibicao = new clsCampo { Nome = "PrecoExibicao", Valor = (parPrecoMinimoVenda / 100).ToString("c"), Tipo = DbType.String };
                oParam_ProdutoPreco_PrecoVendaUnitario = new clsCampo
                {
                  Nome = "PrecoVendaUnitario",
                  Valor = Math.Round(Convert.ToDouble(oParam_ProdutoPreco_PrecoVenda.Valor) /
                                                                                                                        Convert.ToDouble(oRow_Produto["fator_venda"]), 2),
                  Tipo = DbType.Double
                };
                oParam_ProdutoPreco_PrecoExibicaoUnitario = new clsCampo { Nome = "PrecoVendaUnitario", Valor = (Convert.ToDouble(oParam_ProdutoPreco_PrecoVendaUnitario.Valor) / 100).ToString("c"), Tipo = DbType.String };
                oParam_ProdutoPreco_ValorPromocional = new clsCampo { Nome = "PrecoVenda", Valor = 0, Tipo = DbType.Double };

                //Ajuste de Faixa
                switch (parFaixa)
                {
                  case 0:
                  case 1:
                    if (parFaixaInicialNova < 1) { parFaixaInicialNova = 1; }
                    break;
                  case 2:
                    if (parFaixaInicialNova < 2) { parFaixaInicialNova = 2; }
                    break;
                  case 3:
                    if (parFaixaInicialNova < 3) { parFaixaInicialNova = 3; }
                    break;
                }

                if (parFaixaInicialNova > parFaixaInicialFinalNova) { parFaixaInicialFinalNova = parFaixaInicialNova; }

                oParam_ProdutoPreco_PrecoFaixaInicial = new clsCampo { Nome = "PrecoFaixaInicial", Valor = parFaixaInicialNova, Tipo = DbType.Double };
                oParam_ProdutoPreco_PrecoFaixaFinal = new clsCampo { Nome = "PrecoFaixaFinal", Valor = parFaixaInicialNova, Tipo = DbType.Double };

                switch (parFaixa)
                {
                  case 1:
                    if (parFaixaInicialNova == parFaixaInicialFinalNova)
                    {
                      oParam_ProdutoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Valor = parFaixaInicialNova.ToString().Trim(), Tipo = DbType.String };
                    }
                    else
                    {
                      oParam_ProdutoPreco_TextoFaixa = new clsCampo
                      {
                        Nome = "TextoFaixa",
                        Valor = parFaixaInicialNova.ToString().Trim() + " a " +
                                                                                                   parFaixaInicialFinalNova.ToString().Trim(),
                        Tipo = DbType.String
                      };
                    }
                    break;
                  case 2:
                    if (parFaixaInicialNova == parFaixaInicialFinalNova)
                    {
                      oParam_ProdutoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Valor = parFaixaInicialNova.ToString().Trim(), Tipo = DbType.String };
                    }
                    else
                    {
                      oParam_ProdutoPreco_TextoFaixa = new clsCampo
                      {
                        Nome = "TextoFaixa",
                        Valor = parFaixaInicialNova.ToString().Trim() + " a " +
                                                                                                   parFaixaInicialFinalNova.ToString().Trim(),
                        Tipo = DbType.String
                      };
                    }
                    break;
                  case 3:
                    oParam_ProdutoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Valor = parFaixaInicialNova.ToString().Trim() + " ou mais", Tipo = DbType.String };
                    break;
                  default:
                    {
                      oParam_ProdutoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Valor = parFaixaInicialNova.ToString().Trim(), Tipo = DbType.String };
                    }
                    break;
                }

                oParam_ProdutoPreco_StatusPreco = new clsCampo { Nome = "StatusPreco", Valor = 1, Tipo = DbType.Int32 };

                //Atualiza produto
                if (parFaixa == 1)
                {
                  sSql = "update tb_produtos" +
                         " set Preco1Venda = " + oParam_ProdutoPreco_PrecoVenda.Valor.ToString() +
                             ",Preco2VendaUnitario = " + oParam_ProdutoPreco_PrecoVendaUnitario.Valor.ToString() +
                             ",PrecoVendaUnitarioExib = '" + oParam_ProdutoPreco_PrecoVendaUnitario.Valor.ToString() + "'" +
                             ",Custo = 0" +
                             ",CustoUnitario = 0" +
                             ",Sincronizado='S'" +
                             ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                             ",verSincronizador='" + ProductVersion + "'" +
                             ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                         " where idProduto = " + oRow_Produto["idProduto"].ToString();
                  oBancoDados_Destino.DBExecutar(sSql);
                }

                sSql = "update tb_tabela_precos_produtos";

                //          End If



                //          rcoProduto.Close

                //proximoproduto:
                //          'Seta produto
                //          vProduto = Val(vCampos(0))


                //        Loop While Not EOF(1)

                //        Close 1


                //        Close #21

                //        If Not ConectarDB Then
                //           MsgBox "Erro ao conectar banco de dados!", vbCritical + vbOKOnly, "mrSoft"
                //           Exit Function
                //        End If


                //        'Ajusta faixas
                //        Set rcoProduto = New ADODB.Recordset

                //        sSql = "SELECT tb_produtos.* FROM tb_produtos "
                //        sSql = sSql & vbCr & "LEFT JOIN tb_categorias1 ON tb_categorias1.idCategoria1 = tb_produtos.idCategoria1"
                //        sSql = sSql & vbCr & "LEFT JOIN tb_status    ON tb_status.idStatus = tb_produtos.idStatus"
                //        sSql = sSql & vbCr & "Where tb_produtos.idEmpresa=5 and tb_categorias1.Categoria1Ativa = 1 And tb_status.PermitidoVender = 1"
                //        rcoProduto.Open sSql, Conexao, adOpenKeyset, adLockOptimistic



                //         While Not rcoProduto.EOF


                //           lblInfo.Caption = "Ajustando Preco " & rcoProduto!idProduto
                //           DoEvents


                //           'If rcoProduto!idProduto = 2333 Then Stop


                //           Set rcoPreco = New ADODB.Recordset


                //           'Busca Precos Escalonados
                //           sSql = "SELECT * FROM tb_tabela_precos_produtos "
                //           sSql = sSql & vbCr & "WHERE idTabelaPreco=4 and idProduto=" & rcoProduto!idProduto
                //           sSql = sSql & vbCr & "Order by nroFaixa desc"
                //           rcoPreco.Open sSql, Conexao, adOpenKeyset, adLockOptimistic


                //           vFaixaInicialNova = 0
                //           iFaixas = 0
                //           iFaixa1 = 0
                //           iFaixa2 = 0
                //           iFaixa3 = 0


                //           While Not rcoPreco.EOF

                //                 If rcoPreco!nroFaixa = 1 Then
                //                    iFaixa1 = rcoPreco!PrecoFaixaInicial


                //                    Set rcoPreco2 = New ADODB.Recordset


                //                   'Busca Precos Escalonados
                //                   sSql = "SELECT * FROM tb_tabela_precos_produtos "
                //                   sSql = sSql & vbCr & "WHERE idTabelaPreco=4 and nroFaixa=2 and idProduto=" & rcoProduto!idProduto
                //                   sSql = sSql & vbCr & "Order by nroFaixa"
                //                   rcoPreco2.Open sSql, Conexao, adOpenKeyset, adLockOptimistic


                //                   If Not rcoPreco2.EOF Then
                //                     If rcoPreco!PrecoFaixaFinal >= rcoPreco2!PrecoFaixaInicial Or rcoPreco2!PrecoFaixaInicial > 1 Or rcoPreco2!PrecoFaixaInicial - rcoPreco!PrecoFaixaFinal > 1 Then
                //                        rcoPreco!PrecoFaixaFinal = rcoPreco2!PrecoFaixaInicial - 1
                //                        If rcoPreco!PrecoFaixaFinal < 1 Then rcoPreco!PrecoFaixaFinal = 1
                //                        If rcoPreco!PrecoFaixaInicial > rcoPreco!PrecoFaixaFinal Then rcoPreco!PrecoFaixaInicial = rcoPreco!PrecoFaixaFinal


                //                        If rcoPreco!PrecoFaixaInicial > 1 Then rcoPreco!PrecoFaixaInicial = 1


                //                        If rcoPreco!PrecoFaixaInicial = rcoPreco!PrecoFaixaFinal Then
                //                            rcoPreco!TextoFaixa = Trim(Str(rcoPreco!PrecoFaixaInicial))
                //                        Else
                //                            rcoPreco!TextoFaixa = Trim(Str(rcoPreco!PrecoFaixaInicial)) & " a " & Trim(Str(rcoPreco!PrecoFaixaFinal))
                //                        End If


                //                        rcoPreco.Update

                //                     End If

                //                   End If


                //                   rcoPreco2.Close

                //                 End If

                //                 If rcoPreco!nroFaixa = 2 Then
                //                    iFaixa2 = rcoPreco!PrecoFaixaInicial


                //                    Set rcoPreco2 = New ADODB.Recordset


                //                   'Busca Precos Escalonados
                //                   sSql = "SELECT * FROM tb_tabela_precos_produtos "
                //                   sSql = sSql & vbCr & "WHERE idTabelaPreco=4 and nroFaixa=3 and idProduto=" & rcoProduto!idProduto
                //                   sSql = sSql & vbCr & "Order by nroFaixa"
                //                   rcoPreco2.Open sSql, Conexao, adOpenKeyset, adLockOptimistic


                //                   If Not rcoPreco2.EOF Then
                //                     If rcoPreco!PrecoFaixaFinal >= rcoPreco2!PrecoFaixaInicial Or rcoPreco2!PrecoFaixaInicial - rcoPreco!PrecoFaixaFinal > 1 Then
                //                        rcoPreco!PrecoFaixaFinal = rcoPreco2!PrecoFaixaInicial - 1
                //                        If rcoPreco!PrecoFaixaFinal < 1 Then rcoPreco!PrecoFaixaFinal = 1
                //                        If rcoPreco!PrecoFaixaInicial > rcoPreco!PrecoFaixaFinal Then rcoPreco!PrecoFaixaInicial = rcoPreco!PrecoFaixaFinal


                //                        If rcoPreco!PrecoFaixaInicial = rcoPreco!PrecoFaixaFinal Then
                //                            rcoPreco!TextoFaixa = Trim(Str(rcoPreco!PrecoFaixaInicial))
                //                        Else
                //                            rcoPreco!TextoFaixa = Trim(Str(rcoPreco!PrecoFaixaInicial)) & " a " & Trim(Str(rcoPreco!PrecoFaixaFinal))
                //                        End If


                //                        rcoPreco.Update

                //                     End If
                //                   End If


                //                   rcoPreco2.Close

                //                 End If


                //                 If rcoPreco!nroFaixa = 3 Then
                //                    iFaixa3 = rcoPreco!PrecoFaixaInicial



                //                    If rcoPreco!PrecoFaixaFinal < rcoPreco!PrecoFaixaInicial Then
                //                        rcoPreco!PrecoFaixaFinal = rcoPreco!PrecoFaixaInicial


                //                        If rcoPreco!PrecoFaixaInicial = rcoPreco!PrecoFaixaFinal Then
                //                            rcoPreco!TextoFaixa = Trim(Str(rcoPreco!PrecoFaixaInicial)) & " ou mais"
                //                        Else
                //                            rcoPreco!TextoFaixa = Trim(Str(rcoPreco!PrecoFaixaInicial)) & " a " & Trim(Str(rcoPreco!PrecoFaixaFinal))
                //                        End If


                //                        rcoPreco.Update
                //                    ElseIf rcoPreco!PrecoFaixaFinal = rcoPreco!PrecoFaixaInicial Then


                //                        If rcoPreco!PrecoFaixaInicial = rcoPreco!PrecoFaixaFinal Then
                //                            rcoPreco!TextoFaixa = Trim(Str(rcoPreco!PrecoFaixaInicial)) & " ou mais"
                //                        Else
                //                            rcoPreco!TextoFaixa = Trim(Str(rcoPreco!PrecoFaixaInicial)) & " a " & Trim(Str(rcoPreco!PrecoFaixaFinal))
                //                        End If



                //                        rcoPreco!PrecoFaixaFinal = 99999999


                //                        rcoPreco.Update

                //                    End If


                //                 End If



                //                  'Salvando api
                //                  sSql = "SELECT"
                //                  sSql = sSql & vbCr & "tb_tabela_precos_produtos.idTabelaPrecoProduto as chave"
                //                  sSql = sSql & vbCr & ",cast(tb_tabela_precos_produtos.idTabelaPrecoProduto as char) as idTabelaPrecoProduto"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.idTabelaPreco"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.idTabelaPreco as idTabelaPrecoFaixa"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.idUnidadeMedida"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.idProduto"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoCusto"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoMinimo"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoVenda"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoVendaUnitario"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.ValorPromocional"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoFaixaInicial"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.nroFaixa"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoFaixaFinal"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.AgrupadorPreco"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.TextoFaixa"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoExibicao"
                //                  sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoExibicaoUnitario"
                //                  sSql = sSql & vbCr & "From tb_tabela_precos_produtos"
                //                  sSql = sSql & vbCr & "Where tb_tabela_precos_produtos.idTabelaPreco = 4"
                //                  sSql = sSql & vbCr & "and tb_tabela_precos_produtos.idProduto = " & rcoPreco!idProduto
                //                  sSql = sSql & vbCr & "and tb_tabela_precos_produtos.nroFaixa = " & rcoPreco!nroFaixa
                //                 ' Set rcoPreco2 = New ADODB.Recordset
                //                 ' rcoPreco2.Open sSql, Conexao, adOpenKeyset, adLockOptimistic


                //                  'Atualizando APP
                //               '   sSql = "http://fb.pedidodireto.com/api.php?table=tabelaprecoprodutos&op=edit&"
                //               '   sSql = sSql & "&AgrupadorPreco=" & rcoPreco2!AgrupadorPreco
                //               '   sSql = sSql & "&PrecoCusto=" & rcoPreco2!PrecoCusto
                //               '   sSql = sSql & "&PrecoExibicao=" & rcoPreco2!PrecoExibicao
                //               '   sSql = sSql & "&PrecoExibicaoUnitario=" & rcoPreco2!PrecoExibicaoUnitario
                //               '   sSql = sSql & "&PrecoFaixaFinal=" & rcoPreco2!PrecoFaixaFinal
                //               '   sSql = sSql & "&PrecoFaixaInicial=" & rcoPreco2!PrecoFaixaInicial
                //               '   sSql = sSql & "&PrecoMinimo=" & rcoPreco2!PrecoMinimo
                //               '   sSql = sSql & "&PrecoVenda=" & rcoPreco2!PrecoVenda
                //               '   sSql = sSql & "&PrecoVendaUnitario=" & rcoPreco2!PrecoVendaUnitario
                //               '   sSql = sSql & "&TextoFaixa=" & rcoPreco2!TextoFaixa
                //               '   sSql = sSql & "&ValorPromocional=" & rcoPreco2!ValorPromocional
                //               '   sSql = sSql & "&idProduto=" & rcoPreco2!idProduto
                //               '   sSql = sSql & "&idTabelaPreco=" & rcoPreco2!idTabelaPreco
                //               '   sSql = sSql & "&idTabelaPrecoFaixa=" & rcoPreco2!idTabelaPrecoFaixa
                //               '   sSql = sSql & "&idTabelaPrecoProduto=" & rcoPreco2!idTabelaPrecoProduto
                //               '   sSql = sSql & "&idUnidadeMedida=" & rcoPreco2!idUnidadeMedida
                //               '   sSql = sSql & "&nroFaixa=" & rcoPreco2!nroFaixa
                //               '   sSql = sSql & "&id=" & rcoPreco2!chave


                //                  'sSql = Inet1.OpenURL(sSql)



                //                  'rcoPreco2.Close


                //                 'proximo produto
                //                 rcoPreco.MoveNext
                //           Wend


                //           'Ajustando produtos
                //           sSql = "SELECT"
                //           sSql = sSql & vbCr & "concat(prod.idEmpresa, '-', prod.idProduto) As Chave"
                //           sSql = sSql & vbCr & ",ifnull(tb_categorias1.Categoria1,'A definir') as Categoria1"
                //           sSql = sSql & vbCr & ",ifnull(tb_categorias2.Categoria2,'A definir') as Categoria2"
                //           sSql = sSql & vbCr & ",ifnull(prod.Descricao,'Falta Descricao') as Descricao"
                //           sSql = sSql & vbCr & ",ifnull(prod.Descricao_Detalhada,'Falta Detalhada') as Descricao_Detalhada"
                //           sSql = sSql & vbCr & ",ifnull(prod.Estoque,0) as Estoque"
                //           sSql = sSql & vbCr & ",ifnull(prod.ImagemNormal,'padrao.png') as ImagemNormal"
                //           sSql = sSql & vbCr & ",ifnull(prod.ImagemMobile,'padrao.png') as ImagemMobile"
                //           sSql = sSql & vbCr & ",ifnull(prod.Preco1Venda,0) as MenorPreco"
                //           sSql = sSql & vbCr & ",ifnull(prod.Nome_Amigavel,'Falta Nome') as Nome_Amigavel"
                //           sSql = sSql & vbCr & ",ifnull(tb_categorias2.OrdemCategoria2,'Falta OC2') as OrdemCategoria2"
                //           sSql = sSql & vbCr & ",ifnull(prod.OrdemExibicao,'Falta Ord') as OrdemProduto"
                //           sSql = sSql & vbCr & ",ifnull(prod.Preco1Venda,0) as Preco1Venda"
                //           sSql = sSql & vbCr & ",ifnull(prod.Quantidade_na_embalagem,1) as Quantidade_na_embalagem"
                //           sSql = sSql & vbCr & ",ifnull(prod.Quantidade_Exibicao,'1') as Quantidade_Exibicao"
                //           sSql = sSql & vbCr & ",ifnull(tb_unidademedida.UnidadeMedida,'embalagem') as Embalagem"
                //           sSql = sSql & vbCr & ",ifnull(tb_status.Status,'nao vender') as Status"
                //           sSql = sSql & vbCr & ",ifnull(tb_status.PermitidoVender,0) as StatusVender"
                //           sSql = sSql & vbCr & ",ifnull(tb_status.idStatus,4) as idStatus"
                //           sSql = sSql & vbCr & ",ifnull(prod.SKU,prod.idProduto) as SKU"
                //           sSql = sSql & vbCr & ",ifnull(prod.idCategoria1,0) as idCategoria1"
                //           sSql = sSql & vbCr & ",ifnull(prod.idCategoria2,0) as idCategoria2"
                //           sSql = sSql & vbCr & ",ifnull(prod.idEmpresa,0) as idEmpresa"
                //           sSql = sSql & vbCr & ",prod.idProduto"
                //           sSql = sSql & vbCr & "From"
                //           sSql = sSql & vbCr & "tb_produtos prod"
                //           sSql = sSql & vbCr & "LEFT JOIN tb_categorias1 ON tb_categorias1.idCategoria1 = prod.idCategoria1"
                //           sSql = sSql & vbCr & "LEFT JOIN tb_categorias2 ON tb_categorias2.idCategoria2 = prod.idCategoria2"
                //           sSql = sSql & vbCr & "LEFT JOIN tb_categorias3 ON tb_categorias3.idCategoria3 = prod.idCategoria3"
                //           sSql = sSql & vbCr & "LEFT JOIN tb_embalagem ON tb_embalagem.idEmbalagem = prod.idEmbalagem"
                //           sSql = sSql & vbCr & "LEFT JOIN tb_status    ON tb_status.idStatus = prod.idStatus"
                //           sSql = sSql & vbCr & "LEFT JOIN tb_unidademedida    ON tb_unidademedida.idUnidadeMedida = prod.idUnidadeMedidaVenda"
                //           sSql = sSql & vbCr & "Where tb_categorias1.Categoria1Ativa = 1 And tb_status.PermitidoVender = 1"
                //           sSql = sSql & vbCr & "and idProduto=" & rcoProduto!idProduto


                //            Set rcoPreco2 = New ADODB.Recordset
                //           ' rcoPreco2.Open sSql, Conexao, adOpenKeyset, adLockOptimistic


                //           ' If Not rcoPreco2.EOF Then
                //           '    sSql = "http://fb.pedidodireto.com/api.php?table=products&op=edit&"
                //           '    sSql = sSql & "&Categoria1=" & rcoPreco2!Categoria1
                //           '    sSql = sSql & "&Categoria2=" & rcoPreco2!Categoria2
                //           '    sSql = sSql & "&Descricao=" & rcoPreco2!Descricao
                //           '    sSql = sSql & "&Descricao_Detalhada=" & rcoPreco2!Descricao_Detalhada
                //           '    sSql = sSql & "&Embalagem=" & rcoPreco2!Embalagem
                //           '    sSql = sSql & "&Estoque=" & rcoPreco2!Estoque
                //           '    sSql = sSql & "&ImagemMobile=" & rcoPreco2!ImagemMobile
                //           '   sSql = sSql & "&ImagemNormal=" & rcoPreco2!ImagemNormal
                //           '    sSql = sSql & "&MenorPreco=" & rcoPreco2!MenorPreco
                //           '    sSql = sSql & "&Nome_Amigavel=" & rcoPreco2!Nome_Amigavel
                //           '    sSql = sSql & "&OrdemCategoria2=" & rcoPreco2!OrdemCategoria2
                //           '    sSql = sSql & "&OrdemProduto=" & rcoPreco2!OrdemProduto
                //           '    sSql = sSql & "&Preco1Venda=" & rcoPreco2!Preco1Venda
                //           '    sSql = sSql & "&Quantidade_Exibicao=" & rcoPreco2!Quantidade_Exibicao
                //           '    sSql = sSql & "&Quantidade_na_embalagem=" & rcoPreco2!Quantidade_na_embalagem
                //           '    sSql = sSql & "&SKU=" & rcoPreco2!SKU
                //           '    sSql = sSql & "&Status=" & rcoPreco2!Status
                //           '    sSql = sSql & "&StatusVender=" & rcoPreco2!StatusVender
                //           '    sSql = sSql & "&idCategoria1=" & rcoPreco2!idCategoria1
                //           '    sSql = sSql & "&idCategoria2=" & rcoPreco2!idCategoria2
                //           '    sSql = sSql & "&idEmpresa=" & rcoPreco2!idEmpresa
                //           '    sSql = sSql & "&idProduto=" & rcoPreco2!idProduto
                //           '    sSql = sSql & "&idStatus=" & rcoPreco2!idStatus
                //           '    sSql = sSql & "&id=" & rcoPreco2!chave


                //               'sSql = Inet1.OpenURL(sSql)
                //           'End If


                //           rcoProduto.MoveNext
                //        Wend



                //        rcoProduto.Close

                //        sSql = "SELECT concat(prod.idEmpresa, '-', prod.idProduto) As Chave FROM tb_produtos "
                //        sSql = sSql & vbCr & "LEFT JOIN tb_categorias1 ON tb_categorias1.idCategoria1 = tb_produtos.idCategoria1"
                //        sSql = sSql & vbCr & "LEFT JOIN tb_status    ON tb_status.idStatus = tb_produtos.idStatus"
                //        sSql = sSql & vbCr & "Where tb_produtos.idEmpresa=5  And tb_status.PermitidoVender = 0"


                //        Set rcoProduto = New ADODB.Recordset
                //        rcoProduto.Open sSql, Conexao, adOpenKeyset, adLockOptimistic



                //        While Not rcoProduto.EOF


                //              lblInfo.Caption = "Removendo Produtos que nao vende mais " & rcoProduto!idProduto
                //              DoEvents

                //              sSql = "http://fb.pedidodireto.com/api.php?table=products&op=delete&id=" & rcoProduto!chave
                //              'sSql = Inet1.OpenURL(sSql)


                //              rcoProduto.MoveNext
                //        Wend


                //        rcoProduto.Close






                //        'finaliza log


                //        Log "Importação finalizada"
                //        Log "Novos Registros " & iNovo
                //        Log "Registros Editados  " & iEdita
                //        Log "Erros Registros " & iErro


                //        sDestino = Left(txtArquivo, Len(txtArquivo) - 3) & "pro"
                //        If Dir(sDestino) <> "" Then Kill sDestino
                //        Log "Renomeado > " & sDestino
                //        Name txtArquivo As sDestino


                //        rotImportaPrecosPPBH = True


                //        'cmdImportar.Enabled = True


                //final:
                //            Exit Function


                //AdoError:

                //            Dim errLoop As ADODB.Error
                //            Dim strError As String
                //            Dim sErro As String
                //            Dim I As Integer



                //      'If Err = 3705 And bConectando Then
                //      '   Set mConexaoDb2 = New ADODB.Connection
                //      '   Resume Inicio
                //      'End If


                //      'If Err = 3705 Then Resume Next


                //      'mbErroConexao = True


                //      sErro = ""
                //      sErro = sErro & "Ocorreu algum erro durante a importação do arquivo."


                //      If Err.Number <> 0 Then
                //         sErro = sErro & vbNewLine & String$(80, "-")
                //         sErro = sErro & vbNewLine & "Error # " & Str$(Err.Number)
                //         sErro = sErro & vbNewLine & "Gerado por " & Err.Source
                //         sErro = sErro & vbNewLine & "Descrição  " & Err.Description
                //         sErro = sErro & vbNewLine & String$(80, "-")
                //      End If


                //      MsgBox sErro, vbCritical +vbOKOnly, "mrSoft"

                //                return true;
                //            }
                //            catch (Exception Ex)
                //            {
                //                oBancoDados.DBSQL_Log_Gravar(iIdEmpresa, LogTipo.ErroNaRotina_ImportaPreco, iIdTarefa, Ex.Message, sArquivo);

                //                return false;
                //            }
              }

              intLinha = intLinha + 1;
            }
          }
          catch (Exception Ex)
          {
          }
        }
      }
      catch (Exception)
      {

        throw;
      }

      return bRet;
    }

    private static Boolean FlagWZ_DisUsuario(string sAplicativo,
                                             string sPartner,
                                             string sCdServico,
                                             int iIdEmpresa,
                                             int iIdTarefa,
                                             string sConexao,
                                             string sUsuario,
                                             string sSenha,
                                             string sCOD_PUXADA,
                                             string sDS_STRINGCONEXAODESTINO,
                                             string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;
      int iIdEmpresaGestora = 0;
      int iIdServico = 0;
      int iIdUsuario = 0;
      string ServicoUsuarioAtivo = "";
      DataTable oData = null;
      string sNotificacao_NovaEmpresa_Telefone = "";
      string sNotificacao_NovoUsuario_Telefone = "";
      string sNotificacao_UsuarioSemAcesso_Telefone = "";
      string sNotificacao_NovaEmpresa_Nome = "";
      string sNotificacao_NovoUsuario_Nome = "";
      string sNotificacao_UsuarioSemAcesso_Nome = "";
      bool bNovaEmpresa = false;
      bool bNovoUsuario = false;
      bool bSemAcesso = false;
      string sMensagem = "";
      string sValidacao_Parametro = "";
      string sValidacao_Chave = "";
      string sValidacao_StringConexao = "";
      string sCodPuxada = "";
      string sDepartamento = "";
      string sNome = "";

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DadosUsuario_Root DadosUsuario_Root;
        string sSql;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "";
        string json_util = "";
        double TempoExecucao = 0;

        if (sCOD_PUXADA.Trim() != "")
        {
          json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"}";
        }

        // json = "{\"DAT_ATUALIZACAO\":\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\"}";

        request.ContentLength = json.Length;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          if (json.Trim() != "")
          {
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
          }
        }

        var content = string.Empty;

        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              DadosUsuario_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosUsuario_Root>(content);
            }

            if (DadosUsuario_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = DadosUsuario_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        if (DadosUsuario_Root != null)
        {
          if (DadosUsuario_Root.DadosUsuario.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();
            clsBancoDados oBancoDadosGerenciador = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            oBancoDadosGerenciador.DBConectar(Config_App.sDB_Tipo, Config.dbconstring);

            oData = oBancoDadosGerenciador.DBQuery("select * from tb_Notificacao");

            foreach (DataRow oRow in oData.Rows)
            {
              switch (oRow["GrupoNotificacao"].ToString())
              {
                case "NovaEmpresa":
                  sNotificacao_NovaEmpresa_Telefone = oRow["Telefone"].ToString();
                  sNotificacao_NovaEmpresa_Nome = sNotificacao_NovaEmpresa_Telefone.Split('|')[1].ToString();
                  sNotificacao_NovaEmpresa_Telefone = sNotificacao_NovaEmpresa_Telefone.Split('|')[0].ToString();
                  break;
                case "NovoUsuario":
                  sNotificacao_NovoUsuario_Telefone = oRow["Telefone"].ToString();
                  sNotificacao_NovoUsuario_Nome = sNotificacao_NovoUsuario_Telefone.Split('|')[1].ToString();
                  sNotificacao_NovoUsuario_Telefone = sNotificacao_NovoUsuario_Telefone.Split('|')[0].ToString();
                  break;
                case "UsuarioSemAcesso":
                  sNotificacao_UsuarioSemAcesso_Telefone = oRow["Telefone"].ToString();
                  sNotificacao_UsuarioSemAcesso_Nome = sNotificacao_UsuarioSemAcesso_Telefone.Split('|')[1].ToString();
                  sNotificacao_UsuarioSemAcesso_Telefone = sNotificacao_UsuarioSemAcesso_Telefone.Split('|')[0].ToString();
                  break;
              }
            }

            sSql = "select bd.*" +
                   " from tb_tarefas tf" +
                   " inner join tb_empresas_integracao ei on ei.idEmpresaIntegracao = tf.idEmpresaIntegracao" +
                   " inner join tb_tipo_integracao ti on ti.id_tipo_integracao = ei.idtipointegracao" +
                   " inner join tb_bancodados bd on bd.id_bancodados = ti.idBancoConexaoValidacao" +
                   " where idTarefa = " + iIdTarefa.ToString();
            oData = oBancoDadosGerenciador.DBQuery(sSql);

            if (oData.Rows.Count != 0)
            {
              sValidacao_Parametro = oData.Rows[0]["ds_parametro"].ToString();
              sValidacao_Chave = oData.Rows[0]["ds_chave"].ToString();
              sValidacao_StringConexao = oData.Rows[0]["ds_stringconexao"].ToString();
            }

            oData.Dispose();

            foreach (FlagWSWhatsapp_DadosUsuario DadosUsuario in DadosUsuario_Root.DadosUsuario)
            {
              bNovaEmpresa = false;
              bNovoUsuario = false;
              bSemAcesso = false;

              sCodPuxada = DadosUsuario.PUXADA.Trim();

              if (sCodPuxada.Length != 8)
              {
                sCodPuxada = ("00000000" + sCodPuxada).Substring(("00000000" + sCodPuxada).Length - 8);
              }

              sSql = "select count(*) from tb_servicos where cd_Servico = '" + DadosUsuario.LICENCA.Trim() + "'";

              if ((Convert.ToInt32(oBancoDadosGerenciador.DBQuery_ValorUnico(sSql)) != 0) && (DadosUsuario.TELEFONE.ToString().Trim() != ""))
              {
                if (sValidacao_Chave != "")
                {
                  oData = FlexXTools.FlexXTools_DataTable(sAplicativo,
                                                          sPartner,
                                                          sCdServico,
                                                          ref sStatus,
                                                          ref sErro,
                                                          ref TempoExecucao,
                                                          iIdTarefa,
                                                          sValidacao_Chave,
                                                          sValidacao_StringConexao,
                                                          "", "", "",
                                                          sValidacao_Parametro.Replace("[WS_TELEFONE]", DadosUsuario.TELEFONE.ToString()),
                                                          ref json_util);
                  if (oData != null)
                  {
                    bSemAcesso = (oData.Rows.Count == 0);
                  }
                }

                sSql = "SELECT * FROM dbo.tb_servicos where upper(cd_Servico) = '" + DadosUsuario.LICENCA.Trim().ToUpper() + "'";
                oData = oBancoDadosGerenciador.DBQuery(sSql);

                if (oData.Rows.Count != 0)
                {
                  iIdEmpresaGestora = Convert.ToInt32(oData.Rows[0]["id_Empresa"]);
                  iIdServico = Convert.ToInt32(oData.Rows[0]["id_Servico"]);
                }
                else
                {
                  iIdEmpresaGestora = 0;
                  iIdServico = 0;
                }

                sSql = "select count(*) from tb_empresas where CodPuxada = '" + sCodPuxada + "'";
                if (Convert.ToInt32(oBancoDadosGerenciador.DBQuery_ValorUnico(sSql)) == 0)
                {
                  bNovaEmpresa = true;

                  sSql = "INSERT INTO tb_empresas (id_Tipo_Empresa,idEmpresa_Gestora,Empresa,CNPJ,CodPuxada,SistemaLiberado,tp_liberado)" +
                         " VALUES (#id_Tipo_Empresa,#idEmpresa_Gestora,#Empresa,#CNPJ,#CodPuxada,#SistemaLiberado,#tp_liberado)";
                  oBancoDadosGerenciador.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "id_Tipo_Empresa", Valor = 3, Tipo = DbType.Int32 },
                                                                                             new clsCampo { Nome = "idEmpresa_Gestora", Valor = iIdEmpresaGestora, Tipo = DbType.Int32 },
                                                                                             new clsCampo { Nome = "Empresa", Valor = DadosUsuario.EMPRESA.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "CNPJ", Valor = DadosUsuario.CNPJ.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "CodPuxada", Valor = sCodPuxada, Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "SistemaLiberado", Valor = "S", Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "tp_liberado", Valor = "S", Tipo = DbType.String }});
                }

                sSql = "select idEmpresa from tb_empresas where CodPuxada = '" + sCodPuxada + "'";
                iIdEmpresa = Convert.ToInt32(oBancoDadosGerenciador.DBQuery_ValorUnico(sSql));

                sSql = "select count(*) from tb_tipo_integracao where id_Servico = " + iIdServico.ToString() + " and isnull(ic_ativo, 'N') = 'S'";
                if (Convert.ToInt32(oBancoDadosGerenciador.DBQuery_ValorUnico(sSql)) != 0)
                {
                  sSql = "select count(*) from tb_empresas_servicos where id_Empresa = " + iIdEmpresa.ToString() + " and id_Servico = " + iIdServico.ToString();
                  if (Convert.ToInt32(oBancoDadosGerenciador.DBQuery_ValorUnico(sSql)) == 0)
                  {
                    sSql = "insert into tb_empresas_servicos (id_Empresa,id_Servico,tp_Licenciamento,tp_ativo) values (" + iIdEmpresa.ToString() + "," + iIdServico.ToString() + ", 'S','S')";
                    oBancoDadosGerenciador.DBExecutar(sSql);
                  }
                }

                sSql = "SELECT COUNT(*) FROM tb_Usuario" +
                        " WHERE (COD_PUXADA='" + sCodPuxada + "' AND TELEFONE='" + DadosUsuario.TELEFONE.ToString().Trim() + "')";
                if (Convert.ToInt32(oBancoDadosGerenciador.DBQuery_ValorUnico(sSql)) == 0)
                {
                  bNovoUsuario = true;

                  Processar_Sincronizacao(sAplicativo, sPartner, "INC", sCOD_PUXADA, DadosUsuario.USUARIO.Trim(), UltimoErro, "IFP",
                                          Integrador_Funcoes.TempoExecucaoAPI.ToString(),
                                          Integrador_Funcoes.TempoExecucaoIntegrador.ToString(), "", "", sConexao, sCdServico);

                  sSql = "INSERT INTO tb_Usuario (idEmpresa,Cod_Puxada,Nome,Senha,Departamento,Email,Ativo" +
                                                  ",CNPJ,Cod_Empregado,FLEXXPOWER,LICENCA,SOFIA,ID_USUARIO,TELEFONE,DTINTEGRACAO,VERSAO_INTEGRADOR,tp_liberado)" +
                         " VALUES(#idEmpresa,#Cod_Puxada,#Nome,#Senha,#Departamento,#Email,#Ativo" +
                                ",#CNPJ,#Cod_Empregado,#FLEXXPOWER,#LICENCA,#SOFIA,#ID_USUARIO,#TELEFONE,#DTINTEGRACAO,#VERSAO_INTEGRADOR,'S')";
                  oBancoDadosGerenciador.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "idEmpresa", Valor = iIdEmpresa, Tipo = DbType.Int32 },
                                                                                             new clsCampo { Nome = "COD_PUXADA", Valor = sCodPuxada, Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Nome", Valor = DadosUsuario.USUARIO.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Senha", Valor = DadosUsuario.SENHA.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Departamento", Valor = DadosUsuario.DEPARTAMENTO.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Email", Valor = DadosUsuario.EMAIL.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Ativo", Valor = Convert.ToInt32(DadosUsuario.ATIVO.Trim()), Tipo = DbType.Int32 },
                                                                                             new clsCampo { Nome = "CNPJ", Valor = DadosUsuario.CNPJ.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Cod_Empregado", Valor = DadosUsuario.EMPREGADO.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "FLEXXPOWER", Valor = DadosUsuario.FLEXXPOWER.Trim().ToUpper().Substring(0, 1), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "LICENCA", Valor = DadosUsuario.LICENCA.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "SOFIA", Valor = "S", Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "ID_USUARIO", Valor = DadosUsuario.ID_USUARIO, Tipo = DbType.Int32 },
                                                                                             new clsCampo { Nome = "TELEFONE", Valor = DadosUsuario.TELEFONE.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                             new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});

                  Bot.Webook_Util(1, DadosUsuario.LICENCA.Trim(), Constantes.const_TratamentoSofia_BEM_VINDO_DASH, "", Constantes.const_Provider_Zap, DadosUsuario.USUARIO.Trim(), DadosUsuario.TELEFONE.Trim(), "sofia", "");
                }
                else
                {
                  Processar_Sincronizacao(sAplicativo, sPartner, "ALT", sCOD_PUXADA, DadosUsuario.USUARIO.Trim(), UltimoErro, "IFP",
                                          Integrador_Funcoes.TempoExecucaoAPI.ToString(),
                                          Integrador_Funcoes.TempoExecucaoIntegrador.ToString(), "", "", sConexao, sCdServico);

                  sSql = "UPDATE tb_Usuario" +
                         " SET Cod_Puxada=#Cod_Puxada," +
                              "Nome=#Nome," +
                              //"Senha=#Senha," +
                              "Departamento=#Departamento," +
                              "Email=#Email," +
                              "Ativo=#Ativo," +
                              "CNPJ=#CNPJ," +
                              "Cod_Empregado=#Cod_Empregado," +
                              "FLEXXPOWER=#FLEXXPOWER," +
                              "LICENCA=#LICENCA," +
                              "SOFIA=#SOFIA," +
                              "ID_USUARIO=#ID_USUARIO," +
                              "TELEFONE=#TELEFONE," +
                              "DTINTEGRACAO=#DTINTEGRACAO," +
                              "VERSAO_INTEGRADOR=#VERSAO_INTEGRADOR" +
                      " WHERE (COD_PUXADA='" + sCodPuxada + "' AND TELEFONE='" + DadosUsuario.TELEFONE.ToString().Trim() + "')";
                  oBancoDadosGerenciador.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCodPuxada, Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Nome", Valor = DadosUsuario.USUARIO.Trim(), Tipo = DbType.String },
                                                                                             /////new clsCampo { Nome = "Senha", Valor = DadosUsuario.SENHA.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Departamento", Valor = DadosUsuario.DEPARTAMENTO.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Email", Valor = DadosUsuario.EMAIL.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Ativo", Valor = Convert.ToInt32(DadosUsuario.ATIVO.Trim()), Tipo = DbType.Int32 },
                                                                                             new clsCampo { Nome = "CNPJ", Valor = DadosUsuario.CNPJ.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "Cod_Empregado", Valor = DadosUsuario.EMPREGADO.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "FLEXXPOWER", Valor = DadosUsuario.FLEXXPOWER.Trim().ToUpper().Substring(0, 1), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "LICENCA", Valor = DadosUsuario.LICENCA.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "SOFIA", Valor = DadosUsuario.SOFIA.Trim().ToUpper().Substring(0, 1), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "ID_USUARIO", Valor = DadosUsuario.ID_USUARIO, Tipo = DbType.Int32 },
                                                                                             new clsCampo { Nome = "TELEFONE", Valor = DadosUsuario.TELEFONE.Trim(), Tipo = DbType.String },
                                                                                             new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                             new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
                }

                sSql = "SELECT idUsuario FROM tb_Usuario" +
                        " WHERE (COD_PUXADA='" + sCodPuxada + "' AND TELEFONE='" + DadosUsuario.TELEFONE.ToString().Trim() + "')";
                iIdUsuario = Convert.ToInt32(oBancoDadosGerenciador.DBQuery_ValorUnico(sSql));

                if (DadosUsuario.ATIVO.Trim() == "1") { ServicoUsuarioAtivo = "S"; } else { ServicoUsuarioAtivo = "N"; };

                //1 Sofia
                //2 Flexxpower
                if (DadosUsuario.SOFIA.Trim().ToUpper().Substring(0, 1) == "S")
                {
                  sSql = "select count(*) from tb_Usuario_Servico" +
                         " where idUsuario = " + iIdUsuario.ToString() +
                           " and idEmpresa = " + iIdEmpresa.ToString() +
                           " and idProduto = 1" +
                           " and idServico = " + iIdServico.ToString();
                  if (Convert.ToInt32(oBancoDadosGerenciador.DBQuery_ValorUnico(sSql)) == 0)
                  {
                    Processar_Sincronizacao(sAplicativo, sPartner, "ISV", sCOD_PUXADA, DadosUsuario.USUARIO.Trim(), UltimoErro, "IFP",
                                            Integrador_Funcoes.TempoExecucaoAPI.ToString(),
                                            Integrador_Funcoes.TempoExecucaoIntegrador.ToString(), "", "", sConexao, sCdServico);

                    sSql = "insert into tb_Usuario_Servico (idUsuario,idEmpresa,idProduto,idServico,Login,SenhaAPP,ServicoUsuarioAtivo,Departamento,TipoUsuario)" +
                           " values (#idUsuario,#idEmpresa,#idProduto,#idServico,#Login,#SenhaAPP,#ServicoUsuarioAtivo,#Departamento,#TipoUsuario)";

                    oBancoDadosGerenciador.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "idUsuario", Valor = iIdUsuario, Tipo = DbType.Int32 },
                                                                                                 new clsCampo { Nome = "idEmpresa", Valor = iIdEmpresa, Tipo = DbType.Int32 },
                                                                                                 new clsCampo { Nome = "idProduto", Valor = 1, Tipo = DbType.Int32 },
                                                                                                 new clsCampo { Nome = "idServico", Valor = iIdServico, Tipo = DbType.Int32 },
                                                                                                 new clsCampo { Nome = "Login", Valor = DadosUsuario.USUARIO.Trim(), Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "SenhaAPP", Valor = DadosUsuario.SENHA.Trim(), Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "ServicoUsuarioAtivo", Valor = ServicoUsuarioAtivo, Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "Departamento", Valor = DadosUsuario.DEPARTAMENTO.Trim(), Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "TipoUsuario", Valor = "US", Tipo = DbType.String }});
                  }
                }

                if (DadosUsuario.FLEXXPOWER.Trim().ToUpper().Substring(0, 1) == "S")
                {
                  sSql = "select count(*) from tb_Usuario_Servico" +
                         " where idUsuario = " + iIdUsuario.ToString() +
                           " and idEmpresa = " + iIdEmpresa.ToString() +
                           " and idProduto = 2" +
                           " and idServico = " + iIdServico.ToString();
                  if (Convert.ToInt32(oBancoDadosGerenciador.DBQuery_ValorUnico(sSql)) == 0)
                  {
                    sSql = "insert into tb_Usuario_Servico (idUsuario,idEmpresa,idProduto,idServico,Login,SenhaAPP,ServicoUsuarioAtivo,Departamento,TipoUsuario)" +
                           " values (#idUsuario,#idEmpresa,#idProduto,#idServico,#Login,#SenhaAPP,#ServicoUsuarioAtivo,#Departamento,#TipoUsuario)";

                    oBancoDadosGerenciador.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "idUsuario", Valor = iIdUsuario, Tipo = DbType.Int32 },
                                                                                                 new clsCampo { Nome = "idEmpresa", Valor = iIdEmpresa, Tipo = DbType.Int32 },
                                                                                                 new clsCampo { Nome = "idProduto", Valor = 2, Tipo = DbType.Int32 },
                                                                                                 new clsCampo { Nome = "idServico", Valor = iIdServico, Tipo = DbType.Int32 },
                                                                                                 new clsCampo { Nome = "Login", Valor = DadosUsuario.USUARIO.Trim(), Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "SenhaAPP", Valor = DadosUsuario.SENHA.Trim(), Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "ServicoUsuarioAtivo", Valor = ServicoUsuarioAtivo, Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "Departamento", Valor = DadosUsuario.DEPARTAMENTO.Trim(), Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "TipoUsuario", Valor = "US", Tipo = DbType.String }});
                  }
                }

                if (DadosUsuario.FLEXXPOWER.Trim().ToUpper().Substring(0, 1) == "S")
                {
                  if (DadosUsuario.EMPREGADO.Trim() == "")
                  {
                    sNome = Convert.ToInt32(DadosUsuario.EMPREGADO.Trim()).ToString();
                  }

                  if (DadosUsuario.DEPARTAMENTO.Trim().ToUpper() == "SUPERVISOR")
                  {
                    sDepartamento = "S" + FNC_Right("000" + DadosUsuario.EMPREGADO.Trim(), 3);
                    sNome = DadosUsuario.USUARIO.Trim() + " (" + sNome + ")";
                  }
                  else if (DadosUsuario.DEPARTAMENTO.Trim().ToUpper() == "VENDEDOR")
                  {
                    sDepartamento = "V" + FNC_Right("000" + DadosUsuario.EMPREGADO.Trim(), 3);
                    sNome = DadosUsuario.USUARIO.Trim() + " (" + sNome + ")";
                  }
                  else
                  {
                    sDepartamento = DadosUsuario.DEPARTAMENTO.Trim();
                    sNome = DadosUsuario.USUARIO.Trim();
                  }

                  sSql = "SELECT COUNT(*) FROM tb_Usuario" +
                         " WHERE (Cod_Puxada='" + sCodPuxada + "' AND TELEFONE='" + DadosUsuario.TELEFONE.ToString().Trim() + "')";
                  if (Convert.ToInt32(oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                  {
                    bNovoUsuario = true;

                    sSql = "INSERT INTO tb_Usuario (Cod_Puxada,Nome,Senha,Departamento,Email,Ativo" +
                                                  ",CNPJ,Cod_Empregado,FLEXXPOWER,LICENCA,SOFIA,ID_USUARIO,TELEFONE,DTINTEGRACAO,VERSAO_INTEGRADOR)" +
                           " VALUES(#Cod_Puxada,#Nome,#Senha,#Departamento,#Email,#Ativo" +
                                  ",#CNPJ,#Cod_Empregado,#FLEXXPOWER,#LICENCA,#SOFIA,#ID_USUARIO,#TELEFONE,#DTINTEGRACAO,#VERSAO_INTEGRADOR)";
                    oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCodPuxada, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Nome", Valor = DadosUsuario.USUARIO.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Senha", Valor = DadosUsuario.SENHA.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Departamento", Valor = sDepartamento, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Email", Valor = DadosUsuario.EMAIL.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Ativo", Valor = Convert.ToInt32(DadosUsuario.ATIVO.Trim()), Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "CNPJ", Valor = DadosUsuario.CNPJ.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Cod_Empregado", Valor = DadosUsuario.EMPREGADO.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "FLEXXPOWER", Valor = DadosUsuario.FLEXXPOWER.Trim().ToUpper().Substring(0, 1), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "LICENCA", Valor = DadosUsuario.LICENCA.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "SOFIA", Valor = DadosUsuario.SOFIA.Trim().ToUpper().Substring(0, 1), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "ID_USUARIO", Valor = DadosUsuario.ID_USUARIO, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "TELEFONE", Valor = DadosUsuario.TELEFONE.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                      new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
                  }
                  else
                  {
                    sSql = "UPDATE tb_Usuario" +
                           " SET Cod_Puxada=#Cod_Puxada," +
                                "Nome=#Nome," +
                                "Senha=#Senha," +
                                "Departamento=#Departamento," +
                                "Email=#Email," +
                                "Ativo=#Ativo," +
                                "CNPJ=#CNPJ," +
                                "Cod_Empregado=#Cod_Empregado," +
                                "FLEXXPOWER=#FLEXXPOWER," +
                                "LICENCA=#LICENCA," +
                                "SOFIA=#SOFIA," +
                                "ID_USUARIO=#ID_USUARIO," +
                                "TELEFONE=#TELEFONE," +
                                "DTINTEGRACAO=#DTINTEGRACAO," +
                                "VERSAO_INTEGRADOR=#VERSAO_INTEGRADOR" +
                    " WHERE (COD_PUXADA='" + sCodPuxada + "' AND TELEFONE='" + DadosUsuario.TELEFONE.ToString().Trim() + "')";
                    oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCodPuxada, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Nome", Valor = DadosUsuario.USUARIO.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Senha", Valor = DadosUsuario.SENHA.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Departamento", Valor = sDepartamento, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Email", Valor = DadosUsuario.EMAIL.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Ativo", Valor = Convert.ToInt32(DadosUsuario.ATIVO.Trim()), Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "CNPJ", Valor = DadosUsuario.CNPJ.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "Cod_Empregado", Valor = DadosUsuario.EMPREGADO.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "FLEXXPOWER", Valor = DadosUsuario.FLEXXPOWER.Trim().ToUpper().Substring(0, 1), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "LICENCA", Valor = DadosUsuario.LICENCA.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "SOFIA", Valor = DadosUsuario.SOFIA.Trim().ToUpper().Substring(0, 1), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "ID_USUARIO", Valor = DadosUsuario.ID_USUARIO, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "TELEFONE", Valor = DadosUsuario.TELEFONE.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                      new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
                  }
                }
              }

              try
              {
                bNovoUsuario = false;
                if (bNovaEmpresa)
                {
                  sMensagem = "A empresa " + DadosUsuario.EMPRESA.Trim() + " (" + sCodPuxada + ") foi adicionada";
                  Integradores.Bot.Webook_Util(1, DadosUsuario.LICENCA, "", sMensagem, "ZP", sNotificacao_NovaEmpresa_Nome, sNotificacao_NovaEmpresa_Telefone, "Sofia", "");
                }
                if ((bNovoUsuario) && (DadosUsuario.SOFIA.Trim().ToUpper().Substring(0, 1) == "S"))
                {
                  sMensagem = "O usuário " + DadosUsuario.USUARIO.Trim() + " (" + DadosUsuario.TELEFONE + ") foi adicionado";
                  Integradores.Bot.Webook_Util(1, DadosUsuario.LICENCA, "", sMensagem, "ZP", sNotificacao_NovoUsuario_Nome, sNotificacao_NovoUsuario_Telefone, "Sofia", "");
                }
                if (bSemAcesso)
                {
                  sMensagem = "O usuário " + DadosUsuario.USUARIO.Trim() + " (" + DadosUsuario.TELEFONE + ") está sem acesso a base da Flag";
                  Integradores.Bot.Webook_Util(1, DadosUsuario.LICENCA, "", sMensagem, "ZP", sNotificacao_UsuarioSemAcesso_Nome, sNotificacao_UsuarioSemAcesso_Telefone, "Sofia", "");
                }
              }
              catch (Exception)
              {
              }
            }

            Processar_Sincronizacao(sAplicativo, sPartner, "FIN", sCOD_PUXADA, "Totalização - " + DadosUsuario_Root.DadosUsuario.Count.ToString(), UltimoErro, "IFP",
                                    Integrador_Funcoes.TempoExecucaoAPI.ToString(),
                                    Integrador_Funcoes.TempoExecucaoIntegrador.ToString(), "", "", sConexao, sCdServico);

            DadosUsuario_Root = null;
            oData.Dispose();

            oBancoDados.DBDesconectar();
            oBancoDadosGerenciador.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, "", "", sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DisUsuario [" + sCOD_PUXADA + "] > " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    private static Boolean FlagWZ_DisDevolucao(string sAplicativo,
                                               string sPartner,
                                               string sCdServico,
                                               int iIdEmpresa,
                                               int iIdTarefa,
                                               string sConexao,
                                               string sUsuario,
                                               string sSenha,
                                               string sCOD_PUXADA,
                                               string sTIPO_REGISTRO,
                                               string sTEL_CELULAR,
                                               string sTIPO_CONSULTA,
                                               string sVISAO_FATURAMENTO,
                                               string sDS_STRINGCONEXAODESTINO,
                                               string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      string sStatus = "";
      string sErro = "";
      double TempoExecucaoAPI = 0;
      double TempoExecucaoIntegrador = 0;
      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      UltimoLocalErro = "FlagWZ_DisDevolucao";

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;
      UltimoLocalErro = UltimoLocalErro + " - WebRequest.Create - " + sConexao;
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DisDevolucao_Root DisDevolucao_Root;
        string sSql;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                       "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";

        request.ContentLength = json.Length;

        UltimoLocalErro = UltimoLocalErro + " - StreamWriter(request.GetRequestStream())";
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
              DisDevolucao_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DisDevolucao_Root>(content);

              if (DisDevolucao_Root.codigo == 0)
              {
                sStatus = "Executado";
              }
              else
              {
                sStatus = "ERRO";
              }

              sErro = DisDevolucao_Root.codigo.ToString();

              ts = DateTime.Now - dUtil;
              TempoExecucaoAPI = ts.TotalSeconds;
            }
          }
        }

        if (DisDevolucao_Root != null)
        {
          if (DisDevolucao_Root.DadosDISTDevolucao.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            foreach (FlagWSWhatsapp_DisDevolucao_Dados DisDevolucao_Dados in DisDevolucao_Root.DadosDISTDevolucao)
            {
              sSql = "DELETE FROM dadosdistdevolucao" +
                     " WHERE COD_PUXADA=#COD_PUXADA" +
                       " AND CODIGO_GERENTE=#CODIGO_GERENTE" +
                       " AND CODIGO_SUPERVISOR=#CODIGO_SUPERVISOR" +
                       " AND CODIGO_VENDEDOR=#CODIGO_VENDEDOR";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_GERENTE", Valor = DisDevolucao_Dados.CODIGO_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = DisDevolucao_Dados.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = DisDevolucao_Dados.CODIGO_VENDEDOR, Tipo = DbType.String },});

              sSql = "INSERT INTO dadosdistdevolucao (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
                                                     "CODIGO_GERENTE, CODIGO_SUPERVISOR, CODIGO_VENDEDOR, NOME_GERENTE, NOME_SUPERVISOR," +
                                                     "NOME_VENDEDOR, PERCENTUAL_DEVOLUCAO, VALOR_BRUTO_DEVOLUCAO, VALOR_BRUTO_VENDA," +
                                                     "DTINTEGRACAO, VERSAO_INTEGRADOR)" +
                     " VALUES (#COD_PUXADA, #TIPO_REGISTRO, #TEL_CELULAR, #TIPO_CONSULTA, #VISAO_FATURAMENTO," +
                              "#CODIGO_GERENTE, #CODIGO_SUPERVISOR, #CODIGO_VENDEDOR, #NOME_GERENTE, #NOME_SUPERVISOR," +
                              "#NOME_VENDEDOR, #PERCENTUAL_DEVOLUCAO, #VALOR_BRUTO_DEVOLUCAO, #VALOR_BRUTO_VENDA," +
                              "#DTINTEGRACAO, #VERSAO_INTEGRADOR)";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TIPO_REGISTRO", Valor = sTIPO_REGISTRO, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "TEL_CELULAR", Valor = sTEL_CELULAR, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "TIPO_CONSULTA", Valor = sTIPO_CONSULTA, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "VISAO_FATURAMENTO", Valor = sVISAO_FATURAMENTO, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "CODIGO_GERENTE", Valor = DisDevolucao_Dados.CODIGO_GERENTE, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = DisDevolucao_Dados.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = DisDevolucao_Dados.CODIGO_VENDEDOR, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "NOME_GERENTE", Valor = DisDevolucao_Dados.NOME_GERENTE, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "NOME_SUPERVISOR", Valor = DisDevolucao_Dados.NOME_SUPERVISOR, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "NOME_VENDEDOR", Valor = DisDevolucao_Dados.NOME_VENDEDOR, Tipo = DbType.String },
                                                                        new clsCampo { Nome = "PERCENTUAL_DEVOLUCAO", Valor = DisDevolucao_Dados.PERCENTUAL_DEVOLUCAO, Tipo = DbType.Decimal },
                                                                        new clsCampo { Nome = "VALOR_BRUTO_DEVOLUCAO", Valor = DisDevolucao_Dados.VALOR_BRUTO_DEVOLUCAO, Tipo = DbType.Decimal },
                                                                        new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = DisDevolucao_Dados.VALOR_BRUTO_VENDA, Tipo = DbType.Decimal },
                                                                        new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                        new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
            }

            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        UltimoLocalErro = UltimoLocalErro + " - Erro: " + Ex.Message;
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DisDevolucao [" + sCOD_PUXADA + "] >> " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }
    private static Boolean FlagWZ_DocumentosaVencer(int idEmpresaIntegracao,
                                                    string sAplicativo,
                                                    string sPartner,
                                                    string sCdServico,
                                                    int iIdEmpresa,
                                                    int iIdTarefa,
                                                    string sConexao,
                                                    string sUsuario,
                                                    string sSenha,
                                                    string sCOD_PUXADA,
                                                    string sTIPO_REGISTRO,
                                                    string sTEL_CELULAR,
                                                    string sTIPO_CONSULTA,
                                                    string sDS_STRINGCONEXAODESTINO,
                                                    string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      string sErro = "";
      double TempoExecucaoAPI = 0;
      double TempoExecucaoIntegrador = 0;
      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      UltimoLocalErro = "FlagWZ_DocumentosaVencer";

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;
      UltimoLocalErro = UltimoLocalErro + " - WebRequest.Create - " + sConexao;
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DISTSfDocumentosaVencerRoot DISTSfDocumentosaVencerRoot;
        DataTable oData;
        string sSql;

        string sBot;
        string sNOTIFICACAO;
        string sTELEFONE;
        string sMensagem;

        sSql = "select bt.Apelido" +
               " from tb_empresas_integracao ei" +
                " inner join tb_tipo_integracao ti on ti.id_Tipo_Integracao = ei.idTipoIntegracao" +
                " inner join tb_produto pd on pd.id_Produto = ti.id_Produto" +
                " inner join tb_bot bt on bt.idBot = pd.id_bot" +
               " where ei.idEmpresaIntegracao = " + idEmpresaIntegracao.ToString();
        sBot = oBancoDados.DBQuery_ValorUnico(sSql).ToString();

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"TEL_CELULAR\":\"" + sTEL_CELULAR + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        UltimoLocalErro = UltimoLocalErro + " - StreamWriter(request.GetRequestStream())";
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
              DISTSfDocumentosaVencerRoot = JsonConvert.DeserializeObject<FlagWSWhatsapp_DISTSfDocumentosaVencerRoot>(content);

              if (DISTSfDocumentosaVencerRoot.codigo == 0)
              {
                sStatus = "Executado";
              }
              else
              {
                sStatus = "ERRO";
              }

              sErro = DISTSfDocumentosaVencerRoot.codigo.ToString();

              ts = DateTime.Now - dUtil;
              TempoExecucaoAPI = ts.TotalSeconds;
            }
          }
        }

        if (DISTSfDocumentosaVencerRoot != null)
        {
          if (DISTSfDocumentosaVencerRoot.DadosDISTSfDocumentosaVencer.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            foreach (FlagWSWhatsapp_DISTSfDocumentosaVencer DadosDISTSfDocumentosaVencer in DISTSfDocumentosaVencerRoot.DadosDISTSfDocumentosaVencer)
            {
              if (DadosDISTSfDocumentosaVencer.COD_PUXADA.Trim() == sCOD_PUXADA.Trim())
              {
                sTELEFONE = DadosDISTSfDocumentosaVencer.DDD_CELULAR.ToString().Trim() + DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim();
                sTELEFONE = _Funcoes.FNC_FormatarTelefone(sTELEFONE);

                if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "998453026") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "997419458") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "981556795") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "32731095") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "997510906") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "996099125") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "96531374") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "998147266") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "998181800") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "998374747") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "997089777") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "996686501") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "32771260") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "998172780") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "91359976") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "32593713") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "991621954") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "997786307") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "998473165") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "32593302") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "997761512") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "981298048") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "998122274") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "998414698") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "998117444") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosaVencer.NUMERO_CELULAR.ToString().Trim() == "996040614") { sTELEFONE = "5531984847936"; }
                else { sTELEFONE = "5531984698701"; }

                sSql = "SELECT * FROM TB_CLIENTE WHERE TELEFONE = '" + sTELEFONE.Trim() + "' AND COD_PUXADA = '" + DadosDISTSfDocumentosaVencer.COD_PUXADA.Trim() + "'";
                oData = oBancoDados.DBQuery(sSql);

                if (oData.Rows.Count == 0)
                {
                  sSql = "INSERT INTO TB_CLIENTE (NOME, TELEFONE, COD_PUXADA) VALUES (#NOME, #TELEFONE, #COD_PUXADA)";
                  oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "NOME", Valor = DadosDISTSfDocumentosaVencer.NOME_FANTASIA, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "TELEFONE", Valor = sTELEFONE, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "COD_PUXADA", Valor = DadosDISTSfDocumentosaVencer.COD_PUXADA, Tipo = DbType.String }});

                  Bot.Webook_Util(1, "COB", Constantes.const_TratamentoSofia_CONFIRMARNOTIFICACAO, "", "ZP",
                                            DadosDISTSfDocumentosaVencer.NOME_FANTASIA,
                                            sTELEFONE, sBot,
                                            DadosDISTSfDocumentosaVencer.COD_PUXADA);
                }
                else
                {
                  if (oData.Rows[0]["PODE_NOTIFICAR"].ToString() == "S")
                  {
                    sNOTIFICACAO = "CODIGO_CLIENTE=" + DadosDISTSfDocumentosaVencer.CODIGO_CLIENTE.ToString() + "-" +
                                   "COD_PUXADA=" + DadosDISTSfDocumentosaVencer.COD_PUXADA.ToString() + "-" +
                                   "TELEFONE=" + sTELEFONE + "-" +
                                   "DOCUMENTO=" + DadosDISTSfDocumentosaVencer.DOCUMENTO.ToString() + "-" +
                                   "DOCUMENTO_ORIGEM=" + DadosDISTSfDocumentosaVencer.DOCUMENTO_ORIGEM.ToString();

                    sSql = "SELECT COUNT(*) FROM TB_CLIENTE_NOTIFICACAO" +
                           " WHERE TELEFONE = '" + sTELEFONE + "' AND NOTIFICACAO = '" + sNOTIFICACAO + "' AND COD_PUXADA = '" + DadosDISTSfDocumentosaVencer.COD_PUXADA.Trim() + "'";

                    if (Convert.ToInt32(oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                    {
                      sMensagem = "Ola! " + DadosDISTSfDocumentosaVencer.NOME_FANTASIA + Environment.NewLine +
                                  "Sou " + sBot + Environment.NewLine + Environment.NewLine +
                                  "Passando pra lembrar que dia " + DadosDISTSfDocumentosaVencer.DATA_VENCIMENTO.Substring(0, 10) +
                                  " ira vencer seu boleto " + DadosDISTSfDocumentosaVencer.DOCUMENTO +
                                  " no valor de " + string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", DadosDISTSfDocumentosaVencer.VALOR_DOCUMENTO) + Environment.NewLine + Environment.NewLine +
                                  "Obrigado";

                      sSql = "INSERT INTO TB_CLIENTE_NOTIFICACAO (TELEFONE, NOTIFICACAO, COD_PUXADA) VALUES ('" + sTELEFONE + "', '" + sNOTIFICACAO + "', '" + DadosDISTSfDocumentosaVencer.COD_PUXADA + "')";
                      oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "TELEFONE", Valor = sTELEFONE, Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "NOTIFICACAO", Valor = sNOTIFICACAO, Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "COD_PUXADA", Valor = DadosDISTSfDocumentosaVencer.COD_PUXADA, Tipo = DbType.String }});

                      Bot.Webook_Util(1, sCdServico.ToUpper(), "",
                                         sMensagem, "ZP",
                                         DadosDISTSfDocumentosaVencer.NOME_FANTASIA,
                                         sTELEFONE, sBot,
                                         DadosDISTSfDocumentosaVencer.COD_PUXADA);
                    }
                  }
                }

                oData.Dispose();
              }
            }

            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        UltimoLocalErro = UltimoLocalErro + " - Erro: " + Ex.Message;
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DocumentosaVencer [" + sCOD_PUXADA + "] >> " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }
    private static Boolean FlagWZ_DisSfDocumentosVencidos(int idEmpresaIntegracao,
                                                            string sAplicativo,
                                                            string sPartner,
                                                            string sCdServico,
                                                            int iIdEmpresa,
                                                            int iIdTarefa,
                                                            string sConexao,
                                                            string sUsuario,
                                                            string sSenha,
                                                            string sCOD_PUXADA,
                                                            string sTIPO_REGISTRO,
                                                            string sTEL_CELULAR,
                                                            string sTIPO_CONSULTA,
                                                            string sDS_STRINGCONEXAODESTINO,
                                                            string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      string sErro = "";
      double TempoExecucaoAPI = 0;
      double TempoExecucaoIntegrador = 0;
      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      UltimoLocalErro = "FlagWZ_DocumentosaVencer";

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;
      UltimoLocalErro = UltimoLocalErro + " - WebRequest.Create - " + sConexao;
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DISTSfDocumentosVencidoRoot DISTSfDocumentosVencidoRoot;
        DataTable oData;
        string sSql;

        string sBot;
        string sNOTIFICACAO;
        string sTELEFONE;
        string sMensagem;
        Boolean bPodeEnviar;

        sSql = "select bt.Apelido" +
               " from tb_empresas_integracao ei" +
                " inner join tb_tipo_integracao ti on ti.id_Tipo_Integracao = ei.idTipoIntegracao" +
                " inner join tb_produto pd on pd.id_Produto = ti.id_Produto" +
                " inner join tb_bot bt on bt.idBot = pd.id_bot" +
               " where ei.idEmpresaIntegracao = " + idEmpresaIntegracao.ToString();
        sBot = oBancoDados.DBQuery_ValorUnico(sSql).ToString();

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"TEL_CELULAR\":\"" + sTEL_CELULAR + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        UltimoLocalErro = UltimoLocalErro + " - StreamWriter(request.GetRequestStream())";
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
              DISTSfDocumentosVencidoRoot = JsonConvert.DeserializeObject<FlagWSWhatsapp_DISTSfDocumentosVencidoRoot>(content);

              if (DISTSfDocumentosVencidoRoot.codigo == 0)
              {
                sStatus = "Executado";
              }
              else
              {
                sStatus = "ERRO";
              }

              sErro = DISTSfDocumentosVencidoRoot.codigo.ToString();

              ts = DateTime.Now - dUtil;
              TempoExecucaoAPI = ts.TotalSeconds;
            }
          }
        }

        if (DISTSfDocumentosVencidoRoot != null)
        {
          if (DISTSfDocumentosVencidoRoot.DadosDISTSfDocumentosVencido.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            foreach (FlagWSWhatsapp_DISTSfDocumentosVencido DadosDISTSfDocumentosVencido in DISTSfDocumentosVencidoRoot.DadosDISTSfDocumentosVencido)
            {
              if (DadosDISTSfDocumentosVencido.COD_PUXADA.Trim() == sCOD_PUXADA.Trim())
              {
                sTELEFONE = DadosDISTSfDocumentosVencido.DDD_CELULAR.ToString().Trim() + DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim();
                sTELEFONE = _Funcoes.FNC_FormatarTelefone(sTELEFONE);

                if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "998453026") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "997419458") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "981556795") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "32731095") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "997510906") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "996099125") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "96531374") { sTELEFONE = "5531998015606"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "998147266") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "998181800") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "998374747") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "997089777") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "996686501") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "32771260") { sTELEFONE = "5537991945139"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "998172780") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "91359976") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "32593713") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "991621954") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "997786307") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "998473165") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "32593302") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "997761512") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "981298048") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "998122274") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "998414698") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "998117444") { sTELEFONE = "5531984847936"; }
                else if (DadosDISTSfDocumentosVencido.NUMERO_CELULAR.ToString().Trim() == "996040614") { sTELEFONE = "5531984847936"; }
                else { sTELEFONE = "5531984698701"; }

                sSql = "SELECT * FROM TB_CLIENTE WHERE TELEFONE = '" + sTELEFONE.Trim() + "' AND COD_PUXADA = '" + DadosDISTSfDocumentosVencido.COD_PUXADA.Trim() + "'";
                oData = oBancoDados.DBQuery(sSql);

                if (oData.Rows.Count == 0)
                {
                  sSql = "INSERT INTO TB_CLIENTE (NOME, TELEFONE, COD_PUXADA) VALUES (#NOME, #TELEFONE, #COD_PUXADA)";
                  oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "NOME", Valor = DadosDISTSfDocumentosVencido.NOME_FANTASIA, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "TELEFONE", Valor = sTELEFONE, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "COD_PUXADA", Valor = DadosDISTSfDocumentosVencido.COD_PUXADA, Tipo = DbType.String }});

                  Bot.Webook_Util(1, sCdServico.ToUpper(),
                                     Constantes.const_TratamentoSofia_CONFIRMARNOTIFICACAO, "", "ZP",
                                     DadosDISTSfDocumentosVencido.NOME_FANTASIA,
                                     sTELEFONE, sBot,
                                     DadosDISTSfDocumentosVencido.COD_PUXADA);
                }
                else
                {
                  if (oData.Rows[0]["PODE_NOTIFICAR"].ToString() == "S")
                  {
                    sNOTIFICACAO = "CODIGO_CLIENTE=" + DadosDISTSfDocumentosVencido.CODIGO_CLIENTE.ToString() + "-" +
                                   "COD_PUXADA=" + DadosDISTSfDocumentosVencido.COD_PUXADA.ToString() + "-" +
                                   "TELEFONE=" + sTELEFONE + "-" +
                                   "DOCUMENTO=" + DadosDISTSfDocumentosVencido.DOCUMENTO.ToString() + "-" +
                                   "DOCUMENTO_ORIGEM=" + DadosDISTSfDocumentosVencido.DOCUMENTO_ORIGEM.ToString();

                    bPodeEnviar = false;

                    sSql = "SELECT * FROM TB_CLIENTE_NOTIFICACAO WHERE TELEFONE = '" + sTELEFONE + "' AND NOTIFICACAO = '" + sNOTIFICACAO + "'";
                    oData = oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count == 0)
                    {
                      bPodeEnviar = true;
                    }
                    else
                    {
                      if (oData.Rows.Count == 1)
                      {
                        if (Convert.ToDateTime(oData.Rows[0]["DATA"]).AddDays(2).Date <= DateTime.Now.AddDays(2).Date)
                        {
                          bPodeEnviar = true;
                        }
                      }
                    }

                    if (Convert.ToInt32(oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                    {
                      sMensagem = "Ola! " + DadosDISTSfDocumentosVencido.NOME_FANTASIA + Environment.NewLine +
                                  "Sou " + sBot + Environment.NewLine + Environment.NewLine +
                                  "Passando pra lembrar que dia " + DadosDISTSfDocumentosVencido.DATA_VENCIMENTO.PadLeft(10) +
                                  " venceu seu boleto " + DadosDISTSfDocumentosVencido.DOCUMENTO +
                                  " no valor de " + string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", DadosDISTSfDocumentosVencido.VALOR_DOCUMENTO) + Environment.NewLine + Environment.NewLine +
                                  "Obrigado";

                      sSql = "INSERT INTO TB_CLIENTE_NOTIFICACAO (TELEFONE, NOTIFICACAO) VALUES ('" + sTELEFONE + "', '" + sNOTIFICACAO + "')";
                      oBancoDados.DBExecutar(sSql);

                      Bot.Webook_Util(1, sCdServico.ToUpper(), "",
                                         sMensagem, "ZP",
                                         DadosDISTSfDocumentosVencido.NOME_FANTASIA,
                                         sTELEFONE, sBot,
                                         DadosDISTSfDocumentosVencido.COD_PUXADA);
                    }
                  }
                }

                oData.Dispose();
              }
            }

            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        UltimoLocalErro = UltimoLocalErro + " - Erro: " + Ex.Message;
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DocumentosaVencer [" + sCOD_PUXADA + "] >> " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    private static Boolean FlagWZ_DisTroca(string sAplicativo,
                                           string sPartner,
                                           string sCdServico,
                                           int iIdEmpresa,
                                           int iIdTarefa,
                                           string sConexao,
                                           string sUsuario,
                                           string sSenha,
                                           string sCOD_PUXADA,
                                           string sTIPO_REGISTRO,
                                           string sTEL_CELULAR,
                                           string sTIPO_CONSULTA,
                                           string sVISAO_FATURAMENTO,
                                           string sDS_STRINGCONEXAODESTINO,
                                           string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      Integrador_Funcoes.sStatus = "";
      string sErro = "";
      double TempoExecucaoAPI = 0;
      double TempoExecucaoIntegrador = 0;
      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DadosDISTTroca_Root DisTroca_Root;
        string sSql;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                       "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";

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
              DisTroca_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTTroca_Root>(content);
            }

            if (DisTroca_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = DisTroca_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        if (DisTroca_Root != null)
        {
          if (DisTroca_Root.DadosDISTTroca.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            foreach (FlagWSWhatsapp_DadosDISTTroca DisTroca_Dados in DisTroca_Root.DadosDISTTroca)
            {
              sSql = "DELETE FROM DadosDISTTroca" +
                     " WHERE COD_PUXADA=#COD_PUXADA" +
                       " AND CODIGO_GERENTE=#CODIGO_GERENTE" +
                       " AND CODIGO_SUPERVISOR=#CODIGO_SUPERVISOR" +
                       " AND CODIGO_VENDEDOR=#CODIGO_VENDEDOR";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_GERENTE", Valor = DisTroca_Dados.CODIGO_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = DisTroca_Dados.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = DisTroca_Dados.CODIGO_VENDEDOR, Tipo = DbType.String },});

              sSql = "INSERT INTO DadosDISTTroca (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
                                                 "CODIGO_GERENTE, CODIGO_SUPERVISOR, CODIGO_VENDEDOR, NOME_GERENTE, NOME_SUPERVISOR," +
                                                 "NOME_VENDEDOR, PERCENTUAL_TROCA, VALOR_BRUTO_TROCA, VALOR_BRUTO_VENDA," +
                                                 "DTINTEGRACAO, VERSAO_INTEGRADOR)" +
                     " VALUES (#COD_PUXADA, #TIPO_REGISTRO, #TEL_CELULAR, #TIPO_CONSULTA, #VISAO_FATURAMENTO," +
                              "#CODIGO_GERENTE, #CODIGO_SUPERVISOR, #CODIGO_VENDEDOR, #NOME_GERENTE, #NOME_SUPERVISOR," +
                              "#NOME_VENDEDOR, #PERCENTUAL_TROCA, #VALOR_BRUTO_TROCA, #VALOR_BRUTO_VENDA," +
                              "#DTINTEGRACAO, #VERSAO_INTEGRADOR)";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TIPO_REGISTRO", Valor = sTIPO_REGISTRO, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TEL_CELULAR", Valor = sTEL_CELULAR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TIPO_CONSULTA", Valor = sTIPO_CONSULTA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "VISAO_FATURAMENTO", Valor = sVISAO_FATURAMENTO, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_GERENTE", Valor = DisTroca_Dados.CODIGO_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = DisTroca_Dados.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = DisTroca_Dados.CODIGO_VENDEDOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_GERENTE", Valor = DisTroca_Dados.NOME_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_SUPERVISOR", Valor = DisTroca_Dados.NOME_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_VENDEDOR", Valor = DisTroca_Dados.NOME_VENDEDOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "PERCENTUAL_TROCA", Valor = DisTroca_Dados.PERCENTUAL_TROCA, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "VALOR_BRUTO_TROCA", Valor = DisTroca_Dados.VALOR_BRUTO_TROCA, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = DisTroca_Dados.VALOR_BRUTO_VENDA, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                          new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
            }

            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DisTroca [" + sCOD_PUXADA + "] >> " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    private static Boolean FlagWZ_DisIav_iv(string sAplicativo,
                                            string sPartner,
                                            string sCdServico,
                                            int iIdEmpresa,
                                            int iIdTarefa,
                                            string sConexao,
                                            string sUsuario,
                                            string sSenha,
                                            string sCOD_PUXADA,
                                            string sTIPO_REGISTRO,
                                            string sTEL_CELULAR,
                                            string sTIPO_CONSULTA,
                                            string sVISAO_FATURAMENTO,
                                            string sDS_STRINGCONEXAODESTINO,
                                            string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      UltimoLocalErro = "FlagWZ_DisIav_iv";

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;
      UltimoLocalErro = UltimoLocalErro + " - WebRequest.Create - " + sConexao;
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DadosDISTIavIv_Root DisTIav_iv_Root;
        string sSql;

        UltimoLocalErro = UltimoLocalErro + " - POST";
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                       "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";

        request.ContentLength = json.Length;

        UltimoLocalErro = UltimoLocalErro + " - StreamWriter(request.GetRequestStream())";
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
              DisTIav_iv_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTIavIv_Root>(content);
            }

            if (DisTIav_iv_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = DisTIav_iv_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        if (DisTIav_iv_Root != null)
        {
          if (DisTIav_iv_Root.DadosDISTIav_iv.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            foreach (FlagWSWhatsapp_DadosDISTIavIv DisIav_iv_Dados in DisTIav_iv_Root.DadosDISTIav_iv)
            {
              sSql = "DELETE FROM DadosDISTIav_iv" +
                     " WHERE COD_PUXADA=#COD_PUXADA" +
                       " AND CODIGO_GERENTE=#CODIGO_GERENTE" +
                       " AND CODIGO_SUPERVISOR=#CODIGO_SUPERVISOR" +
                       " AND CODIGO_VENDEDOR=#CODIGO_VENDEDOR";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_GERENTE", Valor = DisIav_iv_Dados.CODIGO_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = DisIav_iv_Dados.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = DisIav_iv_Dados.CODIGO_VENDEDOR, Tipo = DbType.String },});

              sSql = "INSERT INTO DadosDISTIav_iv (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
                                                  "CODIGO_GERENTE, CODIGO_SUPERVISOR, CODIGO_VENDEDOR, NOME_GERENTE, NOME_SUPERVISOR," +
                                                  "NOME_VENDEDOR, PERCENTUAL_VISITA_REALIZADA, PERCENTUAL_VISITA_REALIZADA_VENDA, QTDE_VISITA_PREVISTA," +
                                                  "QTDE_VISITA_REALIZADA, QTDE_VISITA_REALIZADA_VENDA,DTINTEGRACAO, VERSAO_INTEGRADOR)" +
                     " VALUES (#COD_PUXADA, #TIPO_REGISTRO, #TEL_CELULAR, #TIPO_CONSULTA, #VISAO_FATURAMENTO," +
                              "#CODIGO_GERENTE, #CODIGO_SUPERVISOR, #CODIGO_VENDEDOR, #NOME_GERENTE, #NOME_SUPERVISOR," +
                              "#NOME_VENDEDOR, #PERCENTUAL_VISITA_REALIZADA, #PERCENTUAL_VISITA_REALIZADA_VENDA, #QTDE_VISITA_PREVISTA," +
                              "#QTDE_VISITA_REALIZADA,#QTDE_VISITA_REALIZADA_VENDA,#DTINTEGRACAO, #VERSAO_INTEGRADOR)";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TIPO_REGISTRO", Valor = sTIPO_REGISTRO, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TEL_CELULAR", Valor = sTEL_CELULAR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TIPO_CONSULTA", Valor = sTIPO_CONSULTA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "VISAO_FATURAMENTO", Valor = sVISAO_FATURAMENTO, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_GERENTE", Valor = DisIav_iv_Dados.CODIGO_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = DisIav_iv_Dados.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = DisIav_iv_Dados.CODIGO_VENDEDOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_GERENTE", Valor = DisIav_iv_Dados.NOME_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_SUPERVISOR", Valor = DisIav_iv_Dados.NOME_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_VENDEDOR", Valor = DisIav_iv_Dados.NOME_VENDEDOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "PERCENTUAL_VISITA_REALIZADA", Valor = DisIav_iv_Dados.PERCENTUAL_VISITA_REALIZADA, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "PERCENTUAL_VISITA_REALIZADA_VENDA", Valor = DisIav_iv_Dados.PERCENTUAL_VISITA_REALIZADA_VENDA, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "QTDE_VISITA_PREVISTA", Valor = Convert.ToDecimal(DisIav_iv_Dados.QTDE_VISITA_PREVISTA) / 100, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "QTDE_VISITA_REALIZADA", Valor = Convert.ToDecimal(DisIav_iv_Dados.QTDE_VISITA_REALIZADA) / 100, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "QTDE_VISITA_REALIZADA_VENDA", Valor = Convert.ToDecimal(DisIav_iv_Dados.QTDE_VISITA_REALIZADA_VENDA) / 100, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                          new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
            }

            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DisIav_iv [" + sCOD_PUXADA + "] > " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    private static Boolean FlagWZ_DisInadimplencia(string sAplicativo,
                                                   string sPartner,
                                                   string sCdServico,
                                                   int iIdEmpresa,
                                                   int iIdTarefa,
                                                   string sConexao,
                                                   string sUsuario,
                                                   string sSenha,
                                                   string sCOD_PUXADA,
                                                   string sTIPO_REGISTRO,
                                                   string sTEL_CELULAR,
                                                   string sTIPO_CONSULTA,
                                                   string sVISAO_FATURAMENTO,
                                                   string sDS_STRINGCONEXAODESTINO,
                                                   string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DadosDISTInadimplencia_Root DisInadimplencia_Root;
        string sSql;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                       "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";

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
              DisInadimplencia_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTInadimplencia_Root>(content);
            }

            if (DisInadimplencia_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = DisInadimplencia_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        if (DisInadimplencia_Root != null)
        {
          if (DisInadimplencia_Root.DadosDISTInadimplencia.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            foreach (FlagWSWhatsapp_DadosDISTInadimplencia DisInadimplencia in DisInadimplencia_Root.DadosDISTInadimplencia)
            {
              sSql = "DELETE FROM DadosDISTInadimplencia" +
                     " WHERE COD_PUXADA=#COD_PUXADA" +
                       " AND CODIGO_GERENTE=#CODIGO_GERENTE" +
                       " AND CODIGO_SUPERVISOR=#CODIGO_SUPERVISOR" +
                       " AND CODIGO_VENDEDOR=#CODIGO_VENDEDOR";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_GERENTE", Valor = DisInadimplencia.CODIGO_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = DisInadimplencia.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = DisInadimplencia.CODIGO_VENDEDOR, Tipo = DbType.String },});

              sSql = "INSERT INTO DadosDISTInadimplencia (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
                                                         "CODIGO_GERENTE, CODIGO_SUPERVISOR, CODIGO_VENDEDOR, NOME_GERENTE, NOME_SUPERVISOR," +
                                                         "NOME_VENDEDOR, PERCENTUAL_INADIMPLENCIA, VALOR_BRUTO_VENDA, VALOR_INADIMPLENCIA," +
                                                         "DTINTEGRACAO, VERSAO_INTEGRADOR)" +
                     " VALUES (#COD_PUXADA, #TIPO_REGISTRO, #TEL_CELULAR, #TIPO_CONSULTA, #VISAO_FATURAMENTO," +
                              "#CODIGO_GERENTE, #CODIGO_SUPERVISOR, #CODIGO_VENDEDOR, #NOME_GERENTE, #NOME_SUPERVISOR," +
                              "#NOME_VENDEDOR, #PERCENTUAL_INADIMPLENCIA, #VALOR_BRUTO_VENDA, #VALOR_INADIMPLENCIA," +
                              "#DTINTEGRACAO, #VERSAO_INTEGRADOR)";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TIPO_REGISTRO", Valor = sTIPO_REGISTRO, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TEL_CELULAR", Valor = sTEL_CELULAR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TIPO_CONSULTA", Valor = sTIPO_CONSULTA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "VISAO_FATURAMENTO", Valor = sVISAO_FATURAMENTO, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_GERENTE", Valor = DisInadimplencia.CODIGO_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = DisInadimplencia.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = DisInadimplencia.CODIGO_VENDEDOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_GERENTE", Valor = DisInadimplencia.NOME_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_SUPERVISOR", Valor = DisInadimplencia.NOME_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_VENDEDOR", Valor = DisInadimplencia.NOME_VENDEDOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "PERCENTUAL_INADIMPLENCIA", Valor = DisInadimplencia.PERCENTUAL_INADIMPLENCIA, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = DisInadimplencia.VALOR_BRUTO_VENDA, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "VALOR_INADIMPLENCIA", Valor = DisInadimplencia.VALOR_INADIMPLENCIA, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                          new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
            }

            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DisInadimplencia [" + sCOD_PUXADA + "] >> " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    private static Boolean FlagWZ_DisLogDevolucao(string sAplicativo,
                                                  string sPartner,
                                                  string sCdServico,
                                                  int iIdEmpresa,
                                                  int iIdTarefa,
                                                   string sConexao,
                                                   string sUsuario,
                                                   string sSenha,
                                                   string sCOD_PUXADA,
                                                   string sTIPO_REGISTRO,
                                                   string sTEL_CELULAR,
                                                   string sTIPO_CONSULTA,
                                                   string sVISAO_FATURAMENTO,
                                                   string sDS_STRINGCONEXAODESTINO,
                                                   string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DadosDISTLogDevolucao_Root DisLogDevolucao_Root;
        string sSql;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                       "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";

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
              DisLogDevolucao_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTLogDevolucao_Root>(content);
            }

            if (DisLogDevolucao_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = DisLogDevolucao_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        if (DisLogDevolucao_Root != null)
        {
          if (DisLogDevolucao_Root.DadosDISTLogDevolucao.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            foreach (FlagWSWhatsapp_DadosDISTLogDevolucao LogDevolucao in DisLogDevolucao_Root.DadosDISTLogDevolucao)
            {
              sSql = "DELETE FROM DadosDISTLogDevolucao" +
                     " WHERE COD_PUXADA=#COD_PUXADA" +
                       " AND CODIGO_GERENTE=#CODIGO_GERENTE" +
                       " AND CODIGO_SUPERVISOR=#CODIGO_SUPERVISOR" +
                       " AND CODIGO_VENDEDOR=#CODIGO_VENDEDOR";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_GERENTE", Valor = LogDevolucao.CODIGO_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = LogDevolucao.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = LogDevolucao.CODIGO_VENDEDOR, Tipo = DbType.String }});

              sSql = "INSERT INTO DadosDISTLogDevolucao (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
                                                        "CODIGO_GERENTE, CODIGO_SUPERVISOR, CODIGO_VENDEDOR, NOME_GERENTE," +
                                                        "NOME_SUPERVISOR, NOME_VENDEDOR, PERCENTUAL_DEVOLUCAO, VALOR_BRUTO_DEVOLUCAO," +
                                                        "VALOR_BRUTO_VENDA, DTINTEGRACAO, VERSAO_INTEGRADOR)" +
                     " VALUES (#COD_PUXADA, #TIPO_REGISTRO, #TEL_CELULAR, #TIPO_CONSULTA, #VISAO_FATURAMENTO," +
                              "#CODIGO_GERENTE, #CODIGO_SUPERVISOR, #CODIGO_VENDEDOR, #NOME_GERENTE," +
                              "#NOME_SUPERVISOR, #NOME_VENDEDOR, #PERCENTUAL_DEVOLUCAO, #VALOR_BRUTO_DEVOLUCAO," +
                              "#VALOR_BRUTO_VENDA, #DTINTEGRACAO, #VERSAO_INTEGRADOR)";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TIPO_REGISTRO", Valor = sTIPO_REGISTRO, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TEL_CELULAR", Valor = sTEL_CELULAR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "TIPO_CONSULTA", Valor = sTIPO_CONSULTA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "VISAO_FATURAMENTO", Valor = sVISAO_FATURAMENTO, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_GERENTE", Valor = LogDevolucao.CODIGO_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_SUPERVISOR", Valor = LogDevolucao.CODIGO_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VENDEDOR", Valor = LogDevolucao.CODIGO_VENDEDOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_GERENTE", Valor = LogDevolucao.NOME_GERENTE, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "NOME_SUPERVISOR", Valor = LogDevolucao.NOME_SUPERVISOR, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "PERCENTUAL_DEVOLUCAO", Valor = Convert.ToDecimal(LogDevolucao.PERCENTUAL_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "VALOR_BRUTO_DEVOLUCAO", Valor = Convert.ToDecimal(LogDevolucao.VALOR_BRUTO_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = Convert.ToDecimal(LogDevolucao.VALOR_BRUTO_VENDA) / 100, Tipo = DbType.Decimal },
                                                                          new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                          new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
            }

            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DisLogDevolucao [" + sCOD_PUXADA + "] >> " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    private static Boolean FlagWZ_DisLog_Taxa_Ocupacao(string sAplicativo,
                                                       string sPartner,
                                                       string sCdServico,
                                                       int iIdEmpresa,
                                                       int iIdTarefa,
                                                       string sConexao,
                                                       string sUsuario,
                                                       string sSenha,
                                                       string sCOD_PUXADA,
                                                       string sTIPO_REGISTRO,
                                                       string sTEL_CELULAR,
                                                       string sTIPO_CONSULTA,
                                                       string sVISAO_FATURAMENTO,
                                                       string sDS_STRINGCONEXAODESTINO,
                                                       string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      UltimoLocalErro = "FlagWZ_DisLog_Taxa_Ocupacao [" + ProductVersion + "]";

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      UltimoLocalErro = UltimoLocalErro + " - WebRequest.Create - " + sConexao;
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao_Root DisTLog_Taxa_Ocupacao_Root;
        string sSql;

        UltimoLocalErro = UltimoLocalErro + " - POST";
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                       "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";

        request.ContentLength = json.Length;

        UltimoLocalErro = UltimoLocalErro + " - StreamWriter(request.GetRequestStream())";
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        UltimoLocalErro = UltimoLocalErro + " - (HttpWebResponse)request.GetResponse()";
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              DisTLog_Taxa_Ocupacao_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao_Root>(content);
            }

            if (DisTLog_Taxa_Ocupacao_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = DisTLog_Taxa_Ocupacao_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        UltimoLocalErro = UltimoLocalErro + " - DisTLog_Taxa_Ocupacao_Root";
        if (DisTLog_Taxa_Ocupacao_Root != null)
        {
          if (DisTLog_Taxa_Ocupacao_Root.DadosDISTLog_Taxa_Ocupacao.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            UltimoLocalErro = UltimoLocalErro + " - oBancoDados.DBConectar (" + sTP_BANCODADOSDESTINO + "-" + sDS_STRINGCONEXAODESTINO + ")";
            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            if (!oBancoDados.DBConectado())
            {
              UltimoLocalErro = UltimoLocalErro + " - !oBancoDados.DBConectado()";
            }

            foreach (FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao LogDespersaoRota in DisTLog_Taxa_Ocupacao_Root.DadosDISTLog_Taxa_Ocupacao)
            {
              UltimoLocalErro = UltimoLocalErro + " - FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao";
              sSql = "DELETE FROM DadosDISTLogTaxaOcupacao" +
                     " WHERE COD_PUXADA=#COD_PUXADA" +
                       " AND CODIGO_VEICULO=#CODIGO_VEICULO";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                          new clsCampo { Nome = "CODIGO_VEICULO", Valor = LogDespersaoRota.CODIGO_VEICULO, Tipo = DbType.String }});

              sSql = "INSERT INTO DadosDISTLogTaxaOcupacao (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
                                                           "CODIGO_VEICULO, NUM_PLACA, PERCENTUAL_DEVOLUCAO, QTD_CAPACIDADE,QTD_VIAGEM ," +
                                                           "VALOR_BRUTO_DEVOLUCAO, VALOR_BRUTO_VENDA, DTINTEGRACAO, VERSAO_INTEGRADOR)" +
                     " VALUES (#COD_PUXADA, #TIPO_REGISTRO, #TEL_CELULAR, #TIPO_CONSULTA, #VISAO_FATURAMENTO," +
                              "#CODIGO_VEICULO, #NUM_PLACA, #PERCENTUAL_DEVOLUCAO, #QTD_CAPACIDADE,#QTD_VIAGEM," +
                              "#VALOR_BRUTO_DEVOLUCAO, #VALOR_BRUTO_VENDA, #DTINTEGRACAO, #VERSAO_INTEGRADOR)";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TIPO_REGISTRO", Valor = sTIPO_REGISTRO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TEL_CELULAR", Valor = sTEL_CELULAR, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TIPO_CONSULTA", Valor = sTIPO_CONSULTA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "VISAO_FATURAMENTO", Valor = sVISAO_FATURAMENTO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_VEICULO", Valor = LogDespersaoRota.CODIGO_VEICULO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "NUM_PLACA", Valor = LogDespersaoRota.NUM_PLACA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "PERCENTUAL_DEVOLUCAO", Valor = LogDespersaoRota.PERCENTUAL_DEVOLUCAO, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "QTD_CAPACIDADE", Valor = Convert.ToDecimal(LogDespersaoRota.QTDE_CAPACIDADE) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "QTD_VIAGEM", Valor = LogDespersaoRota.QTDE_VIAGEM, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "QTD_CARGA", Valor = LogDespersaoRota.QTD_CARGA, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_DEVOLUCAO", Valor = LogDespersaoRota.VALOR_BRUTO_DEVOLUCAO, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = LogDespersaoRota.VALOR_BRUTO_VENDA, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                        new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
            }

            UltimoLocalErro = UltimoLocalErro + " - DBDesconectar";
            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DisLog_Taxa_Ocupacao [" + sCOD_PUXADA + "] >> " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    private static Boolean FlagWZ_DisLogDevolucaoMotorista(string sAplicativo,
                                                           string sPartner,
                                                           string sCdServico,
                                                           int iIdEmpresa,
                                                           int iIdTarefa,
                                                            string sConexao,
                                                            string sUsuario,
                                                            string sSenha,
                                                            string sCOD_PUXADA,
                                                            string sTIPO_REGISTRO,
                                                            string sTEL_CELULAR,
                                                            string sTIPO_CONSULTA,
                                                            string sVISAO_FATURAMENTO,
                                                            string sDS_STRINGCONEXAODESTINO,
                                                            string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista_Root DisTLogDevolucaoMotorista_Root;
        string sSql;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                       "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";

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
              DisTLogDevolucaoMotorista_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista_Root>(content);
            }

            if (DisTLogDevolucaoMotorista_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = DisTLogDevolucaoMotorista_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        if (DisTLogDevolucaoMotorista_Root != null)
        {
          if (DisTLogDevolucaoMotorista_Root.DadosDISTLogDevolucaoMotorista.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            foreach (FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista LogDevolucaoMotorista in DisTLogDevolucaoMotorista_Root.DadosDISTLogDevolucaoMotorista)
            {
              sSql = "DELETE FROM DadosDISTLogDevolucaoMotorista" +
                     " WHERE COD_PUXADA=#COD_PUXADA" +
                       " AND CODIGO_MOTORISTA=#CODIGO_MOTORISTA";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_MOTORISTA", Valor = LogDevolucaoMotorista.CODIGO_MOTORISTA, Tipo = DbType.String }});

              sSql = "INSERT INTO DadosDISTLogDevolucaoMotorista (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
                                                                 "CODIGO_MOTORISTA, NOME_MOTORISTA, PERCENTUAL_DEVOLUCAO," +
                                                                 "VALOR_BRUTO_DEVOLUCAO ,VALOR_BRUTO_VENDA, DTINTEGRACAO, VERSAO_INTEGRADOR)" +
                     " VALUES (#COD_PUXADA, #TIPO_REGISTRO, #TEL_CELULAR, #TIPO_CONSULTA, #VISAO_FATURAMENTO," +
                              "#CODIGO_MOTORISTA, #NOME_MOTORISTA, #PERCENTUAL_DEVOLUCAO," +
                              "#VALOR_BRUTO_DEVOLUCAO, @VALOR_BRUTO_VENDA, #DTINTEGRACAO, #VERSAO_INTEGRADOR)";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TIPO_REGISTRO", Valor = sTIPO_REGISTRO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TEL_CELULAR", Valor = sTEL_CELULAR, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TIPO_CONSULTA", Valor = sTIPO_CONSULTA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "VISAO_FATURAMENTO", Valor = sVISAO_FATURAMENTO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_MOTORISTA", Valor = LogDevolucaoMotorista.CODIGO_MOTORISTA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "NOME_MOTORISTA", Valor = LogDevolucaoMotorista.NOME_MOTORISTA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "PERCENTUAL_DEVOLUCAO", Valor = LogDevolucaoMotorista.PERCENTUAL_DEVOLUCAO, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_DEVOLUCAO", Valor = LogDevolucaoMotorista.VALOR_BRUTO_DEVOLUCAO, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = LogDevolucaoMotorista.VALOR_BRUTO_VENDA, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                        new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
            }

            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DisLogDevolucaoMotorista [" + sCOD_PUXADA + "] >> " + Ex.Message);
        bOk = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    private static Boolean DisLogDevolucaoCarro(string sAplicativo,
                                                string sPartner,
                                                string sCdServico,
                                                int iIdEmpresa,
                                                int iIdTarefa,
                                                string sConexao,
                                                string sUsuario,
                                                string sSenha,
                                                string sCOD_PUXADA,
                                                string sTIPO_REGISTRO,
                                                string sTEL_CELULAR,
                                                string sTIPO_CONSULTA,
                                                string sVISAO_FATURAMENTO,
                                                string sDS_STRINGCONEXAODESTINO,
                                                string sTP_BANCODADOSDESTINO)
    {
      bool bOk = false;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
        FlagWSWhatsapp_DadosDISTLogDevolucaoCarro_Root DisTLogDevolucaoCarro_Root;
        string sSql;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                       "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";

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
              DisTLogDevolucaoCarro_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTLogDevolucaoCarro_Root>(content);
            }

            if (DisTLogDevolucaoCarro_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = DisTLogDevolucaoCarro_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        if (DisTLogDevolucaoCarro_Root != null)
        {
          if (DisTLogDevolucaoCarro_Root.DadosDISTLogDevolucaoCarro.Count > 0)
          {
            clsBancoDados oBancoDados = new clsBancoDados();

            oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

            foreach (FlagWSWhatsapp_DadosDISTLogDevolucaoCarro LogDevolucaoCarro in DisTLogDevolucaoCarro_Root.DadosDISTLogDevolucaoCarro)
            {
              sSql = "DELETE FROM DadosDISTLogDevolucaoCarro" +
                     " WHERE COD_PUXADA=#COD_PUXADA" +
                       " AND CODIGO_VEICULO=#CODIGO_VEICULO";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_VEICULO", Valor = LogDevolucaoCarro.CODIGO_VEICULO, Tipo = DbType.String }});

              sSql = "INSERT INTO DadosDISTLogDevolucaoCarro (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
                                                             "CODIGO_VEICULO, NUM_PLACA, PERCENTUAL_DEVOLUCAO," +
                                                             "VALOR_BRUTO_DEVOLUCAO ,VALOR_BRUTO_VENDA, DTINTEGRACAO, VERSAO_INTEGRADOR)" +
                     " VALUES (#COD_PUXADA, #TIPO_REGISTRO, #TEL_CELULAR, #TIPO_CONSULTA, #VISAO_FATURAMENTO," +
                              "#CODIGO_VEICULO, #NUM_PLACA, #PERCENTUAL_DEVOLUCAO," +
                              "#VALOR_BRUTO_DEVOLUCAO, @VALOR_BRUTO_VENDA, #DTINTEGRACAO, #VERSAO_INTEGRADOR)";
              oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TIPO_REGISTRO", Valor = sTIPO_REGISTRO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TEL_CELULAR", Valor = sTEL_CELULAR, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TIPO_CONSULTA", Valor = sTIPO_CONSULTA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "VISAO_FATURAMENTO", Valor = sVISAO_FATURAMENTO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_VEICULO", Valor = LogDevolucaoCarro.CODIGO_VEICULO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "NUM_PLACA", Valor = LogDevolucaoCarro.NUM_PLACA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "PERCENTUAL_DEVOLUCAO", Valor = Convert.ToDecimal(LogDevolucaoCarro.PERCENTUAL_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_DEVOLUCAO", Valor = Convert.ToDecimal(LogDevolucaoCarro.VALOR_BRUTO_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = Convert.ToDecimal(LogDevolucaoCarro.VALOR_BRUTO_VENDA) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                        new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
            }

            oBancoDados.DBDesconectar();

            bOk = true;
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "DisLogDevolucaoCarro [" + sCOD_PUXADA + "] >> " + Ex.Message);
        bOk = false;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    public class CONDICAO_PAGAMENTO
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
      public List<CONDICAO_PAGAMENTO> CONDICAO_PAGAMENTO { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    private static bool OlaPDV_CondicaoPagamento(string sAplicativo,
                                                 string sPartner,
                                                 string sCdServico,
                                                 string sTarefa,
                                                 int nr_ordem_execucao,
                                                 int iIdEmpresa,
                                                 int iIdTarefa,
                                                 long iGrupoTarefas,
                                                 string sConexao,
                                                 string sUsuario,
                                                 string sSenha,
                                                 string sCOD_PUXADA,
                                                 string sTIPO_REGISTRO,
                                                 string sTIPO_CONSULTA,
                                                 string sDS_STRINGCONEXAODESTINO,
                                                 string sTP_BANCODADOSDESTINO,
                                                 string sDS_Origem,
                                                 string sDS_Destino,
                                                 ref string sJsonRet)
    {
      bool bRet = false;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int CondicaoPadrao = 0;
      int iEmpresa = 0;
      string sEDI_Integracao;
      string sCOD_TIPO_DOCUMENTO_COBRANCA = "";
      int iCont = 0;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      UltimoLocalErro = "FlagWZ_DisLog_Taxa_Ocupacao [" + ProductVersion + "]";

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      UltimoLocalErro = UltimoLocalErro + " - WebRequest.Create - " + sConexao;
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        OlaPDV_CondicaoPagamento_Root OlaPDV_CondicaoPagamento_Root;

        UltimoLocalErro = UltimoLocalErro + " - POST";
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        UltimoLocalErro = UltimoLocalErro + " - StreamWriter(request.GetRequestStream())";
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        UltimoLocalErro = UltimoLocalErro + " - (HttpWebResponse)request.GetResponse()";
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              sJsonRet = content;
              OlaPDV_CondicaoPagamento_Root = JsonConvert.DeserializeObject<OlaPDV_CondicaoPagamento_Root>(content);
            }

            if (OlaPDV_CondicaoPagamento_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = OlaPDV_CondicaoPagamento_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);
        UltimoLocalErro = UltimoLocalErro + " - oBancoDados.DBConectar (" + sTP_BANCODADOSDESTINO + "-" + sDS_STRINGCONEXAODESTINO + ")";

        UltimoLocalErro = UltimoLocalErro + " - OlaPDV_PrecoProduto_TabelaPrecoProduto_Root";

        if (OlaPDV_CondicaoPagamento_Root != null)
        {
          if (OlaPDV_CondicaoPagamento_Root.CONDICAO_PAGAMENTO.Count > 0 && oBancoDados_Destino.DBConectado())
          {
            iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("SELECT idEmpresa FROM " + oBancoDados_Destino.BancoDados() + "tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_condicoes_pagamento" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where id_Empresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);

            foreach (CONDICAO_PAGAMENTO CONDICAO_PAGAMENTO in OlaPDV_CondicaoPagamento_Root.CONDICAO_PAGAMENTO)
            {
              iCont++;
              Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, OlaPDV_CondicaoPagamento_Root.CONDICAO_PAGAMENTO.Count);

              sEDI_Integracao = CONDICAO_PAGAMENTO.COD_CONDICAO_PAGAMENTO.ToString().Trim().PadLeft(3, '0');

              if (CONDICAO_PAGAMENTO.DSC_CONDICAO_PAGAMENTO.ToString().Trim().ToUpper() == "DINHEIRO - A VISTA")
              {
                CondicaoPadrao = 1;
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_condicoes_pagamento set CondicaoPadrao = 0 where id_empresa = " + iEmpresa.ToString();
              }
              else
                CondicaoPadrao = 0;

              try
              {
                sCOD_TIPO_DOCUMENTO_COBRANCA = CONDICAO_PAGAMENTO.COD_TIPO_DOCUMENTO_COBRANCA;
              }
              catch (Exception)
              {
                sCOD_TIPO_DOCUMENTO_COBRANCA = "00";     
              }

              sSql = "SELECT count(*) FROM " + oBancoDados_Destino.BancoDados() + "tb_condicoes_pagamento where id_empresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + sEDI_Integracao.Trim() + "'";

              if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
              {
                sSql = "insert into " + oBancoDados_Destino.BancoDados() + "tb_condicoes_pagamento " +
                    "           (CondicaoPadrao, COD_TIPO_CONDICAO_PAGAMENTO, condicoes_pagamento, status_condicoes_pagamento," +
                                "PRC_ADICIONAL_FINANCEIRO, PRIORIDADE_CONDICAO_PAGAMENTO, TIPO_DOCUMENTO,valor_minimo, NroParcelas," +
                                "Sincronizado,CodTipoDocumentoCobranca,dtSincronizacao,verSincronizador,idSincronizador,id_empresa,EDI_Integracao)" +
                       " values (?CondicaoPadrao,?COD_TIPO_CONDICAO_PAGAMENTO,?DSC_CONDICAO_PAGAMENTO,?IND_ATIVO," +
                                "?PRC_ADICIONAL_FINANCEIRO,?PRIORIDADE_CONDICAO_PAGAMENTO,?TIPO_DOCUMENTO,?valor_minimo,?NroParcelas," +
                                "?Sincronizado,?CodTipoDocumentoCobranca, " + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador," +
                                "?id_empresa,?EDI_Integracao)";
              }
              else
              {
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_condicoes_pagamento" +
                       " set CondicaoPadrao=?CondicaoPadrao," +
                            "COD_TIPO_CONDICAO_PAGAMENTO=?COD_TIPO_CONDICAO_PAGAMENTO," +
                            "condicoes_pagamento=?DSC_CONDICAO_PAGAMENTO," +
                            "status_condicoes_pagamento=?IND_ATIVO," +
                            "PRC_ADICIONAL_FINANCEIRO=?PRC_ADICIONAL_FINANCEIRO," +
                            "PRIORIDADE_CONDICAO_PAGAMENTO=?PRIORIDADE_CONDICAO_PAGAMENTO," +
                            "TIPO_DOCUMENTO=?TIPO_DOCUMENTO," +
                            "CodTipoDocumentoCobranca=?CodTipoDocumentoCobranca," +
                            "valor_minimo=?valor_minimo," +
                            "NroParcelas=?NroParcelas," +
                            "Sincronizado=?Sincronizado," +
                            "dtSincronizacao=current_timestamp," +
                            "verSincronizador=?verSincronizador," +
                            "idSincronizador=?idSincronizador" +                            
                       " where id_empresa=?id_empresa" +
                         " and EDI_Integracao=?EDI_Integracao";
              }

              oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?CondicaoPadrao", Valor = CondicaoPadrao, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?COD_TIPO_CONDICAO_PAGAMENTO", Valor = Convert.ToInt32(CONDICAO_PAGAMENTO.COD_TIPO_CONDICAO_PAGAMENTO), Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?DSC_CONDICAO_PAGAMENTO", Valor = CONDICAO_PAGAMENTO.DSC_CONDICAO_PAGAMENTO.ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?IND_ATIVO", Valor = Convert.ToInt32(CONDICAO_PAGAMENTO.IND_ATIVO), Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?PRC_ADICIONAL_FINANCEIRO", Valor = Convert.ToDouble(CONDICAO_PAGAMENTO.PRC_ADICIONAL_FINANCEIRO), Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?PRIORIDADE_CONDICAO_PAGAMENTO", Valor = Convert.ToInt32(CONDICAO_PAGAMENTO.PRIORIDADE_CONDICAO_PAGAMENTO), Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?TIPO_DOCUMENTO", Valor = _Funcoes.FNC_NuloString(CONDICAO_PAGAMENTO.TIPO_DOCUMENTO), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?CodTipoDocumentoCobranca", Valor = sCOD_TIPO_DOCUMENTO_COBRANCA, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?valor_minimo", Valor = Convert.ToDouble(CONDICAO_PAGAMENTO.VLR_MINIMO_PEDIDO), Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?NroParcelas", Valor = 1, Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?id_empresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?EDI_Integracao", Valor = sEDI_Integracao.Trim(), Tipo = DbType.String }});
            }

            sSql = "update olapdv.tb_condicoes_pagamento set CondicaoAmigavel = concat(initcap(condicoes_pagamento), ' (R$', round(valor_minimo, 2), ')')" +
                   " where id_Empresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);

            bRet = true;
          }
        }
      }
      catch (Exception Ex)
      {
        sStatus = "ERRO";
        sErro = Ex.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }

    private static bool OlaPDV_ConfiguracaoEscalonada(string sAplicativo,
                                                      string sPartner,
                                                      string sCdServico,
                                                      string sTarefa,
                                                      int nr_ordem_execucao,
                                                      int iIdEmpresa,
                                                      int iIdTarefa,
                                                      long iGrupoTarefas,
                                                      string sConexao,
                                                      string sUsuario,
                                                      string sSenha,
                                                      string sCOD_PUXADA,
                                                      string sTIPO_REGISTRO,
                                                      string sTIPO_CONSULTA,
                                                      string sDS_STRINGCONEXAODESTINO,
                                                      string sTP_BANCODADOSDESTINO,
                                                      string sDS_Origem,
                                                      string sDS_Destino,
                                                      ref string json)
    {
      bool bRet = false;
      DataTable oData = null;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int iEmpresa = 0;
      int iCont = 0;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      try
      {
        oData = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico, ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa, sDS_Origem, sConexao, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, "", ref json);

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

        if (oData != null && oBancoDados_Destino.DBConectado())
        {
          iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("SELECT idEmpresa FROM " + oBancoDados_Destino.BancoDados() + "tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

          sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_escalonada_configuracao" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);

          foreach (DataRow oRow in oData.Rows)
          {
            iCont++;
            Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, oData.Rows.Count);

            sSql = "SELECT count(*) FROM " + oBancoDados_Destino.BancoDados() + "tb_escalonada_configuracao where idEmpresa = " + iEmpresa.ToString() + " and COD_ESCALONADA = '" + oRow["COD_ESCALONADA"] + "'";

            if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
            {
              sSql = "insert into " + oBancoDados_Destino.BancoDados() + "tb_escalonada_configuracao" +
                     "(DSC_ESCALONADA,dtSincronizacao,Sincronizado,verSincronizador,idSincronizador," +
                      "idEmpresa,COD_ESCALONADA)" +
                     " values (?DSC_ESCALONADA, " + oBancoDados_Destino.DBSData() + ",?Sincronizado,?verSincronizador,?idSincronizador," +
                              "?idEmpresa,?COD_ESCALONADA)";
            }
            else
            {
              sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_escalonada_configuracao" +
                     " set DSC_ESCALONADA=?DSC_ESCALONADA," +
                          "Sincronizado=?Sincronizado," +
                          "verSincronizador=?verSincronizador," +
                          "idSincronizador=?idSincronizador" +
                     " where idEmpresa=?idEmpresa" +
                       " and COD_ESCALONADA=?COD_ESCALONADA";
            }

            oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?DSC_ESCALONADA", Valor = oRow["DSC_ESCALONADA"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?COD_ESCALONADA", Valor = oRow["COD_ESCALONADA"].ToString(), Tipo = DbType.String }});
          }

          bRet = true;
        }
      }
      catch (Exception E)
      {
        sStatus = "ERRO";
        sErro = E.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }

    private static bool OlaPDV_CategoriaEscalonada(string sAplicativo,
                                                   string sPartner,
                                                   string sCdServico,
                                                   string sTarefa,
                                                   int nr_ordem_execucao,
                                                   int iIdEmpresa,
                                                   int iIdTarefa,
                                                   long iGrupoTarefas,
                                                   string sConexao,
                                                   string sUsuario,
                                                   string sSenha,
                                                   string sCOD_PUXADA,
                                                   string sTIPO_REGISTRO,
                                                   string sTIPO_CONSULTA,
                                                   string sDS_STRINGCONEXAODESTINO,
                                                   string sTP_BANCODADOSDESTINO,
                                                   string sDS_Origem,
                                                   string sDS_Destino,
                                                   ref string sJsonRet)
    {
      bool bRet = false;
      DataTable oData = null;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int iEmpresa = 0;
      int IdEscalonadaConfiguracao = 0;
      int IdTabelaPreco = 0;
      int iCont = 0;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      try
      {
        oData = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico, ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa, sDS_Origem, sConexao,
                                                sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, "", ref sJsonRet);

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

        if (oData != null && oBancoDados_Destino.DBConectado())
        {
          iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("select idEmpresa from " + oBancoDados_Destino.BancoDados() + "tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

          if (iEmpresa != 0)
          {
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_escalonada_categoria" +
                    " set Sincronizado = 'N'" +
                        ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                        ",verSincronizador='" + ProductVersion + "'" +
                        ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                    " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);

            foreach (DataRow oRow in oData.Rows)
            {
              iCont++;
              Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, oData.Rows.Count);

              sSql = "select IdEscalonadaConfiguracao from " + oBancoDados_Destino.BancoDados() + "tb_escalonada_configuracao where idEmpresa = " + iEmpresa.ToString() + " and IdEscalonadaConfiguracao = " + oRow["COD_ESCALONADA"].ToString();
              IdEscalonadaConfiguracao = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              sSql = "select idTabelaPreco from " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos where idEmpresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + oRow["COD_TABELA_PRECO"].ToString().Trim().PadLeft(2, '0') + "'";
              IdTabelaPreco = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              sSql = "SELECT count(*) FROM " + oBancoDados_Destino.BancoDados() + "tb_escalonada_categoria" +
                     " where IdEscalonadaConfiguracao = " + IdEscalonadaConfiguracao.ToString() + " and IdTabelaPreco = " + IdTabelaPreco.ToString() + " and COD_CTI = " + oRow["COD_CTI"].ToString();

              if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_escalonada_categoria" +
                       "(IdEscalonadaConfiguracao,IdTabelaPreco,idEmpresa,COD_CTI,COD_TABELA_PRECO,COD_ESCALONADA," +
                        "Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                       " values " +
                       "(?IdEscalonadaConfiguracao,?IdTabelaPreco,?idEmpresa,?COD_CTI,?COD_TABELA_PRECO,?COD_ESCALONADA," +
                        "?Sincronizado, " + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador)";
                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?IdEscalonadaConfiguracao", Valor = IdEscalonadaConfiguracao, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?IdTabelaPreco", Valor = IdTabelaPreco, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?COD_CTI", Valor = oRow["COD_CTI"].ToString(), Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?COD_TABELA_PRECO", Valor = oRow["COD_TABELA_PRECO"].ToString(), Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?COD_ESCALONADA", Valor = oRow["COD_ESCALONADA"].ToString(), Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});
              }
            }

            //sSql = "DELETE FROM " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos WHERE NROFAIXA > 0";
            //oBancoDados_Destino.DBExecutar(sSql);

            //sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos " +
            //       "(idTabelaPreco, idEmpresa, idTabelaPrecoFaixa, idUnidend if;adeMedida, idProduto, PrecoCusto, PrecoMinimo, PrecoVenda, PrecoVendaUnitario, " +
            //        "ValorPromocional, PrecoFaixaInicial, nroFaixa, PrecoFaixaFinal, AgrupadorPreco, TextoFaixa, PrecoExibicao, PrecoExibicaoUnitario, " +
            //        "EDI_Integracao, StatusPreco, dtAtualizacao, VersaoPreco, idProtocolo, QTD_MAXIMA_PEDIDA, QTD_MINIMA_PEDIDA, IND_MULTIPLO," +
            //        "Sincronizado,dtSincronizacao,verSincronizador,idSincronizador) " +
            //        "SELECT idTabelaPreco, idEmpresa, idTabelaPrecoFaixa, idUnidadeMedida, idProduto, PrecoCusto, PrecoMinimo, PrecoVenda, PrecoVendaUnitario," +
            //               "ValorPromocional, PrecoFaixaInicial, 1, 99999, AgrupadorPreco, TextoFaixa, PrecoExibicao, PrecoExibicaoUnitario, EDI_Integracao," +
            //               "StatusPreco, dtAtualizacao, 1, idProtocolo,QTD_MAXIMA_PEDIDA,QTD_MINIMA_PEDIDA,IND_MULTIPLO," +
            //               "'S'," + oBancoDados_Destino.DBSData() + ",'" + ProductVersion + "'," + Config_App.idIntegrador.ToString() +
            //        " FROM " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos" +
            //        " WHERE tb_tabela_precos_produtos.NROFAIXA = 0" +
            //          " AND tb_tabela_precos_produtos.idTabelaPreco = 7" +
            //          " AND NOT tb_tabela_precos_produtos.IDPRODUTO IN (SELECT IDPRODUTO FROM " + oBancoDados_Destino.BancoDados() + "tb_escalonada_faixa_desconto)";
            //oBancoDados_Destino.DBExecutar(sSql);

            //sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos " +
            //       "(idTabelaPreco, idEmpresa, idTabelaPrecoFaixa, idUnidadeMedida, idProduto, PrecoCusto, PrecoMinimo, PrecoVenda, PrecoVendaUnitario," +
            //        "ValorPromocional, PrecoFaixaInicial, nroFaixa, PrecoFaixaFinal, AgrupadorPreco, TextoFaixa, PrecoExibicao, PrecoExibicaoUnitario," +
            //        "EDI_Integracao, StatusPreco, dtAtualizacao, VersaoPreco, idProtocolo, QTD_MAXIMA_PEDIDA, QTD_MINIMA_PEDIDA, IND_MULTIPLO," +
            //        "Sincronizado,dtSincronizacao,verSincronizador,idSincronizador) " +
            //       "SELECT tpc.idTabelaPreco, tpc.idEmpresa, tpc.idTabelaPrecoFaixa, tpc.idUnidadeMedida, tpc.idProduto, tpc.PrecoCusto," +
            //              "round(tpc.PrecoMinimo - (tpc.PrecoMinimo * PCT_MAXIMO_DESCONTO / 100), 2), round(tpc.PrecoVenda - (tpc.PrecoVenda * PCT_MAXIMO_DESCONTO / 100), 2)," +
            //              "round(tpc.PrecoVendaUnitario - (tpc.PrecoVendaUnitario * PCT_MAXIMO_DESCONTO / 100), 2), tpc.ValorPromocional, tdesc.qtd_inicial_escalonada," +
            //              "tdesc.num_sequencia_escalonada, tdesc.qtd_final_escalonada, tpc.AgrupadorPreco, concat(case when tdesc.qtd_inicial_escalonada = 0 then 1 else tdesc.qtd_inicial_escalonada end, ' a ', tdesc.qtd_final_escalonada)," +
            //              "tpc.PrecoExibicao, tpc.PrecoExibicaoUnitario, tpc.EDI_Integracao, tpc.StatusPreco, tpc.dtAtualizacao, 1, tpc.idProtocolo, tpc.QTD_MAXIMA_PEDIDA," +
            //              "tpc.QTD_MINIMA_PEDIDA, tpc.IND_MULTIPLO," +
            //               "'S'," + oBancoDados_Destino.DBSData() + ",'" + ProductVersion + "'," + Config_App.idIntegrador.ToString() +
            //        " FROM " + oBancoDados_Destino.BancoDados() + "tb_escalonada_faixa_desconto as tdesc" +
            //        " left join " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos as tpc on tpc.idproduto = tdesc.idproduto " +
            //       " WHERE tpc.NROFAIXA = 0" +
            //         " and tpc.idTabelaPreco = 7" +
            //         " and tdesc.NUM_SEQUENCIA_ESCALONADA = 1" +
            //         " and tdesc.COD_EMPREGADO = 203" +
            //       " order by tpc.idProduto, tpc.PrecoFaixaInicial";
            //oBancoDados_Destino.DBExecutar(sSql);

            //sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos" +
            //       "(idTabelaPreco, idEmpresa, idTabelaPrecoFaixa, idUnidadeMedida, idProduto, PrecoCusto, PrecoMinimo, PrecoVenda, PrecoVendaUnitario," +
            //        "ValorPromocional, PrecoFaixaInicial, nroFaixa, PrecoFaixaFinal, AgrupadorPreco, TextoFaixa, PrecoExibicao, PrecoExibicaoUnitario," +
            //        "EDI_Integracao, StatusPreco, dtAtualizacao, VersaoPreco, idProtocolo, QTD_MAXIMA_PEDIDA, QTD_MINIMA_PEDIDA, IND_MULTIPLO," +
            //        "Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
            //       "SELECT tpc.idTabelaPreco, tpc.idEmpresa, tpc.idTabelaPrecoFaixa, tpc.idUnidadeMedida, tpc.idProduto, tpc.PrecoCusto," +
            //              "round(tpc.PrecoMinimo - (tpc.PrecoMinimo * PCT_MAXIMO_DESCONTO / 100), 2), round(tpc.PrecoVenda - (tpc.PrecoVenda * PCT_MAXIMO_DESCONTO / 100), 2), round(tpc.PrecoVendaUnitario - (tpc.PrecoVendaUnitario * PCT_MAXIMO_DESCONTO / 100), 2)," +
            //              "tpc.ValorPromocional, tdesc.qtd_inicial_escalonada, tdesc.num_sequencia_escalonada, tdesc.qtd_final_escalonada, tpc.AgrupadorPreco, concat(case when tdesc.qtd_inicial_escalonada = 0 then 1 else tdesc.qtd_inicial_escalonada end, ' a ', tdesc.qtd_final_escalonada)," +
            //              "tpc.PrecoExibicao, tpc.PrecoExibicaoUnitario, tpc.EDI_Integracao, tpc.StatusPreco, tpc.dtAtualizacao, 1, tpc.idProtocolo, tpc.QTD_MAXIMA_PEDIDA," +
            //              "tpc.QTD_MINIMA_PEDIDA, tpc.IND_MULTIPLO," +
            //               "'S'," + oBancoDados_Destino.DBSData() + ",'" + ProductVersion + "'," + Config_App.idIntegrador.ToString() +
            //        " FROM " + oBancoDados_Destino.BancoDados() + "tb_escalonada_faixa_desconto as tdesc" +
            //        " left join " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos as tpc on tpc.idproduto = tdesc.idproduto" +
            //       " WHERE tpc.NROFAIXA = 0" +
            //         " and tpc.idTabelaPreco = 7" +
            //         " and tdesc.NUM_SEQUENCIA_ESCALONADA > 1" +
            //         " and tdesc.COD_EMPREGADO = 203" +
            //         " and tdesc.PCT_MAXIMO_DESCONTO > 0" +
            //       " order by tpc.idProduto, tpc.PrecoFaixaInicial";
            //oBancoDados_Destino.DBExecutar(sSql);

            //sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos" +
            //       " inner join " + oBancoDados_Destino.BancoDados() + "tb_produtos on tb_produtos.idProduto = tb_tabela_precos_produtos.idProduto" +
            //       " set tb_tabela_precos_produtos.PrecoCusto = 0," +
            //            "tb_tabela_precos_produtos.Sincronizado = 'S'," +
            //            "tb_tabela_precos_produtos.idTabelaPrecoFaixa = tb_tabela_precos_produtos.idTabelaPreco," +
            //            "tb_tabela_precos_produtos.StatusPreco = 1," +
            //            "tb_tabela_precos_produtos.PrecoMinimo = tb_tabela_precos_produtos.PrecoVenda," +
            //            "tb_tabela_precos_produtos.PrecoExibicao = Concat('R$ ', Replace(Replace(Replace(Format(tb_tabela_precos_produtos.PrecoVenda, 2), '.', '|'), ',', '.'), '|', ','))," +
            //            "tb_tabela_precos_produtos.PrecoVendaUnitario = round(tb_tabela_precos_produtos.PrecoVenda / tb_produtos.Fator_Venda, 2)," +
            //            "tb_tabela_precos_produtos.PrecoExibicaoUnitario = Concat('R$ ', Replace(Replace(Replace(Format(tb_tabela_precos_produtos.PrecoVenda / tb_produtos.Fator_Venda, 2), '.', '|'), ',', '.'), '|', ','))," +
            //            "tb_tabela_precos_produtos.Sincronizado='S'," +
            //            "tb_tabela_precos_produtos.dtSincronizacao=" + oBancoDados_Destino.DBSData() + "," +
            //            "tb_tabela_precos_produtos.verSincronizador='" + ProductVersion + "'," +
            //            "tb_tabela_precos_produtos.idSincronizador=" + Config_App.idIntegrador.ToString() +
            //       " where tb_tabela_precos_produtos.idProduto > 0";
            //oBancoDados_Destino.DBExecutar(sSql);

            //sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos" +
            //       " set tb_tabela_precos_produtos.textofaixa = Concat('Maior que ', tb_tabela_precos_produtos.precofaixainicial)," +
            //            "tb_tabela_precos_produtos.Sincronizado='S'," +
            //            "tb_tabela_precos_produtos.dtSincronizacao=" + oBancoDados_Destino.DBSData() + "," +
            //            "tb_tabela_precos_produtos.verSincronizador='" + ProductVersion + "'," +
            //            "tb_tabela_precos_produtos.idSincronizador=" + Config_App.idIntegrador.ToString() +
            //       " where tb_tabela_precos_produtos.nroFaixa > 1" +
            //         " and tb_tabela_precos_produtos.precofaixafinal > 99999";
            //oBancoDados_Destino.DBExecutar(sSql);

            //                    /*, tb_produtos.Descricao_Detalhada=(case when tb_tabela_precos_produtos.IND_MULTIPLO=0 then tb_produtos.Descricao_Detalhada else 'Unitario' end)
            //, tb_produtos.Tamanho=(case when tb_tabela_precos_produtos.IND_MULTIPLO=0 then tb_produtos.Tamanho else 'UN' end)
            //, tb_produtos.Quantidade_Exibicao=(case when tb_tabela_precos_produtos.IND_MULTIPLO=0 then tb_produtos.Quantidade_Exibicao else 'UN' end)
            //, tb_produtos.Medida=(case when tb_tabela_precos_produtos.IND_MULTIPLO=0 then tb_produtos.Medida else 'UN' end)
            //*/
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_produtos" +
                   " inner join " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos on tb_produtos.idProduto = tb_tabela_precos_produtos.idProduto" +
                   " inner join " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos on tb_tabela_precos_produtos.idTabelaPreco = tb_tabela_precos_produtos.idTabelaPreco" +
                   " set tb_produtos.Preco1Venda = (case when tb_tabela_precos_produtos.IND_MULTIPLO = 0 then tb_tabela_precos_produtos.PrecoVenda else tb_tabela_precos_produtos.PrecoVendaUnitario end)" +
                       ",tb_produtos.Preco2VendaUnitario = tb_tabela_precos_produtos.PrecoVendaUnitario" +
                       ",Custo = 0, CustoUnitario = 0, usaMultiplicador = tb_tabela_precos_produtos.IND_MULTIPLO" +
                       ",tb_produtos.Preco1Venda = (case when tb_tabela_precos_produtos.IND_MULTIPLO = 0 then tb_tabela_precos_produtos.PrecoVenda else tb_tabela_precos_produtos.PrecoVendaUnitario end)" +
                       ",tb_produtos.PrecoVendaUnitarioExib = Concat('R$ ', Replace(Replace(Replace(Format(tb_tabela_precos_produtos.PrecoVendaUnitario, 2), '.', '|'), ',', '.'), '|', ','))" +
                   " where tb_tabela_precos_produtos.idProduto > 0" +
                     " and tb_tabela_precos.TabelaPadrao = 1; ";
            oBancoDados_Destino.DBExecutar(sSql);

            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_categorias1" +
                   " set Categoria1Amigavel = Categoria1" +
                       ",Sincronizado = 'N'" +
             //          ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
             //        ",verSincronizador='" + ProductVersion + "'" +
             //          ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where Categoria1Amigavel is null";
            oBancoDados_Destino.DBExecutar(sSql);
          }

          bRet = true;
        }
      }
      catch (Exception E)
      {
        sStatus = "ERRO";
        sErro = E.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }

    private static bool OlaPDV_FaixaDescontoEscalonada(string sAplicativo,
                                                       string sPartner,
                                                       string sCdServico,
                                                       string sTarefa,
                                                       int nr_ordem_execucao,
                                                       int iIdEmpresa,
                                                       int iIdTarefa,
                                                       long iGrupoTarefas,
                                                       string sConexao,
                                                       string sUsuario,
                                                       string sSenha,
                                                       string sCOD_PUXADA,
                                                       string sTIPO_REGISTRO,
                                                       string sTIPO_CONSULTA,
                                                       string sDS_STRINGCONEXAODESTINO,
                                                       string sTP_BANCODADOSDESTINO,
                                                       string sDS_Origem,
                                                       string sDS_Destino,
                                                       ref string sJsonRet)
    {
      bool bRet = false;
      DataTable oData = null;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int iEmpresa = 0;
      int IdProduto = 0;
      int iCont = 0;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      try
      {
        oData = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico, ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa, sDS_Origem, sConexao, sCOD_PUXADA,
                                                sTIPO_REGISTRO, sTIPO_CONSULTA, "", ref sJsonRet);

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

        if (oData != null && oBancoDados_Destino.DBConectado())
        {
          iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("select idEmpresa from " + oBancoDados_Destino.BancoDados() + "tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

          sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_escalonada_faixa_desconto" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);

          foreach (DataRow oRow in oData.Rows)
          {
            iCont++;
            Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, oData.Rows.Count);

            sSql = "select IdProduto from " + oBancoDados_Destino.BancoDados() + "tb_produtos where idEmpresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + oRow["COD_PRODUTO"].ToString().Trim() + "'";
            IdProduto = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            sSql = "SELECT count(*) FROM " + oBancoDados_Destino.BancoDados() + "tb_escalonada_faixa_desconto where COD_EMPREGADO = " + _Funcoes.FNC_NuloString(oRow["COD_EMPREGADO"]) +
                                                                              " and COD_ESCALONADA = " + _Funcoes.FNC_NuloString(oRow["COD_ESCALONADA"]) +
                                                                              " and COD_FAMILIA = " + _Funcoes.FNC_NuloString(oRow["COD_FAMILIA"]) +
                                                                              " and COD_GRUPO = " + _Funcoes.FNC_NuloString(oRow["COD_GRUPO"]) +
                                                                              " and COD_PRODUTO = " + _Funcoes.FNC_NuloString(oRow["COD_PRODUTO"]) +
                                                                              " and NUM_SEQUENCIA_ESCALONADA=" + _Funcoes.FNC_NuloString(oRow["NUM_SEQUENCIA_ESCALONADA"]);

            if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
            {
              sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_escalonada_faixa_desconto" +
                     "(idEmpresa,idProduto,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador," +
                      "PCT_MAXIMO_DESCONTO,QTD_FINAL_ESCALONADA,QTD_INICIAL_ESCALONADA," +
                      "COD_EMPREGADO,COD_ESCALONADA,COD_FAMILIA,COD_GRUPO,COD_PRODUTO,NUM_SEQUENCIA_ESCALONADA)" +
                     " values (?idEmpresa,?idProduto,?Sincronizado," + oBancoDados_Destino.DBSData() + "," +
                              "?verSincronizador,?idSincronizador," +
                              "?PCT_MAXIMO_DESCONTO,?QTD_FINAL_ESCALONADA,?QTD_INICIAL_ESCALONADA," +
                              "?COD_EMPREGADO,?COD_ESCALONADA,?COD_FAMILIA,?COD_GRUPO,?COD_PRODUTO,?NUM_SEQUENCIA_ESCALONADA)";
            }
            else
            {
              sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_escalonada_faixa_desconto" +
                     " set idEmpresa=?idEmpresa," +
                          "idProduto=?idProduto," +
                          "Sincronizado=?Sincronizado," +
                          "verSincronizador=?verSincronizador," +
                          "idSincronizador=?idSincronizador," +
                          "PCT_MAXIMO_DESCONTO=?PCT_MAXIMO_DESCONTO," +
                          "QTD_FINAL_ESCALONADA=?QTD_FINAL_ESCALONADA," +
                          "QTD_INICIAL_ESCALONADA=?QTD_INICIAL_ESCALONADA" +
                      " where COD_EMPREGADO=?COD_EMPREGADO" +
                        " and COD_ESCALONADA=?COD_ESCALONADA" +
                        " and COD_FAMILIA=?COD_FAMILIA" +
                        " and COD_GRUPO=?COD_GRUPO" +
                        " and COD_PRODUTO=?COD_PRODUTO" +
                        " and NUM_SEQUENCIA_ESCALONADA=?NUM_SEQUENCIA_ESCALONADA";
            }

            oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idProduto", Valor = IdProduto, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?PCT_MAXIMO_DESCONTO", Valor = Convert.ToDouble(oRow["PCT_MAXIMO_DESCONTO"]), Tipo = DbType.Double },
                                                                              new clsCampo { Nome = "?QTD_FINAL_ESCALONADA", Valor = Convert.ToDouble(oRow["QTD_FINAL_ESCALONADA"]), Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?QTD_INICIAL_ESCALONADA", Valor = Convert.ToDouble(oRow["QTD_INICIAL_ESCALONADA"]), Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?COD_EMPREGADO", Valor = oRow["COD_EMPREGADO"].ToString().Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?COD_ESCALONADA", Valor = oRow["COD_ESCALONADA"].ToString().Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?COD_FAMILIA", Valor = oRow["COD_FAMILIA"].ToString().Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?COD_GRUPO", Valor = oRow["COD_GRUPO"].ToString().Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?COD_PRODUTO", Valor = oRow["COD_PRODUTO"].ToString().Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?NUM_SEQUENCIA_ESCALONADA", Valor = Convert.ToDouble(oRow["NUM_SEQUENCIA_ESCALONADA"]), Tipo = DbType.Int32 }});
          }

          bRet = true;
        }
      }
      catch (Exception E)
      {
        sStatus = "ERRO";
        sErro = E.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }

    public class PDV_ESCALONADA
    {
      public int COD_ESCALONADA { get; set; }
      public int COD_PDV { get; set; }
    }

    public class OlaPDV_PDV_ESCALONADA_Root
    {
      public List<PDV_ESCALONADA> PDV_ESCALONADA { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    private static bool OlaPDV_Escalonada(string sAplicativo,
                                          string sPartner,
                                          string sCdServico,
                                          string sTarefa,
                                          int nr_ordem_execucao,
                                          int iIdEmpresa,
                                          int iIdTarefa,
                                          long iGrupoTarefas,
                                          string sConexao,
                                          string sUsuario,
                                          string sSenha,
                                          string sCOD_PUXADA,
                                          string sTIPO_REGISTRO,
                                          string sTIPO_CONSULTA,
                                          string sDS_STRINGCONEXAODESTINO,
                                          string sTP_BANCODADOSDESTINO,
                                          string sDS_Origem,
                                          string sDS_Destino,
                                          ref string sJsonRet)
    {
      bool bRet = false;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int iEmpresa = 0;
      string sEDI_Integracao;
      int iCont = 0;
      int idEntidade = 0;
      int idEscalonadaConfiguracao = 0;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      UltimoLocalErro = "FlagWZ_DisLog_Escalonada [" + ProductVersion + "]";

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      UltimoLocalErro = UltimoLocalErro + " - WebRequest.Create - " + sConexao;
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        OlaPDV_PDV_ESCALONADA_Root OlaPDV_PDV_ESCALONADA_Root;

        UltimoLocalErro = UltimoLocalErro + " - POST";
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        UltimoLocalErro = UltimoLocalErro + " - StreamWriter(request.GetRequestStream())";
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        UltimoLocalErro = UltimoLocalErro + " - (HttpWebResponse)request.GetResponse()";
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              sJsonRet = content;
              OlaPDV_PDV_ESCALONADA_Root = JsonConvert.DeserializeObject<OlaPDV_PDV_ESCALONADA_Root>(content);
            }

            if (OlaPDV_PDV_ESCALONADA_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = OlaPDV_PDV_ESCALONADA_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);
        UltimoLocalErro = UltimoLocalErro + " - oBancoDados.DBConectar (" + sTP_BANCODADOSDESTINO + "-" + sDS_STRINGCONEXAODESTINO + ")";

        UltimoLocalErro = UltimoLocalErro + " - OlaPDV_PrecoProduto_TabelaPrecoProduto_Root";

        if (OlaPDV_PDV_ESCALONADA_Root != null)
        {
          if (OlaPDV_PDV_ESCALONADA_Root.PDV_ESCALONADA.Count > 0 && oBancoDados_Destino.DBConectado())
          {
            iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("SELECT idEmpresa FROM " + oBancoDados_Destino.BancoDados() + "tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_entidade_escalonada" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);

            foreach (PDV_ESCALONADA PDV_ESCALONADA in OlaPDV_PDV_ESCALONADA_Root.PDV_ESCALONADA)
            {
              iCont++;
              Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, OlaPDV_PDV_ESCALONADA_Root.PDV_ESCALONADA.Count);

              sSql = "select idEntidade from olapdv.tb_entidade where idEmpresa = " + iEmpresa.ToString() + " and edi_integracao = '" + PDV_ESCALONADA.COD_PDV + "'";
              idEntidade = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              sSql = "select IdEscalonadaConfiguracao from olapdv.tb_escalonada_configuracao where idEmpresa = " + iEmpresa.ToString() + " and COD_ESCALONADA = '" + PDV_ESCALONADA.COD_ESCALONADA.ToString() + "'";
              idEscalonadaConfiguracao = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              sEDI_Integracao = PDV_ESCALONADA.COD_ESCALONADA.ToString() + ":" + PDV_ESCALONADA.COD_PDV.ToString();

              sSql = "SELECT count(*) FROM " + oBancoDados_Destino.BancoDados() + "tb_entidade_escalonada where idEmpresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + sEDI_Integracao.Trim() + "'";

              if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
              {
                sSql = "insert into " + oBancoDados_Destino.BancoDados() + "tb_entidade_escalonada " +
                    "           (idEntidade,idEscalonadaConfiguracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador,idEmpresa,EDI_Integracao)" +
                       " values (?idEntidade,?idEscalonadaConfiguracao,?Sincronizado,current_timestamp,?verSincronizador,?idSincronizador,?idEmpresa,?EDI_Integracao)";
              }
              else
              {
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_entidade_escalonada" +
                       " set idEntidade=?idEntidade," +
                            "idEscalonadaConfiguracao=?idEscalonadaConfiguracao," +
                            "Sincronizado=?Sincronizado," +
                            "dtSincronizacao=current_timestamp," +
                            "verSincronizador=?verSincronizador," +
                            "idSincronizador=?idSincronizador" +
                       " where idEmpresa=?idEmpresa" +
                         " and EDI_Integracao=?EDI_Integracao";
              }

              oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEntidade", Valor = idEntidade, Tipo = DbType.Int32 },
                                                                    new clsCampo { Nome = "?idEscalonadaConfiguracao", Valor = idEscalonadaConfiguracao, Tipo = DbType.Int32 },
                                                                    new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                    new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                    new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                    new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                    new clsCampo { Nome = "?EDI_Integracao", Valor = sEDI_Integracao.Trim(), Tipo = DbType.String }});
            }

            bRet = true;
          }
        }
      }
      catch (Exception Ex)
      {
        sStatus = "ERRO";
        sErro = Ex.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }
    private static bool OlaPDV_Estoque(string sAplicativo,
                                       string sPartner,
                                       string sCdServico,
                                       string sTarefa,
                                       int nr_ordem_execucao,
                                       int iIdEmpresa,
                                       int iIdTarefa,
                                       long iGrupoTarefas,
                                       string sConexao,
                                       string sUsuario,
                                       string sSenha,
                                       string sCOD_PUXADA,
                                       string sTIPO_REGISTRO,
                                       string sTIPO_CONSULTA,
                                       string sDS_STRINGCONEXAODESTINO,
                                       string sTP_BANCODADOSDESTINO,
                                       string sDS_Origem,
                                       string sDS_Destino,
                                       ref string sJsonRet)
    {
      bool bRet = false;
      DataTable oData = null;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int iEmpresa = 0;
      int iCont = 0;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      try
      {
        oData = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico,
                                                ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa, sDS_Origem, sConexao,
                                                sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, "", ref sJsonRet);

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

        if (oData != null && oBancoDados_Destino.DBConectado())
        {
          iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("SELECT idEmpresa FROM " + oBancoDados_Destino.BancoDados() + "tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

          sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_produtos" +
                 " set idStatusEstoque=1" +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);

          foreach (DataRow oRow in oData.Rows)
          {
            iCont++;
            Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, oData.Rows.Count);

            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_produtos" +
                    " set idStatusEstoque=0," +
                         "Estoque=?Estoque" +
                    " where idEmpresa=?idEmpresa" +
                      " and EDI_Integracao=?EDI_Integracao";
            oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?Estoque", Valor = Convert.ToDouble(oRow["QTD_ESTOQUE"]), Tipo = DbType.Double },
                                                                              new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_PRODUTO_INTERNO"].ToString().Trim(), Tipo = DbType.String }});
          }

          sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_produtos set Estoque = 0 where idEmpresa = " + iEmpresa.ToString() + " and idStatusEstoque=1";
          oBancoDados_Destino.DBExecutar(sSql);

          bRet = true;
        }
      }
      catch (Exception E)
      {
        sStatus = "ERRO";
        sErro = E.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }

    class TABELA_PRECO_PRODUTO
    {
      public int COD_PRODUTO_INTERNO { get; set; }
      public int COD_TABELA_PRECO { get; set; }
      public int IND_MULTIPLO { get; set; }
      public double PCT_MAXIMO_DESCONTO_TABPRE { get; set; }
      public double QTD_MAXIMA_PEDIDA { get; set; }
      public double QTD_MINIMA_PEDIDA { get; set; }
      public double VLR_FINAL_PRODUTO { get; set; }
    }

    class OlaPDV_PrecoProduto_TabelaPrecoProduto_Root
    {
      public List<TABELA_PRECO_PRODUTO> TABELA_PRECO_PRODUTO { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    private static bool OlaPDV_PrecoProduto(string sAplicativo,
                                            string sPartner,
                                            string sCdServico,
                                            string sTarefa,
                                            int nr_ordem_execucao,
                                            int iIdEmpresa,
                                            int iIdTarefa,
                                            long iGrupoTarefas,
                                            string sConexao,
                                            string sUsuario,
                                            string sSenha,
                                            string sCOD_PUXADA,
                                            string sTIPO_REGISTRO,
                                            string sTIPO_CONSULTA,
                                            string sDS_STRINGCONEXAODESTINO,
                                            string sTP_BANCODADOSDESTINO,
                                            string sDS_Origem,
                                            string sDS_Destino,
                                            ref string sJsonRet)
    {
      bool bRet = false;
      DataTable oDataAux = null;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int iEmpresa = 0;
      int idTabelaPreco = 0;
      int idProduto = 0;
      Int64 idProtocolo = 0;
      DateTime hoje = DateTime.Now;
      double Fator_Venda = 0;
      int TabelaPadrao = 0;
      int iQTD_MINIMA_PEDIDA = 0;
      string tpExportaTabelaPreco = "N";
      int iCont = 0;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      UltimoLocalErro = "FlagWZ_DisLog_Taxa_Ocupacao [" + ProductVersion + "]";

      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      UltimoLocalErro = UltimoLocalErro + " - WebRequest.Create - " + sConexao;
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        OlaPDV_PrecoProduto_TabelaPrecoProduto_Root OlaPDV_PrecoProduto_TabelaPrecoProduto_Root;

        UltimoLocalErro = UltimoLocalErro + " - POST";
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                       "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                       "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        UltimoLocalErro = UltimoLocalErro + " - StreamWriter(request.GetRequestStream())";
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        UltimoLocalErro = UltimoLocalErro + " - (HttpWebResponse)request.GetResponse()";
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              OlaPDV_PrecoProduto_TabelaPrecoProduto_Root = JsonConvert.DeserializeObject<OlaPDV_PrecoProduto_TabelaPrecoProduto_Root>(content);
            }

            if (OlaPDV_PrecoProduto_TabelaPrecoProduto_Root.codigo == 0)
            {
              sStatus = "Executado";
            }
            else
            {
              sStatus = "ERRO";
            }

            sErro = OlaPDV_PrecoProduto_TabelaPrecoProduto_Root.codigo.ToString();

            ts = DateTime.Now - dUtil;
            TempoExecucaoAPI = ts.TotalSeconds;
          }
        }

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);
        UltimoLocalErro = UltimoLocalErro + " - oBancoDados.DBConectar (" + sTP_BANCODADOSDESTINO + "-" + sDS_STRINGCONEXAODESTINO + ")";

        idProtocolo = Convert.ToInt64(hoje.Year.ToString() +
                                      hoje.Month.ToString().PadLeft(2, '0') +
                                      hoje.Day.ToString().PadLeft(2, '0') +
                                      hoje.Hour.ToString().PadLeft(2, '0') +
                                      hoje.Minute.ToString().PadLeft(2, '0') +
                                      hoje.Second.ToString().PadLeft(2, '0'));

        iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("SELECT idEmpresa FROM " + oBancoDados_Destino.BancoDados() + "tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

        sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos" +
               " set StatusPreco=0" +
                   ",Sincronizado = 'N'" +
                   ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                   ",verSincronizador='" + ProductVersion + "'" +
                   ",idSincronizador=" + Config_App.idIntegrador.ToString() +
               " where idEmpresa = " + iEmpresa.ToString();
        oBancoDados_Destino.DBExecutar(sSql);

        UltimoLocalErro = UltimoLocalErro + " - OlaPDV_PrecoProduto_TabelaPrecoProduto_Root";

        if (OlaPDV_PrecoProduto_TabelaPrecoProduto_Root != null)
        {
          if (OlaPDV_PrecoProduto_TabelaPrecoProduto_Root.TABELA_PRECO_PRODUTO.Count > 0)
          {
            if (!oBancoDados.DBConectado())
            {
              UltimoLocalErro = UltimoLocalErro + " - !oBancoDados.DBConectado()";
            }

            foreach (TABELA_PRECO_PRODUTO TabelaPrecoProduto in OlaPDV_PrecoProduto_TabelaPrecoProduto_Root.TABELA_PRECO_PRODUTO)
            {
              iCont++;
              Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, OlaPDV_PrecoProduto_TabelaPrecoProduto_Root.TABELA_PRECO_PRODUTO.Count);

              sSql = "select * from " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos where idEmpresa = " + iEmpresa.ToString() + " and edi_integracao = '" + TabelaPrecoProduto.COD_TABELA_PRECO.ToString().Trim().PadLeft(2, '0') + "'";
              oDataAux = oBancoDados_Destino.DBQuery(sSql, 0);

              if (oDataAux != null)
              {
                if (oDataAux.Rows.Count == 0)
                {
                  idTabelaPreco = 0;
                }
                else
                {
                  tpExportaTabelaPreco = oDataAux.Rows[0]["tpExportaTabelaPreco"].ToString();
                  idTabelaPreco = Convert.ToInt32(oDataAux.Rows[0]["idTabelaPreco"]);
                  TabelaPadrao = Convert.ToInt32(oDataAux.Rows[0]["TabelaPadrao"]);
                }
              }

              if (Convert.ToInt32(TabelaPrecoProduto.COD_PRODUTO_INTERNO) >= 8200)
              {
                tpExportaTabelaPreco = tpExportaTabelaPreco;
              }

              if (tpExportaTabelaPreco == "S")
              {
                sSql = "select * from " + oBancoDados_Destino.BancoDados() + "tb_produtos where idEmpresa = " + iEmpresa.ToString() + " and codigo = '" + TabelaPrecoProduto.COD_PRODUTO_INTERNO.ToString().Trim() + "'";
                oDataAux = oBancoDados_Destino.DBQuery(sSql, 0);

                if (oDataAux != null)
                {
                  if (oDataAux.Rows.Count == 0)
                  {
                    idProduto = 0;
                  }
                  else
                  {
                    idProduto = Convert.ToInt32(oDataAux.Rows[0]["idProduto"]);
                    Fator_Venda = Convert.ToDouble(oDataAux.Rows[0]["Fator_Venda"]);
                  }
                }

                sSql = "select count(*) from " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos where idEmpresa = " + iEmpresa.ToString() +
                                                                                                               " and idTabelaPreco = " + idTabelaPreco.ToString() +
                                                                                                               " and idProduto = " + idProduto.ToString() +
                                                                                                               " and nroFaixa = 0" +
                                                                                                               " and EDI_Integracao = '" + TabelaPrecoProduto.COD_PRODUTO_INTERNO.ToString().Trim() + "' limit 1";

                if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
                {
                  sSql = "insert into " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos" +
                         "(idTabelaPrecoFaixa,PrecoCusto,PrecoMinimo,PrecoVenda,PrecoVendaUnitario,PrecoExibicaoUnitario,ValorPromocional," +
                          "PrecoFaixaInicial,PrecoFaixaFinal,AgrupadorPreco,StatusPreco,dtAtualizacao,VersaoPreco,idProtocolo,IND_MULTIPLO," +
                          "QTD_MAXIMA_PEDIDA,QTD_MINIMA_PEDIDA,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador," +
                          "idEmpresa,idTabelaPreco,idProduto,nroFaixa,EDI_Integracao)" +
                         " values " +
                         "(?idTabelaPrecoFaixa,?PrecoCusto,?PrecoMinimo,?PrecoVenda,?PrecoVendaUnitario,?PrecoExibicaoUnitario,?ValorPromocional," +
                          "?PrecoFaixaInicial,?PrecoFaixaFinal,?AgrupadorPreco,?StatusPreco,?dtAtualizacao,?VersaoPreco,?idProtocolo,?IND_MULTIPLO," +
                          "?QTD_MAXIMA_PEDIDA,?QTD_MINIMA_PEDIDA,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador," +
                          "?idEmpresa,?idTabelaPreco,?idProduto,?nroFaixa,?EDI_Integracao)";
                }
                else
                {
                  sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos" +
                          " set idTabelaPrecoFaixa=?idTabelaPrecoFaixa," +
                               "PrecoCusto=?PrecoCusto," +
                               "PrecoMinimo=?PrecoMinimo," +
                               "PrecoVenda=?PrecoVenda," +
                               "PrecoVendaUnitario=?PrecoVendaUnitario," +
                               "PrecoExibicaoUnitario=?PrecoExibicaoUnitario," +
                               "ValorPromocional=?ValorPromocional," +
                               "PrecoFaixaInicial=?PrecoFaixaInicial," +
                               "PrecoFaixaFinal=?PrecoFaixaFinal," +
                               "AgrupadorPreco=?AgrupadorPreco," +
                               "StatusPreco=?StatusPreco," +
                               "dtAtualizacao=?dtAtualizacao," +
                               "VersaoPreco=?VersaoPreco," +
                               "idProtocolo=?idProtocolo," +
                               "IND_MULTIPLO=?IND_MULTIPLO," +
                               "QTD_MAXIMA_PEDIDA=?QTD_MAXIMA_PEDIDA," +
                               "QTD_MINIMA_PEDIDA=?QTD_MINIMA_PEDIDA," +
                               "Sincronizado=?Sincronizado," +
                               "dtSincronizacao=" + oBancoDados_Destino.DBSData() + "," +
                               "verSincronizador=?verSincronizador," +
                               "idSincronizador=?idSincronizador" +
                          " where idEmpresa = ?idEmpresa" +
                            " and idTabelaPreco = ?idTabelaPreco" +
                            " and idProduto = ?idProduto" +
                            " and nroFaixa=?nroFaixa" +
                            " and EDI_Integracao=?EDI_Integracao";
                }

                if (idTabelaPreco != 0 && idProduto != 0)
                {
                  if (_Funcoes.FNC_NuloZero(Convert.ToDouble(TabelaPrecoProduto.QTD_MINIMA_PEDIDA)) == 0)
                  {
                    iQTD_MINIMA_PEDIDA = 1;
                  }
                  else
                  {
                    iQTD_MINIMA_PEDIDA = Convert.ToInt32(TabelaPrecoProduto.QTD_MINIMA_PEDIDA);
                  }

                  oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idTabelaPrecoFaixa", Valor = idTabelaPreco, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?PrecoCusto", Valor = 0, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?PrecoMinimo", Valor = TabelaPrecoProduto.VLR_FINAL_PRODUTO, Tipo = DbType.Double },
                                                                                          new clsCampo { Nome = "?PrecoVenda", Valor = TabelaPrecoProduto.VLR_FINAL_PRODUTO, Tipo = DbType.Double },
                                                                                          new clsCampo { Nome = "?PrecoVendaUnitario", Valor = Convert.ToDouble(TabelaPrecoProduto.VLR_FINAL_PRODUTO) / Fator_Venda, Tipo = DbType.Double },
                                                                                          new clsCampo { Nome = "?PrecoExibicaoUnitario", Valor = (Convert.ToDouble(TabelaPrecoProduto.VLR_FINAL_PRODUTO) / Fator_Venda).ToString("0.00"), Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "?ValorPromocional", Valor = 0, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?PrecoFaixaInicial", Valor = 0, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?PrecoFaixaFinal", Valor = 0, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?PrecoExibicao", Valor = Convert.ToDouble(TabelaPrecoProduto.VLR_FINAL_PRODUTO).ToString("0.00"), Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "?AgrupadorPreco", Valor = idProduto.ToString(), Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "?StatusPreco", Valor = 1, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?dtAtualizacao", Valor = DateTime.Now, Tipo = DbType.DateTime },
                                                                                          new clsCampo { Nome = "?VersaoPreco", Valor = 0, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?idProtocolo", Valor = idProtocolo, Tipo = DbType.Int64 },
                                                                                          new clsCampo { Nome = "?IND_MULTIPLO", Valor = Convert.ToDouble(TabelaPrecoProduto.IND_MULTIPLO), Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "?QTD_MAXIMA_PEDIDA", Valor = Convert.ToDouble(TabelaPrecoProduto.QTD_MAXIMA_PEDIDA), Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "?QTD_MINIMA_PEDIDA", Valor = iQTD_MINIMA_PEDIDA, Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?idTabelaPreco", Valor = idTabelaPreco, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?idProduto", Valor = idProduto, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?nroFaixa", Valor = 0, Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?EDI_Integracao", Valor = TabelaPrecoProduto.COD_PRODUTO_INTERNO.ToString().Trim(), Tipo = DbType.String }});
                }
              }
            }

            //if (TabelaPadrao == 1)
            //{
            //    sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_produtos" +
            //           " inner join " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos on tb_tabela_precos_produtos.idProduto = tb_produtos.idProduto" +
            //           " set tb_produtos.Preco1Venda = tb_tabela_precos_produtos.precovenda," +
            //                "tb_produtos.Preco2VendaUnitario = tb_tabela_precos_produtos.PrecoVendaUnitario," +
            //                "tb_produtos.Custo = 0," +
            //                "tb_produtos.CustoUnitario = 0," +
            //                "tb_produtos.PrecoVendaUnitarioExib = tb_tabela_precos_produtos.PrecoExibicaoUnitario," +
            //                "tb_produtos.Sincronizado='S'," +
            //                "tb_produtos.dtSincronizacao=" + oBancoDados_Destino.DBSData() + "," +
            //                "tb_produtos.verSincronizador='" + ProductVersion + "'," +
            //                "tb_produtos.idSincronizador=" + Config_App.idIntegrador.ToString() +
            //           " where idTabelaPreco = " + idTabelaPreco.ToString();
            //    oBancoDados_Destino.DBExecutar(sSql);
            //}

            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos_produtos" +
                   " inner join " + oBancoDados_Destino.BancoDados() + "tb_produtos on tb_produtos.idProduto = tb_tabela_precos_produtos.idProduto" +
                    " set tb_tabela_precos_produtos.PrecoCusto = 0 " +
                       ", tb_tabela_precos_produtos.idTabelaPrecoFaixa = tb_tabela_precos_produtos.idTabelaPreco" +
                       ", tb_tabela_precos_produtos.StatusPreco = 1" +
                       ", tb_tabela_precos_produtos.PrecoMinimo = tb_tabela_precos_produtos.PrecoVenda" +
                       ", tb_tabela_precos_produtos.PrecoExibicao = Concat('R$ ', Replace(Replace(Replace(Format(tb_tabela_precos_produtos.PrecoVenda, 2), '.', '|'), ',', '.'), '|', ','))" +
                       ", tb_tabela_precos_produtos.PrecoVendaUnitario = round(tb_tabela_precos_produtos.PrecoVenda / tb_produtos.Fator_Venda, 2)" +
                       ", tb_tabela_precos_produtos.PrecoExibicaoUnitario = Concat('R$ ', Replace(Replace(Replace(Format(tb_tabela_precos_produtos.PrecoVenda / tb_produtos.Fator_Venda, 2), '.', '|'), ',', '.'), '|', ','))" + ", tb_tabela_precos_produtos.dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                   " where tb_tabela_precos_produtos.idProduto > 0 and tb_tabela_precos_produtos.Sincronizado='S'";
            oBancoDados_Destino.DBExecutar(sSql);

            //sSql = "update tb_produtos" +
            //       " inner join tb_tabela_precos_produtos on tb_produtos.idProduto = tb_tabela_precos_produtos.idProduto" +
            //       " inner join tb_tabela_precos on tb_tabela_precos_produtos.idTabelaPreco = tb_tabela_precos_produtos.idTabelaPreco" +
            //        " set tb_produtos.Preco1Venda = tb_tabela_precos_produtos.PrecoVenda" +
            //            ",tb_produtos.Preco2VendaUnitario = tb_tabela_precos_produtos.PrecoVendaUnitario" +
            //            ",Custo = 0, CustoUnitario = 0" +
            //            ",tb_produtos.PrecoVendaUnitarioExib = Concat('R$ ', Replace(Replace(Replace(Format(tb_tabela_precos_produtos.PrecoVendaUnitario, 2), '.', '|'), ',', '.'), '|', ','))" +
            //            ",tb_produtos.Sincronizado='S'" +
            //            ",tb_produtos.dtSincronizacao=" + oBancoDados_Destino.DBSData() +
            //            ",tb_produtos.verSincronizador='" + ProductVersion + "'" +
            //            ",tb_produtos.idSincronizador=" + Config_App.idIntegrador.ToString() +
            //       " where tb_tabela_precos_produtos.idProduto > 0" +
            //         " and tb_tabela_precos.TabelaPadrao = 1";
            //oBancoDados_Destino.DBExecutar(sSql);

            bRet = true;

            UltimoLocalErro = UltimoLocalErro + " - DBDesconectar";
            oBancoDados.DBDesconectar();

            bRet = true;
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlagWZ_DisLog_Taxa_Ocupacao [" + sCOD_PUXADA + " >> " + Ex.Message + "]");
        bRet = false;

        sStatus = "ERRO";
        sErro = Ex.Message;
      }
      finally
      {
        request = null;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }

    private static bool OlaPDV_TabelaPreco(string sAplicativo,
                                           string sPartner,
                                           string sCdServico,
                                           string sTarefa,
                                           int nr_ordem_execucao,
                                           int iIdEmpresa,
                                           int iIdTarefa,
                                           long iGrupoTarefas,
                                           string sConexao,
                                           string sUsuario,
                                           string sSenha,
                                           string sCOD_PUXADA,
                                           string sTIPO_REGISTRO,
                                           string sTIPO_CONSULTA,
                                           string sDS_STRINGCONEXAODESTINO,
                                           string sTP_BANCODADOSDESTINO,
                                           string sDS_Origem,
                                           string sDS_Destino,
                                           ref string sJsonRet)
    {
      bool bRet = false;
      DataTable oData = null;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int TabelaPadrao = 0;
      int iEmpresa = 0;
      int iCont = 0;

      string sEDI_Integracao = "";

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      try
      {
        oData = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico,
                                                ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa, sDS_Origem, sConexao,
                                                sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, "", ref sJsonRet);

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

        if (oData != null && oBancoDados_Destino.DBConectado())
        {
          iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("select idEmpresa from " + oBancoDados_Destino.BancoDados() + "tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

          sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);

          sSql = "select count(*) from " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos where idEmpresa = " + iEmpresa.ToString();

          if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
            TabelaPadrao = 1;

          foreach (DataRow oRow in oData.Rows)
          {
            iCont++;
            Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, oData.Rows.Count);

            sEDI_Integracao = oRow["COD_TABELA_PRECO"].ToString().Trim().PadLeft(2, '0');

            sSql = "SELECT count(*) FROM " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos where idempresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + sEDI_Integracao.Trim() + "'";

            if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
            {
              sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos" +
                     "(TabelaPreco,TabelaPadrao,Sincronizado,dtSincronizacao," +
                      "verSincronizador,idSincronizador,idEmpresa,EDI_Integracao)" +
                     " VALUES " +
                     "(?TabelaPreco," + TabelaPadrao.ToString() + ",?Sincronizado," + oBancoDados_Destino.DBSData() + "," +
                      "?verSincronizador,?idSincronizador,?idEmpresa,?EDI_Integracao)";
            }
            else
            {
              sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos" +
                     " set TabelaPreco=?TabelaPreco," +
                          "Sincronizado=?Sincronizado," +
                          "dtSincronizacao=" + oBancoDados_Destino.DBSData() + "," +
                          "verSincronizador=?verSincronizador," +
                          "idSincronizador=?idSincronizador" +
                     " where idEmpresa=?idEmpresa" +
                       " and EDI_Integracao=?EDI_Integracao";
            }

            oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?TabelaPreco", Valor = oRow["DSC_TABELA_PRECO"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?EDI_Integracao", Valor = sEDI_Integracao.Trim(), Tipo = DbType.String }});
            TabelaPadrao = 0;
          }

          bRet = true;
        }
      }
      catch (Exception E)
      {
        sStatus = "ERRO";
        sErro = E.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }

    private static bool OlaPDV_Vendedor(string sAplicativo,
                                        string sPartner,
                                        string sCdServico,
                                        string sTarefa,
                                        int nr_ordem_execucao,
                                        int iIdEmpresa,
                                        int iIdTarefa,
                                        long iGrupoTarefas,
                                        string sConexao,
                                        string sUsuario,
                                        string sSenha,
                                        string sCOD_PUXADA,
                                        string sTIPO_REGISTRO,
                                        string sTIPO_CONSULTA,
                                        string sDS_STRINGCONEXAODESTINO,
                                        string sTP_BANCODADOSDESTINO,
                                        string sDS_Origem,
                                        string sDS_Destino,
                                        ref string sJsonRet)
    {
      bool bRet = false;
      DataTable oData = null;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int VendedorPadrao = 0;
      int iEmpresa = 0;
      int idVendedor = 0;
      int idGrupoProdutos = 0;
      int iCont = 0;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      try
      {
        oData = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico,
                                                ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa, sDS_Origem, sConexao,
                                                sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, "", ref sJsonRet);

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

        if (oData != null && oBancoDados_Destino.DBConectado())
        {
          iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("select idEmpresa from " + oBancoDados_Destino.BancoDados() + "tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

          sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_vendedores" +
                " set Sincronizado = 'N'" +
                    ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                    ",verSincronizador='" + ProductVersion + "'" +
                    ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);
          sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_vendedores_grupoprodutos" +
                " set Sincronizado = 'N'" +
                    ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                    ",verSincronizador='" + ProductVersion + "'" +
                    ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                " where idVendedor in (select idVendedor from tb_vendedores where idEmpresa = " + iEmpresa.ToString() + ")";
          oBancoDados_Destino.DBExecutar(sSql);

          sSql = "select count(*) from " + oBancoDados_Destino.BancoDados() + "tb_vendedores where idEmpresa = " + iEmpresa.ToString();

          if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
            VendedorPadrao = 1;

          foreach (DataRow oRow in oData.Rows)
          {
            iCont++;
            Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, oData.Rows.Count);

            sSql = "SELECT count(*) FROM " + oBancoDados_Destino.BancoDados() + "tb_vendedores where idempresa = " + iEmpresa.ToString() + " and Integracao_EDI = '" + oRow["COD_VENDEDOR"].ToString().Trim() + "'";

            if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
            {
              sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_vendedores" +
                     "(CodigoVendedor,Vendedor,Apelido,TelefoneVendedor,VendedorPadrao,Sincronizado," +
                      "dtSincronizacao,verSincronizador,idSincronizador,idEmpresa,Integracao_EDI)" +
                     " values " +
                     "(?CodigoVendedor,?Vendedor,?Apelido,?TelefoneVendedor," + VendedorPadrao.ToString() + ",?Sincronizado," +
                       oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador,?idEmpresa,?Integracao_EDI)";
            }
            else
            {
              sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_vendedores" +
                     " set CodigoVendedor=?CodigoVendedor," +
                          "Vendedor=?Vendedor," +
                          "Apelido=?Apelido," +
                          "TelefoneVendedor=?TelefoneVendedor," +
                          "Sincronizado=?Sincronizado," +
                          "dtSincronizacao=" + oBancoDados_Destino.DBSData() + "," +
                          "verSincronizador=?verSincronizador," +
                          "idSincronizador=?idSincronizador" +
                     " where idEmpresa=?idEmpresa" +
                       " and Integracao_EDI=?Integracao_EDI";
            }

            oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?CodigoVendedor", Valor = oRow["COD_VENDEDOR"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Vendedor", Valor = oRow["NOM_VENDEDOR"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Apelido", Valor = oRow["NOM_VENDEDOR"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?TelefoneVendedor", Valor = oRow["NUM_TELEFONE"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?Integracao_EDI", Valor = oRow["COD_VENDEDOR"].ToString().Trim(), Tipo = DbType.String }});

            sSql = "SELECT idVendedor FROM " + oBancoDados_Destino.BancoDados() + "tb_vendedores where idempresa = " + iEmpresa.ToString() + " and Integracao_EDI = '" + oRow["COD_VENDEDOR"].ToString().Trim() + "'";
            idVendedor = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            sSql = "delete from " + oBancoDados_Destino.BancoDados() + "tb_vendedores_grupoprodutos where idVendedor = " + idVendedor.ToString();
            oBancoDados_Destino.DBExecutar(sSql);

            if (oRow["COD_GRUPO_COMERCIALIZACAO"] != null)
            {
              string[] sGrupos = oRow["COD_GRUPO_COMERCIALIZACAO"].ToString().Split(',');

              foreach (string sGrupo in sGrupos)
              {
                sSql = "select idGrupoProdutos from " + oBancoDados_Destino.BancoDados() + "tb_grupoprodutos where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + sGrupo.Trim() + "'";
                idGrupoProdutos = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

                if (idGrupoProdutos != 0)
                {
                  sSql = "insert into " + oBancoDados_Destino.BancoDados() + "tb_vendedores_grupoprodutos" +
                         "(idVendedor, idGrupoProdutos,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                         " values (" + idVendedor.ToString() + "," + idGrupoProdutos.ToString() + ",'S'," + oBancoDados_Destino.DBSData() + ",'" + ProductVersion + "'," + Config_App.idIntegrador.ToString() + ")";
                  oBancoDados_Destino.DBExecutar(sSql);
                }
              }
            }

            VendedorPadrao = 0;
          }

          bRet = true;
        }
      }
      catch (Exception E)
      {
        sStatus = "ERRO";
        sErro = E.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }

    private static bool OlaPDV_Configuracao(string sAplicativo,
                                            string sPartner,
                                            string sCdServico,
                                            string sTarefa,
                                            int nr_ordem_execucao,
                                            int iIdEmpresa,
                                            int iIdTarefa,
                                            long iGrupoTarefas,
                                            string sConexao,
                                            string sUsuario,
                                            string sSenha,
                                            string sCOD_PUXADA,
                                            string sTIPO_REGISTRO,
                                            string sTIPO_CONSULTA,
                                            string sDS_STRINGCONEXAODESTINO,
                                            string sTP_BANCODADOSDESTINO,
                                            string sDS_Origem,
                                            string sDS_Destino,
                                            ref string sJsonRet)
    {
      bool bRet = false;
      DataTable oData = null;
      DataTable oData_CondicaoPadrao = null;

      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int iEmpresa = 0;
      string idTipoEmpresa = "";
      string sCNPJ = "";
      int idTipoEntidade = 0;
      int idEstado = 0;
      int idCidade = 0;
      int Entidade_Ativa = 0;
      int idEntidade = 0;
      int idGrupoEntidade = 0;
      int idEntidadeCategoria = 0;
      int id_condicoes_pagamento = 0;
      int idTabelaPreco = 0;
      int idRotaVenda = 0;
      int idVendedor = 0;
      int iPRIORIDADE_CONDICAO_PAGAMENTO = 0;
      int iCont = 0;

      double ParcelaMinima = 0;
      double MultiplicadorAPI = 0;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      try
      {
        oData = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico,
                                                ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa, sDS_Origem, sConexao,
                                                sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, "", ref sJsonRet);

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

        if (oData != null && oBancoDados_Destino.DBConectado())
        {
          iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico("select idEmpresa from olapdv.tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'", 0));

          sSql = "update olapdv.tb_entidade" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);
          sSql = "update olapdv.tb_entidade_empresa" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where id_empresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);
          sSql = "update olapdv.tb_entidade_endereco" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);
          sSql = "update olapdv.tb_entidade_tabela_preco" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);
          sSql = "update olapdv.tb_perfil" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);
          sSql = "update olapdv.tb_entidade_condicoes_pagamento" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);
          sSql = "update olapdv.tb_grupo_entidade" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);
          sSql = "update olapdv.tb_entidade_categoria" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);
          sSql = "update olapdv.tb_rotavenda" +
                 " set Sincronizado = 'N'" +
                     ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     ",verSincronizador='" + ProductVersion + "'" +
                     ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                 " where idEmpresa = " + iEmpresa.ToString();
          oBancoDados_Destino.DBExecutar(sSql);

          foreach (DataRow oRow in oData.Rows)
          {
            iCont++;
            Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, oData.Rows.Count);

            if (oRow["CNPJ_CPF"].ToString().Trim().Replace(".", "").Replace("-", "").Replace("/", "").Length == 11)
            {
              idTipoEmpresa = "F";
              sCNPJ = Convert.ToUInt64(oRow["CNPJ_CPF"].ToString()).ToString(@"000\.000\.000\-00");
            }
            else
            {
              idTipoEmpresa = "J";
              sCNPJ = Convert.ToUInt64(oRow["CNPJ_CPF"].ToString()).ToString(@"00\.000\.000\/0000\-00");
            }

            sSql = "select idTabelaPreco from " + oBancoDados_Destino.BancoDados() + "tb_tabela_precos where idEmpresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + oRow["TABELA_PRECO"].ToString().Trim().PadLeft(2, '0') + "'";
            idTabelaPreco = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            sSql = "select idPerfil from " + oBancoDados_Destino.BancoDados() + "tb_perfil where idEmpresa=" + iEmpresa.ToString() + " and EDI_Integracao= '" + oRow["COD_CATEGORIA_VAREJO"].ToString().Trim() + "'";
            idTipoEntidade = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            if (idTipoEntidade == 0)
            {
              sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_perfil" +
                     " (idEmpresa,Perfil,PerfilApelido,EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                     " VALUES (?idEmpresa,?Perfil,?PerfilApelido,?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador)";
              oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Perfil", Valor = oRow["COD_CATEGORIA_VAREJO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?PerfilApelido", Valor = oRow["COD_CATEGORIA_VAREJO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_CATEGORIA_VAREJO"].ToString().Trim(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

              sSql = "select idPerfil from " + oBancoDados_Destino.BancoDados() + "tb_perfil where idEmpresa=" + iEmpresa.ToString() + " and EDI_Integracao= '" + oRow["COD_CATEGORIA_VAREJO"].ToString().Trim() + "'";
              idTipoEntidade = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
            }

            sSql = "SELECT idEstado FROM " + oBancoDados_Destino.BancoDados() + "tb_estado where trim(SiglaEstado) = '" + oRow["UF"].ToString().Trim() + "'";
            idEstado = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            sSql = "SELECT idCidade FROM " + oBancoDados_Destino.BancoDados() + "tb_cidade where idEstado = " + idEstado.ToString() + " and upper(trim(Cidade)) = '" + oRow["CIDADE"].ToString().Trim() + "'";
            idCidade = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            if (oRow["IND_ATIVO"].ToString() == "A")
            { Entidade_Ativa = 1; }
            else
            { Entidade_Ativa = 0; }

            sSql = "SELECT count(*) FROM " + oBancoDados_Destino.BancoDados() + "tb_entidade where idempresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + oRow["COD_PDV"].ToString().Trim() + "'";

            if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0)) == 0)
            {
              sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_entidade" +
                     "(idTipoEmpresa,idTipoEntidade,idEstado,idCidade,CNPJ,CNPJNumerico,Entidade,Fantasia,Endereco,Bairro,cidade," +
                      "UF,idPais,Cliente,CEP,idStatusEntidade,idOrigemEntidade,idStatusIntegracaoEntidade,CliLatitude,CliLongitude," +
                      "Entidade_Ativa,Sincronizado,verSincronizador,idSincronizador,idEmpresa,EDI_Integracao,dtSincronizacao)" +
                     " values " +
                     "(?idTipoEmpresa,?idTipoEntidade,?idEstado,?idCidade,?CNPJ,?CNPJNumerico,?Entidade,?Fantasia,?Endereco,?Bairro,?cidade," +
                      "?UF,?idPais,?Cliente,?CEP,?idStatusEntidade,?idOrigemEntidade,?idStatusIntegracaoEntidade,?CliLatitude,?CliLongitude," +
                      "Entidade_Ativa,?Sincronizado,?verSincronizador,?idSincronizador,?idEmpresa,?EDI_Integracao," + oBancoDados_Destino.DBSData() + ")";
            }
            else
            {
              sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_entidade" +
                     " set idTipoEmpresa=?idTipoEmpresa," +
                          "idTipoEntidade=?idTipoEntidade," +
                          "idEstado=?idEstado," +
                          "idCidade=?idCidade," +
                          "CNPJ=?CNPJ," +
                          "CNPJNumerico=?CNPJNumerico," +
                          "Entidade=?Entidade," +
                          "Fantasia=?Fantasia," +
                          "Endereco=?Endereco," +
                          "Bairro=?Bairro," +
                          "cidade=?cidade," +
                          "UF=?UF," +
                          "idPais=?idPais," +
                          "Cliente=?Cliente," +
                          "CEP=?CEP," +
                          "idStatusEntidade=?idStatusEntidade," +
                          "idOrigemEntidade=?idOrigemEntidade," +
                          "idStatusIntegracaoEntidade=?idStatusIntegracaoEntidade," +
                          "CliLatitude=?CliLatitude," +
                          "CliLongitude=?CliLongitude," +
                          "Entidade_Ativa=?Entidade_Ativa," +
                          "Sincronizado=?Sincronizado," +
                          "verSincronizador=?verSincronizador," +
                          "idSincronizador=?idSincronizador," +
                          "dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                     " where idEmpresa=?idEmpresa" +
                       " and EDI_Integracao=?EDI_Integracao";
            }

            oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idTipoEmpresa", Valor = idTipoEmpresa, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idTipoEntidade", Valor = idTipoEntidade, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idEstado", Valor = idEstado, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idCidade", Valor = idCidade, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?CNPJ", Valor = sCNPJ, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?CNPJNumerico", Valor = oRow["CNPJ_CPF"].ToString().Replace(".","").Replace("-", "").Replace("/","").Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Entidade", Valor = oRow["RAZAO_SOCIAL"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Fantasia", Valor = oRow["NOME_FANTASIA"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Endereco", Valor = oRow["ENDERECO"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Bairro", Valor = oRow["BAIRRO"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?cidade", Valor = oRow["CIDADE"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?UF", Valor = oRow["UF"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idPais", Valor = 1, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?Cliente", Valor = 1, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?CEP", Valor = oRow["CEP"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idStatusEntidade", Valor = Entidade_Ativa, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idOrigemEntidade", Valor = 3, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idStatusIntegracaoEntidade", Valor = 26, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?CliLatitude", Valor = oRow["LATITUDE"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?CliLongitude", Valor = oRow["LONGITUDE"].ToString(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?Entidade_Ativa", Valor = Entidade_Ativa, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_PDV"].ToString().Trim(), Tipo = DbType.String }});

            sSql = "SELECT idEntidade FROM " + oBancoDados_Destino.BancoDados() + "tb_entidade where idempresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + oRow["COD_PDV"].ToString().Trim() + "'";
            idEntidade = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            sSql = "select id_condicoes_pagamento from " + oBancoDados_Destino.BancoDados() + "tb_condicoes_pagamento where id_empresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + oRow["CONDICAO_PAGTO"].ToString().Trim() + "'";
            id_condicoes_pagamento = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            if (oRow["VENDEDOR"].ToString().IndexOf(",") == -1)
            {
              sSql = "select idVendedor from " + oBancoDados_Destino.BancoDados() + "tb_vendedores where idEmpresa = " + iEmpresa.ToString() + " and Integracao_EDI = '" + oRow["VENDEDOR"].ToString().Trim() + "'";
              idVendedor = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
            }
            else
            {
              sSql = "select idVendedor from " + oBancoDados_Destino.BancoDados() + "tb_vendedores where idEmpresa = " + iEmpresa.ToString() + " and Integracao_EDI = '" + oRow["VENDEDOR"].ToString().Split(',')[0].Trim() + "'";
              idVendedor = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
            }

            sSql = "select idGrupoEntididade from " + oBancoDados_Destino.BancoDados() + "tb_grupo_entidade where idEmpresa = " + iEmpresa.ToString() + " and grupopadrao = 1";
            idGrupoEntidade = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            if (idGrupoEntidade == 0)
            {
              sSql = "insert into " + oBancoDados_Destino.BancoDados() + "tb_grupo_entidade" +
                     "(idEmpresa,GrupoEntidade,GrupoPadrao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                     " values (" + iEmpresa.ToString() + ", 'PADRAO', 1,'S'," + oBancoDados_Destino.DBSData() + ",'" + ProductVersion + "'," + Config_App.idIntegrador.ToString() + ")";

              sSql = "select idGrupoEntididade from " + oBancoDados_Destino.BancoDados() + "tb_grupo_entidade where idEmpresa = " + iEmpresa.ToString() + " and grupopadrao = 1";
              idGrupoEntidade = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
            }

            sSql = "select idEntidadeCategoria from " + oBancoDados_Destino.BancoDados() + "tb_entidade_categoria where idEmpresa = " + iEmpresa.ToString() + " and CategoriaPadrao = 1";
            idEntidadeCategoria = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            if (idGrupoEntidade == 0)
            {
              sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_entidade_categoria(idEmpresa,EntidadeCategoria,CategoriaPadrao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                     " values (" + iEmpresa.ToString() + ", 'PADRAO', 1,'S'," + oBancoDados_Destino.DBSData() + ",'" + ProductVersion + "'," + Config_App.idIntegrador.ToString() + ")";

              sSql = "select idEntidadeCategoria from " + oBancoDados_Destino.BancoDados() + "tb_entidade_categoria where idEmpresa = " + iEmpresa.ToString() + " and CategoriaPadrao = 1";
              idEntidadeCategoria = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
            }

            sSql = "select idRotaVenda from " + oBancoDados_Destino.BancoDados() + "tb_rotavenda where idEmpresa = " + iEmpresa.ToString() + " and RotaPadrao = 1";
            idRotaVenda = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

            if (idRotaVenda == 0)
            {
              sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_rotavenda(idEmpresa,rotavenda,RotaPadrao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                     " values (" + iEmpresa.ToString() + ", 'PADRAO', 1,'S'," + oBancoDados_Destino.DBSData() + ",'" + ProductVersion + "'," + Config_App.idIntegrador.ToString() + ")";
              oBancoDados_Destino.DBExecutar(sSql);

              sSql = "select idRotaVenda from " + oBancoDados_Destino.BancoDados() + "tb_rotavenda where idEmpresa = " + iEmpresa.ToString() + " and RotaPadrao = 1";
              idRotaVenda = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
            }

            sSql = "select * FROM " + oBancoDados_Destino.BancoDados() + "tb_condicoes_pagamento where id_empresa = " + iEmpresa.ToString() + " and EDI_Integracao = '" + oRow["CONDICAO_PAGTO"].ToString().Trim().PadLeft(3, '0') + "'";
            oData_CondicaoPadrao = oBancoDados_Destino.DBQuery(sSql);

            if (oData_CondicaoPadrao.Rows.Count != 0)
            {
              iPRIORIDADE_CONDICAO_PAGAMENTO = Convert.ToInt32(oData_CondicaoPadrao.Rows[0]["PRIORIDADE_CONDICAO_PAGAMENTO"]);
              id_condicoes_pagamento = Convert.ToInt32(oData_CondicaoPadrao.Rows[0]["id_condicoes_pagamento"]);
              ParcelaMinima = Convert.ToDouble(oData_CondicaoPadrao.Rows[0]["valor_minimo"]);
              MultiplicadorAPI = Convert.ToDouble(oData_CondicaoPadrao.Rows[0]["prazo_medio"]);
            }

            oData_CondicaoPadrao.Dispose();
            oData_CondicaoPadrao = null;

            sSql = "SELECT count(*) FROM " + oBancoDados_Destino.BancoDados() + "tb_entidade_empresa" +
                   " where id_empresa = " + iEmpresa.ToString() +
                     " and id_entidade = " + idEntidade.ToString();

            if ((iEmpresa != 0) && (idEntidade != 0))
            {
              if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0)) == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_entidade_empresa" +
                       "(id_frete_padrao,idStatusAcesso,idEntidadeEndereco,dtAlteracaoEndereco,dtAlteracaoCondPgto," +
                        "NroEnderecos,NroCondicoesPgto,SaldoDisponivel,LimiteCredito,EDI_Integracao," +
                        "idGrupoEntidade,idCategoriaEntidade, " +
                        "idTabelaPreco,idTabelaPrecoSistema,idRotaVenda,idVendedor,Sincronizado,verSincronizador,idSincronizador," +
                        "dtSincronizacao,id_entidade,id_empresa) VALUES " +
                       "(?id_frete_padrao,?idStatusAcesso,?idEntidadeEndereco," + oBancoDados_Destino.DBSData() + "," + oBancoDados_Destino.DBSData() + "," +
                        "?NroEnderecos,?NroCondicoesPgto,?SaldoDisponivel,?LimiteCredito,?EDI_Integracao," +
                        "?idGrupoEntidade,?idCategoriaEntidade, " +
                        "?idTabelaPreco,?idTabelaPrecoSistema,?idRotaVenda,?idVendedor,?Sincronizado,?verSincronizador,?idSincronizador," +
                        oBancoDados_Destino.DBSData() + ",?id_entidade,?id_empresa)";
              }
              else
              {
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_entidade_empresa" +
                       " set id_frete_padrao=?id_frete_padrao," +
                            "idStatusAcesso=?idStatusAcesso," +
                            "idEntidadeEndereco=?idEntidadeEndereco," +
                            "dtAlteracaoEndereco=" + oBancoDados_Destino.DBSData() + "," +
                            "dtAlteracaoCondPgto=" + oBancoDados_Destino.DBSData() + "," +
                            "NroEnderecos=?NroEnderecos," +
                            "NroCondicoesPgto=?NroCondicoesPgto," +
                            "SaldoDisponivel=?SaldoDisponivel," +
                            "LimiteCredito=?LimiteCredito," +
                            "EDI_Integracao=?EDI_Integracao," +
                            "idGrupoEntidade =?idGrupoEntidade," +
                            "idCategoriaEntidade=?idCategoriaEntidade," +
                            "idTabelaPreco=?idTabelaPreco," +
                            "idTabelaPrecoSistema=?idTabelaPrecoSistema," +
                            "idRotaVenda=?idRotaVenda," +
                            "idVendedor=?idVendedor," +
                            "Sincronizado=?Sincronizado," +
                            "verSincronizador=?verSincronizador," +
                            "idSincronizador=?idSincronizador," +
                            "dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       " where id_empresa = " + iEmpresa.ToString() +
                         " and id_entidade = " + idEntidade.ToString();
              }

              oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?id_frete_padrao", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idStatusAcesso", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idEntidadeEndereco", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?NroEnderecos", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?NroCondicoesPgto", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?SaldoDisponivel", Valor = oRow["SALDO_DISPONIVEL"], Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?LimiteCredito", Valor = oRow["SALDO_DISPONIVEL"], Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_PDV"].ToString().Trim(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idGrupoEntidade", Valor = idGrupoEntidade, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idCategoriaEntidade", Valor = idEntidadeCategoria, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idTabelaPreco", Valor = idTabelaPreco, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idTabelaPrecoSistema", Valor = idTabelaPreco, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idRotaVenda", Valor = idRotaVenda, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idVendedor", Valor = idVendedor, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?id_empresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?id_entidade", Valor = idEntidade, Tipo = DbType.Int32 }});


              if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0)) == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_entidade_empresa" +
                       "(id_frete_padrao,idStatusAcesso,idEntidadeEndereco,dtAlteracaoEndereco,dtAlteracaoCondPgto," +
                        "NroEnderecos,NroCondicoesPgto,SaldoDisponivel,LimiteCredito,EDI_Integracao," +
                        "idGrupoEntidade,idCategoriaEntidade, " +
                        "id_condicoes_pagamento,idRotaVenda,idVendedor,Sincronizado,verSincronizador,idSincronizador," +
                        "dtSincronizacao,id_entidade,id_empresa) VALUES " +
                       "(?id_frete_padrao,?idStatusAcesso,?idEntidadeEndereco," + oBancoDados_Destino.DBSData() + "," + oBancoDados_Destino.DBSData() + "," +
                        "?NroEnderecos,?NroCondicoesPgto,?SaldoDisponivel,?LimiteCredito,?EDI_Integracao," +
                        "?idGrupoEntidade,?idCategoriaEntidade, " +
                        "?id_condicoes_pagamento,?idRotaVenda,?idVendedor,?Sincronizado,?verSincronizador,?idSincronizador," +
                        oBancoDados_Destino.DBSData() + ",?id_entidade,?id_empresa)";
              }
              else
              {
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_entidade_empresa" +
                       " set id_frete_padrao=?id_frete_padrao," +
                            "idStatusAcesso=?idStatusAcesso," +
                            "idEntidadeEndereco=?idEntidadeEndereco," +
                            "dtAlteracaoEndereco=" + oBancoDados_Destino.DBSData() + "," +
                            "dtAlteracaoCondPgto=" + oBancoDados_Destino.DBSData() + "," +
                            "NroEnderecos=?NroEnderecos," +
                            "NroCondicoesPgto=?NroCondicoesPgto," +
                            "SaldoDisponivel=?SaldoDisponivel," +
                            "LimiteCredito=?LimiteCredito," +
                            "EDI_Integracao=?EDI_Integracao," +
                            "idGrupoEntidade =?idGrupoEntidade," +
                            "idCategoriaEntidade=?idCategoriaEntidade," +
                            "id_condicoes_pagamento=?id_condicoes_pagamento," +
                            "idRotaVenda=?idRotaVenda," +
                            "idVendedor=?idVendedor," +
                            "Sincronizado=?Sincronizado," +
                            "verSincronizador=?verSincronizador," +
                            "idSincronizador=?idSincronizador," +
                            "dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       " where id_empresa = " + iEmpresa.ToString() +
                         " and id_entidade = " + idEntidade.ToString();
              }

              oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?id_frete_padrao", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idStatusAcesso", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idEntidadeEndereco", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?NroEnderecos", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?NroCondicoesPgto", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?SaldoDisponivel", Valor = oRow["SALDO_DISPONIVEL"], Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?LimiteCredito", Valor = oRow["SALDO_DISPONIVEL"], Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_PDV"].ToString().Trim(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idGrupoEntidade", Valor = idGrupoEntidade, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idCategoriaEntidade", Valor = idEntidadeCategoria, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?id_condicoes_pagamento", Valor = id_condicoes_pagamento, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idRotaVenda", Valor = idRotaVenda, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idVendedor", Valor = idVendedor, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?id_empresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?id_entidade", Valor = idEntidade, Tipo = DbType.Int32 }});

              sSql = "update tb_entidade_empresa" +
                     " set idTabelaPreco = " + idTabelaPreco.ToString() +
                     " where id_empresa = " + iEmpresa.ToString() +
                       " and id_entidade = " + idEntidade.ToString();
              oBancoDados_Destino.DBExecutar(sSql);

              sSql = "update tb_entidade_empresa" +
                     " set id_condicoes_pagamento = " + id_condicoes_pagamento.ToString() +
                     " where id_empresa = " + iEmpresa.ToString() +
                       " and id_entidade = " + idEntidade.ToString();
              oBancoDados_Destino.DBExecutar(sSql);
            }

            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_entidade_condicoes_pagamento set EntidadeCondicaoAtiva = 0 where idEmpresa=" + iEmpresa.ToString() + " and idEntidade = " + idEntidade.ToString();
            oBancoDados_Destino.DBExecutar(sSql);

            sSql = "select count(*) from " + oBancoDados_Destino.BancoDados() + "tb_entidade_condicoes_pagamento" +
                   " where idEmpresa=" + iEmpresa.ToString() +
                     " and idEntidade = " + idEntidade.ToString() +
                     " and idCondicaoPagamento = " + id_condicoes_pagamento.ToString();

            if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
            {
              sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_entidade_condicoes_pagamento" +
                    "(idCondicaoPagamento,ParcelaMinima,Multiplicador,MultiplicadorAPI,EntidadeCondicaoAtiva," +
                     "Sincronizado,dtSincronizacao,verSincronizador,idSincronizador,idEntidade,idEmpresa,EDI_Integracao)" +
                     " VALUES (?idCondicaoPagamento,?ParcelaMinima,?Multiplicador,?MultiplicadorAPI,?EntidadeCondicaoAtiva," +
                              "?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador,?idEntidade,?idEmpresa,?EDI_Integracao)";
            }
            else
            {
              sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_entidade_condicoes_pagamento" +
                     " set idCondicaoPagamento=?idCondicaoPagamento," +
                          "ParcelaMinima=?ParcelaMinima," +
                          "Multiplicador=?Multiplicador," +
                          "MultiplicadorAPI=?MultiplicadorAPI," +
                          "EntidadeCondicaoAtiva=?EntidadeCondicaoAtiva," +
                          "Sincronizado=?Sincronizado," +
                          "dtSincronizacao=" + oBancoDados_Destino.DBSData() + "," +
                          "verSincronizador=?verSincronizador," +
                          "idSincronizador=?idSincronizador" +
                     " where idEntidade= ?idEntidade" +
                       " and idEmpresa=?idEmpresa" +
                       " and edi_integracao= ?edi_integracao";
            }

            oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idCondicaoPagamento", Valor = id_condicoes_pagamento, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?ParcelaMinima", Valor = ParcelaMinima, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?Multiplicador", Valor = 1 , Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?MultiplicadorAPI", Valor = MultiplicadorAPI , Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?EntidadeCondicaoAtiva", Valor = 1, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idEntidade", Valor = idEntidade, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["CONDICAO_PAGTO"].ToString().Trim(), Tipo = DbType.String }});

            oBancoDados_Destino.DBProcedure("sp_rot_criaCondicoesPgto", new clsCampo[] { new clsCampo { Nome = "iEmpresa", Valor = iEmpresa, Tipo = DbType.String },
                                                                                                      new clsCampo { Nome = "iCondicaoPadrao", Valor = id_condicoes_pagamento, Tipo = DbType.Int32 },
                                                                                                      new clsCampo { Nome = "iEntidade", Valor = idEntidade, Tipo = DbType.Int32 },
                                                                                                      new clsCampo { Nome = "iPrioridade", Valor = iPRIORIDADE_CONDICAO_PAGAMENTO, Tipo = DbType.Int32 } });

            if ((iEmpresa != 0) && (idEntidade != 0))
            {
              sSql = "select count(*) from " + oBancoDados_Destino.BancoDados() + "tb_entidade_endereco where idEmpresa = " + iEmpresa.ToString() + " and idEntidade= " + idEntidade.ToString() + " and idTipoEndereco='E'";

              if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_entidade_endereco" +
                       "(EnderecoExibicao,idTipoInscricao,CNPJEndereco,CEP,Endereco,Numero,Bairro,Cidade,idCidade,UF," +
                        "EnderecoAtivo,EnderecoPadrao,Sincronizado,dtSincronizacao," +
                        "verSincronizador,idSincronizador,idEmpresa,idEntidade,idTipoEndereco)" +
                       " VALUES " +
                       "(?EnderecoExibicao,?idTipoInscricao,?CNPJEndereco,?CEP,?Endereco,?Numero,?Bairro,?Cidade,?idCidade,?UF," +
                        "?EnderecoAtivo,?EnderecoPadrao,?Sincronizado," + oBancoDados_Destino.DBSData() + "," +
                        "?verSincronizador,?idSincronizador,?idEmpresa,?idEntidade,?idTipoEndereco)";
              }
              else
              {
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_entidade_endereco" +
                       " set EnderecoExibicao=?EnderecoExibicao," +
                            "idTipoInscricao=?idTipoInscricao," +
                            "CNPJEndereco=?CNPJEndereco," +
                            "CEP=?CEP," +
                            "Endereco=?Endereco," +
                            "Numero=?Numero," +
                            "Bairro=?Bairro," +
                            "Cidade=?Cidade," +
                            "idCidade=?idCidade," +
                            "UF=?UF," +
                            "EnderecoAtivo=?EnderecoAtivo," +
                            "EnderecoPadrao=?EnderecoPadrao," +
                            "dtAlteracao=" + oBancoDados_Destino.DBSData() + "," +
                            "Sincronizado=?Sincronizado," +
                            "verSincronizador=?verSincronizador," +
                            "idSincronizador=?idSincronizador" +
                       " where idEmpresa=?idEmpresa" +
                         " and idEntidade=?idEntidade" +
                         " and idTipoEndereco=?idTipoEndereco";
              }

              oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?EnderecoExibicao", Valor = oRow["ENDERECO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idTipoInscricao", Valor = idTipoEmpresa, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?CNPJEndereco", Valor = oRow["CNPJ_CPF"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?CEP", Valor = oRow["CEP"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Endereco", Valor = oRow["ENDERECO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Numero", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Bairro", Valor = oRow["BAIRRO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Cidade", Valor = oRow["CIDADE"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idCidade", Valor = idCidade, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?UF", Valor = oRow["UF"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?EnderecoAtivo", Valor = 1, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?EnderecoPadrao", Valor = 1, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idEntidade", Valor = idEntidade, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idTipoEndereco", Valor = "E", Tipo = DbType.String } });
            }

            if ((idEntidade != 0))
            {
              sSql = "select count(*) from " + oBancoDados_Destino.BancoDados() + "tb_entidade_tabela_preco where idEmpresa= " + iEmpresa.ToString() + " and idEntidade= " + idEntidade.ToString() + " and EDI_Integracao= '" + oRow["TABELA_PRECO"].ToString().Trim() + "'";
              sSql = "select count(*) from " + oBancoDados_Destino.BancoDados() + "tb_entidade_tabela_preco where idEmpresa= " + iEmpresa.ToString() + " and idEntidade= " + idEntidade.ToString();

              if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
              {
                sSql = "insert into " + oBancoDados_Destino.BancoDados() + "tb_entidade_tabela_preco" +
                       "(idTabelaPreco,TabelaAtivaEntidade,ValorMinimoPedido,TabelaValidaDesde," +
                        "TabelaValidaAte,Sincronizado,dtSincronizacao," +
                        "verSincronizador,idSincronizador,idEmpresa,EDI_Integracao,idEntidade)" +
                       " values (?idTabelaPreco,?TabelaAtivaEntidade,?ValorMinimoPedido," + oBancoDados_Destino.DBSData() + "," +
                                 oBancoDados_Destino.DBSData(5) + ",?Sincronizado," + oBancoDados_Destino.DBSData() + "," +
                                "?verSincronizador,?idSincronizador,?idEmpresa,?EDI_Integracao,?idEntidade)";
              }
              else
              {
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_entidade_tabela_preco" +
                       " set idTabelaPreco=?idTabelaPreco," +
                            "TabelaAtivaEntidade=?TabelaAtivaEntidade," +
                            "ValorMinimoPedido=?ValorMinimoPedido," +
                            "TabelaValidaDesde=cast(" + oBancoDados_Destino.DBSData() + " as date)," +
                            "TabelaValidaAte=cast(" + oBancoDados_Destino.DBSData(5) + " as date)," +
                            "Sincronizado=?Sincronizado," +
                            "verSincronizador=?verSincronizador," +
                            "idSincronizador=?idSincronizador" +
                       " where idEmpresa=?idEmpresa" +
                         " and idEntidade=?idEntidade";
              }

              oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idTabelaPreco", Valor = idTabelaPreco, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?TabelaAtivaEntidade", Valor = -1, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?ValorMinimoPedido", Valor = 0, Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["TABELA_PRECO"].ToString().Trim(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idEntidade", Valor = idEntidade, Tipo = DbType.Int32 }});
            }
          }

          oBancoDados_Destino.DBProcedure("rot_final_integracao_olapdvV2", new clsCampo[] { new clsCampo { Nome = "?iEmpresa", Valor = iIdEmpresa, Tipo = DbType.Int32 } });

          bRet = true;
        }
      }
      catch (Exception Ex)
      {
        sStatus = "ERRO";
        sErro = Ex.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
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

    private static bool OlaPDV_Produto(string sAplicativo,
                                       string sPartner,
                                       string sCdServico,
                                       string sTarefa,
                                       int nr_ordem_execucao,
                                       int iIdEmpresa,
                                       int iIdTarefa,
                                       long iGrupoTarefas,
                                       string sConexao,
                                       string sUsuario,
                                       string sSenha,
                                       string sCOD_PUXADA,
                                       string sTIPO_REGISTRO,
                                       string sTIPO_CONSULTA,
                                       string sDS_STRINGCONEXAODESTINO,
                                       string sTP_BANCODADOSDESTINO,
                                       string sDS_Origem,
                                       string sDS_Destino,
                                       ref string sJsonRet)
    {
      bool bRet = false;
      DataTable oData = null;
      DataTable oDataAux = null;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";
      int iEmpresa = 0;
      int idCatalogo = 0;
      int idMarca = 0;
      int idFornecedor = 0;
      int idLinhadeProdutos = 0;
      int idUnidadeMedida = 0;
      int idTamanho = 0;
      int idEmbalagem = 0;
      int idGrupoProdutos = 0;
      int idCategoria1 = 0;
      int idCategoria2 = 0;
      int idStatus = 0;
      string sPrefixoPedido = "";
      string CategoriaCorFundo = "";
      string CategoriaCorLetra = "";
      string sExtArquivoImg = "";
      int iCont = 0;
      string sNUMERO_ORDEM_IMPRESSAO = "";

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      TimeSpan ts;
      DateTime dUtil = DateTime.Now;

      try
      {
        oData = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico, ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa,
                                                sDS_Origem, sConexao, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, "", ref sJsonRet);

        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

        if (oData != null && oBancoDados_Destino.DBConectado())
        {
          oDataAux = oBancoDados_Destino.DBQuery("select * from " + oBancoDados_Destino.BancoDados() + " tb_empresas where rtrim(ChaveEdi) = '" + sCOD_PUXADA.Trim() + "'");

          if (oDataAux.Rows.Count != 0)
          {
            iEmpresa = Convert.ToInt32(oDataAux.Rows[0]["idEmpresa"]);
            sExtArquivoImg = oDataAux.Rows[0]["ExtArquivoImg"].ToString();
            CategoriaCorFundo = _Funcoes.FNC_NuloString(oDataAux.Rows[0]["CategoriaCorFundo"]);
            CategoriaCorLetra = _Funcoes.FNC_NuloString(oDataAux.Rows[0]["CategoriaCorLetra"]);
            sPrefixoPedido = oDataAux.Rows[0]["PrefixoPedido"].ToString();

            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_produtos" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                       ",produtoAtivo='N'" +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_catalogo" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_marcas" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_linhaprodutos" +
                   " set Sincronizado = 'N'" +
                       ",LinhaAtivo = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_unidademedida" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_tamanho" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_embalagem" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_grupoprodutos" +
                   " set Sincronizado = 'N'" +
                       ",grupoAtivo = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_categorias1" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);
            sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_categorias2" +
                   " set Sincronizado = 'N'" +
                       ",dtSincronizacao=" + oBancoDados_Destino.DBSData() +
                       ",verSincronizador='" + ProductVersion + "'" +
                       ",idSincronizador=" + Config_App.idIntegrador.ToString() +
                   " where idEmpresa = " + iEmpresa.ToString();
            oBancoDados_Destino.DBExecutar(sSql);

            foreach (DataRow oRow in oData.Rows)
            {
              iCont++;
              Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processando", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), sConexao, iCont, oData.Rows.Count);

              sSql = "select idCatalogo from " + oBancoDados_Destino.BancoDados() + "tb_catalogo where EDI_Integracao= '" + sCOD_PUXADA.Trim() + "' and CatalogoPadrao=1";
              idCatalogo = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              sSql = "select idFornecedor from " + oBancoDados_Destino.BancoDados() + "tb_fornecedor where EDI_Integracao= '" + sCOD_PUXADA.Trim() + "' and FornecedorPadrao=1";
              idFornecedor = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              sSql = "select idMarca from " + oBancoDados_Destino.BancoDados() + "tb_marcas where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_MARCA"].ToString().Trim() + "'";
              idMarca = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              if (idCatalogo == 0)
              {
                sSql = "insert into " + oBancoDados_Destino.BancoDados() + "tb_catalogo" +
                       " (idEmpresa,Catalogo,CatalogoPadrao,EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                       " values (?idEmpresa,?Catalogo,?CatalogoPadrao,?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador)";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?Catalogo", Valor = "OlaPdv", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?CatalogoPadrao", Valor = 1, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = sCOD_PUXADA.Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                sSql = "select idCatalogo from " + oBancoDados_Destino.BancoDados() + "tb_catalogo where EDI_Integracao= '" + sCOD_PUXADA.Trim() + "' and CatalogoPadrao=1";
                idCatalogo = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
              }

              if (idMarca == 0)
              {
                sSql = "insert into " + oBancoDados_Destino.BancoDados() + "tb_marcas" +
                      "(idEmpresa,Marca,MarcaPadrao,OrdemMarca,EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                       " values (?idEmpresa,?Marca,?MarcaPadrao,?OrdemMarca,?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador)";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?Marca", Valor = "a definir " + oRow["COD_MARCA"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?MarcaPadrao", Valor = 0, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?OrdemMarca", Valor = "00", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_MARCA"].ToString().Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                sSql = "select idMarca from " + oBancoDados_Destino.BancoDados() + "tb_marcas where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_MARCA"].ToString().Trim() + "'";
                idMarca = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
              }

              sSql = "select idLinhadeProdutos from " + oBancoDados_Destino.BancoDados() + "tb_linhaprodutos where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_FAMILIA"].ToString().Trim() + "'";
              idLinhadeProdutos = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              if (idLinhadeProdutos == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_linhaprodutos(idEmpresa,LinhaProdutos,OrdemLinhaProdutos,EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador,LinhaAtivo)" +
                       " VALUES (?idEmpresa,?LinhaProdutos,?OrdemLinhaProdutos,?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador,?LinhaAtivo)";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?LinhaProdutos", Valor = oRow["DSC_FAMILIA"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?OrdemLinhaProdutos", Valor = "00", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_FAMILIA"].ToString().Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?LinhaAtivo", Valor = OlaPDV_Produto_SN(oRow["B2B_FAMILIA"].ToString()).Trim(), Tipo = DbType.String }});

                sSql = "select idLinhadeProdutos from " + oBancoDados_Destino.BancoDados() + "tb_linhaprodutos where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_FAMILIA"].ToString().Trim() + "'";
                idLinhadeProdutos = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
              }
              else
              {
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_linhaprodutos" +
                       " set LinhaProdutos = ?LinhaProdutos," +
                           " OrdemLinhaProdutos = ?OrdemLinhaProdutos," +
                           " Sincronizado = ?Sincronizado," +
                           " verSincronizador = ?verSincronizador," +
                           " idSincronizador = ?idSincronizador," +
                           " LinhaAtivo = ?LinhaAtivo" +
                       " where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_FAMILIA"].ToString().Trim() + "'";
                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?LinhaProdutos", Valor = oRow["DSC_FAMILIA"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?OrdemLinhaProdutos", Valor = "00", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?LinhaAtivo", Valor = OlaPDV_Produto_SN(oRow["B2B_FAMILIA"].ToString()).Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_FAMILIA"].ToString().Trim(), Tipo = DbType.String }});

              }

              sSql = "select idUnidadeMedida from " + oBancoDados_Destino.BancoDados() + "tb_unidademedida where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_UNIDADE_MEDIDA"].ToString().Trim() + "'";
              idUnidadeMedida = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              if (idUnidadeMedida == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_unidademedida (idEmpresa,UnidadeMedida,Sigla,EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                       " VALUES( ?idEmpresa, ?UnidadeMedida, ?Sigla, ?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador)";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?UnidadeMedida", Valor = oRow["COD_UNIDADE_MEDIDA"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sigla", Valor = oRow["COD_UNIDADE_MEDIDA"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_UNIDADE_MEDIDA"].ToString().Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                sSql = "select idUnidadeMedida from " + oBancoDados_Destino.BancoDados() + ".tb_unidademedida where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_UNIDADE_MEDIDA"].ToString().Trim() + "'";
                idUnidadeMedida = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
              }

              sSql = "select idTamanho from " + oBancoDados_Destino.BancoDados() + "tb_tamanho where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_UNIDADE_MEDIDA"].ToString().Trim() + "'";
              idTamanho = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              if (idTamanho == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_tamanho(idEmpresa,Tamanho,EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                       " VALUES (?idEmpresa,?Tamanho,?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador)";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?Tamanho", Valor = oRow["COD_UNIDADE_MEDIDA"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_UNIDADE_MEDIDA"].ToString().Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                sSql = "select idTamanho from " + oBancoDados_Destino.BancoDados() + "tb_tamanho where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_UNIDADE_MEDIDA"].ToString().Trim() + "'";
                idTamanho = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
              }

              sSql = "select idEmbalagem from " + oBancoDados_Destino.BancoDados() + "tb_embalagem where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_UNIDADE_MEDIDA"].ToString().Trim() + "'";
              idEmbalagem = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              if (idEmbalagem == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_embalagem(idEmpresa,Embalagem,EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                       " VALUES(?idEmpresa,?Embalagem,?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador)";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?Embalagem", Valor = oRow["COD_UNIDADE_MEDIDA"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_UNIDADE_MEDIDA"].ToString().Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                sSql = "select idEmbalagem from " + oBancoDados_Destino.BancoDados() + "tb_embalagem where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_UNIDADE_MEDIDA"].ToString().Trim() + "'";
                idEmbalagem = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
              }

              sSql = "select idGrupoProdutos from " + oBancoDados_Destino.BancoDados() + "tb_grupoprodutos where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_GRUPO_COMERCIALIZACAO"].ToString().Trim() + "'";
              idGrupoProdutos = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              if (idGrupoProdutos == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_grupoprodutos(idEmpresa,GrupoProdutos,OrdemGrupoProdutos,EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador,grupoAtivo)" +
                       " VALUES(?idEmpresa,?GrupoProdutos,?OrdemGrupoProdutos,?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador,?grupoAtivo)";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?GrupoProdutos", Valor = "a definir " + oRow["COD_GRUPO_COMERCIALIZACAO"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?OrdemGrupoProdutos", Valor = "00", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_GRUPO_COMERCIALIZACAO"].ToString().Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?grupoAtivo", Valor = OlaPDV_Produto_SN(oRow["B2B_GRUPO"].ToString()).Trim(), Tipo = DbType.String }});

                sSql = "select idGrupoProdutos from " + oBancoDados_Destino.BancoDados() + "tb_grupoprodutos where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_GRUPO_COMERCIALIZACAO"].ToString().Trim() + "'";
                idGrupoProdutos = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
              }
              else
              {
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_grupoprodutos" +
                       " set GrupoProdutos = ?GrupoProdutos," +
                            "OrdemGrupoProdutos = ?OrdemGrupoProdutos," +
                            "Sincronizado = ?Sincronizado," +
                            "verSincronizador = ?verSincronizador," +
                            "idSincronizador = ?idSincronizador," +
                            "grupoAtivo = ?grupoAtivo" +
                       " where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_GRUPO_COMERCIALIZACAO"].ToString().Trim() + "'";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?GrupoProdutos", Valor = "a definir " + oRow["COD_GRUPO_COMERCIALIZACAO"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?OrdemGrupoProdutos", Valor = "00", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?grupoAtivo", Valor = OlaPDV_Produto_SN(oRow["B2B_GRUPO"].ToString()).Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_GRUPO_COMERCIALIZACAO"].ToString().Trim(), Tipo = DbType.String }});
              }

              sSql = "select idCategoria1 from " + oBancoDados_Destino.BancoDados() + "tb_categorias1 where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_GRUPO"].ToString().Trim() + "'";
              idCategoria1 = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              if (idCategoria1 == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_categorias1(idEmpresa,idCatalogo,Categoria1,OrdemCategoria1,CategoriaIcone," +
                                                          "CategoriaDescricao,CategoriaCorFundo,CategoriaCorLetra,Categoria1Prioridade," +
                                                          "Categoria1Ativa,dtCadastro,dtAlteracao,dtImportacao,EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                       " VALUES (?idEmpresa,?idCatalogo,?Categoria1,?OrdemCategoria1,?CategoriaIcone," +
                                "?CategoriaDescricao,?CategoriaCorFundo,?CategoriaCorLetra,0," +
                                "?Categoria1Ativa,?dtCadastro,?dtAlteracao,?dtImportacao,?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador)";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?idCatalogo", Valor = idCatalogo, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?Categoria1", Valor = oRow["DSC_GRUPO"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?OrdemCategoria1", Valor = "00", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?CategoriaIcone", Valor = iEmpresa.ToString() + "_padrao.png", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?CategoriaDescricao", Valor =  oRow["DSC_GRUPO"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?CategoriaCorFundo", Valor = CategoriaCorFundo, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?CategoriaCorLetra", Valor = CategoriaCorLetra, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Categoria1Ativa", Valor = 1, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?dtCadastro", Valor = DateTime.Now, Tipo = DbType.DateTime },
                                                                                      new clsCampo { Nome = "?dtAlteracao", Valor = DateTime.Now, Tipo = DbType.DateTime },
                                                                                      new clsCampo { Nome = "?dtImportacao", Valor = DateTime.Now, Tipo = DbType.DateTime },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_GRUPO"].ToString().Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                sSql = "select idCategoria1 from " + oBancoDados_Destino.BancoDados() + "tb_categorias1 where idEmpresa = " + iEmpresa.ToString().Trim() + " and EDI_Integracao= '" + oRow["COD_GRUPO"].ToString().Trim() + "'";
                idCategoria1 = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
              }

              sSql = "select idCategoria2 from " + oBancoDados_Destino.BancoDados() + "tb_categorias2 where idEmpresa = " + iEmpresa.ToString().Trim() +
                                                                      " and idCategoria1 = " + idCategoria1.ToString().Trim() +
                                                                      " and EDI_Integracao= '" + oRow["COD_FAMILIA"].ToString().Trim() + "'";
              idCategoria2 = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));

              if (idCategoria2 == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_categorias2(idEmpresa,idCategoria1,Categoria2,Categoria2Ativa,OrdemCategoria2," +
                       "EDI_Integracao,Sincronizado,dtSincronizacao,verSincronizador,idSincronizador)" +
                       " values (?idEmpresa,?idCategoria1,?Categoria2,?Categoria2Ativa,?OrdemCategoria2,?EDI_Integracao,?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador)";

                oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?idCategoria1", Valor = idCategoria1, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?Categoria2", Valor = oRow["DSC_FAMILIA"].ToString(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Categoria2Ativa", Valor = 1, Tipo = DbType.Int32 },
                                                                                      new clsCampo { Nome = "?OrdemCategoria2", Valor = "00", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_FAMILIA"].ToString().Trim(), Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                      new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                sSql = "select idCategoria2 from " + oBancoDados_Destino.BancoDados() + "tb_categorias2 where idEmpresa = " + iEmpresa.ToString().Trim() +
                                                                       " and idCategoria1 = " + idCategoria1.ToString().Trim() +
                                                                       " and EDI_Integracao= '" + oRow["COD_FAMILIA"].ToString().Trim() + "'";
                idCategoria2 = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0));
              }

              if (oRow["IND_ATIVO"].ToString() == "A")
                idStatus = 1;
              else
                idStatus = 2;

              sSql = "select * from " + oBancoDados_Destino.BancoDados() + "tb_produtos where idEmpresa= " + iEmpresa.ToString() + " and EDI_Integracao = '" + oRow["COD_PRODUTO_INTERNO"].ToString().Trim() + "'";

              try
              {
                sNUMERO_ORDEM_IMPRESSAO = FNC_ZerosEsquerda(oRow["NUMERO_ORDEM_IMPRESSAO"].ToString().Trim(), 5);
              }
              catch (Exception)
              {
                sNUMERO_ORDEM_IMPRESSAO = "00000";
              }

              if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
              {
                sSql = "INSERT INTO " + oBancoDados_Destino.BancoDados() + "tb_produtos" +
                       "(idCatalogo,idMarca,Marca_Filtro,SKU,Codigo,CodigoFornecedor,idFornecedor,Busca,idLinhadeProdutos,EAN," +
                        "Destaque,SaiListaGeral,OrdemExibicao,Nome_Amigavel,Descricao_Limpa,Nome_Curto,Descricao,Descricao_Longa," +
                        "Descricao_Detalhada,idUnidadeMedida,idUnidadeMedidaVenda,Fator_Venda,Fator_Unitario,idTamanho,Tamanho,Preco1Venda," +
                        "Preco2VendaUnitario,PrecoVendaUnitarioExib,Custo,CustoUnitario,idEmbalagem,Quantidade_na_embalagem,Quantidade_Exibicao," +
                        "Quantidade_minima_pedida,Multiplicador,Capacidade,Medida,idGrupoProdutos,idCategoria2,idCategoria1," +
                        "Estoque,Agrupador,TemVariacao,idStatus,idStatusImagem,ICMS,PIS,COFINS,IPI,Impostos," +
                        "ImagemHD,ImagemNormal,ImagemMobile,ProdutoBusca,ComissaoProduto,DescontoMaximo,ValorPesoProduto," +
                        "Sincronizado,dtSincronizacao,verSincronizador,idSincronizador,produtoAtivo,vGrupoAtivo,vLinhaAtiva,NumeroOrdemImpressao,idEmpresa,EDI_Integracao)" +
                        " values " +
                        "(?idCatalogo,?idMarca,?Marca_Filtro,?SKU,?Codigo,?CodigoFornecedor,?idFornecedor,?Busca,?idLinhadeProdutos,?EAN," +
                         "?Destaque,?SaiListaGeral,?OrdemExibicao,?Nome_Amigavel,?Descricao_Limpa,?Nome_Curto,?Descricao,?Descricao_Longa," +
                         "?Descricao_Detalhada,?idUnidadeMedida,?idUnidadeMedidaVenda,?Fator_Venda,?Fator_Unitario,?idTamanho,?Tamanho,0,0," +
                         "'0.00',0,0,?idEmbalagem,?Quantidade_na_embalagem,?Quantidade_Exibicao,?Quantidade_minima_pedida," +
                         "?Multiplicador,?Capacidade,?Medida,?idGrupoProdutos,?idCategoria2,?idCategoria1," +
                         "0,?Agrupador,?TemVariacao,?idStatus,0,?ICMS,?PIS,?COFINS,?IPI,?Impostos," +
                         "?ImagemHD,?ImagemNormal,?ImagemMobile,?ProdutoBusca,?ComissaoProduto,?DescontoMaximo,?ValorPesoProduto," +
                         "?Sincronizado," + oBancoDados_Destino.DBSData() + ",?verSincronizador,?idSincronizador,?produtoAtivo,?vGrupoAtivo,?vLinhaAtiva,?NumeroOrdemImpressao,?idEmpresa,?EDI_Integracao)";
              }
              else
              {
                sSql = "update " + oBancoDados_Destino.BancoDados() + "tb_produtos" +
                       " set idCatalogo=?idCatalogo," +
                            "idMarca=?idMarca," +
                            "Marca_Filtro=?Marca_Filtro," +
                            "SKU=?SKU," +
                            "Codigo=?Codigo," +
                            "CodigoFornecedor=?CodigoFornecedor," +
                            "idFornecedor=?idFornecedor," +
                            "Busca=?Busca," +
                            "idLinhadeProdutos=?idLinhadeProdutos," +
                            "EAN=?EAN," +
                            "Destaque=?Destaque," +
                            "SaiListaGeral=?SaiListaGeral," +
                            "OrdemExibicao=?OrdemExibicao," +
                            "Nome_Amigavel=?Nome_Amigavel," +
                            "Descricao_Limpa=?Descricao_Limpa," +
                            "Nome_Curto=?Nome_Curto," +
                            "Descricao=?Descricao," +
                            "Descricao_Longa=?Descricao_Longa," +
                            "Descricao_Detalhada=?Descricao_Detalhada," +
                            "idUnidadeMedida=?idUnidadeMedida," +
                            "idUnidadeMedidaVenda=?idUnidadeMedidaVenda," +
                            "Fator_Venda=?Fator_Venda," +
                            "Fator_Unitario=?Fator_Unitario," +
                            "idTamanho=?idTamanho," +
                            "Tamanho=?Tamanho," +
                            "idEmbalagem=?idEmbalagem," +
                            "Quantidade_na_embalagem=?Quantidade_na_embalagem," +
                            "Quantidade_Exibicao=?Quantidade_Exibicao," +
                            "Quantidade_minima_pedida=?Quantidade_minima_pedida," +
                            "Multiplicador=?Multiplicador," +
                            "Capacidade=?Capacidade," +
                            "Medida=?Medida," +
                            "idGrupoProdutos=?idGrupoProdutos," +
                            "idCategoria1=?idCategoria1," +
                            "idCategoria2=?idCategoria2," +
                            "Agrupador=?Agrupador," +
                            "TemVariacao=?TemVariacao," +
                            "idStatus=?idStatus," +
                            "ICMS=?ICMS," +
                            "PIS=?PIS," +
                            "COFINS=?COFINS," +
                            "IPI=?IPI," +
                            "Impostos=?Impostos," +
                            "ImagemHD=?ImagemHD," +
                            "ImagemNormal=?ImagemNormal," +
                            "ImagemMobile=?ImagemMobile," +
                            "ProdutoBusca=?ProdutoBusca," +
                            "ComissaoProduto=?ComissaoProduto," +
                            "DescontoMaximo=?DescontoMaximo," +
                            "ValorPesoProduto=?ValorPesoProduto," +
                            "Sincronizado=?Sincronizado," +
                            "dtSincronizacao=" + oBancoDados_Destino.DBSData() + "," +
                            "verSincronizador=?verSincronizador," +
                            "idSincronizador=?idSincronizador," +
                            "produtoAtivo=?produtoAtivo," +
                            "vGrupoAtivo=?vGrupoAtivo," +
                            "vLinhaAtiva=?vLinhaAtiva," +
                            "NumeroOrdemImpressao=?NumeroOrdemImpressao" +
                           " where idEmpresa=?idEmpresa" +
                             " and EDI_Integracao=?EDI_Integracao";
              }

              oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?idCatalogo", Valor = idCatalogo, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idMarca", Valor = idMarca, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Marca_Filtro", Valor = "UNICO", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?SKU", Valor = sPrefixoPedido + oRow["COD_PRODUTO_INTERNO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Codigo", Valor = oRow["COD_PRODUTO_INTERNO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?CodigoFornecedor", Valor = oRow["COD_PRODUTO_APELIDO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idFornecedor", Valor = idFornecedor, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Busca", Valor = oRow["COD_PRODUTO_INTERNO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idLinhadeProdutos", Valor = idLinhadeProdutos, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?EAN", Valor = oRow["COD_EAN"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Destaque", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?SaiListaGeral", Valor = "Sim", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?OrdemExibicao", Valor = "10100000", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Nome_Amigavel", Valor = oRow["DSC_PRODUTO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Descricao_Limpa", Valor = oRow["DSC_PRODUTO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Nome_Curto", Valor = oRow["DSC_PRODUTO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Descricao", Valor = oRow["DSC_PRODUTO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Descricao_Longa", Valor = oRow["DSC_PRODUTO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Descricao_Detalhada", Valor = oRow["QTD_UNIDADE_MEDIDA"].ToString() + " x " + oRow["COD_UNIDADE_MEDIDA"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idUnidadeMedida", Valor = idUnidadeMedida, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idUnidadeMedidaVenda", Valor = idUnidadeMedida, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Fator_Venda", Valor = oRow["QTD_UNIDADE_MEDIDA"], Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?Fator_Unitario", Valor = 1, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idTamanho", Valor = idTamanho, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Tamanho", Valor = oRow["COD_UNIDADE_MEDIDA"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idEmbalagem", Valor = idEmbalagem, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Quantidade_na_embalagem", Valor = oRow["QTD_UNIDADE_MEDIDA"], Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?Quantidade_Exibicao", Valor = oRow["COD_UNIDADE_MEDIDA"] + " c/" + oRow["QTD_UNIDADE_MEDIDA"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?Quantidade_minima_pedida", Valor = 1, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Multiplicador", Valor = 1, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Capacidade", Valor = 1, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Medida", Valor = oRow["COD_UNIDADE_MEDIDA"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idGrupoProdutos", Valor = idGrupoProdutos, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idCategoria1", Valor = idCategoria1, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idCategoria2", Valor = idCategoria2, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Agrupador", Valor = sPrefixoPedido + oRow["COD_PRODUTO_INTERNO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?TemVariacao", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?idStatus", Valor = idStatus, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?ICMS", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?PIS", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?COFINS", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?IPI", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?Impostos", Valor = 0, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?ImagemHD", Valor = oRow["COD_PRODUTO_INTERNO"].ToString() + sExtArquivoImg, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?ImagemNormal", Valor = oRow["COD_PRODUTO_INTERNO"].ToString() + sExtArquivoImg, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?ImagemMobile", Valor = oRow["COD_PRODUTO_INTERNO"].ToString() + ".png", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?ProdutoBusca", Valor = oRow["COD_PRODUTO_INTERNO"].ToString(), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?ComissaoProduto", Valor = 1, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?DescontoMaximo", Valor = oRow["PCT_MAXIMO_DESCONTO"], Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?ValorPesoProduto", Valor = oRow["VLR_PESO_PRODUTO"], Tipo = DbType.Double },
                                                                                  new clsCampo { Nome = "?Sincronizado", Valor = "S", Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?verSincronizador", Valor = ProductVersion, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?produtoAtivo", Valor = OlaPDV_Produto_SN(oRow["B2B_PRODUTO"].ToString()), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?vGrupoAtivo", Valor = OlaPDV_Produto_SN(oRow["B2B_GRUPO"].ToString()), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?vLinhaAtiva", Valor = OlaPDV_Produto_SN(oRow["B2B_FAMILIA"].ToString()), Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?NumeroOrdemImpressao", Valor = sNUMERO_ORDEM_IMPRESSAO, Tipo = DbType.String },
                                                                                  new clsCampo { Nome = "?idEmpresa", Valor = iEmpresa, Tipo = DbType.Int32 },
                                                                                  new clsCampo { Nome = "?EDI_Integracao", Valor = oRow["COD_PRODUTO_INTERNO"].ToString().Trim(), Tipo = DbType.String }});
            }

          }

          bRet = true;
        }
      }
      catch (Exception E)
      {
        sStatus = "ERRO";
        sErro = E.Message;
      }

      ts = DateTime.Now - dUtil;
      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bRet;
    }

    private static bool OlaPDV_TrackingDevolucoes(string sAplicativo,
                                                  string sPartner,
                                                  string sCdServico,
                                                  int iIdEmpresa,
                                                  int iIdTarefa,
                                                  string sConexao,
                                                  string sUsuario,
                                                  string sSenha,
                                                  string sCOD_PUXADA,
                                                  string sTIPO_REGISTRO,
                                                  string sTIPO_CONSULTA,
                                                  string sDS_STRINGCONEXAODESTINO,
                                                  string sTP_BANCODADOSDESTINO,
                                                  string sDS_Origem,
                                                  string sDS_Destino,
                                                  ref string sJsonRet)
    {
      bool bRet = false;
      DataTable oData_01 = null;
      DataTable oData_02 = null;
      clsBancoDados oBancoDados_Destino = null;
      string sSql = "";

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      try
      {
        oBancoDados_Destino = new clsBancoDados();
        oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

        if (oBancoDados_Destino.DBConectado())
        {
          string[] Lista = null;

          sSql = "SELECT customer_id, CodigoEDI, CodEdiAgrupado FROM lksuite.lk_customer";
          oData_01 = oBancoDados_Destino.DBQuery(sSql);

          foreach (DataRow oRow01 in oData_01.Rows)
          {
            Lista = oRow01["CodEdiAgrupado"].ToString().Split(',');

            sSql = "update lksuite.lk_reasons set ControleImportacao = 9 where customer_id = " + oRow01["customer_id"].ToString();
            oBancoDados_Destino.DBExecutar(sSql);

            foreach (string sLista in Lista)
            {
              sSql = sConexao + "&COD_PUXADA_DIS=" + sLista.Trim();
              oData_02 = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico, ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa, sDS_Origem, sSql, "",
                                                         sTIPO_REGISTRO, sTIPO_CONSULTA, "", ref sJsonRet);

              if (oData_02.Columns.Count == 2)
              {
                foreach (DataRow oRow02 in oData_02.Rows)
                {
                  sSql = "select count(*) from lksuite.lk_reasons" +
                         " where customer_id = " + oRow01["customer_id"].ToString() +
                           " and idIntegracaoEDI = " + oRow02["CODIGO"].ToString();

                  if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
                  {
                    sSql = "insert into lksuite.lk_reasons (Reason, customer_id, idIntegracaoEDI, ControleImportacao)" +
                           " values (?Reason,?customer_id,?idIntegracaoEDI, 0)";
                  }
                  else
                  {
                    sSql = "update lksuite.lk_reasons" +
                           " set Reason=?Reason," +
                                "ControleImportacao=0" +
                           " where customer_id = ?customer_id" +
                             " and idIntegracaoEDI = ?idIntegracaoEDI";
                  }

                  oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "?Reason", Valor = oRow02["DESCRICAO"].ToString(), Tipo = DbType.String },
                                                                                          new clsCampo { Nome = "?customer_id", Valor = Convert.ToInt32(oRow01["customer_id"].ToString()), Tipo = DbType.Int32 },
                                                                                          new clsCampo { Nome = "?idIntegracaoEDI", Valor =oRow02["CODIGO"].ToString(), Tipo = DbType.String }});
                }
              }

            }

            sSql = "delete from lksuite.lk_reasons where customer_id = " + oRow01["customer_id"].ToString() + " and ControleImportacao = 9";
            oBancoDados_Destino.DBExecutar(sSql);
          }

          bRet = true;
        }
      }
      catch (Exception)
      {
      }

      return bRet;
    }

    private static bool PeP_USP_ACOMP_D_VENDA_SOFIA(int iIdTarefa,
                                                    string sDS_STRINGCONEXAOORIGEM,
                                                    string sTP_BANCODADOSORIGEM,
                                                    string sDS_STRINGCONEXAODESTINO,
                                                    string sTP_BANCODADOSDESTINO)
    {
      string sSql;
      bool bOk = false;

      DataTable oData;
      clsBancoDados oBancoDados_Origem = new clsBancoDados();
      clsBancoDados oBancoDados_Destino = new clsBancoDados();
      CommandType Tipo;

      oBancoDados_Origem.DBConectar(sTP_BANCODADOSORIGEM, sDS_STRINGCONEXAOORIGEM);
      oBancoDados_Destino.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

      oData = oBancoDados_Origem.DBQuery("SELECT * FROM PREVISTO_SOPHIA");

      foreach (DataRow oRow in oData.Rows)
      {
        sSql = "SELECT COUNT(*) " +
               " FROM tb_USP_ACOMP_D_VENDA_SOFIA" +
               " WHERE LTRIM(RTRIM([COD PUXADA])) = '" + oRow["COD PUXADA"].ToString() + "'" +
                 " AND SETOR = " + oRow["SETOR"].ToString();
        if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql)) == 0)
        {
          sSql = "INSERT INTO [dbo].[tb_USP_ACOMP_D_VENDA_SOFIA] ([COD PUXADA],[SETOR],[VENDEDOR],[COD GER],[COD SUP],[VENDA DIA],[PDV  A VST]," +
                                                                 "[POSITIVACAO],[% POSIT],[DDD],[TELEFONE],[DTINTEGRACAO],[VERSAO_INTEGRADOR])" +
                 " VALUES (#COD_PUXADA,#SETOR,#VENDEDOR,#COD_GER,#COD_SUP,#VENDA_DIA,#PDV_A_VST, " +
                          "#POSITIVACAO,#POSIT,#DDD,#TELEFONE,#DTINTEGRACAO,#VERSAO_INTEGRADOR)";
          bOk = oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor =  oRow["COD PUXADA"].ToString(), Tipo = DbType.String },
                                                                                new clsCampo { Nome = "SETOR", Valor = oRow["SETOR"], Tipo = DbType.Int16 },
                                                                                new clsCampo { Nome = "VENDEDOR", Valor = oRow["VENDEDOR"].ToString().Trim(), Tipo = DbType.String },
                                                                                new clsCampo { Nome = "COD_GER", Valor = oRow["COD GER"], Tipo = DbType.Int16 },
                                                                                new clsCampo { Nome = "COD_SUP", Valor = oRow["COD SUP"], Tipo = DbType.Int16 },
                                                                                new clsCampo { Nome = "VENDA_DIA", Valor = Convert.ToDouble(oRow["VENDA DIA"]), Tipo = DbType.Decimal },
                                                                                new clsCampo { Nome = "PDV_A_VST", Valor = Convert.ToDouble(oRow["PDV  A VST"]), Tipo = DbType.Decimal },
                                                                                new clsCampo { Nome = "POSITIVACAO", Valor = Convert.ToDouble(oRow["POSITIVACAO"]), Tipo = DbType.Decimal },
                                                                                new clsCampo { Nome = "POSIT", Valor = Convert.ToDouble(oRow["% POSIT"]), Tipo = DbType.Decimal },
                                                                                new clsCampo { Nome = "DDD", Valor = oRow["DDD"].ToString().Trim(), Tipo = DbType.String },
                                                                                new clsCampo { Nome = "TELEFONE", Valor = oRow["TELEFONE"].ToString().Trim(), Tipo = DbType.String },
                                                                                new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
        }
        else
        {
          sSql = "UPDATE tb_USP_ACOMP_D_VENDA_SOFIA" +
                 " SET VENDEDOR = #VENDEDOR," +
                     "[COD GER] = #COD_GER," +
                     "[COD SUP] = #COD_SUP," +
                     "[VENDA DIA] = #VENDA_DIA," +
                     "[PDV  A VST] = #PDV_A_VST," +
                     "[POSITIVACAO] = #POSITIVACAO," +
                     "[% POSIT] = #POSIT," +
                     "[DDD] = #DDD," +
                     "[TELEFONE] = #TELEFONE," +
                     "[DTINTEGRACAO] = #DTINTEGRACAO," +
                     "[VERSAO_INTEGRADOR] = #VERSAO_INTEGRADOR" +
                 " WHERE LTRIM(RTRIM([COD PUXADA])) = '" + oRow["COD PUXADA"].ToString() + "'" +
                   " AND SETOR = " + oRow["SETOR"].ToString();
          bOk = oBancoDados_Destino.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "VENDEDOR", Valor = oRow["VENDEDOR"].ToString().Trim(), Tipo = DbType.String },
                                                                                new clsCampo { Nome = "COD_GER", Valor = oRow["COD GER"], Tipo = DbType.Int16 },
                                                                                new clsCampo { Nome = "COD_SUP", Valor = oRow["COD SUP"], Tipo = DbType.Int16 },
                                                                                new clsCampo { Nome = "VENDA_DIA", Valor = Convert.ToDouble(oRow["VENDA DIA"]), Tipo = DbType.Decimal },
                                                                                new clsCampo { Nome = "PDV_A_VST", Valor = Convert.ToDouble(oRow["PDV  A VST"]), Tipo = DbType.Decimal },
                                                                                new clsCampo { Nome = "POSITIVACAO", Valor = Convert.ToDouble(oRow["POSITIVACAO"]), Tipo = DbType.Decimal },
                                                                                new clsCampo { Nome = "POSIT", Valor = Convert.ToDouble(oRow["% POSIT"]), Tipo = DbType.Decimal },
                                                                                new clsCampo { Nome = "DDD", Valor = oRow["DDD"].ToString().Trim(), Tipo = DbType.String },
                                                                                new clsCampo { Nome = "TELEFONE", Valor = oRow["TELEFONE"].ToString().Trim(), Tipo = DbType.String },
                                                                                new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});
        }
      }

      return bOk;
    }

    private static void Log_Integrador_Registar(string sAplicativo,
                                                string sPartner,
                                                string sCOD_PUXADA,
                                                string sTIPO_REGISTRO,
                                                string sTIPO_CONSULTA,
                                                string sConexao,
                                                string sCdServico,
                                                int iEmpresa,
                                                int iIdTarefa,
                                                string sLog)
    {
      oBancoDados.DBSQL_Integrador_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                              iEmpresa, iIdTarefa, sLog);
    }

    private static void Log_Registar(string sAplicativo,
                                     string sPartner,
                                     string sCOD_PUXADA,
                                     string sTIPO_REGISTRO,
                                     string sTIPO_CONSULTA,
                                     string sConexao,
                                     string sCdServico,
                                     int iEmpresa,
                                     LogTipo Tipo,
                                     int iIdTarefa,
                                     string sLog,
                                     string sNomeArquivo)
    {
      oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                   iEmpresa, Tipo, iIdTarefa, sNomeArquivo + " >> " + sLog);
    }

    private static void Tools_Integrador(string sAplicativo,
                                         string sPartner,
                                         string sCdServico,
                                         long iIdTarefa,
                                         long iGrupoTarefas,
                                         string sCOD_PUXADA,
                                         string sAcao,
                                         string sMensagem,
                                         string sNomeIntegrador,
                                         string sComplemento,
                                         string sApi,
                                         long nRegistros,
                                         long nRegistrosTotal)
    {
      string sStatus = "";
      string sErro = "";
      string sConexao;
      string sJsonRet = "";

      sConexao = Integrador_Funcoes.sApiLog;
      sConexao = sConexao.Replace("[COD_PUXADA]", sCOD_PUXADA);
      sConexao = sConexao.Replace("[Acao]", sAcao);
      sConexao = sConexao.Replace("[Mensagem]", sMensagem);
      sConexao = sConexao.Replace("[NomeIntegrador]", sNomeIntegrador);
      sConexao = sConexao.Replace("[Complemento]", sComplemento);
      sConexao = sConexao.Replace("[Api]", sApi);
      sConexao = sConexao.Replace("[nRegistros]", nRegistros.ToString());
      sConexao = sConexao.Replace("[nRegistrosTotal]", nRegistrosTotal.ToString());
      sConexao = sConexao.Replace("[iGrupoTarefas]", iGrupoTarefas.ToString());
      sConexao = sConexao.Replace("[Protocolo]", "''");

      FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico, ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa,
                                      "", sConexao, sCOD_PUXADA, "", "", "", ref sJsonRet, null, false);
    }

    private static Boolean DBSQL_Tarefas_AutoAgendar_Gravar(string sAplicativo,
                                                            string sPartner,
                                                            string sCdServico,
                                                            string sCOD_PUXADA,
                                                            string sTIPO_REGISTRO,
                                                            string sTIPO_CONSULTA,
                                                            string sConexao,
                                                            int iId_Tarefa)
    {
      Boolean bOk = false;

      try
      {
        oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_tarefas_autoagendar_cad", new clsCampo[] { new clsCampo { Nome = "p_idTarefa", Tipo = DbType.Double, Valor = iId_Tarefa } });

        bOk = true;
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNoBancoDados, iId_Tarefa, Ex.Message);
      }

      return bOk;
    }

    private static void Tarefa_Concluir(long iIdTarefa,
                                        long iGrupoTarefas,
                                        string sAplicativo,
                                        string sPartner,
                                        string sCdServico,
                                        string sCOD_PUXADA,
                                        string sTarefa,
                                        string sJson,
                                        int nr_ordem_execucao,
                                        bool Log_Tools_Integrador)
    {
      oBancoDados.DBDesconectar();
      oBancoDados.DBConectar(Config_App.sDB_Tipo, Config_App.sDB_StringConexao);
      oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_tarefas_final", new clsCampo[] { new clsCampo { Nome = "idTarefa", Tipo = DbType.Double, Valor = iIdTarefa },
                                                                                                new clsCampo { Nome = "json", Tipo = DbType.String, Valor = sJson },
                                                                                                new clsCampo { Nome = "tpFim", Tipo = DbType.String, Valor = "N",  Direcao = ParameterDirection.Output }});

      if (Log_Tools_Integrador)
      {
        Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processo - Fim", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), "", 0, 0);

        try
        {
          if (oBancoDados.Retorno.ParametroRetorno != null)
          {
            if (oBancoDados.Retorno.Campo("tpFim").Valor.ToString() == "S")
            {
              Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Fim", sCOD_PUXADA, Config_App.sProcessador, nr_ordem_execucao.ToString(), "", 0, 0);
            }
          }
        }
        catch (Exception)
        {
        }
      }
    }

    private static void Tarefa_Iniciar(long iIdTarefa,
                                       long iGrupoTarefas,
                                       int iId_Integrador,
                                       string sAplicativo,
                                       string sPartner,
                                       string sCdServico,
                                       string sCOD_PUXADA,
                                       string sTarefa,
                                       int nr_ordem_execucao,
                                       bool Log_Tools_Integrador)
    {
      oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_tarefas_inico", new clsCampo[] { new clsCampo { Nome = "idTarefa", Tipo = DbType.Double, Valor = iIdTarefa },
                                                                                                new clsCampo { Nome = "idIntegrador", Tipo = DbType.Double, Valor = iId_Integrador },
                                                                                                new clsCampo { Nome = "tpInicio", Tipo = DbType.String, Valor = "N", Direcao = ParameterDirection.Output }});

      if (Log_Tools_Integrador)
      {
        try
        {
          if (oBancoDados.Retorno.ParametroRetorno != null)
          {
            if (oBancoDados.Retorno.Campo("tpInicio").Valor.ToString() == "S")
            {
              Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Início", sCOD_PUXADA, Config_App.sProcessador, nr_ordem_execucao.ToString(), "", 0, 0);
            }
          }
        }
        catch (Exception)
        {
        }

        Tools_Integrador(sAplicativo, sPartner, sCdServico, iIdTarefa, iGrupoTarefas, sCOD_PUXADA, "Processo - Início", sTarefa, Config_App.sProcessador, nr_ordem_execucao.ToString(), "", 0, 0);
      }
    }

    private static void Error(string Local, string Erro)
    {

    }

    private static void Pausar()
    {
      //if (Pausa != null)
      //   Pausa.Invoke();
    }

    private static double BancoDados_Migracao_Calcular(object Valor, string sCalculo)
    {
      double ret = 0;

      try
      {
        sCalculo = sCalculo.Trim();

        switch (sCalculo.Substring(0, 1))
        {
          case "*":
            ret = Convert.ToDouble(Valor) * Convert.ToDouble(sCalculo.Substring(1));
            break;
          case "/":
            ret = Convert.ToDouble(Valor) / Convert.ToDouble(sCalculo.Substring(1));
            break;
          case "-":
            ret = Convert.ToDouble(Valor) - Convert.ToDouble(sCalculo.Substring(1));
            break;
          case "+":
            ret = Convert.ToDouble(Valor) + Convert.ToDouble(sCalculo.Substring(1));
            break;
        }
      }
      catch (Exception)
      {
        ret = 0;
      }

      return ret;
    }

    private static void BancoDados_Migracao_Log(int idEmpresaIntegracao,
                                                int idParceiro,
                                                string scodPuxada,
                                                string swebservice,
                                                string sdsStatus,
                                                string sdsErro,
                                                double dTempoConsumoAPI,
                                                double dTempoProcessamentoIntegrador,
                                                string sParametro_01,
                                                string sParametro_02 = " ",
                                                string sParametro_03 = " ",
                                                string sParametro_04 = " ",
                                                string sParametro_05 = " ")
    {
      Integrador_Funcoes.oBancoDados.DBDesconectar();
      Integrador_Funcoes.oBancoDados.DBConectar(Config_App.sDB_Tipo, Config.dbconstring);
      Integrador_Funcoes.oBancoDados.DBProcedure("sp_Status_Service_cad", new clsCampo[] {
                                                                                   new clsCampo {Nome = "idEmpresaIntegracao", Tipo = DbType.Int16, Valor = idEmpresaIntegracao.GetHashCode() },
                                                                                   new clsCampo {Nome = "idParceiro", Tipo = DbType.Int16, Valor = idParceiro.GetHashCode() },
                                                                                   new clsCampo {Nome = "idIntegrador", Tipo = DbType.Int16, Valor = Config_App.idIntegrador },
                                                                                   new clsCampo {Nome = "codPuxada", Tipo = DbType.String, Valor = scodPuxada},
                                                                                   new clsCampo {Nome = "webservice", Tipo = DbType.String, Valor = swebservice},
                                                                                   new clsCampo {Nome = "dataUltStatus", Tipo = DbType.DateTime, Valor = DateTime.Now },
                                                                                   new clsCampo {Nome = "dsStatus", Tipo = DbType.String, Valor = sdsStatus },
                                                                                   new clsCampo {Nome = "dsErro", Tipo = DbType.String, Valor = sdsErro },
                                                                                   new clsCampo {Nome = "TempoConsumoAPI", Tipo = DbType.Double, Valor = dTempoConsumoAPI },
                                                                                   new clsCampo {Nome = "TempoProcessamentoIntegrador", Tipo = DbType.Double, Valor = dTempoProcessamentoIntegrador },
                                                                                   new clsCampo {Nome = "Parametro_01", Tipo = DbType.String, Valor = sParametro_01 },
                                                                                   new clsCampo {Nome = "Parametro_02", Tipo = DbType.String, Valor = sParametro_02 },
                                                                                   new clsCampo {Nome = "Parametro_03", Tipo = DbType.String, Valor = sParametro_03 },
                                                                                   new clsCampo {Nome = "Parametro_04", Tipo = DbType.String, Valor = sParametro_04 },
                                                                                   new clsCampo {Nome = "Parametro_05", Tipo = DbType.String, Valor = sParametro_05 }});

    }

    private static bool BancoDados_Migracao(string sAplicativo,
                                            string sPartner,
                                            string sCdServico,
                                            int iIdEmpresa,
                                            int iIdTipoIntegracao,
                                            int iIdEmpresaIntegracao,
                                            int iIdTarefa,
                                            string sCOD_PUXADA,
                                            string sTIPO_REGISTRO,
                                            string sTIPO_CONSULTA,
                                            string sVISAO_FATURAMENTO,
                                            string sFILTRO_EXCLUSAO,
                                            string sFILTRO_SELECAO,
                                            string sQUERY_ATUALIZACAO,
                                            string sDS_StringConexaoOrigem,
                                            string sTP_BancoDadosOrigem,
                                            string sDS_StringConexaoDestino,
                                            string sTP_BancoDadosDestino,
                                            string sDS_Origem,
                                            string sDS_Destino,
                                            string sTarefa,
                                            string sTP_LeituraDados)
    {
      bool bOk = false;

      Integrador_Funcoes.sStatus = "";
      Integrador_Funcoes.sErro = "";
      Integrador_Funcoes.TempoExecucaoAPI = 0;
      Integrador_Funcoes.TempoExecucaoIntegrador = 0;

      DateTime dUtil = DateTime.Now;

      try
      {
        string sPonto = "Início";
        sPonto = "";
        UltimoErro = "";

        if (sPonto.Trim() != "") { Config.appLog_Escrever("BancoDados_Migracao - " + sPonto); }

        DataTable oData_Origem = new DataTable();
        DataTable oData_DePara;
        DataRow[] oRows;
        clsBancoDados oBancoDados_Origem = new clsBancoDados();
        clsBancoDados oBancoDados_Destino = new clsBancoDados();
        clsCampo[] oParamento_InsertUpdate = null;
        clsCampo[] oParamento_Where = null;
        int iAux_InsertUpdate = 0;
        int iAux_Where = 0;
        bool bTabelaApagada = false;
        bool bInserir = false;
        bool bTemCampoChave = false;

        string sSql = "";
        string sSql_Insert = "";
        string sSql_Update = "";
        string sSql_Where = "";
        string sColuna = "";
        CommandType oTipoComando = CommandType.Text;

        Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - Início";

        Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - Conectado Origem";
        oBancoDados_Destino.DBConectar(sTP_BancoDadosDestino, sDS_StringConexaoDestino);

        if (oBancoDados_Destino.DBConectado())
        {
          Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - Conectado Destino";
        }
        else
        {
          Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - Não Conectado Destino " + oBancoDados_Destino.DBErro();
        }

        switch (sTP_LeituraDados)
        {
          case Constantes.const_TipoLeituraDados_DBProc:
          case Constantes.const_TipoLeituraDados_DBTabela:
          case Constantes.const_TipoLeituraDados_DBQuery:
            {
              oBancoDados_Origem.DBConectar(sTP_BancoDadosOrigem, sDS_StringConexaoOrigem);
              if (oBancoDados_Origem.DBConectado())
              {
                if (oBancoDados_Origem.DBConectado() && oBancoDados_Destino.DBConectado())
                {
                  //Montagem select de consulta
                  switch (sTP_LeituraDados)
                  {
                    case Constantes.const_TipoLeituraDados_DBQuery:     //Banco de Dados - Query
                      {
                        Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - TipoLeituraDados_DBQuery";
                        sSql = sDS_Origem;
                        oTipoComando = CommandType.Text;
                        break;
                      }
                    case Constantes.const_TipoLeituraDados_DBTabela:     //Banco de Dados - Tabela
                      {
                        Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - TipoLeituraDados_DBTabela";
                        sSql = "select * from " + sDS_Origem;
                        oTipoComando = CommandType.TableDirect;
                        break;
                      }
                    case Constantes.const_TipoLeituraDados_DBProc:     //Banco de Dados - Tabela
                      {
                        Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - TipoLeituraDados_DBProc";
                        sSql = sDS_Origem;
                        oTipoComando = CommandType.StoredProcedure;
                        break;
                      }
                  }

                  oData_Origem = oBancoDados_Origem.DBQuery(sSql, oTipoComando);
                }
              }
              else
              {
                Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - Não Conectado Origem (" + sDS_StringConexaoOrigem + ") + " + oBancoDados_Origem.DBErro();
              }

              break;
            }
          case Constantes.const_TipoLeituraDados_WSJson:
            {
              string sJsonRet = "";

              oData_Origem = FlexXTools.FlexXTools_DataTable(sAplicativo, sPartner, sCdServico, ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa, sDS_Origem, sDS_StringConexaoOrigem,
                                                             sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sVISAO_FATURAMENTO, ref sJsonRet);

              break;
            }
        }

        if (oBancoDados_Destino.DBConectado())
        {
          sDS_Destino = sDS_Destino.Trim();
          Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - oBancoDados_Origem.DBQuery";

          //Carregar De-Para
          oData_DePara = Integrador_Funcoes.oBancoDados.DBQuery("select * from tb_empresas_integracao_depara where idEmpresaIntegracao = " + iIdEmpresaIntegracao.ToString());
          Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - oData_DePara.DBQuery";

          if (oData_DePara.Rows.Count == 0)
          {
            sSql = "select ti.*" +
                  " from tb_tipo_integracao_depara ti" +
                   " inner join tb_empresas_integracao ei on ei.idtipointegracao = ti.idtipointegracao" +
                   " where ei.idEmpresaIntegracao = " + iIdEmpresaIntegracao.ToString();

            oData_DePara = Integrador_Funcoes.oBancoDados.DBQuery(sSql);
            Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - oData_DePara.DBQuery";
          }

          if (sDS_Destino.Substring(sDS_Destino.Length - 1, 1) == "*")
          {
            sDS_Destino = sDS_Destino.Substring(0, sDS_Destino.Length - 1);
            bTabelaApagada = true;
          }

          if (oData_DePara.Rows.Count != 0)
          {
            Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - Tem Data_DePara";

            if (bTabelaApagada)
            {
              sSql = "delete from " + sDS_Destino;
              sSql_Where = "";

              if (sCOD_PUXADA.Trim() != "")
              {
                if (oBancoDados_Destino.sTipoBancoDados == BancoDados_Constantes.const_TipoBancoDados_MySql)
                {
                  _Funcoes.FNC_Str_Adicionar(ref sSql_Where, " TRIM(COD_PUXADA) = '" + sCOD_PUXADA.Trim() + "'", " AND ");
                }
                else if (oBancoDados_Destino.sTipoBancoDados == BancoDados_Constantes.const_TipoBancoDados_SqlServer)
                {
                  _Funcoes.FNC_Str_Adicionar(ref sSql_Where, " RTRIM(COD_PUXADA) = '" + sCOD_PUXADA.Trim() + "'", " AND ");
                }
              }

              if (sFILTRO_EXCLUSAO.Trim() != "")
              {
                _Funcoes.FNC_Str_Adicionar(ref sSql_Where, sFILTRO_EXCLUSAO, " AND ");
              }

              if (sSql_Where != "")
              {
                sSql = sSql + " where " + sSql_Where;
              }

              Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - " + sSql;
              oBancoDados_Destino.DBExecutar(sSql);
              Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - Deltados registros";
            }

            sSql_Where = "";

            if (oData_Origem != null)
            {
              bOk = (oData_Origem.Rows.Count == 0);

              if (sFILTRO_SELECAO.Trim() != "")
              {
                oRows = oData_Origem.Select(sFILTRO_SELECAO);
              }
              else
              {
                oRows = oData_Origem.Select();
              }

              Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - Processando";
              foreach (DataRow oRow in oRows)
              {
                try
                {
                  bInserir = true;
                  oParamento_InsertUpdate = null;
                  oParamento_Where = null;
                  iAux_InsertUpdate = 2;
                  iAux_Where = 0;
                  sSql_Where = "";
                  sSql_Insert = "";
                  sSql_Update = "";

                  FNC_Str_Adicionar(ref sSql_Update, "DTINTEGRACAO=#DTINTEGRACAO", ", ");
                  FNC_Str_Adicionar(ref sSql_Update, "VERSAO_INTEGRADOR=#VERSAO_INTEGRADOR", ",");
                  FNC_Str_Adicionar(ref sSql_Insert, "#DTINTEGRACAO", ", ");
                  FNC_Str_Adicionar(ref sSql_Insert, "#VERSAO_INTEGRADOR", ", ");
                  Array.Resize(ref oParamento_InsertUpdate, iAux_InsertUpdate);
                  oParamento_InsertUpdate[0] = new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime };
                  oParamento_InsertUpdate[1] = new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String };

                  if (sCOD_PUXADA.Trim() != "")
                  {
                    FNC_Str_Adicionar(ref sSql_Update, "COD_PUXADA=#COD_PUXADA", ",");
                    FNC_Str_Adicionar(ref sSql_Insert, "#COD_PUXADA", ", ");

                    Array.Resize(ref oParamento_InsertUpdate, iAux_InsertUpdate + 1);
                    oParamento_InsertUpdate[iAux_InsertUpdate] = new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String };
                    iAux_InsertUpdate = iAux_InsertUpdate + 1;
                  }

                  //Verifica se existem campos chaves para poder verificar se existe
                  foreach (DataRow oRow_DePara in oData_DePara.Rows)
                  {
                    sColuna = oRow_DePara["no_campo_origem"].ToString();

                    if (!bTabelaApagada)
                    {
                      //Se for campo chave é colocado no Where
                      if (oRow_DePara["ic_campo_chave"].ToString() == "S")
                      {
                        bTemCampoChave = true;

                        Array.Resize(ref oParamento_Where, iAux_Where + 1);
                        oParamento_Where[iAux_Where] = new clsCampo { Nome = oRow_DePara["no_campo_destino"].ToString(), Valor = oRow[sColuna], Tipo = _Funcoes.SystemType_para_DbType(oRow[sColuna].GetType()) };
                        FNC_Str_Adicionar(ref sSql_Where, oRow_DePara["no_campo_destino"].ToString() + "=#" + oRow_DePara["no_campo_destino"].ToString(), " and ");
                        iAux_Where = iAux_Where + 1;
                      }
                      //Se não for campo chave é colocado no Set
                      else
                      {
                        FNC_Str_Adicionar(ref sSql_Update, oRow_DePara["no_campo_destino"].ToString() + "=#" + oRow_DePara["no_campo_destino"].ToString(), ",");
                      }
                    }

                    //Montar o insert
                    Array.Resize(ref oParamento_InsertUpdate, iAux_InsertUpdate + 1);
                    if (_Funcoes.FNC_NuloString(oRow_DePara["ds_calculo"]) == "")
                    {
                      switch (sColuna)
                      {
                        case "[DATA_ATUAL]":
                          oParamento_InsertUpdate[iAux_InsertUpdate] = new clsCampo { Nome = oRow_DePara["no_campo_destino"].ToString(), Valor = oBancoDados_Destino.DBData().Date, Tipo = DbType.Date };
                          break;
                        default:
                          oParamento_InsertUpdate[iAux_InsertUpdate] = new clsCampo { Nome = oRow_DePara["no_campo_destino"].ToString(), Valor = oRow[sColuna], Tipo = _Funcoes.SystemType_para_DbType(oRow[sColuna].GetType()) };
                          break;
                      }
                    }
                    else
                    {
                      oParamento_InsertUpdate[iAux_InsertUpdate] = new clsCampo { Nome = oRow_DePara["no_campo_destino"].ToString(), Valor = BancoDados_Migracao_Calcular(oRow[sColuna], oRow_DePara["ds_calculo"].ToString()), Tipo = _Funcoes.SystemType_para_DbType(oRow[sColuna].GetType()) };
                    }
                    FNC_Str_Adicionar(ref sSql_Insert, "#" + oRow_DePara["no_campo_destino"].ToString(), ",");
                    iAux_InsertUpdate = iAux_InsertUpdate + 1;
                  }

                  if (sCOD_PUXADA.Trim() != "" && bTemCampoChave)
                  {
                    Array.Resize(ref oParamento_Where, iAux_Where + 1);
                    oParamento_Where[iAux_Where] = new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String };
                    FNC_Str_Adicionar(ref sSql_Where, "COD_PUXADA=#COD_PUXADA", " and ");
                    iAux_Where = iAux_Where + 1;
                  }

                  //Verifica se existe campos chaves
                  if (sSql_Where.Trim() != "")
                  {
                    sSql = "select count(*) from " + sDS_Destino + " where " + sSql_Where;
                    if (Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, 0, null, oParamento_Where)) != 0)
                    {
                      bInserir = false;
                    }
                  }

                  if (bInserir)
                  {
                    sSql = "insert " + sDS_Destino + "(" + sSql_Insert.Replace("#", "") + ") values (" + sSql_Insert + ")";
                  }
                  else
                  {
                    sSql = "update " + sDS_Destino + " set " + sSql_Update + " where " + sSql_Where;
                  }

                  bOk = oBancoDados_Destino.DBExecutar(sSql, oParamento_InsertUpdate);

                  if (!bOk)
                  {
                    Integrador_Funcoes.UltimoErro = "BancoDados_Migracao - Erro";
                    sPonto = "(DBErro) " + oBancoDados_Destino.DBErro();
                    Integrador_Funcoes.UltimoErro = sPonto;
                    break;
                  }
                  else
                  {
                    sSql = "update " + sQUERY_ATUALIZACAO + sSql_Where;
                  }

                  Integrador_Funcoes.UltimoErro = "";
                }
                catch (Exception Ex)
                {
                  Integrador_Funcoes.UltimoErro = Ex.Message;
                  Config.appLog_Escrever("Erro na rotina BancoDados_Migracao [" + sTarefa + "]: " + Ex.Message.ToString());
                }
              }

              Integrador_Funcoes.UltimoErro = "";
            }
          }
        }

        if (sPonto.Trim() != "") { Config.appLog_Escrever("BancoDados_Migracao - " + sPonto); }

        oBancoDados_Origem.DBDesconectar();
        oBancoDados_Destino.DBDesconectar();
        oBancoDados_Origem = null;
        oBancoDados_Destino = null;
      }
      catch (Exception Ex)
      {
        Integrador_Funcoes.UltimoErro = Ex.Message;
        Config.appLog_Escrever("Erro na rotina BancoDados_Migracao." + Ex.Message.ToString());
      }

      TimeSpan ts = DateTime.Now - dUtil;

      TempoExecucaoIntegrador = ts.TotalSeconds;

      return bOk;
    }

    private static bool Integrador_ProcessarMensagem(int iIdEmpresa)
    {
      bool bOk = false;
      string sSql = "";
      DataTable oData01;
      DataTable oData02;
      string[] Lista = null;
      string[] Nome = null;
      string sNome = "";
      string sPonto = "";

      try
      {
        sPonto = "Integrador_ProcessarMensagem (01)";

        sSql = "select *" +
               " from tb_mensagem" +
               " where id_Empresa = " + iIdEmpresa.ToString() +
                 " and isnull(tp_Processado, 'N') = 'N'";
        sPonto = "Integrador_ProcessarMensagem (01.1) - " + sSql;
        oData01 = oBancoDados.DBQuery(sSql);
        sPonto = "Integrador_ProcessarMensagem (01.2) - " + sSql;

        if (oData01 != null)
        {
          if (oData01.Rows.Count == 0)
          {
            bOk = true;
          }
          else
          {
            sPonto = "Integrador_ProcessarMensagem (01.4) - " + sSql + oData01.ToString();
            foreach (DataRow oRow01 in oData01.Rows)
            {
              sPonto = "Integrador_ProcessarMensagem (02)";
              if (_Funcoes.FNC_NuloString(oRow01["no_usuario"].ToString()) != "")
              {
                sPonto = "Integrador_ProcessarMensagem (03)";
                Nome = _Funcoes.FNC_NuloString(oRow01["no_usuario"]).Split(';');
              }

              if (_Funcoes.FNC_NuloString(oRow01["cd_usuario"].ToString()) != "")
              {
                sPonto = "Integrador_ProcessarMensagem (04)";
                Lista = _Funcoes.FNC_NuloString(oRow01["cd_usuario"]).Split(';');

                for (int i = 0; i < Lista.Length; i++)
                {
                  sPonto = "Integrador_ProcessarMensagem (05)";
                  if (Nome == null)
                  { sNome = ""; }
                  else
                  { sNome = Nome[i].Trim(); }

                  sSql = "SELECT distinct Nome, telegram, telefone" +
                          " FROM tb_Usuario" +
                          " WHERE idEmpresa = " + iIdEmpresa.ToString();

                  sPonto = "Integrador_ProcessarMensagem (06)";
                  if (oRow01["cd_provider_destino"].ToString() == Constantes.const_Provider_Telegram)
                  { sSql = sSql + " AND telegram in ('" + Lista[i].Trim() + "')"; }
                  else
                  { sSql = sSql + " AND telefone in ('" + Lista[i].Trim() + "')"; }
                  oData02 = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

                  sPonto = "Integrador_ProcessarMensagem (07)";
                  if (oData02.Rows.Count == 0)
                  {
                    sPonto = "Integrador_ProcessarMensagem (08)";
                    bOk = Bot.Webook_Util(1, _Funcoes.FNC_NuloString(oRow01["Servico"]),
                                             oRow01["ds_Termo"].ToString(),
                                             oRow01["ds_Mensagem"].ToString(),
                                             oRow01["cd_provider_destino"].ToString(),
                                             sNome,
                                             Lista[i].Trim(),
                                             oRow01["ds_bot"].ToString(), "");
                  }
                  else
                  {
                    if (oRow01["cd_provider_destino"].ToString() == Constantes.const_Provider_Telegram)
                    {
                      sPonto = "Integrador_ProcessarMensagem (09)";
                      bOk = Bot.Webook_Util(1, _Funcoes.FNC_NuloString(oRow01["Servico"]),
                                               oRow01["ds_Termo"].ToString(),
                                               oRow01["ds_Mensagem"].ToString(),
                                               oRow01["cd_provider_destino"].ToString(),
                                               oData02.Rows[0]["Nome"].ToString(),
                                               oData02.Rows[0]["telegram"].ToString(),
                                               oRow01["ds_bot"].ToString(), "");
                    }
                    else
                    {
                      sPonto = "Integrador_ProcessarMensagem (10) " + Config_App.sWebHook_Url;
                      bOk = Bot.Webook_Util(1, _Funcoes.FNC_NuloString(oRow01["Servico"]),
                                               oRow01["ds_Termo"].ToString(),
                                               oRow01["ds_Mensagem"].ToString(),
                                               oRow01["cd_provider_destino"].ToString(),
                                               oData02.Rows[0]["Nome"].ToString(),
                                               oData02.Rows[0]["telefone"].ToString(),
                                               oRow01["ds_bot"].ToString(), "");
                    }
                  }
                }
              }

              sPonto = "Integrador_ProcessarMensagem (11)";
              if (_Funcoes.FNC_NuloString(oRow01["tp_usuarios"].ToString()) != "")
              {
                sPonto = "Integrador_ProcessarMensagem (12)";
                Lista = _Funcoes.FNC_NuloString(oRow01["tp_usuarios"]).Split(';');

                foreach (string sLista in Lista)
                {
                  sPonto = "Integrador_ProcessarMensagem (13)";
                  sSql = "SELECT distinct Nome, telegram, telefone " + sSql +
                          " FROM tb_Usuario" +
                          " WHERE id_Empresa = " + iIdEmpresa.ToString() +
                              " AND tp_Usuario in (" + _Funcoes.FNC_ListaParaListaComAspa(oRow01["tp_usuarios"].ToString(), ";", ",") + ")";
                  oData02 = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

                  sPonto = "Integrador_ProcessarMensagem (14)";
                  foreach (DataRow oRow02 in oData02.Rows)
                  {
                    sPonto = "Integrador_ProcessarMensagem (15)";
                    if (oRow01["cd_provider_destino"].ToString() == Constantes.const_Provider_Telegram)
                    {
                      sPonto = "Integrador_ProcessarMensagem (16)";
                      bOk = Bot.Webook_Util(1, _Funcoes.FNC_NuloString(oRow01["Servico"]),
                                                  oRow01["ds_Termo"].ToString(),
                                                  oRow01["ds_Mensagem"].ToString(),
                                                  oRow01["cd_provider_destino"].ToString(),
                                                  oData02.Rows[0]["Nome"].ToString(),
                                                  oData02.Rows[0]["telegram"].ToString(),
                                                  oRow01["ds_bot"].ToString(), "");
                    }
                    else
                    {
                      sPonto = "Integrador_ProcessarMensagem (17)";
                      bOk = Bot.Webook_Util(1, _Funcoes.FNC_NuloString(oRow01["Servico"]),
                                                  oRow01["ds_Termo"].ToString(),
                                                  oRow01["ds_Mensagem"].ToString(),
                                                  oRow01["cd_provider_destino"].ToString(),
                                                  oData02.Rows[0]["Nome"].ToString(),
                                                  oData02.Rows[0]["telefone"].ToString(),
                                                  oRow01["ds_bot"].ToString(), "");
                    }
                  }
                }
              }

              if (bOk)
              {
                sPonto = "Integrador_ProcessarMensagem (18)";
                sSql = "UPDATE " + Config_App.sDB_BancoDados + ".tb_mensagem SET tp_Processado = 'S' WHERE id_Mensagem = " + oRow01["id_Mensagem"].ToString();
                Integrador_Funcoes.oBancoDados.DBExecutar(sSql);
              }
            }
          }
        }
        else
        {
          bOk = true;
        }
      }
      catch (Exception Ex)
      {
        Config.appLog_Escrever(sPonto + " - " + Ex.Message);
      }

      return bOk;
    }

    private static void app_botautocadastro_LeadExport_Mensagem(int iEmpresa,
                                                                string sEMail,
                                                                string sAssunto,
                                                                string sMensagem,
                                                                string sCodigoAtivacao = "")
    {
      DataTable oData;
      string sSql = "";
      string sMensagem_Ativacao = "";
      string sMensagem_Enviar = "";

      if (sCodigoAtivacao != "")
        sMensagem_Ativacao = sMensagem + Environment.NewLine + Environment.NewLine +
                             "Link de ativação: " + Propriedade_Ler(const_Propriedade_Notificacao_WebServiceAtivacao).ToString() + sCodigoAtivacao;
      else
        sMensagem_Ativacao = sMensagem;

      sSql = "select * from vw_usuario_notificacao where (idEmpresa = " + iEmpresa.ToString() + " and Tipo in ('G')) or Tipo in ('N', 'T')";
      oData = Integrador_Funcoes.oBancoDados.DBQuery(sSql);

      try
      {
        if (sEMail != "")
        {
          FNC_Enviar(sEMail, "", sAssunto, sMensagem);
        }

        foreach (DataRow oRow in oData.Rows)
        {
          if (FNC_NuloString(oRow["Tipo"]).Trim() != "N" || FNC_NuloString(oRow["Tipo"]).Trim() != "T")
            sMensagem_Enviar = sMensagem_Ativacao.Trim() + "." + String.Format("{0:D4}", FNC_NuloString(oRow["Id"]));
          else
            sMensagem_Enviar = sMensagem;

          if (FNC_NuloString(oRow["Email"]).Trim() != "")
          {
            FNC_Enviar(oRow["Email"].ToString(), "", sAssunto, sMensagem_Enviar);
          }
          if (FNC_NuloString(oRow["TELEGRAM"]).Trim() != "")
          {
            Integradores.Bot.Webook_Util(1, "HNK", "", sMensagem_Enviar, "TG", oRow["Nome"].ToString(), oRow["TELEGRAM"].ToString(), "alice", "");
          }
          if (FNC_NuloString(oRow["TELEFONE"]).Trim() != "")
          {
            Integradores.Bot.Webook_Util(1, "HNK", "", sMensagem_Enviar, "ZP", oRow["Nome"].ToString(), oRow["TELEFONE"].ToString(), "alice", "");
          }
        }
        //Enviar BOT - Fim
      }
      catch (Exception Ex)
      {
        Config.appLog_Escrever("app_botautocadastro_LeadExport_Mensagem - " + Ex.Message);
      }
    }

    private static bool app_botautocadastro_LeadExport(string sDS_StringConexaoOrigem,
                                                       string sTP_BancoDadosOrigem,
                                                       string sDS_StringConexaoDestino,
                                                       string sTP_BancoDadosDestino)
    {
      bool bOk = false;
      int iIdEmpresa_Gestora;

      string sPonto = "Início";

      Config.appLog_Escrever("app_botautocadastro_LeadExport - " + sPonto);

      try
      {
        DataTable oData_Origem;
        clsBancoDados oBancoDados_Origem = new clsBancoDados();
        clsBancoDados oBancoDados_Destino = new clsBancoDados();

        string sSql = "";
        int iIdEmpresa = 0;
        string sMensagem = "";
        string sMensagem_Informada = "";
        string sMensagem_Faltante = "";
        bool CNPJ_Valido = false;
        string sCodigoAtivacao = "";

        oBancoDados_Origem.DBConectar(sTP_BancoDadosOrigem, sDS_StringConexaoOrigem);
        oBancoDados_Destino.DBConectar(sTP_BancoDadosDestino, sDS_StringConexaoDestino);

        if (oBancoDados_Origem.DBConectado() && oBancoDados_Destino.DBConectado())
        {
          sSql = "select * from LeadExport where isnull(tp_Processado, 'N') = 'N'";
          oData_Origem = oBancoDados_Origem.DBQuery(sSql);

          foreach (DataRow oRow in oData_Origem.Rows)
          {
            bOk = false;
            iIdEmpresa = 0;
            sMensagem = "";
            sMensagem_Informada = "";
            sMensagem_Faltante = "";

            if (FNC_NuloString(oRow["appConfirma"]).Trim() == "?? Sim, Confirmo")
            {
              if (FNC_NuloString(oRow["appCNPJ"]).Trim() != "")
              {
                sSql = "select idempresa from tb_empresas where upper(CNPJ) = '" + oRow["appCNPJ"].ToString().Trim() + "'";
                iIdEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql, iIdEmpresa));

                if (!FNC_CNPJ_Verifica(oRow["appCNPJ"].ToString()))
                {
                  CNPJ_Valido = false;
                  FNC_Str_Adicionar(ref sMensagem_Faltante, "- CNPJ Inválido: " + oRow["appCNPJ"].ToString(), Environment.NewLine);
                }
                else
                {
                  FNC_Str_Adicionar(ref sMensagem_Informada, "CNPJ informado: " + oRow["appCNPJ"].ToString(), Environment.NewLine);
                  CNPJ_Valido = true;
                }
              }
              else
                FNC_Str_Adicionar(ref sMensagem_Faltante, "- CNPJ não informado", Environment.NewLine);

              switch (FNC_NuloString(oRow["Perfil"]).Trim())
              {
                case "sou Parceiro":
                  {
                    switch (FNC_NuloString(oRow["partnerOpcoes"]).Trim())
                    {
                      case "Cadastrar Usuario APP":
                        {
                          if (FNC_NuloString(oRow["appUsuarioNome"]).Trim() == "")
                            FNC_Str_Adicionar(ref sMensagem_Faltante, "- Nome do usuário não informado", Environment.NewLine);
                          else
                            FNC_Str_Adicionar(ref sMensagem_Informada, "Nome do usuário: " + oRow["appUsuarioNome"].ToString(), Environment.NewLine);
                          if (FNC_NuloString(oRow["appContatoCelular"]).Trim() == "")
                            FNC_Str_Adicionar(ref sMensagem_Faltante, "- Número de celular não informado", Environment.NewLine);
                          else
                            FNC_Str_Adicionar(ref sMensagem_Informada, "Número de celular: " + oRow["appContatoCelular"].ToString(), Environment.NewLine);
                          if (FNC_NuloString(oRow["appPerfil"]).Trim() == "")
                            FNC_Str_Adicionar(ref sMensagem_Faltante, "- Licença não informada", Environment.NewLine);
                          else
                            FNC_Str_Adicionar(ref sMensagem_Informada, "Licença não informada: " + FNC_NuloString(oRow["appPerfil"]).Trim(), Environment.NewLine);

                          if (FNC_NuloString(oRow["appUsuarioSeleciona"]).Trim() == "")
                          {
                            oRow["appUsuarioSeleciona"] = "";
                            FNC_Str_Adicionar(ref sMensagem_Faltante, "- Nenhum aplicativo informado", Environment.NewLine);
                          }
                          else
                          {
                            if (!(FNC_NuloString(oRow["appUsuarioSeleciona"]).Trim() == "Bot + Dash" ||
                                  FNC_NuloString(oRow["appUsuarioSeleciona"]).Trim() == "Dash" ||
                                  FNC_NuloString(oRow["appUsuarioSeleciona"]).Trim() == "Bot"))
                              FNC_Str_Adicionar(ref sMensagem_Faltante, "- Aplicativo informado inexistente", Environment.NewLine);
                          }

                          if (FNC_NuloString(oRow["appUsuarioNome"]).Trim() == "" || FNC_NuloString(oRow["appContatoCelular"]).Trim() == "")
                          {
                            sMensagem = "Usuário não cadastrado por falta de informações principais";
                            if (sMensagem_Informada != "")
                              FNC_Str_Adicionar(ref sMensagem, sMensagem_Informada, Environment.NewLine + Environment.NewLine);
                            if (sMensagem_Faltante != "")
                              FNC_Str_Adicionar(ref sMensagem,
                                                sMensagem_Faltante,
                                                Environment.NewLine + Environment.NewLine);

                            app_botautocadastro_LeadExport_Mensagem(iIdEmpresa,
                                                                    FNC_NuloString(oRow["partnerEmail"]).Trim(),
                                                                    "Cadastrar Usuario APP - Falta de informações",
                                                                    sMensagem);
                          }
                          else if (FNC_NuloString(oRow["appContatoCelular"]).Trim() != "")
                          {
                            if (sMensagem_Faltante.Trim() == "")
                            {
                              sCodigoAtivacao = Constantes.const_Ativar_Usuario + FNC_GerarCodigo();
                              sMensagem = "Usuário cadastrado";
                            }
                            else
                              sMensagem_Informada = "Usuário cadastrado com informações faltando";

                            if (sMensagem_Informada != "")
                              FNC_Str_Adicionar(ref sMensagem, sMensagem_Informada, Environment.NewLine + Environment.NewLine);
                            if (sMensagem_Faltante != "")
                              FNC_Str_Adicionar(ref sMensagem,
                                                sMensagem_Faltante,
                                                Environment.NewLine + Environment.NewLine);

                            string sFLEXXPOWER = "N";
                            string sSOFIA = "N";

                            if (FNC_NuloString(oRow["appUsuarioSeleciona"]).Trim() == "Bot + Dash" || FNC_NuloString(oRow["appUsuarioSeleciona"]).Trim() == "Dash")
                              sFLEXXPOWER = "S";
                            if (FNC_NuloString(oRow["appUsuarioSeleciona"]).Trim() == "Bot + Dash" || FNC_NuloString(oRow["appUsuarioSeleciona"]).Trim() == "Bot")
                              sSOFIA = "S";

                            bOk = oBancoDados_Destino.DBProcedure("sp_Usuario_cad", new clsCampo[] { new clsCampo { Nome = "p_idEmpresa", Tipo = DbType.Double, Valor = iIdEmpresa },
                                                                                                                                 new clsCampo { Nome = "p_tp_Usuario", Tipo = DbType.String, Valor = "C" },
                                                                                                                                 new clsCampo { Nome = "p_Cod_Puxada", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appIntegracao"]) },
                                                                                                                                 new clsCampo { Nome = "p_Nome", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appUsuarioNome"]).Trim() },
                                                                                                                                 new clsCampo { Nome = "p_Senha", Tipo = DbType.String, Valor = "1234" },
                                                                                                                                 new clsCampo { Nome = "p_CodTelefone", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appContatoCelular"]) },
                                                                                                                                 new clsCampo { Nome = "p_Departamento", Tipo = DbType.String, Valor = null },
                                                                                                                                 new clsCampo { Nome = "p_Email", Tipo = DbType.String, Valor = null },
                                                                                                                                 new clsCampo { Nome = "p_Device", Tipo = DbType.String, Valor = null },
                                                                                                                                 new clsCampo { Nome = "p_Ativo", Tipo = DbType.String, Valor = "S" },
                                                                                                                                 new clsCampo { Nome = "p_CNPJ", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appCNPJ"]) },
                                                                                                                                 new clsCampo { Nome = "p_Cod_Empregado", Tipo = DbType.String, Valor = null },
                                                                                                                                 new clsCampo { Nome = "p_FLEXXPOWER", Tipo = DbType.String, Valor = sFLEXXPOWER },
                                                                                                                                 new clsCampo { Nome = "p_LICENCA", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appPerfil"]) },
                                                                                                                                 new clsCampo { Nome = "p_SOFIA", Tipo = DbType.String, Valor = sSOFIA },
                                                                                                                                 new clsCampo { Nome = "p_TELEFONE", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appContatoCelular"]) },
                                                                                                                                 new clsCampo { Nome = "p_TELEGRAM", Tipo = DbType.String, Valor = null },
                                                                                                                                 new clsCampo { Nome = "p_ID_USUARIO", Tipo = DbType.String, Valor = null },
                                                                                                                                 new clsCampo { Nome = "p_VERSAO_INTEGRADOR", Tipo = DbType.String, Valor = ProductVersion },
                                                                                                                                 new clsCampo { Nome = "p_cd_ativacao", Tipo = DbType.String, Valor = sCodigoAtivacao }});
                          }

                          app_botautocadastro_LeadExport_Mensagem(iIdEmpresa,
                                                                  FNC_NuloString(oRow["partnerEmail"]).Trim(),
                                                                  "Cadastrar Usuario APP",
                                                                  sMensagem,
                                                                  sCodigoAtivacao);

                          break;
                        }
                      case "Cadastrar um Cliente Novo":
                        {
                          string sNotificarCliente = "N";
                          string sCNPJ = "";
                          int iId_Servico = 0;

                          if (FNC_NuloString(oRow["appNomeCliente"]).Trim() == "")
                            FNC_Str_Adicionar(ref sMensagem_Faltante, "- Nome da empresa não informada", Environment.NewLine);
                          else
                            FNC_Str_Adicionar(ref sMensagem_Informada, "Nome da empresa: " + oRow["appNomeCliente"].ToString(), Environment.NewLine);
                          if (FNC_NuloString(oRow["appIntegracao"]).Trim() == "")
                            FNC_Str_Adicionar(ref sMensagem_Faltante, "- Código de Puxada não informado", Environment.NewLine);
                          else
                            FNC_Str_Adicionar(ref sMensagem_Informada, "Código de Puxada: " + oRow["appIntegracao"].ToString(), Environment.NewLine);
                          if (FNC_NuloString(oRow["partnerEmpresa"]).Trim() == "")
                            FNC_Str_Adicionar(ref sMensagem_Faltante, "- Partner não informado", Environment.NewLine);
                          else
                            FNC_Str_Adicionar(ref sMensagem_Informada, "Partner: " + oRow["partnerEmpresa"].ToString(), Environment.NewLine);
                          if (FNC_NuloString(oRow["appPerfil"]).Trim() == "")
                            FNC_Str_Adicionar(ref sMensagem_Faltante, "- Serviço não informado", Environment.NewLine);
                          else
                            FNC_Str_Adicionar(ref sMensagem_Informada, "Serviço: " + oRow["appPerfil"].ToString(), Environment.NewLine);

                          sSql = "select idempresa from tb_empresas where upper(Sigla) like '%" + FNC_NuloString(oRow["partnerEmpresa"]).ToUpper().Trim() + ";%'";
                          iIdEmpresa_Gestora = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql));

                          sCNPJ = oRow["appCNPJ"].ToString().Trim().Replace(".", "").Replace("-", "").Replace("/", "");

                          if (FNC_NuloString(oRow["appUsuarioRecebeNotificacao"]).ToUpper() == "SIM" || FNC_NuloString(oRow["appUsuarioRecebeNotificacao"]).ToUpper() == "S")
                          {
                            sNotificarCliente = "S";
                          }

                          if (CNPJ_Valido)
                          {
                            sCodigoAtivacao = Constantes.const_Ativar_Empresa + FNC_GerarCodigo();

                            sMensagem = "Empresa cadastrada";
                            if (sMensagem_Informada != "")
                              FNC_Str_Adicionar(ref sMensagem, sMensagem_Informada, Environment.NewLine + Environment.NewLine);
                            if (sMensagem_Faltante != "")
                              FNC_Str_Adicionar(ref sMensagem,
                                                sMensagem_Faltante,
                                                Environment.NewLine + Environment.NewLine);

                            bOk = oBancoDados_Destino.DBProcedure("sp_empresas_cad", new clsCampo[] { new clsCampo { Nome = "#id_Tipo_Empresa", Tipo = DbType.Double, Valor = 3 },
                                                                                                                                  new clsCampo { Nome = "#idEmpresa_Gestora", Tipo = DbType.Double, Valor = iIdEmpresa_Gestora },
                                                                                                                                  new clsCampo { Nome = "#Empresa", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appNomeCliente"]) },
                                                                                                                                  new clsCampo { Nome = "#CNPJ", Tipo = DbType.String, Valor = sCNPJ },
                                                                                                                                  new clsCampo { Nome = "#Cidade", Tipo = DbType.String, Valor = FNC_NuloString(oRow["City"]) },
                                                                                                                                  new clsCampo { Nome = "#CodTelefone", Tipo = DbType.String, Valor = "" },
                                                                                                                                  new clsCampo { Nome = "#CodPuxada", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appIntegracao"]) },
                                                                                                                                  new clsCampo { Nome = "#Licenca", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appPerfil"]) },
                                                                                                                                  new clsCampo { Nome = "#Contato", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appContatoNome"]) },
                                                                                                                                  new clsCampo { Nome = "#ContatoTelefone", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appContatoCelular"]) },
                                                                                                                                  new clsCampo { Nome = "#ContatoEMail", Tipo = DbType.String, Valor = "" },
                                                                                                                                  new clsCampo { Nome = "#NotificarCliente", Tipo = DbType.String, Valor = sNotificarCliente },
                                                                                                                                  new clsCampo { Nome = "#SistemaLiberado", Tipo = DbType.String, Valor = FNC_NuloString(oRow["appUsuarioSeleciona"]) },
                                                                                                                                  new clsCampo { Nome = "#cd_ativacao", Tipo = DbType.String, Valor = sCodigoAtivacao },
                                                                                                                                  new clsCampo { Nome = "#iAtivou", Tipo = DbType.Int32, Valor = 1 },
                                                                                                                                  new clsCampo { Nome = "@nLicDash", Tipo = DbType.Int32, Valor = 1 }});

                            sSql = "select idempresa from tb_empresas where upper(CNPJ) = '" + sCNPJ + "'";
                            iIdEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql));

                            if (FNC_NuloString(oRow["appPerfil"]).Trim() != "")
                            {
                              sSql = "select id_Servico from tb_servicos where cd_Servico = '" + oRow["appPerfil"].ToString().Trim() + "'";
                              iId_Servico = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSql));

                              if (iId_Servico != 0)
                              {
                                bOk = oBancoDados_Destino.DBProcedure("sp_empresas_servicos_cad", new clsCampo[] { new clsCampo { Nome = "p_id_Empresa", Tipo = DbType.Double, Valor = iIdEmpresa },
                                                                                                                                                   new clsCampo { Nome = "p_id_Servico", Tipo = DbType.Double, Valor = iId_Servico },
                                                                                                                                                   new clsCampo { Nome = "p_tp_Licenciamento", Tipo = DbType.String, Valor = oRow["appPerfil"].ToString() },
                                                                                                                                                   new clsCampo { Nome = "p_tp_ativo", Tipo = DbType.String, Valor = 'S' }});
                              }
                            }

                            app_botautocadastro_LeadExport_Mensagem(iIdEmpresa_Gestora,
                                                                    FNC_NuloString(oRow["partnerEmail"]).Trim(),
                                                                    "Cadastrar um Cliente Novo",
                                                                    sMensagem,
                                                                    sCodigoAtivacao);
                          }
                          else
                          {
                            sMensagem = "Empresa não cadastrada por falta de informações principais";
                            if (sMensagem_Informada != "")
                              FNC_Str_Adicionar(ref sMensagem, sMensagem_Informada, Environment.NewLine + Environment.NewLine);
                            if (sMensagem_Faltante != "")
                              FNC_Str_Adicionar(ref sMensagem,
                                                sMensagem_Faltante,
                                                Environment.NewLine + Environment.NewLine);


                            app_botautocadastro_LeadExport_Mensagem(iIdEmpresa_Gestora,
                                                                    FNC_NuloString(oRow["partnerEmail"]).Trim(),
                                                                    "Cadastrar um Cliente Novo - Falta de informações",
                                                                    sMensagem);
                          }
                          break;
                        }
                    }

                    break;
                  }
                case "Já sou Cliente":
                  {
                    break;
                  }
              }
            }
            else
              bOk = true;

            sSql = "update [app_botautocadastro].[dbo].[LeadExport] set tp_processado = 'S' where idLead = " + oRow["IdLead"].ToString();
            oBancoDados_Origem.DBExecutar(sSql);
          }
        }

        Config.appLog_Escrever("BancoDados_Migracao - " + sPonto);

        oBancoDados_Origem.DBDesconectar();
        oBancoDados_Destino.DBDesconectar();
        oBancoDados_Origem = null;
        oBancoDados_Destino = null;
      }
      catch (Exception Ex)
      {
        Config.appLog_Escrever("app_botautocadastro_LeadExport - " + sPonto + " - " + Ex.Message);
      }

      return bOk;
    }
  }
}