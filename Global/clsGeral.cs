using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using static BancoDados;
using static BancoDados.clsBancoDados;
using static Integradores.Entidades;

namespace Integradores
{
  public static class Constantes
  {
    public const string const_Senha = "gltuWmM5Nr";
    public const string const_Senha_Administrador = "Admin";
    public const string const_Senha_Usuario = "Usuário";

    public const string const_Sistema_Nome = "mrSoft - trade2UP - Integrador";

    public const int const_TipoIntegracao_1_PedFacil = 1;
    public const int const_TipoIntegracao_2_Boomerangue = 2;
    public const int const_TipoIntegracao_3_Lokalizei = 3;
    public const int const_TipoIntegracao_4_MeLeva = 4;
    public const int const_TipoIntegracao_5_DashPlus = 5;
    public const int const_TipoIntegracao_6_FlexxPower = 6;
    public const int const_TipoIntegracao_7_PedFacilREP = 7;
    public const int const_TipoIntegracao_8_B2BWeb = 8;
    public const int const_TipoIntegracao_9_FlagWSWhatsapp_DisDevolucao = 9;
    public const int const_TipoIntegracao_10_FlagWSWhatsapp_DisTroca = 10;
    public const int const_TipoIntegracao_11_FlagWSWhatsapp_DisIav_iv = 11;
    public const int const_TipoIntegracao_12_FlagWSWhatsapp_DisInadimplencia = 12;
    public const int const_TipoIntegracao_13_FlagWSWhatsapp_DisLogDevolucao = 13;
    public const int const_TipoIntegracao_14_FlagWSWhatsapp_DisLogDespersaoRota = 14;
    public const int const_TipoIntegracao_15_SofiaChat = 15;

    public const string const_TratamentoSofia_SEMACESSO = "SEM ACESSO";
    public const string const_TratamentoSofia_OFFLINE = "OFFLINE";
    public const string const_TratamentoSofia_AJUDAERRO = "AJUDAERRO";
    public const string const_TratamentoSofia_ERRO998 = "ERRO998";
    public const string const_TratamentoSofia_OLA = "OLA";
    public const string const_TratamentoSofia_BEMVINDO = "BEMVINDO";
    public const string const_TratamentoSofia_BEM_VINDO_DASH = "BEM_VINDO_DASH";
    public const string const_TratamentoSofia_NOTIFICACAOCONFIRMADA = "NOTIFICACAOCONFIRMADA";
    public const string const_TratamentoSofia_CONFIRMARNOTIFICACAO = "CONFIRMARNOTIFICACAO";

    public const string const_TipoBancoDados_Bot = "SQLSRV";

    public const string const_Provider_Waboxapp = "WA";
    public const string const_Provider_Telegram = "TG";
    public const string const_Provider_ChartAPI = "CA";
    public const string const_Provider_Zap = "ZP";

    public const string const_Mensagem_Log_Mensagem = "Mensagem";
    public const string const_Mensagem_Log_Mensagem_ChatAPI = "ChatAPI";
    public const string const_Mensagem_Log_Mensagem_Waboxapp = "Waboxapp";
    public const string const_Mensagem_Log_Mensagem_Webhook = "Webhook";
    public const string const_Mensagem_Log_BancoDados = "BancoDados";
    public const string const_Mensagem_Log_BancoDados_Tems = "BancoDados_Tems";
    public const string const_Mensagem_Log_ProviderFlexXTools = "Provider_FlexXTools";

    public const string const_TipoLeituraDados_DBQuery = "DBQuery";
    public const string const_TipoLeituraDados_DBTabela = "DBTabela";
    public const string const_TipoLeituraDados_DBProc = "DBProc";
    public const string const_TipoLeituraDados_WSJson = "WSJson";

    public const string const_Ativar_Usuario = "1P0";
    public const string const_Ativar_Empresa = "2E3";
  }

  public static class Config_App
  {
    public static int idIntegrador = 0;
    public static string sDB_Tipo = "";
    public static string sDB_StringConexao = "";
    public static string sDB_TabelaView = "";
    public static string sDB_BancoDados = "";
    public static string sNomeSistema = "";
    public static string sProcessador = "";
    public static string sWebHook_Url = "";
    public static string ProductVersion;

    public static string sAplicativo;
    public static string sPartner;
    public static string sCdServico;

    public static string Senha(string sTipo, string sSenhaAdministrador, string sSenhaUsuario)
    {
      string sSenha = "";

      switch (sTipo)
      {
        case Constantes.const_Senha_Administrador:
          {
            sSenha = sSenhaAdministrador;
            break;
          }
        case Constantes.const_Senha_Usuario:
          {
            sSenha = sSenhaUsuario;
            break;
          }
      }

      return sSenha;
    }
  }

  public static class Config
  {
    public static string dbuser = "";
    public static string dbpassword = "";
    public static string host = "";
    public static string dbport = "";
    public static string dbname = "";
    public static string table = "";
    public static string sConexaoDestino_waboxapp_chat = "";
    public static string dbconstring = "";

    public static string FlexXTools_Usuario = "flagwhats4280en";
    public static string FlexXTools_Senha = "Odfofot59flag2344959";

    public static System.Diagnostics.EventLog appLog;

    public static string Provider = "";

    public static void appLog_Escrever(string sTexto)
    {
      if (appLog != null)
        appLog.WriteEntry(sTexto, System.Diagnostics.EventLogEntryType.Information);
    }

    public static void Carregar(JObject Json)
    {
      dbuser = Json["Config"]["dbuser"].ToString();
      dbpassword = Json["Config"]["dbpassword"].ToString();
      host = Json["Config"]["host"].ToString();
      dbport = Json["Config"]["dbport"].ToString();
      dbname = Json["Config"]["dbname"].ToString();
      table = Json["Config"]["table"].ToString();
      sConexaoDestino_waboxapp_chat = Json["Config"]["waboxapp_chat"].ToString();

      dbconstring = string.Format("Server={0};Database={1};Uid={2};Pwd={3};SslMode=none;", host, dbname, dbuser, dbpassword);

      FlexXTools_Usuario = Json["flexxtools"]["Usuario"].ToString();
      FlexXTools_Senha = Json["flexxtools"]["Senha"].ToString();
    }
  }

  public static class Mensagem
  {
    public static string events { get; set; }
    public static string token { get; set; }
    public static string contactuid { get; set; }
    public static string contactname { get; set; }
    public static string contacttype { get; set; }
    public static string messagedtm { get; set; }
    public static string messagercv { get; set; }
    public static string messagemtd { get; set; }
    public static string messagerqt { get; set; }
    public static string messagerst { get; set; }
    public static string messageuid { get; set; }
    public static string messagecuid { get; set; }
    public static string messagedir { get; set; }
    public static string messagetype { get; set; }
    public static string messagebody { get; set; }
    public static string messageack { get; set; }
    public static int idMessageTerms { get; set; }

    public static string Agente { get; set; }
    public static string Token { get; set; }
    public static string Uid { get; set; }
    public static string To { get; set; }

    public static long idMensagem { get; set; }
    public static Int32 idStatusMensagem { get; set; }
    public static Int32 idBot { get; set; }
    public static string botname { get; set; }

    public static long idTarefa { get; set; }
  }

  public class Mensagem_log
  {
    public class Item
    {
      public string messageuid { get; set; }
      public int message_log_ordem { get; set; }
      public DateTime message_log_evento { get; set; }
      public string message_log_processo { get; set; }
      public string message_log_comentario { get; set; }
    }

    public Item[] Itens;
    public int Itens_Qtde = 0;

    public void Adicionar(string _message_log_processo, string _message_log_comentario = "")
    {
      Itens_Qtde++;

      Array.Resize(ref Itens, Itens_Qtde);

      Itens[Itens_Qtde - 1] = new Item()
      {
        messageuid = Mensagem.messageuid,
        message_log_ordem = Itens_Qtde,
        message_log_evento = DateTime.Now,
        message_log_processo = _message_log_processo,
        message_log_comentario = _message_log_comentario
      };
    }
  }

  public class Entidades
  {
    public class DadosDISTSfVenda
    {
      public string COD_PUXADA { get; set; }
      public int VALOR_HECTO { get; set; }
      public int VALOR_VENDA { get; set; }
    }

