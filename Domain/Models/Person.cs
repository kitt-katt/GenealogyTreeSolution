using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public class Person
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }

        public List<Guid> Parents { get; set; } = new();
        public List<Guid> Children { get; set; } = new();
        public Guid? Spouse { get; set; }
    }
}
