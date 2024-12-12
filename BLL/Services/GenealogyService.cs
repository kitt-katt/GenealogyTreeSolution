using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces;
using Domain.Models;

namespace BLL.Services
{
    public class GenealogyService : IFamilyTreeService
    {
        private readonly IRepository _repository;

        public GenealogyService(IRepository repository)
        {
            _repository = repository;
        }

        public Person CreatePerson(string fullName, DateTime birthDate, Gender gender)
        {
            return new Person
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                BirthDate = birthDate,
                Gender = gender
            };
        }

        public void AddPersonToTree(Person person)
        {
            _repository.AddPerson(person);
        }

        public void SetParentChildRelationship(Guid parentId, Guid childId)
        {
            var parent = _repository.GetPersonById(parentId);
            var child = _repository.GetPersonById(childId);

            if (parent == null || child == null) return;

            if (!parent.Children.Contains(child.Id))
                parent.Children.Add(child.Id);
            if (!child.Parents.Contains(parent.Id))
                child.Parents.Add(parent.Id);

            _repository.UpdatePerson(parent);
            _repository.UpdatePerson(child);
        }

        public void SetSpouseRelationship(Guid personId, Guid spouseId)
        {
            var p1 = _repository.GetPersonById(personId);
            var p2 = _repository.GetPersonById(spouseId);

            if (p1 == null || p2 == null) return;

            p1.Spouse = p2.Id;
            p2.Spouse = p1.Id;

            _repository.UpdatePerson(p1);
            _repository.UpdatePerson(p2);
        }

        public IEnumerable<Person> GetParents(Guid personId)
        {
            var person = _repository.GetPersonById(personId);
            if (person == null) return Enumerable.Empty<Person>();

            return person.Parents.Select(pid => _repository.GetPersonById(pid)).Where(p => p != null);
        }

        public IEnumerable<Person> GetChildren(Guid personId)
        {
            var person = _repository.GetPersonById(personId);
            if (person == null) return Enumerable.Empty<Person>();

            return person.Children.Select(cid => _repository.GetPersonById(cid)).Where(c => c != null);
        }

        public void DisplayTree()
        {
            var all = _repository.GetAllPersons().ToList();
            if (!all.Any())
            {
                Console.WriteLine("Дерево пусто.");
                return;
            }

            foreach (var p in all)
            {
                Console.WriteLine($"[{p.Id}] {p.FullName}, {p.BirthDate:yyyy-MM-dd}, {p.Gender}");
                var parents = GetParents(p.Id).Select(x => x.FullName);
                var children = GetChildren(p.Id).Select(x => x.FullName);
                var spouse = p.Spouse.HasValue ? _repository.GetPersonById(p.Spouse.Value)?.FullName : "Нет";

                Console.WriteLine($"  Родители: {(parents.Any() ? string.Join(", ", parents) : "Нет")}");
                Console.WriteLine($"  Дети: {(children.Any() ? string.Join(", ", children) : "Нет")}");
                Console.WriteLine($"  Супруг(а): {spouse}");
                Console.WriteLine();
            }
        }

        public void ClearTree()
        {
            var all = _repository.GetAllPersons().ToList();
            foreach (var p in all)
            {
                _repository.DeletePerson(p.Id);
            }
            _repository.SaveChanges();
        }

        public int GetAncestorAgeAtDescendantBirth(Guid ancestorId, Guid descendantId)
        {
            var ancestor = _repository.GetPersonById(ancestorId);
            var descendant = _repository.GetPersonById(descendantId);

            if (ancestor == null || descendant == null) return -1;

            var age = descendant.BirthDate.Year - ancestor.BirthDate.Year;
            if (descendant.BirthDate < ancestor.BirthDate.AddYears(age))
                age--;
            return age;
        }

        public IEnumerable<Person> GetCommonAncestors(Guid personId1, Guid personId2)
        {
            var ancestors1 = GetAllAncestors(personId1);
            var ancestors2 = GetAllAncestors(personId2);

            return ancestors1.Intersect(ancestors2, new PersonIdComparer());
        }

        private HashSet<Person> GetAllAncestors(Guid personId)
        {
            var visited = new HashSet<Guid>();
            var result = new HashSet<Person>(new PersonIdComparer());

            void DFS(Guid pid)
            {
                if (visited.Contains(pid)) return;
                visited.Add(pid);

                var p = _repository.GetPersonById(pid);
                if (p == null) return;

                foreach (var parentId in p.Parents)
                {
                    var parent = _repository.GetPersonById(parentId);
                    if (parent != null)
                    {
                        result.Add(parent);
                        DFS(parent.Id);
                    }
                }
            }

            DFS(personId);
            return result;
        }

        private class PersonIdComparer : IEqualityComparer<Person>
        {
            public bool Equals(Person x, Person y) => x.Id == y.Id;
            public int GetHashCode(Person obj) => obj.Id.GetHashCode();
        }
    }
}
