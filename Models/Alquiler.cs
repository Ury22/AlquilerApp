using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; 
using System;

namespace AlquilerApp.Models
{
    [Table("Alquiler")] // Añadido Table attribute para claridad, si no estaba
    public class Alquiler
    {
        [Key]
        public int CodAlquiler { get; set; }

        // --- LLAVES FORÁNEAS EXPLÍCITAS ---
        
        // ******************************************************
        // FK a Vivienda (CORRECCIÓN CRÍTICA: Debe ser string)
        // ******************************************************
        [Required]
        [Display(Name = "Vivienda")]
       public string CodigoVivienda { get; set; } = string.Empty;
        
        // FK a Estado
        [Required]
        [Display(Name = "Estado")]
        public int IdEstado { get; set; }

        // FK a Persona (Propietario)
        [Required]
        [Display(Name = "Propietario (Identidad)")]
        [StringLength(13)] 
        public string Propietario { get; set; } = string.Empty; 

        // FK a Persona (Inquilino)
        [Required]
        [Display(Name = "Inquilino (Identidad)")]
        [StringLength(13)] 
        public string Inquilino { get; set; } = string.Empty; 

        // --- PROPIEDADES DE DATOS ---
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Alquiler")]
        public DateTime FechaAlquiler { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Finalización")]
        public DateTime FechaFinalizacion { get; set; }

        [Required]
        [Column("CuotaMensual", TypeName = "decimal(18, 2)")]
        [Display(Name = "Cuota Mensual")]
        public decimal CuotaMensual { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Depósito de Garantía")]
        public decimal DepositoGarantia { get; set; }

        // --- PROPIEDADES DE NAVEGACIÓN ---

        // Relación 1:N con Vivienda
        [ValidateNever]
        [ForeignKey("CodigoVivienda")]
        [InverseProperty("HistorialAlquileres")]
        public Vivienda? Vivienda { get; set; }

        [ValidateNever]
        [ForeignKey("IdEstado")] // Se necesita para la navegación
        public Estado? Estado { get; set; }
        
        [ValidateNever]
        [ForeignKey("Propietario")] // Se necesita para la navegación
        [InverseProperty("AlquileresComoPropietario")]
        public Persona? PropietarioPersona { get; set; }

        [ValidateNever]
        [ForeignKey("Inquilino")] // Se necesita para la navegación
        [InverseProperty("AlquileresComoInquilino")]
        public Persona? InquilinoPersona { get; set; }
    }
}