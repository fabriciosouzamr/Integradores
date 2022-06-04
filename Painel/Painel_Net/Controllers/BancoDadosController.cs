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
    public class BancoDadosController : Controller
    {
        private Contexto db = new Contexto();

        // GET: BancoDados
        public ActionResult Index()
        {
            return View(db.BancoDados.ToList());
        }

        // GET: BancoDados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BancoDados bancoDados = db.BancoDados.Find(id);
            if (bancoDados == null)
            {
                return HttpNotFound();
            }
            return View(bancoDados);
        }

        // GET: BancoDados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BancoDados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ds_bancodados,tp_bancodados,ds_stringconexao")] BancoDados bancoDados)
        {
            if (ModelState.IsValid)
            {
                db.BancoDados.Add(bancoDados);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bancoDados);
        }

        // GET: BancoDados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BancoDados bancoDados = db.BancoDados.Find(id);
            if (bancoDados == null)
            {
                return HttpNotFound();
            }
            return View(bancoDados);
        }

        // POST: BancoDados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ds_bancodados,tp_bancodados,ds_stringconexao")] BancoDados bancoDados)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bancoDados).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bancoDados);
        }

        // GET: BancoDados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BancoDados bancoDados = db.BancoDados.Find(id);
            if (bancoDados == null)
            {
                return HttpNotFound();
            }
            return View(bancoDados);
        }

        // POST: BancoDados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BancoDados bancoDados = db.BancoDados.Find(id);
            db.BancoDados.Remove(bancoDados);
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
