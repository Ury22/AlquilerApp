using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlquilerApp.Data;
using AlquilerApp.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AlquilerApp.Controllers
{
    [Route("Reporte")]
    public class ReporteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Configurar licencia QuestPDF
        private void ConfigurarLicencia()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // =========================================
        // VISTA PRINCIPAL
        // =========================================
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var modelo = new ReporteGeneralViewModel
            {
                Personas = await _context.Persona
                    .Include(p => p.Ciudad)
                    .OrderBy(p => p.Apellidos)
                    .ToListAsync(),

                Viviendas = await _context.Vivienda
                    .Include(v => v.Ciudad)
                    .OrderBy(v => v.CodigoVivienda)
                    .ToListAsync(),

                Alquileres = await _context.Alquiler
                    .Include(a => a.Estado)
                    .Include(a => a.InquilinoPersona)
                    .Include(a => a.PropietarioPersona)
                    .OrderByDescending(a => a.FechaAlquiler)
                    .ToListAsync()
            };

            // Indicar explícitamente la vista "ReporteGeneral.cshtml"
            return View("ReporteGeneral", modelo);
        }

        // =========================================
        // PDF GENERAL (Todo en un PDF)
        // =========================================
        [HttpGet("DescargarPDF")]
        public async Task<IActionResult> DescargarPDF()
        {
            ConfigurarLicencia();

            var modelo = new ReporteGeneralViewModel
            {
                Personas = await _context.Persona
                    .Include(p => p.Ciudad)
                    .OrderBy(p => p.Apellidos)
                    .ToListAsync(),

                Viviendas = await _context.Vivienda
                    .Include(v => v.Ciudad)
                    .OrderBy(v => v.CodigoVivienda)
                    .ToListAsync(),

                Alquileres = await _context.Alquiler
                    .Include(a => a.Estado)
                    .Include(a => a.InquilinoPersona)
                    .Include(a => a.PropietarioPersona)
                    .OrderByDescending(a => a.FechaAlquiler)
                    .ToListAsync()
            };

            using var ms = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Content().Column(col =>
                    {
                        col.Item().Text("REPORTE GENERAL DEL SISTEMA").FontSize(22).Bold();

                        // PERSONAS
                        col.Item().Text("\n--- PERSONAS ---").FontSize(16).Bold();
                        foreach (var p in modelo.Personas)
                            col.Item().Text($"{p.Nombres} {p.Apellidos} - {p.NumIdentidad} - {p?.Ciudad?.CiudadNombre}");

                        // VIVIENDAS
                        col.Item().Text("\n--- VIVIENDAS ---").FontSize(16).Bold();
                        foreach (var v in modelo.Viviendas)
                            col.Item().Text($"Vivienda #{v.CodigoVivienda} - {v.DescripcionVivienda} - {v?.Ciudad?.CiudadNombre}");

                        // ALQUILERES
                        col.Item().Text("\n--- ALQUILERES ---").FontSize(16).Bold();
                        foreach (var a in modelo.Alquileres)
                            col.Item().Text($"#{a.CodAlquiler} | {a.FechaAlquiler:d} | Propietario: {a.PropietarioPersona?.Nombres} → Inquilino: {a.InquilinoPersona?.Nombres} | Estado: {a.Estado?.EstadoNombre}");
                    });
                });
            }).GeneratePdf(ms);

            return File(ms.ToArray(), "application/pdf", "ReporteGeneral.pdf");
        }

        // =========================================
        // PDF INDIVIDUAL: PERSONAS
        // =========================================
        [HttpGet("PersonasPDF")]
        public async Task<IActionResult> PersonasPDF()
        {
            ConfigurarLicencia();

            var personas = await _context.Persona
                .Include(p => p.Ciudad)
                .OrderBy(p => p.Apellidos)
                .ToListAsync();

            using var ms = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text("Reporte de Personas").FontSize(20).Bold();
                    page.Content().Column(col =>
                    {
                        foreach (var p in personas)
                            col.Item().Text($"{p.Nombres} {p.Apellidos} - {p.NumIdentidad} - {p?.Ciudad?.CiudadNombre}");
                    });
                });
            }).GeneratePdf(ms);

            return File(ms.ToArray(), "application/pdf", "ReportePersonas.pdf");
        }

        // =========================================
        // PDF INDIVIDUAL: VIVIENDAS
        // =========================================
        [HttpGet("ViviendasPDF")]
        public async Task<IActionResult> ViviendasPDF()
        {
            ConfigurarLicencia();

            var viviendas = await _context.Vivienda
                .Include(v => v.Ciudad)
                .OrderBy(v => v.CodigoVivienda)
                .ToListAsync();

            using var ms = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text("Reporte de Viviendas").FontSize(20).Bold();
                    page.Content().Column(col =>
                    {
                        foreach (var v in viviendas)
                            col.Item().Text($"Vivienda #{v.CodigoVivienda} - {v.DescripcionVivienda} - {v?.Ciudad?.CiudadNombre}");
                    });
                });
            }).GeneratePdf(ms);

            return File(ms.ToArray(), "application/pdf", "ReporteViviendas.pdf");
        }

        // =========================================
        // PDF INDIVIDUAL: ALQUILERES
        // =========================================
        [HttpGet("AlquileresPDF")]
        public async Task<IActionResult> AlquileresPDF()
        {
            ConfigurarLicencia();

            var alquileres = await _context.Alquiler
                .Include(a => a.Estado)
                .Include(a => a.InquilinoPersona)
                .Include(a => a.PropietarioPersona)
                .OrderByDescending(a => a.FechaAlquiler)
                .ToListAsync();

            using var ms = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text("Reporte de Alquileres").FontSize(20).Bold();
                    page.Content().Column(col =>
                    {
                        foreach (var a in alquileres)
                            col.Item().Text($"#{a.CodAlquiler} | {a.FechaAlquiler:d} | Propietario: {a.PropietarioPersona?.Nombres} → Inquilino: {a.InquilinoPersona?.Nombres} | Estado: {a.Estado?.EstadoNombre}");
                    });
                });
            }).GeneratePdf(ms);

            return File(ms.ToArray(), "application/pdf", "ReporteAlquileres.pdf");
        }
    }
}
