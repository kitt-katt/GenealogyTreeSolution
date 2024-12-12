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

        public void DeletePerson(Guid personId)
        {
            _repository.DeletePerson(personId);
        }

        public void AddParentToPerson(Guid personId, Person parent)
        {
            var child = _repository.GetPersonById(personId);
            if (child == null) return;
            _repository.AddPerson(parent);

            parent.Children.Add(new PersonChild { ParentId = parent.Id, ChildId = child.Id });
            _repository.UpdatePerson(parent);
            _repository.UpdatePerson(child);
        }

        public void AddChildToPerson(Guid personId, Person child)
        {
            var parent = _repository.GetPersonById(personId);
            if (parent == null) return;

            _repository.AddPerson(child);
            parent.Children.Add(new PersonChild { ParentId = parent.Id, ChildId = child.Id });
            _repository.UpdatePerson(parent);
            _repository.UpdatePerson(child);
        }

        public void SetSpouseRelationship(Guid personId, Guid spouseId)
        {
            var p1 = _repository.GetPersonById(personId);
            var p2 = _repository.GetPersonById(spouseId);

            if (p1 == null || p2 == null) return;

            p1.SpouseId = p2.Id;
            p2.SpouseId = p1.Id;

            _repository.UpdatePerson(p1);
            _repository.UpdatePerson(p2);
        }

        public IEnumerable<Person> GetAllDescendants(Guid personId)
        {
            var descendants = new List<Person>();
            var visited = new HashSet<Guid>();

            void DFS(Guid pid)
            {
                var p = _repository.GetPersonById(pid);
                if (p == null) return;

                foreach (var ch in p.Children)
                {
                    if (!visited.Contains(ch.ChildId))
                    {
                        visited.Add(ch.ChildId);
                        var child = _repository.GetPersonById(ch.ChildId);
                        if (child != null)
                        {
                            descendants.Add(child);
                            DFS(child.Id);
                        }
                    }
                }
            }

            DFS(personId);
            return descendants;
        }

        public IEnumerable<Person> GetAllAncestors(Guid personId)
        {
            var ancestors = new List<Person>();
            var visited = new HashSet<Guid>();

            void DFS(Guid cid)
            {
                var c = _repository.GetPersonById(cid);
                if (c == null) return;

                foreach (var pr in c.Parents)
                {
                    if (!visited.Contains(pr.ParentId))
                    {
                        visited.Add(pr.ParentId);
                        var parent = _repository.GetPersonById(pr.ParentId);
                        if (parent != null)
                        {
                            ancestors.Add(parent);
                            DFS(parent.Id);
                        }
                    }
                }
            }

            DFS(personId);
            return ancestors;
        }

        public IEnumerable<Person> GetAllPersons()
        {
            return _repository.GetAllPersons();
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

        public void DisplayTree()
        {
            var all = _repository.GetAllPersons().ToList();
            var roots = all.Where(p => !p.Parents.Any()).ToList();

            if (!roots.Any())
            {
                Console.WriteLine("Дерево пусто или нет корневых предков.");
                return;
            }

            foreach (var root in roots)
            {
                PrintPerson(root, 0, lastChild:true);
            }
        }

        private void PrintPerson(Person p, int indent, bool lastChild)
        {
            var prefix = new string(' ', indent * 4);
            var connector = lastChild ? "└──" : "├──";
            // Добавляем дату рождения при выводе
            Console.WriteLine(prefix + connector + $"{p.FullName} ({p.BirthDate:yyyy-MM-dd}) [{p.Id}]");
            var children = p.Children.Select(pc => pc.Child).ToList();
            for (int i = 0; i < children.Count; i++)
            {
                PrintPerson(children[i], indent + 1, i == children.Count - 1);
            }
        }

        public IEnumerable<Person> GetParents(Guid personId)
        {
            var person = _repository.GetPersonById(personId);
            if (person == null) return Enumerable.Empty<Person>();

            return person.Parents.Select(p => p.Parent);
        }

        public IEnumerable<Person> GetChildren(Guid personId)
        {
            var person = _repository.GetPersonById(personId);
            if (person == null) return Enumerable.Empty<Person>();

            return person.Children.Select(c => c.Child);
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
            var ancestors1 = GetAllAncestors(personId1).ToHashSet(new PersonIdComparer());
            var ancestors2 = GetAllAncestors(personId2).ToHashSet(new PersonIdComparer());

            return ancestors1.Intersect(ancestors2, new PersonIdComparer());
        }

        private class PersonIdComparer : IEqualityComparer<Person>
        {
            public bool Equals(Person x, Person y) => x.Id == y.Id;
            public int GetHashCode(Person obj) => obj.Id.GetHashCode();
        }
    }
}
