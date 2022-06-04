using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Integradores._Funcoes;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using static Integradores.clsEntidades;

namespace Integradores
{
    public class Integrador_Funcoes
    {
        public static string PastaImportacao;
        public static string PastaLokalizei;
        public static clsBancoDados oBancoDados;
        public static string ProductVersion;
        public static DateTime Processamento_Inicio;

        public static bool Processar(int iIdEmpresa, 
                                     int iIdTipoIntegracao,
                                     int iIdTarefa,
                                     string sNomeArquivo,
                                     string sTipoTarefa,
                                     string sStringConexaoOrigem,
                                     string sStringConexaoDestino,
                                     string sTpBancoDadosDestino,
                                     string sUsuario,
                                     string sSenha,
                                     string sCOD_PUXADA,
                                     string sTIPO_REGISTRO,
                                     string sTEL_CELULAR,
                                     string sTIPO_CONSULTA,
                                     string sVISAO_FATURAMENTO)
        {
            bool bOk = false;
            string sArquivo = "";
            int iEmpresa = 0;

            bOk = false;

            try
            {
                switch (FNC_NuloZero(iIdTipoIntegracao))
                {
                    case 0:
                        if (sNomeArquivo != "")
                        {
                            switch (sTipoTarefa)
                            {
                                case "csv.DadosInformados":
                                    bOk = rotImportaDadosDisInformados(iIdEmpresa,
                                                                       iIdTarefa,
                                                                       sNomeArquivo.ToString(),
                                                                       sStringConexaoDestino,
                                                                       sTpBancoDadosDestino);

                                    break;
                                case "pdf.CriaDatasEntrega":
                                    bOk = rotCriarDatasEntrega(iIdTarefa);

                                    break;
                                default:
                                    sArquivo = sNomeArquivo.ToString();

                                    if (sArquivo.Substring(1, 4).Trim() != "")
                                    {
                                        try
                                        {
                                            iEmpresa = Convert.ToInt32(sArquivo.Substring(0, 4));
                                        }
                                        catch (Exception)
                                        {

                                        }
                                    }

                                    string[] fileEntries = Directory.GetFiles(FNC_Diretorio_Tratar(PastaImportacao), sArquivo, SearchOption.AllDirectories);

                                    foreach (string fileName in fileEntries)
                                    {
                                        sArquivo = Path.GetFileName(fileName);

                                        switch (sTipoTarefa)
                                        {
                                            case "pdf.estoque":
                                                bOk = rotImportaEstoque(iEmpresa, iIdTarefa, sArquivo);

                                                break;
                                            case "pdf.preco":
                                                bOk = rotImportaPrecosPPBH(iEmpresa, iIdTarefa, sArquivo);

                                                break;
                                        }

                                        if (!bOk) break;
                                    }

                                    break;
                            }
                        }

                        break;
                    case 9:         //FlagWSWhatsapp_DisDevolucao
                        bOk = FlagWZ_DisDevolucao(iIdTarefa,
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
                        bOk = FlagWZ_DisTroca(iIdTarefa,
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
                        bOk = FlagWZ_DisIav_iv(iIdTarefa,
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
                        bOk = FlagWZ_DisInadimplencia(iIdTarefa,
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
                        bOk = FlagWZ_DisLogDevolucao(iIdTarefa,
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
                        bOk = FlagWZ_DisLog_Taxa_Ocupacao(iIdTarefa,
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
                        bOk = FlagWZ_DisLogDevolucaoMotorista(iIdTarefa,
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
                        bOk = DisLogDevolucaoCarro(iIdTarefa,
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
                        bOk = rotImportaDadosDisInformados(iEmpresa,
                                                           iIdTarefa,
                                                           sNomeArquivo.ToString(),
                                                           sStringConexaoDestino,
                                                           sTpBancoDadosDestino);

                        break;
                    case 19:        //DashPlus - DadosUsuario
                        bOk = FlagWZ_DisUsuario(iIdTarefa,
                                                sStringConexaoOrigem,
                                                sUsuario,
                                                sSenha,
                                                sCOD_PUXADA,
                                                sStringConexaoDestino,
                                                sTpBancoDadosDestino);

                        break;
                }

                if (bOk)
                {
                    DBSQL_Tarefas_AutoAgendar_Gravar(iIdTarefa);
                    Tarefa_Concluir(iIdTarefa);
                }
            }
            catch (Exception Ex)
            {
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroLeituraTarefas, 0, Ex.Message, null);
            }

            return bOk;
        }

        private static bool rotImportaDadosDisInformados(int iIdEmpresa, int iIdTarefa, string sArquivo, string sBancoDestino, string sTipoBanco)
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
                    Log_Registar(iIdEmpresa, LogTipo.ArquivoNaoEncontrado, iIdTarefa, "Arquivo " + sFile + " não localizado", sArquivo);
                    return false;
                }

                rotImportaCSV(iIdEmpresa, iIdTarefa, sFile, oBancoDados, "DadosInformados", new clsCampo[] {new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                                                                          new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String }});

                Log_Registar(iIdEmpresa, LogTipo.ArquivoImportadoComSucesso, iIdTarefa, "Importação Concluída", sArquivo);

                sDestino = sFile.Substring(0, sFile.Trim().Length - 3) + "pro";

                if (System.IO.File.Exists(sDestino))
                    System.IO.File.Delete(sDestino);

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Renomeado > " + sDestino);
                System.IO.File.Move(sFile, sDestino);

                return true;
            }
            catch (Exception Ex)
            {
                Log_Registar(iIdEmpresa, LogTipo.ErroNaRotina_ImportaEstoque, iIdTarefa, Ex.Message, sArquivo);
                return false;
            }
        }

        private static bool rotImportaCSV(int iIdEmpresa, int iIdTarefa, string sArquivo, clsBancoDados oBancoDestino, string sTabelaDestino, clsCampo[] CamposAdicionais)
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
                oBancoDados.DBSQL_Log_Gravar(iIdEmpresa, LogTipo.ErroNaRotina_ImportaPreco, iIdTarefa, Ex.Message, sArquivo);

                bOk = false;
            }

            return bOk;
        }

        private static bool rotCriarDatasEntrega(int iIdTarefa)
        {
            string vTemp;
            int intLinha;
            string sLinha;
            object vCampos;

            int iCampo;
            string sCampo;

            String[] sidCaracteristica = new String[200];

            int iCampoCodigo;
            bool bNovo;

            string sSql;
            DataTable rcoEmpresa;
            DataTable rcoRotas;
            DataTable rcoRotasEntrega;

            DateTime datCalculada;

            object iCodigo;

            int iErro;
            int iNovo;
            int iEdita;

            object vProduto;
            int iFaixa;

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
                    Log_Registar(0, LogTipo.ErroNoBancoDados, iIdTarefa, "Erro ao conectar banco de dados!", sRotina);
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
                        oBancoDados.DBSQL_Integrador_Log_Gravar(iIdEmpresa, iIdTarefa, "Removendo RotaEntrega " + oRow_Rotas["chave"]);
                        ////lblTarefa_Informacao.Text = "Importando " + oRow_Rotas["chave"];
                        ////Application.DoEvents();

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
                        oBancoDados.DBSQL_Integrador_Log_Gravar(iIdEmpresa, iIdTarefa, "Importando " + oRow_Rotas["RotaVenda"]);
                        //lblTarefa_Informacao.Text = "Importando " + oRow_Rotas["RotaVenda"];
                        ////Application.DoEvents();

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

                Log_Integrador_Registar(0, iIdTarefa, "Processamento finalizado - " + sRotina);

                return true;
            }
            catch (Exception Ex)
            {
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_ImportaPreco, iIdTarefa, Ex.Message, sRotina);

                return false;
            }
        }

        private static bool rotImportaPrecosPPBH(int iIdEmpresa,
                                                 int iIdTarefa,
                                                 String sArquivo)
        {
            int intLinha = 0;
            string[] vCampos;

            string sSql;
            string sDestino;

            string sFile;

            sFile = FNC_Diretorio_Tratar(PastaImportacao) + sArquivo;

            try
            {
                if (!oBancoDados.DBConectado())
                {
                    Log_Registar(iIdEmpresa, LogTipo.ErroNoBancoDados, iIdTarefa, "Erro ao conectar banco de dados!", sArquivo);
                    return false;
                }
                if (!System.IO.File.Exists(sFile))
                {
                    Log_Registar(iIdEmpresa, LogTipo.ErroNoBancoDados, iIdTarefa, "Não localizei o arquivo " + sArquivo, sArquivo);
                    return false;
                }

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Importando " + sArquivo);

                sSql = "delete from pdsuite.gertarefas_rotimportaprecosppbh" +
                       " where idTarefa = " + iIdTarefa +
                         " and idEmpresa = " + iIdEmpresa;
                oBancoDados.DBExecutar(sSql);

                string[] lines = System.IO.File.ReadAllLines(sFile);

                foreach (string sLinha in lines)
                {
                    try
                    {
                        intLinha = intLinha + 1;
                        //lblTarefa_Informacao.Text = "Importando " + intLinha;
                        //lblTarefa_Informacao.Refresh();
                        //Application.DoEvents();

                        //Busca linha
                        vCampos = sLinha.Split(new char[] { '|' });

                        sSql = "insert into gertarefas_rotimportaprecosppbh(idTarefa,idEmpresa,idTabelaPrecoFaixa,Codigo,PrecoMinimo," +
                                                                           "PrecoVenda,idFaixaInicialNova,idFaixaInicialFinalNova)" +
                               " values (" + iIdTarefa + ","
                                           + iIdEmpresa + ","
                                           + Convert.ToInt32(vCampos[6]) + ",'"
                                           + vCampos[0] + "',"
                                           + (Convert.ToDouble(vCampos[10]) / 100).ToString().Replace(",", ".") + ","
                                           + (Convert.ToDouble(vCampos[10]) / 100).ToString().Replace(",", ".") + ","
                                           + Convert.ToInt32(vCampos[8]) + ","
                                           + Convert.ToInt32(vCampos[9]) + ")";
                        oBancoDados.DBExecutar(sSql);
                    }
                    catch (Exception Ex)
                    {
                        //lblTarefa_Informacao.Text = "Importando " + intLinha + " - Erro " + Ex.Message;
                        oBancoDados.DBSQL_Log_Gravar(iIdEmpresa, LogTipo.ErroNaRotina_ImportaPreco, iIdTarefa, Ex.Message, sArquivo);
                    }
                }

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Executando procecure " + sArquivo);
                
                oBancoDados.DBProcedure("pdsuite.sp_integrador_rotImportaPrecosPPB", new clsCampo[] {
                                                                                        new clsCampo {Nome = "p_idEmpresa", Tipo = DbType.Double, Valor = iIdEmpresa},
                                                                                        new clsCampo {Nome = "p_idTarefa", Tipo = DbType.Double, Valor = iIdTarefa}});

                //finaliza log
                //lblTarefa_Informacao.Text = "Importação finalizada";

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Importação finalizada - " + sArquivo);

                sDestino = sFile.Substring(0, sFile.Trim().Length - 3) + "pro";

                if (System.IO.File.Exists(sDestino))
                    System.IO.File.Delete(sDestino);

                System.IO.File.Move(sFile, sDestino);

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Renomeado > " + sDestino);

                return true;
            }
            catch (Exception Ex)
            {
                oBancoDados.DBSQL_Log_Gravar(iIdEmpresa, LogTipo.ErroNaRotina_ImportaPreco, iIdTarefa, Ex.Message, sArquivo);

                return false;
            }
        }

