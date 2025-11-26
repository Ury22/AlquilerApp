using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AlquilerApp.Models
{
    [Table("Vivienda")]
    public class Vivienda
    {
        // === CLAVE PRIMARIA (PK) ===
        [Key]
        [Column("CodigoVivienda")]
        [Display(Name = "Código")]
        public string CodigoVivienda { get; set; } = string.Empty; 

        // === CLAVES FORÁNEAS (FK) ===
        
        [Required]
        [Display(Name = "Propietario (Identidad)")]
        [StringLength(13)]
        public string Propietario { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Ciudad")]
        public int IdCiudad { get; set; }

        [Display(Name = "Alquiler Activo")]
        public int? CodigoAlquilerActivo { get; set; } 
        

        // === PROPIEDADES DE LA VIVIENDA ===

        // EN LA BD ES INT => LO CORREGIMOS A INT
        [Required]
        [Display(Name = "Número de Vivienda")]
        public int NumVivienda { get; set; }

        // EN LA BD ES INT => LO CORREGIMOS A INT
        [Required]
        [Display(Name = "Número de Calle")]
        public int NumCalle { get; set; }

        // EN LA BD ES INT => LO CORREGIMOS A INT?
        // pero es NULLABLE, así que lo dejamos como int?
        [Display(Name = "Número de Bloque")]
        public int? NumBloque { get; set; }
        
        [Required]
        [Range(1, 10)]
        [Display(Name = "Capacidad Máx.")]
        public int CapacidadPersonas { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Dormitorios")]
        public int CantDormitorios { get; set; } 

        [Required]
        [Range(1, 4)]
        [Display(Name = "Baños")]
        public int CantBanios { get; set; } 

        [Display(Name = "Descripción")]
        [StringLength(255)]
        public string DescripcionVivienda { get; set; } = string.Empty;
        
        // === PROPIEDADES DE NAVEGACIÓN ===
        
        [ValidateNever]
        [ForeignKey("Propietario")]
        public Persona? PropietarioPersona { get; set; }

        [ValidateNever]
        [ForeignKey("IdCiudad")]
        public Ciudad? Ciudad { get; set; }

        [ValidateNever]
        [ForeignKey("CodigoAlquilerActivo")] 
        public Alquiler? AlquilerActual { get; set; }
        
        [ValidateNever]
        [InverseProperty("Vivienda")]
        public ICollection<Alquiler>? HistorialAlquileres { get; set; }
    }
}
