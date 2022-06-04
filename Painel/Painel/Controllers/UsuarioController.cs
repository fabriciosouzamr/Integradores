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
    public class UsuarioController : Controller
    {
        private readonly Contexto _contexto;

        public UsuarioController(Contexto contexto)
        {
            _contexto = contexto;
        }

        private ViewResult Visualizar(Usuario Usuario)
        {
            CarregarViewBag();
            return View(Usuario);
        }

        public IActionResult Usuario()
        {
            var Usuario = _contexto.Usuario.OrderBy(s => s.no_usuario);
            CarregarViewBag();
            return View(Usuario.ToList());
        }

        [HttpGet]
        public IActionResult Usuario_Criar()
        {
            var Usuario = new Usuario();

            return Visualizar(Usuario);
        }

        [HttpPost]
        public IActionResult Usuario_Criar(Usuario Usuario)
        {
            if (ModelState.IsValid)
            {
                _contexto.Usuario.Add(Usuario);
                _contexto.SaveChanges();

                return RedirectToAction("Usuario");
            }

            return Visualizar(Usuario);
        }

        [HttpGet]
        public IActionResult Usuario_Editar(int Id)
        {
            var Usuario = _contexto.Usuario.Find(Id);

            if (Usuario != null)
            {
                return Visualizar(Usuario);
            }

            return Visualizar(Usuario);
        }

        [HttpPost]
        public IActionResult Usuario_Editar(Usuario Usuario)
        {
            if (ModelState.IsValid)
            {
                _contexto.Usuario.Update(Usuario);
                _contexto.SaveChanges();

                return RedirectToAction("Usuario");
            }
            else
            {
                return Visualizar(Usuario);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Usuario_Deletar(int Id)
        {
            var Usuario = await _contexto.Usuario.FirstOrDefaultAsync(m => m.Id == Id);

            if (Usuario == null)
            {
                return NotFound();
            }

            return View(Usuario);
        }

        [HttpPost]
        public IActionResult Usuario_Deletar(Usuario _Usuario)
        {
            var Usuario = _contexto.Usuario.Find(_Usuario.Id);
            if (Usuario != null)
            {
                _contexto.Usuario.Remove(Usuario);
                _contexto.SaveChanges();

                return RedirectToAction("Usuario");
            }
            else
            {
                return View(Usuario);
            }
        }

        [HttpGet]
        //public IActionResult Integracao_Detalhe(int Id)
        public async Task<IActionResult> Usuario_Detalhe(int? id)
        {
            var Usuario = await _contexto.Usuario.FirstOrDefaultAsync(m => m.Id == id);

            if (Usuario == null)
            {
                return NotFound();
            }

            return View(Usuario);
        }

        public void CarregarViewBag()
        {
            ViewBag.TipoUsuario = FuncoesLocal.FNC_Lista_TipoUsuario();
        }
    }
}