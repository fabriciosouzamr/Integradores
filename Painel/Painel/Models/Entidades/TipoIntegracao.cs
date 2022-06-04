using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Painel.Models.Entidades
{
    [Table("vw_tabelasgerais_Tipo_Integracao")]
    public class TipoIntegracao
    {
        [Key]
        [Display(Name = "Código")]
        public int id_Tipo_Integracao { get; set; }

        [Display(Name = "Tipo de Integração")]
        [Column("no_Tipo_Integracao")]
        public string no_Tipo_Integracao { get; set; }
    }
}
