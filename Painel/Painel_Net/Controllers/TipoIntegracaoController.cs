using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Painel_Net.Models.Contexto;
using Painel_Net.Models.Entidades;

namespace Painel_Net.Controllers
{
    public class TipoIntegracaoController : Controller
    {
        private Contexto db = new Contexto();

        // GET: TipoIntegracao
        public ActionResult Index()
        {
            return View(db.TipoIntegracao.ToList());
        }

        // GET: TipoIntegracao/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoIntegracao tipoIntegracao = db.TipoIntegracao.Find(id);
            if (tipoIntegracao == null)
            {
                return HttpNotFound();
            }
            return View(tipoIntegracao);
        }

        // GET: TipoIntegracao/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoIntegracao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_Tipo_Integracao,no_Tipo_Integracao")] TipoIntegracao tipoIntegracao)
        {
            if (ModelState.IsValid)
            {
                db.TipoIntegracao.Add(tipoIntegracao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoIntegracao);
        }

        // GET: TipoIntegracao/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoIntegracao tipoIntegracao = db.TipoIntegracao.Find(id);
            if (tipoIntegracao == null)
            {
                return HttpNotFound();
            }
            return View(tipoIntegracao);
        }

        // POST: TipoIntegracao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_Tipo_Integracao,no_Tipo_Integracao")] TipoIntegracao tipoIntegracao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoIntegracao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoIntegracao);
        }

        // GET: TipoIntegracao/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoIntegracao tipoIntegracao = db.TipoIntegracao.Find(id);
            if (tipoIntegracao == null)
            {
                return HttpNotFound();
            }
            return View(tipoIntegracao);
        }

        // POST: TipoIntegracao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoIntegracao tipoIntegracao = db.TipoIntegracao.Find(id);
            db.TipoIntegracao.Remove(tipoIntegracao);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
