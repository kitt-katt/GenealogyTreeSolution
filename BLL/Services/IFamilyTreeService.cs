using System;
using System.Collections.Generic;
using Domain.Models;

namespace BLL.Services
{
    public interface IFamilyTreeService
    {
        Person CreatePerson(string fullName, DateTime birthDate, Gender gender);
        void AddPersonToTree(Person person);
        void SetParentChildRelationship(Guid parentId, Guid childId);
        void SetSpouseRelationship(Guid personId, Guid spouseId);
        IEnumerable<Person> GetParents(Guid personId);
        IEnumerable<Person> GetChildren(Guid personId);
        void DisplayTree();
        void ClearTree();
        int GetAncestorAgeAtDescendantBirth(Guid ancestorId, Guid descendantId);
        IEnumerable<Person> GetCommonAncestors(Guid personId1, Guid personId2);
    }
}
