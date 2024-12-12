using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces;
using Domain.Models;
using DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class EfRepository : IRepository
    {
        private readonly PersonContext _context;

        public EfRepository(PersonContext context)
        {
            _context = context;
            _context.Database.EnsureCreated(); // Или использовать миграции
        }

        public Person GetPersonById(Guid id)
        {
            return _context.Persons
                .Include(p => p.Parents).ThenInclude(pc => pc.Parent)
                .Include(p => p.Children).ThenInclude(pc => pc.Child)
                .Include(p => p.Spouse)
                .FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Person> GetAllPersons()
        {
            return _context.Persons
                .Include(p => p.Parents).ThenInclude(pc => pc.Parent)
                .Include(p => p.Children).ThenInclude(pc => pc.Child)
                .Include(p => p.Spouse)
                .ToList();
        }

        public void AddPerson(Person person)
        {
            if (person.Id == Guid.Empty)
                person.Id = Guid.NewGuid();
            _context.Persons.Add(person);
            SaveChanges();
        }

        public void UpdatePerson(Person person)
        {
            _context.Persons.Update(person);
            SaveChanges();
        }

        public void DeletePerson(Guid id)
        {
            var person = _context.Persons
                .Include(p => p.Parents)
                .Include(p => p.Children)
                .FirstOrDefault(p => p.Id == id);
            if (person != null)
            {
                // EF сам удалит связи (cascade), но нужно убедиться, что потомки лишатся родителя.
                // При удалении Person будут удалены связанные PersonChild записи.
                _context.Persons.Remove(person);
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
