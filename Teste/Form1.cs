using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using System.Net.Mail;
using static BancoDados;
using static BancoDados.clsBancoDados;
using Integradores;

namespace Teste
{
  public partial class Form1 : Form
  {
    public class cInterface
    {
      public int iTarefa;
      public string CodPuxada;
      public string Nome;
      public string JSon;
      public int Erro;
      public string Mensagem;
      public int nr_ordem_execucao;

      public string BancoDestino_ConectionString;
      public string BancoDestino_Tipo;
    }

    cInterface[] oInterface;

    clsBancoDados oBancoDados;
    clsBancoDados oBancoDados_Destino;
    public class DadosDISTDevolucao
    {
      public int CODIGO_GERENTE { get; set; }
      public int CODIGO_VENDEDOR { get; set; }
      public string NOME_GERENTE { get; set; }
      public string NOME_SUPERVISOR { get; set; }
      public string NOME_VENDEDOR { get; set; }
      public int PERCENTUAL_DEVOLUCAO { get; set; }
      public int VALOR_BRUTO_DEVOLUCAO { get; set; }
      public int VALOR_BRUTO_VENDA { get; set; }
    }

    public class RootObject
    {
      public List<DadosDISTDevolucao> DadosDISTDevolucao { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      oBancoDados = new clsBancoDados();

      oBancoDados.DBConectar("SQLSRV", "server=sql1.plugthink.com;database=i9ativa;uid=i9admin;pwd=8nbzw4FFrXEzJui");

      timer1.Enabled = true;
    }

    private void Carregar()
    {

    }

    [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, Unrestricted = true)]
    internal static object CallWebService(string webServiceAsmxUrl, string serviceName, string methodName, object[] args)
    {
      // Veja http://social.msdn.microsoft.com/Forums/vstudio/en-US/39138d08-aa08-4c0c-9a58-0eb81a672f54/adding-a-web-reference-dynamically-at-runtime?forum=csharpgeneral
      System.Net.WebClient client = new System.Net.WebClient();

      // Connect To the web service
      System.IO.Stream stream = client.OpenRead(webServiceAsmxUrl + "?wsdl");

      // Now read the WSDL file describing a service.
      var description = System.Web.Services.Description.ServiceDescription.Read(stream);

      ///// LOAD THE DOM /////////       
      // Initialize a service description importer.
      var importer = new System.Web.Services.Description.ServiceDescriptionImporter();

      importer.ProtocolName = "Soap12"; // Use SOAP 1.2.
      importer.AddServiceDescription(description, null, null);

      // Generate a proxy client.
      importer.Style = System.Web.Services.Description.ServiceDescriptionImportStyle.Client;

      // Generate properties to represent primitive values.
      importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

      // Initialize a Code-DOM tree into which we will import the service.
      var nmspace = new System.CodeDom.CodeNamespace();
      var unit1 = new System.CodeDom.CodeCompileUnit();

      unit1.Namespaces.Add(nmspace);

      // Import the service into the Code-DOM tree. This creates proxy code that uses the service.
      var warning = importer.Import(nmspace, unit1);

      if (warning == 0) // If zero then we are good to go
      {
        // Generate the proxy code
        var provider1 = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp");

        // Compile the assembly proxy with the appropriate references
        string[] assemblyReferences = new string[5] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll" };

        var parms = new System.CodeDom.Compiler.CompilerParameters(assemblyReferences);
        var results = provider1.CompileAssemblyFromDom(parms, unit1);

        // Check For Errors
        if (results.Errors.Count > 0)
        {
          foreach (System.CodeDom.Compiler.CompilerError oops in results.Errors)
          {
            System.Diagnostics.Debug.WriteLine("========Compiler error============");
            System.Diagnostics.Debug.WriteLine(oops.ErrorText);
          }

          throw new System.Exception("Compile Error Occured calling webservice. Check Debug ouput window.");
        }

        // Finally, Invoke the web service method
        object wsvcClass = results.CompiledAssembly.CreateInstance(serviceName);

        var mi = wsvcClass.GetType().GetMethod(methodName);

        return mi.Invoke(wsvcClass, args);
      }
      else
      {
        return null;
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.flexxtools.com.br/Flag.WS.Whatsapp/WSMovimenta.svc/DisDevolucao");

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        //request.Headers.Add("Authorization", "Basic ZmxhZ3doYXRzNDI4MGVuOk9kZm9mb3Q1OWZsYWcyMzQ0OTU5");
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("flagwhats4280en:Odfofot59flag2344959")));
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        string json = "{\"COD_PUXADA\":\"00110100\"," +
                      "\"TIPO_REGISTRO\":\"1\"," +
                      "\"TEL_CELULAR\":\"31988888888\"," +
                      "\"TIPO_CONSULTA\":\"2\"}";

