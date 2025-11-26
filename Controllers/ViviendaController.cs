using AlquilerApp.Data;
using AlquilerApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlquilerApp.Controllers
{
    public class ViviendaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViviendaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================================
        // INDEX (READ - LISTADO)
        // ===============================================
        public async Task<IActionResult> Index()
        {
            // Incluimos las propiedades de navegación necesarias para la vista de lista.
            var applicationDbContext = _context.Vivienda
                .Include(v => v.PropietarioPersona)
                .Include(v => v.Ciudad); // Aseguramos que Ciudad también se incluye si se usa en la vista
            
            return View(await applicationDbContext.ToListAsync()); 
        }

        // ===============================================
        // DETAILS (READ - DETALLE)
        // ===============================================
        // ** CORRECCIÓN: El ID ahora es STRING **
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) // Usamos IsNullOrEmpty para validar el string ID
            {
                return NotFound();
            }

            var vivienda = await _context.Vivienda
                .Include(v => v.PropietarioPersona)
                .Include(v => v.Ciudad) 
                .FirstOrDefaultAsync(m => m.CodigoVivienda == id); // Comparación: string == string

            if (vivienda == null)
            {
                return NotFound();
            }

            return View(vivienda);
        }

        // ===============================================
        // CREATE (GET - Muestra el formulario)
        // ===============================================
        public async Task<IActionResult> Create()
        {
            await PopulatePropietariosList();
            await PopulateCiudadesList(); // Aseguramos que la lista de ciudades se carga
            return View();
        }

        // ===============================================
        // CREATE (POST - Procesa el formulario)
        // ===============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoVivienda,Propietario,IdCiudad,NumVivienda,NumCalle,NumBloque,CapacidadPersonas,CantDormitorios,CantBanios,DescripcionVivienda")] Vivienda vivienda)
        {
            if (ModelState.IsValid)
            {
                // 1. Verificar que el CodigoVivienda sea único (validación de negocio)
                if (await _context.Vivienda.AnyAsync(v => v.CodigoVivienda == vivienda.CodigoVivienda))
                {
                    ModelState.AddModelError("CodigoVivienda", "Ya existe una vivienda con este código. Por favor, ingrese uno diferente.");
                }
                else
                {
                    try
                    {
                        // 2. Guardar el registro
                        _context.Add(vivienda);
                        await _context.SaveChangesAsync();
                        
                        // 3. Inserción exitosa
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException ex)
                    {
                        // 4. Manejo de errores de base de datos
                        ModelState.AddModelError("", "Error al guardar la vivienda. Verifique que el Propietario y la Ciudad seleccionados sean válidos.");
                        System.Diagnostics.Debug.WriteLine($"[ERROR FATAL DE DB] Excepción: {ex.GetType().Name}. Mensaje: {ex.InnerException?.Message}");
                    }
                    catch (Exception ex)
                    {
                        // Manejo de otras excepciones genéricas
                        ModelState.AddModelError("", $"Ocurrió un error inesperado al intentar guardar la vivienda. Detalle: {ex.Message}");
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[ERROR DE VALIDACIÓN] El ModelState NO es válido. Errores detectados:");
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($" - Campo '{state.Key}': {string.Join("; ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }
            
            // Si hay errores, recargamos la vista y el ViewBag
            await PopulatePropietariosList(vivienda.Propietario); 
            await PopulateCiudadesList(vivienda.IdCiudad);
            return View(vivienda);
        }

        // ===============================================
        // EDIT (GET - Muestra el formulario de edición)
        // ===============================================
        // ** CORRECCIÓN: El ID ahora es STRING **
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // FindAsync acepta string como PK
            var vivienda = await _context.Vivienda.FindAsync(id);
            if (vivienda == null)
            {
                return NotFound();
            }

            // Se pasa la FK actual para que aparezca seleccionada en el dropdown
            await PopulatePropietariosList(vivienda.Propietario);
            await PopulateCiudadesList(vivienda.IdCiudad);
            return View(vivienda);
        }

        // ===============================================
        // EDIT (POST - Procesa la edición)
        // ===============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ** CORRECCIÓN: El ID ahora es STRING **
        public async Task<IActionResult> Edit(string id, [Bind("CodigoVivienda,Propietario,IdCiudad,CodigoAlquilerActivo,NumVivienda,NumCalle,NumBloque,CapacidadPersonas,CantDormitorios,CantBanios,DescripcionVivienda")] Vivienda vivienda)
        {
            // Ahora ambos son string, la comparación es válida.
            if (id != vivienda.CodigoVivienda)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vivienda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViviendaExists(vivienda.CodigoVivienda))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Manejo de errores de base de datos
                    ModelState.AddModelError("", "Error al actualizar la vivienda. Verifique que el Propietario y la Ciudad seleccionados sean válidos.");
                    System.Diagnostics.Debug.WriteLine($"[ERROR FATAL DE DB] Excepción: {ex.GetType().Name}. Mensaje: {ex.InnerException?.Message}");
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulatePropietariosList(vivienda.Propietario);
            await PopulateCiudadesList(vivienda.IdCiudad);
            return View(vivienda);
        }

        // ===============================================
        // DELETE (GET - Confirmación de eliminación)
        // ===============================================
        // ** CORRECCIÓN: El ID ahora es STRING **
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var vivienda = await _context.Vivienda
                .Include(v => v.PropietarioPersona)
                .Include(v => v.Ciudad)
                .FirstOrDefaultAsync(m => m.CodigoVivienda == id); // Comparación: string == string

            if (vivienda == null)
            {
                return NotFound();
            }

            return View(vivienda);
        }

        // ===============================================
        // DELETE (POST - Ejecuta la eliminación)
        // ===============================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        // ** CORRECCIÓN: El ID ahora es STRING **
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Vivienda == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vivienda' is null.");
            }
            var vivienda = await _context.Vivienda.FindAsync(id); // FindAsync acepta string
            if (vivienda != null)
            {
                _context.Vivienda.Remove(vivienda);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===============================================
        // MÉTODO PRIVADO: Cargar lista de propietarios (USANDO ViewBag)
        // ===============================================
        private async Task PopulatePropietariosList(object? selectedPropietario = null)
        {
            const string ROL_PROPIETARIO = "Propietario";

            var rolPropietario = await _context.Rol
                .FirstOrDefaultAsync(r => r.RolNombre == ROL_PROPIETARIO);

            // Manejo de caso donde el rol no existe (para evitar excepciones NullReference)
            if (rolPropietario == null)
            {
                ViewBag.Propietario = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.ErrorMessagePropietario = "Advertencia: El rol 'Propietario' no está configurado. Asegúrese de que el rol exista en la tabla 'Rol'.";
                return;
            }

            var propietarios = await _context.Persona
                .Where(p => p.PersonaRoles.Any(pr => pr.IdRol == rolPropietario.IdRol))
                .OrderBy(p => p.Nombres)
                .Select(p => new SelectListItem
                {
                    Value = p.NumIdentidad,
                    Text = $"{p.Nombres} {p.Apellidos} ({p.NumIdentidad})"
                })
                .ToListAsync();

            ViewBag.Propietario = new SelectList(propietarios, "Value", "Text", selectedPropietario);
        }
        
        // ===============================================
        // MÉTODO PRIVADO: Cargar lista de Ciudades (NUEVO)
        // ===============================================
        private async Task PopulateCiudadesList(object? selectedCiudad = null)
        {
            var ciudades = await _context.Ciudad
                .OrderBy(c => c.CiudadNombre)
                .ToListAsync();

            ViewBag.IdCiudad = new SelectList(ciudades, "IdCiudad", "CiudadNombre", selectedCiudad);
        }

        // ===============================================
        // MÉTODO PRIVADO: Verifica existencia de Vivienda
        // ===============================================
        // ** CORRECCIÓN: El ID ahora es STRING **
        private bool ViviendaExists(string id)
        {
            // ** CORRECCIÓN CRÍTICA: Cambiado el operador de asignación (=) por el de comparación (==) **
            return (_context.Vivienda?.Any(e => e.CodigoVivienda == id)).GetValueOrDefault();
        }
    }
}