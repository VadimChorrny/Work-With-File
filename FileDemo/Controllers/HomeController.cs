using FileDemo.Data;
using FileDemo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File = FileDemo.Models.File;

namespace FileDemo.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _host;
        FileDbContext _context;
        public HomeController(FileDbContext context)
        {
            _context = context;
        }

        public IActionResult Create() => View();
        [HttpPost]
        public IActionResult Create(File file)
        {
            if (!ModelState.IsValid) return View();
            var files = HttpContext.Request.Form.Files;
            string fileName = SaveCarImage(files[0]);
            file.Image = fileName;
            _context.Files.Add(file);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id <= 0) return NotFound();

            var fileToRemove = _context.Files.Find(id);

            if (fileToRemove == null) return NotFound();

            if (fileToRemove.Image != null)
            {
                string imagePath = _host.WebRootPath + Path.Combine(WebConstants.carImagesPath, fileToRemove.Image);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Files.Remove(fileToRemove);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private string SaveCarImage(IFormFile img)
        {
            string root = _host.WebRootPath;
            string folder = root + WebConstants.carImagesPath;
            string name = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(img.FileName);

            string fullPath = Path.Combine(folder, name + extension);

            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                img.CopyTo(fs);
            }

            return name + extension;
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id <= 0) return NotFound();

            var file = _context.Files.Find(id);

            if (file == null) return NotFound();

            return View(file);
        }

        [HttpPost]
        public IActionResult Edit(File updatedFile)
        {
            if (!ModelState.IsValid) return View();

            var files = HttpContext.Request.Form.Files;
            var oldCar = _context.Files.AsNoTracking().FirstOrDefault(c => c.Id == updatedFile.Id);

            if (files.Any())
            {
                if (oldCar.Image != null)
                {
                    string oldCarImagePath = _host.WebRootPath + Path.Combine(WebConstants.carImagesPath, oldCar.Image);

                    if (System.IO.File.Exists(oldCarImagePath))
                    {
                        System.IO.File.Delete(oldCarImagePath);
                    }
                }

                string fileName = SaveCarImage(files[0]);

                updatedFile.Image = fileName;
            }
            else
            {
                updatedFile.Image = oldCar.Image;
            }

            _context.Files.Update(updatedFile);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {
            return View(_context.Files.ToList());
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
