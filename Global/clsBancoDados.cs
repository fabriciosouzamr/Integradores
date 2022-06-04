using Integradores;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using static BancoDados.clsBancoDados;

public static class BancoDados
{
  public static class BancoDados_Constantes
  {
    public const string const_TipoBancoDados_MySql = "MYSQL";
    public const string const_TipoBancoDados_SqlServer = "SQLSRV";
    public const string const_TipoBancoDados_Firebird = "FIREBIRD";
  }

  public class clsBancoDados
  {
    public object oConexao;

    public string sTipoBancoDados;
    string[] SqlTexte_Executada = new string[0];

    public class clsCampo
    {
      public string Nome { get; set; }
      public DbType Tipo { get; set; }
      public object Valor { get; set; }
      public System.Data.ParameterDirection Direcao { get; set; }
    }

    public class clsCampo_Retorno
    {
      public clsCampo[] ParametroRetorno;

      public clsCampo Campo(string Nome)
      {
        clsCampo oRetorno = null;

        foreach (clsCampo Parametro in ParametroRetorno)
        {
          if (Parametro.Nome == Nome)
          {
            oRetorno = Parametro;
            break;
          }
        }

        return oRetorno;
      }
    }

    public clsCampo_Retorno Retorno
    {
      get
      {
        switch (sTipoBancoDados)
        {
          case BancoDados_Constantes.const_TipoBancoDados_MySql:
            {
              return ((clsMySql)oConexao).Retorno;
            }
          case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
            {
              return ((clsSqlServer)oConexao).Retorno;
            }
          default:
            return null;
        };
      }
    }

    public string DBErro()
    {
      string sRet = "";

      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          {
            sRet = ((clsMySql)oConexao).sErro;
            break;
          }
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          {
            sRet = ((clsSqlServer)oConexao).sErro;
            break;
          }
      }

      return sRet;
    }

    public Boolean DBReconectar(string sTpBancoDados, string sStringConexao)
    {
      DBDesconectar();
      return DBConectar(sTpBancoDados, sStringConexao);
    }

    public Boolean DBConectar(string sTpBancoDados, string sStringConexao)
    {
      bool bOk = false;

      sTipoBancoDados = sTpBancoDados;

      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          {
            oConexao = new clsMySql();

            bOk = ((clsMySql)oConexao).DBConectar(sStringConexao);

            break;
          }
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          {
            oConexao = new clsSqlServer();

            bOk = ((clsSqlServer)oConexao).DBConectar(sStringConexao);

            break;
          }
      }

