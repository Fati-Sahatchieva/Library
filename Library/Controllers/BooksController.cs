using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using System.Text;

namespace Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var libraryDbContext = _context.Books.Include(b => b.Category);
            return View(await libraryDbContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,Title,Author,PublicationYear,BookCopies,CategoryId")] Book book)
        {
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", book.CategoryId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", book.CategoryId);
            return View(book);
        }

        // POST: Books/Edit/5
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Author,PublicationYear,BookCopies,CategoryId")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", book.CategoryId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }

        public async Task<IActionResult> ExportToCSV()
        {
            var books = await _context.Books
                .Include(b => b.Category).ToListAsync();

            var csv = new StringBuilder();

            csv.AppendLine("BookId,Title,Author,PublicationYear,BookCopies,CategoryId");

            foreach (var book in books) 
            {
                csv.AppendLine($"{book.BookId}, {book.Title}, {book.Author}, {book.PublicationYear}, {book.BookCopies}, {book.Category?.Name}");
            }

            var filename = $"books_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", filename);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFromCSV(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Pleas upload a valid CSV file.");
            }

            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                while(!streamReader.EndOfStream)
                {
                    var line = await streamReader.ReadLineAsync();
                    string[] values = line.Split(',');

                    if (values.Length == 5)
                    {
                        var title = values[0];
                        var author = values[1];
                        if (!int.TryParse(values[2], out int publicationYear))
                        {
                            return BadRequest($"Invalid publication year for book: {title}");
                        }
                        if (!int.TryParse(values[3], out int bookCopies))
                        {
                            return BadRequest($"Invalid number of copies for book: {title}");
                        }
                        var categoryId = values[4];

                        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryId);
                        if (category == null)
                        {
                            category = new Category { Name = categoryId };
                            _context.Categories.Add(category);
                            await _context.SaveChangesAsync();
                        }
                        var book = new Book
                        {
                            Title = title,
                            Author = author,
                            PublicationYear = publicationYear,
                            BookCopies = bookCopies,
                            CategoryId = category.CategoryId
                        };
                        _context.Books.Add(book);
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        
    }
}
