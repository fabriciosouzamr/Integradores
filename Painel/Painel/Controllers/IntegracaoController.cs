using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Painel.Models.Classes;
using Painel.Models.Contexto;
using Painel.Models.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Painel.Controllers
{
    public class IntegracaoController : Controller
    {
        private readonly Contexto _contexto;

        public IntegracaoController(Contexto contexto)
        {
            _contexto = contexto;
        }

        private ViewResult Visualizar(Integracao Integracao)
        {
            CarregarViewBag();
            return View(Integracao);
        }

        public IActionResult Integracao()
        {
            var Integracao = _contexto.Integracao.Include(e => e.TipoIntegracao).Include(e => e.BancoConexaoOrigem).Include(e => e.BancoConexaoDestino).Include(e => e.Empresa).OrderBy(s => s.Id);
            return View(Integracao.ToList());
        }

        [HttpGet]
        public IActionResult Integracao_Criar()
        {
            var Integracao = new Integracao();

            Integracao.idEmpresa = -1;

            return Visualizar(Integracao);
        }

        [HttpPost]
        public IActionResult Integracao_Criar(Integracao Integracao)
        {
            if (ModelState.IsValid)
            {
                _contexto.Integracao.Add(Integracao);
                _contexto.SaveChanges();

                return RedirectToAction("Integracao");
            }

            return Visualizar(Integracao);
        }

        [HttpGet]
        public IActionResult Integracao_Editar(int Id)
        {
            var Integracao = _contexto.Integracao.Find(Id);

            if (Integracao != null)
            {
                return Visualizar(Integracao);
            }

            return Visualizar(Integracao);
        }

        [HttpPost]
        public IActionResult Integracao_Editar(Integracao Integracao)
        {
            if (ModelState.IsValid)
            {
                _contexto.Integracao.Update(Integracao);
                _contexto.SaveChanges();

                return RedirectToAction("Integracao");
            }
            else
            {
                return Visualizar(Integracao);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Integracao_Deletar(int Id)
        {
            var integracao = await _contexto.Integracao.Include(i => i.BancoConexaoOrigem).Include(i => i.Empresa).Include(i => i.TipoIntegracao).FirstOrDefaultAsync(m => m.Id == Id);

            if (integracao == null)
            {
                return NotFound();
            }

            return View(integracao);
        }

        [HttpPost]
        public IActionResult Integracao_Deletar(Integracao _Integracao)
        {
            var Integracao = _contexto.Integracao.Find(_Integracao.Id);
            if (Integracao != null)
            {
                _contexto.Integracao.Remove(Integracao);
                _contexto.SaveChanges();

                return RedirectToAction("Integracao");
            }
            else
            {
                return View(Integracao);
            }
        }

        [HttpGet]
        //public IActionResult Integracao_Detalhe(int Id)
        public async Task<IActionResult> Integracao_Detalhe(int? id)
        {
            var integracao = await _contexto.Integracao.Include(i => i.BancoConexaoOrigem).Include(i => i.Empresa).Include(i => i.TipoIntegracao).FirstOrDefaultAsync(m => m.Id == id);

            if (integracao == null)
            {
                return NotFound();
            }

            return View(integracao);
        }

        public void CarregarViewBag()
        {
            ViewBag.TipoIntegracao = new SelectList(_contexto.TipoIntegracao, "id_Tipo_Integracao", "no_Tipo_Integracao");
            ViewBag.Empresa = new SelectList(_contexto.Empresa, "Id", "no_Empresa");
            ViewBag.BancoDadosOrigem = new SelectList(_contexto.BancoDados.Where(p => p.tp_entradasaida == "SS" || p.tp_entradasaida == "ES"), "Id", "ds_bancodados");
            ViewBag.BancoDadosDestino = new SelectList(_contexto.BancoDados.Where(p => p.tp_entradasaida == "EE" || p.tp_entradasaida == "ES"), "Id", "ds_bancodados");
        }
    }
}