        request.ContentLength = json.Length;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Flush();
          streamWriter.Close();
        }
        //request.UserAgent = RequestConstants.UserAgentValue;
        //request.Headers.Add("content-type", "application/json; charset=utf-8");
        //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

        var content = string.Empty;

        using (var response = (HttpWebResponse)request.GetResponse())
        {
          using (var stream = response.GetResponseStream())
          {
            using (var sr = new StreamReader(stream))
            {
              content = sr.ReadToEnd();
              RootObject previsao = JsonConvert.DeserializeObject<RootObject>(content);
            }
          }
        }
      }
      catch (Exception Ex)
      {
        MessageBox.Show(Ex.Message);
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      textBox2.Text = DateTime.Now.ToString(textBox1.Text);
    }

    private void button3_Click(object sender, EventArgs e)
    {
      if (Enviar(textBoxPara.Text, "",
                 textBoxAssunto.Text,
                 richTextBoxCorpo.Text,
                 new string[] { "P:\\29181009316617000195550020000002251100221920.xml" }))
      {
        MessageBox.Show("Enviado");
      }
    }

    private bool Enviar(string sPara,
                        string sCC,
                        string sAssunto,
                        string sMensagem,
                        string[] sAnexo = null)
    {
      bool bOk = false;

      try
      {
        using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
        {
          smtp.Host = "smtp.gmail.com";
          smtp.Port = 587;
          smtp.EnableSsl = true;
          smtp.UseDefaultCredentials = false;
          smtp.Credentials = new System.Net.NetworkCredential("prog05@mrsoft.com.br", "Job#P01MS__");

          using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
          {
            mail.From = new System.Net.Mail.MailAddress("prog05@mrsoft.com.br");

            if (!string.IsNullOrWhiteSpace(sPara))
            {
              mail.To.Add(new System.Net.Mail.MailAddress(sPara));
            }
            else
            {
              MessageBox.Show("Campo 'para' é obrigatório.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        MessageBox.Show(Ex.Message);
      }

      Sair:
      return bOk;
    }

    public static string EnviaMensagemEmail(string Destinatario, string Remetente, string Assunto, string enviaMensagem)
    {
      try
      {
        // cria uma mensagem
        MailMessage mensagemEmail = new MailMessage(Remetente, Destinatario, Assunto, enviaMensagem);

        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
        client.EnableSsl = true;
        NetworkCredential cred = new NetworkCredential("prog05@mrsoft.com.br", "Job#P01MS__");
        client.Credentials = cred;

        // inclui as credenciais
        client.UseDefaultCredentials = false;

        // envia a mensagem
        client.Send(mensagemEmail);

        return "Mensagem enviada para  " + Destinatario + " às " + DateTime.Now.ToString() + ".";
      }
      catch (Exception ex)
      {
        return ex.Message;
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

    public static bool FNC_IsNumeric(string s)
    {
      try
      {
        float output;
        return float.TryParse(s, out output);
      }
      catch (Exception)
      {
        return false;
      }
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

    private void button4_Click(object sender, EventArgs e)
    {
      oBancoDados = new clsBancoDados();

      oBancoDados.DBConectar("SQLSRV", "server=sql1.plugthink.com;database=i9ativa;uid=i9admin;pwd=8nbzw4FFrXEzJui");

      label1.Text = DateTime.Now.ToString();
      label1.Refresh();

      Application.DoEvents();

      Processar();

      label2.Text = DateTime.Now.ToString();
      label2.Refresh();
    }

    private void Processar()
    {
      DataTable oData;
      string sSqlText = "";
      string sCOD_PUXADA = "";
      bool bFim = false;
      int iCont = 0;
      string ds_parametro = "";
      string[,] oParametro = null;
      OlaPdv_Root OlaPdv_Root;
      bool Erro = false;
      string sIntegracao = "OlaPdv_" + DateTime.Now.ToString("yyyyMMddHHmm");
      clsBancoDados oBancoDados_Destino;

      string idTipoEmpresa = "";
      string sCNPJ_CPF = "";
      int iEmpresa = 0;
      string sCONDICAO_PAGTO = "";
      string sTABELA_PRECO = "";

      Config_App.ProductVersion = "10.0.0.1";
      sSqlText = "select * from vw_tarefas where id_Servico = 9 and HostLeitura = '" + Config_App.sProcessador + "' order by CodPuxada, nr_ordem_execucao";
      sSqlText = "select * from vw_tarefas_executar where id_servico = 9 and idEmpresa = 29 order by nr_ordem_execucao";
      oData = oBancoDados.DBQuery(sSqlText);

      if (oData.Rows.Count != 0)
      {
        oInterface = new cInterface[oData.Rows.Count];

        foreach (DataRow oRow in oData.Rows)
        {
          iCont = iCont + 1;
          bFim = false;

          oInterface[iCont - 1] = new cInterface();
          oInterface[iCont - 1].iTarefa = Convert.ToInt32(oRow["idTarefa"]);
          oInterface[iCont - 1].CodPuxada = oRow["CodPuxada"].ToString();
          oInterface[iCont - 1].Nome = oRow["Servico"].ToString();
          oInterface[iCont - 1].nr_ordem_execucao = Convert.ToInt32(oRow["nr_ordem_execucao"].ToString());
          oInterface[iCont - 1].BancoDestino_ConectionString = oRow["ds_stringconexaoDestino"].ToString();
          oInterface[iCont - 1].BancoDestino_Tipo = oRow["tp_bancodadosDestino"].ToString();

          if (sCOD_PUXADA.Trim() != oRow["CodPuxada"].ToString())
          {
            sCOD_PUXADA = oRow["CodPuxada"].ToString();
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

          if (iCont == oData.Rows.Count )
          {
            bFim = true;
          }
          else
          {
            if (oRow["CodPuxada"].ToString() != oData.Rows[iCont]["CodPuxada"].ToString())
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

            sSqlText = "select idEmpresa from tb_empresas where trim(ChaveEdi) = '" + sCOD_PUXADA + "'";
            iEmpresa = Convert.ToInt32(oBancoDados_Destino.DBQuery_ValorUnico(sSqlText, 0));

            sCaminho = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + sIntegracao
                                                                               + "_" + sCOD_PUXADA 
                                                                               + "_"+ _Funcoes.FNC_ZerosEsquerda(oItem.nr_ordem_execucao.ToString(), 2)
                                                                               + "_" + oItem.Nome + ".txt";

            x = File.CreateText(sCaminho);
            x.WriteLine(oItem.JSon);
            x.Close();

            iCont = 0;

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

                    if (OlaPDV_CondicaoPagamento_Root.CONDICAO_PAGAMENTO.Count > 0)
                    {
                      foreach (OlaPDV_CondicaoPagamento OlaPDV_CondicaoPagamento in OlaPDV_CondicaoPagamento_Root.CONDICAO_PAGAMENTO)
                      {
                        iCont++;

                        sEDI_Integracao = OlaPDV_CondicaoPagamento.COD_CONDICAO_PAGAMENTO.ToString().Trim().PadLeft(3, '0');

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_CondicaoPagamento_Root.CONDICAO_PAGAMENTO.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_CondicaoPagamento", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                        new clsCampo { Nome = "sDSC_CONDICAO_PAGAMENTO", Valor = OlaPDV_CondicaoPagamento.DSC_CONDICAO_PAGAMENTO, Tipo = DbType.String },
                                                                                                        new clsCampo { Nome = "iCOD_TIPO_CONDICAO_PAGAMENTO", Valor = OlaPDV_CondicaoPagamento.COD_CONDICAO_PAGAMENTO, Tipo = DbType.Int32 },
                                                                                                        new clsCampo { Nome = "sEDI_Integracao", Valor = sEDI_Integracao, Tipo = DbType.String },
                                                                                                        new clsCampo { Nome = "iIND_ATIVO", Valor = OlaPDV_CondicaoPagamento.IND_ATIVO, Tipo = DbType.Int32 },
                                                                                                        new clsCampo { Nome = "dPRC_ADICIONAL_FINANCEIRO", Valor = OlaPDV_CondicaoPagamento.PRC_ADICIONAL_FINANCEIRO, Tipo = DbType.Double },
                                                                                                        new clsCampo { Nome = "iPRIORIDADE_CONDICAO_PAGAMENTO", Valor = OlaPDV_CondicaoPagamento.PRIORIDADE_CONDICAO_PAGAMENTO, Tipo = DbType.Int32 },
                                                                                                        new clsCampo { Nome = "sTIPO_DOCUMENTO", Valor = OlaPDV_CondicaoPagamento.TIPO_DOCUMENTO, Tipo = DbType.String },
                                                                                                        new clsCampo { Nome = "dVALOR_MINIMO", Valor = OlaPDV_CondicaoPagamento.VLR_MINIMO_PEDIDO, Tipo = DbType.Double },
                                                                                                        new clsCampo { Nome = "iNroParcelas", Valor = FNC_NuloZero(OlaPDV_CondicaoPagamento.QUANTIDADE_PARCELAS), Tipo = DbType.Int32 },
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

                    if (OlaPDV_Vendedor_Root.VENDEDOR.Count > 0)
                    {
                      foreach (OlaPDV_Vendedor OlaPDV_Vendedor in OlaPDV_Vendedor_Root.VENDEDOR)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_Vendedor_Root.VENDEDOR.Count);

                        oBancoDados_Destino.DBProcedure("sp_OlaPdv_Vendedor", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                               new clsCampo { Nome = "sCOD_VENDEDOR", Valor = OlaPDV_Vendedor.COD_VENDEDOR, Tipo = DbType.String },
                                                                                               new clsCampo { Nome = "sVendedor", Valor = OlaPDV_Vendedor.NOM_VENDEDOR, Tipo = DbType.String },
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

                    if (OlaPDV_TabelaPreco_Root.TABELA_PRECO.Count > 0)
                    {
                      foreach (OlaPDV_TabelaPreco OlaPDV_TabelaPreco in OlaPDV_TabelaPreco_Root.TABELA_PRECO)
                      {
                        iCont++;

                        sEDI_Integracao = OlaPDV_TabelaPreco.COD_TABELA_PRECO.ToString().Trim().PadLeft(2, '0');

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_TabelaPreco_Root.TABELA_PRECO.Count);

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
                    string sNUMERO_ORDEM_IMPRESSAO;

                    OlaPDV_Produto_Root = JsonConvert.DeserializeObject<OlaPDV_Produto_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_olapdv_produto_inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                 new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_Produto_Root.PRODUTO.Count > 0)
                    {
                      foreach (OlaPDV_Produto OlaPDV_Produto in OlaPDV_Produto_Root.PRODUTO)
                      {
                        iCont++;

                        try
                        {
                          sNUMERO_ORDEM_IMPRESSAO = FNC_ZerosEsquerda(OlaPDV_Produto.NUMERO_ORDEM_IMPRESSAO.ToString().Trim(), 5);
                        }
                        catch (Exception)
                        {
                          sNUMERO_ORDEM_IMPRESSAO = "00000";
                        }

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_Produto_Root.PRODUTO.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_produto", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                              new clsCampo { Nome = "sCOD_MARCA", Valor = OlaPDV_Produto.COD_MARCA, Tipo = DbType.String },
                                                                                              new clsCampo { Nome = "sCOD_FAMILIA", Valor = OlaPDV_Produto.COD_FAMILIA, Tipo = DbType.String },
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
                                                                                              new clsCampo { Nome = "sIND_ATIVO", Valor = OlaPDV_Produto.IND_ATIVO.Trim(), Tipo = DbType.String },
                                                                                              new clsCampo { Nome = "dPCT_MAXIMO_DESCONTO", Valor = OlaPDV_Produto.PCT_MAXIMO_DESCONTO, Tipo = DbType.Double },
                                                                                              new clsCampo { Nome = "dVLR_PESO_PRODUTO", Valor = OlaPDV_Produto.VLR_PESO_PRODUTO, Tipo = DbType.Double },
                                                                                              new clsCampo { Nome = "sNumeroOrdemImpressao", Valor = sNUMERO_ORDEM_IMPRESSAO, Tipo = DbType.String },
                                                                                              new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                              new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } });
                      }
                    }

                    break;
                  }
                case "OlaPdvEstoqueProduto":
                  {
                    OlaPDV_EstoqueProduto_Root = JsonConvert.DeserializeObject<OlaPDV_EstoqueProduto_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_olapdv_EstoqueProduto_inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String } });

                    if (OlaPDV_EstoqueProduto_Root.ESTOQUE_PRODUTO.Count > 0)
                    {
                      foreach (OlaPDV_EstoqueProduto OlaPDV_EstoqueProduto in OlaPDV_EstoqueProduto_Root.ESTOQUE_PRODUTO)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_EstoqueProduto_Root.ESTOQUE_PRODUTO.Count);

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

                    if (OlaPDV_TabelaPrecoProduto_Root.TABELA_PRECO_PRODUTO.Count > 0)
                    {
                      foreach (OlaPDV_TabelaPrecoProduto OlaPDV_TabelaPrecoProduto in OlaPDV_TabelaPrecoProduto_Root.TABELA_PRECO_PRODUTO)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_TabelaPrecoProduto_Root.TABELA_PRECO_PRODUTO.Count);

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

                      oBancoDados_Destino.DBProcedure("sp_olapdv_tabelaprecoproduto_fim", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String } });

                    }

                    break;
                  }
                case "OlaPdvConfiguracaoEscalonada":
                  {
                    OlaPDV_ConfiguracaoEscalonada_Root = JsonConvert.DeserializeObject<OlaPDV_ConfiguracaoEscalonada_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_OlaPdv_ConfiguracaoEscalonada_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                                new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                                new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_ConfiguracaoEscalonada_Root.CONFIGURACAO_ESCALONADA.Count > 0)
                    {
                      foreach (OlaPDV_ConfiguracaoEscalonada OlaPDV_ConfiguracaoEscalonada in OlaPDV_ConfiguracaoEscalonada_Root.CONFIGURACAO_ESCALONADA)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_ConfiguracaoEscalonada_Root.CONFIGURACAO_ESCALONADA.Count);

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

                    if (OlaPDV_CategoriaEscalonada_Root.CATEGORIA_ESCALONADA.Count > 0)
                    {
                      foreach (OlaPDV_CategoriaEscalonada OlaPDV_CategoriaEscalonada in OlaPDV_CategoriaEscalonada_Root.CATEGORIA_ESCALONADA)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_CategoriaEscalonada_Root.CATEGORIA_ESCALONADA.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_CategoriaEscalonada", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                          new clsCampo { Nome = "sCOD_TABELA_PRECO", Valor = OlaPDV_CategoriaEscalonada.COD_TABELA_PRECO.ToString().Trim().PadLeft(2, '0'), Tipo = DbType.String },
                                                                                                          new clsCampo { Nome = "iCOD_CTI", Valor = OlaPDV_CategoriaEscalonada.COD_CTI, Tipo = DbType.Int32 },
                                                                                                          new clsCampo { Nome = "iCOD_ESCALONADA", Valor = OlaPDV_CategoriaEscalonada.COD_ESCALONADA, Tipo = DbType.Int32 },
                                                                                                          new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                          new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } }); ;
                      }

                      oBancoDados_Destino.DBProcedure("sp_olapdv_CategoriaEscalonada_Total", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String } });
                    }

                    break;
                  }
                case "OlaPdvEscalonadaFaixaDesconto":
                  {
                    OlaPDV_EscalonadaFaixaDesconto_Root = JsonConvert.DeserializeObject<OlaPDV_EscalonadaFaixaDesconto_Root>(oItem.JSon);

                    oBancoDados_Destino.DBProcedure("sp_olapdv_FaixaDescontoEscalonada_Inicio", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                                 new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                                 new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 }});

                    if (OlaPDV_EscalonadaFaixaDesconto_Root.ESCALONADA_FAIXA_DESCONTO.Count > 0)
                    {
                      foreach (OlaPDV_EscalonadaFaixaDesconto OlaPDV_EscalonadaFaixaDesconto in OlaPDV_EscalonadaFaixaDesconto_Root.ESCALONADA_FAIXA_DESCONTO)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_EscalonadaFaixaDesconto_Root.ESCALONADA_FAIXA_DESCONTO.Count);

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

                    if (OlaPDV_Escalonada_Root.PDV_ESCALONADA.Count > 0)
                    {
                      foreach (OlaPDV_Escalonada OlaPDV_Escalonada in OlaPDV_Escalonada_Root.PDV_ESCALONADA)
                      {
                        iCont++;

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_Escalonada_Root.PDV_ESCALONADA.Count);

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

                    if (OlaPDV_CONFIGURACAO_PDV_Root.CONFIGURACAO_PDV.Count > 0)
                    {
                      foreach (OlaPDV_Configuracao_PDV OlaPdv_CONFIGURACAO_PDV in OlaPDV_CONFIGURACAO_PDV_Root.CONFIGURACAO_PDV)
                      {
                        iCont++;

                        if (OlaPdv_CONFIGURACAO_PDV.CNPJ_CPF.ToString().Trim().Replace(".", "").Replace("-", "").Replace("/", "").Length == 11)
                        {
                          idTipoEmpresa = "F";
                          sCNPJ_CPF = Convert.ToUInt64(OlaPdv_CONFIGURACAO_PDV.CNPJ_CPF.ToString()).ToString(@"000\.000\.000\-00");
                        }
                        else
                        {
                          idTipoEmpresa = "J";
                          sCNPJ_CPF = Convert.ToUInt64(OlaPdv_CONFIGURACAO_PDV.CNPJ_CPF.ToString()).ToString(@"00\.000\.000\/0000\-00");
                        }

                        if (OlaPdv_CONFIGURACAO_PDV.CONDICAO_PAGTO.ToString().IndexOf(",") == -1)
                        {
                          sCONDICAO_PAGTO = OlaPdv_CONFIGURACAO_PDV.CONDICAO_PAGTO;
                        }
                        else
                        {
                          sCONDICAO_PAGTO = OlaPdv_CONFIGURACAO_PDV.CONDICAO_PAGTO.ToString().Split(',')[0].Trim();
                        }

                        if (OlaPdv_CONFIGURACAO_PDV.TABELA_PRECO.ToString().IndexOf(",") == -1)
                        {
                          sTABELA_PRECO = OlaPdv_CONFIGURACAO_PDV.TABELA_PRECO;
                        }
                        else
                        {
                          sTABELA_PRECO = OlaPdv_CONFIGURACAO_PDV.TABELA_PRECO.ToString().Split(',')[0].Trim();
                        }

                        Tools_Integrador(Config_App.sAplicativo, Config_App.sPartner, Config_App.sCdServico, 0, 0, sCOD_PUXADA, "Processando", oItem.Nome, Config_App.sProcessador, oItem.nr_ordem_execucao.ToString(), oItem.BancoDestino_ConectionString, iCont, OlaPDV_CONFIGURACAO_PDV_Root.CONFIGURACAO_PDV.Count);

                        oBancoDados_Destino.DBProcedure("sp_olapdv_Configuracao", new clsCampo[] { new clsCampo { Nome = "sCOD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCOD_PDV", Valor = OlaPdv_CONFIGURACAO_PDV.COD_PDV, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCNPJ_CPF", Valor = sCNPJ_CPF, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sRAZAO_SOCIAL", Valor = OlaPdv_CONFIGURACAO_PDV.RAZAO_SOCIAL, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sNOME_FANTASIA", Valor = OlaPdv_CONFIGURACAO_PDV.NOME_FANTASIA, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCOD_CATEGORIA_VAREJO", Valor = OlaPdv_CONFIGURACAO_PDV.COD_CATEGORIA_VAREJO, Tipo = DbType.Int32 },
                                                                                                   new clsCampo { Nome = "sUF", Valor = OlaPdv_CONFIGURACAO_PDV.UF, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sENDERECO", Valor = OlaPdv_CONFIGURACAO_PDV.ENDERECO, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sBairro", Valor = OlaPdv_CONFIGURACAO_PDV.BAIRRO, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCidade", Valor = OlaPdv_CONFIGURACAO_PDV.CIDADE, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sCEP", Valor = OlaPdv_CONFIGURACAO_PDV.CEP, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sLATITUDE", Valor = FNC_NuloZero(OlaPdv_CONFIGURACAO_PDV.LATITUDE), Tipo = DbType.Double },
                                                                                                   new clsCampo { Nome = "sLONGITUDE", Valor = FNC_NuloZero(OlaPdv_CONFIGURACAO_PDV.LONGITUDE), Tipo = DbType.Double },
                                                                                                   new clsCampo { Nome = "sCONDICAO_PAGTO", Valor = FNC_ZerosEsquerda(sCONDICAO_PAGTO, 3), Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sVENDEDOR", Valor = OlaPdv_CONFIGURACAO_PDV.VENDEDOR, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "dSALDO_DISPONIVEL", Valor = OlaPdv_CONFIGURACAO_PDV.SALDO_DISPONIVEL, Tipo = DbType.Double },
                                                                                                   new clsCampo { Nome = "sTABELA_PRECO", Valor = sTABELA_PRECO.Trim(), Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sIND_ATIVO", Valor = OlaPdv_CONFIGURACAO_PDV.IND_ATIVO, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "iTipoEmpresa", Valor = idTipoEmpresa, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "sVerSincronizador", Valor = Config_App.ProductVersion, Tipo = DbType.String },
                                                                                                   new clsCampo { Nome = "iSincronizador", Valor = Config_App.idIntegrador, Tipo = DbType.Int32 } });
                      }

                      oBancoDados_Destino.DBProcedure("rot_final_integracao_olapdvV2", new clsCampo[] { new clsCampo { Nome = "iEmpresa", Valor = iEmpresa, Tipo = DbType.String } });
                    }
;
                    break;
                  }
              }
            }
            catch (Exception Ex)
            {
              sCaminho = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + sIntegracao
                                                                                 + "_" + sCOD_PUXADA
                                                                                 + "_" + _Funcoes.FNC_ZerosEsquerda(oItem.nr_ordem_execucao.ToString(), 2)
                                                                                 + "_" + oItem.Nome + "_ERRO.txt";

              x = File.CreateText(sCaminho);
              x.WriteLine(Ex.Message);
              x.Close();
            }

            oBancoDados.DBDesconectar();
            oBancoDados.DBConectar("SQLSRV", "server=sql1.plugthink.com;database=i9ativa;uid=i9admin;pwd=8nbzw4FFrXEzJui");
            Config_App.sDB_BancoDados = "dbo";
            oBancoDados.DBProcedure(Config_App.sDB_BancoDados + ".sp_tarefas_final", new clsCampo[] { new clsCampo { Nome = "idTarefa", Tipo = DbType.Double, Valor = oItem.iTarefa },
                                                                                                      new clsCampo { Nome = "json", Tipo = DbType.String, Valor = " " },
                                                                                                      new clsCampo { Nome = "tpFim", Tipo = DbType.String, Valor = "N",  Direcao = ParameterDirection.Output }});

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

    private void Tools_Integrador(object sAplicativo, object sPartner, object sCdServico, int v1, int v2, string sCOD_PUXADA, string v3, object nome, string sProcessador, object p, object bancoDestino_ConectionString, int iCont, int count)
    {
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
      public string DATA_CADASTRO { get; set; }
      public string ENDERECO { get; set; }
      public string GRUPO_COMERCIALIZACAO { get; set; }
      public string IND_ATIVO { get; set; }
      public double? LATITUDE { get; set; }
      public double LIMITE_CREDITO { get; set; }
      public double? LONGITUDE { get; set; }
      public string NOME_FANTASIA { get; set; }
      public string RAZAO_SOCIAL { get; set; }
      public double SALDO_DISPONIVEL { get; set; }
      public string STATUS_FINANCEIRO { get; set; }
      public string TABELA_PRECO { get; set; }
      public string UF { get; set; }
      public string VENDEDOR { get; set; }
    }

    public class OlaPDV_Configuracao_PDV_Root
    {
      public List<OlaPDV_Configuracao_PDV> CONFIGURACAO_PDV { get; set; }
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
      public List<OlaPDV_CondicaoPagamento> CONDICAO_PAGAMENTO { get; set; }
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
      public List<OlaPDV_PDV_Escalonada> PDV_ESCALONADA { get; set; }
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
      public List<OlaPDV_TabelaPrecoProduto> TABELA_PRECO_PRODUTO { get; set; }
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
      public List<OlaPDV_TabelaPreco> TABELA_PRECO { get; set; }
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
      public List<OlaPDV_Vendedor> VENDEDOR { get; set; }
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
      public double PCT_MAXIMO_DESCONTO { get; set; }
      public int QTD_UNIDADE_MEDIDA { get; set; }
      public double VLR_PESO_PRODUTO { get; set; }
    }

    public class OlaPDV_Produto_Root
    {
      public List<OlaPDV_Produto> PRODUTO { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    public class OlaPDV_EstoqueProduto
    {
      public int COD_PRODUTO_INTERNO { get; set; }
      public double QTD_ESTOQUE { get; set; }
    }

    public class OlaPDV_EstoqueProduto_Root
    {
      public List<OlaPDV_EstoqueProduto> ESTOQUE_PRODUTO { get; set; }
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
      public List<OlaPDV_ConfiguracaoEscalonada> CONFIGURACAO_ESCALONADA { get; set; }
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
      public List<OlaPDV_CategoriaEscalonada> CATEGORIA_ESCALONADA { get; set; }
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
      public double QTD_FINAL_ESCALONADA { get; set; }
      public double QTD_INICIAL_ESCALONADA { get; set; }
    }

    public class OlaPDV_EscalonadaFaixaDesconto_Root
    {
      public List<OlaPDV_EscalonadaFaixaDesconto> ESCALONADA_FAIXA_DESCONTO { get; set; }
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
      public List<OlaPDV_Escalonada> PDV_ESCALONADA { get; set; }
      public int codigo { get; set; }
      public string mensagem { get; set; }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      oBancoDados.DBReconectar("SQLSRV", "server=sql1.plugthink.com;database=i9ativa;uid=i9admin;pwd=8nbzw4FFrXEzJui");

      label1.Text = DateTime.Now.ToString();
      label1.Refresh();

      Application.DoEvents();

      Processar();

      label2.Text = DateTime.Now.ToString();
      label2.Refresh();
    }
  }
}
