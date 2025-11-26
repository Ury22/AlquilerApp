using System.Collections.Generic;

namespace AlquilerApp.Models.ViewModels
{
    public class ReporteGeneralViewModel
    {
        public List<Persona> Personas { get; set; } = new();
        public List<Vivienda> Viviendas { get; set; } = new();
        public List<Alquiler> Alquileres { get; set; } = new();
    }
}
