using GadgetStoreASPExam.Cache;
using GadgetStoreASPExam.Data;
using GadgetStoreASPExam.Model;
using GadgetStoreASPExam.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http.Headers;

namespace GadgetStoreASPExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class GadgetsController : ControllerBase
    {
        private readonly DbContextClass _context;
        private readonly ICacheService _cacheService;
        public static IWebHostEnvironment _environment;


        public GadgetsController(DbContextClass context, ICacheService cacheService, IWebHostEnvironment environment)
        {
            _context = context;
            _cacheService = cacheService;
            _environment = environment;
        }

        [HttpGet]
        [Route("GadgetsList")]
        public async Task<ActionResult<IEnumerable<Gadget>>> Get()
        {
            List<Gadget> productsCache = _context.Gadgets.ToList();
            if (productsCache == null)
            {
                var gadgetsSQL = await _context.Gadgets.ToListAsync();
                if (gadgetsSQL.Count > 0)
                {
                    _cacheService.SetData("Gadget", gadgetsSQL, DateTimeOffset.Now.AddDays(1));
                }
            }
            return productsCache;
        }

        [HttpGet]
        [Route("MinMediumMaxGadgetList")]
        public async Task<ActionResult<IEnumerable<Gadget>>> Get(int? categoryId, string sort)
        {
            List<Gadget> productsCache = _context.Gadgets.ToList();

            if (productsCache == null)
            {
                productsCache = await _context.Gadgets.Include(g => g.IdCategoryNavigation).ToListAsync();
                if (productsCache.Count > 0)
                {
                    _cacheService.SetData("Gadget", productsCache, DateTimeOffset.Now.AddDays(1));
                }
            }

            var filteredProducts = categoryId.HasValue
                ? productsCache.Where(p => p.IdCategory == categoryId.Value)
                : productsCache;

            var sortedProducts = sort == "min"
                ? filteredProducts.OrderBy(p => p.Price)
                : sort == "max"
                    ? filteredProducts.OrderByDescending(p => p.Price)
                    : filteredProducts.OrderBy(p => p.IdCategoryNavigation?.Id).ThenBy(p => p.Price);

            return sortedProducts.ToList();
        }





        [HttpGet]
        [Route("SearchGadgetsList")]
        public async Task<ActionResult<IEnumerable<Gadget>>> Get([FromQuery] string search)
        {
            List<Gadget> productsCache = _context.Gadgets.ToList();
            if (productsCache == null)
            {
                var gadgetsSQL = await _context.Gadgets.ToListAsync();
                if (gadgetsSQL.Count > 0)
                {
                    _cacheService.SetData("Gadget", gadgetsSQL, DateTimeOffset.Now.AddDays(1));
                    productsCache = gadgetsSQL;
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                productsCache = productsCache.Where(g => g.Name.Contains(search)).ToList();
            }

            return productsCache;
        }

        [HttpGet]
        [Route("SelectedForCategory")]
        public async Task<ActionResult<IEnumerable<Gadget>>> Get(int idcategory)
        {
            List<Gadget> productCache = _context.Gadgets.ToList();
            if (productCache == null)
            {
                var gadgetsSQL = _context.Gadgets.ToList();

                if (gadgetsSQL.Count > 0)
                {
                    _cacheService.SetData("Gadget", gadgetsSQL, DateTimeOffset.Now.AddDays(1));
                }
            }

            if (idcategory > 0)
            {
                productCache = productCache.Where(x => x.IdCategory == idcategory).ToList();
            }

            return productCache;
        }


        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Manager}")]
        [Route("Upload")]
        public async Task<ActionResult<string>> Post([FromForm] FileModel file)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FormFile.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await file.FormFile.CopyToAsync(stream);
                }

                string baseUrl = "C:\\Users\\Toucc\\source\\repos\\GadgetStoreASPExam\\GadgetStoreASPExam\\wwwroot";
                string fileUrl = $"{baseUrl}//{fileName.Replace("\\", "/")}";

                return Ok(fileUrl);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet]
        [Route("FilterPriceByIdCategory")]
        public async Task<ActionResult<IEnumerable<Gadget>>> Get(double? minPrice = null, double? maxPrice = null, int? idcategory = null)
        {
            List<Gadget> productsCache = _context.Gadgets.ToList();
            if (productsCache == null)
            {
                var gadgetsSQL = await _context.Gadgets.ToListAsync();
                if (gadgetsSQL.Count > 0)
                {
                    _cacheService.SetData("Gadget", gadgetsSQL, DateTimeOffset.Now.AddDays(1));
                }
                productsCache = gadgetsSQL;
            }

            if (minPrice.HasValue && idcategory.HasValue)
            {
                productsCache = productsCache.Where(p => p.Price >= minPrice.Value && p.IdCategory == idcategory).ToList();
            }

            if (maxPrice.HasValue && idcategory.HasValue)
            {
                productsCache = productsCache.Where(p => p.Price <= maxPrice.Value && p.IdCategory == idcategory).ToList();
            }

            return productsCache;
        }

        [HttpGet]
        [Route("SearchByIsPremium")]
        public async Task<ActionResult<IEnumerable<Gadget>>> SelectPremium([FromQuery] string checkPremium)
        {
            List<Gadget> productsCache = _context.Gadgets.ToList();
            if (productsCache == null)
            {
                var gadgetsSQL = await _context.Gadgets.ToListAsync();
                if (gadgetsSQL.Count > 0)
                {
                    _cacheService.SetData("Gadget", gadgetsSQL, DateTimeOffset.Now.AddDays(1));
                    productsCache = gadgetsSQL;
                }
            }

            if (!string.IsNullOrEmpty(checkPremium))
            {
                productsCache = productsCache.Where(g => g.IsPremium.Contains(checkPremium)).ToList();
            }

            return productsCache;
        }


        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Manager}")]
        [Route("CreateGadget")]
        public ActionResult Add(Gadget gadget)
        {
            var item = _context.Gadgets.FirstOrDefault(x => x.Name.Equals(gadget.Name));

            if (item == null)
            {
                _context.Add(new Gadget { IdCategory = gadget.IdCategory, IsPremium = gadget.IsPremium, Name = gadget.Name, Price = gadget.Price, Image = gadget.Image });
                _context.SaveChanges();
                _cacheService.SetData("Gadget", _context.Gadgets, DateTimeOffset.Now.AddDays(1));
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Manager}")]
        [Route("DeleteGadget")]

        public ActionResult Delete(Gadget gadget)
        {

            var item = _context.Gadgets.FirstOrDefault(x => x.Id.Equals(gadget.Id));

            if (item != null)
            {
                _context.Remove(item);
                _context.SaveChanges();
                _cacheService.SetData("Gadget", _context.Gadgets, DateTimeOffset.Now.AddDays(1));
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Manager}")]
        [Route("EditGadget")]

        public ActionResult Edit(Gadget gadget)
        {
            var item = _context.Gadgets.FirstOrDefault(x => x.Id.Equals(gadget.Id));

            if (item != null)
            {
                item.IdCategory = gadget.IdCategory;
                item.IsPremium = gadget.IsPremium;
                item.Name = gadget.Name;
                item.Price = gadget.Price;
                item.Image = gadget.Image;
                item.IsPremium = gadget.IsPremium;
                _context.SaveChanges();
                _cacheService.SetData("Gadget", _context.Gadgets, DateTimeOffset.Now.AddDays(1));
                return Ok();
            }
            return NotFound();
        }
    }
}