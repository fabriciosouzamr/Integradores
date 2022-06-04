using System;
using System.Data;
using static BancoDados;

namespace Integradores
{
    public static class WhatsApp
    {
       public static bool EnviarMensagem(string sAplicativo, string sPartner, string sCdServico, string message_body)
       {
            bool bOk = false;
            clsBancoDados oBancoDados;

            Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados, "Conectando");
            oBancoDados = new clsBancoDados();
            oBancoDados.DBConectar(Constantes.const_TipoBancoDados_Bot, Config.dbconstring);
            Declaracao.oMensagem_log.Adicionar(Constantes.const_Mensagem_Log_BancoDados, "Conectado");

            //Tratar somente mensagem de entrada (Mensagens enviadas pelos os usuário e não mensagens de confirmação do Chat Bot)
            if (Mensagem.messagedir.Trim().ToUpper() == "I")
            {
                int idMessageTerms = 0;
                string[] sMensagem;
                object Valor;

                cls_tbmessageterms.cls_messageterms Terms;

                if (Declaracao.otbmessageterms.messageterms == null)
                    Bot.Terms_Carregar();

                Bot.CarregarTabelas();
                Valor = Bot.Tabelas_BuscarValor("tbbot", "idBot");

                if (Valor != null)
                {
                    Mensagem.idBot = Convert.ToInt32(Valor);
                    Mensagem.Uid = Bot.Tabelas_BuscarValor("tbbot", "nroOrigem").ToString();
                }

                if (message_body != null)
                {                   
                    Terms = Declaracao.otbmessageterms.IdentificarTermo(message_body);

                    if (Terms != null)
                    {
                        if (Mensagem.idMessageTerms == 0)
                            Mensagem.idMessageTerms = Terms.idMessageTerms;

                        if (Terms.WS_TipoRetornoTipo.Trim() == "" || Terms.WS_TipoRetornoTipo == "I")
                        {
                            idMessageTerms = Terms.idMessageTerms;

                            while (idMessageTerms != 0)
                            {
                                sMensagem = Bot.Terms_Processar(sAplicativo, sPartner, sCdServico, Terms);

                                bOk = Bot.EnviarTexto(Terms, sMensagem);

                                if (Terms.WS_ProximaMensagem != 0)
                                {
                                    Terms = Declaracao.otbmessageterms.PesquisarPorId(Terms.WS_ProximaMensagem);
                                    idMessageTerms = Terms.idMessageTerms;
                                }
                                else
                                    break;
                            }
                        }
                        else
                            bOk = true;
                    }
                }

                Bot.Terms_Descarregar();

                if (!bOk)
                {
                    Mensagem.idStatusMensagem = 9;
                }
            }
            else
            {
                Mensagem.messagemtd = _Funcoes.FNC_Data_Atual_DB();
                bOk = true;
            }

            try
            {
                oBancoDados.DBDesconectar();
                oBancoDados.DBConectar(Constantes.const_TipoBancoDados_Bot, Config.dbconstring);
            }
            catch (Exception)
            {
            }

            //Gravar mensagem - Início
            string sql = "INSERT INTO {0} ({1}) VALUES({2}) ";
            string fields = @"Origem,Agente,Bot,Token,uid,contact_uid,contact_name,contact_type," +
                             "message_mtd,message_dtm,message_rcv,message_prov_rqt,message_prov_rst,message_uid,message_cuid,message_diretion,message_type," +
                             "message_body,ack_status,EventoWZ,processador,idStatusMensagem,idProtocolo,idMensagemEnviada,idMessageTerms";

            string values = string.Format("'azure','{0}','{1}','ppbh90','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',0,0,'{20}'",
                                          Mensagem.botname,
                                          Mensagem.botname,
                                          Mensagem.Uid,
                                          Mensagem.contactuid,
                                          Mensagem.contactname,
                                          Mensagem.contacttype,
                                          Mensagem.messagemtd,
                                          Mensagem.messagedtm,
                                          Mensagem.messagercv,
                                          Mensagem.messagerqt,
                                          Mensagem.messagerst,
                                          Mensagem.messageuid,
                                          Mensagem.messagecuid,
                                          Mensagem.messagedir,
                                          Mensagem.messagetype,
                                          Mensagem.messagebody,
                                          Mensagem.messageack,
                                          Mensagem.events,
                                          Declaracao.processador,
                                          Mensagem.idStatusMensagem,
                                          Mensagem.idMessageTerms);
            sql = string.Format(sql, Config.table, fields, values);
            oBancoDados.DBExecutar(sql);
            //Gravar mensagem - Fim

            //Gravar log mensagem - Início
            foreach (Mensagem_log.Item Item in Declaracao.oMensagem_log.Itens)
            {
                sql = "INSERT INTO dbo.tbmessage_log(message_uid,message_log_evento,message_log_processo,message_log_comentario, nr_ordem) VALUES" +
                                                   "('" + Item.messageuid + "','" + _Funcoes.FNC_Data_DB(Item.message_log_evento) + "','" +
                                                          Item.message_log_processo + "','" + Item.message_log_comentario + "'," +
                                                          Item.message_log_ordem.ToString() + ")";
                oBancoDados.DBExecutar(sql);
            }
            //Gravar log mensagem - Fim

            oBancoDados.DBDesconectar();
            oBancoDados = null;

            return bOk;
        }
    }
}