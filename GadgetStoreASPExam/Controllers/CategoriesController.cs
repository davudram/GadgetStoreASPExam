using GadgetStoreASPExam.Cache;
using GadgetStoreASPExam.Data;
using GadgetStoreASPExam.Model;
using GadgetStoreASPExam.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GadgetStoreASPExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly DbContextClass _context;
        private readonly ICacheService _cacheService;

        public CategoriesController(DbContextClass context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("CategoryList")]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            List<Category> categoryCache = _cacheService.GetData<List<Category>>("Category");
            if (categoryCache == null)
            {
                var categorySQL = await _context.Categories.ToListAsync();
                if (categorySQL.Count > 0)
                {
                    _cacheService.SetData("Category", categorySQL, DateTimeOffset.Now.AddDays(1));
                }
            }
            return categoryCache;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [Authorize(Roles = UserRoles.Manager)]
        [Route("CreateCategory")]
        public ActionResult Add(Category category)
        {
            var item = _context.Categories.FirstOrDefault(x => x.NameGadgets.Equals(category.NameGadgets));

            if (item == null)
            {
                _context.Add(new Category { NameGadgets = category.NameGadgets });
                _context.SaveChanges();
                _cacheService.SetData("Category", _context.Categories, DateTimeOffset.Now.AddDays(1));
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [Authorize(Roles = UserRoles.Manager)]
        [Route("DeleteCategory")]

        public ActionResult Delete(Category category)
        {
            var item = _context.Categories.FirstOrDefault(x => x.Id.Equals(category.Id));

            if (item != null)
            {
                _context.Remove(item);
                _context.SaveChanges();
                _cacheService.SetData("Category", _context.Categories, DateTimeOffset.Now.AddDays(1));
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [Authorize(Roles = UserRoles.Manager)]
        [Route("EditGadget")]

        public ActionResult Edit(Category category)
        {
            var item = _context.Categories.FirstOrDefault(x => x.Id.Equals(category.Id));

            if (item != null)
            {
                _context.Remove(item);
                _context.SaveChanges();
                _context.Add(new Category { NameGadgets = category.NameGadgets });
                _context.SaveChanges();
                _cacheService.SetData("Category", _context.Categories, DateTimeOffset.Now.AddDays(1));
                return Ok();
            }
            return NotFound();
        }
    }
}
