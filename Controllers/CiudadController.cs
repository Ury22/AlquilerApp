using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlquilerApp.Data;
using AlquilerApp.Models;

namespace AlquilerApp.Controllers
{
    public class CiudadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CiudadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ciudad
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ciudad.Include(c => c.Pais);
            return View(await applicationDbContext.ToListAsync());
        }
        
        // GET: Ciudad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // IMPORTANTE: Incluir Pais para evitar NullReferenceException en la vista Details
            var ciudad = await _context.Ciudad
                .Include(c => c.Pais) 
                .FirstOrDefaultAsync(m => m.IdCiudad == id);

            if (ciudad == null)
            {
                return NotFound();
            }

            return View(ciudad);
        }

        // GET: Ciudad/Create
        public IActionResult Create()
        {
            // Usa "PaisNombre" para el texto visible
            ViewData["IdPais"] = new SelectList(_context.Pais, "IdPais", "PaisNombre");
            return View();
        }

        // POST: Ciudad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCiudad,CiudadNombre,IdPais")] Ciudad ciudad)
        {
            if (ModelState.IsValid)
            {
                ciudad.IdCiudad = 0; 
                _context.Add(ciudad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdPais"] = new SelectList(_context.Pais, "IdPais", "PaisNombre", ciudad.IdPais);
            return View(ciudad);
        }

        // GET: Ciudad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ciudad = await _context.Ciudad.FindAsync(id);
            if (ciudad == null)
            {
                return NotFound();
            }
            // Usa "PaisNombre"
            ViewData["IdPais"] = new SelectList(_context.Pais, "IdPais", "PaisNombre", ciudad.IdPais);
            return View(ciudad);
        }

        // POST: Ciudad/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCiudad,CiudadNombre,IdPais")] Ciudad ciudad)
        {
            if (id != ciudad.IdCiudad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ciudad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CiudadExists(ciudad.IdCiudad))
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
            // Usa "PaisNombre"
            ViewData["IdPais"] = new SelectList(_context.Pais, "IdPais", "PaisNombre", ciudad.IdPais);
            return View(ciudad);
        }

        // GET: Ciudad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // IMPORTANTE: Incluir Pais para evitar NullReferenceException en la vista Delete
            var ciudad = await _context.Ciudad
                .Include(c => c.Pais)
                .FirstOrDefaultAsync(m => m.IdCiudad == id);
            
            if (ciudad == null)
            {
                return NotFound();
            }

            return View(ciudad);
        }

        // POST: Ciudad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ciudad = await _context.Ciudad.FindAsync(id);
            if (ciudad != null)
            {
                _context.Ciudad.Remove(ciudad);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool CiudadExists(int id)
        {
            return _context.Ciudad.Any(e => e.IdCiudad == id);
        }
    }
}