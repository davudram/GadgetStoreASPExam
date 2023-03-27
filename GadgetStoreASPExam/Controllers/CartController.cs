using GadgetStoreASPExam.Cache;
using GadgetStoreASPExam.Data;
using GadgetStoreASPExam.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace GadgetStoreASPExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class CartController : ControllerBase
    {
        private readonly DbContextClass _context;
        private readonly ICacheService _cacheService;

        public CartController(DbContextClass context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("GetCart")]
        public async Task<ActionResult<IEnumerable<CartItem>>> Get()
        {
            List<CartItem> cartItemsCache = _context.CartItems.ToList();
            if (cartItemsCache == null)
            {
                var cartSQL = await _context.CartItems.ToListAsync();
                if (cartSQL.Count > 0)
                {
                    _cacheService.SetData("CartItem", cartSQL, DateTimeOffset.Now.AddDays(1));
                }
            }
            return Ok(cartItemsCache);
        }

        [HttpPost]
        [Route("AddCart")]
        public IActionResult AddToCart([FromBody] CartItem cartItem)
        {
            var userId = User.Identity.Name;
            var existingItem = _context.CartItems.FirstOrDefault(x => x.UserId == userId && x.ProductId == cartItem.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += cartItem.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                cartItem.UserId = userId;
                cartItem.CreatedAt = DateTime.UtcNow;
                cartItem.UpdatedAt = DateTime.UtcNow;
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            _cacheService.SetData("CartItem", _context.CartItems, DateTimeOffset.Now.AddDays(1));

            var cartItems = _context.CartItems
                .Where(x => x.UserId == userId)
                .ToList();

            return Ok(cartItems);
        }

        [HttpPost]
        [Route("UpdateCart")]
        public IActionResult UpdateCartItem([FromBody] CartItem cartItem)
        {
            var existingItem = _context.CartItems
                .FirstOrDefault(x => x.ProductId == cartItem.ProductId);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.ProductName = cartItem.ProductName;
            existingItem.Price = cartItem.Price;
            existingItem.Quantity = cartItem.Quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
            _cacheService.SetData("CartItem", _context.CartItems, DateTimeOffset.Now.AddDays(1));

            var cartItems = _context.CartItems.ToList();

            return Ok(cartItems);
        }

        [HttpPost]
        [Route("DeleteCart")]
        public IActionResult DeleteCartItem([FromQuery] int Id)
        {
            var existingItem = _context.CartItems
                .FirstOrDefault(x => x.ProductId == Id);

            if (existingItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(existingItem);
            _context.SaveChanges();
            _cacheService.SetData("CartItem", _context.CartItems, DateTimeOffset.Now.AddDays(1));

            var cartItems = _context.CartItems.ToList();

            return Ok(cartItems);
        }
    }
}
