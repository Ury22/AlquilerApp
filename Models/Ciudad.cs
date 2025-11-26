using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AlquilerApp.Models
{
    [Table("Ciudad")]
    public class Ciudad
    {
        [Key]
        public int IdCiudad { get; set; }

        // La propiedad de nombre es 'CiudadNombre'
        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "El nombre de la ciudad es obligatorio."), StringLength(50)]
        [Column("Ciudad")] 
        public string CiudadNombre { get; set; } = string.Empty;

        // Propiedad escalar corregida
        [Required(ErrorMessage = "Debe seleccionar un país.")]
        public int? IdPais { get; set; } 

        // Propiedad de Navegación con Clave Foránea
        [ForeignKey("IdPais")] 
        [AllowNull] 
        public Pais? Pais { get; set; }

        // Colección de Navegación:
        [AllowNull]
        public ICollection<Persona> Personas { get; set; } = new List<Persona>();
        // Dentro de la clase 'Ciudad'
        public ICollection<Vivienda> Viviendas { get; set; } = new List<Vivienda>();
    }
}