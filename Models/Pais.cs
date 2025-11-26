using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerApp.Models
{
    [Table("Pais")] // Asegura que busque la tabla "Pais"
    public class Pais
    {
        [Key]
        public int IdPais { get; set; }

        [Display(Name = "País")]
        [Required(ErrorMessage = "El nombre del país es obligatorio.")]
        [StringLength(50)]
        [Column("Pais")] // <--- ¡MAGIA! Conecta 'PaisNombre' con la columna 'Pais' de SQL
        public string PaisNombre { get; set; } = string.Empty;

        

        public ICollection<Ciudad> Ciudades { get; set; } = new List<Ciudad>();
    }
}