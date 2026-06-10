using Microsoft.AspNetCore.Mvc;
using MVCConciertos.Services;

namespace MVCConciertos.Controllers
{
    public class ConciertosController : Controller
    {
        private readonly ServiceConciertos _service;

        public ConciertosController(ServiceConciertos service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var eventos = await _service.GetEventosAsync();
            return View(eventos);
        }

        public async Task<IActionResult> Categorias()
        {
            var categorias = await _service.GetCategoriasAsync();
            return View(categorias);
        }

        public async Task<IActionResult> EventosCategoria(int idCategoria)
        {
            var eventos = await _service.GetEventosByCategoriaAsync(idCategoria);
            ViewData["ID_CATEGORIA"] = idCategoria;
            return View("Index", eventos); // Podemos reusar la vista Index si usa el mismo modelo
        }
    }
}
