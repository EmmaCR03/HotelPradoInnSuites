using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Bitacora;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cliente;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesDepartamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesHabitacion;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.TipoDepartamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.TipoHabitacion;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos
{
    public class Contexto : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartamentoTabla>().ToTable("Departamento");
            modelBuilder.Entity<TipoDepartamentoTabla>().ToTable("TipoDepartamento");
            modelBuilder.Entity<ClienteTabla>().ToTable("Cliente");
            modelBuilder.Entity<BitacoraTabla>().ToTable("bitacoraEventos");
            modelBuilder.Entity<CitasTabla>().ToTable("Citas");
            modelBuilder.Entity<ColaboradorTabla>().ToTable("Colaborador");
            modelBuilder.Entity<ImagenesDepartamentoTabla>().ToTable("ImagenesDepartamento");
            modelBuilder.Entity<HabitacionesTabla>().ToTable("Habitaciones");
            modelBuilder.Entity<TipoHabitacionTabla>().ToTable("TipoHabitacion");
            modelBuilder.Entity<ImagenesHabitacionTabla>().ToTable("ImagenesHabitacion");
            modelBuilder.Entity<ReservasTabla>().ToTable("Reservas");




        }
        public DbSet<DepartamentoTabla> DepartamentoTabla { get; set; }
        public DbSet<TipoDepartamentoTabla> TipoDepartamentoTabla { get; set; }
        public DbSet<ClienteTabla> ClienteTabla { get; set; }
        public DbSet<BitacoraTabla> BitacoraTabla { get; set; }
        public DbSet<CitasTabla> CitasTabla { get; set; }
        public DbSet<ColaboradorTabla> ColaboradorTabla { get; set; }
        public DbSet<ImagenesDepartamentoTabla> ImagenesDepartamentoTabla { get; set; }
        public DbSet<HabitacionesTabla> HabitacionesTabla { get; set; }
        public DbSet<TipoHabitacionTabla> TipoHabitacionTabla { get; set; }
        public DbSet<ImagenesHabitacionTabla> ImagenesHabitacionTabla { get; set; }
        public DbSet<ReservasTabla> ReservasTabla { get; set; }



    }
}
