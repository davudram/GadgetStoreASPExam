using GadgetStoreASPExam.Cache;
using GadgetStoreASPExam.Data;
using GadgetStoreASPExam.Model;
using GadgetStoreASPExam.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GadgetStoreASPExam.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuyController : ControllerBase
    {
        private readonly DbContextClass _context;
        private readonly ICacheService _cacheService;

        public BuyController(DbContextClass context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("BuyGadgetList")]
        public async Task<ActionResult<IEnumerable<BuyGadget>>> Get()
        {
            List<BuyGadget> buyGadgetsCache = _context.BuyGadgets.ToList();
            if (buyGadgetsCache == null)
            {
                var buyGadgetsSQL = await _context.BuyGadgets.ToListAsync();
                if (buyGadgetsSQL.Count > 0)
                {
                    _cacheService.SetData("BuyGadget", buyGadgetsSQL, DateTimeOffset.Now.AddDays(1));
                }
            }
            return buyGadgetsCache;
        }

        [HttpPost]
        [Route("AddBuyGadget")]
        public IActionResult AddToBuyGadget([FromBody] BuyGadget buyGadget)
        {
            var userId = User.Identity.Name;
            var existingBuy = _context.BuyGadgets.FirstOrDefault(x => x.User == userId && x.ProductId == buyGadget.ProductId);

            if (existingBuy != null)
            {
                existingBuy.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                buyGadget.User = userId;
                buyGadget.CreatedAt = DateTime.UtcNow;
                buyGadget.UpdatedAt = DateTime.UtcNow;
                _context.BuyGadgets.Add(buyGadget);
            }

            _context.SaveChanges();
            _cacheService.SetData("BuyGadget", _context.BuyGadgets, DateTimeOffset.Now.AddDays(1));

            var buyGadgets = _context.BuyGadgets
                .Where(x => x.User == userId)
                .ToList();

            return Ok(buyGadgets);
        }

        [HttpPost]
        [Route("UpdateBuyGadget")]
        public IActionResult UpdateBuyGadget([FromBody] BuyGadget buyGadget)
        {
            var existingBuy = _context.BuyGadgets
                .FirstOrDefault(x => x.ProductId == buyGadget.ProductId);

            if (existingBuy == null)
            {
                return NotFound();
            }

            existingBuy.ProductId = buyGadget.ProductId;
            existingBuy.NameCard = buyGadget.NameCard;
            existingBuy.Month = buyGadget.Month;
            existingBuy.Years = buyGadget.Years;
            existingBuy.CVV = buyGadget.CVV;
            existingBuy.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
            _cacheService.SetData("BuyGadget", _context.BuyGadgets, DateTimeOffset.Now.AddDays(1));

            var buyGadgets = _context.BuyGadgets.ToList();

            return Ok(buyGadgets);
        }

        [HttpPost]
        [Route("DeleteBuyGadget")]
        public IActionResult DeleteBuyGadget([FromQuery] int Id)
        {
            var existingBuy = _context.BuyGadgets
                .FirstOrDefault(x => x.ProductId == Id);

            if (existingBuy == null)
            {
                return NotFound();
            }

            _context.BuyGadgets.Remove(existingBuy);
            _context.SaveChanges();
            _cacheService.SetData("BuyGadgets", _context.BuyGadgets, DateTimeOffset.Now.AddDays(1));

            var buyGadgets = _context.BuyGadgets.ToList();

            return Ok(buyGadgets);
        }
    }
}
