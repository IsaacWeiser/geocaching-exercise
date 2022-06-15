using System;
using System.ComponentModel.DataAnnotations;

namespace GeocachingExercise.Models
{
    public class Item
    {
        public int Id { get; set; }

        //this specifies that names must be no longer than 50 characters and that it cant have any special characters
        [MaxLength(50, ErrorMessage ="Name too long (beyond 50 characters)")]
        [RegularExpression(@"^[a-zA-Z0-9\d\s]*$", ErrorMessage ="names can only contain letters, numbers, and spaces")]
        public string Name { get; set; }

        public int? CacheId { get; set; }

        //this annotation makes sure that the date is displayed in a more readable format
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public string ActiveStartDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public string ActiveEndDate { get; set; }
    }
}
