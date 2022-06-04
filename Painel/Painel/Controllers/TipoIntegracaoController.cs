using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Painel.Models.Contexto;
using Painel.Models.Entidades;

namespace Painel.Controllers
{
    public class TipoIntegracaoController : Controller
    {
        private ViewResult Visualizar(TipoIntegracao TipoIntegracao)
        {
            return View(TipoIntegracao);
        }

        private readonly Contexto _contexto;
        public TipoIntegracaoController(Contexto contexto)
        {
            _contexto = contexto;
        }

        public IActionResult TipoIntegracao()
        {
            var lista = _contexto.TipoIntegracao.ToList();

            return View(lista);
        }
    }
}
