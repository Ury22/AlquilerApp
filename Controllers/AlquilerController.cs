using AlquilerApp.Data;
using AlquilerApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AlquilerApp.Controllers
{
    public class AlquilerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlquilerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Alquiler
        public async Task<IActionResult> Index()
        {
            // Incluimos las propiedades de navegación para mostrar los nombres de las relaciones.
            var applicationDbContext = _context.Alquiler
                .Include(a => a.Estado)
                .Include(a => a.InquilinoPersona) // Persona como Inquilino
                .Include(a => a.PropietarioPersona) // Persona como Propietario
                .Include(a => a.Vivienda); // Vivienda

            return View(await applicationDbContext.ToListAsync());
        }

        // --- MÉTODOS DE SOPORTE ---
        
        // Método para cargar los ViewBag necesarios para los DropDowns
        private void PopulateDropDowns(object selectedVivienda = null, object selectedInquilino = null, object selectedPropietario = null, object selectedEstado = null)
        {
            // 1. Viviendas (Llave Foránea: string CodigoVivienda)
            var viviendas = _context.Vivienda
                .Select(v => new
                {
                    v.CodigoVivienda,
                    Display = $"C: {v.CodigoVivienda} - Bloque: {v.NumBloque}, Calle: {v.NumCalle}"
                })
                .OrderBy(v => v.CodigoVivienda);

            ViewBag.Viviendas = new SelectList(viviendas, "CodigoVivienda", "Display", selectedVivienda);

            // 2. Personas (Propietarios) (Llave Foránea: string NumIdentidad)
            const int PropietarioRolId = 1; 
            var propietarios = _context.Persona
                // Filtramos solo las personas que tienen el rol de Propietario (IdRol = 1) en PersonaRol
                .Where(p => _context.PersonaRol.Any(pr => pr.NumIdentidad == p.NumIdentidad && pr.IdRol == PropietarioRolId))
                .Select(p => new
                {
                    p.NumIdentidad,
                    Display = $"{p.Nombres} {p.Apellidos} (Prop. - {p.NumIdentidad})"
                })
                .OrderBy(p => p.NumIdentidad);

            ViewBag.Propietarios = new SelectList(propietarios, "NumIdentidad", "Display", selectedPropietario);

            // 3. Personas (Inquilinos) (Llave Foránea: string NumIdentidad)
            const int InquilinoRolId = 2; 
            var inquilinos = _context.Persona
                // Filtramos solo las personas que tienen el rol de Inquilino (IdRol = 2) en PersonaRol
                .Where(p => _context.PersonaRol.Any(pr => pr.NumIdentidad == p.NumIdentidad && pr.IdRol == InquilinoRolId))
                .Select(p => new
                {
                    p.NumIdentidad,
                    Display = $"{p.Nombres} {p.Apellidos} (Inq. - {p.NumIdentidad})"
                })
                .OrderBy(p => p.NumIdentidad);

            ViewBag.Inquilinos = new SelectList(inquilinos, "NumIdentidad", "Display", selectedInquilino);
            
            
            // 4. Estados (Llave Foránea: int IdEstado)
            var estados = _context.Estado
                .OrderBy(e => e.EstadoNombre) // Asumiendo que el campo es EstadoNombre o similar
                .Select(e => new 
                {
                    e.IdEstado,
                    e.EstadoNombre // Asegúrate de que este campo exista en tu modelo Estado
                });

            ViewBag.Estados = new SelectList(estados, "IdEstado", "EstadoNombre", selectedEstado);
        }

        // --- ACCIONES CREATE ---

        // GET: Alquiler/Create
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        // POST: Alquiler/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoVivienda,IdEstado,Propietario,Inquilino,FechaAlquiler,FechaInicio,FechaFinalizacion,CuotaMensual,DepositoGarantia")] Alquiler alquiler)
        {
            if (ModelState.IsValid)
            {
                // Validación de Identidades (Opcional, pero buena práctica)
                if (alquiler.Propietario == alquiler.Inquilino)
                {
                    ModelState.AddModelError(string.Empty, "El Propietario y el Inquilino no pueden ser la misma persona.");
                    PopulateDropDowns(alquiler.CodigoVivienda, alquiler.Inquilino, alquiler.Propietario, alquiler.IdEstado);
                    return View(alquiler);
                }

                _context.Add(alquiler);
                await _context.SaveChangesAsync();
                
                // Si el guardado es exitoso, redirecciona al Index
                return RedirectToAction(nameof(Index)); 
            }

            // Si falla, recarga las listas y vuelve a la vista con los datos introducidos
            PopulateDropDowns(alquiler.CodigoVivienda, alquiler.Inquilino, alquiler.Propietario, alquiler.IdEstado);
            return View(alquiler);
        }

        // --- ACCIONES EDIT ---

        // GET: Alquiler/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquiler = await _context.Alquiler.FindAsync(id);
            if (alquiler == null)
            {
                return NotFound();
            }
            
            PopulateDropDowns(alquiler.CodigoVivienda, alquiler.Inquilino, alquiler.Propietario, alquiler.IdEstado);
            return View(alquiler);
        }

        // POST: Alquiler/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodAlquiler,CodigoVivienda,IdEstado,Propietario,Inquilino,FechaAlquiler,FechaInicio,FechaFinalizacion,CuotaMensual,DepositoGarantia")] Alquiler alquiler)
        {
            if (id != alquiler.CodAlquiler)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validación de Identidades
                if (alquiler.Propietario == alquiler.Inquilino)
                {
                    ModelState.AddModelError(string.Empty, "El Propietario y el Inquilino no pueden ser la misma persona.");
                    PopulateDropDowns(alquiler.CodigoVivienda, alquiler.Inquilino, alquiler.Propietario, alquiler.IdEstado);
                    return View(alquiler);
                }
                
                try
                {
                    // Asumiendo que CodAlquiler es la clave primaria y está incluida en el Bind
                    _context.Update(alquiler);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlquilerExists(alquiler.CodAlquiler))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Si el guardado es exitoso, redirecciona al Index
                return RedirectToAction(nameof(Index));
            }

            // Si falla, recarga las listas y vuelve a la vista con los datos introducidos
            PopulateDropDowns(alquiler.CodigoVivienda, alquiler.Inquilino, alquiler.Propietario, alquiler.IdEstado);
            return View(alquiler);
        }

        private bool AlquilerExists(int id)
        {
            return _context.Alquiler.Any(e => e.CodAlquiler == id);
        }
        
        // Puedes agregar las acciones Details y Delete si las necesitas.
    }
}