using Evening.Domain.Entities;
using Evening.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace EveningWeb.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext _db;
        public VillaNumberController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            var villasNumbers = _db.VillasNumbers.Include(u=>u.Villa).ToList();
            return View(villasNumbers);
        }

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> list = _db.Villas.ToList().Select(u => new SelectListItem
            {
               Text = u.Name,
               Value= u.Id.ToString(),
            });
            ViewData["VillaList"] = list;
            return View();
        }

        [HttpPost]
        public IActionResult Create(VillaNumber obj)
        {
            //ModelState.Remove("Villa");
            bool roomNumberExists = _db.VillasNumbers.Any(u => u.Villa_Number == obj.Villa_Number);
           
            if (ModelState.IsValid && !roomNumberExists)
            {
                _db.VillasNumbers.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "The villa has been created successfully.";
                return RedirectToAction("Index");
            }
            if (roomNumberExists)
            {
                TempData["error"] = " The villa number already exists.";
            }
            IEnumerable<SelectListItem> list = _db.Villas.ToList().Select(u => new SelectListItem
            { 

               Text = u.Name,
                Value = u.Id.ToString(),
            });

            return View(obj);
        }
        public IActionResult Update(int VillaNumberId)
        {
            var villaNumber = _db.VillasNumbers.FirstOrDefault(u => u.Villa_Number == VillaNumberId);
            if (villaNumber == null)
            {
                TempData["error"] = "The villa number could not be found.";
                return RedirectToAction("Index");
            }

            // Load Villa dropdown for the view
            ViewBag.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            return View(villaNumber);
        }


        [HttpPost]
        public IActionResult Update(VillaNumber obj)
        {
            if (ModelState.IsValid)
            {
                var villaNumberFromDb = _db.VillasNumbers.FirstOrDefault(u => u.Villa_Number == obj.Villa_Number);
                if (villaNumberFromDb != null)
                {
                    // Update the properties
                    villaNumberFromDb.VillaId = obj.VillaId;
                    villaNumberFromDb.SpecialDetails = obj.SpecialDetails;

                    _db.SaveChanges();
                    TempData["success"] = "The villa number has been updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            // Repopulate dropdown list in case of failure
            ViewBag.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            TempData["error"] = "Unable to update the villa number.";
            return View(obj);
        }


        public IActionResult Delete(int VillaNumberId)
        {
            var villaNumber = _db.VillasNumbers.Include(u => u.Villa)
                .FirstOrDefault(u => u.Villa_Number == VillaNumberId);

            if (villaNumber == null)
            {
                TempData["error"] = "The villa number could not be found.";
                return RedirectToAction("Index");
            }

            return View(villaNumber);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int VillaNumberId)
        {
            var villaNumberFromDb = _db.VillasNumbers.FirstOrDefault(u => u.Villa_Number == VillaNumberId);

            if (villaNumberFromDb != null)
            {
                _db.VillasNumbers.Remove(villaNumberFromDb);
                _db.SaveChanges();
                TempData["success"] = "The villa number has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "Unable to delete the villa number. It may not exist.";
            }

            return RedirectToAction("Index");
        }

    }
}