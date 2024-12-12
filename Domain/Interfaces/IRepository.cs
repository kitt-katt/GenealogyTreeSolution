using System;
using System.Collections.Generic;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IRepository
    {
        Person GetPersonById(Guid id);
        IEnumerable<Person> GetAllPersons();
        void AddPerson(Person person);
        void UpdatePerson(Person person);
        void DeletePerson(Guid id);
        void SaveChanges();
    }
}
