using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace AlquilerApp.Models
{
    // Mapea a la tabla 'Persona'. Su clave primaria es NumIdentidad (string)
    public class Persona
    {
        // --- CLAVE PRIMARIA (String) ---
        // Corregido: La propiedad en C# se llama 'NumIdentidad' para coincidir con la columna en SQL y el controlador.
        [Key]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(13, MinimumLength = 4, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
        [Column("NumIdentidad", TypeName = "NVARCHAR(13)")] // La columna en la DB se llama 'NumIdentidad'
        [Display(Name = "Número de Identidad")]
        public string NumIdentidad { get; set; } = string.Empty; // <--- CORREGIDO: Usar NumIdentidad

        // --- Campos de Información ---
        
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los 100 caracteres.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        // Se mapea la propiedad 'Email' de C# a la columna 'Correo' en SQL.
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo {0} no es una dirección de correo válida.")]
        [Column("Correo")] // Mapeo a Correo (según tu DDL)
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        // Se mapea la propiedad 'Telefono' de C# a la columna 'TelefonoContacto' en SQL.
        [Display(Name = "Teléfono")]
        [StringLength(20)]
        [Column("TelefonoContacto")] // Mapeo a TelefonoContacto (según tu DDL)
        public string? Telefono { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(255)]
        [Required(ErrorMessage = "La dirección es obligatoria.")] 
        public string Direccion { get; set; } = string.Empty;
        
        // --- Claves Foráneas (FK) ---

        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "Debe seleccionar un {0}.")]
        public int IdSexo { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "Debe seleccionar una {0}.")]
        public int IdCiudad { get; set; }

        // --- Propiedades de Navegación ---

        [ForeignKey("IdSexo")]
        public Sexo? Sexo { get; set; }

        [ForeignKey("IdCiudad")]
        public Ciudad? Ciudad { get; set; }

        // Relación con PersonaRol (una persona puede tener múltiples roles)
        public ICollection<PersonaRol> PersonaRoles { get; set; } = new HashSet<PersonaRol>();
        
        // Relaciones inversas (para seguimiento)
        // La columna NumIdentidad de PersonaRol se relaciona con esta propiedad.
        public ICollection<Vivienda> ViviendasPropias { get; set; } = new HashSet<Vivienda>();
        public ICollection<Alquiler> AlquileresComoPropietario { get; set; } = new HashSet<Alquiler>();
        public ICollection<Alquiler> AlquileresComoInquilino { get; set; } = new HashSet<Alquiler>();
    }
}