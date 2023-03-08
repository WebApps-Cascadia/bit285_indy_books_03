﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IndyBooks.Models;
using IndyBooks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IndyBooks.Controllers
{
    public class AdminController : Controller
    {
        private IndyBooksDbContext _db;
        public AdminController(IndyBooksDbContext db) { _db = db; }

        /***
         * DELETE
         */
        [HttpGet]
        public IActionResult DeleteBook(long id)
        {

            //TODO: Remove the Book associated with the given id number; Save Changes - DONE

            _db.Remove(new Book { Id = id });
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        /***
         * READ       
         */
        [HttpGet]
        public IActionResult Index(long id)
        {
            //TODO: Index will return the list of books (with Author), ordered by SKU
            //or a single book (with Author), if passed an id > 0
            /*IEnumerable<Book> books = _db.Books.Include(b => b.Author);*/
            IEnumerable<Book> books = (id > 0) ? _db.Books.Include(b => b.Author).Where(b => b.Id == id) : _db.Books.Include(b => b.Author).OrderBy(b => b.SKU);

            var searchResults = new SearchResultsVM
            {
                FoundBooks = books,
                HalfPriceSale = false //Just display the regular prices
            };
            return View("SearchResults", searchResults);
        }
        /***
         * CREATE
         */
        [HttpGet]
        public IActionResult CreateBook()
        {
            //Form uses the collection of Writers to populate the drop-down list
            return View(new CreateBookVM { Writers = _db.Writers.OrderBy(w => w.Name) });
        }
        [HttpPost]
        public IActionResult CreateBook(CreateBookVM bookVM, long id)
        {
            Writer writer;
            //TODO: Assign the writer object depending on what is entered for the AUTHOR NAME in the ViewModel
            // if the VM has an AuthorId (from the drop-down) you can use it to get that writer, otherwise create a new one
            writer = (bookVM.Name != null) ? new Writer { Name = bookVM.Name } : _db.Writers.Find(bookVM.AuthorId);


            //TODO: Create a new Book (with the writer above)

            Book book = new Book { Title = bookVM.Title, Author = writer, SKU = bookVM.SKU, Price = bookVM.Price };

            //TODO: Add book (and perhaps a new writer) to the Db; SaveChanges

            _db.Add(book);
            if (!_db.Writers.Any(w => w.Name == writer.Name)) _db.Add(writer);
            _db.SaveChanges();

            //TODO: Display the single book that was added using the Index Action and Route
            return RedirectToAction("Index", new { id = book.Id });
        }
        /***
         * UPDATE (reusing the CreateBook View, passing it the Book id to be updated) 
         */
         [HttpGet]
         public IActionResult UpdateBook(long id)
        {
            //TODO: Display the database info for the Book (and its Author) indicated by the id 
            var book = _db.Books.Include(b => b.Author).Single(b => b.Id == id);

            var createbookVM = new CreateBookVM
            {
                BookId = book.Id,
                AuthorId = book.Author.Id,
                Price = book.Price,
                SKU = book.SKU,
                Title = book.Title,
                Writers = _db.Writers
            };
            return View("CreateBook", createbookVM);
            
        }
        /***
         * Search Method (from IndyBook ) - NO EDITS NEEDED BELOW HERE
         */
        [HttpGet]
        public IActionResult Search() { return View(); }
        [HttpPost]
        public IActionResult Search(SearchVM searchVM)
        {
            //Full Collection Search
            IQueryable<Book> foundBooks = _db.Books.Include(b=>b.Author); // start with entire collection

            //Partial Title Search
            if (searchVM.Title != null)
            {
                foundBooks = foundBooks
                            .Where(b => b.Title.Contains(searchVM.Title))
                            .OrderBy(b => b.Author.Name)
                            ;
            }

            //Author's Last Name Search
            if (searchVM.Name != null)
            {
                //Use the Name property of the Book's Author entity
                foundBooks = foundBooks
                            .Where(b => b.Author.Name.EndsWith(searchVM.Name))
                            ;
            }
            //Priced Between Search (min and max price entered)
            if (searchVM.Min > 0 && searchVM.Max > 0)
            {
                foundBooks = foundBooks
                            .Where(b => b.Price >= searchVM.Min && b.Price <= searchVM.Max)
                            ;
            }
            if (searchVM.Sale)
            {
                foundBooks = foundBooks
                             .Where(b => b.Price > 80)
                             .Select(b => new Book
                             {
                                 Title = b.Title,
                                 Author = b.Author,
                                 Id = b.Id,
                                 SKU = b.SKU,
                                 Price = b.Price / 2
                             });

            }
            //TODO: Pass a SearchResultsVM object to View
            var searchResults = new SearchResultsVM
            {
                FoundBooks = foundBooks,
                HalfPriceSale = searchVM.Sale
            };

            return View("SearchResults", searchResults);
        }

    }
}
