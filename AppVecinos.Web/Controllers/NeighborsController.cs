using Microsoft.AspNetCore.Mvc;
using AppVecinos.Web.Services;
using Microsoft.Extensions.Logging;
using System;
using AppVecinos.Web.Models;
using System.Threading.Tasks;

namespace AppVecinos.Web.Controllers
{
    public class NeighborsController : Controller
    {
        private readonly NeighborService _neighborService;
        private readonly ILogger<NeighborsController> _logger;

        public NeighborsController(NeighborService neighborService, ILogger<NeighborsController> logger)
        {
            _neighborService = neighborService;
            _logger = logger;
        }
        // GET: NeighborsController
        public async Task<ActionResult> Index()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            Console.WriteLine($"Token session: {token}");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            var neighbors = await _neighborService.GetNeighborsAsync(token);
            return View(neighbors);
        }

        // GET: NeighborsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NeighborsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NeighborsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Neighbor neighbor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var token = HttpContext.Session.GetString("AuthToken");
                    var newNeighbor = await _neighborService.AddNeighborAsync(neighbor, token);
                    if (newNeighbor != null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        return View(neighbor);
                    }
                }

                return View(neighbor);
            }
            catch
            {
                return View(neighbor);
            }
        }

        // GET: NeighborsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NeighborsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                 _logger.LogError(ex, "An error occurred while editing the neighbor.");
                 return View();
            }
        }

        // GET: NeighborsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NeighborsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a neighbor.");
                return View();
            }
        }
    }
}