      return bOk;
    }

    public DateTime DBData()
    {
      DateTime dOk = DateTime.Now;

      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          {
            dOk = Convert.ToDateTime(((clsMySql)oConexao).DBQuery_ValorUnico("select now()"));

            break;
          }
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          {
            dOk = Convert.ToDateTime(((clsSqlServer)oConexao).DBQuery_ValorUnico("select getdate()"));

            break;
          }
      }

      return dOk;
    }
    public string DBSData(int iDias = 0)
    {
      string sRet = "";

      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          {
            if (iDias == 0)
            { sRet = "current_timestamp"; }
            else
            { sRet = "date_add(current_timestamp(), interval " + iDias.ToString() + " day)"; }

            break;
          }
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          {
            if (iDias == 0)
            { sRet = "getdate()"; }
            else
            { sRet = "DATEADD(day, " + iDias.ToString() + ", getdate())"; }

            break;
          }
      }

      return sRet;
    }

    public string DBConnectionString()
    {
      string oRet = null;

      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          {
            oRet = ((clsMySql)oConexao).connNivelAcesso.ConnectionString;

            break;
          }
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          {
            oRet = ((clsSqlServer)oConexao).cnn.ConnectionString;

            break;
          }
      }

      return oRet;
    }

    public string BancoDados(string sSeparador = ".")
    {
      string oRet = null;

      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          {
            oRet = ((clsMySql)oConexao).BancoDados(sSeparador);

            break;
          }
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          {
            oRet = ((clsSqlServer)oConexao).cnn.ConnectionString;

            break;
          }
      }

      return oRet;
    }

    public Boolean DBConectado()
    {
      bool bOk = false;

      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          {
            bOk = ((clsMySql)oConexao).DBConectado();

            break;
          }
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          {
            bOk = ((clsSqlServer)oConexao).DBConectado();

            break;
          }
      }

      return bOk;
    }

    public void DBDesconectar()
    {
      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          {
            ((clsMySql)oConexao).DBDesconectar();

            break;
          }
      }
    }

    public DbCommand DBComando(string sSqlText)
    {
      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          return ((clsMySql)oConexao).DBComando(sSqlText);
        default:
          return null;
      }
    }

    public DataTable DBQuery(String sSqlText,
                                CommandType Tipo = CommandType.Text,
                                clsCampo[] oParametro = null)
    {
      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          sSqlText = sSqlText.Replace("#", "?");
          return ((clsMySql)oConexao).DBQuery(sSqlText, Tipo, (clsMySql.clsMySql_Campo[])DBParametro_Transformar(oParametro));
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          sSqlText = sSqlText.Replace("#", "@");
          return ((clsSqlServer)oConexao).DBQuery(sSqlText, Tipo, (clsSqlServer.clsCampo[])DBParametro_Transformar(oParametro));
        default:
          return null;
      }
    }

    public string DBQuery_Listar(String sSqlText,
                                    CommandType Tipo = CommandType.Text,
                                    clsCampo[] oParametro = null)
    {
      DataTable oData;
      string sRet = "";

      oData = DBQuery(sSqlText, Tipo, oParametro);

      foreach (DataRow oRow in oData.Rows)
      {
        if (oRow[0].GetType().Name.ToUpper() == "STRING")
        {
          _Funcoes.FNC_Str_Adicionar(ref sRet, "'" + oRow[0].ToString().Trim() + "'", ",");
        }
        else
        {
          _Funcoes.FNC_Str_Adicionar(ref sRet, oRow[0].ToString(), ",");
        }
      }

      return sRet;
    }

    public DataRow DBQuery_PrimeiraLinha(String sSqlText)
    {
      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          return ((clsMySql)oConexao).DBQuery_PrimeiraLinha(sSqlText);
        default:
          return null;
      }
    }

    public object DBQuery_ValorUnico(String sSqlText,
                                        object ValorPadrao = null,
                                        object Campos = null,
                                        clsCampo[] oParametro = null)
    {
      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          sSqlText = sSqlText.Replace("#", "?");
          return ((clsMySql)oConexao).DBQuery_ValorUnico(sSqlText, ValorPadrao, Campos, (clsMySql.clsMySql_Campo[])DBParametro_Transformar(oParametro));
        default:
          sSqlText = sSqlText.Replace("#", "@");
          return ((clsSqlServer)oConexao).DBQuery_ValorUnico(sSqlText, ValorPadrao, Campos, (clsSqlServer.clsCampo[])DBParametro_Transformar(oParametro));
      }
    }

    public string[] DBValidarCampos(string Tabela, string[] Campos, ref int iColunasValidas)
    {
      DataTable oData;
      Boolean bAchou = false;

      iColunasValidas = 0;

      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          oData = ((clsSqlServer)oConexao).DBCampos(Tabela);
          break;
        default:
          return null;
      }

      for (int intI = 0; intI < Campos.Length; intI++)
      {
        bAchou = false;

        foreach (DataColumn Coluna in oData.Columns)
        {
          if (Campos[intI].ToString().Trim().ToUpper() == Coluna.ColumnName.Trim().ToUpper())
          {
            iColunasValidas++;
            bAchou = true;
            break;
          }
        }

        if (!bAchou)
          Campos[intI] = "?";

      }

      oData.Dispose();

      return Campos;
    }

    public object DBParametro_Transformar(clsCampo[] oParametro)
    {
      try
      {
        if (oParametro != null)
        {
          switch (sTipoBancoDados)
          {
            case BancoDados_Constantes.const_TipoBancoDados_MySql:
              clsMySql.clsMySql_Campo[] oMySqlParametro_Ret = new clsMySql.clsMySql_Campo[oParametro.Length];

              MySqlDbType oMySqlTipo = MySqlDbType.VarChar;

              for (int intI = 0; intI < oParametro.Length; intI++)
              {
                if (oParametro[intI].Direcao == 0) { oParametro[intI].Direcao = ParameterDirection.Input; };

                oMySqlTipo = MySqlDbType.VarChar;

                if (oParametro[intI].Tipo == DbType.Decimal)
                  oMySqlTipo = MySqlDbType.Decimal;
                else if (oParametro[intI].Tipo == DbType.Int32)
                  oMySqlTipo = MySqlDbType.Int32;
                else if (oParametro[intI].Tipo == DbType.Double)
                  oMySqlTipo = MySqlDbType.Double;
                else if (oParametro[intI].Tipo == DbType.Date)
                  oMySqlTipo = MySqlDbType.Date;
                else if (oParametro[intI].Tipo == DbType.Time)
                  oMySqlTipo = MySqlDbType.Time;
                else if (oParametro[intI].Tipo == DbType.DateTime)
                  oMySqlTipo = MySqlDbType.DateTime;
                else if (oParametro[intI].Tipo == DbType.String)
                  oMySqlTipo = MySqlDbType.VarChar;

                oParametro[intI].Nome = oParametro[intI].Nome.Replace("#", "?");

                oMySqlParametro_Ret[intI] = new clsMySql.clsMySql_Campo
                {
                  Nome = oParametro[intI].Nome,
                  Tipo = oMySqlTipo,
                  Valor = oParametro[intI].Valor,
                  Direcao = oParametro[intI].Direcao
                };
              }

              return oMySqlParametro_Ret;
            case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
              clsSqlServer.clsCampo[] oSqlServerParametro_Ret = new clsSqlServer.clsCampo[oParametro.Length];

              SqlDbType oSqlServerTipo = SqlDbType.VarChar;

              for (int intI = 0; intI < oParametro.Length; intI++)
              {
                if (oParametro[intI].Direcao == 0) { oParametro[intI].Direcao = ParameterDirection.Input; };

                oSqlServerTipo = SqlDbType.VarChar;

                if (oParametro[intI].Tipo == DbType.Decimal)
                  oSqlServerTipo = SqlDbType.Decimal;
                else if (oParametro[intI].Tipo == DbType.Int32)
                  oSqlServerTipo = SqlDbType.Int;
                else if (oParametro[intI].Tipo == DbType.Double)
                  oSqlServerTipo = SqlDbType.Float;
                else if (oParametro[intI].Tipo == DbType.Date)
                  oSqlServerTipo = SqlDbType.Date;
                else if (oParametro[intI].Tipo == DbType.Time)
                  oSqlServerTipo = SqlDbType.Time;
                else if (oParametro[intI].Tipo == DbType.DateTime)
                  oSqlServerTipo = SqlDbType.DateTime;
                else if (oParametro[intI].Tipo == DbType.String)
                  oSqlServerTipo = SqlDbType.VarChar;

                oParametro[intI].Nome = oParametro[intI].Nome.Replace("#", "@");

                oSqlServerParametro_Ret[intI] = new clsSqlServer.clsCampo
                {
                  Nome = oParametro[intI].Nome,
                  Tipo = oSqlServerTipo,
                  Valor = oParametro[intI].Valor,
                  Direcao = oParametro[intI].Direcao
                };
              }

              return oSqlServerParametro_Ret;
            default:
              return null;
          }
        }
        else
          return null;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public Boolean DBExecutar(string sSqlText)
    {
      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          sSqlText = sSqlText.Replace("#", "?");
          return ((clsMySql)oConexao).DBSQL_Executar(sSqlText);
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          sSqlText = sSqlText.Replace("#", "@");
          return ((clsSqlServer)oConexao).DBSQL_Executar(sSqlText);
        default:
          return false;
      }
    }

    public Boolean DBExecutar(string sSqlText,
                              clsCampo[] oParametro)
    {
      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          sSqlText = sSqlText.Replace("#", "?");
          return ((clsMySql)oConexao).DBSQL_Executar(sSqlText, (clsMySql.clsMySql_Campo[])DBParametro_Transformar(oParametro));
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          sSqlText = sSqlText.Replace("#", "@");
          return ((clsSqlServer)oConexao).DBSQL_Executar(sSqlText, (clsSqlServer.clsCampo[])DBParametro_Transformar(oParametro));
        default:
          return false;
      }
    }

    public Boolean DBProcedure(string sTexto,
                               clsCampo[] oParametro = null)
    {
      switch (sTipoBancoDados)
      {
        case BancoDados_Constantes.const_TipoBancoDados_MySql:
          return ((clsMySql)oConexao).DBSQL_Procedure(sTexto, (clsMySql.clsMySql_Campo[])DBParametro_Transformar(oParametro));
        case BancoDados_Constantes.const_TipoBancoDados_SqlServer:
          return ((clsSqlServer)oConexao).DBSQL_Procedure(sTexto, (clsSqlServer.clsCampo[])DBParametro_Transformar(oParametro));
        default:
          return false;
      }
    }

    public Boolean DBSQL_Integrador_Log_Gravar(string sAplicativo,
                                               string sPartner,
                                               string sCOD_PUXADA,
                                               string sTIPO_REGISTRO,
                                               string sTIPO_CONSULTA,
                                               string sCodigoConexaoOrigem,
                                               string sCdServico,
                                               int iId_Empresa,
                                               int iId_Tarefa,
                                               string sDs_log)
    {
      Boolean bOk = false;

      return bOk;
    }

    public Boolean DBSQL_Log_Gravar(string sAplicativo,
                                    string sPartner,
                                    string sCOD_PUXADA,
                                    string sTIPO_REGISTRO,
                                    string sTIPO_CONSULTA,
                                    string sCodigoConexaoOrigem,
                                    string sCdServico,
                                    int iId_Empresa,
                                    LogTipo idOperacao,
                                    long iIdTarefa,
                                    string sDs_log)
    {
      Boolean bOk = false;

      try
      {
        Integrador_Funcoes.oBancoDados.DBProcedure("sp_integrador_log_ins", new clsCampo[] {
                                                                            new clsCampo {Nome = "Hostname", Tipo = DbType.String, Valor = Config_App.sProcessador},
                                                                            new clsCampo {Nome = "Tipo", Tipo = DbType.Int16, Valor = idOperacao.GetHashCode() },
                                                                            new clsCampo {Nome = "id_empresa", Tipo = DbType.Int16, Valor = iId_Empresa },
                                                                            new clsCampo {Nome = "idTarefa", Tipo = DbType.String, Valor = iIdTarefa },
                                                                            new clsCampo {Nome = "ds_log", Tipo = DbType.String, Valor = sDs_log }});

        if (sAplicativo != "")
        {
          Integrador_Funcoes.Processar_Sincronizacao(sAplicativo, sPartner, "ERR", sCOD_PUXADA, "ErroExecucao", sDs_log, "IFP",
                                                     Integrador_Funcoes.TempoExecucaoAPI.ToString(),
                                                     Integrador_Funcoes.TempoExecucaoIntegrador.ToString(),
                                                     sTIPO_REGISTRO, sTIPO_CONSULTA, sCodigoConexaoOrigem, sCdServico);
        }

        bOk = true;
      }
      catch (Exception Ex)
      {
        //tr.Rollback();
        //DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message, "");
      }

      return bOk;
    }
  }

  class clsSqlServer
  {
    public class clsCampo
    {
      public string Nome { get; set; }
      public SqlDbType Tipo { get; set; }
      public object Valor { get; set; }
      public ParameterDirection Direcao { get; set; }
    }

    public clsCampo_Retorno Retorno;
    public long UltimoIdInserido;
    public string sErro = "";

    public SqlConnection cnn;

    public Boolean DBConectar(string sConnetionString = null)
    {
      Boolean bOk = false;

      cnn = new SqlConnection(sConnetionString);

      try
      {
        cnn.Open();
        bOk = true;
      }
      catch (Exception ex)
      {
        sErro = ex.Message;
      }

      return bOk;
    }

    public Boolean DBConectado()
    {
      try
      {
        return (cnn.State == System.Data.ConnectionState.Open);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public DataTable DBCampos(string sTabela)
    {
      sTabela = "SELECT TOP 0 * FROM " + sTabela;

      return DBQuery(sTabela);
    }

    public SqlCommand DBComando(string sSqlText)
    {
      return new SqlCommand(sSqlText, cnn);
    }

    public int DBRetornarInt(string sSqlText)
    {
      int ID = 0;

      SqlCommand cmd = new SqlCommand(sSqlText, cnn);

      //instância do leitor 
      SqlDataReader ret = cmd.ExecuteReader();

      //enquanto leitor lê 
      while (ret.Read())
      {
        ID = Convert.ToInt32(ret[0].ToString());
      }

      ret.Close();
      ret = null;

      return ID;
    }

    public DataTable DBQuery(string sSqlText,
                             CommandType Tipo = CommandType.Text,
                             clsCampo[] oParametro = null)
    {
      DataTable oData = new DataTable();

      cnn.Close();
      cnn.Open();

      try
      {
        bool bRetornaValor = false;
        SqlCommand oCmd = new SqlCommand(sSqlText, cnn);
        oCmd.CommandTimeout = 0;

        try
        {
          DBParametro_Montar(ref oCmd, ref bRetornaValor, oParametro);
        }
        catch (Exception Ex)
        {
          Config.appLog_Escrever("Erro na rotina DBQuery.DBParametro_Montar (SqlServer)." + Ex.Message);
        }

        //instância do leitor 
        oCmd.CommandType = Tipo;
        SqlDataReader oReader = oCmd.ExecuteReader();

        oData.Load(oReader);

        oReader.Close();
        oReader = null;
      }
      catch (Exception Ex)
      {
        Config.appLog_Escrever("Erro na rotina DBQuery (SqlServer)." + Ex.Message);
      }

      return oData;
    }

    public object DBQuery_ValorUnico(String sSqlText,
                                        object ValorPadrao = null,
                                        object Campos = null,
                                        clsCampo[] oParametro = null)
    {
      DataTable oRet = null;
      object iRet = 0;

      try
      {
        oRet = DBQuery(sSqlText, CommandType.Text, oParametro);

        if (oRet.Rows.Count > 0)
        {
          if (Campos == null)
          {
            iRet = oRet.Rows[0][Convert.ToInt32(Campos)];
          }
        }
        else
        {
          iRet = ValorPadrao;
        }
      }
      catch (MySqlException ex)
      {
        //Erro_Setar(ex.Message);
      }

      return iRet;
    }

    public Boolean DBSQL_Executar(string sTexto,
                                    clsCampo[] oParametro = null)
    {
      return DBSQL_Executar(sTexto, System.Data.CommandType.Text, oParametro);
    }

    public Boolean DBSQL_Procedure(string sTexto,
                                    clsCampo[] oParametro = null)
    {
      return DBSQL_Executar(sTexto, System.Data.CommandType.StoredProcedure, oParametro);
    }

    private Boolean DBSQL_Executar(string sTexto,
                                    System.Data.CommandType osCommandType,
                                    clsCampo[] oParametro = null)
    {
      Boolean bOk = false;
      Boolean bRetornaValor = false;

      cnn.Close();
      cnn.Open();

      try
      {
        //Basic command and connection initialization 
        SqlCommand cmd = new SqlCommand(sTexto, cnn);
        cmd.CommandType = osCommandType;
        cmd.CommandTimeout = 0;

        DBParametro_Montar(ref cmd, ref bRetornaValor, oParametro);

        //Open connection and Execute 
        if (bRetornaValor)
        {
          Retorno = new clsCampo_Retorno();
          Retorno.ParametroRetorno = new clsBancoDados.clsCampo[cmd.Parameters.Count];
          int iCont = 0;

          cmd.ExecuteReader();

          foreach (SqlParameter Parametro in cmd.Parameters)
          {
            Retorno.ParametroRetorno[iCont] = new clsBancoDados.clsCampo();
            Retorno.ParametroRetorno[iCont].Nome = "";

            if (Parametro.Direction == ParameterDirection.InputOutput || Parametro.Direction == ParameterDirection.Output)
            {
              Retorno.ParametroRetorno[iCont].Nome = Parametro.ParameterName.Substring(1);
              Retorno.ParametroRetorno[iCont].Valor = Parametro.Value;
            }
            iCont++;
          }

          cmd.Dispose();
        }
        else
        {
          cmd.ExecuteNonQuery();
          //UltimoIdInserido = cmd.;
        }

        bOk = true;
      }
      catch (Exception Ex)
      {
        sErro = "DBSQL_Executar [SqlServer] - (" + sTexto + ") " + Ex.Message;
        Config.appLog_Escrever(sErro);

        bOk = false;
      }

      return bOk;
    }

    private void DBParametro_Montar(ref SqlCommand cmd,
                                    ref bool bRetornaValor,
                                    clsCampo[] oParametro = null)
    {
      if (oParametro != null)
      {
        foreach (clsCampo Parametro in oParametro)
        {
          if (Parametro.Direcao == 0)
            Parametro.Direcao = ParameterDirection.Input;

          if (Parametro.Direcao == ParameterDirection.InputOutput || Parametro.Direcao == ParameterDirection.Output)
            bRetornaValor = true;

          DBParametro(cmd, "@" + Parametro.Nome, Parametro.Valor, Parametro.Tipo, Parametro.Direcao);
        }
      }
    }

    private void DBParametro(SqlCommand cmd,
                                string sNome,
                                object Valor,
                                SqlDbType oTipo = SqlDbType.VarChar,
                                ParameterDirection oDirecao = ParameterDirection.Input)
    {
      cmd.Parameters.Add(new SqlParameter(sNome, oTipo));
      cmd.Parameters[sNome].Direction = oDirecao;
      cmd.Parameters[sNome].Value = Valor;
      if (Valor == null)
      {
        cmd.Parameters[sNome].Size = 100;
      }
      else
      {
        if (oTipo == SqlDbType.VarChar) { cmd.Parameters[sNome].Size = Valor.ToString().Length; } else { cmd.Parameters[sNome].Size = 100; }
      }
    }

    public static explicit operator clsSqlServer(clsBancoDados v)
    {
      throw new NotImplementedException();
    }
  }

  class clsMySql
  {
    public class clsMySql_Campo
    {
      public string Nome { get; set; }
      public MySqlDbType Tipo { get; set; }
      public object Valor { get; set; }
      public ParameterDirection Direcao { get; set; }
    }

    //public HttpResponse oResponse;
    public Boolean bErro;
    public string sErro;
    public string sBancoDados = "";
    public clsCampo_Retorno Retorno;
    public long UltimoIdInserido;

    public MySqlConnection connNivelAcesso = new MySqlConnection();

    public string BancoDados(string sSeparador = ".")
    {
      if (connNivelAcesso.Database.Trim() == "")
      { return connNivelAcesso.Database; }
      else
      { return connNivelAcesso.Database + sSeparador; }
    }

    public Boolean DBConectado()
    {
      try
      {
        return (connNivelAcesso.State == System.Data.ConnectionState.Open);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public Boolean DBConectar(string sStringConexao = "")
    {
      Boolean bOk = false;

      Erro_Limpar();

      try
      {
        if (sStringConexao.Trim() != "")
          connNivelAcesso.ConnectionString = sStringConexao;

        connNivelAcesso.Open();

        bOk = true;
      }
      catch (Exception ex)
      {
        DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, ex.Message, "");
        Erro_Setar(ex.Message);

        bOk = false;
      }

      return bOk;
    }

    public void DBDesconectar()
    {
      try
      {
        connNivelAcesso.Close();
      }
      catch (Exception)
      {
      }
    }

    public DataTable DBQuery(String sSqlText,
                                CommandType Tipo = CommandType.Text,
                                clsMySql_Campo[] oParametro = null)
    {
      DataTable oRet = new DataTable();
      bool bRetornaValor = false;

      Erro_Limpar();

      try
      {
        if (connNivelAcesso.State != ConnectionState.Open)
          DBConectar();

        MySqlCommand cmd = new MySqlCommand(sSqlText, connNivelAcesso);
        cmd.CommandType = Tipo;

        try
        {
          DBParametro_Montar(ref cmd, ref bRetornaValor, oParametro);
        }
        catch (Exception)
        {
        }

        MySqlDataReader reader = cmd.ExecuteReader();

        oRet.Load(reader);

        reader.Close();
        reader.Dispose();
      }
      catch (MySqlException ex)
      {
        Erro_Setar(ex.Message);
        Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar("", "", "", "", "", "", "", 0, LogTipo.ErroNoBancoDados, 0, ex.Message);
      }

      return oRet;
    }

    public DataRow DBQuery_PrimeiraLinha(String sSqlText)
    {
      DataTable oData = new DataTable();
      DataRow oRow;

      oData = DBQuery(sSqlText);

      oRow = oData.Rows[0];

      return oRow;
    }

    public object DBQuery_ValorUnico(String sSqlText,
                                        object ValorPadrao = null,
                                        object Campos = null,
                                        clsMySql_Campo[] oParametro = null)
    {
      DataTable oRet = null;
      object iRet = 0;

      Erro_Limpar();

      try
      {
        oRet = DBQuery(sSqlText, CommandType.Text, oParametro);

        if (oRet.Rows.Count > 0)
        {
          if (Campos == null)
          {
            iRet = oRet.Rows[0][Convert.ToInt32(Campos)];
          }
        }
        else
        {
          iRet = ValorPadrao;
        }
      }
      catch (MySqlException ex)
      {
        Erro_Setar(ex.Message);
      }

      return iRet;
    }

    public string DBTabela(string sNome)
    {
      string sRet;

      sNome = sNome.ToLower().ToString();

      if (sBancoDados.Trim() == "")
      {
        sRet = sNome;
      }
      else
      {
        sRet = sBancoDados.Trim() + "." + sNome.Trim();
      }

      return sRet;
    }

    private void DBParametro_Montar(ref MySqlCommand cmd,
                                    ref bool bRetornaValor,
                                    clsMySql_Campo[] oParametro = null)
    {
      if (oParametro != null)
      {
        foreach (clsMySql_Campo Parametro in oParametro)
        {
          if (Parametro.Direcao == 0)
            Parametro.Direcao = ParameterDirection.Input;

          if (Parametro.Direcao == ParameterDirection.InputOutput || Parametro.Direcao == ParameterDirection.Output)
            bRetornaValor = true;

          DBParametro(ref cmd, "?" + Parametro.Nome, Parametro.Valor, Parametro.Tipo, Parametro.Direcao);
        }
      }
    }

    private void DBParametro(ref MySqlCommand cmd,
                                string sNome,
                                object Valor,
                                MySqlDbType oTipo = MySqlDbType.VarChar,
                                ParameterDirection oDirecao = ParameterDirection.Input)
    {
      cmd.Parameters.Add(new MySqlParameter(sNome, oTipo));
      cmd.Parameters[sNome].Direction = oDirecao;
      cmd.Parameters[sNome].Value = Valor;
    }

    private void Erro_Limpar()
    {
      //bErro = false;
      //sErro = "";
    }

    private void Erro_Setar(string sMensagem)
    {
      bErro = true;
      sErro = sMensagem;
    }

    public MySqlCommand DBComando(string sSqlText)
    {
      return new MySqlCommand(sSqlText, connNivelAcesso);
    }

    public Boolean DBSQL_Executar(string sTexto,
                                    clsMySql_Campo[] oParametro = null)
    {
      return DBSQL_Executar(sTexto, System.Data.CommandType.Text, oParametro);
    }

    public Boolean DBSQL_Procedure(string sTexto,
                                    clsMySql_Campo[] oParametro = null)
    {
      return DBSQL_Executar(sTexto, System.Data.CommandType.StoredProcedure, oParametro);
    }

    private Boolean DBSQL_Executar(string sTexto,
                                    System.Data.CommandType osCommandType,
                                    clsMySql_Campo[] oParametro = null)
    {
      Boolean bOk = false;
      Boolean bRetornaValor = false;

      MySqlCommand cmd = new MySqlCommand(sTexto, connNivelAcesso);

      try
      {
        if (connNivelAcesso.State == ConnectionState.Closed)
        {
          connNivelAcesso.Open();
        }

        //Basic command and connection initialization 
        cmd.CommandTimeout = 0;
        cmd.CommandType = osCommandType;

        DBParametro_Montar(ref cmd, ref bRetornaValor, oParametro);

        //Open connection and Execute 
        if (bRetornaValor)
        {
          Retorno = new clsCampo_Retorno();
          Retorno.ParametroRetorno = new clsCampo[cmd.Parameters.Count];
          int iCont = 0;

          cmd.ExecuteReader();

          foreach (MySqlParameter Parametro in cmd.Parameters)
          {
            Retorno.ParametroRetorno[iCont] = new clsCampo();
            Retorno.ParametroRetorno[iCont].Nome = "";

            if (Parametro.Direction == ParameterDirection.InputOutput || Parametro.Direction == ParameterDirection.Output)
            {
              Retorno.ParametroRetorno[iCont].Nome = Parametro.ParameterName.Substring(1);
              Retorno.ParametroRetorno[iCont].Valor = Parametro.Value;
            }
            iCont++;
          }

          cmd.Dispose();
        }
        else
        {
          cmd.ExecuteNonQuery();
          //////UltimoIdInserido = cmd.LastInsertedId;
        }

        //bOk = true;
      }
      catch (Exception Ex)
      {
        if ((Ex.Message == "Connection must be valid and open.") || 
            (Ex.Message == "Fatal error encountered during command execution.") || 
            (Ex.Message == "Lock wait timeout exceeded; try restarting transaction") ||
            (Ex.Message == "Deadlock found when trying to get lock; try restarting transaction"))
        {
          try
          {
            connNivelAcesso.Close();
            connNivelAcesso.Open();
            cmd.ExecuteNonQuery();
          }
          catch (Exception)
          {
          }
        }
        else
        {
          Erro_Setar(Ex.Message);
          sErro = "DBSQL_Executar [MySql] - (" + sTexto + ") " + Ex.Message;
          Integrador_Funcoes.oBancoDados.DBSQL_Log_Gravar("", "", "", "", "", "", "", 0, LogTipo.ErroNoBancoDados, 0, Ex.Message);
        }
      }

      return bOk;
    }

    public Boolean DBSQL_Tarefas_AutoAgendar_Gravar(int iId_Tarefa)
    {
      Boolean bOk = false;

      try
      {
        MySqlTransaction tr = connNivelAcesso.BeginTransaction();

        //Basic command and connection initialization 
        MySqlCommand cmd = new MySqlCommand("sp_gertarefas_autoagendar_cad", connNivelAcesso);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Transaction = tr;

        DBParametro(ref cmd, "?p_idTarefa", iId_Tarefa, MySqlDbType.Int32);

        //Open connection and Execute 
        cmd.ExecuteNonQuery();

        tr.Commit();

        bOk = true;
      }
      catch (Exception Ex)
      {
        DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, iId_Tarefa, Ex.Message, "");
      }

      return bOk;
    }

    public Boolean DBSQL_Integrador_Log_Gravar(int iId_Empresa,
                                                int iId_Tarefa,
                                                string sDs_log)
    {
      Boolean bOk = false;

      try
      {
        MySqlTransaction tr = connNivelAcesso.BeginTransaction();

        //Basic command and connection initialization 
        MySqlCommand cmd = new MySqlCommand("sp_integrador_log_cad", connNivelAcesso);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Transaction = tr;

        DBParametro(ref cmd, "?p_id_empresa", iId_Empresa, MySqlDbType.Int32);
        DBParametro(ref cmd, "?p_idTarefa", iId_Tarefa, MySqlDbType.Int32);
        DBParametro(ref cmd, "?p_ds_log", sDs_log, MySqlDbType.VarChar);

        //Open connection and Execute 
        cmd.ExecuteNonQuery();

        tr.Commit();

        bOk = true;
      }
      catch (Exception Ex)
      {
        DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, iId_Tarefa, Ex.Message, "");
      }

      return bOk;
    }

    public Boolean DBSQL_Log_Gravar(int iId_Empresa,
                                    LogTipo idOperacao,
                                    int iIdRegistro,
                                    string sDs_log,
                                    string sNomeArquivo)
    {
      Boolean bOk = false;

      //MySqlTransaction tr = connNivelAcesso.BeginTransaction();

      try
      {
        string sSql = "INSERT INTO tb_log(idEmpresa,idEntidade,idUsuario,idDispositivo,idOperacao,idRegistro,idRotina," +
                                            "Latitude,Longitude,ComplementoLog,ComplementoAdicional,DataLog)" +
                        " VALUES(?idEmpresa,?idEntidade,?idUsuario,?idDispositivo,?idOperacao,?idRegistro,?idRotina," +
                                "?Latitude,?Longitude,?ComplementoLog,?ComplementoAdicional,Now())";

        //Basic command and connection initialization 
        MySqlCommand cmd = new MySqlCommand(sSql, connNivelAcesso);
        cmd.CommandType = System.Data.CommandType.Text;
        //cmd.Transaction = tr;

        DBParametro(ref cmd, "?idEmpresa", iId_Empresa, MySqlDbType.Int32);
        DBParametro(ref cmd, "?idEntidade", 0, MySqlDbType.Int32);
        DBParametro(ref cmd, "?idUsuario", 0, MySqlDbType.Int32);
        DBParametro(ref cmd, "?idDispositivo", 0, MySqlDbType.Int32);
        DBParametro(ref cmd, "?idOperacao", idOperacao, MySqlDbType.Int32);
        DBParametro(ref cmd, "?idRegistro", iIdRegistro, MySqlDbType.Int32);
        DBParametro(ref cmd, "?idRotina", 500, MySqlDbType.Int32);
        DBParametro(ref cmd, "?Latitude", 0, MySqlDbType.Int32);
        DBParametro(ref cmd, "?Longitude", 0, MySqlDbType.Int32);
        DBParametro(ref cmd, "?ComplementoLog", sDs_log, MySqlDbType.VarChar);
        DBParametro(ref cmd, "?ComplementoAdicional", sNomeArquivo, MySqlDbType.VarChar);

        //Open connection and Execute 
        cmd.ExecuteNonQuery();

        //tr.Commit();

        bOk = true;
      }
      catch (Exception Ex)
      {
        //tr.Rollback();
        //DBSQL_Log_Gravar(0, LogTipo.ErroNoBancoDados, 0, Ex.Message, "");
      }

      return bOk;
    }

    public string DataMySql(DateTime Data)
    {
      try
      {
        return Data.ToString("yyyy-MM-dd HH:mm:ss");
      }
      catch (Exception)
      {

        return null;
      }
    }

    public DateTime DBStringToData(string sData, string sHora = "")
    {
      DateTime dData = new DateTime();

      try
      {
        string sDia = sData.Substring(0, sData.IndexOf("/"));
        string sMes = sData.Substring(sData.IndexOf("/") + 1, sData.IndexOf("/", sData.IndexOf("/") + 1) - sData.IndexOf("/") - 1);
        string sAno = sData.Substring(sData.IndexOf("/", sData.IndexOf("/") + 1) + 1);

        dData = dData.AddDays(Convert.ToInt32(sDia) - 1);
        dData = dData.AddMonths(Convert.ToInt32(sMes) - 1);
        dData = dData.AddYears(Convert.ToInt32(sAno) - 1);

        if (sHora.Trim() != "")
        {
          dData = dData.AddHours(Convert.ToInt32(sHora.Substring(0, 2)));
          dData = dData.AddMinutes(Convert.ToInt32(sHora.Substring(3, 2)));
        }
      }
      catch (Exception ex)
      {
      }

      return dData;
    }

    public static string DBDataToString(DateTime dData)
    {
      string sData = "";

      sData = _Funcoes.FNC_ZerosEsquerda(dData.Day.ToString().Trim(), 2) + "/" +
              _Funcoes.FNC_ZerosEsquerda(dData.Month.ToString().Trim(), 2) + "/" +
              _Funcoes.FNC_ZerosEsquerda(dData.Year.ToString().Trim(), 4);

      return sData;
    }

    public static string DBDataToStringDB(string sData)
    {
      string sDataRet = "";

      try
      {
        sDataRet = sData.Substring(6, 4) + "-" + sData.Substring(3, 2) + "-" + sData.Substring(0, 2);
      }
      catch (Exception)
      {
      }

      return sDataRet;
    }
  }
}