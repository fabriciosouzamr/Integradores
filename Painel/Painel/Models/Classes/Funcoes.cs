using Microsoft.AspNetCore.Mvc.Rendering;
using Painel.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class FuncoesLocal
{
    public static List<SelectListItem> FNC_Lista_TipoBancoDados()
    {
        var Itens = new List<SelectListItem>();

        Itens.Add(new SelectListItem { Value = "SSXXX", Text = "Sql Server" });
        Itens.Add(new SelectListItem { Value = "MSPDS", Text = "MySql - PdSuite" });
        Itens.Add(new SelectListItem { Value = "WSFGP", Text = "Flexx Gps" });
        Itens.Add(new SelectListItem { Value = "WSFTL", Text = "Flexx Tools" });

        return Itens;
    }

    public static List<SelectListItem> FNC_Lista_TipoUsuario()
    {
        var Itens = new List<SelectListItem>();

        Itens.Add(new SelectListItem { Value = "0", Text = "trade2up" });
        Itens.Add(new SelectListItem { Value = "1", Text = "Administradores" });
        Itens.Add(new SelectListItem { Value = "2", Text = "Operadores" });
        Itens.Add(new SelectListItem { Value = "5", Text = "Parceiros Administradores" });
        Itens.Add(new SelectListItem { Value = "6", Text = "Parceiros Operadores" });
        Itens.Add(new SelectListItem { Value = "10", Text = "Cliente Admin" });
        Itens.Add(new SelectListItem { Value = "11", Text = "Cliente Operador" });

        Itens.Where(item => item.Value == "whatever");

        return Itens;
    }
}
