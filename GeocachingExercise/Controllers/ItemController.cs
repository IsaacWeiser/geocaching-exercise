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
            try
            {
                return Ok(_itemRepo.GetActiveItems(todayDate, cacheId));
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult AddItem(Item item)
        {

            DateTime tmpDate = DateTime.ParseExact(item.ActiveStartDate, "MM/dd/yyyy", null);
            item.ActiveStartDate = tmpDate.ToString("dd-MM-yyyy");
            tmpDate = DateTime.ParseExact(item.ActiveEndDate, "MM/dd/yyyy", null);
            item.ActiveEndDate = tmpDate.ToString("dd-MM-yyyy");

            if (item.CacheId == 0)
            {
                item.CacheId = null;
            }

            if (_itemRepo.CacheItemCount(item.CacheId)< 3 )
            {
                _itemRepo.AddItem(item);
                return CreatedAtAction("GetById", new { id = item.Id }, item);
            }

            return BadRequest();
        }

        [HttpPut("{itemId}")]
        public IActionResult MoveItem (int itemId, Item item)
        {
            if (_itemRepo.CacheItemCount(item.CacheId) < 3)
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
