using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Painel_Net.Models.Entidades
{
    [Table("tb_empresas")]
    public class Empresa
    {
        [Column("idEmpresa"), Display(Name = "Código")]
        public int Id { get; set; }

        [Column("Empresa"), Display(Name = "Empresa")]
        public string no_Empresa { get; set; }
    }
}