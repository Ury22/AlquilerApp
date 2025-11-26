using Microsoft.EntityFrameworkCore;
using AlquilerApp.Models;

namespace AlquilerApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- DbSets ---
        public DbSet<Pais> Pais { get; set; } = default!;
        public DbSet<Ciudad> Ciudad { get; set; } = default!;
        public DbSet<Sexo> Sexo { get; set; } = default!;
        public DbSet<Rol> Rol { get; set; } = default!;
        
        public DbSet<Estado> Estado { get; set; } = default!; 
        public DbSet<Vivienda> Vivienda { get; set; } = default!;
        public DbSet<Alquiler> Alquiler { get; set; } = default!;

        // Módulo Persona
        public DbSet<Persona> Persona { get; set; } = default!;
        
        // Tabla de Unión
        public DbSet<PersonaRol> PersonaRol { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // =======================================================
            // 1. CONFIGURACIÓN DE LA RELACIÓN M:M (Persona <-> Rol)
            // =======================================================
            modelBuilder.Entity<PersonaRol>()
                .HasKey(pr => new { pr.NumIdentidad, pr.IdRol });

            modelBuilder.Entity<PersonaRol>()
                .HasOne(pr => pr.Persona)
                .WithMany(p => p.PersonaRoles)
                .HasForeignKey(pr => pr.NumIdentidad)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PersonaRol>()
                .HasOne(pr => pr.Rol)
                .WithMany(r => r.PersonaRoles)
                .HasForeignKey(pr => pr.IdRol)
                .OnDelete(DeleteBehavior.NoAction);
            
            
            // =======================================================
            // 2. CONFIGURACIÓN DE VIVIENDA
            // =======================================================
            
            // Relación 1:N (Propietario -> Vivienda)
            modelBuilder.Entity<Vivienda>()
                .HasOne(v => v.PropietarioPersona) 
                .WithMany(p => p.ViviendasPropias) 
                .HasForeignKey(v => v.Propietario) 
                .HasPrincipalKey(p => p.NumIdentidad) 
                .OnDelete(DeleteBehavior.Restrict); 

            // Relación 1:N (Ciudad -> Vivienda). Se eliminó .HasColumnName()
            modelBuilder.Entity<Vivienda>()
                .HasOne(v => v.Ciudad)
                .WithMany(c => c.Viviendas) 
                .HasForeignKey(v => v.IdCiudad)
                .IsRequired();

            // =======================================================
            // 3. CONFIGURACIÓN DE ALQUILER 
            // =======================================================
            
            // ----------------------------------------------------
            // CONFIGURACIÓN DE RELACIONES PARA LA TABLA ALQUILER
            // ----------------------------------------------------

         
            
            // 2. Relación Alquiler -> Estado (Usa IdEstado como FK)
            modelBuilder.Entity<Alquiler>()
                .HasOne(a => a.Estado)
                .WithMany(e => e.Alquileres) 
                .HasForeignKey(a => a.IdEstado)
                .IsRequired();

            // 3. Relación Alquiler -> Persona (Propietario)
            modelBuilder.Entity<Alquiler>()
                .HasOne(a => a.PropietarioPersona)
                .WithMany(p => p.AlquileresComoPropietario) 
                .HasForeignKey(a => a.Propietario) 
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Relación Alquiler -> Persona (Inquilino)
            modelBuilder.Entity<Alquiler>()
                .HasOne(a => a.InquilinoPersona)
                .WithMany(p => p.AlquileresComoInquilino) 
                .HasForeignKey(a => a.Inquilino) 
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // =======================================================
            // 4. CONFIGURACIÓN DE OTRAS ENTIDADES (Ciudad -> País)
            // =======================================================
            
            // Relación 1:N (País -> Ciudad). Se eliminó .HasColumnName()
            modelBuilder.Entity<Ciudad>()
                .HasOne(c => c.Pais) 
                .WithMany(p => p.Ciudades) 
                .HasForeignKey(c => c.IdPais) 
                .IsRequired();
        }
    }
}