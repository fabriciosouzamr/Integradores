using Painel_Net.Models.Entidades;
using System.Data.Entity;

namespace Painel_Net.Models.Contexto
{
    public class Contexto: DbContext
    {
        public Contexto() : base("name=MyContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Integracao> Integracao { get; set; }
        public DbSet<TipoIntegracao> TipoIntegracao { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<BancoDados> BancoDados { get; set; }
    }
}