    public class DadosDISTSfVendaRoot
    {
      public List<DadosDISTSfVenda> DadosDISTSfVenda { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class DadosDISTSfCoberturaRoot
    {
      public List<DadosDISTSfCobertura> DadosDISTSfCobertura { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class DadosDISTSfCobertura
    {
      public int COD_GRUPO { get; set; }
      public string COD_PUXADA { get; set; }
      public string DSC_GRUPO { get; set; }
      public int QTDE_COBERTURA { get; set; }
    }

    public class DadosDISTSfVendasGrupoRoot
    {
      public List<DadosDISTSfVendasGrupo> DadosDISTSfVendas_Grupo { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class DadosDISTSfVendasGrupo
    {
      public int COD_GRUPO { get; set; }
      public string COD_PUXADA { get; set; }
      public string DSC_GRUPO { get; set; }
      public int QTDE_VOLUME { get; set; }
      public int VALOR_HECTO { get; set; }
      public int VALOR_VENDA { get; set; }
    }

    public class DadosDISTSfIVIAVRoot
    {
      public List<DadosDISTSfIVIAV> DadosDISTSfIV_IAV { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class DadosDISTSfIVIAV
    {
      public string COD_PUXADA { get; set; }
      public int PERCENTUAL_IAV { get; set; }
      public int PERCENTUAL_IV { get; set; }
      public int QTDE_IAV { get; set; }
      public int QTDE_IV { get; set; }
      public int QTDE_VISITA_PREVISTA { get; set; }
    }

    public class DadosDISTSfDevolucaoRoot
    {
      public List<DadosDISTSfDevolucao> DadosDISTSfDevolucao { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class DadosDISTSfDevolucao
    {
      public string COD_PUXADA { get; set; }
      public int PERCENTUAL_DEVOLUCAO { get; set; }
      public int VALOR_DEVOLUCAO { get; set; }
    }

    public class WaboxApp_WhatsApp
    {
      public bool success { get; set; }
      public string custom_uid { get; set; }
    }
  }

  public static class Declaracao
  {
    public static Mensagem_log oMensagem_log;
    public static cls_tbmessageterms otbmessageterms = new cls_tbmessageterms();
    public static DataTable[] oTabela;

    public static string processador = "";
  }

  public class FlagWSWhatsapp_DadosUsuario
  {
    public string ATIVO { get; set; }
    public string CNPJ { get; set; }
    public string DEPARTAMENTO { get; set; }
    public string EMAIL { get; set; }
    public string EMPREGADO { get; set; }
    public string EMPRESA { get; set; }
    public string FLEXXPOWER { get; set; }
    public int ID_USUARIO { get; set; }
    public string LICENCA { get; set; }
    public string PUXADA { get; set; }
    public string SENHA { get; set; }
    public string SOFIA { get; set; }
    public string TELEFONE { get; set; }
    public string USUARIO { get; set; }
  }

  public class FlagWSWhatsapp_DadosUsuario_Root
  {
    public List<FlagWSWhatsapp_DadosUsuario> DadosUsuario { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DisDevolucao_Root
  {
    public List<FlagWSWhatsapp_DisDevolucao_Dados> DadosDISTDevolucao { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DisDevolucao_Dados
  {
    public int CODIGO_GERENTE { get; set; }
    public int CODIGO_SUPERVISOR { get; set; }
    public int CODIGO_VENDEDOR { get; set; }
    public string NOME_GERENTE { get; set; }
    public string NOME_SUPERVISOR { get; set; }
    public string NOME_VENDEDOR { get; set; }
    public double PERCENTUAL_DEVOLUCAO { get; set; }
    public double VALOR_BRUTO_DEVOLUCAO { get; set; }
    public double VALOR_BRUTO_VENDA { get; set; }
  }

  public class FlagWSWhatsapp_DISTSfDocumentosaVencer
  {
    public int CODIGO_CLASSE { get; set; }
    public int CODIGO_CLIENTE { get; set; }
    public string COD_PUXADA { get; set; }
    public string DATA_VENCIMENTO { get; set; }
    public int DDD_CELULAR { get; set; }
    public string DESCRICAO_CLASSE { get; set; }
    public int DIA_A_VENCER { get; set; }
    public string DOCUMENTO { get; set; }
    public string DOCUMENTO_ORIGEM { get; set; }
    public string E_MAIL { get; set; }
    public string LINHA_DIGITAVEL { get; set; }
    public string NOME_FANTASIA { get; set; }
    public int NUMERO_CELULAR { get; set; }
    public string RAZAO_SOCIAL { get; set; }
    public double VALOR_DOCUMENTO { get; set; }
  }

  public class FlagWSWhatsapp_DISTSfDocumentosaVencerRoot
  {
    public List<FlagWSWhatsapp_DISTSfDocumentosaVencer> DadosDISTSfDocumentosaVencer { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DISTSfDocumentosVencido
  {
    public int CODIGO_CLASSE { get; set; }
    public int CODIGO_CLIENTE { get; set; }
    public string COD_PUXADA { get; set; }
    public string DATA_VENCIMENTO { get; set; }
    public int DDD_CELULAR { get; set; }
    public string DESCRICAO_CLASSE { get; set; }
    public int DIA_VENCIDOS { get; set; }
    public string DOCUMENTO { get; set; }
    public string DOCUMENTO_ORIGEM { get; set; }
    public string E_MAIL { get; set; }
    public string LINHA_DIGITAVEL { get; set; }
    public string NOME_FANTASIA { get; set; }
    public int NUMERO_CELULAR { get; set; }
    public string RAZAO_SOCIAL { get; set; }
    public double VALOR_DOCUMENTO { get; set; }
  }

  public class FlagWSWhatsapp_DISTSfDocumentosVencidoRoot
  {
    public List<FlagWSWhatsapp_DISTSfDocumentosVencido> DadosDISTSfDocumentosVencido { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTTroca_Root
  {
    public List<FlagWSWhatsapp_DadosDISTTroca> DadosDISTTroca { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTTroca
  {
    public int CODIGO_GERENTE { get; set; }
    public int CODIGO_SUPERVISOR { get; set; }
    public int CODIGO_VENDEDOR { get; set; }
    public string NOME_GERENTE { get; set; }
    public string NOME_SUPERVISOR { get; set; }
    public string NOME_VENDEDOR { get; set; }
    public double PERCENTUAL_TROCA { get; set; }
    public double VALOR_BRUTO_TROCA { get; set; }
    public double VALOR_BRUTO_VENDA { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTIavIv_Root
  {
    public List<FlagWSWhatsapp_DadosDISTIavIv> DadosDISTIav_iv { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTIavIv
  {
    public int CODIGO_GERENTE { get; set; }
    public int CODIGO_SUPERVISOR { get; set; }
    public int CODIGO_VENDEDOR { get; set; }
    public string NOME_GERENTE { get; set; }
    public string NOME_SUPERVISOR { get; set; }
    public string NOME_VENDEDOR { get; set; }
    public double PERCENTUAL_VISITA_REALIZADA { get; set; }
    public double PERCENTUAL_VISITA_REALIZADA_VENDA { get; set; }
    public int QTDE_VISITA_PREVISTA { get; set; }
    public int QTDE_VISITA_REALIZADA { get; set; }
    public int QTDE_VISITA_REALIZADA_VENDA { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTInadimplencia_Root
  {
    public List<FlagWSWhatsapp_DadosDISTInadimplencia> DadosDISTInadimplencia { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTInadimplencia
  {
    public int CODIGO_GERENTE { get; set; }
    public int CODIGO_SUPERVISOR { get; set; }
    public int CODIGO_VENDEDOR { get; set; }
    public string NOME_GERENTE { get; set; }
    public string NOME_SUPERVISOR { get; set; }
    public string NOME_VENDEDOR { get; set; }
    public double PERCENTUAL_INADIMPLENCIA { get; set; }
    public double VALOR_BRUTO_VENDA { get; set; }
    public double VALOR_INADIMPLENCIA { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTLogDevolucao_Root
  {
    public List<FlagWSWhatsapp_DadosDISTLogDevolucao> DadosDISTLogDevolucao { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTLogDevolucao
  {
    public int CODIGO_GERENTE { get; set; }
    public int CODIGO_SUPERVISOR { get; set; }
    public int CODIGO_VENDEDOR { get; set; }
    public string NOME_GERENTE { get; set; }
    public string NOME_SUPERVISOR { get; set; }
    public string NOME_VENDEDOR { get; set; }
    public int PERCENTUAL_DEVOLUCAO { get; set; }
    public int VALOR_BRUTO_DEVOLUCAO { get; set; }
    public int VALOR_BRUTO_VENDA { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao_Root
  {
    public List<FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao> DadosDISTLog_Taxa_Ocupacao { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao
  {
    public int CODIGO_VEICULO { get; set; }
    public string NUM_PLACA { get; set; }
    public double PERCENTUAL_DEVOLUCAO { get; set; }
    public int QTDE_CAPACIDADE { get; set; }
    public int QTDE_VIAGEM { get; set; }
    public double QTD_CARGA { get; set; }
    public double VALOR_BRUTO_DEVOLUCAO { get; set; }
    public double VALOR_BRUTO_VENDA { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista_Root
  {
    public List<FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista> DadosDISTLogDevolucaoMotorista { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista
  {
    public int CODIGO_MOTORISTA { get; set; }
    public string NOME_MOTORISTA { get; set; }
    public double PERCENTUAL_DEVOLUCAO { get; set; }
    public double VALOR_BRUTO_DEVOLUCAO { get; set; }
    public double VALOR_BRUTO_VENDA { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTLogDevolucaoCarro_Root
  {
    public List<FlagWSWhatsapp_DadosDISTLogDevolucaoCarro> DadosDISTLogDevolucaoCarro { get; set; }
    public int codigo { get; set; }
    public string mensagem { get; set; }
  }

  public class FlagWSWhatsapp_DadosDISTLogDevolucaoCarro
  {
    public int CODIGO_VEICULO { get; set; }
    public string NUM_PLACA { get; set; }
    public Double PERCENTUAL_DEVOLUCAO { get; set; }
    public Double VALOR_BRUTO_DEVOLUCAO { get; set; }
    public Double VALOR_BRUTO_VENDA { get; set; }
  }

  public class JsonHelper
  {
    private const int INDENT_SIZE = 4;

    public static string FormatJson(string str)
    {
      str = (str ?? "").Replace("{}", @"\{\}").Replace("[]", @"\[\]");

      var inserts = new List<int[]>();
      bool quoted = false, escape = false;
      int depth = 0/*-1*/;

      for (int i = 0, N = str.Length; i < N; i++)
      {
        var chr = str[i];

        if (!escape && !quoted)
          switch (chr)
          {
            case '{':
            case '[':
              inserts.Add(new[] { i, +1, 0, INDENT_SIZE * ++depth });
              //int n = (i == 0 || "{[,".Contains(str[i - 1])) ? 0 : -1;
              //inserts.Add(new[] { i, n, INDENT_SIZE * ++depth * -n, INDENT_SIZE - 1 });
              break;
            case ',':
              inserts.Add(new[] { i, +1, 0, INDENT_SIZE * depth });
              //inserts.Add(new[] { i, -1, INDENT_SIZE * depth, INDENT_SIZE - 1 });
              break;
            case '}':
            case ']':
              inserts.Add(new[] { i, -1, INDENT_SIZE * --depth, 0 });
              //inserts.Add(new[] { i, -1, INDENT_SIZE * depth--, 0 });
              break;
            case ':':
              inserts.Add(new[] { i, 0, 1, 1 });
              break;
          }

        quoted = (chr == '"') ? !quoted : quoted;
        escape = (chr == '\\') ? !escape : false;
      }

      if (inserts.Count > 0)
      {
        var sb = new System.Text.StringBuilder(str.Length * 2);

        int lastIndex = 0;
        foreach (var insert in inserts)
        {
          int index = insert[0], before = insert[2], after = insert[3];
          bool nlBefore = (insert[1] == -1), nlAfter = (insert[1] == +1);

          sb.Append(str.Substring(lastIndex, index - lastIndex));

          if (nlBefore) sb.AppendLine();
          if (before > 0) sb.Append(new String(' ', before));

          sb.Append(str[index]);

          if (nlAfter) sb.AppendLine();
          if (after > 0) sb.Append(new String(' ', after));

          lastIndex = index + 1;
        }

        str = sb.ToString();
      }

      return str.Replace(@"\{\}", "{}").Replace(@"\[\]", "[]");
    }
  }

  public class cls_tbmessageterms
  {
    public class cls_messageterms
    {
      public int idMessageTerms { get; set; }
      public string Cod_Puxada { get; set; }
      public string Agente { get; set; }
      public string Command { get; set; }
      public string Servico { get; set; }
      public string TipoComando { get; set; }
      public string dsTerms { get; set; }
      public string dsTemplateHeader { get; set; }
      public string dsTemplate { get; set; }
      public string dsTemplateFooter { get; set; }
      public string WebService { get; set; }
      public string WS_TipoRegistro { get; set; }
      public string WS_TipoConsulta { get; set; }
      public string WS_JsonParametros { get; set; }

      public int WS_IntervaloConsulta { get; set; }
      public int WS_VezesAoDia { get; set; }
      public int WS_TempoEspera { get; set; }
      public string WS_TipoRetorno { get; set; }
      public string WS_TipoRetornoCampo { get; set; }
      public string WS_TipoRetornoRespostas { get; set; }
      public string WS_TipoRetornoScript { get; set; }
      public string WS_TipoRetornoTipo { get; set; }
      public int WS_ComprimentoMin { get; set; }
      public int WS_CampoObrigatorio { get; set; }
      public string WS_MetodoValidarUsuario { get; set; }

      public Boolean SepararMensagens { get; set; }
      public int WS_ProximaMensagem { get; set; }
    }

    public cls_messageterms[] messageterms;

    public cls_messageterms IdentificarTermo(string sExpressao)
    {
      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados_Tems, "Identificando");

      cls_messageterms Ret = null;

      Ret = PesquisarPorTermo(sExpressao);

      if (Ret == null)
        Ret = PesquisarPorTermo(Constantes.const_TratamentoSofia_AJUDAERRO, true);

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados_Tems, "Identificado");

      return Ret;
    }

    public cls_messageterms PesquisarPorId(int idMessageTerms)
    {
      cls_messageterms Ret = null;

      foreach (cls_messageterms Item in messageterms)
      {
        if (Item.idMessageTerms == idMessageTerms)
        {
          Ret = Item;
          break;
        }
      }

      return Ret;
    }

    public cls_messageterms PesquisarPorTermo(string sExpressao, Boolean PorComnado = false, string sCod_Puxada = "", string sServico = "")
    {
      cls_messageterms Ret = null;

      foreach (cls_messageterms Item in messageterms)
      {
        if (Item.Agente.Trim().ToUpper() == Mensagem.Agente.Trim().ToUpper() &&
           (Item.Cod_Puxada.Trim().ToUpper() == sCod_Puxada.Trim().ToUpper() || sCod_Puxada == "") &&
           (Item.Servico.Trim().ToUpper() == sServico.Trim().ToUpper() || sServico == ""))
        {
          if (PorComnado)
          {
            if (Item.Command == sExpressao)
            {
              Ret = Item;
              break;
            }
          }
          else
          {
            string[] Termos = Item.dsTerms.Split(new char[] { ';' });

            foreach (string Termo in Termos)
            {
              if (Termo.ToUpper().IndexOf(sExpressao.ToUpper().Trim()) > -1)
              {
                Ret = Item;
                break;
              }
            }
          }

          if (Ret != null)
            break;
        }

      }

      return Ret;
    }
  }

  public static class _Funcoes
  {
    public static DbType SystemType_para_DbType(System.Type oTipo)
    {
      DbType oRet = DbType.String;

      switch (oTipo.Name.ToUpper().Trim())
      {
        case "STRING":
          {
            oRet = DbType.String;
            break;
          }
        case "BOOLEAN":
          {
            oRet = DbType.Boolean;
            break;
          }
        case "INT32":
          {
            oRet = DbType.Int32;
            break;
          }
        case "DOUBLE":
          {
            oRet = DbType.Double;
            break;
          }
        case "CHAR":
          {
            oRet = DbType.String;
            break;
          }
        case "DECIMAL":
          {
            oRet = DbType.Decimal;
            break;
          }
        case "DATE":
        case "DATETIME":
          {
            oRet = DbType.DateTime;
            break;
          }
        default:
          {
            oRet = DbType.String;
            break;
          }
      }

      return oRet;
    }

    public static string Replace(string sTexto, string sCampo, object Valor)
    {
      try
      {
        int iIndice = sTexto.IndexOf(sCampo.Trim() + "]");
        string sValor = "";
        decimal dValor = 0;

        if (iIndice > -1)
        {
          for (int i = iIndice - 1; i >= 0; i--)
          {
            if (sTexto.Substring(i, 1) == "[")
            {
              sCampo = sTexto.Substring(i, iIndice + sCampo.Length - i + 1);

              string[] sSeparador = sCampo.Substring(1, sCampo.Length - 2).Replace("||", "|").Split(new char[] { '|' });

              //Divisão
              if (sSeparador[1].Trim() != "-")
              {
                dValor = (Convert.ToDecimal(Valor) / Convert.ToInt32(sSeparador[1].Substring(1)));
              }
              else
              {
                if (_Funcoes.FNC_IsNumeric(Valor.ToString()))
                {
                  dValor = Convert.ToDecimal(Valor);
                }
              }
              //Formatação
              if (sSeparador[0].Trim() != "-")
              {
                if (_Funcoes.FNC_IsNumeric(Valor.ToString()))
                {
                  sValor = dValor.ToString(sSeparador[0].ToString());
                }
                else if (FNC_Data_Valida(Valor.ToString()))
                {
                  sValor = Convert.ToDateTime(Valor).ToString(sSeparador[0]);
                }
                else
                {
                  sValor = Valor.ToString();
                }
              }
              else
              {
                if (_Funcoes.FNC_IsNumeric(Valor.ToString()))
                {
                  sValor = dValor.ToString();
                }
                else
                {
                  sValor = Valor.ToString();
                }
              }

              sTexto = sTexto.Replace(sCampo, sValor);

              break;
            }
          }
        }
      }
      catch (Exception)
      {
      }

      return sTexto;
    }

    public static bool FNC_Data_Vazio(DataTable oData)
    {
      bool bOk = false;

      if (oData != null)
      {
        bOk = (oData.Rows.Count == 0);
      }

      return bOk;
    }

    public static string FNC_DataTable_ColunaParAdicionar(DataTable oData, string NomeColunaOrigem, string NomeColunaDestino)
    {
      string sRet = "";

      foreach (DataColumn oColuna in oData.Columns)
      {
        if (NomeColunaOrigem == oColuna.ColumnName)
        {
          if (NomeColunaDestino.Trim() == "")
            NomeColunaDestino = NomeColunaOrigem;

          sRet = NomeColunaDestino;

          if (oColuna.DataType.ToString() == "Boolean")
            sRet = sRet + " bit";
          else if (oColuna.DataType.ToString() == "Byte")
            sRet = sRet + " tinyint";
          else if (oColuna.DataType.ToString() == "String")
            sRet = sRet + " varchar(" + oColuna.MaxLength.ToString() + ")";
          else if (oColuna.DataType.ToString() == "Char")
            sRet = sRet + " Char(" + oColuna.MaxLength.ToString() + ")";
          else if (oColuna.DataType.ToString() == "DateTime")
            sRet = sRet + " DateTime";
          else if (oColuna.DataType.ToString() == "Decimal")
            sRet = sRet + " Decimal(" + oColuna.MaxLength.ToString() + ")";
          else if (oColuna.DataType.ToString() == "Double")
            sRet = sRet + " Double";
          else if (oColuna.DataType.ToString() == "Int32" || oColuna.DataType.ToString() == "Single")
            sRet = sRet + " int";
          else if (oColuna.DataType.ToString() == "Int64")
            sRet = sRet + " bigint";
          else if (oColuna.DataType.ToString() == "SByte" || oColuna.DataType.ToString() == "Int16")
            sRet = sRet + " smallint";
          else if (oColuna.DataType.ToString() == "UInt16" || oColuna.DataType.ToString() == "UInt32" || oColuna.DataType.ToString() == "UInt64")
            sRet = sRet + " bigint";
        }
      }

      return sRet;
    }

    public static string FNC_FormatarTelefone(string sTelefone)
    {
      sTelefone = sTelefone.Trim();

      if (sTelefone.Length == 12)
      {
        sTelefone = sTelefone.Substring(0, 4) + "9" + sTelefone.Substring(4);
      }

      return sTelefone;
    }

    public static string FNC_Diretorio_Tratar(string sPath)
    {
      if (FNC_Right(sPath.Trim(), 1) != @"\")
        return sPath.Trim() + @"\";
      else
        return sPath.Trim();
    }

    public static string FNC_Right(string sTexto, int Tamanho)
    {
      string sRet;

      sRet = sTexto.Trim();

      if (sRet.Length > Tamanho)
      {
        sRet = sRet.Substring(sRet.Length - Tamanho);
      }

      return sRet;
    }

    public static bool FNC_Enviar(string sPara,
                                  string sCC,
                                  string sAssunto,
                                  string sMensagem,
                                  string[] sAnexo = null)
    {
      bool bOk = false;
      string[] Para = null;

      if (sPara.Contains(";"))
      {
        Para = sPara.Split(new char[] { ';' });
      }
      else
      {
        Array.Resize(ref Para, 1);
        Para[Para.Length - 1] = sPara;
      }

      try
      {
        using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
        {
          smtp.Host = Integrador_Funcoes.Propriedade_Ler(Integrador_Funcoes.const_Propriedade_EMail_Host).ToString();
          smtp.Port = Convert.ToInt32(Integrador_Funcoes.Propriedade_Ler(Integrador_Funcoes.const_Propriedade_EMail_Port));
          smtp.EnableSsl = (Integrador_Funcoes.Propriedade_Ler(Integrador_Funcoes.const_Propriedade_EMail_EnableSsl).ToString().ToUpper() == "S");
          smtp.UseDefaultCredentials = (Integrador_Funcoes.Propriedade_Ler(Integrador_Funcoes.const_Propriedade_EMail_UseDefaultCredentials).ToString().ToUpper() == "S");

          if (FNC_NuloString(Integrador_Funcoes.Propriedade_Ler(Integrador_Funcoes.const_Propriedade_EMail_UserName)).Trim() != "")
            smtp.Credentials = new System.Net.NetworkCredential(Integrador_Funcoes.Propriedade_Ler(Integrador_Funcoes.const_Propriedade_EMail_UserName).ToString(),
                                                                Integrador_Funcoes.Propriedade_Ler(Integrador_Funcoes.const_Propriedade_EMail_Password).ToString());

          using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
          {
            mail.From = new System.Net.Mail.MailAddress(Integrador_Funcoes.Propriedade_Ler(Integrador_Funcoes.const_Propriedade_EMail_UserName).ToString());

            if (!string.IsNullOrWhiteSpace(sPara))
            {
              foreach (string sTo in Para)
              {
                mail.To.Add(new System.Net.Mail.MailAddress(sTo));
              }
            }
            else
            {
              goto Sair;
            }
            if (!string.IsNullOrWhiteSpace(sCC))
              mail.CC.Add(new System.Net.Mail.MailAddress(sCC));

            mail.Subject = sAssunto;
            mail.Body = sMensagem;

            if (sAnexo != null)
            {
              foreach (string file in sAnexo)
              {
                mail.Attachments.Add(new System.Net.Mail.Attachment(file));
              }
            }

            smtp.Send(mail);
          }
        }

        bOk = true;
      }
      catch (Exception Ex)
      {
      }

      Sair:
      return bOk;
    }

    public static bool FNC_IsHour(string sHora)
    {
      try
      {
        if (sHora.Length == 5)
        {
          if (sHora.Trim().Substring(2, 1) == ":")
          {
            if ((Convert.ToInt32(sHora.Trim().Substring(0, 2)) >= 0) && (Convert.ToInt32(sHora.Trim().Substring(0, 2)) <= 23))
            {
              if ((Convert.ToInt32(sHora.Trim().Substring(3, 2)) >= 0) && (Convert.ToInt32(sHora.Trim().Substring(3, 2)) <= 59))
              {
                return true;
              }
              else
                return false;
            }
            else
              return false;
          }
          else
            return false;
        }
        else
          return false;
      }
      catch
      {
        return false;
      }
    }

    public static string FNC_SoNumero(string sValor)
    {
      int iCont;
      string sAux = "";

      for (iCont = 1; (iCont <= sValor.Length); iCont++)
      {
        if (FNC_IsNumeric(sValor.Substring((iCont - 1), 1)))
        {
          sAux = (sAux + sValor.Substring((iCont - 1), 1));
        }
      }

      return sAux;
    }

    public static bool FNC_IsNumeric(this string s)
    {
      float output;
      return float.TryParse(s, out output);
    }

    public static double FNC_NuloZero(object Valor)
    {
      try
      {
        if (Valor == null)
          return 0;
        else
        {
          if (FNC_IsNumeric(Valor.ToString()))
          {
            Valor = Valor.ToString().Replace(",", "").Replace(".", ",");

            return Convert.ToDouble(Valor);
          }
          else
            return 0;
        }
      }
      catch (Exception)
      {
        return 0;
      }
    }

    public static string FNC_NuloString(object Valor)
    {
      try
      {
        if (Valor == null)
          return "";
        else
        {
          return Valor.ToString();
        }
      }
      catch (Exception)
      {
        return "";
      }
    }

    public static string[,] Parametro_Desmontar(string sParametro)
    {
      string[] sAux = null;

      if (sParametro.Trim() != "")
      {
        sAux = sParametro.Split(new char[] { ';' });
      }

      string[,] oParametro = new string[sAux.Length, 2];

      for (int i = 0; i < sAux.Length; i++)
      {
        oParametro[i, 0] = sAux[i].Split(new char[] { ':' })[0];
        oParametro[i, 1] = sAux[i].Split(new char[] { ':' })[1];
      }

      return oParametro;
    }

    public static string Parametro_Valor(string[,] oParametro, string sNome)
    {
      string sValor = "";

      try
      {
        if (oParametro != null)
        {
          for (int i = 0; i < oParametro.GetLength(0); i++)
          {
            if (oParametro[i, 0] == sNome)
            {
              sValor = oParametro[i, 1];
              break;
            }
          }
        }
      }
      catch (Exception)
      {
      }

      return sValor;
    }

    public static decimal FNC_NuloZeroDecimal(object Valor)
    {
      if (Valor == null)
        return 0;
      else
      {
        if (FNC_IsNumeric(Valor.ToString()))
        {
          return Convert.ToDecimal(Valor);
        }
        else
          return 0;
      }
    }

    public static string FNC_CPFCNPJ_Formatar(string sCPFCNPJ)
    {
      if (sCPFCNPJ.Length <= 11)
      {
        MaskedTextProvider mtpCpf = new MaskedTextProvider(@"000\.000\.000-00");
        mtpCpf.Set(FNC_ZerosEsquerda(sCPFCNPJ, 11));
        return mtpCpf.ToString();
      }
      else
      {
        MaskedTextProvider mtpCnpj = new MaskedTextProvider(@"00\.000\.000/0000-00");
        mtpCnpj.Set(FNC_ZerosEsquerda(sCPFCNPJ, 11));
        return mtpCnpj.ToString();
      }
    }

    public static Boolean FNC_CNPJ_Verifica(String cnpj)
    {
      if (Regex.IsMatch(cnpj, @"(^(\d{2}.\d{3}.\d{3}/\d{4}-\d{2})|(\d{14})$)"))
      {
        return FNC_CNPJ_Valida(cnpj);
      }
      else
      {
        return false;
      }
    }

    private static Boolean FNC_CNPJ_Valida(String cnpj)
    {
      Int32[] digitos, soma, resultado;
      Int32 nrDig;
      String ftmt;
      Boolean[] cnpjOk;

      cnpj = cnpj.Replace("/", "");
      cnpj = cnpj.Replace(".", "");
      cnpj = cnpj.Replace("-", "");

      if (cnpj == "00000000000000")
      {
        return false;
      }

      ftmt = "6543298765432";
      digitos = new Int32[14];
      soma = new Int32[2];
      soma[0] = 0;
      soma[1] = 0;
      resultado = new Int32[2];
      resultado[0] = 0;
      resultado[1] = 0;
      cnpjOk = new Boolean[2];
      cnpjOk[0] = false;
      cnpjOk[1] = false;

      try
      {
        for (nrDig = 0; nrDig < 14; nrDig++)
        {
          digitos[nrDig] = int.Parse(cnpj.Substring(nrDig, 1));

          if (nrDig <= 11)
            soma[0] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig + 1, 1)));

          if (nrDig <= 12)
            soma[1] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig, 1)));
        }

        for (nrDig = 0; nrDig < 2; nrDig++)
        {
          resultado[nrDig] = (soma[nrDig] % 11);

          if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
            cnpjOk[nrDig] = (digitos[12 + nrDig] == 0);
          else
            cnpjOk[nrDig] = (digitos[12 + nrDig] == (11 - resultado[nrDig]));
        }

        return (cnpjOk[0] && cnpjOk[1]);
      }
      catch
      {
        return false;
      }
    }

    public static string FNC_ZerosEsquerda(string strString, int intTamanho)
    {
      string strResult = "";

      strString = strString.Trim();

      for (int intCont = 1; intCont <= (intTamanho - strString.Length); intCont++)
      {
        strResult += "0";
      }
      return strResult + strString;
    }

    public static string FNC_ListaParaListaComAspa(string sLista, string sSeparadorAtual, string sNovoSeparador)
    {
      string sRet = "";
      string[] Lista = sLista.Split(Convert.ToChar(sSeparadorAtual));

      foreach (string Ret in Lista)
      {
        FNC_Str_Adicionar(ref sRet, "'" + Ret.Trim() + "'", sNovoSeparador);
      }

      return sRet;
    }

    public static void FNC_Str_Adicionar(ref string vVariavel, string sValor, string sSeparador)
    {
      if ((sValor.Trim() != ""))
      {
        if ((vVariavel.Trim() != ""))
        {
          vVariavel = (vVariavel + sSeparador);
        }

        vVariavel = (vVariavel + sValor);
      }

    }

    public static bool FNC_Data_Valida(string data)
    {
      Regex r = new Regex(@"(\d{2}\/\d{2}\/\d{4} \d{2}:\d{2})");
      return r.Match(data).Success;
    }

    public static string FNC_Data_Atual_DB()
    {
      string Data = "";

      Data = FNC_Data_DB(DateTime.Now);

      return Data;
    }

    public static string FNC_GerarCodigo()
    {
      int Tamanho = 10; // Numero de digitos da senha
      string senha = string.Empty;
      for (int i = 0; i < Tamanho; i++)
      {
        Random random = new Random();
        int codigo = Convert.ToInt32(random.Next(48, 122).ToString());

        if ((codigo >= 48 && codigo <= 57) || (codigo >= 97 && codigo <= 122))
        {
          string _char = ((char)codigo).ToString();
          if (!senha.Contains(_char))
          {
            senha += _char;
          }
          else
          {
            i--;
          }
        }
        else
        {
          i--;
        }
      }
      return senha;
    }

    public static string FNC_Data_DB(DateTime Date)
    {
      string Data = "";

      Data = Date.Year.ToString() + "-" +
             FNC_ZerosEsquerda(Date.Month.ToString(), 2) + "-" +
             FNC_ZerosEsquerda(Date.Day.ToString(), 2) + " " +
             FNC_ZerosEsquerda(Date.Hour.ToString(), 2) + ":" +
             FNC_ZerosEsquerda(Date.Minute.ToString(), 2) + ":" +
             FNC_ZerosEsquerda(Date.Second.ToString(), 2);

      return Data;
    }
  }

  public static class Bot
  {
    public static void Terms_Carregar()
    {
      int idMessageTerms = 0;
      int iCont = -1;

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados_Tems, "Carregando");

      DataTable oData_01;

      oData_01 = Integrador_Funcoes.oBancoDados.DBQuery("SELECT * FROM vwmessagetermswords where idMessageTerms is not null order by idMessageTerms");

      for (int intI = 0; intI < oData_01.Rows.Count; intI++)
      {
        if (idMessageTerms != Convert.ToInt32(oData_01.Rows[intI]["idMessageTerms"]))
        {
          iCont++;
          Array.Resize(ref Declaracao.otbmessageterms.messageterms, iCont + 1);
          Declaracao.otbmessageterms.messageterms[iCont] = new cls_tbmessageterms.cls_messageterms();
          Declaracao.otbmessageterms.messageterms[iCont].idMessageTerms = Convert.ToInt32(oData_01.Rows[intI]["idMessageTerms"]);
          Declaracao.otbmessageterms.messageterms[iCont].Cod_Puxada = oData_01.Rows[intI]["Cod_Puxada"].ToString();
          Declaracao.otbmessageterms.messageterms[iCont].Agente = oData_01.Rows[intI]["Agente"].ToString();
          Declaracao.otbmessageterms.messageterms[iCont].Servico = oData_01.Rows[intI]["Servico"].ToString();
          Declaracao.otbmessageterms.messageterms[iCont].Command = oData_01.Rows[intI]["Command"].ToString();
          Declaracao.otbmessageterms.messageterms[iCont].TipoComando = oData_01.Rows[intI]["TipoComando"].ToString();
          Declaracao.otbmessageterms.messageterms[iCont].dsTerms = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["dsTerms"]).Trim();
          Declaracao.otbmessageterms.messageterms[iCont].dsTemplateHeader = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["dsTemplateHeader"]);
          Declaracao.otbmessageterms.messageterms[iCont].dsTemplate = oData_01.Rows[intI]["dsTemplate"].ToString();
          Declaracao.otbmessageterms.messageterms[iCont].dsTemplateFooter = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["dsTemplateFooter"]);
          Declaracao.otbmessageterms.messageterms[iCont].WebService = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WebService"]);
          Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRegistro = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRegistro"]);
          Declaracao.otbmessageterms.messageterms[iCont].WS_TipoConsulta = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoConsulta"]);
          Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_JsonParametros"]);
          Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros = Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros.Replace("WS_TipoConsulta]", Declaracao.otbmessageterms.messageterms[iCont].WS_TipoConsulta);
          Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros = Declaracao.otbmessageterms.messageterms[iCont].WS_JsonParametros.Replace("[WS_TipoRegistro]", Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRegistro);
          Declaracao.otbmessageterms.messageterms[iCont].SepararMensagens = (_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["SepararMensagens"]) == 1);
          Declaracao.otbmessageterms.messageterms[iCont].WS_ProximaMensagem = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_ProximaMensagem"]));
          Declaracao.otbmessageterms.messageterms[iCont].WS_IntervaloConsulta = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_IntervaloConsulta"]));
          Declaracao.otbmessageterms.messageterms[iCont].WS_VezesAoDia = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_VezesAoDia"]));
          Declaracao.otbmessageterms.messageterms[iCont].WS_TempoEspera = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_TempoEspera"]));
          Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetorno = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetorno"]).Trim();
          Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetornoCampo = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetornoCampo"]).Trim();
          Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetornoRespostas = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetornoRespostas"]).Trim();
          Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetornoScript = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetornoScript"]).Trim();
          Declaracao.otbmessageterms.messageterms[iCont].WS_TipoRetornoTipo = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_TipoRetornoTipo"]).Trim();
          Declaracao.otbmessageterms.messageterms[iCont].WS_ComprimentoMin = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_ComprimentoMin"]));
          Declaracao.otbmessageterms.messageterms[iCont].WS_CampoObrigatorio = Convert.ToInt32(_Funcoes.FNC_NuloZero(oData_01.Rows[intI]["WS_CampoObrigatorio"]));
          Declaracao.otbmessageterms.messageterms[iCont].WS_MetodoValidarUsuario = _Funcoes.FNC_NuloString(oData_01.Rows[intI]["WS_MetodoValidarUsuario"]).Trim();
          idMessageTerms = Convert.ToInt32(oData_01.Rows[intI]["idMessageTerms"]);
        }

        if (Declaracao.otbmessageterms.messageterms[iCont].dsTerms.Substring(Declaracao.otbmessageterms.messageterms[iCont].dsTerms.Length - 1, 1) == ";")
          Declaracao.otbmessageterms.messageterms[iCont].dsTerms = Declaracao.otbmessageterms.messageterms[iCont].dsTerms.Substring(0, Declaracao.otbmessageterms.messageterms[iCont].dsTerms.Length - 1);

        if (oData_01.Rows[intI]["SearchTerm"] != null)
          Declaracao.otbmessageterms.messageterms[iCont].dsTerms = Declaracao.otbmessageterms.messageterms[iCont].dsTerms + ";" + oData_01.Rows[intI]["SearchTerm"].ToString();
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados_Tems, "Carregado");
    }

    public static void Terms_Descarregar()
    {
      Declaracao.otbmessageterms.messageterms = null;
    }

    public static void CarregarTabelas()
    {
      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados, "Carregando Tabelas");

      if (Declaracao.oTabela == null)
      {
        Declaracao.oTabela = new DataTable[2];
        if (Mensagem.idBot != 0)
        {
          Declaracao.oTabela[0] = Integrador_Funcoes.oBancoDados.DBQuery("select * from tbbot where idBot = " + Mensagem.idBot.ToString());
        }
        else
        {
          Declaracao.oTabela[0] = Integrador_Funcoes.oBancoDados.DBQuery("select * from tbbot where trim(upper(Apelido)) = '" + Mensagem.botname.Trim().ToUpper() + "'");
        }
        Declaracao.oTabela[1] = Integrador_Funcoes.oBancoDados.DBQuery("select * from tbmessage where idMessage = " + Mensagem.idMensagem.ToString());
      }
      else
      {
        if (Mensagem.idBot != 0)
        {
          if (Convert.ToUInt32(Tabelas_BuscarValor("tbbot", "idMessage", 0)) != Mensagem.idBot)
            Declaracao.oTabela[0] = Integrador_Funcoes.oBancoDados.DBQuery("select * from tbbot where idBot = " + Mensagem.idBot.ToString());
        }
        if (Mensagem.idMensagem != 0)
        {
          if (Convert.ToUInt32(Tabelas_BuscarValor("tbmessage", "idMessage", 0)) != Mensagem.idMensagem)
            Declaracao.oTabela[1] = Integrador_Funcoes.oBancoDados.DBQuery("select * from tbmessage where idMessage = " + Mensagem.idMensagem.ToString());
        }
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados, "Tabelas Carregadas");
    }

    public static object Tabelas_BuscarValor(string sTabela, string sCampo, object ValorPadrao = null)
    {
      object Ret = "";

      try
      {
        foreach (DataTable Tabela in Declaracao.oTabela)
        {
          if (Tabela.TableName.ToUpper() == sTabela.ToUpper())
          {
            Ret = Tabela.Rows[0][sCampo];
          }

          break;
        }
      }
      catch (Exception)
      {
        if (ValorPadrao != null)
          Ret = ValorPadrao;
      }

      return Ret;
    }

    public static string LerTag(string sAplicativo,
                                string sPartner,
                                string sCdServico,
                                string sTemplate, ref cls_tbmessageterms.cls_messageterms Terms)
    {
      string Ret = "";
      int iAux = 0;
      string sAux = "";

      Ret = sTemplate;

      //Procura TAGs acesso a tabelas
      try
      {
        while (true)
        {
          if (Ret.IndexOf("|CPO.") > -1)
          {
            iAux = Ret.Substring(1, Ret.IndexOf("|CPO.")).LastIndexOf("|TAB.");
            sAux = Ret.Substring(Ret.Substring(0, iAux).LastIndexOf("["), Ret.Substring(iAux).IndexOf("]") - Ret.Substring(1, iAux).LastIndexOf("[") + iAux);

            Ret = _Funcoes.Replace(Ret, sAux.Substring(sAux.IndexOf("TAB."), sAux.Length - sAux.IndexOf("TAB.") - 1),
                                   Bot.Tabelas_BuscarValor(sAux.Substring(sAux.IndexOf("TAB.") + ("TAB.").Length, sAux.IndexOf("|CPO.") - sAux.IndexOf("TAB.") - ("TAB.").Length),
                                                           sAux.Substring(sAux.IndexOf("CPO.") + ("CPO.").Length, sAux.Length - sAux.IndexOf("CPO.") - ("CPO.").Length - 1)));
          }
          else
            break;
        }
      }
      catch (Exception)
      {
      }

      //Procura TAGs de acesso a outros terms
      iAux = 0;

      while (Ret.IndexOf("[-||-|TERMS.", iAux) > -1)
      {
        sAux = Ret.Substring(Ret.IndexOf("[-||-|TERMS."));
        sAux = sAux.Substring(0, sAux.IndexOf("]") + 1);
        string[] sGrupo = sAux.Substring(1, sAux.Length - 2).Replace("||", "|").Split(new char[] { '|' });
        string[] sFiltro = sGrupo[2].Split(new char[] { '.' });

        iAux = Ret.IndexOf(sAux) + sAux.Length;

        Terms = Declaracao.otbmessageterms.PesquisarPorTermo(sGrupo[3], true, sFiltro[1], sFiltro[3]);

        if (Terms != null)
        {
          Ret = _Funcoes.Replace(Ret, sGrupo[3], Terms.dsTemplate);
          Terms.dsTemplate = Ret;
          Ret = Bot.Terms_Processar(sAplicativo, sPartner, sCdServico, Terms)[0];
          Terms = null;
          iAux = 0;
        }
      }

      return Ret;
    }

    public static string TratarTexto(string sTemplate, string sProtocolo = "")
    {
      string Ret = "";

      Ret = sTemplate;

      if (Mensagem.contactname.Trim() != "")
        Ret = _Funcoes.Replace(Ret, "NOME_CONTATO", Mensagem.contactname);
      if (sProtocolo.Trim() != "")
        Ret = _Funcoes.Replace(Ret, "NRO_PROTOCOLO", sProtocolo);

      Ret = _Funcoes.Replace(Ret, "DATA_HORA_HOJE", DateTime.Now.ToString());
      return Ret;
    }

    public static Boolean ValidarRetornoJSon(int codigo, string mensagem, ref string[] Texto)
    {
      Boolean Ret = false;

      if (codigo == -1 || codigo == 9 || codigo == 998 || codigo == 999)
      {
        Texto = new string[1];

        switch (codigo)
        {
          case -1:
            Texto[0] = Constantes.const_TratamentoSofia_OFFLINE;
            break;
          case 9:
            Texto[0] = Constantes.const_TratamentoSofia_SEMACESSO;
            break;
          case 998:
            Texto[0] = Constantes.const_TratamentoSofia_ERRO998;
            break;
        }
      }
      else
      {
        Ret = true;
      }

      return Ret;
    }

    public static string[] Terms_Processar(string sAplicativo,
                                           string sPartner,
                                           string sCdServico,
                                           cls_tbmessageterms.cls_messageterms Terms)
    {
      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados_Tems, "Processando");

      string[] sMensagem = null;
      cls_tbmessageterms.cls_messageterms NovaTerms = null;

      while (true)
      {
        switch (Terms.TipoComando)
        {
          case "T":
            {
              sMensagem = new string[1];
              sMensagem[0] = LerTag(sAplicativo, sPartner, sCdServico, Terms.dsTemplate, ref NovaTerms);
              break;
            }
          case "W":
            {
              switch (Terms.Command)
              {
                case "DadosDISTSfVenda":
                  {
                    sMensagem = FlexXTools.FlexXTools_DadosDISTSfVenda(sAplicativo,
                                                                       sPartner,
                                                                       sCdServico,
                                                                       Integrador_Funcoes.oBancoDados,
                                                                       Mensagem.idTarefa,
                                                                       Terms.WebService,
                                                                       Terms.Cod_Puxada,
                                                                       Terms.WS_TipoRegistro,
                                                                       _Funcoes.FNC_FormatarTelefone(Mensagem.contactuid),
                                                                       Terms.WS_TipoConsulta,
                                                                       Terms.dsTemplate,
                                                                       "",
                                                                       Terms.SepararMensagens);
                    break;
                  }
                case "DadosDISTSfCobertura":
                  {
                    sMensagem = FlexXTools.FlexXTools_DadosDisSfCobertura(sAplicativo,
                                                                          sPartner,
                                                                          sCdServico,
                                                                          Integrador_Funcoes.oBancoDados,
                                                                          Mensagem.idTarefa,
                                                                          Terms.WebService,
                                                                          Terms.Cod_Puxada,
                                                                          Terms.WS_TipoRegistro,
                                                                          _Funcoes.FNC_FormatarTelefone(Mensagem.contactuid),
                                                                          Terms.WS_TipoConsulta,
                                                                          Terms.dsTemplate,
                                                                          "", //Terms.WS_JsonParametros.Replace("[WS_TELEFONE]", FNC_FormatarTelefone(sTo)),
                                                                          Terms.SepararMensagens);
                    break;
                  }
                case "DadosDISTSfVendas_Grupo":
                  {
                    sMensagem = FlexXTools.FlexXTools_DadosDISTSfVendasGrupo(sAplicativo,
                                                                             sPartner,
                                                                             sCdServico,
                                                                             Integrador_Funcoes.oBancoDados,
                                                                             Mensagem.idTarefa,
                                                                             Terms.WebService,
                                                                             Terms.Cod_Puxada,
                                                                             Terms.WS_TipoRegistro,
                                                                             _Funcoes.FNC_FormatarTelefone(Mensagem.contactuid),
                                                                             Terms.WS_TipoConsulta,
                                                                             Terms.dsTemplate,
                                                                             "", //Terms.WS_JsonParametros.Replace("[WS_TELEFONE]", FNC_FormatarTelefone(sTo)),
                                                                             Terms.SepararMensagens);
                    break;
                  }
                case "DadosDISTSfIV_IAV":
                  {
                    sMensagem = FlexXTools.FlexXTools_DadosDisSfIv_Iav(sAplicativo,
                                                                       sPartner,
                                                                       sCdServico,
                                                                       Integrador_Funcoes.oBancoDados,
                                                                       Mensagem.idTarefa,
                                                                       Terms.WebService,
                                                                       Terms.Cod_Puxada,
                                                                       Terms.WS_TipoRegistro,
                                                                       _Funcoes.FNC_FormatarTelefone(Mensagem.contactuid),
                                                                       Terms.WS_TipoConsulta,
                                                                       Terms.dsTemplate,
                                                                       "", //Terms.WS_JsonParametros.Replace("[WS_TELEFONE]", FNC_FormatarTelefone(sTo)),
                                                                       Terms.SepararMensagens);
                    break;
                  }
                case "DadosDISTSfDevolucao":
                  {
                    sMensagem = FlexXTools.FlexXTools_DadosDISTSfDevolucao(sAplicativo,
                                                                           sPartner,
                                                                           sCdServico,
                                                                           Integrador_Funcoes.oBancoDados,
                                                                           Mensagem.idTarefa,
                                                                           Terms.WebService,
                                                                           Terms.Cod_Puxada,
                                                                           Terms.WS_TipoRegistro,
                                                                           _Funcoes.FNC_FormatarTelefone(Mensagem.contactuid),
                                                                           Terms.WS_TipoConsulta,
                                                                           Terms.dsTemplate,
                                                                           "", //Terms.WS_JsonParametros.Replace("[WS_TELEFONE]", FNC_FormatarTelefone(sTo)),
                                                                           Terms.SepararMensagens);
                    break;
                  }
              }

              break;
            }
        }

        if (NovaTerms == null)
          break;
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados_Tems, "Processado");

      return sMensagem;
    }

    public static string Custom_uid_Novo(ref int iId_bot_requisicao)
    {
      string sCustom_uid = "";

      Integrador_Funcoes.oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_bot_requisicao_new", new clsCampo[] {
                                                                                new clsCampo {Nome = "p_idBot", Tipo = DbType.Double, Valor = Mensagem.idBot},
                                                                                new clsCampo {Nome = "p_id_bot_requisicao", Tipo = DbType.Double, Valor = null, Direcao = ParameterDirection.Output },
                                                                                new clsCampo {Nome = "p_custom_uid", Tipo = DbType.String, Valor = null, Direcao = ParameterDirection.Output }});

      if (Integrador_Funcoes.oBancoDados.Retorno.ParametroRetorno != null)
      {
        iId_bot_requisicao = Convert.ToInt32(Integrador_Funcoes.oBancoDados.Retorno.Campo("p_id_bot_requisicao").Valor);
        sCustom_uid = Integrador_Funcoes.oBancoDados.Retorno.Campo("p_custom_uid").Valor.ToString();
      }

      return sCustom_uid;
    }

    public static void Custom_uid_Atualizar(int iId_bot_requisicao,
                                            string p_status)
    {
      Integrador_Funcoes.oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_bot_requisicao_upd", new clsCampo[] {
                                                                                new clsCampo {Nome = "p_id_bot_requisicao", Tipo = DbType.Double, Valor = iId_bot_requisicao},
                                                                                new clsCampo {Nome = "p_status", Tipo = DbType.String, Valor = p_status }});
    }

    public static void MessageSend(string custom_uid,
                                   string message_body,
                                   string message_caption,
                                   string message_response_Custon_uid,
                                   int iIdMessageTerms)
    {
      Integrador_Funcoes.oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_messagesend_ins", new clsCampo[] {
                                                                             new clsCampo {Nome = "p_idMessage", Tipo = DbType.Double, Valor = Mensagem.idMensagem},
                                                                             new clsCampo {Nome = "p_custom_uid", Tipo = DbType.String, Valor = custom_uid},
                                                                             new clsCampo {Nome = "p_message_body", Tipo = DbType.String, Valor = message_body},
                                                                             new clsCampo {Nome = "p_message_response_OK", Tipo = DbType.Double, Valor = Mensagem.idStatusMensagem},
                                                                             new clsCampo {Nome = "p_message_caption", Tipo = DbType.String, Valor = message_caption},
                                                                             new clsCampo {Nome = "p_message_response_Custon_uid", Tipo = DbType.String, Valor = message_response_Custon_uid},
                                                                             new clsCampo {Nome = "p_idStatusMensagem", Tipo = DbType.Double, Valor = Mensagem.idStatusMensagem},
                                                                             new clsCampo {Nome = "p_idMessageTerms", Tipo = DbType.Double, Valor = iIdMessageTerms},
                                                                             new clsCampo {Nome = "p_Processador", Tipo = DbType.String, Valor = Declaracao.processador}});
    }

    public static Boolean EnviarTexto(cls_tbmessageterms.cls_messageterms Terms,
                                      string[] sMensagem)
    {
      bool bOk = false;
      //ttps://www.waboxapp.com/api/send/chat?token=3850ae881c808e24a5c8281dadf15c2f5bf561a267c02&uid=16288000515&to=557399009349&custom_uid=flag_1811211216_012&text=Hello world!'

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_Mensagem, "Enviado");

      try
      {
        if (sMensagem.Length == 1)
        {
          if (sMensagem[0] == Constantes.const_TratamentoSofia_SEMACESSO ||
              sMensagem[0] == Constantes.const_TratamentoSofia_OFFLINE ||
              sMensagem[0] == Constantes.const_TratamentoSofia_ERRO998)
          {
            try
            {
              Terms = Declaracao.otbmessageterms.PesquisarPorTermo(sMensagem[0], true);
              sMensagem[0] = TratarTexto(Terms.dsTemplate, Mensagem.contactname);
            }
            catch (Exception)
            {
            }
          }
        }

        int iId_bot_requisicao = 0;
        string sCustom_uid;
        string sAux = "";

        foreach (string sMens in sMensagem)
        {
          sCustom_uid = Bot.Custom_uid_Novo(ref iId_bot_requisicao);
          sAux = sMens;

          if (Terms.dsTemplateHeader.Trim() != "") { sAux = Terms.dsTemplateHeader + Environment.NewLine + sAux; }
          if (Terms.dsTemplateFooter.Trim() != "") { sAux = sAux + Environment.NewLine + Terms.dsTemplateFooter; }

          sAux = Bot.TratarTexto(sAux, sCustom_uid);

          Mensagem.messagemtd = _Funcoes.FNC_Data_Atual_DB();

          switch (Config.Provider)
          {
            case Constantes.const_Provider_Waboxapp:
              {
                bOk = EnviarTexto_Waboxapp(Terms, sAux, sCustom_uid);
                break;
              }
            case Constantes.const_Provider_ChartAPI:
              {
                bOk = EnviarTexto_ChatAPI(sAux);
                break;
              }
          }

          Mensagem.messagedtm = _Funcoes.FNC_Data_Atual_DB();

          if (bOk)
          {
            Bot.Custom_uid_Atualizar(iId_bot_requisicao, "S");

            if (Mensagem.idMensagem != 0)
            {
              Bot.MessageSend(sCustom_uid,
                              sAux,
                              "",
                              sCustom_uid,
                              Terms.idMessageTerms);
            }
          }
        }
      }
      catch (Exception Ex1)
      {
        Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar("", "", "", "", "", "", "", 0, LogTipo.ErroNaRotina_Bot, 0, "EnviarTexto >> " + Ex1.Message);
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_Mensagem, "Enviado");

      return bOk;
    }

    public static Boolean EnviarTexto_Waboxapp(cls_tbmessageterms.cls_messageterms Terms,
                                               string sMens,
                                               string sCustom_uid)
    {
      bool Ret = false;
      string sConexao = "";

      sConexao = Config.sConexaoDestino_waboxapp_chat + "?token=" + Mensagem.Token + "&uid=" + Mensagem.Uid + "&to=" + Mensagem.To + "&custom_uid=" + sCustom_uid + "&text=" + sMens;

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_Mensagem_Waboxapp, "Enviando");

      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
        WaboxApp_WhatsApp WaboxApp_WhatsApp_Root;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        string content = "";

        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              WaboxApp_WhatsApp_Root = JsonConvert.DeserializeObject<WaboxApp_WhatsApp>(content);
            }
          }
        }

        if (WaboxApp_WhatsApp_Root != null)
        {
          Mensagem.idStatusMensagem = 0;

          if (WaboxApp_WhatsApp_Root.success) { Mensagem.idStatusMensagem = 1; } else { Mensagem.idStatusMensagem = 9; }

          Ret = WaboxApp_WhatsApp_Root.success;
        }
      }
      catch (Exception Ex)
      {
        Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar("", "", "", "", "", "", "", 0, LogTipo.ErroNaRotina_Bot, 0, "EnviarTexto_Waboxapp >> " + Ex.Message);
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_Mensagem_Waboxapp, "Enviado");

      return Ret;
    }

    public static Boolean EnviarTexto_ChatAPI(string sMens)
    {
      bool bOk = false;

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_Mensagem_ChatAPI, "Enviando");

      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://eu22.chat-api.com/instance19026/message?token=a9c3229cja75eebb"));

        request.ContentType = "application/json";
        request.Method = "POST";
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          string json = new JavaScriptSerializer().Serialize(new
          {
            chatId = Mensagem.To,
            body = sMens
          });
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)request.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
          var result = streamReader.ReadToEnd();
        }

        bOk = true;
      }
      catch (Exception Ex)
      {
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_Mensagem_ChatAPI, "Enviado");

      return bOk;
    }

    public static Boolean Webook_Util(int Solicitacao,
                                      string Servico,
                                      string Termo,
                                      string Mensagem,
                                      string Provider,
                                      string USUARIO,
                                      string Para,
                                      string Botname,
                                      string CodPuxada)
    {
      bool bOk = false;

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_Mensagem_ChatAPI, "Enviando");

      Config_App.sWebHook_Url = "http://localhost:7071/api";
      Config_App.sWebHook_Url = "http://plugthink.azurewebsites.net/api";

      try
      {
        Config_App.sWebHook_Url = Config_App.sWebHook_Url.Trim();
        if (Config_App.sWebHook_Url.Substring(Config_App.sWebHook_Url.Length - 1, 1) != "/")
          Config_App.sWebHook_Url = Config_App.sWebHook_Url.Trim() + "/";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(Config_App.sWebHook_Url + "MessageWebHook_Util"));

        request.ContentType = "application/json";
        request.Method = "POST";
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          string json = new JavaScriptSerializer().Serialize(new
          {
            Solicitacao = Solicitacao,
            Chave = "x3|b0$/; 0KvpP34%WUl|qN!|U~$OPbco`elYQGuN(gs(A#]0A!",
            Provider = Provider,
            Servico = Servico,
            Termo = Termo,
            Mensagem = Mensagem,
            Para = Para,
            Contact_Name = USUARIO,
            Botname = Botname,
            CodPuxada = CodPuxada
          });
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)request.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
          var result = streamReader.ReadToEnd();
        }

        bOk = true;
      }
      catch (Exception Ex)
      {
        Config.appLog_Escrever("Webook_Util [ERRO] " + Ex.Message);
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_Mensagem_ChatAPI, "Enviado");

      return bOk;
    }
  }

  public static class FlexXTools
  {
    public static string[] FlexXTools_DadosDISTSfVenda(string sAplicativo,
                                                       string sPartner,
                                                       string sCdServico,
                                                       clsBancoDados oBancoDados,
                                                       long iIdTarefa,
                                                       string sConexao,
                                                       string sCOD_PUXADA,
                                                       string sTIPO_REGISTRO,
                                                       string sTEL_CELULAR,
                                                       string sTIPO_CONSULTA,
                                                       string sTemplate,
                                                       string json = "",
                                                       Boolean Separar = false)
    {
      string[] sTexto = null;
      int iLinha = -1;

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVenda - Início");

      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
        DadosDISTSfVendaRoot DadosDISTSfVenda_Root;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Config.FlexXTools_Usuario + ":" + Config.FlexXTools_Senha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        if (json.Trim() == "")
          json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                  "\"TEL_CELULAR\":\"" + sTEL_CELULAR + "\"," +
                  "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        Mensagem.messagerqt = _Funcoes.FNC_Data_Atual_DB();
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              DadosDISTSfVenda_Root = JsonConvert.DeserializeObject<DadosDISTSfVendaRoot>(content);
            }
          }
        }
        Mensagem.messagerst = _Funcoes.FNC_Data_Atual_DB();

        if (DadosDISTSfVenda_Root != null)
        {
          if (Bot.ValidarRetornoJSon(DadosDISTSfVenda_Root.codigo, DadosDISTSfVenda_Root.mensagem, ref sTexto))
          {
            if (DadosDISTSfVenda_Root.DadosDISTSfVenda.Count > 0)
            {
              if (Separar) { sTexto = new string[DadosDISTSfVenda_Root.DadosDISTSfVenda.Count]; }
              { sTexto = new string[1]; }

              foreach (DadosDISTSfVenda Item in DadosDISTSfVenda_Root.DadosDISTSfVenda)
              {
                if (Separar) { iLinha = iLinha + 1; }
                { iLinha = 0; }

                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "VALOR_HECTO", Item.VALOR_HECTO);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "VALOR_VENDA", Item.VALOR_VENDA);
              }
            }
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlexXTools_DadosDISTSfVenda [" + sCOD_PUXADA + "] >> " + Ex.Message);
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVenda - Fim");

      return sTexto;
    }

    public class Wrapper
    {
      [JsonProperty("root")]
      public DataSet DataSet { get; set; }
    }

    public static DataTable FlexXTools_DataTable(string sAplicativo,
                                                 string sPartner,
                                                 string sCdServico,
                                                 ref string sStatus,
                                                 ref string sErro,
                                                 ref double TempoExecucao,
                                                 long iIdTarefa,
                                                 string sChave,
                                                 string sConexao,
                                                 string sCOD_PUXADA,
                                                 string sTIPO_REGISTRO,
                                                 string sTIPO_CONSULTA,
                                                 string sVISAO_FATURAMENTO,
                                                 ref string json,
                                                 DataTable oData = null,
                                                 bool UsarToken = true)
    {
      DataTable oDataTable = null;
      DateTime dUtil = DateTime.Now;
 
      ServicePointManager.Expect100Continue = true;
      ServicePointManager.DefaultConnectionLimit = 9999;
      ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 |
                                             SecurityProtocolType.Ssl3;

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);

      try
      {
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        if (UsarToken)
        {
          request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Config.FlexXTools_Usuario + ":" + Config.FlexXTools_Senha)));
        }
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        if (json.Trim() == "" && sCOD_PUXADA.Trim() != "")
        {
          if (json.Trim() == "" && sTIPO_REGISTRO.Trim() != "")
          {
            json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                    "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                    "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"," +
                    "\"VISAO_FATURAMENTO\":\"" + sVISAO_FATURAMENTO + "\"}";
          }
          else
          {
            json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"}";
          }
        }

        request.ContentLength = json.Length;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              json = content;

              object my_obj;

              try
              {
                my_obj = JsonConvert.DeserializeObject<JObject>(content.ToString());
                Dictionary<string, JToken> dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(my_obj.ToString());

                foreach (KeyValuePair<string, JToken> Item in dict)
                {
                  if (Item.Key == sChave)
                  {
                    if (oData != null)
                    {
                      var serializer = new JsonSerializer();
                      //serializer.Serialize(Item, oData);
                    }
                    else
                    {
                      var settings = new JsonSerializerSettings
                      {
                        FloatParseHandling = FloatParseHandling.Decimal //hint
                      };

                      // oDataTable = JsonConvert.DeserializeObject<DataTable>(Item.Value.ToString(), settings);
                      oDataTable = toDataTable(Item.Value.ToString());
                    }

                    if (oDataTable == null)
                    {
                      Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                                                      0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlexXTools_DataTable [" + sCOD_PUXADA + "] >> Interface Inativa. " + Item.Value.ToString());
                    }
                    else if (oDataTable.Rows.Count == 0)
                    {
                      Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                                                      0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlexXTools_DataTable [" + sCOD_PUXADA + "] >> Não retornou dados.");
                    }
                  }
                  else if (Item.Key.ToUpper() == "CODIGO")
                  {
                    sErro = Item.Value.ToString();

                    switch (sErro)
                    {
                      case "999":
                        sStatus = "Erro na execução";
                        break;
                      case "998":
                        sStatus = "Sem dados";
                        break;
                      case "0":
                        sStatus = "Executado";
                        break;
                      default:
                        sStatus = "Erro desconhecido";
                        break;
                    }
                  }
                }
              }
              catch (Exception)
              {
                oDataTable = toDataTable(content.ToString());
              }
            }
          }
        }
      }
      catch (Exception Ex)
      {
        if (Ex.Message == "Error reading JArray from JsonReader. Path '', line 0, position 0.")
        {
          sErro = Ex.Message;
        }
        else
        {
          sErro = Ex.Message;
        }

        sStatus = "ERRO";
        Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                                        0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlexXTools_DataTable [" + sCOD_PUXADA + "] >> " + Ex.Message);
      }
      finally
      {
        request = null;
      }

