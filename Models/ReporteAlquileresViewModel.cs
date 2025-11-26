using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlquilerApp.Models;
using AlquilerApp.Models.ViewModels;
using AlquilerApp.Data;
using System.Reflection.Metadata;

using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public class ReporteController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReporteController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new ReporteGeneralViewModel
        {
            Personas = await _context.Persona
                .Include(p => p.Ciudad)
                .ToListAsync(),

            Viviendas = await _context.Vivienda
                .Include(v => v.Ciudad)
                .ToListAsync(),

            Alquileres = await _context.Alquiler
                .Include(a => a.InquilinoPersona)
                .Include(a => a.PropietarioPersona)
                .Include(a => a.Estado)
                .ToListAsync()
        };

        return View(model);
    }

    public async Task<IActionResult> DescargarPDF()
    {
        var modelo = new ReporteGeneralViewModel
        {
            Personas = await _context.Persona
                .Include(p => p.Ciudad)
                .ToListAsync(),

            Viviendas = await _context.Vivienda
                .Include(v => v.Ciudad)
                .ToListAsync(),

            Alquileres = await _context.Alquiler
                .Include(a => a.InquilinoPersona)
                .Include(a => a.PropietarioPersona)
                .Include(a => a.Estado)
                .ToListAsync()
        };

        var pdfBytes = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Content().Column(col =>
                {
                    col.Item().Text("REPORTE GENERAL DEL SISTEMA").FontSize(22).Bold();

                    // PERSONAS
                    col.Item().Text("\nPERSONAS").FontSize(16).Bold();
                    foreach (var p in modelo.Personas)
                        col.Item().Text($"{p.Nombres} {p.Apellidos} - {p.NumIdentidad} - {p?.Ciudad?.CiudadNombre}");

                    // VIVIENDAS
                    col.Item().Text("\nVIVIENDAS").FontSize(16).Bold();
                    foreach (var v in modelo.Viviendas)
                        col.Item().Text($"#{v.CodigoVivienda} - {v.DescripcionVivienda} - {v?.Ciudad?.CiudadNombre}");

                    // ALQUILERES
                    col.Item().Text("\nALQUILERES").FontSize(16).Bold();
                    foreach (var a in modelo.Alquileres)
                        col.Item().Text(
                            $"#{a.CodAlquiler} | {a.FechaAlquiler:d} | {a.PropietarioPersona?.Nombres} → {a.InquilinoPersona?.Nombres} | {a.Estado?.EstadoNombre}"
                        );
                });
            });
        })
        .GeneratePdf();

        return File(pdfBytes, "application/pdf", "ReporteGeneral.pdf");
    }
}
