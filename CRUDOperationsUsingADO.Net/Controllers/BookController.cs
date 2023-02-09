using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using CRUDOperationsUsingADO.Net.Data;
using CRUDOperationsUsingADO.Net.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using static CRUDOperationsUsingADO.Net.Helper;
using System.Drawing.Printing;
using System.Xml.Linq;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace CRUDOperationsUsingADO.Net.Controllers
{
    public class BookController : Controller
    {
        private readonly IConfiguration _configuration;

        //Constructor
        public BookController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        // GET: Book
        public IActionResult Index()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("BookConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("BookViewAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);                        
        }

        // GET: Book/Edit/5
        [NoDirectAccess]
        public IActionResult AddOrEdit(int? id)
        {
            BookViewModel bookViewModel = new BookViewModel();
            if (id > 0)
                bookViewModel = FetchBookByID(id);
            return View(bookViewModel);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(int id, [Bind("BookID,Title,Author,Price")] BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                //Sql Connection
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("BookConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("BookAddOrEdit", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("BookID", bookViewModel.BookID);
                    sqlCmd.Parameters.AddWithValue("Title", bookViewModel.Title);
                    sqlCmd.Parameters.AddWithValue("Author", bookViewModel.Author);
                    sqlCmd.Parameters.AddWithValue("Price", bookViewModel.Price);
                    sqlCmd.ExecuteNonQuery();
                }
                return Json(new { isValid = true, redirectToUrl = Url.Action("Index", "Book") });
                //return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "Index") });
            }           
            return Json(new {isValid= false,html = Helper.RenderRazorViewToString(this,"AddOrEdit", bookViewModel) });
            //return Json(new { isValid = false, redirectToUrl = Url.Action("AddOrEdit", bookViewModel) });
        }

        // GET: Book/Delete/5
        public IActionResult Delete(int? id)
        {
            BookViewModel bookViewModel = FetchBookByID(id);
            return View(bookViewModel);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("BookConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("BookDeleteByID", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("BookID", id);
                sqlCmd.ExecuteNonQuery();
            }
            //return RedirectToAction(nameof(Index));            
            return Json(new { redirectToUrl = Url.Action("Index", "Book") });
        }

        //// GET: Book/DownloadPDF/5
        public IActionResult DownloadPDF()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("BookConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM Books ORDER BY Title, Author, Price", sqlConnection);
                sqlDa.Fill(dtbl);
            }

            return File(GeneratePDF(dtbl), "application/pdf", "Books.pdf");
        }

        private byte[] GeneratePDF(DataTable dataTable)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                var paragraph = new Paragraph("Book Information")
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10
                };

                PdfPTable pdfTable = new PdfPTable(dataTable.Columns.Count);
                pdfTable.WidthPercentage = 100;

                foreach (DataColumn column in dataTable.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName));
                    cell.BackgroundColor = new BaseColor(240, 240, 240);
                    pdfTable.AddCell(cell);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        pdfTable.AddCell(item.ToString());
                    }
                }

                pdfDoc.Add(pdfTable);
                pdfDoc.Close();

                return stream.ToArray();
            }
        }

        public BookViewModel FetchBookByID(int? id)
        {
            BookViewModel bookViewModel = new BookViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("BookConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("BookViewByID", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("BookID", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    bookViewModel.BookID = Convert.ToInt32(dtbl.Rows[0]["BookID"].ToString());
                    bookViewModel.Title = dtbl.Rows[0]["Title"].ToString();
                    bookViewModel.Author = dtbl.Rows[0]["Author"].ToString();
                    bookViewModel.Price = Convert.ToInt32(dtbl.Rows[0]["Price"].ToString());
                }
                return bookViewModel;
            }
        }  
    }
}
