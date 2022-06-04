using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Painel.Models.Entidades
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

        [Display(Description = "Banco de Dados de Origem")]
        public int idBancoConexaoOrigem { get; set; }

        [Display(Description = "Banco de Dados de Destino")]
        public int idBancoConexaoDestino { get; set; }

        [ForeignKey("idEmpresa"), Display(Description = "Empresa")]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey("idBancoConexaoOrigem"), Display(Description = "Banco de Dados de Origem")]
        public virtual BancoDados BancoConexaoOrigem { get; set; }

        [ForeignKey("idBancoConexaoDestino"), Display(Description = "Banco de Dados de Destino")]
        public virtual BancoDados BancoConexaoDestino { get; set; }

        [ForeignKey("idTipoIntegracao")]
        public virtual TipoIntegracao TipoIntegracao { get; set; }
    }
}
