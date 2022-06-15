using GeocachingExercise.Models;
using System;
using System.Collections.Generic;
using Xunit;
using GeocachingExercise.Tests.Mocks;
using GeocachingExercise.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GeocachingExercise.Tests
{
    public class ItemControllerTests
    {
        [Fact]
        public void Get_By_Id_Returns_Item_With_Given_Id()
        {
            //arrange
            var testItemId = 45;
            var items = CreateTestItems(5);
            items[0].Id = testItemId;

            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            //act
            var result = controller.GetById(testItemId);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualItem = Assert.IsType<Item>(okResult.Value);

            Assert.Equal(testItemId, actualItem.Id);
        }

        [Fact]
        public void Get_Active_Items_Returns_Items_In_A_Particular_Cache_That_Are_In_Date()
        {
            //arrange
            var number = new Random().Next();
            var items = CreateTestItemsOfParticularCache(number);
            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            //act
            var results = controller.getActiveItems(number);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(results);
            var actualList = Assert.IsType<List<Item>>(okResult.Value);

            Assert.All(actualList, i => Assert.True(i.Id <3));

        }

        private List<Item> CreateTestItems(int count)
        {
            var items = new List<Item>();
            for (var i = 1; i <= count; i++)
            {
                items.Add(new Item()
                {
                    Id = i,
                    Name = $"Name {i}",
                    ActiveStartDate = DateTime.Today.AddDays(-new Random().NextDouble()).ToString(),
                    ActiveEndDate = DateTime.Today.ToString(),
                    CacheId = i,
                    
                });
            }
            return items;
        }

        private List<Item> CreateTestItemsOfParticularCache(int id)
        {
            var items = new List<Item>();

            items.Add(new Item()
            {
                Id = 1,
                Name = $"Name",
                ActiveStartDate = DateTime.Today.AddDays(-5).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(2).ToString(),
                CacheId = id,

            });

            items.Add(new Item()
            {
                Id = 2,
                Name = $"Name",
                ActiveStartDate = DateTime.Today.AddDays(-100).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(1).ToString(),
                CacheId = id,

            });

            items.Add(new Item()
            {
                Id = 3,
                Name = $"Name",
                ActiveStartDate = DateTime.Today.AddDays(1).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(8).ToString(),
                CacheId = id,

            });

            return items;
        }
    }
}
