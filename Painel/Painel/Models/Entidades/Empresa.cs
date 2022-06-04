using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Painel.Models.Entidades
{
    [Table("tb_empresas")]
    public class Empresa
    {
        [Column("idEmpresa"), Display(Name = "Código")]
        public int Id { get; set; }

        [Column("CodEmpresa"), Display(Name = "Cód. Empresa")]
        public string CodEmpresa { get; set; }

        [Column("Empresa"), Display(Name = "Empresa")]
        public string no_Empresa { get; set; }

        [Column("TokenFlexXGps"), Display(Name = "Token Flex X Gps")]
        public string TokenFlexXGps { get; set; }
    }
}
