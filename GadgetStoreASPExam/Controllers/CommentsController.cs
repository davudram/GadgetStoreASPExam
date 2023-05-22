using GadgetStoreASPExam.Cache;
using GadgetStoreASPExam.Data;
using GadgetStoreASPExam.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GadgetStoreASPExam.Controllers
{
    [ApiController, Authorize]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly DbContextClass _context;
        private readonly ICacheService _cacheService;

        public CommentsController(DbContextClass context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("GetComment")]
        public async Task<ActionResult<IEnumerable<Comments>>> Get()
        {
            List<Comments> commentsCache = _context.Comments.ToList();
            if(commentsCache == null)
            {
                var commentsSQL = await _context.Comments.ToListAsync();
                if(commentsSQL.Count > 0)
                {
                    _cacheService.SetData("Comments", commentsSQL, DateTimeOffset.Now.AddDays(1));
                }
            }
            return Ok(commentsCache);
        }

        [HttpPost]
        [Route("CreateComment")]
        public IActionResult AddComment([FromBody] Comments comment)
        {
            var userName = User.Identity.Name;
            var items = _context.Comments.FirstOrDefault(x => x.UserName.Equals(userName) && x.Title.Equals(comment.Title));

            if(items == null)
            {
                comment.UserName = userName;
                comment.CreateAt = DateTime.UtcNow;
                _context.Comments.Add(comment);
                _context.SaveChanges();
                _cacheService.SetData("Comments", _context.Comments, DateTime.Now.AddDays(1));
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("UpdateComment")]
        public IActionResult EditComment([FromBody] Comments comment)
        {
            var userName = User.Identity.Name;
            var item = _context.Comments.FirstOrDefault(x => x.UserName.Equals(userName) && x.Id.Equals(comment.Id));

            if(item != null)
            {
                item.Stars = comment.Stars;
                item.Title = comment.Title;
                item.Text = comment.Text;
                item.CreateAt = DateTime.UtcNow;
                _context.SaveChanges();
                _cacheService.SetData("Comments", _context.Comments, DateTime.Now.AddDays(1));
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("DeleteComment")]
        public IActionResult RemoveComment([FromBody] Comments comment)
        {
            var userName = User.Identity.Name;
            var item = _context.Comments.FirstOrDefault(x => x.UserName.Equals(userName) && x.Id.Equals(comment.Id));

            if(item != null)
            {
                _context.Comments.Remove(item);
                _context.SaveChanges();
                _cacheService.SetData("Comments", _context.Comments, DateTime.Now.AddDays(1));
                return Ok();
            }
            return BadRequest();
        }
    }
}
