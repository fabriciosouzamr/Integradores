using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Data.Common;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Integradores;
using static Integradores._Funcoes;
using static BancoDados;

public static class Funcoes
{
    public enum Mensasagem_Tipo
    {
        Erro = 1,
        Informacao = 2
    }

    public static void FNC_Mensagem(string sMensagem, Mensasagem_Tipo eMensasagem_Tipo)
    {
        switch (eMensasagem_Tipo)
        {
            case Mensasagem_Tipo.Erro:
                MessageBox.Show(sMensagem, Constantes.const_Sistema_Nome, MessageBoxButtons.OK, MessageBoxIcon.Error);
                break;
            case Mensasagem_Tipo.Informacao:
                MessageBox.Show(sMensagem, Constantes.const_Sistema_Nome, MessageBoxButtons.OK, MessageBoxIcon.Information);
                break;
            default:
                MessageBox.Show(sMensagem, Constantes.const_Sistema_Nome);
                break;
        }
    }

    public static bool FNC_Perguntar(string sMensagem)
    {
        return (MessageBox.Show(sMensagem, Constantes.const_Sistema_Nome, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
    }

    public static void DBComboBox_Carregar(clsBancoDados BancoDados, ComboBox oCombo, string sSqlText)
    {
        DataTable oData;

        oData = BancoDados.DBQuery(sSqlText);

        oCombo.DataSource = oData;
        oCombo.DisplayMember = "DS";
        oCombo.ValueMember = "ID";
    }

    public static object DBComboBox_Tratar(DropDownList oCombo)
    {
        object oRet = null;

        if (oCombo.SelectedIndex > -1)
        {
            if (oCombo.SelectedValue.Trim() != "")
            {
                if (Convert.ToInt32(oCombo.SelectedValue) != 0)
                {
                    oRet = Convert.ToInt32(oCombo.SelectedValue);
                }
            }
        }

        return oRet;
    }
}

//        Private Function rotImportaPrecosPPBH(sFile As String) As Boolean

//        Dim vTemp As String
//        Dim intLinha As Integer
//        Dim sLinha As String
//        Dim vCampos As Variant


//        Dim iCampo As Integer
//        Dim sCampo As String
//        Dim sidCaracteristica(0 To 200) As String
//        Dim iCampoCodigo As Integer

//        Dim vFaixaInicialNova As Variant
//        Dim vFaixaInicialFinalNova As Variant

//        Dim sSql As String
//        Dim rcoProduto As ADODB.Recordset
//        Dim rcoPreco As ADODB.Recordset
//        Dim rcoCaracteristica As ADODB.Recordset
//        Dim rcoPreco2 As ADODB.Recordset
//        Dim vProtocolo As Variant

//        Dim iCodigo As Variant
//        Dim iFaixas As Integer
//        Dim iFaixa1 As Variant
//        Dim iFaixa2 As Variant
//        Dim iFaixa3 As Variant

//        Dim iErro As Integer
//        Dim iNovo As Integer
//        Dim iEdita As Integer

//        Dim sDestino As String
//        Dim vProduto As Variant
//        Dim iFaixa As Integer

//        'On Local Error GoTo AdoError

//       ' If Not ConectarDB Then
//        '   MsgBox "Erro ao conectar banco de dados!", vbCritical + vbOKOnly, "mrSoft"
//        '   Exit Function
//        'End If
//        ' OpenConn(Conexao, "bd2.pedidodireto.com", "pdsuite", "root", "9pJz8rF-GWtS", 3306)
//        If Not ConectarDB Then
//           MsgBox "Erro ao conectar banco de dados!", vbCritical + vbOKOnly, "mrSoft"
//           Exit Function
//        End If


//        If Dir(txtArquivo) = "" Then
//           MsgBox "Não localizei o arquivo " & txtArquivo.Text, vbCritical + vbOKOnly, "mrSoft"
//           Exit Function
//        End If




//        cmdImportar.Enabled = False


//        Log "Importando " & txtArquivo

//        'inicia leitura dos cabecalhos
//        Open txtArquivo For Input As #1

//        'inicia laco de busca de caracteristicas
//        intLinha = -1
//        iCampoCodigo = -1
//        vProduto = -1

//        'Gera Procoloco
//        vProtocolo = Format(Now, "yyyymmddhhnnss")


//        'Zerando tabela de precos nao alteradas
//        sSql = "Update tb_tabela_precos_produtos set TextoFaixa=nroFaixa, PrecoVenda=0, PrecoExibicao='0.00', PrecoVendaUnitario=0, PrecoExibicaoUnitario='0.00' where idTabelaPreco=4" ' and idProtocolo<>" & vProtocolo
//        Conexao.Execute sSql

//        Do

//          intLinha = intLinha + 1
//          lblInfo.Caption = "Importando " & intLinha
//          DoEvents

//          'Busca linha
//          Line Input #1, sLinha

//          vCampos = Split(sLinha, "|")

//          'Verifica faixa
//          If vProduto <> Val(vCampos(0)) Then
//             iFaixa = 1
//          Else
//             iFaixa = iFaixa + 1
//          End If


//          iFaixa = Val(vCampos(6))

//          'Adiciona campo no produto
//          Set rcoProduto = New ADODB.Recordset

//         ' If Val(vCampos(0)) = 13181 Then Stop

//          sSql = "SELECT * FROM tb_produtos WHERE idEmpresa=5 and Codigo='" & Val(vCampos(0)) & "'"
//          rcoProduto.Open sSql, Conexao, adOpenKeyset, adLockOptimistic

//          If Not rcoProduto.EOF Then

//             Set rcoPreco = New ADODB.Recordset

//             'Busca Precos Escalonados
//             sSql = "SELECT * FROM tb_tabela_precos_produtos "
//             sSql = sSql & vbCr & "WHERE idTabelaPreco=4 and idProduto=" & rcoProduto!idProduto
//             sSql = sSql & vbCr & "and nroFaixa=" & iFaixa
//             sSql = sSql & vbCr & "and idUnidadeMedida=1" 'fixo por enquanto
//             rcoPreco.Open sSql, Conexao, adOpenKeyset, adLockOptimistic


//             If rcoPreco.EOF Then
//                rcoPreco.AddNew
//                rcoPreco!idTabelaPreco = 4
//                rcoPreco!idProduto = rcoProduto!idProduto
//                rcoPreco!idUnidadeMedida = 1
//             End If



//             rcoPreco!idProtocolo = vProtocolo


//             rcoPreco!nroFaixa = iFaixa
//             rcoPreco!idTabelaPrecoFaixa = Val(vCampos(6))
//             rcoPreco!AgrupadorPreco = Trim(Str(rcoProduto!idProduto))




//             rcoPreco!PrecoCusto = 0
//             rcoPreco!PrecoMinimo = CDbl(vCampos(10)) / 100
//             rcoPreco!PrecoVenda = CDbl(vCampos(10)) / 100


//             rcoPreco!PrecoExibicao = Replace(Format(rcoPreco!PrecoVenda, "0.00"), ".", ",")


//             rcoPreco!PrecoVendaUnitario = Round(rcoPreco!PrecoVenda / rcoProduto!fator_venda, 2)


//             rcoPreco!PrecoExibicaoUnitario = Replace(Format(rcoPreco!PrecoVendaUnitario, "0.00"), ".", ",") & "/un"


//             rcoPreco!ValorPromocional = 0

//             'Ajuste de Faixa
//             vFaixaInicialNova = Val(Val(vCampos(8)))
//              Select Case iFaixa
//                    Case 0, 1
//                         If vFaixaInicialNova < 1 Then vFaixaInicialNova = 1
//                    Case 2
//                         If vFaixaInicialNova < 2 Then vFaixaInicialNova = 2
//                    Case 3
//                         If vFaixaInicialNova < 3 Then vFaixaInicialNova = 3
//             End Select


//             vFaixaInicialFinalNova = Val(Val(vCampos(9)))
//             If vFaixaInicialNova > vFaixaInicialFinalNova Then vFaixaInicialFinalNova = vFaixaInicialNova


//             rcoPreco!PrecoFaixaInicial = vFaixaInicialNova
//             rcoPreco!PrecoFaixaFinal = vFaixaInicialNova


//             Select Case iFaixa
//                    Case 1
//                         If vFaixaInicialNova = vFaixaInicialFinalNova Then
//                            rcoPreco!TextoFaixa = Trim(Str(vFaixaInicialNova))
//                         Else
//                            rcoPreco!TextoFaixa = Trim(Str(vFaixaInicialNova)) & " a " & Trim(Str(vFaixaInicialFinalNova))
//                         End If


//                    Case 2
//                         If vFaixaInicialNova = vFaixaInicialFinalNova Then
//                            rcoPreco!TextoFaixa = Trim(Str(vFaixaInicialNova))
//                         Else
//                            rcoPreco!TextoFaixa = Trim(Str(vFaixaInicialNova)) & " a " & Trim(Str(vFaixaInicialFinalNova))
//                         End If
//                    Case 3
//                         rcoPreco!TextoFaixa = Trim(Str(vFaixaInicialNova)) & " ou mais"
//                    Case Else
//                         rcoPreco!TextoFaixa = Trim(Str(vFaixaInicialNova))
//             End Select




//             rcoPreco!StatusPreco = 1

//             'vUltimaQuantidade = rcoPreco!PrecoFaixaFinal

//             'Atualiza produto
//             If iFaixa = 1 Then
//                rcoProduto!Preco1Venda = rcoPreco!PrecoVenda
//                rcoProduto!Preco2VendaUnitario = rcoPreco!PrecoVendaUnitario
//                rcoProduto!PrecoVendaUnitarioExib = Replace(Format(rcoPreco!PrecoVendaUnitario, "0.00"), ".", ",")


//                rcoProduto!Custo = 0
//                rcoProduto!CustoUnitario = 0
//                rcoProduto.Update
//             End If

//             rcoPreco.Update

//             rcoPreco.Close


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
//                        If rcoPreco!PrecoFaixaFinal< 1 Then rcoPreco!PrecoFaixaFinal = 1
//                        If rcoPreco!PrecoFaixaInicial> rcoPreco!PrecoFaixaFinal Then rcoPreco!PrecoFaixaInicial = rcoPreco!PrecoFaixaFinal

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
//                        If rcoPreco!PrecoFaixaFinal< 1 Then rcoPreco!PrecoFaixaFinal = 1
//                        If rcoPreco!PrecoFaixaInicial> rcoPreco!PrecoFaixaFinal Then rcoPreco!PrecoFaixaInicial = rcoPreco!PrecoFaixaFinal

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


//                    If rcoPreco!PrecoFaixaFinal<rcoPreco!PrecoFaixaInicial Then
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
//        Exit Function


//AdoError:


//      Dim errLoop As ADODB.Error
//      Dim strError As String
//      Dim sErro As String
//      Dim I As Integer


//      'If Err = 3705 And bConectando Then
//      '   Set mConexaoDb2 = New ADODB.Connection
//      '   Resume Inicio
//      'End If

//      'If Err = 3705 Then Resume Next

//      'mbErroConexao = True


//      sErro = ""
//      sErro = sErro & "Ocorreu algum erro durante a importação do arquivo."


//      If Err.Number<> 0 Then
//         sErro = sErro & vbNewLine & String$(80, "-")
//         sErro = sErro & vbNewLine & "Error # " & Str$(Err.Number)
//         sErro = sErro & vbNewLine & "Gerado por " & Err.Source
//         sErro = sErro & vbNewLine & "Descrição  " & Err.Description
//         sErro = sErro & vbNewLine & String$(80, "-")
//      End If


//      MsgBox sErro, vbCritical + vbOKOnly, "mrSoft"

//End Function

//        Private Function rotImportaPrecosSql(sFile As String) As Boolean
//        Dim vTemp As String
//        Dim intLinha As Integer
//        Dim sLinha As String
//        Dim vCampos As Variant

//        Dim iCampo As Integer
//        Dim sCampo As String
//        Dim sidCaracteristica(0 To 200) As String
//        Dim iCampoCodigo As Integer
//        Dim vFaixaInicialNova As Variant

//        Dim sSql As String
//        Dim rcoProduto As ADODB.Recordset
//        Dim rcoPreco As ADODB.Recordset
//        Dim rcoCaracteristica As ADODB.Recordset

//        Dim vFaixaInicialFinalNova As Variant

//        Dim iCodigo As Variant

//        Dim iErro As Integer
//        Dim iNovo As Integer
//        Dim iEdita As Integer

//        Dim sDestino As String
//        Dim vProduto As Variant
//        Dim iFaixa As Integer

//        'On Local Error GoTo AdoError

//        If Not ConectarDB Then
//           MsgBox "Erro ao conectar banco de dados!", vbCritical + vbOKOnly, "mrSoft"
//           Exit Function
//        End If


//        If Dir(txtArquivo) = "" Then
//           MsgBox "Não localizei o arquivo " & txtArquivo.Text, vbCritical + vbOKOnly, "mrSoft"
//           Exit Function
//        End If


//        cmdImportar.Enabled = False


//        Log "Importando " & txtArquivo

//        'inicia leitura dos cabecalhos
//        Open txtArquivo For Input As #1

//        'inicia laco de busca de caracteristicas
//        intLinha = -1
//        iCampoCodigo = -1
//        vProduto = -1

//        Open "c:\comandopreco.sql" For Append As 21

//        Do

//          intLinha = intLinha + 1
//          lblInfo.Caption = "Importando " & intLinha
//          DoEvents

//          'Busca linha
//          Line Input #1, sLinha

//          vCampos = Split(sLinha, "|")

//          'Verifica faixa
//          If vProduto <> Val(vCampos(0)) Then
//             iFaixa = 1
//          Else
//             iFaixa = iFaixa + 1
//          End If


//          iFaixa = Val(vCampos(6))

//          'Adiciona campo no produto
//          Set rcoProduto = New ADODB.Recordset

//         ' If Val(vCampos(0)) = 13181 Then Stop

//          sSql = "SELECT * FROM tb_produtos WHERE idEmpresa=5 and Codigo='" & Val(vCampos(0)) & "'"
//          rcoProduto.Open sSql, Conexao, adOpenKeyset, adLockOptimistic

//      '    If Not rcoProduto.EOF Then
//             Set rcoPreco = New ADODB.Recordset

//             sSql = "Update tb_tabela_precos_produtos"
//             sSql = sSql & vbNewLine & "left join tb_produtos on tb_produtos.idProduto=tb_tabela_precos_produtos.idProduto"
//             sSql = sSql & vbNewLine & "Set tb_tabela_precos_produtos.PrecoCusto = 0"
//             sSql = sSql & vbNewLine & ", PrecoMinimo=" & Format(CDbl(vCampos(10)) / 100, "0.00")
//             sSql = sSql & vbNewLine & ", PrecoVenda=" & Format(CDbl(vCampos(10)) / 100, "0.00")
//             sSql = sSql & vbNewLine & ", PrecoExibicao='" & Replace(Format(CDbl(vCampos(10)) / 100, "0.00"), ".", ",") & "'"
//             sSql = sSql & vbNewLine & ", PrecoVendaUnitario=round(" & CDbl(vCampos(10)) / 100 & "/tb_produtos.Quantidade_na_embalagem,2)"
//             sSql = sSql & vbNewLine & ", PrecoExibicaoUnitario=round(" & CDbl(vCampos(10)) / 100 & "/tb_produtos.Quantidade_na_embalagem,2)"
//             sSql = sSql & vbNewLine & ", ValorPromocional=0"

//             vFaixaInicialNova = Val(Val(vCampos(8)))







//             Select Case iFaixa
//                    Case 0, 1


//                         If vFaixaInicialNova< 1 Then vFaixaInicialNova = 1

//                         'If Val(Val(vCampos(8))) = Val(Val(vCampos(9))) Then
//                            sSql = sSql & vbNewLine & ", TextoFaixa='" & Trim(Str(vFaixaInicialNova)) & "'"
//                         'Else
//                         '   sSql = sSql & vbNewLine & ", TextoFaixa='" & Trim(Str(vFaixaInicialNova)) & "'" ' a " & Trim(Str(Val(Val(vCampos(9))))) & "'"
//                         'End If


//                    Case 2
//                         If vFaixaInicialNova < 2 Then vFaixaInicialNova = 2

//                         'If Val(Val(vCampos(8))) = Val(Val(vCampos(9))) Then
//                            sSql = sSql & vbNewLine & ", TextoFaixa='" & Trim(Str(vFaixaInicialNova)) & "'"
//                         'Else
//                         '   sSql = sSql & vbNewLine & ", TextoFaixa='" & Trim(Str(vFaixaInicialNova)) & "'"  '& a " & Trim(Str(Val(Val(vCampos(9))))) & "'"
//                         'End If
//                    Case 3
//                         If vFaixaInicialNova < 3 Then vFaixaInicialNova = 3
//                         sSql = sSql & vbNewLine & ", TextoFaixa='" & Trim(Str(vFaixaInicialNova)) & " ou mais" & "'"
//                    'Case Else
//                    '     sSql = sSql & vbNewLine & ", TextoFaixa='" & Trim(Str(vFaixaInicialNova)) & "'"
//             End Select


//             sSql = sSql & vbNewLine & ", PrecoFaixaInicial=" & vFaixaInicialNova



//             vFaixaInicialFinalNova = Val(Val(vCampos(9)))
//             If vFaixaInicialNova > vFaixaInicialFinalNova Then vFaixaInicialFinalNova = vFaixaInicialNova


//             sSql = sSql & vbNewLine & ", PrecoFaixaFinal=" & vFaixaInicialFinalNova


//             sSql = sSql & vbNewLine & "Where tb_tabela_precos_produtos.idTabelaPreco = 4 and nroFaixa=" & iFaixa
//             sSql = sSql & vbNewLine & "and tb_tabela_precos_produtos.idProduto=tb_produtos.idProduto and tb_produtos.sku='PP-" & Val(vCampos(0)) & "';"



//             Print #21, sSql


//             GoTo proximoproduto

//             'Busca Precos Escalonados
//             sSql = "SELECT * FROM tb_tabela_precos_produtos "
//             sSql = sSql & vbCr & "WHERE idTabelaPreco=4 and idProduto=" & rcoProduto!idProduto
//             sSql = sSql & vbCr & "and nroFaixa=" & iFaixa
//             sSql = sSql & vbCr & "and idUnidadeMedida=1" 'fixo por enquanto
//             rcoPreco.Open sSql, Conexao, adOpenKeyset, adLockOptimistic

//             If rcoPreco.EOF Then
//                rcoPreco.AddNew
//                rcoPreco!idTabelaPreco = 4
//                rcoPreco!idProduto = rcoProduto!idProduto
//                rcoPreco!idUnidadeMedida = 1
//             End If



//             rcoPreco!nroFaixa = iFaixa
//             rcoPreco!idTabelaPrecoFaixa = Val(vCampos(6))
//             rcoPreco!AgrupadorPreco = Trim(Str(rcoProduto!idProduto))


//             rcoPreco!PrecoCusto = 0
//             rcoPreco!PrecoMinimo = CDbl(vCampos(10)) / 100
//             rcoPreco!PrecoVenda = CDbl(vCampos(10)) / 100


//             rcoPreco!PrecoExibicao = Replace(Format(rcoPreco!PrecoVenda, "0.00"), ".", ",")


//             rcoPreco!PrecoVendaUnitario = Round(rcoPreco!PrecoVenda / rcoProduto!Quantidade_na_embalagem, 2)


//             rcoPreco!PrecoExibicaoUnitario = Replace(Format(rcoPreco!PrecoVendaUnitario, "0.00"), ".", ",") & "/un"





//             rcoPreco!ValorPromocional = 0


//             rcoPreco!PrecoFaixaInicial = Val(Val(vCampos(8)))
//             rcoPreco!PrecoFaixaFinal = Val(Val(vCampos(9)))


//             Select Case iFaixa
//                    Case 1
//                         If Val(Val(vCampos(8))) = Val(Val(vCampos(9))) Then
//                            rcoPreco!TextoFaixa = Trim(Str(Val(Val(vCampos(8)))))
//                         Else
//                            rcoPreco!TextoFaixa = Trim(Str(Val(Val(vCampos(8))))) & " a " & Trim(Str(Val(Val(vCampos(9)))))
//                         End If


//                    Case 2
//                         If Val(Val(vCampos(8))) = Val(Val(vCampos(9))) Then
//                            rcoPreco!TextoFaixa = Trim(Str(Val(Val(vCampos(8)))))
//                         Else
//                            rcoPreco!TextoFaixa = Trim(Str(Val(Val(vCampos(8))))) & " a " & Trim(Str(Val(Val(vCampos(9)))))
//                         End If
//                    Case 3
//                         rcoPreco!TextoFaixa = Trim(Str(Val(Val(vCampos(8))))) & " ou mais"
//                    Case Else
//                         rcoPreco!TextoFaixa = Trim(Str(Val(Val(vCampos(8)))))
//             End Select




//             rcoPreco!StatusPreco = 1

//             'vUltimaQuantidade = rcoPreco!PrecoFaixaFinal

//             'Atualiza produto
//             If iFaixa = 1 Then
//                rcoProduto!Preco1Venda = rcoPreco!PrecoVenda
//                rcoProduto!Preco2VendaUnitario = rcoPreco!PrecoVendaUnitario
//                rcoProduto!PrecoVendaUnitarioExib = Replace(Format(rcoPreco!PrecoVendaUnitario, "0.00"), ".", ",")

//                rcoProduto!Custo = 0
//                rcoProduto!CustoUnitario = 0
//                rcoProduto.Update
//             End If

//             rcoPreco.Update
//             rcoPreco.Close
//   '       End If



//          rcoProduto.Close

//proximoproduto:
//          'Seta produto
//          vProduto = Val(vCampos(0))


//        Loop While Not EOF(1)

//        Close 1

//        Close #21

//        Log "Importação finalizada"
//        Log "Novos Registros " & iNovo
//        Log "Registros Editados  " & iEdita
//        Log "Erros Registros " & iErro

//        sDestino = Left(txtArquivo, Len(txtArquivo) - 3) & "pro"
//        If Dir(sDestino) <> "" Then Kill sDestino
//        Log "Renomeado > " & sDestino
//        Name txtArquivo As sDestino

//        rotImportaPrecosSql = True

//        'cmdImportar.Enabled = True


//final:
//        Exit Function


//AdoError:

//      Dim errLoop As ADODB.Error
//      Dim strError As String
//      Dim sErro As String
//      Dim I As Integer


//      'If Err = 3705 And bConectando Then
//      '   Set mConexaoDb2 = New ADODB.Connection
//      '   Resume Inicio
//      'End If

//      'If Err = 3705 Then Resume Next

//      'mbErroConexao = True


//      sErro = ""
//      sErro = sErro & "Ocorreu algum erro durante a importação do arquivo."


//      If Err.Number<> 0 Then
//         sErro = sErro & vbNewLine & String$(80, "-")
//         sErro = sErro & vbNewLine & "Error # " & Str$(Err.Number)
//         sErro = sErro & vbNewLine & "Gerado por " & Err.Source
//         sErro = sErro & vbNewLine & "Descrição  " & Err.Description
//         sErro = sErro & vbNewLine & String$(80, "-")
//      End If


//      MsgBox sErro, vbCritical + vbOKOnly, "mrSoft"

//End Function

//        Private Function rotImportaEstoque(sFile As String) As Boolean
//        Dim vTemp As String
//        Dim intLinha As Integer
//        Dim sLinha As String
//        Dim vCampos As Variant

//        Dim iCampo As Integer
//        Dim sCampo As String
//        Dim sidCaracteristica(0 To 200) As String
//        Dim iCampoCodigo As Integer

//        Dim sSql As String
//        Dim rcoProduto As ADODB.Recordset
//        Dim rcoPreco As ADODB.Recordset
//        Dim rcoCaracteristica As ADODB.Recordset

//        Dim iCodigo As Variant

//        Dim iErro As Integer
//        Dim iNovo As Integer
//        Dim iEdita As Integer

//        Dim sDestino As String
//        Dim vProduto As Variant
//        Dim iFaixa As Integer
//        Dim vProtocolo As Variant

//        'On Local Error GoTo AdoError

//        If Not ConectarDB Then
//           MsgBox "Erro ao conectar banco de dados!", vbCritical + vbOKOnly, "mrSoft"
//           Exit Function
//        End If

//        If Dir(txtArquivo) = "" Then
//           MsgBox "Não localizei o arquivo " & txtArquivo.Text, vbCritical + vbOKOnly, "mrSoft"
//           Exit Function
//        End If

//        cmdImportar.Enabled = False

//        'Gerando protocolo
//        vProtocolo = Format(Now, "yyyymmddhhnnss")

//        Log "Importando " & txtArquivo

//        'inicia leitura dos cabecalhos
//        Open txtArquivo For Input As #1

//        'inicia laco de busca de caracteristicas
//        intLinha = -1
//        iCampoCodigo = -1
//        vProduto = -1

//        Do

//          'Busca linha
//          Line Input #1, sLinha

//          sLinha = Trim(Replace(sLinha, ";", ""))
//          vCampos = Split(sLinha, ",")


//          intLinha = intLinha + 1
//          lblInfo.Caption = "Importando " & intLinha & " " & Val(vCampos(0))
//          DoEvents

//          'Adiciona campo no produto
//          Set rcoProduto = New ADODB.Recordset

//          sSql = "SELECT * FROM tb_produtos WHERE idEmpresa=5 and codigo='" & Val(vCampos(0)) & "'"
//          rcoProduto.Open sSql, Conexao, adOpenKeyset, adLockOptimistic

//          If Not rcoProduto.EOF Then
//             rcoProduto!Estoque = CDbl(vCampos(1))
//             rcoProduto.Update


//             'Atualizando tabela do estoque
//             sSql = "INSERT INTO tb_estoque"
//             sSql = sSql & vbCr & "(idEstoque, idEmpresa, idProduto, idProtocolo, Estoque)"


//             sSql = sSql & vbCr & "select"
//             sSql = sSql & vbCr & rcoProduto!idProduto & " , " & rcoProduto!idEmpresa & ", " & rcoProduto!idProduto & ", " & vProtocolo & ", 0"


//             sSql = sSql & vbCr & "ON DUPLICATE KEY UPDATE"
//             sSql = sSql & vbCr & "idEmpresa = " & rcoProduto!idEmpresa
//             sSql = sSql & vbCr & ", idProduto=" & rcoProduto!idProduto & ""
//             sSql = sSql & vbCr & ", idProtocolo=" & vProtocolo
//             Conexao.Execute sSql

//             'Limpa Tabela de Baixas
//             sSql = "Delete from tb_estoque_baixa"
//             sSql = sSql & vbCr & "Where idEmpresa= " & rcoProduto!idEmpresa
//             sSql = sSql & vbCr & "and idProduto=" & rcoProduto!idProduto
//             Conexao.Execute sSql


//             sSql = "INSERT INTO tb_estoque_baixa"
//             sSql = sSql & vbCr & "(idEstoque, idEmpresa,idTipoMovimento, idProduto, idlstPedidoItem, EstoqueBaixa)"
//             sSql = sSql & vbCr & "select " & rcoProduto!idProduto & " , " & rcoProduto!idEmpresa & ",1," & rcoProduto!idProduto & ", 0, " & CDbl(vCampos(1))
//             Conexao.Execute sSql

//             '=edit&&=string&idEmpresa=string&Estoque=string&id=XXXXX

//             'Atualizando APP
//             sSql = "http://fb.pedidodireto.com/api.php?table=Stock&op=edit&"
//             sSql = sSql & "&idProduto=" & rcoProduto!idProduto
//             sSql = sSql & "&idEmpresa=" & rcoProduto!idEmpresa
//             sSql = sSql & "&Estoque=" & CDbl(vCampos(1))
//             sSql = sSql & "&id=" & rcoProduto!idEmpresa & "-" & rcoProduto!idProduto
//             'sSql = Inet1.OpenURL(sSql)

//          Else
//            Log "Produto estoque nao encontrado " & Val(vCampos(0))
//          End If

//          rcoProduto.Close

//        Loop While Not EOF(1)

//        Close 1

//        'Limpa Itens que nao alterou
//        Set rcoProduto = New ADODB.Recordset

//        sSql = "select tb_estoque.idProduto, tb_estoque.idEmpresa from tb_estoque where (idEmpresa=5 and idProtocolo<>" & vProtocolo & ")"
//        sSql = sSql & " or (idEmpresa=5 and idProduto in (select tb_Produtos.idProduto from tb_Produtos where idEmpresa=5 and idStatus=2))"
//        rcoProduto.Open sSql, Conexao, adOpenKeyset, adLockOptimistic

//        While Not rcoProduto.EOF

//             intLinha = intLinha + 1
//             lblInfo.Caption = "Limpando " & rcoProduto!idProduto
//             DoEvents

//              'Limpa Tabela de Baixas
//             sSql = "Delete from tb_estoque_baixa"
//             sSql = sSql & vbCr & "Where idEmpresa= " & rcoProduto!idEmpresa
//             sSql = sSql & vbCr & "and idProduto=" & rcoProduto!idProduto
//             sSql = sSql & vbCr & "and idTipoMovimento=1"
//             Conexao.Execute sSql

//             'sSql = "Update tb_produtos set Estoque=0 where idEmpresa=5 and idProduto=" & rcoProduto!idProduto
//             'Conexao.Execute sSql

//             'Atualizando APP
//             sSql = "http://fb.pedidodireto.com/api.php?table=Stock&op=edit&"
//             sSql = sSql & "&idProduto=" & rcoProduto!idProduto
//             sSql = sSql & "&idEmpresa=" & rcoProduto!idEmpresa
//             sSql = sSql & "&Estoque=0"
//             sSql = sSql & "&id=" & rcoProduto!idEmpresa & "-" & rcoProduto!idProduto
//             'sSql = Inet1.OpenURL(sSql)

//             rcoProduto.MoveNext
//        Wend

//        rcoProduto.Close

//        'Log
//        Log "Importação finalizada"
//        Log "Novos Registros " & iNovo
//        Log "Registros Editados  " & iEdita
//        Log "Erros Registros " & iErro

//        sDestino = Left(txtArquivo, Len(txtArquivo) - 3) & "pro"
//        If Dir(sDestino) <> "" Then Kill sDestino
//        Log "Renomeado > " & sDestino
//        Name txtArquivo As sDestino

//        rotImportaEstoque = True

//        'cmdImportar.Enabled = True


//final:
//        Exit Function


//AdoError:

//      Dim errLoop As ADODB.Error
//      Dim strError As String
//      Dim sErro As String
//      Dim I As Integer


//      'If Err = 3705 And bConectando Then
//      '   Set mConexaoDb2 = New ADODB.Connection
//      '   Resume Inicio
//      'End If

//      'If Err = 3705 Then Resume Next

//      'mbErroConexao = True


//      sErro = ""
//      sErro = sErro & "Ocorreu algum erro durante a importação do arquivo."


//      If Err.Number<> 0 Then
//         sErro = sErro & vbNewLine & String$(80, "-")
//         sErro = sErro & vbNewLine & "Error # " & Str$(Err.Number)
//         sErro = sErro & vbNewLine & "Gerado por " & Err.Source
//         sErro = sErro & vbNewLine & "Descrição  " & Err.Description
//         sErro = sErro & vbNewLine & String$(80, "-")
//      End If


//      MsgBox sErro, vbCritical + vbOKOnly, "mrSoft"

//End Function

//Private Sub as3Button26_Click()


//        Dim vTemp As String
//        Dim intLinha As Integer
//        Dim sLinha As String
//        Dim vCampos As Variant


//        Dim iCampo As Integer
//        Dim sCampo As String
//        Dim sidCaracteristica(0 To 200) As String
//        Dim iCampoCodigo As Integer
//        Dim bNovo As Boolean


//        Dim sSql As String
//        Dim rcoProduto As ADODB.Recordset
//        Dim rcoPreco As ADODB.Recordset
//        Dim rcoCaracteristica As ADODB.Recordset

//        Dim datCalculada As Variant
//        Dim intI As Integer

//        Dim iCodigo As Variant

//        Dim iErro As Integer
//        Dim iNovo As Integer
//        Dim iEdita As Integer

//        Dim sDestino As String
//        Dim vProduto As Variant
//        Dim iFaixa As Integer
//        Dim dataInicial As Variant
//        Dim rcoRotas As ADODB.Recordset
//        Dim rcoRotasEntrega As ADODB.Recordset
//        Dim intSemana As Integer
//        Dim datLancamento As Variant

//        'On Local Error GoTo AdoError


//        If Not ConectarDB Then
//           MsgBox "Erro ao conectar banco de dados!", vbCritical + vbOKOnly, "mrSoft"
//           Exit Sub
//        End If


//        cmdImportar.Enabled = False


//        Log "Gerando Datas"


//        dataInicial = DateAdd("y", 1, Now)


//        'Verifica rotas para desativar
//        Set rcoRotas = New ADODB.Recordset
//        sSql = "select concat(idempresa,'-',idRotaVenda,'-',idEntidade,'-',idRotaVendaEntrega) as chave from tb_rotavenda_entrega where Disponivel=1 and DataInicial <= '" & Format(Now, "YYYY-MM-DD") & "'"
//        rcoRotas.Open sSql, Conexao, adOpenKeyset, adLockOptimistic

//        While Not rcoRotas.EOF

//               Log "Removendo RotaEntrega " & rcoRotas!chave
//               lblInfo.Caption = "Importando " & rcoRotas!chave
//          DoEvents

//             ' sSql = Inet1.OpenURL("http://fb.pedidodireto.com/api.php?table=RotaVendaEntrega&op=delete&id=" & rcoRotas!chave)



//              rcoRotas.MoveNext
//        Wend


//        rcoRotas.Close

//        '

//        'Desativa datas Antigas
//        sSql = "Update tb_rotavenda_entrega set Disponivel=0 where DataInicial <= '" & Format(Now, "YYYY-MM-DD") & "'"
//        Conexao.Execute sSql

//        'Buscando Rotas
//        Set rcoRotas = New ADODB.Recordset

//        sSql = "Select * from tb_rotavenda where idEmpresa=5"
//        rcoRotas.Open sSql, Conexao, adOpenKeyset, adLockOptimistic

//        While Not rcoRotas.EOF

//          Log "Importando " & rcoRotas!RotaVenda
//          lblInfo.Caption = "Importando " & rcoRotas!RotaVenda
//          DoEvents

//          'DataCalculada
//          For intI = 0 To 6
//              If (DatePart("w", DateAdd("y", intI, dataInicial)) = rcoRotas!diaEntrega) Or(DatePart("w", DateAdd("y", intI, dataInicial)) = 6 And rcoRotas!diaEntrega = 0) Then
//                datCalculada = DateAdd("y", intI, dataInicial)
//                 Exit For
//              End If
//          Next intI



//          For intSemana = 0 To 4

//               'Ajusta semanas
//               datLancamento = DateAdd("y", intSemana* 7, datCalculada)

//               'Adiciona campo no produto
//               Set rcoRotasEntrega = New ADODB.Recordset

//               sSql = "SELECT * FROM tb_rotavenda_entrega WHERE idEmpresa=5 and idRotaVenda='" & rcoRotas!idRotaVenda & "'  and DataInicial='" & Format(datLancamento, "YYYY-MM-DD") & "'"
//               rcoRotasEntrega.Open sSql, Conexao, adOpenKeyset, adLockOptimistic

//               bNovo = False
//               If rcoRotasEntrega.EOF Then
//                  rcoRotasEntrega.AddNew
//                  bNovo = True
//               End If


//               rcoRotasEntrega!idEmpresa = 5
//               rcoRotasEntrega!idRotaVenda = rcoRotas!idRotaVenda

//              ' If rcoRotas!diaEntrega = 0 Then 'rota padrao
//              '    rcoRotasEntrega!Legenda = "a Confirmar " & Format(datLancamento, "dd/mm/yy")
//              '    rcoRotasEntrega!LegendaCombo = Format(datLancamento, "dd/mm/yy") & " (a Confirmar)"
//              ' Else
//                  rcoRotasEntrega!Legenda = "Frete Gratis " & Format(datLancamento, "dd/mm/yy")
//                  rcoRotasEntrega!LegendaCombo = Format(datLancamento, "dd/mm/yy") & " (Gratis)"
//             '  End If

//               rcoRotasEntrega!dataInicial = datLancamento
//               rcoRotasEntrega!DataFinal = datLancamento
//               rcoRotasEntrega!DataLimite = DateAdd("Y", -1, Format(datLancamento, "dd/mm/yy")) & " 17:00:00"
//               rcoRotasEntrega!Disponivel = 1
//               rcoRotasEntrega!ValorFrete = 0
//               rcoRotasEntrega!CompraMinima = 0
//               rcoRotasEntrega!idEntidade = 0
//               rcoRotasEntrega!idVendedor = 0

//               rcoRotasEntrega.Update
//               rcoRotasEntrega.Close


//               If bNovo Then
//                     'Busca para salvar no firebase
//                     Set rcoRotasEntrega = New ADODB.Recordset

//                     sSql = "SELECT"
//                     sSql = sSql & vbCr & "concat(rte.idempresa,'-',rte.idRotaVenda,'-',rte.idEntidade,'-',rte.idRotaVendaEntrega) as chave"
//                     sSql = sSql & vbCr & ",cast(rte.idRotaVendaEntrega as char) as idRotaVendaEntrega"
//                     sSql = sSql & vbCr & ",rte.idempresa"
//                     sSql = sSql & vbCr & ",rte.identidade"
//                     sSql = sSql & vbCr & ",rte.idRotaVenda"
//                     sSql = sSql & vbCr & ",rte.Disponivel"
//                     sSql = sSql & vbCr & ",rte.idVendedor"
//                     sSql = sSql & vbCr & ",rte.DataFinal"
//                     sSql = sSql & vbCr & ",rte.DataLimite"
//                     sSql = sSql & vbCr & ",rte.ValorFrete"
//                     sSql = sSql & vbCr & ",(case when rte.ValorFrete=0 then 'Gratis' else 'Pago' end) as tpFrete"
//                     sSql = sSql & vbCr & ",rte.Legenda as FreteExibicao"
//                     sSql = sSql & vbCr & ",rte.LegendaCombo as TextoCombo"
//                     sSql = sSql & vbCr & "FROM tb_rotavenda_entrega rte"
//                     sSql = sSql & vbCr & "WHERE idEmpresa=5 and idRotaVenda='" & rcoRotas!idRotaVenda & "'  and DataInicial='" & Format(datLancamento, "YYYY-MM-DD") & "'"
//                     rcoRotasEntrega.Open sSql, Conexao, adOpenKeyset, adLockOptimistic


//                     sSql = "http://fb.pedidodireto.com/api.php?table=RotaVendaEntrega&op=edit&"
//                     sSql = sSql & "&DataFinal=" & Format(rcoRotasEntrega!DataFinal, "YYYY-MM-DD")
//                     sSql = sSql & "&DataLimite=" & Format(rcoRotasEntrega!DataLimite, "YYYY-MM-DD HH:NN:SS")
//                     sSql = sSql & "&Disponivel=" & rcoRotasEntrega!Disponivel
//                     sSql = sSql & "&FreteExibicao=" & rcoRotasEntrega!FreteExibicao
//                     sSql = sSql & "&TextoCombo=" & rcoRotasEntrega!TextoCombo
//                     sSql = sSql & "&ValorFrete=" & rcoRotasEntrega!ValorFrete
//                     sSql = sSql & "&idEmpresa=" & rcoRotasEntrega!idEmpresa
//                     sSql = sSql & "&idEntidade=" & rcoRotasEntrega!idEntidade
//                     sSql = sSql & "&idRotaVenda=" & rcoRotasEntrega!idRotaVenda
//                     sSql = sSql & "&idRotaVendaEntrega=" & rcoRotasEntrega!idRotaVendaEntrega
//                     sSql = sSql & "&idVendedor=" & rcoRotasEntrega!idVendedor
//                     sSql = sSql & "&tpFrete=" & rcoRotasEntrega!tpFrete
//                     sSql = sSql & "&id=" & rcoRotasEntrega!chave

//                    ' sSql = Inet1.OpenURL(sSql)
//               End If


//          Next intSemana


//          rcoRotas.MoveNext


//        Wend


//        rcoRotas.Close



//        Log "Importação finalizada"




//        'cmdImportar.Enabled = True

//final:
//        Exit Sub


//AdoError:

//      Dim errLoop As ADODB.Error
//      Dim strError As String
//      Dim sErro As String
//      Dim I As Integer


//      'If Err = 3705 And bConectando Then
//      '   Set mConexaoDb2 = New ADODB.Connection
//      '   Resume Inicio
//      'End If

//      'If Err = 3705 Then Resume Next

//      'mbErroConexao = True


//      sErro = ""
//      sErro = sErro & "Ocorreu algum erro durante a importação do arquivo."


//      If Err.Number<> 0 Then
//         sErro = sErro & vbNewLine & String$(80, "-")
//         sErro = sErro & vbNewLine & "Error # " & Str$(Err.Number)
//         sErro = sErro & vbNewLine & "Gerado por " & Err.Source
//         sErro = sErro & vbNewLine & "Descrição  " & Err.Description
//         sErro = sErro & vbNewLine & String$(80, "-")
//      End If


//      MsgBox sErro, vbCritical + vbOKOnly, "mrSoft"
//End Sub

//Private Function rotBuscarLokalizei(bAutomatico As Boolean, sMenu As String) As Boolean
//        Dim intI As Integer
//        Dim sArquivo As String
//        Dim sDiretorioDestino As String
//        Dim bEnviaStatus As Boolean
//        Dim sExporta As String
//        Dim sNomeArquivoStatus As String
//        Dim sDestinatarioLokalizei As String
//        Dim sMensagem As String
//        Dim mCfgEmail As as3EmailDados
//        Dim sEMail As String
//        Dim sDestinatarioCliente As String
//        Dim rcoEntidade As ADODB.Recordset
//        Dim rcoViagem As ADODB.Recordset

//        rotBuscarLokalizei = True

//        If mbolProcessoEmExecucao Then Exit Function
//        If Not bAutomatico And mbolAutomatico Then Exit Function
        
//        'On Local Error Resume Next


//        If mbRecebendoArquivos Then
//           Log "Erro - Esta recebendo dados - Aguardar conclusão do processo"
//           Exit Function
//        End If


//        If mbImportandoArquivos Then
//           Log "Erro - Esta importando dados - Aguardar conclusão do processo"
//           Exit Function
//        End If


//        If Not ConectaLokalizei Then
//           Log "Erro - Não consegui abrir banco de dados do Lokalizei"
//           Exit Function
//        End If


//         If Not ConectaSuite Then
//           Log "Erro - Não consegui abrir banco de dados do Suite"
//           Exit Function
//        End If


//        mbolProcessoEmExecucao = True
//        Timer1.Enabled = False
//        DoEvents
        
//        'colocando em status de importar
//        mbImportandoArquivos = True
        
//        'Destinatario dos pedidos
//        sDestinatarioLokalizei = iniEdit(mArquivoCfg, "Email", "EmailLokalizei", "programacao@Trade2UP.com.br", ssIniBuscarCriar)
                
//        'Processando arquivos de viagem
//        fileImporta.Path = txtLokalizei
//        fileImporta.Pattern = "VIAGEM.csv"
//        SSActiveTabs1.Tabs(3).Caption = "&3 Arquivos (Viagens " & fileImporta.ListCount & ")"
//        Log "Viagens:  " & fileImporta.ListCount & " Arquivos"

//        'cria sub-diretiro
//        sDiretorioDestino = txtLokalizei & "Processado"  '   '" & Format(Now, "yyyy") & "\" & Format(Now, "mm")
//        'MkDir sDiretorioDestino & "\"
        
//        'Transfere visistas para arquivo morto
//        For intI = 0 To fileImporta.ListCount - 1
//            fileImporta.ListIndex = intI
//            DoEvents
//            Log "Processando arquivo " & intI & " de " & fileImporta.ListCount & " " & fileImporta.FileName

//            sEMail = sEMail & vbNewLine & "Processando arquivo " & fileImporta.FileName
            
//            'Busca codigo da empresa
//            vCodigoCustumer = Val(iniEdit(mArquivoCfg, "Lokalizei", "ID_" & Left(fileImporta.FileName, 9), "0", ssIniBuscarCriar))


//            If vCodigoCustumer = 0 Then
//               Log "Erro - Informar o codigo interno lokalizei"
//               Exit Function
//            End If
        
            
//            'inicia leitura dos cabecalhos
//            Open txtLokalizei & fileImporta.FileName For Input As #1
            
//            'inicia laco de busca de caracteristicas
//            intLinha = -1

//            Do
                            
//              'Busca linha
//              Line Input #1, sLinha
              
//              'sLinha = Trim(Replace(sLinha, ";", ""))
              
//              'altera endereco antes do split
//              iInicio = InStr(sLinha, Chr(34))
//              sOrigem = Mid(sLinha, iInicio + 1)
//              sendereco = Left(sOrigem, InStr(sOrigem, Chr(34)) - 1)

//              sTracado = Replace(sendereco, ",", "$$-$$")

//              sLinha = Replace(sLinha, sendereco, sTracado)

//              vCampos = Split(sLinha, ",")

//              vCarga = ""
//              vviagem = ""
//              vPlaca = ""
               
//              'Separando informacoes
//              vviagem = vCampos(1) 'pega descricao
//              vviagem = Mid(vviagem, InStr(vviagem, "[#V") + 3)
//              vviagem = "V" & Left(vviagem, InStr(vviagem, "]") - 1)
              
//              'ajusta log
//              intLinha = intLinha + 1
//              Log "Importando " & intLinha & " # Viagem" & vviagem
//              DoEvents
 
//              'Separando placa
//              vPlaca = vCampos(1) 'pega descricao
//              vPlaca = Mid(vPlaca, InStr(vPlaca, "[#P") + 3)
//              vPlaca = Left(vPlaca, InStr(vPlaca, "]") - 1)
              
              
//              'Adiciona campo no produto
//              Set rcoViagem = New ADODB.Recordset

//              sSql = "SELECT * FROM lk_driver_task WHERE EDI_TipoTarefa=1 and customer_id=" & vCodigoCustumer & " and EDI_Viagem='" & vviagem & "'"
//              rcoViagem.Open sSql, mConexaoLokalizei, adOpenKeyset, adLockOptimistic

//              If rcoViagem.EOF Then
//                 rcoViagem.AddNew
//                 rcoViagem!task_token = CrcNet(Format(customer_id, "00000") & vviagem, 8)
//                 rcoViagem!customer_id = vCodigoCustumer
//                 rcoViagem!date_created = Now()
//                 rcoViagem!edi_viagem = vviagem
//                 rcoViagem!edi_tipotarefa = 1
//              Else
//                 rcoViagem!date_modified = Now()
//              End If


//              rcoViagem!task_description = vCampos(1)
//              rcoViagem!trans_type = "Delivery" 'vCampos(0)
//              rcoViagem!contact_number = vCampos(2)
//              rcoViagem!email_address = vCampos(3)
//              rcoViagem!customer_name = vCampos(4)
//              rcoViagem!delivery_date = IIf(CVDate(vCampos(5)) < Now, DateAdd("n", 60, Now()), vCampos(5))
//              rcoViagem!delivery_address = sendereco
//              rcoViagem!task_lat = vCampos(7)
//              rcoViagem!task_lng = vCampos(8)
//              rcoViagem!ip_address = "0.0.0.0"



//              rcoViagem.Update

//              rcoViagem.Close


//            Loop While Not EOF(1)


//            Close 1
            
//            'renomeia arquivo
//            sArquivo = fileImporta.FileName

//            If Arquivo_Existe(sDiretorioDestino & Left(sArquivo, Len(sArquivo) - 3) & "pro") Then
//               Kill sDiretorioDestino & Left(sArquivo, Len(sArquivo) - 3) & "pro"
//            End If


//            Name txtLokalizei & sArquivo As sDiretorioDestino & "\" & Left(sArquivo, Len(sArquivo) - 3) & "pro"
            
            
//        Next intI
        
        
//        'Processando arquivos de viagem
//        fileImporta.Path = txtLokalizei
//        fileImporta.Pattern = "CARGA.csv"
//        SSActiveTabs1.Tabs(3).Caption = "&3 Arquivos (Caras " & fileImporta.ListCount & ")"
//        Log "Cargas:  " & fileImporta.ListCount & " Arquivos"
        
//        'cria sub-diretiro
//        sDiretorioDestino = txtLokalizei & "Processado"  '   '" & Format(Now, "yyyy") & "\" & Format(Now, "mm")
//        'MkDir sDiretorioDestino & "\"
        
//        'Transfere visistas para arquivo morto
//        For intI = 0 To fileImporta.ListCount - 1
//            fileImporta.ListIndex = intI
//            DoEvents
//            Log "Processando Carga " & intI & " de " & fileImporta.ListCount & " " & fileImporta.FileName

//            sEMail = sEMail & vbNewLine & "Processando arquivo " & fileImporta.FileName
            
//            'Busca codigo da empresa
//            vCodigoCustumer = Val(iniEdit(mArquivoCfg, "Lokalizei", "ID_" & Left(fileImporta.FileName, 9), "0", ssIniBuscarCriar))


//            If vCodigoCustumer = 0 Then
//               Log "Erro - Informar o codigo interno lokalizei"
//               Exit Function
//            End If
        
            
//            'inicia leitura dos cabecalhos
//            Open txtLokalizei & fileImporta.FileName For Input As #1
            
//            'inicia laco de busca de caracteristicas
//            intLinha = -1



//            Do
              
              

//              'Busca linha
//              Line Input #1, sLinha
              
//              If Trim(sLinha) = "" Then GoTo proximacarga
              
//              'altera endereco antes do split
//              iInicio = InStr(sLinha, Chr(34))
//              sOrigem = Mid(sLinha, iInicio + 1)
//              sendereco = Left(sOrigem, InStr(sOrigem, Chr(34)) - 1)


//              sTracado = Replace(sendereco, ",", "$$-$$")


//              sLinha = Replace(sLinha, sendereco, sTracado)


//              vCampos = Split(sLinha, ",")


//              vCarga = ""
//              vviagem = ""
//              vPlaca = ""
               
//              'Separando informacoes
//              If InStr(vCampos(1), "[#V") = 0 Then
//                 Log "erro - importar linha " & intLinha & " Arquivo " & fileImporta.FileName
//                 sEMail = sEMail & vbNewLine & "erro - importar linha " & intLinha & " Arquivo " & fileImporta.FileName
//                 GoTo proximacarga
//              End If
//              vviagem = vCampos(1) 'pega descricao
//              vviagem = Mid(vviagem, InStr(vviagem, "[#V") + 3)
//              vviagem = "V" & Left(vviagem, InStr(vviagem, "]") - 1)
              
//              'cnpj
//              sCNPJ = vCampos(1) 'pega descricao
//              sCNPJ = Mid(sCNPJ, InStr(sCNPJ, "[#I") + 3)
//              sCNPJ = Left(sCNPJ, InStr(sCNPJ, "]") - 1)
              
//              'Separando placa
//              vPlaca = vCampos(1) 'pega descricao
//              vPlaca = Mid(vPlaca, InStr(vPlaca, "[#P") + 3)
//              vPlaca = Left(vPlaca, InStr(vPlaca, "]") - 1)
              
//              'Separando Carga
//              vCarga = vCampos(1) 'pega descricao
//              vCarga = Mid(vCarga, InStr(vCarga, "[#C") + 3)
//              vCarga = "C" & Left(vCarga, InStr(vCarga, "]") - 1)
              
//               'ajusta log
//              intLinha = intLinha + 1
//              Log "Importando " & intLinha & " # Carga" & vCarga
//              DoEvents
              
              
//              'Adiciona campo no produto
//              Set rcoViagem = New ADODB.Recordset

//              sSql = "SELECT * FROM lk_driver_task WHERE EDI_TipoTarefa=3 and customer_id=" & vCodigoCustumer & " and EDI_Carga='" & vCarga & "'"
//              rcoViagem.Open sSql, mConexaoLokalizei, adOpenKeyset, adLockOptimistic


//              If rcoViagem.EOF Then
//                 rcoViagem.AddNew
//                 scrc = CrcNet(Format(customer_id, "00000") & vviagem, 8)
//                 rcoViagem!task_token = scrc
//                 rcoViagem!customer_id = vCodigoCustumer
//                 rcoViagem!date_created = Now()
//                 rcoViagem!edi_viagem = vviagem
//                 rcoViagem!edi_carga = vCarga
//                 rcoViagem!edi_tipotarefa = 1
//              Else
//                 scrc = rcoViagem!task_token
//                 rcoViagem!date_modified = Now()
//              End If


//              rcoViagem!edi_CNPJ = sCNPJ




//              rcoViagem!task_description = vCampos(1)
//              rcoViagem!trans_type = "Delivery"  'vCampos(0)
//              rcoViagem!contact_number = vCampos(2)
//              rcoViagem!email_address = vCampos(3)
//              rcoViagem!customer_name = vCampos(4)
//              rcoViagem!delivery_date = IIf(CVDate(vCampos(5)) < Now, DateAdd("n", 60, Now()), vCampos(5))
//              rcoViagem!delivery_address = sendereco
//              rcoViagem!task_lat = vCampos(7)
//              rcoViagem!task_lng = vCampos(8)
//              rcoViagem!ip_address = "0.0.0.0"


//              rcoViagem.Update
              
//              'atualiza pedfacil
//              If sCNPJ <> "" Then
//                 sSql = "Update tb_entidade_empresa"
//                 sSql = sSql & vbCr & "set tb_entidade_empresa.urlMinhasEntregas='http://app.lokalizei.com/track/?id=" & scrc & "'"
//                 sSql = sSql & vbCr & "where tb_entidade_empresa.id_Entidade in (select tb_entidade.idEntidade from tb_entidade where tb_entidade.idEmpresa=5 and tb_entidade.CNPJNumerico='" & sCNPJ & "');"
//                 mConexaoPedFacil.Execute sSql
//              End If


//              rcoViagem.Close

//proximacarga:
//            Loop While Not EOF(1)


//            Close 1
            
//            'renomeia arquivo
//            sArquivo = fileImporta.FileName

//            If Arquivo_Existe(sDiretorioDestino & "\" & Left(sArquivo, Len(sArquivo) - 3) & "pro") Then
//               Kill sDiretorioDestino & "\" & Left(sArquivo, Len(sArquivo) - 3) & "pro"
//            End If


//            Name txtLokalizei & sArquivo As sDiretorioDestino & "\" & Left(sArquivo, Len(sArquivo) - 3) & "pro"



//        Next intI



//        If sTextoEmail <> "" Then
//               sMensagem = "Relação de Viagens Importadas     -  Trade2UP Notifica"
//               sMensagem = sMensagem & vbNewLine & String(60, 45)
//               sMensagem = sMensagem & vbNewLine
//               sMensagem = sMensagem & vbNewLine
//               sMensagem = sMensagem & vbNewLine & "Tipo        NroLinhas             Status"
//               sMensagem = sMensagem & vbNewLine & String(60, 45)
//               sMensagem = sMensagem & sTextoEmail


//               sEMail = "LoKalizei_" & Format(Now, "yyyy\.mm\.dd hh\.hh") & ".txt"
//               Open App.Path & "\" & sEMail For Output As #54
//               Print #54, sMensagem
//               Close #54
               
//             'Inicializa
//               Set mCfgEmail = New as3EmailDados

//               With mCfgEmail
//                    .bHtml = False
//                    .sAnexo = App.Path & "\" & sEMail
//                    .sAssunto = "Trade2UP Integrador - Lokalizei  " & Format(Now, "dd/mm/yy hh:nn")
//                    '.sReply =
//                    .sPara = sDestinatarioLokalizei
//                    .sCC = ""
//                    .sCCO = ""
//                    .sLicenca = "Trade2UP"
//                    .sUsuario = "Trade2UP"
//                    .sObs = ""
//                    .bEnviaPeloSite = False
//                    .bEnvioEspecial = False
//                    .sPortaPOP = 110
//                    .sPortaSTMP = 465
//                    .sTipoAutenticacao = 2
//                    .bUsaSSL = True
//                    .sServidorSMTP = "smtp.gmail.com"
//                    .sBody = "Segue resumo da importação executada em " & Format(Now, "dd/mm/yyyy hh:nn")
//                    .sAutenticacaoSenha = "Trade2UP*2018*19"
//                    .sAutenticacaoUsuario = "service@Trade2UP.com.br"
//                    .sFrom = "service@Trade2UP.com.br"
//                End With
                
//               ' SendAS3 mCfgEmail
//                ''SendMailGMail "service@Trade2UP.com.br", "service@Trade2UP.com.br", "Trade2UP*2018*19", sDestinatarioLokalizei, "Trade2UP Integrador - Pedidos Trade2UP " & Format(Now, "dd/mm/yy hh:nn"), "", sMensagem, ""
             
//                If SendEmail2016(mCfgEmail) Then
//                   Log "email enviado pedidos"
//                Else
//                   Log "falha ao enviar email"
//                End If


//                Kill App.Path & "\" & sEMail
//            End If



//        rotBuscarLokalizei = True
//        mbImportandoArquivos = False


//        mbolProcessoEmExecucao = False
//        Timer1.Enabled = True
//End Function