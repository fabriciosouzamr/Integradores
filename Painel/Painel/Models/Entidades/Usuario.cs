using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Painel.Models.Entidades
{
    [Table("tb_integrador_usuario")]
    public class Usuario
    {
        [Column("id_usuario"), Display(Name = "Código")]
        public int Id { get; set; }

        [Column("cd_usuario"), Display(Name = "Código do Usuário")]
        public string cd_usuario { get; set; }

        [Column("no_usuario"), Display(Name = "Nome do Usuário")]
        public string no_usuario { get; set; }

        [Column("tp_tipousuario"), Display(Name = "Tipo de Usuário")]
        public string tp_tipousuario { get; set; }

        public string no_tipousuario
        {
            get
            {
                return FuncoesLocal.FNC_Lista_TipoBancoDados().Where(p => p.Value == tp_tipousuario).First().Text;
            }
        }
    }
}
