using GadgetStoreASPExam.Data;
using GadgetStoreASPExam.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GadgetStoreASPExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class CartController : ControllerBase
    {
        private readonly DbContextClass _context;

        public CartController(DbContextClass context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetCart")]
        public IActionResult GetCart()
        {
            var cartItems = _context.CartItems.ToList();
            return Ok(cartItems);
        }

        [HttpPost]
        [Route("AddCart")]
        public IActionResult AddToCart([FromBody] CartItem cartItem)
        {
            var userId = User.Identity.Name;
            var existingItem = _context.CartItems
                .FirstOrDefault(x => x.UserId == userId && x.ProductId == cartItem.ProductId);

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

            var cartItems = _context.CartItems
                .Where(x => x.UserId == userId)
                .ToList();

            return Ok(cartItems);
        }

        [HttpPost]
        [Route("UpdateCart")]
        public IActionResult UpdateCartItem([FromBody] CartItem cartItem)
        {
            var userId = User.Identity.Name;
            var existingItem = _context.CartItems
                .FirstOrDefault(x => x.UserId == userId && x.Id == cartItem.Id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.ProductId = cartItem.ProductId;
            existingItem.ProductName = cartItem.ProductName;
            existingItem.Price = cartItem.Price;
            existingItem.Quantity = cartItem.Quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            var cartItems = _context.CartItems
                .Where(x => x.UserId == userId)
                .ToList();

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

            var cartItems = _context.CartItems.ToList();

            return Ok(cartItems);
        }
    }
}
