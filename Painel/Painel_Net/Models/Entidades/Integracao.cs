using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Painel_Net.Models.Entidades
{
    [Table("tb_empresas_integracao")]
    public class Integracao
    {
        [Column("idEmpresaIntegracao"), Display(Description = "Código")]
        public int Id { get; set; }

        [Display(Description = "Tipo de Integração")]
        public int idTipoIntegracao { get; set; }

        [Display(Description = "Empresa")]
        public int idEmpresa { get; set; }

        [Display(Description = "Banco de Dados")]
        public int idBancoConexao { get; set; }

        [ForeignKey("idEmpresa"), Display(Description = "Empresa")]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey("idBancoConexao"), Display(Description = "Banco de Dados")]
        public virtual BancoDados BancoConexao { get; set; }

        [ForeignKey("idTipoIntegracao")]
        public virtual TipoIntegracao TipoIntegracao { get; set; }
    }
}