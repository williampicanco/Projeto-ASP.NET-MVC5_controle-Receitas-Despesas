using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mvc_App_Crud_ControleReceitasDespesas.Models;

namespace Mvc_App_Crud_ControleReceitasDespesas.Controllers
{
    public class ReceitasEmpsController : Controller
    {
        private empresaEntities db = new empresaEntities();

        public ActionResult ReceitasReport()
        {
            ViewBag.ListReceitas = db.ReceitasEmps.ToList();
            return View();
        }

        // GET: ReceitasEmps
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.CategoriaOrdenacaoParam = String.IsNullOrEmpty(sortOrder) ? "Categoria_desc" : "";
            ViewBag.DateOrdenacaoParam = sortOrder == "Date" ? "Date_desc" : "Date";

            var receitas = db.ReceitasEmps.ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                receitas = receitas.Where(d =>
                d.Categoria.ToUpper().Contains(searchString.ToUpper())
                ||
                d.Data.ToString().ToUpper().Contains(searchString.ToUpper())).ToList();
                return View(receitas);
            }

            var recei = from s in db.ReceitasEmps select s;
            switch (sortOrder)
            {

                case "Categoria_desc":
                    recei = recei.OrderByDescending(s => s.Categoria);
                    break;

                case "Data":
                    recei = recei.OrderBy(s => s.Data);
                    break;

                case "Date_desc":
                    recei = recei.OrderByDescending(s => s.Data);
                    break;

                default:
                    recei = recei.OrderBy(s => s.Categoria);
                    break;
            }

            return View(recei.ToList());
           
        }

        // GET: ReceitasEmps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceitasEmp receitasEmp = db.ReceitasEmps.Find(id);
            if (receitasEmp == null)
            {
                return HttpNotFound();
            }
            return View(receitasEmp);
        }

        // GET: ReceitasEmps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReceitasEmps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReceitaId,Valor,Categoria,Data,Observacao")] ReceitasEmp receitasEmp)
        {
            if (ModelState.IsValid)
            {
                db.ReceitasEmps.Add(receitasEmp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(receitasEmp);
        }

        // GET: ReceitasEmps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceitasEmp receitasEmp = db.ReceitasEmps.Find(id);
            if (receitasEmp == null)
            {
                return HttpNotFound();
            }
            return View(receitasEmp);
        }

        // POST: ReceitasEmps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReceitaId,Valor,Categoria,Data,Observacao")] ReceitasEmp receitasEmp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receitasEmp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(receitasEmp);
        }

        // GET: ReceitasEmps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceitasEmp receitasEmp = db.ReceitasEmps.Find(id);
            if (receitasEmp == null)
            {
                return HttpNotFound();
            }
            return View(receitasEmp);
        }

        // POST: ReceitasEmps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReceitasEmp receitasEmp = db.ReceitasEmps.Find(id);
            db.ReceitasEmps.Remove(receitasEmp);
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
