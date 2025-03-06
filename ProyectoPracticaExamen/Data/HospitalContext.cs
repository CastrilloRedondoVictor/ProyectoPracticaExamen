using Microsoft.EntityFrameworkCore;
using ProyectoPracticaExamen.Models;

namespace ProyectoPracticaExamen.Data
{
    public class HospitalContext: DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> options) : base(options)
        {
        }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
    }
}
