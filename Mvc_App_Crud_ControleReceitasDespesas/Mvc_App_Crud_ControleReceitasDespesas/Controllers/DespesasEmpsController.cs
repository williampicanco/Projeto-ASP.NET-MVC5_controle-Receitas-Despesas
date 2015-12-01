using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mvc_App_Crud_ControleReceitasDespesas.Models;
using Microsoft.Reporting.WebForms;
using ReportViewerForMvc;
using System.IO;
using Mvc_App_Crud_ControleReceitasDespesas.Reports;
using CrystalDecisions.CrystalReports.Engine;
using System.Reflection;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Novacode;

namespace Mvc_App_Crud_ControleReceitasDespesas.Controllers
{
    public class DespesasEmpsController : Controller
    {     
       private empresaEntities db = new empresaEntities();             

        //Relatório de LISTA DE DESPESAS .PDF (Crystal Report) - Funcionando mas vem sem parâmetors ainda 24/11/2015.
        public ActionResult GenerateReport()
        {
            ReportDocument rd = new ReportDocument(); //(Crystal Report)
           
            rd.Load(Path.Combine(Server.MapPath("../Reports/CrystalReportDespesas.rpt")));
            ViewBag.ListDespesas = db.DespesasEmps.ToList();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream
               (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ListaDespesas.pdf");

        }

        //.PDF(Report Viewer)
        public ActionResult RenderizaRelatorio()
        {
           LocalReport relatorio = new LocalReport();
           relatorio.ReportPath = Server.MapPath("../Report/RelatorioDespesa.rdlc");          
            ReportDataSource reportDataSource = new ReportDataSource();          
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";
            //The DeviceInfo settings should be changed based on the reportType 
            string deviceInfo =  
               "<DeviceInfo>" +              
               "OutputFormat>PDF</OutputFormat>"  +            
               " <PageWidth>9.2in</PageWidth>" +
               " <PageHeight>12in</PageHeight>" +
                "<MarginTop>0.25in</MarginTop> " +
               " <MarginLeft>0.45in</MarginLeft>" +
               " <MarginRight>0.45in</MarginRight> " +
               " <MarginBottom>0.25in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] bytes;

             bytes = relatorio.Render(
             reportType, 
             deviceInfo, 
             out mimeType, 
             out encoding, 
             out fileNameExtension,
             out streams, 
             out warnings);       

            Response.AddHeader("content-disposition", "attachment; filename=DESPESA." + fileNameExtension);
            return File(bytes, mimeType);
        }
     
        //Mostra uma Lista de Despesas - 23/11/2015.
        public ActionResult DespesasReport()
        {       
            ViewBag.ListDespesas = db.DespesasEmps.ToList();
            return View();
        }

        private DataTable CreateDataTable(PropertyInfo[] properties)
        {
            DataTable dt = new DataTable();
            DataColumn dc = null;
            foreach (PropertyInfo pi in properties)
            {
                if (pi.PropertyType.Name.Contains("Nullable"))
                    dc.DataType = typeof(String);
                else
                    dc.DataType = pi.PropertyType;

                dt.Columns.Add(dc);
            }
            return dt;
        }

        //(Crystal Report) - NÃO ESTÁ FUNCIONANDO.
        public ActionResult Export()
        {
            //empresaEntities db = new empresaEntities();
            // var d = (from s in db.DespesasEmps
            //          select new
            //          {
            //              s.DespesaId,
            //              s.Categoria,
            //              s.Data,
            //              s.Observacao,
            //              newValor = s.Valor == null ? 0.0 : s.Valor
            //          }).ToArray();

            // List<ReceitasEmp> todasDespesas = new List<ReceitasEmp>();
            // using (empresaEntities dc = new empresaEntities())
            //{             
            //     todasDespesas = dc.ReceitasEmps.ToList();                        
            // }
            // ReportDocument rd = new ReportDocument();     
            //CrystalReportDespesas c = new CrystalReportDespesas();
            //c.SetDataSource(d);

            //rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CrystalReportDespesas.rpt"));
            //rd.SetDataSource(d);
            ReportDocument rd = new ReportDocument(); //(Crystal Report)

            rd.Load(Path.Combine(Server.MapPath("../Reports/ListaDespesas.rpt")));
            ViewBag.ListDespesas = db.DespesasEmps.ToList();
            /* rd.SetDataSource(db.DespesasEmps.Select(d => new
             {
                 Despesa = d.DespesaId,
                 Categoria = d.Categoria,
                 Data = d.Data,
                 Observacao = d.Observacao,
                 valor = d.Valor.Value
             }).ToList());*/
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream
               (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ListaDespesas.pdf");          
            //ReportDataSource = c;

            /*Response.Buffer = false;
             Response.ClearContent();
             Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "ListaDespesas.pdf");
            }
            catch (Exception ex)
            {
                throw;
            }   */
        }
        
        public ActionResult Report(string id)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.ServerReport.ReportPath = "/Reports/RelatorioDespesa.rdlc";
            reportViewer.ServerReport.ReportServerUrl = new Uri("http://localhost/ReportServer/");
            
            ViewBag.ReportViewer = reportViewer;       
                                       
            return View();
         } 
  
        // GET: DespesasEmps
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.CategoriaOrdenacaoParam = String.IsNullOrEmpty(sortOrder) ? "Categoria_desc" : "";
            ViewBag.DateOrdenacaoParam = sortOrder == "Date" ? "Date_desc" : "Date";

            var despesas = db.DespesasEmps.ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                despesas = despesas.Where(d =>
                d.Categoria.ToUpper().Contains(searchString.ToUpper())
                ||
                d.Data.ToString().ToUpper().Contains(searchString.ToUpper())).ToList();
                return View(despesas);
            }
          
            var desp = from s in db.DespesasEmps select s;
            switch (sortOrder)
            {
                
                case "Categoria_desc":
                    desp = desp.OrderByDescending(s => s.Categoria);
                    break;

                case "Data":
                    desp = desp.OrderBy(s => s.Data);
                    break;

                case "Date_desc":
                    desp = desp.OrderByDescending(s => s.Data);
                    break;

                default:
                    desp = desp.OrderBy(s => s.Categoria);
                    break;
            }

            return View(desp.ToList());
        }

        // GET: DespesasEmps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DespesasEmp despesasEmp = db.DespesasEmps.Find(id);
            if (despesasEmp == null)
            {
                return HttpNotFound();
            }
            return View(despesasEmp);
        }

        // GET: DespesasEmps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DespesasEmps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DespesaId,Valor,Categoria,Data,Observacao")] DespesasEmp despesasEmp)
        {
            if (ModelState.IsValid)
            {
                db.DespesasEmps.Add(despesasEmp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(despesasEmp);
        }

        // GET: DespesasEmps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DespesasEmp despesasEmp = db.DespesasEmps.Find(id);
            if (despesasEmp == null)
            {
                return HttpNotFound();
            }
            return View(despesasEmp);
        }

        // POST: DespesasEmps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DespesaId,Valor,Categoria,Data,Observacao")] DespesasEmp despesasEmp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(despesasEmp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(despesasEmp);
        }

        // GET: DespesasEmps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DespesasEmp despesasEmp = db.DespesasEmps.Find(id);
            if (despesasEmp == null)
            {
                return HttpNotFound();
            }
            return View(despesasEmp);
        }

        // POST: DespesasEmps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DespesasEmp despesasEmp = db.DespesasEmps.Find(id);
            db.DespesasEmps.Remove(despesasEmp);
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
