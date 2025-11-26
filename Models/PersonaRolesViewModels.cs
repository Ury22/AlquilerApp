using AlquilerApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlquilerApp.Models.ViewModels
{
    // ViewModel utilizado para la vista de edición de Persona, permitiendo asignar roles.
    public class PersonaRolesViewModel
    {
        // La instancia de la persona que se está editando
        public Persona Persona { get; set; } = new Persona();

        // Lista de todos los roles disponibles en la base de datos
        public List<Rol> TodosLosRoles { get; set; } = new List<Rol>();

        // Lista de IDs de los roles que el usuario ha seleccionado en la vista
        [Display(Name = "Roles Asignados")]
        public List<int> RolesAsignadosIds { get; set; } = new List<int>();
    }
}