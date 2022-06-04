using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Integradores;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebHook_ChatAPI
{
    public static class WebHook_ChatAPI
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        [FunctionName("MessageWebHook")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            log.Info("C# HTTP trigger function processed a request.");

            Declaracao.oMensagem_log = new Mensagem_log();

            Mensagem.messagercv = _Funcoes.FNC_Data_Atual_DB();
            Mensagem.messagedtm = _Funcoes.FNC_Data_Atual_DB();

            dynamic data = await req.Content.ReadAsAsync<JObject>();

            string botname = req.GetQueryNameValuePairs()
            .FirstOrDefault(q => string.Compare(q.Key, "botname", true) == 0)
            .Value;

            if (string.IsNullOrEmpty(botname))
                botname = "alice";

            string sfromMe = "";
            string schatId = "";
            string sbody = "";
            string stype = "";

            JObject my_obj = JsonConvert.DeserializeObject<JObject>(data.ToString());

            Dictionary<string, JToken> dict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(my_obj.ToString());

            foreach (KeyValuePair<string, JToken> Item in dict)
            {
                //Ler mensagem - Início
                sfromMe = ((JObject)((JArray)Item.Value).First)["fromMe"].ToString();
                schatId = ((JObject)((JArray)Item.Value).First)["chatId"].ToString();
                sbody = ((JObject)((JArray)Item.Value).First)["body"].ToString();
                stype = ((JObject)((JArray)Item.Value).First)["type"].ToString();

                Mensagem.idMessageTerms = 0;
                Mensagem.Agente = botname;
                Mensagem.botname = botname;
                Mensagem.events = "message";
                Mensagem.token = "ppbh90";
                Mensagem.messagebody = sbody;
                Mensagem.messagetype = stype;
                Mensagem.messageack = "0";
                Mensagem.contacttype = "user";
                if (sfromMe.Trim().ToUpper() == "FALSE") { Mensagem.messagedir = "i"; } else { Mensagem.messagedir = "o"; };
                Mensagem.messageuid = ((JObject)((JArray)Item.Value).First)["id"].ToString();
                Mensagem.contactuid = ((JObject)((JArray)Item.Value).First)["author"].ToString();
                if (Mensagem.contactuid.IndexOf("@") > -1)
                    Mensagem.contactuid = _Funcoes.FNC_FormatarTelefone(Mensagem.contactuid.Split(new char[] { '@' })[0]);
                Mensagem.contactname = ((JObject)((JArray)Item.Value).First)["senderName"].ToString();
                Mensagem.idStatusMensagem = 1;
                Mensagem.To = schatId;

                if (Mensagem.messagedir == "o")
                {
                    Mensagem.Uid = Mensagem.contactuid;
                    Mensagem.contactuid = _Funcoes.FNC_FormatarTelefone((Mensagem.messageuid.Split(new char[] { '@' })[0]).Substring(5));
                }
                //Ler mensagem - Fim

                Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_Mensagem, "Recebida");

                //Carregar configuração - Início
                //string root = context.FunctionAppDirectory;
                //root += "\\Config.json";

                //StreamReader r = new StreamReader(root);
                //var ConfigJson = r.ReadToEnd();
                //var Json = JObject.Parse(ConfigJson);

                //Config.Carregar(Json);

                Config.Provider = Constantes.const_Provider_ChatAPI;

                Config.dbuser = "bdtest";
                Config.dbpassword = "WxA35HVmHvvH245c";
                Config.host = "191.232.185.189";
                Config.dbport = "3306";
                Config.dbname = "botservicehomogation";
                Config.table = "tbmessage";
                Config.sConexaoDestino_waboxapp_chat = "https://www.waboxapp.com/api/send/chat";

                Config.dbconstring = string.Format("Server={0};Database={1};Uid={2};Pwd={3};SslMode=none;", Config.host, Config.dbname, Config.dbuser, Config.dbpassword);
                Config.dbconstring = "Server=tcp:bstrackdb1.database.windows.net,1433;Initial Catalog=bsTrackDB1;Persist Security Info=False;User ID=admbstrack1root;Password=tRA82i6MSKEmCEK;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                Config.FlexXTools_Usuario = "flagwhats4280en";
                Config.FlexXTools_Senha = "Odfofot59flag2344959";
                //Carregar configuração - Fim

                Integradores.WhatsApp.EnviarMensagem(Mensagem.messagebody);

                break;
            }

            if (sfromMe.Trim().ToUpper() == "FALSE" && stype.Trim().ToUpper() == "CHAT")
            {
                //try
                //{
                //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://eu29.chat-api.com/instance18590/message?token=rh56r027e7oybvgd"));                   

                //    request.ContentType = "application/json";
                //    request.Method = "POST";
                //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                //    if (sbody.Trim().ToUpper().IndexOf("OLÁ") > -1 || sbody.Trim().ToUpper().IndexOf("OLA") > -1)
                //    {
                //        sbody = "Ola! Sou a Alice, Assistente Virtual! :)";
                //    }
                //    else if (sbody.Trim().ToUpper().IndexOf("OLÁ") > -1 || sbody.Trim().ToUpper().IndexOf("OLA") > -1)
                //    {
                //        sbody = "Ola! Sou a Alice, Assistente Virtual! :)";
                //    }
                //    else if (sbody.Trim().ToUpper().IndexOf("OBRIGADO") > -1 || sbody.Trim().ToUpper().IndexOf("GRATO") > -1)
                //    {
                //        sbody = "Estou aqui para ajudar ;)";
                //    }
                //    else
                //    {
                //        sbody = "Sou uma assistente virtual e ainda estou aprendendo.";
                //    }

                //    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                //    {
                //        string json = new JavaScriptSerializer().Serialize(new
                //        {
                //            chatId = schatId,
                //            body = sbody
                //        });
                //        streamWriter.Write(json);
                //        streamWriter.Flush();
                //        streamWriter.Close();
                //    }

                //    var httpResponse = (HttpWebResponse)request.GetResponse();
                //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //    {
                //        var result = streamReader.ReadToEnd();
                //    }
                //}
                //catch (Exception Ex)
                //{
                //}
            }

            return null;
        }

        public class RequestState
        {
            // This class stores the request state of the request.
            public WebRequest request;
            public RequestState()
            {
                request = null;
            }
        }

        private static void ReadCallback(IAsyncResult asynchronousResult)
        {
            RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
            WebRequest myWebRequest = myRequestState.request;

            // End the Asynchronus request.
            Stream streamResponse = myWebRequest.EndGetRequestStream(asynchronousResult);

            // Create a string that is to be posted to the uri.
            Console.WriteLine("Please enter a string to be posted:");
            string postData = Console.ReadLine();
            // Convert the string into a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Write the data to the stream.
            streamResponse.Write(byteArray, 0, postData.Length);
            streamResponse.Close();
            allDone.Set();
        }
    }
}
