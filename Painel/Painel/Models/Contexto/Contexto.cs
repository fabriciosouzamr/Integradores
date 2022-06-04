using Microsoft.EntityFrameworkCore;
using Painel.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Painel.Models.Contexto
{
    public class Contexto: DbContext
    {
        public Contexto(DbContextOptions<Contexto> option): base(option)
        {
            //Database.EnsureCreated();
        }

        public DbSet<TipoIntegracao> TipoIntegracao { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<BancoDados> BancoDados { get; set; }
        public DbSet<Integracao> Integracao { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
    }
}
