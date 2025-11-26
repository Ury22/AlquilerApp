using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlquilerApp.Models;
using AlquilerApp.Data;
using System.Threading.Tasks;
using System.Linq;

public class EstadoController : Controller
{
    private readonly ApplicationDbContext _context;

    public EstadoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Estado
    public async Task<IActionResult> Index()
    {
        return View(await _context.Estado.ToListAsync());
    }

    // GET: Estado/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Estado/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Usamos EstadoNombre en el Bind
    public async Task<IActionResult> Create([Bind("IdEstado,EstadoNombre")] Estado estado)
    {
        if (ModelState.IsValid)
        {
            _context.Add(estado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(estado);
    }

    // GET: Estado/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var estado = await _context.Estado.FindAsync(id);
        if (estado == null) return NotFound();
        return View(estado);
    }

    // POST: Estado/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdEstado,EstadoNombre")] Estado estado)
    {
        if (id != estado.IdEstado) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(estado);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Estado.Any(e => e.IdEstado == id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(estado);
    }

    // GET: Estado/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var estado = await _context.Estado
            .FirstOrDefaultAsync(m => m.IdEstado == id);
        if (estado == null) return NotFound();

        return View(estado);
    }

    // POST: Estado/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var estado = await _context.Estado.FindAsync(id);
        _context.Estado.Remove(estado);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}