using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerApp.Models
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }

        [Column("Rol")] // Mapeamos la propiedad C# a la columna 'Rol' de SQL
        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El nombre del {0} es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del {0} no puede exceder los 50 caracteres.")]
        public string RolNombre { get; set; } = string.Empty;

        public ICollection<PersonaRol> PersonaRoles { get; set; } = new List<PersonaRol>();
    }
}
