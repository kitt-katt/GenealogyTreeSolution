using System;
using System.Linq;
using BLL.Services;
using DAL.EF;
using DAL.Repositories;
using Domain.Models;

class Program
{
    static void Main()
    {
        var connectionString = "Data Source=people.db";
        var context = new PersonContext(connectionString);
        var repository = new EfRepository(context);
        var service = new GenealogyService(repository);

        while (true)
        {
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Создать человека");
            Console.WriteLine("2. Удалить человека");
            Console.WriteLine("3. Создать родителя для человека по id");
            Console.WriteLine("4. Создать потомка для человека по id");
            Console.WriteLine("5. Установить отношение супруги");
            Console.WriteLine("6. Показать всех потомков для человека по id");
            Console.WriteLine("7. Показать всех предков для человека по id");
            Console.WriteLine("8. Показать все дерево");
            Console.WriteLine("9. Очистить все дерево");
            Console.WriteLine("10. Вывести ближайших родственников (родителей и детей)");
            Console.WriteLine("11. Вычислить возраст предка при рождении потомка");
            Console.WriteLine("12. Искать общих предков для двух выбранных людей");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");
            var choice = Console.ReadLine();

            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                    CreatePerson(service);
                    break;
                case "2":
                    DeletePerson(service);
                    break;
                case "3":
                    AddParent(service);
                    break;
                case "4":
                    AddChild(service);
                    break;
                case "5":
                    SetSpouse(service);
                    break;
                case "6":
                    ShowDescendants(service);
                    break;
                case "7":
                    ShowAncestors(service);
                    break;
                case "8":
                    service.DisplayTree();
                    break;
                case "9":
                    service.ClearTree();
                    Console.WriteLine("Древо очищено.");
                    break;
                case "10":
                    ShowCloseRelatives(service);
                    break;
                case "11":
                    CalculateAncestorAge(service);
                    break;
                case "12":
                    FindCommonAncestors(service);
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }

            Console.WriteLine();
        }
    }

    static void CreatePerson(IFamilyTreeService service)
    {
        Console.Write("Введите ФИО: ");
        var name = Console.ReadLine();
        Console.Write("Введите дату рождения (гггг-мм-дд): ");
        var dateStr = Console.ReadLine();
        Console.Write("Введите пол (Male/Female/Other): ");
        var genderStr = Console.ReadLine();

        if (DateTime.TryParse(dateStr, out var birthDate) &&
            Enum.TryParse<Gender>(genderStr, out var gender))
        {
            var person = service.CreatePerson(name, birthDate, gender);
            service.AddPersonToTree(person);
            Console.WriteLine($"Человек создан и добавлен в древо: ID = {person.Id}");
        }
        else
        {
            Console.WriteLine("Некорректный ввод.");
        }
    }

    static void DeletePerson(IFamilyTreeService service)
    {
        Console.Write("Введите ID человека: ");
        var idStr = Console.ReadLine();
        if (Guid.TryParse(idStr, out var id))
        {
            service.DeletePerson(id);
            Console.WriteLine("Человек удалён.");
        }
        else
        {
            Console.WriteLine("Некорректный ID.");
        }
    }

    static void AddParent(IFamilyTreeService service)
    {
        Console.Write("Введите ID человека (ребёнка): ");
        var childIdStr = Console.ReadLine();
        if (!Guid.TryParse(childIdStr, out var childId))
        {
            Console.WriteLine("Некорректный ID ребёнка.");
            return;
        }

        Console.Write("Введите ФИО родителя: ");
        var name = Console.ReadLine();
        Console.Write("Введите дату рождения родителя (гггг-мм-дд): ");
        var dateStr = Console.ReadLine();
        Console.Write("Введите пол родителя (Male/Female/Other): ");
        var genderStr = Console.ReadLine();

        if (DateTime.TryParse(dateStr, out var birthDate) &&
            Enum.TryParse<Gender>(genderStr, out var gender))
        {
            var parent = service.CreatePerson(name, birthDate, gender);
            service.AddParentToPerson(childId, parent);
            Console.WriteLine($"Родитель добавлен к человеку {childId}. ID родителя = {parent.Id}");
        }
        else
        {
            Console.WriteLine("Некорректный ввод.");
        }
    }

    static void AddChild(IFamilyTreeService service)
    {
        Console.Write("Введите ID человека (родителя): ");
        var parentIdStr = Console.ReadLine();
        if (!Guid.TryParse(parentIdStr, out var parentId))
        {
            Console.WriteLine("Некорректный ID родителя.");
            return;
        }

        Console.Write("Введите ФИО ребёнка: ");
        var name = Console.ReadLine();
        Console.Write("Введите дату рождения (гггг-мм-дд): ");
        var dateStr = Console.ReadLine();
        Console.Write("Введите пол (Male/Female/Other): ");
        var genderStr = Console.ReadLine();

        if (DateTime.TryParse(dateStr, out var birthDate) &&
            Enum.TryParse<Gender>(genderStr, out var gender))
        {
            var child = service.CreatePerson(name, birthDate, gender);
            service.AddChildToPerson(parentId, child);
            Console.WriteLine($"Потомок добавлен к человеку {parentId}. ID потомка = {child.Id}");
        }
        else
        {
            Console.WriteLine("Некорректный ввод.");
        }
    }

    static void SetSpouse(IFamilyTreeService service)
    {
        Console.Write("Введите ID первого супруга: ");
        var sid1Str = Console.ReadLine();
        Console.Write("Введите ID второго супруга: ");
        var sid2Str = Console.ReadLine();

        if (Guid.TryParse(sid1Str, out var sid1) && Guid.TryParse(sid2Str, out var sid2))
        {
            service.SetSpouseRelationship(sid1, sid2);
            Console.WriteLine("Отношение супругов установлено.");
        }
        else
        {
            Console.WriteLine("Некорректный ввод.");
        }
    }

    static void ShowDescendants(IFamilyTreeService service)
    {
        Console.Write("Введите ID человека: ");
        var idStr = Console.ReadLine();
        if (Guid.TryParse(idStr, out var id))
        {
            var descendants = service.GetAllDescendants(id).ToList();
            if (descendants.Any())
            {
                Console.WriteLine("Потомки:");
                foreach (var d in descendants)
                {
                    Console.WriteLine($"- {d.FullName} ({d.Id})");
                }
            }
            else
            {
                Console.WriteLine("Нет потомков.");
            }
        }
        else
        {
            Console.WriteLine("Некорректный ID.");
        }
    }

    static void ShowAncestors(IFamilyTreeService service)
    {
        Console.Write("Введите ID человека: ");
        var idStr = Console.ReadLine();
        if (Guid.TryParse(idStr, out var id))
        {
            var ancestors = service.GetAllAncestors(id).ToList();
            if (ancestors.Any())
            {
                Console.WriteLine("Предки:");
                foreach (var a in ancestors)
                {
                    Console.WriteLine($"- {a.FullName} ({a.Id})");
                }
            }
            else
            {
                Console.WriteLine("Нет предков.");
            }
        }
        else
        {
            Console.WriteLine("Некорректный ID.");
        }
    }

    static void ShowCloseRelatives(IFamilyTreeService service)
    {
        Console.Write("Введите ID человека: ");
        var idStr = Console.ReadLine();
        if (Guid.TryParse(idStr, out var id))
        {
            var person = service.GetAllPersons().FirstOrDefault(p => p.Id == id);
            if (person == null)
            {
                Console.WriteLine("Человек с таким ID не найден.");
                return;
            }
            
            var parents = service.GetParents(id).ToList();
            var children = service.GetChildren(id).ToList();
            var spouse = person.Spouse; // так как мы подгружаем из репозитория с Include, жена/муж должны быть доступны

            Console.WriteLine("Родители: " + (parents.Any() ? string.Join(", ", parents.Select(p => p.FullName)) : "Нет"));
            Console.WriteLine("Дети: " + (children.Any() ? string.Join(", ", children.Select(p => p.FullName)) : "Нет"));
            Console.WriteLine("Супруг(а): " + (spouse != null ? spouse.FullName : "Нет"));
        }
        else
        {
            Console.WriteLine("Некорректный ID.");
        }
    }


    static void CalculateAncestorAge(IFamilyTreeService service)
    {
        Console.Write("Введите ID предка: ");
        var aidStr = Console.ReadLine();
        Console.Write("Введите ID потомка: ");
        var didStr = Console.ReadLine();

        if (Guid.TryParse(aidStr, out var aid) && Guid.TryParse(didStr, out var did))
        {
            int age = service.GetAncestorAgeAtDescendantBirth(aid, did);
            if (age >= 0)
                Console.WriteLine($"Возраст предка при рождении потомка: {age} лет.");
            else
                Console.WriteLine("Невозможно вычислить возраст.");
        }
        else
        {
            Console.WriteLine("Некорректный ввод.");
        }
    }

    static void FindCommonAncestors(IFamilyTreeService service)
    {
        Console.Write("Введите ID первого человека: ");
        var p1Str = Console.ReadLine();
        Console.Write("Введите ID второго человека: ");
        var p2Str = Console.ReadLine();

        if (Guid.TryParse(p1Str, out var p1) && Guid.TryParse(p2Str, out var p2))
        {
            var common = service.GetCommonAncestors(p1, p2).ToList();
            if (common.Any())
            {
                Console.WriteLine("Общие предки:");
                foreach (var c in common)
                {
                    Console.WriteLine($"- {c.FullName} ({c.Id})");
                }
            }
            else
            {
                Console.WriteLine("Нет общих предков.");
            }
        }
        else
        {
            Console.WriteLine("Некорректный ввод.");
        }
    }
}
