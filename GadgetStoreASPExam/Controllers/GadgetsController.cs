using GadgetStoreASPExam.Cache;
using GadgetStoreASPExam.Data;
using GadgetStoreASPExam.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GadgetStoreASPExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class GadgetsController : ControllerBase
    {
        private readonly DbContextClass _context;
        private readonly ICacheService _cacheService;


        public GadgetsController(DbContextClass context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("GadgetsList")]
        public async Task<ActionResult<IEnumerable<Gadget>>> Get()
        {
            List<Gadget> productsCache = _cacheService.GetData<List<Gadget>>("Gadget");
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("CreateGadget")]
        public ActionResult Add(Gadget gadget)
        {
            var item = _context.Gadgets.FirstOrDefault(x => x.Name.Equals(gadget.Name));

            if (item == null)
            {
                _context.Add(new Gadget { IdCategory = gadget.IdCategory, Name = gadget.Name, Price = gadget.Price, Image = gadget.Image });
                _context.SaveChanges();
                _cacheService.SetData("Gadget", _context.Gadgets, DateTimeOffset.Now.AddDays(1));
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("DeleteGadget")]

        public ActionResult Delete(int id)
        {

            var item = _context.Gadgets.FirstOrDefault(x => x.Id.Equals(id));

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
        [Authorize(Roles = "Admin")]
        [Route("EditGadget")]

        public ActionResult Edit(Gadget gadget)
        {
            var item = _context.Gadgets.FirstOrDefault(x => x.Id.Equals(gadget.Id));

            if (item != null)
            {
                _context.Remove(item);
                _context.SaveChanges();
                _context.Add(new Gadget { IdCategory = gadget.IdCategory, Name = gadget.Name, Price = gadget.Price });
                _context.SaveChanges();
                _cacheService.SetData("Gadget", _context.Gadgets, DateTimeOffset.Now.AddDays(1));
                return Ok();
            }
            return NotFound();
        }
    }
}
