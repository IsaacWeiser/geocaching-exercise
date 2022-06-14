using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeocachingExercise.Models;
using GeocachingExercise.Repositories;

namespace GeocachingExercise.Tests.Mocks
{
    internal class InMemoryItemRepository : IItemRepository
    {
        private readonly List<Item> _data;

        public List<Item> InternalData
        {
            get
            {
                return _data;
            }
        }

        public InMemoryItemRepository(List<Item> startingData)
        {
            _data = startingData;
        }

        public List<Item> GetActiveItems(DateTime today, int cacheId)
        {
            return _data.Where(i => (DateTime.Parse(i.ActiveStartDate) <= today) && (DateTime.Parse(i.ActiveEndDate) >= today)).ToList();
        }

        public Item getItemById( int id)
        {
            return _data.FirstOrDefault(i => i.Id == id);
        }

        public void AddItem(Item item)
        {
            var lastItem = _data.Last();
            item.Id = lastItem.Id + 1;
            _data.Add(item);
        }

        public void MoveItem(int itemId, int? newCacheId )
        {
            Item item = _data.FirstOrDefault(i => i.Id == itemId);
            item.CacheId = newCacheId;
        }

        public int CacheItemCount(int? id)
        {
            return _data.Count(i => i.CacheId == id);
        }
    }
}
