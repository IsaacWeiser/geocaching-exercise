using GeocachingExercise.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GeocachingExercise.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly string _connectionString;
        public ItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection Connection
        {
            get { return new SqlConnection(_connectionString); }
        }

        public List<Item> GetActiveItems(DateTime currentDate, int cacheId)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // this query gets items that have the specified cache id and are within the active date range
                    cmd.CommandText = @"SELECT Id, Name, CacheId, ActiveStartDate, ActiveEndDate FROM Item
                                         WHERE ((ActiveStartDate <= @today) AND (ActiveEndDate >= @today)) AND (CacheId = @cacheId)";

                    cmd.Parameters.AddWithValue("@today", currentDate);
                    cmd.Parameters.AddWithValue("@cacheId", cacheId);

                    var activeItems = new List<Item>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           

                            activeItems.Add(new Item()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CacheId = reader.GetInt32(reader.GetOrdinal("CacheId")),
                                ActiveStartDate = reader.GetDateTime(reader.GetOrdinal("ActiveStartDate")).ToString("MM-dd-yyyy"),
                                ActiveEndDate = reader.GetDateTime(reader.GetOrdinal("ActiveEndDate")).ToString("MM-dd-yyyy")
                            });
                        }

                        return activeItems;
                    }
                }
            }
        }

        public void AddItem(Item item)
        {
            //this code handles whether the user inputs the date in mm-dd-yyyy or mm/dd/yyyy format
           if (!Regex.IsMatch(item.ActiveStartDate, "^[0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]$"))
            {
                DateTime tmpDate = DateTime.ParseExact(item.ActiveStartDate, "MM/dd/yyyy", null);
                item.ActiveStartDate = tmpDate.ToString("MM-dd-yyyy");
            }

            if (!Regex.IsMatch(item.ActiveEndDate, "^[0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]$"))
            {
                var tmpDate = DateTime.ParseExact(item.ActiveEndDate, "MM/dd/yyyy", null);
                item.ActiveEndDate = tmpDate.ToString("MM-dd-yyyy");
            }
                
            // if the user leaves the cache id as zero it will change the value to zero
            if (item.CacheId == 0)
            {
                item.CacheId = null;
            }

            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    //query to insert new item into db
                    // CacheId checks if its value is null and whether to set it to dbnull.value
                    cmd.CommandText = @" INSERT INTO Item ( Name, CacheId, ActiveStartDate, ActiveEndDate)
                                        OUTPUT INSERTED.ID
                                        VALUES (@name, @cacheId, @activeStartDate, @activeEndDate)";
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@cacheId", item.CacheId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@activeStartDate", item.ActiveStartDate);
                    cmd.Parameters.AddWithValue("@activeEndDate", item.ActiveEndDate);


                    item.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void MoveItem(int itemId, int? newCacheId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    //this query alters the cache id of the specified item
                    cmd.CommandText = @"
                        UPDATE Item
                           SET
                               CacheId = @newCacheId
                         WHERE Id = @itemId";

                    cmd.Parameters.AddWithValue("@itemId", itemId);
                    cmd.Parameters.AddWithValue("@newCacheId", newCacheId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int CacheItemCount(int? cacheId)
        {
            //determines whether its passed an actual cache value was passed to it
            if (cacheId == null )
            {
                return 0;
            }

            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    //query counts the number of items assigned to a particular cache
                    cmd.CommandText = @"SELECT Count(CacheId) AS NumOfItems FROM Item WHERE CacheId = @cacheId";
                    cmd.Parameters.AddWithValue("@cacheId", cacheId);

                    var numberOfItemsInCache = 0;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            numberOfItemsInCache = reader.GetInt32(reader.GetOrdinal("NumOfItems"));
                        }
                    }

                    return numberOfItemsInCache;
                }
            }
        }

        public Item getItemById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // query selects the item based on its id
                    cmd.CommandText = @"select * from Item WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    var item = new Item();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // converts the date to a string so it can be fed into the model
                            var actDate = reader.GetDateTime(reader.GetOrdinal("ActiveStartDate")).ToString("MM-dd-yyyy");
                            var endDate = reader.GetDateTime(reader.GetOrdinal("ActiveEndDate")).ToString("MM-dd-yyyy");

                            int? cacheIdValue = null;
                            if (!reader.IsDBNull(reader.GetOrdinal("CacheId")))
                            {
                                cacheIdValue = reader.GetInt32(reader.GetOrdinal("CacheId"));
                            }
                            item = new Item()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                CacheId = cacheIdValue,
                                ActiveStartDate = actDate ,
                                ActiveEndDate = endDate
                            };
                        }
                    }
                    return item;
                }
            }
        }
    }
}
