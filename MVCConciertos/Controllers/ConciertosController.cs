using Microsoft.AspNetCore.Mvc;
using MVCConciertos.Services;
using MVCConciertos.Models;

namespace MVCConciertos.Controllers
{
    public class ConciertosController : Controller
    {
        private readonly ServiceConciertos _service;

        public ConciertosController(ServiceConciertos service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int? idCategoria)
        {
            // Cargamos la lista de categorías para el <select>
            var categorias = await _service.GetCategoriasAsync();
            ViewData["Categorias"] = categorias;

            List<Evento>? eventos;

            // Filtramos dependiendo de si el usuario ha seleccionado algo
            if (idCategoria.HasValue && idCategoria.Value > 0)
            {
                eventos = await _service.GetEventosByCategoriaAsync(idCategoria.Value);
                ViewData["CategoriaSeleccionada"] = idCategoria.Value;
            }
            else
            {
                eventos = await _service.GetEventosAsync();
                ViewData["CategoriaSeleccionada"] = 0;
            }

            return View(eventos);
        }
    }
}
