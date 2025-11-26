using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlquilerApp.Models
{
    [Table("Estado")]
    public class Estado
    {
        [Key]
        public int IdEstado { get; set; }

        [Required, StringLength(50)]
        [Column("Estado")] // Conecta 'EstadoNombre' con la columna 'Estado'
        public string EstadoNombre { get; set; } = string.Empty;

        public ICollection<Alquiler> Alquileres { get; set; } = new List<Alquiler>();
    }
}