using GeocachingExercise.Models;
using System;
using System.Collections.Generic;
using Xunit;
using GeocachingExercise.Tests.Mocks;
using GeocachingExercise.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.ComponentModel.DataAnnotations;

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

        [Fact]
        public void Post_Adds_New_Item()
        {
            //arrange
            var itemCount = 5;
            var items = CreateTestItems(itemCount);

            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            //act
            var newItem = new Item()
            {
                Name = "test",
                CacheId = 1,
                ActiveStartDate = DateTime.Today.AddDays(-5).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(2).ToString(),
            };

          controller.AddItem(newItem);

            // Assert
            Assert.Equal(itemCount + 1, repo.InternalData.Count);


        }

        [Fact]
        public void Post_Returns_Bad_Request_When_Its_Cache_Has_Three_Items()
        {
            //arrange
            var items = CreateTestItemsOfParticularCache(1);

            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            //act
            var newItem = new Item()
            {
                Name = "test",
                CacheId = 1,
                ActiveStartDate = DateTime.Today.AddDays(-5).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(2).ToString(),
            };

            var result =controller.AddItem(newItem);

            //Assert
            Assert.IsType<BadRequestResult>(result);

        }

        [Fact]
        public void Put_Reassigns_The_Cache_Id_Of_The_Item()
        {
            //arrange
            var testItemId = 27;
            var items = CreateTestItems(5);
            items[0].Id = testItemId;

            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            var itemToUpdate = new Item()
            {
                Id = testItemId,
                Name = "test",
                CacheId = 40,
                ActiveStartDate = DateTime.Today.AddDays(-5).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(2).ToString(),
            };

            //act
            controller.MoveItem(testItemId, itemToUpdate);

            //Assert
            var itemFromDb = repo.InternalData.FirstOrDefault(i => i.Id == testItemId);
            Assert.NotNull(itemFromDb);

            Assert.Equal(40, itemFromDb.CacheId);
        }

        [Fact]
        public void Put_Returns_Bad_Request_When_Reassigned_Cache_Is_Full()
        {
            //arrange
            var testItemId = 27;
            var items = CreateTestItemsOfParticularCache(2);
            items[0].Id = testItemId;

            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            var itemToUpdate = new Item()
            {
                Id = testItemId,
                Name = "puttest",
                CacheId = 2,
                ActiveStartDate = DateTime.Today.AddDays(-5).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(2).ToString(),
            };

            //act
            var result = controller.MoveItem(testItemId, itemToUpdate);

            //assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Put_Returns_Bad_Request_When_When_Item_To_Be_Moved_Is_Out_Of_Date()
        {
            //arrange
            var testItemId = 27;
            var items = CreateTestItems(5);
            items[0].Id = testItemId;

            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            var itemToUpdate = new Item()
            {
                Id = testItemId,
                Name = "test",
                CacheId = 2,
                ActiveStartDate = DateTime.Today.AddDays(22).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(95).ToString(),
            };

            //act
            var result = controller.MoveItem(testItemId, itemToUpdate);

            //assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Post_Item_Names_If_Not_Unique_Will_Return_Bad_Request()
        {
            //arrange
            var itemCount = 5;
            var items = CreateTestItems(itemCount);
            items[0].Name = "test 1";

            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            //act

            var newItem = new Item()
            {
                Name = "test 1",
                CacheId = 4,
                ActiveStartDate = DateTime.Today.AddDays(22).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(95).ToString(),
            };

            var result = controller.AddItem(newItem);
            

            //assert
            Assert.IsType<BadRequestResult>(result);

        }

        [Fact]
        public void Post_Item_Names_If_Over_Fifty_Characters_Will_Return_Bad_Request()
        {
            //arrange
            var itemCount = 5;
            var items = CreateTestItems(itemCount);

            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            //act

            var newItem = new Item()
            {
                Name = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                CacheId = 4,
                ActiveStartDate = DateTime.Today.AddDays(22).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(95).ToString(),
            };

            var result = controller.AddItem(newItem);

            //assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Post_Item_Names_If_Not_Only_Consisting_Of_Numbers_Letters_Or_Spaces_Will_Return_Bad_Request()
        {
            //arrange
            var itemCount = 5;
            var items = CreateTestItems(itemCount);

            var repo = new InMemoryItemRepository(items);
            var controller = new ItemController(repo);

            //act

            var newItem = new Item()
            {
                Name = "$",
                CacheId = 4,
                ActiveStartDate = DateTime.Today.AddDays(22).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(95).ToString(),
            };

            var result = controller.AddItem(newItem);

            //assert
            Assert.IsType<BadRequestResult>(result);

        }

       //was a method I was using to test out a possible solution for the failed tests
       /*
        [Fact]
        public void validationTest()
        {
            var newItem = new Item()
            {
                Name = "$",
                CacheId = 4,
                ActiveStartDate = DateTime.Today.AddDays(22).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(95).ToString(),
            };

            Assert.True(ValidateModel(newItem).Any(i => i.ErrorMessage.Contains("Name too long (beyond 50 characters)")));
        }
       */

        //creates a test list of items
        private List<Item> CreateTestItems(int count)
        {
            var items = new List<Item>();
            for (var i = 1; i <= count; i++)
            {
                items.Add(new Item()
                {
                    Id = i,
                    Name = $"test {i}",
                    ActiveStartDate = DateTime.Today.AddDays(-new Random().NextDouble()).ToString(),
                    ActiveEndDate = DateTime.Today.ToString(),
                    CacheId = i,
                    
                });
            }
            return items;
        }

        //creates a test list of items that all belong to the same cache
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
                Name = $"test",
                ActiveStartDate = DateTime.Today.AddDays(1).ToString(),
                ActiveEndDate = DateTime.Today.AddDays(8).ToString(),
                CacheId = id,

            });

            return items;
        }

        //ignore this. was a solution I was attempting to implement from stack overflow
        /*
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
        */
    }
}