      TimeSpan ts = DateTime.Now - dUtil;

      TempoExecucao = ts.TotalSeconds;

      return oDataTable;
    }
    private static DataTable toDataTable(string json)
    {
      var result = new DataTable();
      var jArray = JArray.Parse(json);
      //Initialize the columns, If you know the row type, replace this   
      foreach (var row in jArray)
      {
        foreach (var jToken in row)
        {
          var jproperty = jToken as JProperty;
          if (jproperty == null) continue;
          if (result.Columns[jproperty.Name] == null)
            result.Columns.Add(jproperty.Name, typeof(string));
        }
      }
      foreach (var row in jArray)
      {
        var datarow = result.NewRow();
        foreach (var jToken in row)
        {
          var jProperty = jToken as JProperty;
          if (jProperty == null) continue;
          datarow[jProperty.Name] = jProperty.Value.ToString();
        }
        result.Rows.Add(datarow);
      }

      return result;
    }

    public static string[] FlexXTools_DadosDisSfCobertura(string sAplicativo,
                                                          string sPartner,
                                                          string sCdServico,
                                                          clsBancoDados oBancoDados,
                                                          long iIdTarefa,
                                                          string sConexao,
                                                          string sCOD_PUXADA,
                                                          string sTIPO_REGISTRO,
                                                          string sTEL_CELULAR,
                                                          string sTIPO_CONSULTA,
                                                          string sTemplate,
                                                          string json = "",
                                                          Boolean Separar = false)
    {
      string[] sTexto = null;
      int iLinha = -1;

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDisSfCobertura - Início");

      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
        DadosDISTSfCoberturaRoot DadosDISTSfCobertura_Root;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Config.FlexXTools_Usuario + ":" + Config.FlexXTools_Senha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        if (json.Trim() == "")
          json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                  "\"TEL_CELULAR\":\"" + sTEL_CELULAR + "\"," +
                  "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        Mensagem.messagerqt = _Funcoes.FNC_Data_Atual_DB();
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              DadosDISTSfCobertura_Root = JsonConvert.DeserializeObject<DadosDISTSfCoberturaRoot>(content);
            }
          }
        }
        Mensagem.messagerst = _Funcoes.FNC_Data_Atual_DB();

        if (DadosDISTSfCobertura_Root != null)
        {
          if (Bot.ValidarRetornoJSon(DadosDISTSfCobertura_Root.codigo, DadosDISTSfCobertura_Root.mensagem, ref sTexto))
          {
            if (DadosDISTSfCobertura_Root.DadosDISTSfCobertura.Count > 0)
            {
              if (Separar) { sTexto = new string[DadosDISTSfCobertura_Root.DadosDISTSfCobertura.Count]; }
              { sTexto = new string[1]; }

              foreach (DadosDISTSfCobertura Item in DadosDISTSfCobertura_Root.DadosDISTSfCobertura)
              {
                if (Separar) { iLinha = iLinha + 1; }
                { iLinha = 0; }

                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "DSC_GRUPO", Item.DSC_GRUPO);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "QTDE_COBERTURA", Item.QTDE_COBERTURA);

              }
            }
          }
        }
      }
      catch (Exception Ex)
      {
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                     0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlexXTools_DadosDisSfCobertura [" + sCOD_PUXADA + "] >> " + Ex.Message);
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDisSfCobertura - Fim");

      return sTexto;
    }

    public static string[] FlexXTools_DadosDISTSfVendasGrupo(string sAplicativo,
                                                             string sPartner,
                                                             string sCdServico,
                                                             clsBancoDados oBancoDados,
                                                             long iIdTarefa,
                                                             string sConexao,
                                                             string sCOD_PUXADA,
                                                             string sTIPO_REGISTRO,
                                                             string sTEL_CELULAR,
                                                             string sTIPO_CONSULTA,
                                                             string sTemplate,
                                                             string json = "",
                                                             Boolean Separar = false)
    {
      string[] sTexto = null;
      int iLinha = -1;

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVendasGrupo - Início");

      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
        DadosDISTSfVendasGrupoRoot DadosDISTSfVendasGrupo_Root;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Config.FlexXTools_Usuario + ":" + Config.FlexXTools_Senha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        if (json.Trim() == "")
          json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                  "\"TEL_CELULAR\":\"" + sTEL_CELULAR + "\"," +
                  "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        Mensagem.messagerqt = _Funcoes.FNC_Data_Atual_DB();
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              DadosDISTSfVendasGrupo_Root = JsonConvert.DeserializeObject<DadosDISTSfVendasGrupoRoot>(content);
            }
          }
        }
        Mensagem.messagerst = _Funcoes.FNC_Data_Atual_DB();

        if (DadosDISTSfVendasGrupo_Root != null)
        {
          if (Bot.ValidarRetornoJSon(DadosDISTSfVendasGrupo_Root.codigo, DadosDISTSfVendasGrupo_Root.mensagem, ref sTexto))
          {
            if (DadosDISTSfVendasGrupo_Root.DadosDISTSfVendas_Grupo.Count > 0)
            {
              if (Separar) { sTexto = new string[DadosDISTSfVendasGrupo_Root.DadosDISTSfVendas_Grupo.Count]; }
              { sTexto = new string[1]; }

              foreach (DadosDISTSfVendasGrupo Item in DadosDISTSfVendasGrupo_Root.DadosDISTSfVendas_Grupo)
              {
                if (Separar) { iLinha = iLinha + 1; }
                { iLinha = 0; }

                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "DSC_GRUPO", Item.DSC_GRUPO);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "VALOR_HECTO", Item.VALOR_HECTO);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "QTDE_VOLUME", Item.QTDE_VOLUME);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "VALOR_VENDA", Item.VALOR_VENDA);
              }
            }
          }
        }
      }
      catch (Exception Ex)
      {
        Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                                        0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlexXTools_DadosDISTSfVendasGrupo [" + sCOD_PUXADA + "] >> " + Ex.Message);
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfVendasGrupo - Fim");

      return sTexto;
    }

    public static string[] FlexXTools_DadosDisSfIv_Iav(string sAplicativo,
                                                       string sPartner,
                                                       string sCdServico,
                                                       clsBancoDados oBancoDados,
                                                       long iIdTarefa,
                                                       string sConexao,
                                                       string sCOD_PUXADA,
                                                       string sTIPO_REGISTRO,
                                                       string sTEL_CELULAR,
                                                       string sTIPO_CONSULTA,
                                                       string sTemplate,
                                                       string json = "",
                                                       Boolean Separar = false)
    {
      string[] sTexto = null;
      int iLinha = -1;

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDisSfIv_Iav - Início");

      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
        DadosDISTSfIVIAVRoot DadosDISTSfIVIAV_Root;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Config.FlexXTools_Usuario + ":" + Config.FlexXTools_Senha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        if (json.Trim() == "")
          json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                  "\"TEL_CELULAR\":\"" + sTEL_CELULAR + "\"," +
                  "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        Mensagem.messagerqt = _Funcoes.FNC_Data_Atual_DB();
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              DadosDISTSfIVIAV_Root = JsonConvert.DeserializeObject<DadosDISTSfIVIAVRoot>(content);
            }
          }
        }
        Mensagem.messagerst = _Funcoes.FNC_Data_Atual_DB();

        if (DadosDISTSfIVIAV_Root != null)
        {
          if (Bot.ValidarRetornoJSon(DadosDISTSfIVIAV_Root.codigo, DadosDISTSfIVIAV_Root.mensagem, ref sTexto))
          {
            if (DadosDISTSfIVIAV_Root.DadosDISTSfIV_IAV.Count > 0)
            {
              if (Separar) { sTexto = new string[DadosDISTSfIVIAV_Root.DadosDISTSfIV_IAV.Count]; }
              { sTexto = new string[1]; }

              foreach (DadosDISTSfIVIAV Item in DadosDISTSfIVIAV_Root.DadosDISTSfIV_IAV)
              {
                if (Separar) { iLinha = iLinha + 1; }
                { iLinha = 0; }

                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "QTDE_VISITA_PREVISTA", Item.QTDE_VISITA_PREVISTA);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "QTDE_IV", Item.QTDE_IV);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "QTDE_IAV", Item.QTDE_IAV);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "PERCENTUAL_IV", Item.PERCENTUAL_IV);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "PERCENTUAL_IAV", Item.PERCENTUAL_IAV);
              }
            }
          }
        }
      }
      catch (Exception Ex)
      {
        Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                                        0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlexXTools_DadosDISTSfVendasGrupo [" + sCOD_PUXADA + "] >> " + Ex.Message);
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDisSfIv_Iav - Fim");

      return sTexto;
    }

    public static string[] FlexXTools_DadosDISTSfDevolucao(string sAplicativo,
                                                           string sPartner,
                                                           string sCdServico,
                                                           clsBancoDados oBancoDados,
                                                           long iIdTarefa,
                                                           string sConexao,
                                                           string sCOD_PUXADA,
                                                           string sTIPO_REGISTRO,
                                                           string sTEL_CELULAR,
                                                           string sTIPO_CONSULTA,
                                                           string sTemplate,
                                                           string json = "",
                                                           Boolean Separar = false)
    {
      string[] sTexto = null;
      int iLinha = -1;

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfDevolucao - Início");

      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
        DadosDISTSfDevolucaoRoot DadosDISTSfDevolucao_Root;

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Config.FlexXTools_Usuario + ":" + Config.FlexXTools_Senha)));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        if (json.Trim() == "")
          json = "{\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                  "\"TEL_CELULAR\":\"" + sTEL_CELULAR + "\"," +
                  "\"TIPO_CONSULTA\":\"" + sTIPO_CONSULTA + "\"}";

        request.ContentLength = json.Length;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }

        var content = string.Empty;

        Mensagem.messagerqt = _Funcoes.FNC_Data_Atual_DB();
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              DadosDISTSfDevolucao_Root = JsonConvert.DeserializeObject<DadosDISTSfDevolucaoRoot>(content);
            }
          }
        }
        Mensagem.messagerst = _Funcoes.FNC_Data_Atual_DB();

        if (DadosDISTSfDevolucao_Root != null)
        {
          if (Bot.ValidarRetornoJSon(DadosDISTSfDevolucao_Root.codigo, DadosDISTSfDevolucao_Root.mensagem, ref sTexto))
          {
            if (DadosDISTSfDevolucao_Root.DadosDISTSfDevolucao.Count > 0)
            {
              if (Separar) { sTexto = new string[DadosDISTSfDevolucao_Root.DadosDISTSfDevolucao.Count]; }
              { sTexto = new string[1]; }

              foreach (DadosDISTSfDevolucao Item in DadosDISTSfDevolucao_Root.DadosDISTSfDevolucao)
              {
                if (Separar) { iLinha = iLinha + 1; }
                { iLinha = 0; }

                if (sTexto[iLinha] != "") { sTexto[iLinha] = sTexto[iLinha] + Environment.NewLine; }

                sTexto[iLinha] = sTexto[iLinha].ToString() + _Funcoes.Replace(sTemplate, "VALOR_DEVOLUCAO", Item.VALOR_DEVOLUCAO);
                sTexto[iLinha] = _Funcoes.Replace(sTexto[iLinha], "PERCENTUAL_DEVOLUCAO", Item.PERCENTUAL_DEVOLUCAO);
              }
            }
          }
        }
      }
      catch (Exception Ex)
      {
        Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                                        0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlexXTools_DadosDISTSfVendasGrupo [" + sCOD_PUXADA + "] >> " + Ex.Message);
      }

      Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_ProviderFlexXTools, "DadosDISTSfDevolucao - Fim");

      return sTexto;
    }
  }
}
