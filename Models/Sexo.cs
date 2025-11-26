using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerApp.Models
{
    [Table("Sexo")]
    public class Sexo
    {
        [Key]
        public int IdSexo { get; set; }

        // La propiedad de nombre es 'SexoNombre'
        [Column("Sexo")] 
        [Display(Name = "Género / Sexo")] // Etiqueta amigable para las vistas
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder los 50 caracteres.")]
        public string SexoNombre { get; set; } = string.Empty;

        public ICollection<Persona> Personas { get; set; } = new List<Persona>();
    }
}