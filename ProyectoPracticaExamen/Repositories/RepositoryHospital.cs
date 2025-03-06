using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ProyectoPracticaExamen.Data;
using ProyectoPracticaExamen.Models;


//(@posicion int, @departamento int, @registros int out)
//as
//	select @registros = count(*) from EMP where DEPT_NO = @departamento

//	select EMP_NO, APELLIDO, OFICIO, DIR, FECHA_ALT, SALARIO, COMISION, DEPT_NO from 
//	(
//		select ROW_NUMBER() OVER (ORDER BY APELLIDO, EMP_NO) as POSICION, EMP_NO, APELLIDO, OFICIO, DIR, FECHA_ALT, SALARIO, COMISION, DEPT_NO
//		from EMP
//		where DEPT_NO = @departamento
//	) QUERY
//	where POSICION = @posicion
//go

namespace ProyectoPracticaExamen.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            return await this.context.Departamentos.ToListAsync();
        }

        public async Task<Departamento> BuscarDepartamentoAsync(int id)
        {
            return await this.context.Departamentos.FindAsync(id);
        }

        public async Task<(int registros, Empleado empleados)> GetPaginationEmpleadosDepartamentoAsync(int posicion, int departamento)
        {
            // Definir el parámetro de salida
            SqlParameter registrosParam = new SqlParameter("@registros", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            // Ejecutar el procedimiento almacenado sin capturar el resultado de empleados
            await this.context.Database.ExecuteSqlRawAsync(
                "EXEC SP_EMPLEADOS_DEPARTAMENTO_PAGINATION @posicion, @departamento, @registros OUT",
                new SqlParameter("@posicion", posicion),
                new SqlParameter("@departamento", departamento),
                registrosParam
            );

            // Obtener el número total de registros después de ejecutar el procedimiento
            int registros = (registrosParam.Value != DBNull.Value) ? (int)registrosParam.Value : 0;

            // Ejecutar otra consulta para obtener los empleados
            var empleados = await this.context.Empleados
                .FromSqlRaw("EXEC SP_EMPLEADOS_DEPARTAMENTO_PAGINATION @posicion, @departamento, @registros OUT",
                    new SqlParameter("@posicion", posicion),
                    new SqlParameter("@departamento", departamento),
                    registrosParam)
                .AsNoTracking()
                .ToListAsync();

            return (registros, empleados.FirstOrDefault());
        }



    }
}
