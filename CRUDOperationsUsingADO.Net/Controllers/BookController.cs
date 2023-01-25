﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRUDOperationsUsingADO.Net.Data;
using CRUDOperationsUsingADO.Net.Models;
using Microsoft.Data.SqlClient;
using System.Data;

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
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("BookViewAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);                        
        }

        //// GET: Book/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.BookViewModel == null)
        //    {
        //        return NotFound();
        //    }

        //    var bookViewModel = await _context.BookViewModel
        //        .FirstOrDefaultAsync(m => m.BookID == id);
        //    if (bookViewModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(bookViewModel);
        //}

        //// GET: Book/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Book/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("BookID,Title,Author,Price")] BookViewModel bookViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(bookViewModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(bookViewModel);
        //}

        // GET: Book/Edit/5
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
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
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
                return RedirectToAction(nameof(Index));
            }
            return View(bookViewModel);
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
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("BookDeleteByID", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("BookID", id);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        public BookViewModel FetchBookByID(int? id)
        {
            BookViewModel bookViewModel = new BookViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
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
