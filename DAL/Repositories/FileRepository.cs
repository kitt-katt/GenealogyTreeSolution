using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces;
using Domain.Models;
using DAL.DataAccess;

namespace DAL.Repositories
{
    public class FileRepository : IRepository
    {
        private readonly JsonDataStorage _storage;

        public FileRepository(string filePath)
        {
            _storage = new JsonDataStorage(filePath);
        }

        public Person GetPersonById(Guid id) => _storage.Persons.FirstOrDefault(p => p.Id == id);

        public IEnumerable<Person> GetAllPersons() => _storage.Persons;

        public void AddPerson(Person person)
        {
            if (person.Id == Guid.Empty)
                person.Id = Guid.NewGuid();
            _storage.Persons.Add(person);
            SaveChanges();
        }

        public void UpdatePerson(Person person)
        {
            var index = _storage.Persons.FindIndex(p => p.Id == person.Id);
            if (index >= 0)
            {
                _storage.Persons[index] = person;
                SaveChanges();
            }
        }

        public void DeletePerson(Guid id)
        {
            var person = GetPersonById(id);
            if (person != null)
            {
                _storage.Persons.Remove(person);
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            _storage.Save();
        }
    }
}
