using GeocachingExercise.Models;
using System;
using System.Collections.Generic;

namespace GeocachingExercise.Repositories
{
    public interface IItemRepository
    {
        void AddItem(Item item);
        int CacheItemCount(int? cacheId);
        List<Item> GetActiveItems(DateTime currentDate, int cacheId);
        void MoveItem(int itemId, int? newCacheId);
        Item getItemById(int id);
    }
}