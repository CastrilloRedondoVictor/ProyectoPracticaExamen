using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoPracticaExamen.Models;
using ProyectoPracticaExamen.Repositories;

namespace ProyectoPracticaExamen.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private RepositoryHospital repo;

        public HomeController(ILogger<HomeController> logger, RepositoryHospital repo)
        {
            _logger = logger;
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Departamento> departamentos = await this.repo.GetDepartamentosAsync();
            return View(departamentos);
        }

        public async Task<IActionResult> Departamento(int? posicion, int departamento)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            // Obtener el total de empleados y el empleado en la posición actual
            (int totalRegistros, Empleado? empleado) = await this.repo.GetPaginationEmpleadosDepartamentoAsync(posicion.Value, departamento);

            // Obtener la información del departamento
            Departamento? dept = await this.repo.BuscarDepartamentoAsync(departamento);

            // Guardamos los valores en ViewData
            ViewData["REGISTROS"] = totalRegistros;
            ViewData["DEPARTAMENTO"] = dept;
            ViewData["POSICION"] = posicion;

            if (empleado == null)
            {
                ViewData["MENSAJE"] = "No hay empleados en esta posición";
                return View();
            }

            return View(empleado);
        }





        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
