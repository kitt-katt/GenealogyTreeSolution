using System;
using System.Collections.Generic;
using Domain.Models;

namespace BLL.Services
{
    public interface IFamilyTreeService
    {
        Person CreatePerson(string fullName, DateTime birthDate, Gender gender);
        void AddPersonToTree(Person person);
        void DeletePerson(Guid personId);
        void AddParentToPerson(Guid personId, Person parent);
        void AddChildToPerson(Guid personId, Person child);
        void SetSpouseRelationship(Guid personId, Guid spouseId);
        IEnumerable<Person> GetAllDescendants(Guid personId);
        IEnumerable<Person> GetAllAncestors(Guid personId);
        void DisplayTree(); 
        void ClearTree();
        IEnumerable<Person> GetAllPersons();

        IEnumerable<Person> GetParents(Guid personId);
        IEnumerable<Person> GetChildren(Guid personId);
        int GetAncestorAgeAtDescendantBirth(Guid ancestorId, Guid descendantId);
        IEnumerable<Person> GetCommonAncestors(Guid personId1, Guid personId2);
    }
}
