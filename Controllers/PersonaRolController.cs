using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlquilerApp.Data;
using AlquilerApp.Models;
using AlquilerApp.Models.ViewModels;
using System.Collections.Immutable;

namespace AlquilerApp.Controllers
{
    public class PersonaRolController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonaRolController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PersonaRol/Edit/5 (NoIdentidad de la Persona)
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Buscar la persona por su NoIdentidad
            var persona = await _context.Persona.FindAsync(id);
            if (persona == null)
            {
                return NotFound();
            }

            // 1. Obtener los IDs de roles asignados a esta persona
            // CORREGIDO: Usamos NoIdentidad en lugar de IdPersona
            var rolesAsignadosIds = await _context.PersonaRol
                .Where(pr => pr.NumIdentidad == persona.NumIdentidad)
                .Select(pr => pr.IdRol)
                .ToListAsync();

            // 2. Obtener todos los roles disponibles
            var todosLosRoles = await _context.Rol.ToListAsync();

            // 3. Crear el ViewModel que contiene Persona + Roles
            var viewModel = new PersonaRolesViewModel
            {
                Persona = persona,
                TodosLosRoles = todosLosRoles,
                // Asignamos la lista de IDs de roles obtenidos
                RolesAsignadosIds = rolesAsignadosIds
            };

            return View(viewModel);
        }

        // POST: PersonaRol/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PersonaRolesViewModel viewModel)
        {
            if (id != viewModel.Persona.NumIdentidad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // 1. Obtener la persona del contexto para asegurar que existe
                var persona = await _context.Persona.FindAsync(id);
                if (persona == null)
                {
                    return NotFound();
                }

                // 2. Obtener los roles actuales de la persona
                // CORREGIDO: Usamos NoIdentidad para el filtro
                var rolesActuales = await _context.PersonaRol
                    .Where(pr => pr.NumIdentidad == id)
                    .ToListAsync();
                
                // 3. Crear un HashSet de los IDs de roles que deben estar asignados
                var nuevosRolesIds = new HashSet<int>(viewModel.RolesAsignadosIds ?? new List<int>());

                // 4. Determinar roles a eliminar
                foreach (var rolActual in rolesActuales)
                {
                    if (!nuevosRolesIds.Contains(rolActual.IdRol))
                    {
                        // Si el rol existente no está en la nueva lista, se elimina
                        _context.PersonaRol.Remove(rolActual);
                    }
                    else
                    {
                        // Si el rol ya existe, lo quitamos de nuevosRolesIds para no agregarlo de nuevo
                        nuevosRolesIds.Remove(rolActual.IdRol);
                    }
                }

                // 5. Agregar nuevos roles
                // Iterar sobre los IDs que quedan (los que son nuevos)
                foreach (var nuevoRolId in nuevosRolesIds)
                {
                    // CORREGIDO: Usamos NoIdentidad para el FK
                    var nuevoRol = new PersonaRol
                    {
                        NumIdentidad = persona.NumIdentidad, 
                        IdRol = nuevoRolId
                    };
                    _context.PersonaRol.Add(nuevoRol);
                }

                await _context.SaveChangesAsync();
                
                // Redirigir a la vista de detalles de la Persona
                return RedirectToAction("Details", "Persona", new { id = persona.NumIdentidad });
            }
            
            // Si el modelo no es válido, recargar la información de los roles para la vista
            viewModel.TodosLosRoles = await _context.Rol.ToListAsync();
            return View(viewModel);
        }

        // Las demás acciones (Create, Delete, Index, etc.) de este controlador deben
        // usar el mismo principio: reemplazar IdPersona por NoIdentidad.
    }
}