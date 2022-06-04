using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Painel_Net.Models.Entidades
{
    [Table("tb_bancodados")]
    public class BancoDados
    {
        [Column("id_bancodados"), Display(Name = "Código")]
        public int Id { get; set; }

        [Column("ds_bancodados"), Display(Name = "Banco de Dados")]
        public string ds_bancodados { get; set; }

        [Column("tp_bancodados"), Display(Name = "Tipo de Banco de Dados")]
        public string tp_bancodados { get; set; }

        [Column("ds_stringconexao"), Display(Name = "String de conexão")]
        public string ds_stringconexao { get; set; }
    }
}