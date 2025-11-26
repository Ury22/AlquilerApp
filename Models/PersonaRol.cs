using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerApp.Models
{
    // Modelo de tabla intermedia para la relación muchos a muchos entre Persona y Rol
    public class PersonaRol
    {
        // Clave Foránea de Persona. 
        // ¡CAMBIO CLAVE! Usamos NumIdentidad para coincidir con la columna real de la tabla Persona (visto en los logs).
        [Required]
        [StringLength(20)]
        public string NumIdentidad { get; set; } = string.Empty; // <-- RENOMBRADO A NumIdentidad

        // Clave Foránea de Rol
        [Required]
        public int IdRol { get; set; }

        // Propiedad de Navegación a Persona
        // La ForeignKey apunta a la propiedad NumIdentidad de la clase Persona.
        [ForeignKey("NumIdentidad")]
        public virtual Persona Persona { get; set; } = default!; 

        // Propiedad de Navegación a Rol
        [ForeignKey("IdRol")]
        public virtual Rol Rol { get; set; } = default!; 
    }
}