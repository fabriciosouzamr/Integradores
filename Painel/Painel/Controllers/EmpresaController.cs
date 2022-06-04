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
    public class EmpresaController : Controller
    {
        private readonly Contexto _contexto;

        public EmpresaController(Contexto contexto)
        {
            _contexto = contexto;
        }

        public IActionResult Empresa()
        {
            var lista = _contexto.Empresa.OrderBy(s => s.Id).Where(s => s.Id > 0).ToList();

            return View(lista);
        }

        [HttpGet]
        public IActionResult Empresa_Criar()
        {
            var Empresa = new Empresa();

            return View(Empresa);
        }

        [HttpPost]
        public IActionResult Empresa_Criar(Empresa Empresa)
        {
            if (ModelState.IsValid)
            {
                int maxId = _contexto.Empresa.Max(p => p.Id);

                Empresa.Id = maxId + 1;
                _contexto.Empresa.Add(Empresa);
                _contexto.SaveChanges();

                return RedirectToAction("Empresa");
            }

            return View(Empresa);
        }

        [HttpGet]
        public IActionResult Empresa_Editar(int Id)
        {
            var Empresa = _contexto.Empresa.Find(Id);

            if (Empresa != null)
            {
                return View(Empresa);
            }

            return View(Empresa);
        }

        [HttpPost]
        public IActionResult Empresa_Editar(Empresa Empresa)
        {
            if (ModelState.IsValid)
            {
                _contexto.Empresa.Update(Empresa);
                _contexto.SaveChanges();

                return RedirectToAction("Empresa");
            }
            else
            {
                return View(Empresa);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Empresa_Deletar(int Id)
        {
            var Empresa = await _contexto.Empresa.FindAsync(Id);

            if (Empresa == null)
            {
                return NotFound();
            }

            return View(Empresa);
        }

        [HttpPost]
        public IActionResult Empresa_Deletar(Empresa _Empresa)
        {
            var Empresa = _contexto.Empresa.Find(_Empresa.Id);
            if (Empresa != null)
            {
                _contexto.Empresa.Remove(Empresa);
                _contexto.SaveChanges();

                return RedirectToAction("Empresa");
            }
            else
            {
                return View(Empresa);
            }
        }

        [HttpGet]
        //public IActionResult Integracao_Detalhe(int Id)
        public IActionResult Empresa_Detalhe(int? id)
        {
            var Empresa = _contexto.Empresa.Find(id); ;

            if (Empresa == null)
            {
                return NotFound();
            }

            return View(Empresa);
        }
    }
}