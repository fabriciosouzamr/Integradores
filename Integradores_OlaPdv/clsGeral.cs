using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static BancoDados;

namespace Integradores_OlaPdv
{
  public class clsGeral
  {
    public static clsBancoDados oBancoDados;

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

    public static class Config
    {
      public static System.Diagnostics.EventLog appLog;

      public static void appLog_Escrever(string sTexto)
      {
        if (appLog != null)
          appLog.WriteEntry(sTexto, System.Diagnostics.EventLogEntryType.Information);
      }
    }

    public static class Constantes
    {
      public const string const_Senha = "gltuWmM5Nr";
      public const string const_Senha_Administrador = "Admin";
      public const string const_Senha_Usuario = "Usuário";
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

    public static void Tools_Integrador(string sAplicativo,
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
      double TempoExecucaoAPI = 0;

      sConexao = "http://api02.plugthink.com/api/v2/olapdv/_proc/sp_tools_integrador?sCodigoPuxada=" + sCOD_PUXADA + "&sAcao=" + sAcao + "&sMensagem=" + sMensagem + "&sNomeIntegrador=" +
                                                                                                     sNomeIntegrador + "&sComplemento=" + sComplemento + "&sApi=" + sApi +
                                                                                                     "&nRegistros=" + nRegistros.ToString() + "&nRegistrosTotal=" + nRegistrosTotal.ToString() +
                                                                                                     "&iGrupoTarefas=" + iGrupoTarefas.ToString() + "&sProtocolo=''" +
                                                                                                     "&api_key=61ddb85819ff9348ef4f5b7c3da7e5cc2187a893e3037f80545e1e9f3fc61a42";

      FlexXTools_DataTable(sAplicativo, sPartner, sCdServico, ref sStatus, ref sErro, ref TempoExecucaoAPI, iIdTarefa,
                           "", sConexao, sCOD_PUXADA, "", "", "", "", "", ref sJsonRet, null, false);
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
                                                 string sUsuario,
                                                 string sSenha,
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
          request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
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
                      oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
                                                   0, LogTipo.ErroNaRotina_Flag, iIdTarefa, "FlexXTools_DataTable [" + sCOD_PUXADA + "] >> Interface Inativa. " + Item.Value.ToString());
                    }
                    else if (oDataTable.Rows.Count == 0)
                    {
                      oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
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
        oBancoDados.DBSQL_Log_Gravar(sAplicativo, sPartner, sCOD_PUXADA, sTIPO_REGISTRO, sTIPO_CONSULTA, sConexao, sCdServico,
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

      public static string sAplicativo = "OLAFL";
      public static string sPartner = "FLAG";
      public static string sCdServico = "OPV";

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
  }
}
