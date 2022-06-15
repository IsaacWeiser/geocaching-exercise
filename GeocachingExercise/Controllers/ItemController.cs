using GeocachingExercise.Models;
using GeocachingExercise.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GeocachingExercise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _itemRepo;

        public ItemController(IItemRepository itemRepo)
        {
            _itemRepo = itemRepo;
        }




        [HttpGet("activeItems/{cacheId}")]
        public IActionResult getActiveItems(int cacheId)
        {
            var todayDate = DateTime.Today;
            return Ok(_itemRepo.GetActiveItems(todayDate, cacheId));
        }

        [HttpPost]
        public IActionResult AddItem(Item item)
        {
            try
            {
                if (_itemRepo.CacheItemCount(item.CacheId) < 3)
                {
                    _itemRepo.AddItem(item);
                    return CreatedAtAction("GetById", new { id = item.Id }, item);
                }

                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{itemId}")]
        public IActionResult MoveItem (int itemId, Item item)
        {
            if (_itemRepo.CacheItemCount(item.CacheId) < 3 && DateTime.Parse(item.ActiveStartDate) <DateTime.Today && DateTime.Parse(item.ActiveEndDate) > DateTime.Today)
            {
                _itemRepo.MoveItem(itemId, item.CacheId);
                return Ok(_itemRepo.getItemById(itemId));
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(_itemRepo.getItemById(id));
            }
            catch 
            {
                return NotFound();
            }
        }


    }
}
