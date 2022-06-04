using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Painel.Models.Entidades
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

        [Column("tp_entradasaida"), Display(Name = "Entrada/Saída")]
        public string tp_entradasaida { get; set; }

        public string no_bancodados
        {
            get
            {
                return FuncoesLocal.FNC_Lista_TipoBancoDados().Where(p => p.Value == tp_bancodados).First().Text;
            }
        }
    }
}
