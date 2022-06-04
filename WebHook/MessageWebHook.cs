using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using Integradores;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Reflection;

namespace PdfFunctions
{
    public static class MessageWebHook
    {
        [FunctionName("MessageWebHook")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, ExecutionContext context)
        {
            log.Info("C# HTTP trigger function processed a request.");
            NameValueCollection request = await req.Content.ReadAsFormDataAsync(); //await req.Content.ReadAsStringAsync();

            try
            {
                var data = request;// JObject.Parse(request);

                //NameValueCollection data = await req.Content.ReadAsFormDataAsync();
                Mensagem.botname = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "botname", true) == 0)
                .Value;

                Mensagem.events = data["event"].ToString();
                Mensagem.token = data["token"].ToString();
                Mensagem.contactuid = data["contact[uid]"].ToString();
                Mensagem.contactname = data["contact[name]"].ToString();
                Mensagem.contacttype = data["contact[type]"].ToString();
                Mensagem.messagedtm = data["message[dtm]"].ToString();
                Mensagem.messageuid = data["message[uid]"].ToString();
                Mensagem.messagecuid = data["message[cuid]"].ToString();
                Mensagem.messagedir = data["message[dir]"].ToString();
                Mensagem.messagetype = data["message[type]"].ToString();
                Mensagem.messagebody = data["message[body][text]"].ToString();
                Mensagem.messageack = data["message[ack]"].ToString();
                Mensagem.idStatusMensagem = 1;

                //Carregar configuração - Início
                string root = context.FunctionAppDirectory;
                root += "\\Config.json";

                StreamReader r = new StreamReader(root);
                var ConfigJson = r.ReadToEnd();
                var Json = JObject.Parse(ConfigJson);

                Config.Carregar(Json);

                Config.Provider = Constantes.const_Provider_Waboxapp;
                //Carregar configuração - Fim

                if (string.IsNullOrEmpty(Mensagem.botname))
                    Mensagem.botname = "alice";

                Assembly _assembly = Assembly.GetExecutingAssembly();

                Declaracao.processador = "webhook_" + _assembly.GetName().Version.Major.ToString().Trim() + "." +
                                                      _assembly.GetName().Version.Minor.ToString().Trim() + "." +
                                                      _assembly.GetName().Version.Build.ToString().Trim() + "." +
                                                      _assembly.GetName().Version.Revision.ToString().Trim();

                try
                {
                    bool bOk = false;

                    bOk = Integradores.WhatsApp.EnviarMensagem(Mensagem.messagebody);
                }
                catch (Exception ex)
                {
                    JObject errresponse = new JObject();
                    errresponse.Add("Error", ex.Message);
                    return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
                }

                //{ "status": "success", "attempt": "cuid", "id": "id register create", "request_id": "id register create"}

                JObject response = new JObject();
                response.Add("status", "success");
                response.Add("attempt", Mensagem.contactuid);
                response.Add("id", Mensagem.messagecuid);
                response.Add("request_id", Mensagem.messageuid);

                return req.CreateResponse(HttpStatusCode.OK, response.ToString());
            }
            catch(Exception ex)
                {
                    JObject errresponse = new JObject();
                    errresponse.Add("Error", ex.Message);
                    //errresponse.Add("REQ", request.);
                    return req.CreateResponse(HttpStatusCode.OK, errresponse.ToString());
            }
        }      
    }
}
