using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Painel_Net.Models.Contexto;
using Painel_Net.Models.Entidades;

namespace Painel_Net.Controllers
{
    public class IntegracaoController : Controller
    {
        private Contexto db = new Contexto();

        // GET: Integracao
        public ActionResult Index()
        {
            var integracao = db.Integracao.Include(i => i.BancoConexao).Include(i => i.Empresa).Include(i => i.TipoIntegracao);
            return View(integracao.ToList());
        }

        // GET: Integracao/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Integracao integracao = db.Integracao.Find(id);
            if (integracao == null)
            {
                return HttpNotFound();
            }
            return View(integracao);
        }

        // GET: Integracao/Create
        public ActionResult Create()
        {
            ViewBag.idBancoConexao = new SelectList(db.BancoDados, "Id", "ds_bancodados");
            ViewBag.idEmpresa = new SelectList(db.Empresa, "Id", "no_Empresa");
            ViewBag.idTipoIntegracao = new SelectList(db.TipoIntegracao, "id_Tipo_Integracao", "no_Tipo_Integracao");
            return View();
        }

        // POST: Integracao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,idTipoIntegracao,idEmpresa,idBancoConexao")] Integracao integracao)
        {
            if (ModelState.IsValid)
            {
                db.Integracao.Add(integracao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idBancoConexao = new SelectList(db.BancoDados, "Id", "ds_bancodados", integracao.idBancoConexao);
            ViewBag.idEmpresa = new SelectList(db.Empresa, "Id", "no_Empresa", integracao.idEmpresa);
            ViewBag.idTipoIntegracao = new SelectList(db.TipoIntegracao, "id_Tipo_Integracao", "no_Tipo_Integracao", integracao.idTipoIntegracao);
            return View(integracao);
        }

        // GET: Integracao/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Integracao integracao = db.Integracao.Find(id);
            if (integracao == null)
            {
                return HttpNotFound();
            }
            ViewBag.idBancoConexao = new SelectList(db.BancoDados, "Id", "ds_bancodados", integracao.idBancoConexao);
            ViewBag.idEmpresa = new SelectList(db.Empresa, "Id", "no_Empresa", integracao.idEmpresa);
            ViewBag.idTipoIntegracao = new SelectList(db.TipoIntegracao, "id_Tipo_Integracao", "no_Tipo_Integracao", integracao.idTipoIntegracao);
            return View(integracao);
        }

        // POST: Integracao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,idTipoIntegracao,idEmpresa,idBancoConexao")] Integracao integracao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(integracao).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idBancoConexao = new SelectList(db.BancoDados, "Id", "ds_bancodados", integracao.idBancoConexao);
            ViewBag.idEmpresa = new SelectList(db.Empresa, "Id", "no_Empresa", integracao.idEmpresa);
            ViewBag.idTipoIntegracao = new SelectList(db.TipoIntegracao, "id_Tipo_Integracao", "no_Tipo_Integracao", integracao.idTipoIntegracao);
            return View(integracao);
        }

        // GET: Integracao/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Integracao integracao = db.Integracao.Find(id);
            if (integracao == null)
            {
                return HttpNotFound();
            }
            return View(integracao);
        }

        // POST: Integracao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Integracao integracao = db.Integracao.Find(id);
            db.Integracao.Remove(integracao);
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
