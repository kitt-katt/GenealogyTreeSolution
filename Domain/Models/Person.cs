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

        public Guid? SpouseId { get; set; }
        public Person Spouse { get; set; }

        // Многие-ко-многим для Родителей/Детей
        public List<PersonChild> Children { get; set; } = new();
        public List<PersonChild> Parents { get; set; } = new();
    }

    // Связующая сущность для многие-ко-многим (Parent<->Child)
    // ParentChild: Parent -> Child
    public class PersonChild
    {
        public Guid ParentId { get; set; }
        public Person Parent { get; set; }

        public Guid ChildId { get; set; }
        public Person Child { get; set; }
    }
}
