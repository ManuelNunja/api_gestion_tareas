using Microsoft.EntityFrameworkCore;
using api_gestion_tareas.Models;

namespace api_gestion_tareas.Data
{
    public class TaskManagementContext : DbContext
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
        {
        }

        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Archivo> Archivos { get; set; }
        public DbSet<Estado> Estados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Empresa>().ToTable("TB_EMPRESAS", schema: "GESTION_TAREAS");
            modelBuilder.Entity<Tarea>().ToTable("TB_TAREAS", schema: "GESTION_TAREAS");
            modelBuilder.Entity<Archivo>().ToTable("TB_ARCHIVOS", schema: "GESTION_TAREAS");
            modelBuilder.Entity<Estado>().ToTable("TB_ESTADO", schema: "GESTION_TAREAS");

            modelBuilder.Entity<Empresa>().Property(e => e.EmpresaId).HasColumnName("EMP_EMPRESAID");
            modelBuilder.Entity<Empresa>().Property(e => e.Nombre).HasColumnName("EMP_NOMBRE");
            modelBuilder.Entity<Empresa>().Property(e => e.Direccion).HasColumnName("EMP_DIRECCION");
            modelBuilder.Entity<Empresa>().Property(e => e.Telefono).HasColumnName("EMP_TELEFONO");
            modelBuilder.Entity<Empresa>().Property(e => e.CorreoElectronico).HasColumnName("EMP_CORREOELECTRONICO");
            modelBuilder.Entity<Empresa>().Property(e => e.FechaRegistro).HasColumnName("EMP_FECHAREGISTRO");
            modelBuilder.Entity<Empresa>().Property(e => e.Activo).HasColumnName("EMP_ACTIVO");

            modelBuilder.Entity<Tarea>().Property(t => t.TareaId).HasColumnName("TAR_TAREAID");
            modelBuilder.Entity<Tarea>().Property(t => t.Titulo).HasColumnName("TAR_TITULO");
            modelBuilder.Entity<Tarea>().Property(t => t.Descripcion).HasColumnName("TAR_DESCRIPCION");
            modelBuilder.Entity<Tarea>().Property(t => t.EmpresaId).HasColumnName("TAR_EMPRESAID");
            modelBuilder.Entity<Tarea>().Property(t => t.FechaVencimiento).HasColumnName("TAR_FECHAVENCIMIENTO");
            modelBuilder.Entity<Tarea>().Property(t => t.EstadoId).HasColumnName("TAR_ESTADOID");
            modelBuilder.Entity<Tarea>().Property(t => t.FechaInicio).HasColumnName("TAR_FECHAINICIO");
            modelBuilder.Entity<Tarea>().Property(t => t.FechaFin).HasColumnName("TAR_FECHAFIN");
            modelBuilder.Entity<Tarea>().Property(t => t.TiempoTranscurrido).HasColumnName("TAR_TIEMPO_TRANSCURRIDO");
            modelBuilder.Entity<Tarea>().Property(t => t.Activo).HasColumnName("TAR_ACTIVO");

            //modelBuilder.Entity<Archivo>().Property(a => a.ArchivoId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Archivo>().Property(a => a.ArchivoId).HasColumnName("ARC_ARCHIVOID");
            modelBuilder.Entity<Archivo>().Property(a => a.TareaId).HasColumnName("ARC_TAREAID");
            modelBuilder.Entity<Archivo>().Property(a => a.NombreArchivo).HasColumnName("ARC_NOMBREARCHIVO");
            modelBuilder.Entity<Archivo>().Property(a => a.TipoArchivo).HasColumnName("ARC_TIPOARCHIVO");
            modelBuilder.Entity<Archivo>().Property(a => a.RutaArchivo).HasColumnName("ARC_RUTAARCHIVO");
            modelBuilder.Entity<Archivo>().Property(a => a.FechaSubida).HasColumnName("ARC_FECHASUBIDA");

            modelBuilder.Entity<Estado>().Property(e => e.EstadoId).HasColumnName("EST_ESTADOID");
            modelBuilder.Entity<Estado>().Property(e => e.Nombre).HasColumnName("EST_NOMBRE");
        }
    }
}

