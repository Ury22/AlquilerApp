using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlquilerApp.Data;
using AlquilerApp.Models;
using AlquilerApp.Models.ViewModels;

namespace AlquilerApp.Controllers
{
    public class PersonaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Persona
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Persona
                .Include(p => p.Ciudad)
                .Include(p => p.Ciudad.Pais)
                .Include(p => p.Sexo);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Persona/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .Include(p => p.Ciudad)
                .Include(p => p.Sexo)
                .Include(p => p.PersonaRoles)
                    .ThenInclude(pr => pr.Rol)
                .FirstOrDefaultAsync(m => m.NumIdentidad == id);

            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // GET: Persona/Create
        public IActionResult Create()
        {
            ViewData["IdCiudad"] = new SelectList(_context.Ciudad, "IdCiudad", "CiudadNombre");
            ViewData["IdSexo"] = new SelectList(_context.Sexo, "IdSexo", "SexoNombre");
            return View();
        }

        // POST: Persona/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumIdentidad,Nombres,Apellidos,FechaNacimiento,Email,Telefono,Direccion,IdSexo,IdCiudad")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                _context.Add(persona);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["IdCiudad"] = new SelectList(_context.Ciudad, "IdCiudad", "CiudadNombre", persona.IdCiudad);
            ViewData["IdSexo"] = new SelectList(_context.Sexo, "IdSexo", "SexoNombre", persona.IdSexo);
            return View(persona);
        }

        // GET: Persona/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .Include(p => p.PersonaRoles)
                .FirstOrDefaultAsync(m => m.NumIdentidad == id);

            if (persona == null)
            {
                return NotFound();
            }

            var viewModel = new PersonaRolesViewModel
            {
                Persona = persona,
                TodosLosRoles = await _context.Rol.ToListAsync(),
                RolesAsignadosIds = persona.PersonaRoles?.Select(pr => pr.IdRol).ToList() ?? new List<int>()
            };

            ViewData["IdCiudad"] = new SelectList(_context.Ciudad, "IdCiudad", "CiudadNombre", persona.IdCiudad);
            ViewData["IdSexo"] = new SelectList(_context.Sexo, "IdSexo", "SexoNombre", persona.IdSexo);

            return View(viewModel);
        }

  [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(string id, PersonaRolesViewModel viewModel)
{
    // 1. VALIDACIÓN DE SEGURIDAD: Verifica que el ID de la URL coincida con el del modelo
    if (id != viewModel.Persona.NumIdentidad)
    {
        return NotFound();
    }

    // 2. VALIDACIÓN DEL MODELO
    if (ModelState.IsValid)
    {
        try
        {
            // A. BUSCAR LA PERSONA EN LA BD (Incluyendo sus roles actuales)
            // Nota: Uso _context.Persona y _context.PersonaRoles según tus nombres de tabla
            var personaDb = await _context.Persona
                .Include(p => p.PersonaRoles)
                .FirstOrDefaultAsync(p => p.NumIdentidad == id);

            if (personaDb == null)
            {
                return NotFound();
            }

            // B. ACTUALIZAR DATOS BASICOS
            // Pasamos los valores del formulario (viewModel) a la entidad de la base de datos (personaDb)
            personaDb.Nombres = viewModel.Persona.Nombres;
            personaDb.Apellidos = viewModel.Persona.Apellidos;
            personaDb.FechaNacimiento = viewModel.Persona.FechaNacimiento;
            personaDb.Email = viewModel.Persona.Email;
            personaDb.Telefono = viewModel.Persona.Telefono;
            personaDb.Direccion = viewModel.Persona.Direccion;
            personaDb.IdSexo = viewModel.Persona.IdSexo;
            personaDb.IdCiudad = viewModel.Persona.IdCiudad;

            // C. ACTUALIZAR ROLES (Lógica para Dropdown / Lista)
            
            // Paso 1: Eliminar roles anteriores. 
            // Como es un dropdown, asumimos que el usuario solo puede tener 1 rol principal a la vez.
            // Si existen roles previos, los borramos.
            if (personaDb.PersonaRoles != null && personaDb.PersonaRoles.Any())
            {
                _context.PersonaRol.RemoveRange(personaDb.PersonaRoles);
            }

            // Paso 2: Agregar el nuevo rol seleccionado
            if (viewModel.RolesAsignadosIds != null && viewModel.RolesAsignadosIds.Any())
            {
                foreach (var nuevoRolId in viewModel.RolesAsignadosIds)
                {
                    var nuevaRelacion = new PersonaRol // Asegúrate que tu clase se llame PersonaRole o PersonasRoles
                    {
                        NumIdentidad = id,
                        IdRol = nuevoRolId
                        // Si tu tabla intermedia tiene campos extra como fecha, agrégalos aquí:
                        // FechaAsignacion = DateTime.Now 
                    };
                    _context.Add(nuevaRelacion);
                }
            }

            // D. GUARDAR CAMBIOS
            await _context.SaveChangesAsync();
            
            // Redirigir a la lista principal
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PersonaExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    // 3. SI HAY ERROR EN EL FORMULARIO (Recargar listas)
    // Si llegamos aquí, algo falló en la validación. Recargamos las listas para que el usuario no vea la pantalla vacía.
    
    viewModel.TodosLosRoles = await _context.Rol.ToListAsync();
    
    // Usamos los nombres de propiedades de tu contexto original
    ViewData["IdCiudad"] = new SelectList(_context.Ciudad, "IdCiudad", "CiudadNombre", viewModel.Persona.IdCiudad);
    ViewData["IdSexo"] = new SelectList(_context.Sexo, "IdSexo", "SexoNombre", viewModel.Persona.IdSexo);

    return View(viewModel);
}

        // GET: Persona/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .Include(p => p.Ciudad)
                .Include(p => p.Sexo)
                .FirstOrDefaultAsync(m => m.NumIdentidad == id);

            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // POST: Persona/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string NumIdentidad)
        {
            var persona = await _context.Persona.FindAsync(NumIdentidad);
            if (persona != null)
            {
                _context.Persona.Remove(persona);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(string id)
        {
            return _context.Persona.Any(e => e.NumIdentidad == id);
        }

        private async Task UpdatePersonaRoles(Persona personaToUpdate, List<int> rolesSeleccionados)
        {
            if (rolesSeleccionados == null)
            {
                rolesSeleccionados = new List<int>();
            }

            if (personaToUpdate.PersonaRoles == null)
            {
                await _context.Entry(personaToUpdate).Collection(p => p.PersonaRoles).LoadAsync();
            }

            var rolesActualesIds = personaToUpdate.PersonaRoles.Select(pr => pr.IdRol).ToList();

            var rolesAEliminar = personaToUpdate.PersonaRoles
                .Where(pr => !rolesSeleccionados.Contains(pr.IdRol))
                .ToList();

            foreach (var rol in rolesAEliminar)
            {
                _context.PersonaRol.Remove(rol);
            }

            var rolesAAgregarIds = rolesSeleccionados
                .Where(id => !rolesActualesIds.Contains(id))
                .ToList();

            foreach (var idRol in rolesAAgregarIds)
            {
                _context.PersonaRol.Add(new PersonaRol
                {
                    NumIdentidad = personaToUpdate.NumIdentidad,
                    IdRol = idRol
                });
            }
        }
    }
}