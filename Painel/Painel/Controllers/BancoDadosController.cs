using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Painel.Models.Contexto;
using Painel.Models.Entidades;

namespace Painel.Controllers
{
    public class BancoDadosController : Controller
    {
        private readonly Contexto _contexto;

        public BancoDadosController(Contexto contexto)
        {
            _contexto = contexto;
        }

        public IActionResult BancoDados()
        {
            var lista = _contexto.BancoDados.OrderBy(s => s.Id).ToList();

            return View(lista);
        }
    }
}