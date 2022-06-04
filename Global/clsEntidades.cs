using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integradores
{
    public static class Entidades
    {
        public static class FlagWSWhatsapp_DadosUsuario
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

        public static class FlagWSWhatsapp_DadosUsuario_Root
        {
            public List<FlagWSWhatsapp_DadosUsuario> DadosUsuario { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public static class FlagWSWhatsapp_DisDevolucao_Root
        {
            public List<FlagWSWhatsapp_DisDevolucao_Dados> DadosDISTDevolucao { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public static class FlagWSWhatsapp_DisDevolucao_Dados
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

        public static class FlagWSWhatsapp_DadosDISTTroca_Root
        {
            public List<FlagWSWhatsapp_DadosDISTTroca> DadosDISTTroca { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTTroca
        {
            public int CODIGO_GERENTE { get; set; }
            public int CODIGO_SUPERVISOR { get; set; }
            public int CODIGO_VENDEDOR { get; set; }
            public string NOME_GERENTE { get; set; }
            public string NOME_SUPERVISOR { get; set; }
            public string NOME_VENDEDOR { get; set; }
            public int PERCENTUAL_TROCA { get; set; }
            public int VALOR_BRUTO_TROCA { get; set; }
            public int VALOR_BRUTO_VENDA { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTIavIv_Root
        {
            public List<FlagWSWhatsapp_DadosDISTIavIv> DadosDISTIav_iv { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTIavIv
        {
            public int CODIGO_GERENTE { get; set; }
            public int CODIGO_SUPERVISOR { get; set; }
            public int CODIGO_VENDEDOR { get; set; }
            public string NOME_GERENTE { get; set; }
            public string NOME_SUPERVISOR { get; set; }
            public string NOME_VENDEDOR { get; set; }
            public int PERCENTUAL_VISITA_REALIZADA { get; set; }
            public int PERCENTUAL_VISITA_REALIZADA_VENDA { get; set; }
            public int QTDE_VISITA_PREVISTA { get; set; }
            public int QTDE_VISITA_REALIZADA { get; set; }
            public int QTDE_VISITA_REALIZADA_VENDA { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTInadimplencia_Root
        {
            public List<FlagWSWhatsapp_DadosDISTInadimplencia> DadosDISTInadimplencia { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTInadimplencia
        {
            public int CODIGO_GERENTE { get; set; }
            public int CODIGO_SUPERVISOR { get; set; }
            public int CODIGO_VENDEDOR { get; set; }
            public string NOME_GERENTE { get; set; }
            public string NOME_SUPERVISOR { get; set; }
            public string NOME_VENDEDOR { get; set; }
            public int PERCENTUAL_INADIMPLENCIA { get; set; }
            public int VALOR_BRUTO_VENDA { get; set; }
            public int VALOR_INADIMPLENCIA { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTLogDevolucao_Root
        {
            public List<FlagWSWhatsapp_DadosDISTLogDevolucao> DadosDISTLogDevolucao { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTLogDevolucao
        {
            public int CODIGO_MOTORISTA { get; set; }
            public int CODIGO_VEICULO { get; set; }
            public string NOME_MOTORISTA { get; set; }
            public string NUM_PLACA { get; set; }
            public int PERCENTUAL_DEVOLUCAO { get; set; }
            public int VALOR_BRUTO_DEVOLUCAO { get; set; }
            public int VALOR_BRUTO_VENDA { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao_Root
        {
            public List<FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao> DadosDISTLog_Taxa_Ocupacao { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao
        {
            public int CODIGO_VEICULO { get; set; }
            public string NUM_PLACA { get; set; }
            public int PERCENTUAL_DEVOLUCAO { get; set; }
            public object QTD_CAPACIDADE { get; set; }
            public int QTD_VIAGEM { get; set; }
            public int VALOR_BRUTO_DEVOLUCAO { get; set; }
            public int VALOR_BRUTO_VENDA { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista_Root
        {
            public List<FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista> DadosDISTLogDevolucaoMotorista { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista
        {
            public int CODIGO_MOTORISTA { get; set; }
            public string NOME_MOTORISTA { get; set; }
            public int PERCENTUAL_DEVOLUCAO { get; set; }
            public int VALOR_BRUTO_DEVOLUCAO { get; set; }
            public int VALOR_BRUTO_VENDA { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTLogDevolucaoCarro_Root
        {
            public List<FlagWSWhatsapp_DadosDISTLogDevolucaoCarro> DadosDISTLogDevolucaoCarro { get; set; }
            public int codigo { get; set; }
            public string mensagem { get; set; }
        }

        public static class FlagWSWhatsapp_DadosDISTLogDevolucaoCarro
        {
            public int CODIGO_VEICULO { get; set; }
            public string NUM_PLACA { get; set; }
            public int PERCENTUAL_DEVOLUCAO { get; set; }
            public int VALOR_BRUTO_DEVOLUCAO { get; set; }
            public int VALOR_BRUTO_VENDA { get; set; }
        }
    }
}