        private bool rotImportaPrecosPPBHOld(int iIdEmpresa,
                                             int iIdTarefa,
                                             String sArquivo)
        {
            String vTemp;
            int intLinha;
            string[] vCampos;

            int iCampo;
            String sCampo;
            String[] sidCaracteristica = new String[200];
            int iCampoCodigo;

            double vFaixaInicialNova;
            double vFaixaInicialFinalNova;

            string sSql;
            string sSql_Principal;
            DataTable rcoProduto;
            DataTable rcoPreco;
            DataTable rcoCaracteristica;
            DataTable rcoPreco2;

            clsCampo rcoPreco_PrecoFaixaFinal;
            clsCampo rcoPreco_PrecoFaixaInicial;
            clsCampo rcoPreco_TextoFaixa;

            String sFile;

            object vProtocolo;

            int iCodigo;
            int iFaixas;
            int iFaixa1;
            int iFaixa2;
            int iFaixa3;

            int iErro;
            int iNovo;
            int iEdita;

            String sDestino;
            int vProduto;
            int iFaixa = 0;

            clsCampo[] oParametro = new clsCampo[15];

            sFile = FNC_Diretorio_Tratar(PastaImportacao) + sArquivo;

            try
            {
                if (!oBancoDados.DBConectado())
                {
                    Log_Registar(iIdEmpresa, LogTipo.ErroNoBancoDados, iIdTarefa, "Erro ao conectar banco de dados!", sArquivo);
                    return false;
                }
                if (!System.IO.File.Exists(sFile))
                {
                    Log_Registar(iIdEmpresa, LogTipo.ErroNoBancoDados, iIdTarefa, "Não localizei o arquivo " + sArquivo, sArquivo);
                    return false;
                }

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Importando " + sArquivo);

                // inicia laco de busca de caracteristicas
                intLinha = -1;
                iCampoCodigo = -1;
                vProduto = -1;

                //Gera Procoloco
                vProtocolo = String.Format("{0:yyyyMMddHHmmss}", DateTime.Now);

                //Zerando tabela de precos nao alteradas
                sSql = "update tb_tabela_precos_produtos" +
                       " set TextoFaixa=nroFaixa" +
                           ",PrecoVenda=0" +
                           ",PrecoExibicao='0.00'" +
                           ",PrecoVendaUnitario=0" +
                           ",PrecoExibicaoUnitario = '0.00'" +
                       " where idTabelaPreco = 4";
                //' and idProtocolo<>" & vProtocolo
                oBancoDados.DBExecutar(sSql);

                string[] lines = System.IO.File.ReadAllLines(sFile);

                foreach (string sLinha in lines)
                {
                    try
                    {
                        intLinha = intLinha + 1;

                        //lblTarefa_Informacao.Text = "Importando " + intLinha;
                        //Application.DoEvents();

                        //Busca linha
                        vCampos = sLinha.Split(new char[] { '|' });

                        //Verifica faixa
                        if (vProduto != Convert.ToInt32(vCampos[0]))
                        {
                            iFaixa = 1;
                        }
                        else
                        {
                            iFaixa = iFaixa + 1;
                        }

                        iFaixa = Convert.ToInt32(vCampos[6]);

                        //Adiciona campo no produto
                        sSql = "SELECT * FROM tb_produtos WHERE idEmpresa= " + iIdEmpresa + " and Codigo='" + Convert.ToInt32(vCampos[0]) + "'";
                        rcoProduto = oBancoDados.DBQuery(sSql);

                        if (!FNC_Data_Vazio(rcoProduto))
                        {
                            sSql_Principal = "select count(*) from tb_tabela_precos_produtos" +
                                             " where idTabelaPreco=4 and idProduto=" + rcoProduto.Rows[0]["idProduto"].ToString() +
                                               " and nroFaixa=" + iFaixa +
                                               " and idUnidadeMedida=1";                        //fixo por enquanto
                            if (Convert.ToInt32(oBancoDados.DBQuery_ValorUnico(sSql_Principal)) == 0)
                            {
                                sSql_Principal = "insert into tb_tabela_precos_produtos (idProtocolo,idTabelaPrecoFaixa,AgrupadorPreco,PrecoCusto,PrecoMinimo," +
                                                                                        "PrecoVenda,PrecoExibicao,PrecoVendaUnitario,PrecoExibicaoUnitario," +
                                                                                        "ValorPromocional,PrecoFaixaInicial,PrecoFaixaFinal,StatusPreco," +
                                                                                        "idTabelaPreco,idProduto,nroFaixa,idUnidadeMedida)" +
                                                 " values (?idProtocolo,?idTabelaPrecoFaixa,?AgrupadorPreco,0,?PrecoMinimo," +
                                                          "?PrecoVenda,?PrecoExibicao,?PrecoVendaUnitario,?PrecoExibicaoUnitario," +
                                                          "0,?PrecoFaixaInicial,?PrecoFaixaFinal,1," +
                                                          "?idTabelaPreco,?idProduto,?nroFaixa,?idUnidadeMedida)";
                            }
                            else
                            {
                                sSql_Principal = "update tb_tabela_precos_produtos" +
                                                 " set idProtocolo=?idProtocolo," +
                                                      "idTabelaPrecoFaixa=?idTabelaPrecoFaixa," +
                                                      "AgrupadorPreco=?AgrupadorPreco," +
                                                      "PrecoCusto=0," +
                                                      "PrecoMinimo=?PrecoMinimo," +
                                                      "PrecoVenda=?PrecoVenda," +
                                                      "PrecoExibicao=?PrecoExibicao," +
                                                      "PrecoVendaUnitario=?PrecoVendaUnitario," +
                                                      "PrecoExibicaoUnitario=PrecoExibicaoUnitario," +
                                                      "ValorPromocional=0," +
                                                      "PrecoFaixaInicial=?PrecoFaixaInicial," +
                                                      "PrecoFaixaFinal=?PrecoFaixaFinal," +
                                                      "TextoFaixa=?TextoFaixa," +
                                                      "StatusPreco=1" +
                                                 " where idTabelaPreco=?idTabelaPreco" +
                                                   " and idProduto=?idProduto" +
                                                   " and nroFaixa=?nroFaixa" +
                                                   " and idUnidadeMedida=?idUnidadeMedida";
                            }

                            oParametro[0] = new clsCampo { Nome = "idProtocolo", Tipo = DbType.String, Valor = vProtocolo };
                            oParametro[1] = new clsCampo { Nome = "idTabelaPrecoFaixa", Tipo = DbType.Int16, Valor = vCampos[6] };
                            oParametro[2] = new clsCampo { Nome = "AgrupadorPreco", Tipo = DbType.String, Valor = rcoProduto.Rows[0]["idProduto"] };
                            oParametro[3] = new clsCampo { Nome = "PrecoMinimo", Tipo = DbType.Double, Valor = (Convert.ToDouble(vCampos[10]) / 100) };
                            oParametro[4] = new clsCampo { Nome = "PrecoVenda", Tipo = DbType.Double, Valor = (Convert.ToDouble(vCampos[10]) / 100) };
                            oParametro[5] = new clsCampo { Nome = "PrecoExibicao", Tipo = DbType.String, Valor = Convert.ToDouble(oParametro[4].Valor).ToString("0.00").Replace(".", ",") };
                            oParametro[6] = new clsCampo { Nome = "PrecoVendaUnitario", Tipo = DbType.Double, Valor = Math.Round(Convert.ToDouble(oParametro[4].Valor) / Convert.ToDouble(rcoProduto.Rows[0]["fator_venda"]), 2) };
                            oParametro[7] = new clsCampo { Nome = "PrecoExibicaoUnitario", Tipo = DbType.String, Valor = Convert.ToDouble(oParametro[6].Valor).ToString("0.00").Replace(".", ",") + "/un" };

                            //Ajuste de Faixa
                            vFaixaInicialNova = Convert.ToInt32(vCampos[8]);

                            switch (iFaixa)
                            {
                                case 0:
                                case 1:
                                    {
                                        if (vFaixaInicialNova < 1) vFaixaInicialNova = 1;
                                        break;
                                    }
                                case 2:
                                    {
                                        if (vFaixaInicialNova < 2) vFaixaInicialNova = 2;
                                        break;
                                    }
                                case 3:
                                    {
                                        if (vFaixaInicialNova < 3) vFaixaInicialNova = 3;
                                        break;
                                    }
                            }

                            vFaixaInicialFinalNova = Convert.ToInt32(vCampos[9]);

                            if (vFaixaInicialNova > vFaixaInicialFinalNova) vFaixaInicialFinalNova = vFaixaInicialNova;

                            oParametro[8] = new clsCampo { Nome = "PrecoFaixaInicial", Tipo = DbType.Double, Valor = vFaixaInicialNova };
                            oParametro[9] = new clsCampo { Nome = "PrecoFaixaFinal", Tipo = DbType.Double, Valor = vFaixaInicialNova };

                            switch (iFaixa)
                            {
                                case 1:
                                    {
                                        if (vFaixaInicialNova == vFaixaInicialFinalNova)
                                        {
                                            oParametro[10] = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = vFaixaInicialNova };
                                        }
                                        else
                                        {
                                            oParametro[10] = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = vFaixaInicialNova.ToString().Trim() + " a " + vFaixaInicialFinalNova.ToString().Trim() };
                                        }

                                        break;
                                    }
                                case 2:
                                    {
                                        if (vFaixaInicialNova == vFaixaInicialFinalNova)
                                        {
                                            oParametro[10] = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = vFaixaInicialNova };
                                        }
                                        else
                                        {
                                            oParametro[10] = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = vFaixaInicialNova.ToString().Trim() + " a " + vFaixaInicialFinalNova.ToString().Trim() };
                                        }

                                        break;
                                    }
                                case 3:
                                    {
                                        oParametro[10] = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = vFaixaInicialNova.ToString().Trim() + " ou mais" };

                                        break;
                                    }
                                default:
                                    {
                                        oParametro[10] = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = vFaixaInicialNova.ToString().Trim() + " ou mais" };

                                        break;
                                    }
                            }

                            //Atualiza produto
                            if (iFaixa == 1)
                            {
                                sSql = "update tb_produtos" +
                                       " set Preco1Venda = ?Preco1Venda," +
                                            "Preco2VendaUnitario = ?Preco2VendaUnitario," +
                                            "PrecoVendaUnitarioExib=?PrecoVendaUnitarioExib," +
                                            "Custo=0," +
                                            "CustoUnitario=0" +
                                       " where idEmpresa= " + iIdEmpresa +
                                         " and Codigo='" + Convert.ToInt32(vCampos[0]) + "'";
                                oBancoDados.DBExecutar(sSql, new clsCampo[] {new clsCampo {Nome = "Preco1Venda", Tipo = DbType.Double, Valor = (Convert.ToDouble(vCampos[10]) / 100)},
                                                                                           new clsCampo {Nome = "PrecoVendaUnitario", Tipo = DbType.Double, Valor = Math.Round(Convert.ToDouble(oParametro[4].Valor) / Convert.ToDouble(rcoProduto.Rows[0]["fator_venda"]), 2)},
                                                                                           new clsCampo {Nome = "PrecoVendaUnitarioExib", Tipo = DbType.String, Valor = Math.Round(Convert.ToDouble(oParametro[4].Valor) / Convert.ToDouble(rcoProduto.Rows[0]["fator_venda"]), 2).ToString("0.00").Replace(".", ",")}});
                            }

                            oParametro[11] = new clsCampo { Nome = "idTabelaPreco", Tipo = DbType.Int16, Valor = 4 };
                            oParametro[12] = new clsCampo { Nome = "idProduto", Tipo = DbType.Int16, Valor = rcoProduto.Rows[0]["idProduto"] };
                            oParametro[13] = new clsCampo { Nome = "nroFaixa", Tipo = DbType.Int16, Valor = iFaixa };
                            oParametro[14] = new clsCampo { Nome = "idUnidadeMedida", Tipo = DbType.Int16, Valor = 1 };
                            oBancoDados.DBExecutar(sSql_Principal, oParametro);

                            proximoproduto:
                            //Seta produto
                            vProduto = Convert.ToInt32(vCampos[0]);
                        }

                        //Ajusta faixas
                        sSql = "select tb_produtos.*" +
                               " from tb_produtos " +
                                " left join tb_categorias1 on tb_categorias1.idCategoria1 = tb_produtos.idCategoria1" +
                                " left join tb_status ontb_status.idStatus = tb_produtos.idStatus" +
                               " where tb_produtos.idEmpresa=5" +
                                 " and tb_categorias1.Categoria1Ativa = 1" +
                                 " and tb_status.PermitidoVender = 1";
                        rcoProduto = oBancoDados.DBQuery(sSql);

                        foreach (DataRow oRow_Produto in rcoProduto.Rows)
                        {
                            //lblTarefa_Informacao.Text = "Ajustando Preco " + oRow_Produto["idProduto"];
                            //Application.DoEvents();

                            vFaixaInicialNova = 0;
                            iFaixas = 0;
                            iFaixa1 = 0;
                            iFaixa2 = 0;
                            iFaixa3 = 0;

                            //Busca Precos Escalonados
                            sSql = "select * from tb_tabela_precos_produtos" +
                                   " where idTabelaPreco=4" +
                                     " and idProduto=" + oRow_Produto["idProduto"] +
                                   " order by nroFaixa desc";
                            rcoPreco = oBancoDados.DBQuery(sSql);

                            foreach (DataRow oRow_Preco in rcoPreco.Rows)
                            {
                                if (Convert.ToInt32(oRow_Preco["nroFaixa"]) == 1)
                                {
                                    iFaixa1 = Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]);

                                    //Busca Precos Escalonados
                                    sSql = "select * from tb_tabela_precos_produtos" +
                                           " where idTabelaPreco=4" +
                                             " and nroFaixa=2" +
                                             " and idProduto=" + oRow_Produto["idProduto"] +
                                           " order by nroFaixa";
                                    rcoPreco2 = oBancoDados.DBQuery(sSql);

                                    if (!FNC_Data_Vazio(rcoPreco2))
                                    {
                                        if ((Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]) >= Convert.ToInt32(rcoPreco2.Rows[0]["PrecoFaixaInicial"])) ||
                                            (Convert.ToInt32(rcoPreco2.Rows[0]["PrecoFaixaInicial"]) > 1) ||
                                            (Convert.ToInt32(rcoPreco2.Rows[0]["PrecoFaixaInicial"]) - Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]) > 1))
                                        {
                                            rcoPreco_PrecoFaixaFinal = new clsCampo { Nome = "PrecoFaixaFinal", Tipo = DbType.Double, Valor = Convert.ToInt32(rcoPreco2.Rows[0]["PrecoFaixaInicial"]) - 1 };
                                            rcoPreco_PrecoFaixaInicial = new clsCampo { Nome = "PrecoFaixaInicial", Tipo = DbType.Double, Valor = Convert.ToInt32(rcoPreco2.Rows[0]["PrecoFaixaInicial"]) };

                                            if (Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]) < 1)
                                            {
                                                rcoPreco_PrecoFaixaFinal = new clsCampo { Nome = "PrecoFaixaFinal", Tipo = DbType.Double, Valor = 1 };
                                            }

                                            if (Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]) > Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]))
                                            {
                                                rcoPreco_PrecoFaixaInicial = new clsCampo { Nome = "PrecoFaixaInicial", Tipo = DbType.Double, Valor = 1 };
                                            }

                                            if (Convert.ToInt32(rcoPreco_PrecoFaixaInicial.Valor) > 1)
                                            {
                                                rcoPreco_PrecoFaixaInicial = new clsCampo { Nome = "PrecoFaixaInicial", Tipo = DbType.Double, Valor = 1 };
                                            }

                                            if (Convert.ToInt32(rcoPreco_PrecoFaixaInicial.Valor) == Convert.ToInt32(rcoPreco_PrecoFaixaFinal.Valor))
                                            {
                                                rcoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.Double, Valor = rcoPreco_PrecoFaixaInicial.Valor };
                                            }
                                            else
                                                rcoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.Double, Valor = rcoPreco_PrecoFaixaInicial.Valor };
                                            {
                                                rcoPreco_TextoFaixa = new clsCampo
                                                {
                                                    Nome = "TextoFaixa",
                                                    Tipo = DbType.String,
                                                    Valor = rcoPreco_PrecoFaixaInicial.Valor.ToString().Trim() + " a " + rcoPreco_PrecoFaixaFinal.Valor.ToString().Trim()
                                                };
                                            }

                                            sSql = "update tb_tabela_precos_produtos" +
                                                   " set PrecoFaixaFinal=?PrecoFaixaFinal," +
                                                        "PrecoFaixaInicial=?PrecoFaixaInicial," +
                                                        "TextoFaixa=?TextoFaixa" +
                                                   " where idTabelaPrecoProduto=" + oRow_Preco["idTabelaPrecoProduto"];
                                            oBancoDados.DBExecutar(sSql, new clsCampo[] {rcoPreco_PrecoFaixaFinal,
                                                                                                   rcoPreco_PrecoFaixaInicial,
                                                                                                   rcoPreco_TextoFaixa});

                                        }
                                    }
                                }

                                if (Convert.ToInt32(oRow_Preco["nroFaixa"]) == 2)
                                {
                                    iFaixa2 = Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]);

                                    //Busca Precos Escalonados
                                    sSql = "select * from tb_tabela_precos_produtos" +
                                           " where idTabelaPreco=4" +
                                             " and nroFaixa=3" +
                                             " and idProduto=" + oRow_Produto["idProduto"] +
                                           " Order by nroFaixa";
                                    rcoPreco2 = oBancoDados.DBQuery(sSql);

                                    if (FNC_Data_Vazio(rcoPreco2))
                                    {
                                        if (Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]) >= Convert.ToInt32(rcoPreco2.Rows[0]["PrecoFaixaInicial"]) ||
                                            Convert.ToInt32(rcoPreco2.Rows[0]["PrecoFaixaInicial"]) - Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]) > 1)
                                        {
                                            rcoPreco_PrecoFaixaFinal = new clsCampo { Nome = "PrecoFaixaFinal", Tipo = DbType.Double, Valor = Convert.ToInt32(rcoPreco2.Rows[0]["PrecoFaixaInicial"]) - 1 };

                                            if (Convert.ToInt32(rcoPreco_PrecoFaixaFinal.Valor) < 1)
                                            {
                                                rcoPreco_PrecoFaixaFinal = new clsCampo { Nome = "PrecoFaixaFinal", Tipo = DbType.Double, Valor = 1 };
                                            }

                                            if (Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]) > Convert.ToInt32(rcoPreco_PrecoFaixaFinal.Valor))
                                            {
                                                rcoPreco_PrecoFaixaInicial = new clsCampo { Nome = "PrecoFaixaInicial", Tipo = DbType.Double, Valor = Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]) };
                                            }
                                            else
                                            {
                                                rcoPreco_PrecoFaixaInicial = new clsCampo { Nome = "PrecoFaixaInicial", Tipo = DbType.Double, Valor = Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]) };
                                            }

                                            if (Convert.ToInt32(rcoPreco_PrecoFaixaInicial.Valor) == Convert.ToInt32(rcoPreco_PrecoFaixaFinal.Valor))
                                            {
                                                rcoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]) };
                                            }
                                            else
                                            {
                                                rcoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = oRow_Preco["PrecoFaixaInicial"].ToString().Trim() + " a " + oRow_Preco["PrecoFaixaFinal"].ToString().Trim() };
                                            }

                                            sSql = "update tb_tabela_precos_produtos" +
                                                    " set PrecoFaixaFinal=?PrecoFaixaFinal," +
                                                        "PrecoFaixaInicial=?PrecoFaixaInicial," +
                                                        "TextoFaixa=?TextoFaixa" +
                                                    " where idTabelaPrecoProduto=" + oRow_Preco["idTabelaPrecoProduto"];
                                            oBancoDados.DBExecutar(sSql, new clsCampo[] {rcoPreco_PrecoFaixaFinal,
                                                                                                   rcoPreco_PrecoFaixaInicial,
                                                                                                   rcoPreco_TextoFaixa});
                                        }
                                    }
                                }


                                if (Convert.ToInt32(oRow_Preco["nroFaixa"]) == 3)
                                {
                                    iFaixa3 = Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]);

                                    if (Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]) < Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]))
                                    {
                                        rcoPreco_PrecoFaixaFinal = new clsCampo { Nome = "PrecoFaixaFinal", Tipo = DbType.Double, Valor = Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]) - 1 };

                                        if (Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]) == Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]))
                                        {
                                            rcoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = oRow_Preco["PrecoFaixaInicial"].ToString().Trim() + " ou mais" };
                                        }
                                        else
                                        {
                                            rcoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = oRow_Preco["PrecoFaixaInicial"].ToString().Trim() + " a " + oRow_Preco["PrecoFaixaFinal"].ToString().Trim() };
                                        }

                                        sSql = "update tb_tabela_precos_produtos" +
                                                " set PrecoFaixaFinal=?PrecoFaixaFinal," +
                                                        "TextoFaixa=?TextoFaixa" +
                                                " where idTabelaPrecoProduto=" + oRow_Preco["idTabelaPrecoProduto"];
                                        oBancoDados.DBExecutar(sSql, new clsCampo[] {rcoPreco_PrecoFaixaFinal,
                                                                                               rcoPreco_TextoFaixa});
                                    }
                                    else if (Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]) == Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]))
                                    {
                                        if (Convert.ToInt32(oRow_Preco["PrecoFaixaInicial"]) == Convert.ToInt32(oRow_Preco["PrecoFaixaFinal"]))
                                        {
                                            rcoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = oRow_Preco["PrecoFaixaInicial"].ToString().Trim() + " ou mais" };
                                        }
                                        else
                                        {
                                            rcoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = oRow_Preco["PrecoFaixaInicial"].ToString().Trim() + " a " + oRow_Preco["PrecoFaixaFinal"].ToString().Trim() };
                                        }

                                        rcoPreco_TextoFaixa = new clsCampo { Nome = "TextoFaixa", Tipo = DbType.String, Valor = "99999999" };

                                        sSql = "update tb_tabela_precos_produtos" +
                                                " set TextoFaixa=?TextoFaixa" +
                                                " where idTabelaPrecoProduto=" + oRow_Preco["idTabelaPrecoProduto"];
                                        oBancoDados.DBExecutar(sSql, new clsCampo[] { rcoPreco_TextoFaixa });
                                    }
                                }

                                //                      'Salvando api
                                //                      sSql = "SELECT"
                                //                      sSql = sSql & vbCr & "tb_tabela_precos_produtos.idTabelaPrecoProduto as chave"
                                //                      sSql = sSql & vbCr & ",cast(tb_tabela_precos_produtos.idTabelaPrecoProduto as char) as idTabelaPrecoProduto"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.idTabelaPreco"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.idTabelaPreco as idTabelaPrecoFaixa"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.idUnidadeMedida"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.idProduto"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoCusto"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoMinimo"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoVenda"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoVendaUnitario"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.ValorPromocional"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoFaixaInicial"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.nroFaixa"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoFaixaFinal"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.AgrupadorPreco"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.TextoFaixa"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoExibicao"
                                //                      sSql = sSql & vbCr & ",tb_tabela_precos_produtos.PrecoExibicaoUnitario"
                                //                      sSql = sSql & vbCr & "From tb_tabela_precos_produtos"
                                //                      sSql = sSql & vbCr & "Where tb_tabela_precos_produtos.idTabelaPreco = 4"
                                //                      sSql = sSql & vbCr & "and tb_tabela_precos_produtos.idProduto = " & rcoPreco!idProduto
                                //                      sSql = sSql & vbCr & "and tb_tabela_precos_produtos.nroFaixa = " & rcoPreco!nroFaixa
                                //                     ' Set rcoPreco2 = New ADODB.Recordset
                                //                     ' rcoPreco2.Open sSql, Conexao, adOpenKeyset, adLockOptimistic


                                //                      'Atualizando APP
                                //                   '   sSql = "http://fb.pedidodireto.com/api.php?table=tabelaprecoprodutos&op=edit&"
                                //                   '   sSql = sSql & "&AgrupadorPreco=" & rcoPreco2!AgrupadorPreco
                                //                   '   sSql = sSql & "&PrecoCusto=" & rcoPreco2!PrecoCusto
                                //                   '   sSql = sSql & "&PrecoExibicao=" & rcoPreco2!PrecoExibicao
                                //                   '   sSql = sSql & "&PrecoExibicaoUnitario=" & rcoPreco2!PrecoExibicaoUnitario
                                //                   '   sSql = sSql & "&PrecoFaixaFinal=" & rcoPreco2!PrecoFaixaFinal
                                //                   '   sSql = sSql & "&PrecoFaixaInicial=" & rcoPreco2!PrecoFaixaInicial
                                //                   '   sSql = sSql & "&PrecoMinimo=" & rcoPreco2!PrecoMinimo
                                //                   '   sSql = sSql & "&PrecoVenda=" & rcoPreco2!PrecoVenda
                                //                   '   sSql = sSql & "&PrecoVendaUnitario=" & rcoPreco2!PrecoVendaUnitario
                                //                   '   sSql = sSql & "&TextoFaixa=" & rcoPreco2!TextoFaixa
                                //                   '   sSql = sSql & "&ValorPromocional=" & rcoPreco2!ValorPromocional
                                //                   '   sSql = sSql & "&idProduto=" & rcoPreco2!idProduto
                                //                   '   sSql = sSql & "&idTabelaPreco=" & rcoPreco2!idTabelaPreco
                                //                   '   sSql = sSql & "&idTabelaPrecoFaixa=" & rcoPreco2!idTabelaPrecoFaixa
                                //                   '   sSql = sSql & "&idTabelaPrecoProduto=" & rcoPreco2!idTabelaPrecoProduto
                                //                   '   sSql = sSql & "&idUnidadeMedida=" & rcoPreco2!idUnidadeMedida
                                //                   '   sSql = sSql & "&nroFaixa=" & rcoPreco2!nroFaixa
                                //                   '   sSql = sSql & "&id=" & rcoPreco2!chave


                                //                      'sSql = Inet1.OpenURL(sSql)



                                //                      'rcoPreco2.Close


                                //                     'proximo produto
                                //                     rcoPreco.MoveNext
                            }

                            //               'Ajustando produtos
                            //               sSql = "SELECT"
                            //               sSql = sSql & vbCr & "concat(prod.idEmpresa, '-', prod.idProduto) As Chave"
                            //               sSql = sSql & vbCr & ",ifnull(tb_categorias1.Categoria1,'A definir') as Categoria1"
                            //               sSql = sSql & vbCr & ",ifnull(tb_categorias2.Categoria2,'A definir') as Categoria2"
                            //               sSql = sSql & vbCr & ",ifnull(prod.Descricao,'Falta Descricao') as Descricao"
                            //               sSql = sSql & vbCr & ",ifnull(prod.Descricao_Detalhada,'Falta Detalhada') as Descricao_Detalhada"
                            //               sSql = sSql & vbCr & ",ifnull(prod.Estoque,0) as Estoque"
                            //               sSql = sSql & vbCr & ",ifnull(prod.ImagemNormal,'padrao.png') as ImagemNormal"
                            //               sSql = sSql & vbCr & ",ifnull(prod.ImagemMobile,'padrao.png') as ImagemMobile"
                            //               sSql = sSql & vbCr & ",ifnull(prod.Preco1Venda,0) as MenorPreco"
                            //               sSql = sSql & vbCr & ",ifnull(prod.Nome_Amigavel,'Falta Nome') as Nome_Amigavel"
                            //               sSql = sSql & vbCr & ",ifnull(tb_categorias2.OrdemCategoria2,'Falta OC2') as OrdemCategoria2"
                            //               sSql = sSql & vbCr & ",ifnull(prod.OrdemExibicao,'Falta Ord') as OrdemProduto"
                            //               sSql = sSql & vbCr & ",ifnull(prod.Preco1Venda,0) as Preco1Venda"
                            //               sSql = sSql & vbCr & ",ifnull(prod.Quantidade_na_embalagem,1) as Quantidade_na_embalagem"
                            //               sSql = sSql & vbCr & ",ifnull(prod.Quantidade_Exibicao,'1') as Quantidade_Exibicao"
                            //               sSql = sSql & vbCr & ",ifnull(tb_unidademedida.UnidadeMedida,'embalagem') as Embalagem"
                            //               sSql = sSql & vbCr & ",ifnull(tb_status.Status,'nao vender') as Status"
                            //               sSql = sSql & vbCr & ",ifnull(tb_status.PermitidoVender,0) as StatusVender"
                            //               sSql = sSql & vbCr & ",ifnull(tb_status.idStatus,4) as idStatus"
                            //               sSql = sSql & vbCr & ",ifnull(prod.SKU,prod.idProduto) as SKU"
                            //               sSql = sSql & vbCr & ",ifnull(prod.idCategoria1,0) as idCategoria1"
                            //               sSql = sSql & vbCr & ",ifnull(prod.idCategoria2,0) as idCategoria2"
                            //               sSql = sSql & vbCr & ",ifnull(prod.idEmpresa,0) as idEmpresa"
                            //               sSql = sSql & vbCr & ",prod.idProduto"
                            //               sSql = sSql & vbCr & "From"
                            //               sSql = sSql & vbCr & "tb_produtos prod"
                            //               sSql = sSql & vbCr & "LEFT JOIN tb_categorias1 ON tb_categorias1.idCategoria1 = prod.idCategoria1"
                            //               sSql = sSql & vbCr & "LEFT JOIN tb_categorias2 ON tb_categorias2.idCategoria2 = prod.idCategoria2"
                            //               sSql = sSql & vbCr & "LEFT JOIN tb_categorias3 ON tb_categorias3.idCategoria3 = prod.idCategoria3"
                            //               sSql = sSql & vbCr & "LEFT JOIN tb_embalagem ON tb_embalagem.idEmbalagem = prod.idEmbalagem"
                            //               sSql = sSql & vbCr & "LEFT JOIN tb_status    ON tb_status.idStatus = prod.idStatus"
                            //               sSql = sSql & vbCr & "LEFT JOIN tb_unidademedida    ON tb_unidademedida.idUnidadeMedida = prod.idUnidadeMedidaVenda"
                            //               sSql = sSql & vbCr & "Where tb_categorias1.Categoria1Ativa = 1 And tb_status.PermitidoVender = 1"
                            //               sSql = sSql & vbCr & "and idProduto=" & rcoProduto!idProduto

                            //                Set rcoPreco2 = New ADODB.Recordset
                            //               ' rcoPreco2.Open sSql, Conexao, adOpenKeyset, adLockOptimistic


                            //               ' If Not rcoPreco2.EOF Then
                            //               '    sSql = "http://fb.pedidodireto.com/api.php?table=products&op=edit&"
                            //               '    sSql = sSql & "&Categoria1=" & rcoPreco2!Categoria1
                            //               '    sSql = sSql & "&Categoria2=" & rcoPreco2!Categoria2
                            //               '    sSql = sSql & "&Descricao=" & rcoPreco2!Descricao
                            //               '    sSql = sSql & "&Descricao_Detalhada=" & rcoPreco2!Descricao_Detalhada
                            //               '    sSql = sSql & "&Embalagem=" & rcoPreco2!Embalagem
                            //               '    sSql = sSql & "&Estoque=" & rcoPreco2!Estoque
                            //               '    sSql = sSql & "&ImagemMobile=" & rcoPreco2!ImagemMobile
                            //               '   sSql = sSql & "&ImagemNormal=" & rcoPreco2!ImagemNormal
                            //               '    sSql = sSql & "&MenorPreco=" & rcoPreco2!MenorPreco
                            //               '    sSql = sSql & "&Nome_Amigavel=" & rcoPreco2!Nome_Amigavel
                            //               '    sSql = sSql & "&OrdemCategoria2=" & rcoPreco2!OrdemCategoria2
                            //               '    sSql = sSql & "&OrdemProduto=" & rcoPreco2!OrdemProduto
                            //               '    sSql = sSql & "&Preco1Venda=" & rcoPreco2!Preco1Venda
                            //               '    sSql = sSql & "&Quantidade_Exibicao=" & rcoPreco2!Quantidade_Exibicao
                            //               '    sSql = sSql & "&Quantidade_na_embalagem=" & rcoPreco2!Quantidade_na_embalagem
                            //               '    sSql = sSql & "&SKU=" & rcoPreco2!SKU
                            //               '    sSql = sSql & "&Status=" & rcoPreco2!Status
                            //               '    sSql = sSql & "&StatusVender=" & rcoPreco2!StatusVender
                            //               '    sSql = sSql & "&idCategoria1=" & rcoPreco2!idCategoria1
                            //               '    sSql = sSql & "&idCategoria2=" & rcoPreco2!idCategoria2
                            //               '    sSql = sSql & "&idEmpresa=" & rcoPreco2!idEmpresa
                            //               '    sSql = sSql & "&idProduto=" & rcoPreco2!idProduto
                            //               '    sSql = sSql & "&idStatus=" & rcoPreco2!idStatus
                            //               '    sSql = sSql & "&id=" & rcoPreco2!chave


                            //                   'sSql = Inet1.OpenURL(sSql)
                            //               'End If


                            //               rcoProduto.MoveNext
                        }

                        sSql = "select concat(prod.idEmpresa, '-', prod.idProduto) As Chave" +
                               " from tb_produtos" +
                                " left join tb_categorias1 on tb_categorias1.idCategoria1 = tb_produtos.idCategoria1" +
                                " left join tb_status on tb_status.idStatus = tb_produtos.idStatus" +
                                " where tb_produtos.idEmpresa=" + iIdEmpresa +
                                  " and tb_status.PermitidoVender = 0";
                        rcoProduto = oBancoDados.DBQuery(sSql);

                        foreach (DataRow oRow_Produto in rcoProduto.Rows)
                        {
                            //lblTarefa_Informacao.Text = "Removendo Produtos que nao vende mais " + oRow_Produto["idProduto"];
                            //Application.DoEvents();

                            //sSql = "http://fb.pedidodireto.com/api.php?table=products&op=delete&id=" & rcoProduto!chave
                            //'sSql = Inet1.OpenURL(sSql)
                        }
                    }
                    catch (Exception Ex)
                    {
                        intLinha = intLinha;
                    }
                }

                //finaliza log

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Importação finalizada - " + sArquivo);
                //Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Novos Registros " + iNovo.ToString());
                //Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Registros Editados  " + iEdita);
                //Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Erros Registros " + iErro);

                sDestino = sFile.Substring(0, sFile.Trim().Length - 3) + "pro";

                if (System.IO.File.Exists(sDestino))
                    System.IO.File.Delete(sDestino);

                System.IO.File.Move(sFile, sDestino);

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Renomeado > " + sDestino);

                return true;
            }
            catch (Exception Ex)
            {
                oBancoDados.DBSQL_Log_Gravar(iIdEmpresa, LogTipo.ErroNaRotina_ImportaPreco, iIdTarefa, Ex.Message, sArquivo);

                return false;
            }
        }

        private static bool rotImportaEstoque(int iIdEmpresa, int iIdTarefa, string sArquivo)
        {
            int intLinha;
            string sLinha;
            string[] vCampos;

            String[] sidCaracteristica = new String[10];

            string sSql;
            string sFile;

            int iErro = 0;
            int iNovo = 0;
            int iEdita = 0;

            DataTable oData;

            string vProtocolo;
            string sDestino;
            object vProduto;

            try
            {
                sFile = FNC_Diretorio_Tratar(PastaImportacao) + sArquivo;

                if (!oBancoDados.DBConectado())
                {
                    Log_Registar(iIdEmpresa, LogTipo.ErroNoBancoDados, iIdTarefa, "Banco de dados não conectado", sArquivo);
                    return false;
                }

                if (!System.IO.File.Exists(sFile))
                {
                    Log_Registar(iIdEmpresa, LogTipo.ArquivoNaoEncontrado, iIdTarefa, "Arquivo " + sFile + " não localizado", sArquivo);
                    return false;
                }

                // Gerando protocolo
                vProtocolo = String.Format("{0:yyyyMMddHHmmss}", DateTime.Now);

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Importando " + PastaLokalizei);

                // inicia leitura dos cabecalhos
                intLinha = -1;
                vProduto = -1;

                string[] lines = System.IO.File.ReadAllLines(sFile);

                foreach (string line in lines)
                {
                    sLinha = line.ToString();
                    sLinha = sLinha.Trim().Replace(";", "");
                    vCampos = sLinha.Split(new char[] { ',' });

                    intLinha = intLinha + 1;
                    //lblTarefa_Informacao.Text = "Importando " + intLinha + " " + Convert.ToDouble(vCampos[0]);

                    //Application.DoEvents();

                    //Adiciona campo no produto
                    sSql = "SELECT * FROM tb_produtos WHERE idEmpresa=" + iIdEmpresa.ToString() + " and codigo='" + Convert.ToInt32(vCampos[0]).ToString() + "'";
                    oData = oBancoDados.DBQuery(sSql);

                    if (oData.Rows.Count != 0)
                    {
                        sSql = "update tb_produtos set Estoque = ?Estoque  WHERE idEmpresa=" + iIdEmpresa.ToString() + " and codigo='" + Convert.ToInt32(vCampos[0]).ToString() + "'";
                        oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "Estoque", Valor = Convert.ToDouble(vCampos[1]), Tipo = DbType.Double } });

                        // Atualizando tabela do estoque
                        sSql = "INSERT INTO tb_estoque (idEstoque, idEmpresa, idProduto, idProtocolo, Estoque) values (" +
                                                        oData.Rows[0]["idProduto"] + "," +
                                                        oData.Rows[0]["idEmpresa"] + "," +
                                                        oData.Rows[0]["idProduto"] + ",0," +
                                                        vProtocolo + ") ON DUPLICATE KEY UPDATE " +
                                "idEmpresa = " + oData.Rows[0]["idEmpresa"] + "," +
                                "idProduto=" + oData.Rows[0]["idProduto"] + "," +
                                "idProtocolo=" + vProtocolo;
                        oBancoDados.DBExecutar(sSql);

                        // Limpa Tabela de Baixas
                        sSql = "Delete from tb_estoque_baixa" +
                               " Where idEmpresa= " + oData.Rows[0]["idEmpresa"] +
                                 " and idProduto=" + oData.Rows[0]["idProduto"];
                        oBancoDados.DBExecutar(sSql);

                        sSql = "INSERT INTO tb_estoque_baixa (idEstoque, idEmpresa, idTipoMovimento, idProduto, idlstPedidoItem, EstoqueBaixa)" +
                               " values (" + oData.Rows[0]["idProduto"] + "," +
                                             oData.Rows[0]["idEmpresa"] + ", 1, " +
                                             oData.Rows[0]["idProduto"] + ", 0, " +
                                             System.Convert.ToDouble(vCampos[1]) + ")";
                        oBancoDados.DBExecutar(sSql);

                        // =edit&&=string&idEmpresa=string&Estoque=string&id=XXXXX

                        // Atualizando APP
                        sSql = "http://fb.pedidodireto.com/api.php?table=Stock&op=edit&";
                        sSql = sSql + "&idProduto=" + oData.Rows[0]["idProduto"];
                        sSql = sSql + "&idEmpresa=" + oData.Rows[0]["idEmpresa"];
                        sSql = sSql + "&Estoque=" + System.Convert.ToDouble(vCampos[1]);
                        sSql = sSql + "&id=" + oData.Rows[0]["idEmpresa"] + "-" + oData.Rows[0]["idProduto"];
                        //System.Diagnostics.Process.Start(sSql);
                    }
                    else
                        Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Produto estoque nao encontrado " + System.Convert.ToDouble(vCampos[0]));

                    oData = null;
                }

                sSql = "select tb_estoque.idProduto," +
                              "tb_estoque.idEmpresa" +
                       " from tb_estoque" +
                       " where (idEmpresa==" + iIdEmpresa.ToString() + " and " +
                               "idProtocolo<>" + vProtocolo + ") or " +
                              "(idEmpresa=" + iIdEmpresa.ToString() + " and " +
                               "idProduto in (select tb_Produtos.idProduto from tb_Produtos where idEmpresa=" + iIdEmpresa.ToString() + " and idStatus=2))";
                oData = oBancoDados.DBQuery(sSql);

                foreach (DataRow oRow in oData.Rows)
                {
                    intLinha = intLinha + 1;
                    //lblTarefa_Informacao.Text = "Limpando " + oRow["idProduto"];
                    //Application.DoEvents();

                    // Limpa Tabela de Baixas
                    sSql = "Delete from tb_estoque_baixa" +
                           " Where idEmpresa= " + oRow["idEmpresa"] +
                             " and idProduto=" + oRow["idProduto"] +
                             " and idTipoMovimento=1";
                    oBancoDados.DBExecutar(sSql);

                    // Atualizando APP
                    sSql = "http://fb.pedidodireto.com/api.php?table=Stock&op=edit&" +
                           "&idProduto=" + oRow["idProduto"] +
                           "&idEmpresa=" + oRow["idEmpresa"] +
                           "&Estoque=0" +
                           "&id=" + oRow["idEmpresa"] + " - " + oRow["idProduto"];
                    //System.Diagnostics.Process.Start(sSql);
                }

                // Log
                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Importação finalizada");
                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Novos Registros " + iNovo);
                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Registros Editados  " + iEdita);
                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Erros Registros " + iErro);

                Log_Registar(iIdEmpresa, LogTipo.ArquivoImportadoComSucesso, iIdTarefa, "Importação Concluída", sArquivo);

                sDestino = sFile.Substring(0, sFile.Trim().Length - 3) + "pro";

                if (System.IO.File.Exists(sDestino))
                    System.IO.File.Delete(sDestino);

                Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Renomeado > " + sDestino);
                System.IO.File.Move(sFile, sDestino);

                return true;
            }
            catch (Exception Ex)
            {
                Log_Registar(iIdEmpresa, LogTipo.ErroNaRotina_ImportaEstoque, iIdTarefa, Ex.Message, sArquivo);
                return false;
            }
        }

        private static Boolean FlagWZ_DisUsuario(int iIdTarefa,
                                            string sConexao,
                                            string sUsuario,
                                            string sSenha,
                                            string sCOD_PUXADA,
                                            string sDS_STRINGCONEXAODESTINO,
                                            string sTP_BANCODADOSDESTINO)
        {
            bool bOk = false;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                FlagWSWhatsapp_DadosUsuario_Root DadosUsuario_Root;
                string sSql;

                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"}";

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
                            DadosUsuario_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosUsuario_Root>(content);
                        }
                    }
                }

                if (DadosUsuario_Root != null)
                {
                    if (DadosUsuario_Root.DadosUsuario.Count > 0)
                    {
                        clsBancoDados oBancoDados = new clsBancoDados();

                        oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

                        foreach (FlagWSWhatsapp_DadosUsuario DadosUsuario in DadosUsuario_Root.DadosUsuario)
                        {
                            sSql = "SELECT COUNT(*) FROM tb_Usuario" +
                                   " WHERE CNPJ='" + DadosUsuario.CNPJ.Trim() + "'" +
                                     " AND ID_USUARIO=" + DadosUsuario.ID_USUARIO.ToString().Trim();
                            if (Convert.ToInt32(oBancoDados.DBQuery_ValorUnico(sSql)) == 0)
                            {
                                sSql = "INSERT INTO tb_Usuario (Cod_Puxada,Nome,Senha,Departamento,Email,Ativo" +
                                                              ",CNPJ,Cod_Empregado,FLEXXPOWER,LICENCA,SOFIA,ID_USUARIO,TELEFONE,DTINTEGRACAO,VERSAO_INTEGRADOR)" +
                                       " VALUES(#Cod_Puxada,#Nome,#Senha,#Departamento,#Email,#Ativo" +
                                              ",#CNPJ,#Cod_Empregado,#FLEXXPOWER,#LICENCA,#SOFIA,#ID_USUARIO,#TELEFONE,#DTINTEGRACAO,#VERSAO_INTEGRADOR)";
                                oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "Nome", Valor = DadosUsuario.USUARIO.Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "Senha", Valor = DadosUsuario.SENHA.Trim(), Tipo = DbType.String },
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

                                if (DadosUsuario.SOFIA.Trim().ToUpper().Substring(0, 1) == "S")
                                {
                                    Bot.Webook_Util(1, Constantes.const_TratamentoSofia_BEMVINDO, 2, DadosUsuario.USUARIO.Trim(), DadosUsuario.TELEFONE.Trim(), "sofia");
                                }
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
                                            "Cod_Empregado=#Cod_Empregado," +
                                            "FLEXXPOWER=#FLEXXPOWER," +
                                            "LICENCA=#LICENCA," +
                                            "SOFIA=#SOFIA," +
                                            "TELEFONE=#TELEFONE," +
                                            "DTINTEGRACAO=#DTINTEGRACAO," +
                                            "VERSAO_INTEGRADOR=#VERSAO_INTEGRADOR" +
                                       " WHERE CNPJ=#CNPJ" +
                                         " AND ID_USUARIO=#ID_USUARIO";
                                oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "Nome", Valor = DadosUsuario.USUARIO.Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "Senha", Valor = DadosUsuario.SENHA.Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "Departamento", Valor = DadosUsuario.DEPARTAMENTO.Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "Email", Valor = DadosUsuario.EMAIL.Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "Ativo", Valor = Convert.ToInt32(DadosUsuario.ATIVO.Trim()), Tipo = DbType.Int32 },
                                                                              new clsCampo { Nome = "Cod_Empregado", Valor = DadosUsuario.EMPREGADO.Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "FLEXXPOWER", Valor = DadosUsuario.FLEXXPOWER.Trim().ToUpper().Substring(0, 1), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "LICENCA", Valor = DadosUsuario.LICENCA.Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "SOFIA", Valor = DadosUsuario.SOFIA.Trim().ToUpper().Substring(0, 1), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "DTINTEGRACAO", Valor = Processamento_Inicio, Tipo = DbType.DateTime },
                                                                              new clsCampo { Nome = "VERSAO_INTEGRADOR", Valor = ProductVersion, Tipo = DbType.String },
                                                                              new clsCampo { Nome = "CNPJ", Valor = DadosUsuario.CNPJ.Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "TELEFONE", Valor = DadosUsuario.TELEFONE.Trim(), Tipo = DbType.String },
                                                                              new clsCampo { Nome = "ID_USUARIO", Valor = DadosUsuario.ID_USUARIO, Tipo = DbType.Int32 }});
                            }
                        }

                        oBancoDados.DBDesconectar();

                        bOk = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_FlagWSWhatsapp_DisDevolucao, iIdTarefa, Ex.Message, "FlagWSWhatsapp_DisDevolucao");
                bOk = false;
            }

            return bOk;
        }

        private static Boolean FlagWZ_DisDevolucao(int iIdTarefa,
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

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                FlagWSWhatsapp_DisDevolucao_Root DisDevolucao_Root;
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
                            DisDevolucao_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DisDevolucao_Root>(content);
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
                                                                                        new clsCampo { Nome = "PERCENTUAL_DEVOLUCAO", Valor = Convert.ToDecimal(DisDevolucao_Dados.PERCENTUAL_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_DEVOLUCAO", Valor = Convert.ToDecimal(DisDevolucao_Dados.VALOR_BRUTO_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = Convert.ToDecimal(DisDevolucao_Dados.VALOR_BRUTO_VENDA) / 100, Tipo = DbType.Decimal },
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
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_FlagWSWhatsapp_DisDevolucao, iIdTarefa, Ex.Message, "FlagWSWhatsapp_DisDevolucao");
                bOk = false;
            }

            return bOk;
        }

        private static Boolean FlagWZ_DisTroca(int iIdTarefa,
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

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                FlagWSWhatsapp_DadosDISTTroca_Root DisTroca_Root;
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
                            DisTroca_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTTroca_Root>(content);
                        }
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
                                                                                        new clsCampo { Nome = "PERCENTUAL_TROCA", Valor = Convert.ToDecimal(DisTroca_Dados.PERCENTUAL_TROCA) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_TROCA", Valor = Convert.ToDecimal(DisTroca_Dados.VALOR_BRUTO_TROCA) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = Convert.ToDecimal(DisTroca_Dados.VALOR_BRUTO_VENDA) / 100, Tipo = DbType.Decimal },
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
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_FlagWSWhatsapp_DisTroca, iIdTarefa, Ex.Message, "FlagWSWhatsapp_DisTroca");
                bOk = false;
            }

            return bOk;
        }

        private static Boolean FlagWZ_DisIav_iv(int iIdTarefa,
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

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                FlagWSWhatsapp_DadosDISTIavIv_Root DisTIav_iv_Root;
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
                            DisTIav_iv_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTIavIv_Root>(content);
                        }
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
                                                                                        new clsCampo { Nome = "PERCENTUAL_VISITA_REALIZADA", Valor = Convert.ToDecimal(DisIav_iv_Dados.PERCENTUAL_VISITA_REALIZADA) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "PERCENTUAL_VISITA_REALIZADA_VENDA", Valor = Convert.ToDecimal(DisIav_iv_Dados.PERCENTUAL_VISITA_REALIZADA_VENDA) / 100, Tipo = DbType.Decimal },
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
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_FlagWSWhatsapp_DisTroca, iIdTarefa, Ex.Message, "FlagWSWhatsapp_DisTroca");
                bOk = false;
            }

            return bOk;
        }

        private static Boolean FlagWZ_DisInadimplencia(int iIdTarefa,
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

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                FlagWSWhatsapp_DadosDISTInadimplencia_Root DisInadimplencia_Root;
                string sSql;

                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sUsuario + ":" + sSenha)));
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string json = "{\"COD_PUXADA\":\"" + sCOD_PUXADA + "\"," +
                               "\"TIPO_REGISTRO\":\"" + sTIPO_REGISTRO + "\"," +
                               "\"TEL_CELULAR\":\"" + sTEL_CELULAR + "\"," +
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
                                                                                        new clsCampo { Nome = "PERCENTUAL_INADIMPLENCIA", Valor = Convert.ToDecimal(DisInadimplencia.PERCENTUAL_INADIMPLENCIA) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = Convert.ToDecimal(DisInadimplencia.VALOR_BRUTO_VENDA) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_INADIMPLENCIA", Valor = Convert.ToDecimal(DisInadimplencia.VALOR_INADIMPLENCIA) / 100, Tipo = DbType.Decimal },
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
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_FlagWSWhatsapp_DisTroca, iIdTarefa, Ex.Message, "FlagWSWhatsapp_DisTroca");
                bOk = false;
            }

            return bOk;
        }

        private static Boolean FlagWZ_DisLogDevolucao(int iIdTarefa,
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

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                FlagWSWhatsapp_DadosDISTLogDevolucao_Root DisLogDevolucao_Root;
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
                            DisLogDevolucao_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTLogDevolucao_Root>(content);
                        }
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
                                     " AND CODIGO_MOTORISTA=#CODIGO_MOTORISTA" +
                                     " AND CODIGO_VEICULO=#CODIGO_VEICULO";
                            oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_MOTORISTA", Valor = LogDevolucao.CODIGO_MOTORISTA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_VEICULO", Valor = LogDevolucao.CODIGO_VEICULO, Tipo = DbType.String }});

                            sSql = "INSERT INTO DadosDISTLogDevolucao (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
                                                                      "CODIGO_MOTORISTA, CODIGO_VEICULO, NOME_MOTORISTA, NUM_PLACA, PERCENTUAL_DEVOLUCAO," +
                                                                      "VALOR_BRUTO_DEVOLUCAO, VALOR_BRUTO_VENDA, DTINTEGRACAO, VERSAO_INTEGRADOR)" +
                                   " VALUES (#COD_PUXADA, #TIPO_REGISTRO, #TEL_CELULAR, #TIPO_CONSULTA, #VISAO_FATURAMENTO," +
                                            "#CODIGO_MOTORISTA, #CODIGO_VEICULO, #NOME_MOTORISTA, #NUM_PLACA, #PERCENTUAL_DEVOLUCAO," +
                                            "#VALOR_BRUTO_DEVOLUCAO, #VALOR_BRUTO_VENDA, #DTINTEGRACAO, #VERSAO_INTEGRADOR)";
                            oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TIPO_REGISTRO", Valor = sTIPO_REGISTRO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TEL_CELULAR", Valor = sTEL_CELULAR, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "TIPO_CONSULTA", Valor = sTIPO_CONSULTA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "VISAO_FATURAMENTO", Valor = sVISAO_FATURAMENTO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_MOTORISTA", Valor = LogDevolucao.CODIGO_MOTORISTA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_VEICULO", Valor = LogDevolucao.CODIGO_VEICULO, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "NOME_MOTORISTA", Valor = LogDevolucao.NOME_MOTORISTA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "NUM_PLACA", Valor = LogDevolucao.NUM_PLACA, Tipo = DbType.String },
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
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_FlagWSWhatsapp_DisTroca, iIdTarefa, Ex.Message, "FlagWSWhatsapp_DisTroca");
                bOk = false;
            }

            return bOk;
        }

        private static Boolean FlagWZ_DisLog_Taxa_Ocupacao(int iIdTarefa,
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

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao_Root DisTLog_Taxa_Ocupacao_Root;
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
                            DisTLog_Taxa_Ocupacao_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao_Root>(content);
                        }
                    }
                }

                if (DisTLog_Taxa_Ocupacao_Root != null)
                {
                    if (DisTLog_Taxa_Ocupacao_Root.DadosDISTLog_Taxa_Ocupacao.Count > 0)
                    {
                        clsBancoDados oBancoDados = new clsBancoDados();

                        oBancoDados.DBConectar(sTP_BANCODADOSDESTINO, sDS_STRINGCONEXAODESTINO);

                        foreach (FlagWSWhatsapp_DadosDISTLog_Taxa_Ocupacao LogDespersaoRota in DisTLog_Taxa_Ocupacao_Root.DadosDISTLog_Taxa_Ocupacao)
                        {
                            sSql = "DELETE FROM DadosDISTLog_Despersao_rota" +
                                   " WHERE COD_PUXADA=#COD_PUXADA" +
                                     " AND CODIGO_VEICULO=#CODIGO_VEICULO";
                            oBancoDados.DBExecutar(sSql, new clsCampo[] { new clsCampo { Nome = "COD_PUXADA", Valor = sCOD_PUXADA, Tipo = DbType.String },
                                                                                        new clsCampo { Nome = "CODIGO_VEICULO", Valor = LogDespersaoRota.CODIGO_VEICULO, Tipo = DbType.String }});

                            sSql = "INSERT INTO DadosDISTLog_Despersao_rota (COD_PUXADA, TIPO_REGISTRO, TEL_CELULAR, TIPO_CONSULTA, VISAO_FATURAMENTO," +
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
                                                                                        new clsCampo { Nome = "PERCENTUAL_DEVOLUCAO", Valor = Convert.ToDecimal(LogDespersaoRota.PERCENTUAL_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "QTD_CAPACIDADE", Valor = Convert.ToDecimal(LogDespersaoRota.QTD_CAPACIDADE) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "QTD_VIAGEM", Valor = Convert.ToDecimal(LogDespersaoRota.QTD_VIAGEM) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_DEVOLUCAO", Valor = Convert.ToDecimal(LogDespersaoRota.VALOR_BRUTO_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = Convert.ToDecimal(LogDespersaoRota.VALOR_BRUTO_VENDA) / 100, Tipo = DbType.Decimal },
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
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_FlagWSWhatsapp_DisTroca, iIdTarefa, Ex.Message, "FlagWSWhatsapp_DisTroca");
                bOk = false;
            }

            return bOk;
        }

        private static Boolean FlagWZ_DisLogDevolucaoMotorista(int iIdTarefa,
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

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sConexao);
                FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista_Root DisTLogDevolucaoMotorista_Root;
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
                            DisTLogDevolucaoMotorista_Root = JsonConvert.DeserializeObject<FlagWSWhatsapp_DadosDISTLogDevolucaoMotorista_Root>(content);
                        }
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
                                                                                        new clsCampo { Nome = "PERCENTUAL_DEVOLUCAO", Valor = Convert.ToDecimal(LogDevolucaoMotorista.PERCENTUAL_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_DEVOLUCAO", Valor = Convert.ToDecimal(LogDevolucaoMotorista.VALOR_BRUTO_DEVOLUCAO) / 100, Tipo = DbType.Decimal },
                                                                                        new clsCampo { Nome = "VALOR_BRUTO_VENDA", Valor = Convert.ToDecimal(LogDevolucaoMotorista.VALOR_BRUTO_VENDA) / 100, Tipo = DbType.Decimal },
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
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_FlagWSWhatsapp_DisTroca, iIdTarefa, Ex.Message, "FlagWSWhatsapp_DisTroca");
                bOk = false;
            }

            return bOk;
        }

        private static Boolean DisLogDevolucaoCarro(int iIdTarefa,
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
                               "\"TEL_CELULAR\":\"" + sTEL_CELULAR + "\"," +
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
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNaRotina_FlagWSWhatsapp_DisTroca, iIdTarefa, Ex.Message, "FlagWSWhatsapp_DisTroca");
                bOk = false;
            }

            return bOk;
        }

        private bool rotImportaPrecosSql(int iIdEmpresa,
                                         int iIdTarefa,
                                         string sFile)
        {
            string vTemp;
            int intLinha;
            string[] vCampos;

            int iCampo;
            string sCampo;

            String[] sidCaracteristica = new String[200];

            int iCampoCodigo;
            double vFaixaInicialNova;

            string sSql;
            string sSql_Campo = "";
            DataTable rcoProduto;
            DataTable rcoPreco;
            DataTable rcoCaracteristica;

            double vFaixaInicialFinalNova;

            int iCodigo;

            int iErro;
            int iNovo;
            int iEdita;

            string sDestino;
            double vProduto;
            int iFaixa = 0;

            System.IO.StreamWriter sw;

            // On Local Error GoTo AdoError
            if (!oBancoDados.DBConectado())
            {
                Log_Registar(iIdEmpresa, LogTipo.ErroNoBancoDados, iIdTarefa, "Erro ao conectar banco de dados!", sFile);
                return false;
            }

            if (!System.IO.File.Exists(sFile))
            {
                Log_Registar(iIdEmpresa, LogTipo.ErroNoBancoDados, iIdTarefa, "Não localizei o arquivo " + sFile, sFile);
                return false;
            }

            Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Importando " + sFile);

            // inicia laco de busca de caracteristicas
            intLinha = -1;
            iCampoCodigo = -1;
            vProduto = -1;

            if (System.IO.File.Exists("c:\\comandopreco.sql"))
            {
                sw = System.IO.File.AppendText("c:\\comandopreco.sql");
            }
            else
            {
                sw = System.IO.File.CreateText("c:\\comandopreco.sql");
            }

            string[] lines = System.IO.File.ReadAllLines(sFile);

            foreach (string sLinha in lines)
            {
                intLinha = intLinha + 1;

                //lblTarefa_Informacao.Text = "Importando " + intLinha;
                //Application.DoEvents();

                vCampos = sLinha.Split(new char[] { '|' });

                // Verifica faixa
                if (vProduto != Convert.ToDouble(vCampos[0]))
                    iFaixa = 1;
                else
                    iFaixa = iFaixa + 1;

                iFaixa = Convert.ToInt32(vCampos[6]);

                sSql = "select * from tb_produtos where idEmpresa = " + iIdEmpresa + " and Codigo= '" + vCampos[0] + "'";
                rcoProduto = oBancoDados.DBQuery(sSql);

                vFaixaInicialNova = Convert.ToDouble(vCampos[8]);

                switch (iFaixa)
                {
                    case 0:
                    case 1:
                        {
                            if (vFaixaInicialNova < 1)
                                vFaixaInicialNova = 1;

                            sSql_Campo = sSql_Campo + Environment.NewLine + ", TextoFaixa='" + vFaixaInicialNova.ToString().Trim() + "'";
                            break;
                        }
                    case 2:
                        {
                            if (vFaixaInicialNova < 2)
                                vFaixaInicialNova = 2;

                            sSql_Campo = sSql_Campo + Environment.NewLine + ", TextoFaixa='" + vFaixaInicialNova.ToString().Trim() + "'";
                            break;
                        }

                    case 3:
                        {
                            if (vFaixaInicialNova < 3)
                                vFaixaInicialNova = 3;
                            sSql_Campo = sSql_Campo + Environment.NewLine + ", TextoFaixa='" + vFaixaInicialNova.ToString().Trim() + " ou mais'";
                            break;
                        }
                }

                vFaixaInicialFinalNova = Convert.ToDouble(vCampos[9]);
                if (vFaixaInicialNova > vFaixaInicialFinalNova)
                    vFaixaInicialFinalNova = vFaixaInicialNova;

                sSql = "update tpp" + Environment.NewLine +
                       " set PrecoCusto = 0" + Environment.NewLine +
                           ",PrecoMinimo = " + (Convert.ToDouble(vCampos[10]) / 100).ToString("0.00") + Environment.NewLine +
                           ",PrecoVenda = " + (Convert.ToDouble(vCampos[10]) / 100).ToString("0.00") + Environment.NewLine +
                           ",PrecoExibicao = '" + (Convert.ToDouble(vCampos[10]) / 100).ToString("0.00").Replace(".", ",") + "'" + Environment.NewLine +
                           ",PrecoVendaUnitario = round(" + (Convert.ToDouble(vCampos[10]) / 100).ToString("0.00") + "/ prd.Quantidade_na_embalagem, 2)" + Environment.NewLine +
                           ",PrecoExibicaoUnitario = round(" + (Convert.ToDouble(vCampos[1]) / 100).ToString("0.00") + " / tb_produtos.Quantidade_na_embalagem, 2)" + Environment.NewLine +
                           ",ValorPromocional = 0" + Environment.NewLine +
                           sSql_Campo + Environment.NewLine +
                           ",PrecoFaixaInicial = " + vFaixaInicialNova + Environment.NewLine +
                           ",PrecoFaixaFinal = " + vFaixaInicialFinalNova + Environment.NewLine +
                       " from tb_tabela_precos_produtos tpp" + Environment.NewLine +
                        " left join tb_produtos prd on prd.idProduto = tpp.idProduto" + Environment.NewLine +
                       " where tpp.idTabelaPreco = 4 and nroFaixa=" + iFaixa + Environment.NewLine +
                         " and tpp.idProduto = prd.idProduto" + Environment.NewLine +
                         " and prd.sku='PP-" + vCampos[0].ToString() + "'" + Environment.NewLine + Environment.NewLine;
                sw.WriteLine(sSql);

                //// Busca Precos Escalonados
                //sSql = "SELECT * FROM tb_tabela_precos_produtos ";
                //sSql = sSql + Constants.vbCr + "WHERE idTabelaPreco=4 and idProduto=" + rcoProduto.idProduto;
                //sSql = sSql + Constants.vbCr + "and nroFaixa=" + iFaixa;
                //sSql = sSql + Constants.vbCr + "and idUnidadeMedida=1";
                //rcoPreco.Open(sSql, Conexao, adOpenKeyset, adLockOptimistic);
                //if (rcoPreco.EOF)
                //{
                //    rcoPreco.AddNew();
                //    rcoPreco.idTabelaPreco = 4;
                //    rcoPreco.idProduto = rcoProduto.idProduto;
                //    rcoPreco.idUnidadeMedida = 1;
                //}

                //rcoPreco.nroFaixa = iFaixa;
                //rcoPreco.idTabelaPrecoFaixa = Val(vCampos(6));
                //rcoPreco.AgrupadorPreco = Trim(Str(rcoProduto.idProduto));
                //rcoPreco.PrecoCusto = 0;
                //rcoPreco.PrecoMinimo = System.Convert.ToDouble(vCampos(10)) / 100;
                //rcoPreco.PrecoVenda = System.Convert.ToDouble(vCampos(10)) / 100;
                //rcoPreco.PrecoExibicao = Replace(Format(rcoPreco.PrecoVenda, "0.00"), ".", ",");
                //rcoPreco.PrecoVendaUnitario = Round(rcoPreco.PrecoVenda / (double)rcoProduto.Quantidade_na_embalagem, 2);
                //rcoPreco.PrecoExibicaoUnitario = Replace(Format(rcoPreco.PrecoVendaUnitario, "0.00"), ".", ",") + "/un";
                //rcoPreco.ValorPromocional = 0;
                //rcoPreco.PrecoFaixaInicial = Val(Val(vCampos(8)));
                //rcoPreco.PrecoFaixaFinal = Val(Val(vCampos(9)));
                //switch (iFaixa)
                //{
                //    case 1:
                //        {
                //            if (Val(Val(vCampos(8))) == Val(Val(vCampos(9))))
                //                rcoPreco.TextoFaixa = Trim(Str(Val(Val(vCampos(8)))));
                //            else
                //                rcoPreco.TextoFaixa = Trim(Str(Val(Val(vCampos(8))))) + " a " + Trim(Str(Val(Val(vCampos(9)))));
                //            break;
                //        }

                //    case 2:
                //        {
                //            if (Val(Val(vCampos(8))) == Val(Val(vCampos(9))))
                //                rcoPreco.TextoFaixa = Trim(Str(Val(Val(vCampos(8)))));
                //            else
                //                rcoPreco.TextoFaixa = Trim(Str(Val(Val(vCampos(8))))) + " a " + Trim(Str(Val(Val(vCampos(9)))));
                //            break;
                //        }

                //    case 3:
                //        {
                //            rcoPreco.TextoFaixa = Trim(Str(Val(Val(vCampos(8))))) + " ou mais";
                //            break;
                //        }

                //    default:
                //        {
                //            rcoPreco.TextoFaixa = Trim(Str(Val(Val(vCampos(8)))));
                //            break;
                //        }
                //}

                //rcoPreco.StatusPreco = 1;

                //// vUltimaQuantidade = rcoPreco!PrecoFaixaFinal

                //// Atualiza produto
                //if (iFaixa == 1)
                //{
                //    rcoProduto.Preco1Venda = rcoPreco.PrecoVenda;
                //    rcoProduto.Preco2VendaUnitario = rcoPreco.PrecoVendaUnitario;
                //    rcoProduto.PrecoVendaUnitarioExib = Replace(Format(rcoPreco.PrecoVendaUnitario, "0.00"), ".", ",");
                //    rcoProduto.Custo = 0;
                //    rcoProduto.CustoUnitario = 0;
                //    rcoProduto.Update();
                //}

                //rcoPreco.Update();
                //rcoPreco.Close();
                //// End If



                //rcoProduto.Close();
                //proximoproduto:
                //;

                // Seta produto
                vProduto = Convert.ToDouble(vCampos[0]);
            }

            sw.Close();

            Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Importação finalizada - " + sFile);
            //Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Novos Registros " + iNovo.ToString());
            //Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Registros Editados  " + iEdita);
            //Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Erros Registros " + iErro);

            sDestino = sFile.Substring(0, sFile.Trim().Length - 3) + "pro";

            if (System.IO.File.Exists(sDestino))
                System.IO.File.Delete(sDestino);

            Log_Integrador_Registar(iIdEmpresa, iIdTarefa, "Renomeado > " + sDestino);

            System.IO.File.Move(sFile, sDestino);

            Log_Registar(iIdEmpresa, LogTipo.ArquivoImportadoComSucesso, iIdTarefa, "Importação Concluída", sFile);

            return true;

            // cmdImportar.Enabled = True

            //AdoError:
            //;
            //ADODB.Error errLoop;
            //string strError;
            //string sErro;
            //int I;


            //// If Err = 3705 And bConectando Then
            //// Set mConexaoDb2 = New ADODB.Connection
            //// Resume Inicio
            //// End If

            //// If Err = 3705 Then Resume Next

            //// mbErroConexao = True


            //sErro = "";
            //sErro = sErro + "Ocorreu algum erro durante a importação do arquivo.";


            //if (Information.Err.Number != 0)
            //{
            //    sErro = sErro + Constants.vbNewLine + String(80, "-");
            //    sErro = sErro + Constants.vbNewLine + "Error # " + Str(Information.Err.Number);
            //    sErro = sErro + Constants.vbNewLine + "Gerado por " + Information.Err.Source;
            //    sErro = sErro + Constants.vbNewLine + "Descrição  " + Information.Err.Description;
            //    sErro = sErro + Constants.vbNewLine + String(80, "-");
            //}

            //Interaction.MsgBox(sErro, Constants.vbCritical + Constants.vbOKOnly, "mrSoft");
        }

        private static void Log_Integrador_Registar(int iEmpresa,
                                                    int iIdTarefa,
                                                    string sLog)
        {
            oBancoDados.DBSQL_Integrador_Log_Gravar(iEmpresa, iIdTarefa, sLog);
        }

        private static void Log_Registar(int iEmpresa,
                                         LogTipo Tipo,
                                         int iIdTarefa,
                                         string sLog,
                                         string sNomeArquivo)
        {
            oBancoDados.DBSQL_Log_Gravar(iEmpresa, Tipo, iIdTarefa, sLog, sNomeArquivo);
        }

        private static Boolean DBSQL_Tarefas_AutoAgendar_Gravar(int iId_Tarefa)
        {
            Boolean bOk = false;

            try
            {
                oBancoDados.DBProcedure("pdsuite.sp_gertarefas_autoagendar_cad", new clsCampo[] {
                                                                                     new clsCampo {Nome = "p_idTarefa", Tipo = DbType.Double, Valor = iId_Tarefa}});

                bOk = true;
            }
            catch (Exception Ex)
            {
                oBancoDados.DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, iId_Tarefa, Ex.Message, "");
            }

            return bOk;
        }

        private static void Tarefa_Concluir(int iId_Tarefa)
        {
            string sSql = "update gertarefas set TarefaConcluida = 'S', DataExecucao = now() where idTarefa = " + iId_Tarefa.ToString();

            oBancoDados.DBExecutar(sSql);
        }

    }
}