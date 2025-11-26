using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlquilerApp.Data;
using AlquilerApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AlquilerApp.Controllers
{
    public class PaisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pais (Listado)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pais.ToListAsync());
        }

        // GET: Pais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Pais no tiene relaciones que necesiten .Include(), es sencillo.
            var pais = await _context.Pais
                .FirstOrDefaultAsync(m => m.IdPais == id);

            if (pais == null) return NotFound();
            return View(pais);
        }

        // GET: Pais/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pais/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPais,PaisNombre")] Pais pais)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pais);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pais);
        }

        // GET: Pais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pais = await _context.Pais.FindAsync(id);
            if (pais == null) return NotFound();
            return View(pais);
        }

        // POST: Pais/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPais,PaisNombre")] Pais pais)
        {
            if (id != pais.IdPais) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pais);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaisExists(pais.IdPais)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pais);
        }

        // GET: Pais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // Pais no tiene relaciones que necesiten .Include(), es sencillo.
            var pais = await _context.Pais
                .FirstOrDefaultAsync(m => m.IdPais == id);

            if (pais == null) return NotFound();
            return View(pais);
        }

        // POST: Pais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pais = await _context.Pais.FindAsync(id);
            if (pais != null)
            {
                _context.Pais.Remove(pais);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool PaisExists(int id)
        {
            return _context.Pais.Any(e => e.IdPais == id);
        }
    }
}