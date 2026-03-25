using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Bitacora;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cliente;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Configuracion;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesDepartamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesHabitacion;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.SolicitudLimpieza;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.TipoDepartamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.TipoHabitacion;
using System.Data.Entity;

namespace HotelPrado.AccesoADatos
{
    public class Contexto : DbContext
    {
        public Contexto() : base("Contexto")
        {
            // Aumentar timeout de comando a 30 segundos para conexiones lentas
            this.Database.CommandTimeout = 30;
            // Deshabilitar la inicialización automática de la base de datos para mejor rendimiento
            Database.SetInitializer<Contexto>(null);
            
            // CRÍTICO: Deshabilitar Lazy Loading para evitar consultas N+1
            this.Configuration.LazyLoadingEnabled = false;
            // Deshabilitar Proxy Creation para mejor rendimiento
            this.Configuration.ProxyCreationEnabled = false;
            // Deshabilitar AutoDetectChanges para mejor rendimiento en operaciones masivas
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartamentoTabla>().ToTable("Departamento");
            modelBuilder.Entity<TipoDepartamentoTabla>().ToTable("TipoDepartamento");
            modelBuilder.Entity<ApplicationUser>().ToTable("Cliente");
            modelBuilder.Entity<BitacoraTabla>().ToTable("bitacoraEventos");
            modelBuilder.Entity<CitasTabla>().ToTable("Citas");
            modelBuilder.Entity<ColaboradorTabla>().ToTable("Colaborador");
            modelBuilder.Entity<ImagenesDepartamentoTabla>().ToTable("ImagenesDepartamento");
            modelBuilder.Entity<HabitacionesTabla>().ToTable("Habitaciones");
            modelBuilder.Entity<TipoHabitacionTabla>().ToTable("TipoHabitacion");
            modelBuilder.Entity<ImagenesHabitacionTabla>().ToTable("ImagenesHabitacion");
            modelBuilder.Entity<ReservasTabla>().ToTable("Reservas");
            // Configuración explícita para IdCliente como string (NVARCHAR)
            modelBuilder.Entity<ReservasTabla>()
                .Property(r => r.IdCliente)
                .HasColumnType("nvarchar")
                .HasMaxLength(128);
            modelBuilder.Entity<CargosTabla>().ToTable("Cargos");
            
            // Configuración de FacturasTabla
            modelBuilder.Entity<FacturasTabla>().ToTable("Facturas");
            // Id es la clave primaria (facturación electrónica)
            modelBuilder.Entity<FacturasTabla>()
                .HasKey(f => f.Id);
            // IdFactura es IDENTITY (se genera automáticamente)
            modelBuilder.Entity<FacturasTabla>()
                .Property(f => f.IdFactura)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            
            modelBuilder.Entity<TemporadasTabla>().ToTable("Temporadas");
            
            // Configuración completa de MantenimientoTabla con mapeo explícito de columnas
            modelBuilder.Entity<MantenimientoTabla>().ToTable("Mantenimiento");
            modelBuilder.Entity<MantenimientoTabla>()
                .Property(m => m.IdMantenimiento)
                .HasColumnName("IdMantenimiento");
            modelBuilder.Entity<MantenimientoTabla>()
                .Property(m => m.Descripcion)
                .HasColumnName("Descripcion");
            modelBuilder.Entity<MantenimientoTabla>()
                .Property(m => m.Estado)
                .HasColumnName("Estado");
            modelBuilder.Entity<MantenimientoTabla>()
                .Property(m => m.idHabitacion)
                .HasColumnName("IdHabitacion");
            modelBuilder.Entity<MantenimientoTabla>()
                .Property(m => m.idDepartamento)
                .HasColumnName("idDepartamento");
            modelBuilder.Entity<MantenimientoTabla>()
                .Property(m => m.DepartamentoNombre)
                .HasColumnName("DepartamentoNombre");

            // Configuración completa de SolicitudLimpiezaTabla con mapeo explícito de columnas
            modelBuilder.Entity<SolicitudLimpiezaTabla>().ToTable("SolicitudesLimpieza");
            modelBuilder.Entity<SolicitudLimpiezaTabla>()
                .Property(s => s.IdSolicitudLimpieza)
                .HasColumnName("IdSolicitudLimpieza");
            modelBuilder.Entity<SolicitudLimpiezaTabla>()
                .Property(s => s.Descripcion)
                .HasColumnName("Descripcion");
            modelBuilder.Entity<SolicitudLimpiezaTabla>()
                .Property(s => s.Estado)
                .HasColumnName("Estado");
            modelBuilder.Entity<SolicitudLimpiezaTabla>()
                .Property(s => s.idHabitacion)
                .HasColumnName("idHabitacion");
            modelBuilder.Entity<SolicitudLimpiezaTabla>()
                .Property(s => s.idDepartamento)
                .HasColumnName("idDepartamento");
            modelBuilder.Entity<SolicitudLimpiezaTabla>()
                .Property(s => s.DepartamentoNombre)
                .HasColumnName("DepartamentoNombre");
            modelBuilder.Entity<SolicitudLimpiezaTabla>()
                .Property(s => s.FechaSolicitud)
                .HasColumnName("FechaSolicitud");

            // Configuración de tablas de configuración
            modelBuilder.Entity<ConfiguracionHeroBannerTabla>().ToTable("ConfiguracionHeroBanner");
            modelBuilder.Entity<ConfiguracionPreciosDepartamentosTabla>().ToTable("ConfiguracionPreciosDepartamentos");

        }
        public DbSet<DepartamentoTabla> DepartamentoTabla { get; set; }
        public DbSet<TipoDepartamentoTabla> TipoDepartamentoTabla { get; set; }
        public DbSet<ApplicationUser> ClienteTabla { get; set; }
        public DbSet<BitacoraTabla> BitacoraTabla { get; set; }
        public DbSet<CitasTabla> CitasTabla { get; set; }
        public DbSet<ColaboradorTabla> ColaboradorTabla { get; set; }
        public DbSet<ImagenesDepartamentoTabla> ImagenesDepartamentoTabla { get; set; }
        public DbSet<HabitacionesTabla> HabitacionesTabla { get; set; }
        public DbSet<TipoHabitacionTabla> TipoHabitacionTabla { get; set; }
        public DbSet<ImagenesHabitacionTabla> ImagenesHabitacionesTabla { get; set; }
        public DbSet<ReservasTabla> ReservasTabla { get; set; }
        public DbSet<MantenimientoTabla> MantenimientoTabla { get; set; }
        public DbSet<SolicitudLimpiezaTabla> SolicitudLimpiezaTabla { get; set; }
        public DbSet<CargosTabla> CargosTabla { get; set; }
        public DbSet<FacturasTabla> FacturasTabla { get; set; }
        public DbSet<TemporadasTabla> TemporadasTabla { get; set; }
        public DbSet<ConfiguracionHeroBannerTabla> ConfiguracionHeroBannerTabla { get; set; }
        public DbSet<ConfiguracionPreciosDepartamentosTabla> ConfiguracionPreciosDepartamentosTabla { get; set; }
    }
}
