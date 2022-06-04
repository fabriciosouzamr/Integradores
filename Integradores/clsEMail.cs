using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integradores
{
    class clsEMail
    {
        public string sEMail_Host_Endereco = "smtp.gmail.com";
        public int iEMail_Host_Porta = 587;
        public Boolean bEMail_Host_Habilitar_SSl = true;
        public string sEMail_Host_Usuario = "";
        public string sEMail_Host_Senha = "";
        public Boolean bEMail_Host_UseDefaultCredentials = true;

        public string sEMail_Address_De;
        public string sEMail_Address_Para;
        public string sEMail_Titulo;
        public string sEMail_Mensagem;

        public Boolean Enviar()
        {
            try
            {
                //cria uma mensagem
                MailMessage mail = new MailMessage();

                //define os endereços
                mail.From = new MailAddress(sEMail_Address_De);
                mail.To.Add(sEMail_Address_Para);

                //define o conteúdo
                mail.Subject = sEMail_Titulo;
                mail.Body = sEMail_Mensagem;

                //envia a mensagem
                SmtpClient smtp = new SmtpClient(sEMail_Host_Endereco, iEMail_Host_Porta);
                smtp.EnableSsl = bEMail_Host_Habilitar_SSl;
                if (sEMail_Host_Usuario.Trim() != "")
                {
                    NetworkCredential cred = new NetworkCredential(sEMail_Host_Usuario, sEMail_Host_Senha);
                    smtp.Credentials = cred;
                }

                // inclui as credenciais
                smtp.UseDefaultCredentials = bEMail_Host_UseDefaultCredentials;

                smtp.Send(mail);

                return true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return false;
            }
        }

        public static bool EMail_ValidaEndereco(string enderecoEmail)
        {
            try
            {
                //define a expressão regulara para validar o email
                string texto_Validar = enderecoEmail;
                Regex expressaoRegex = new Regex(@"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}");

                // testa o email com a expressão
                if (expressaoRegex.IsMatch(texto_Validar))
                {
                    // o email é valido
                    return true;
                }
                else
                {
                    // o email é inválido
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
