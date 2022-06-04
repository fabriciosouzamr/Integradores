using Painel_Net.Models.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Painel_Net.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly Contexto _contexto;

        public EmpresaController(Contexto contexto)
        {
            _contexto = contexto;
        }

        public ActionResult Empresa()
        {
            var lista = _contexto.Empresa.OrderBy(s => s.Id).Where(s => s.Id > 0).ToList();

            return View(lista);
        }
    }
}