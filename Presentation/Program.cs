using System;
using System.Linq;
using BLL.Services;
using DAL.Repositories;
using Domain.Models;

class Program
{
    static void Main()
    {
        var repository = new FileRepository("data.json");
        var service = new GenealogyService(repository);

        while (true)
        {
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Создать человека");
            Console.WriteLine("2. Добавить человека в древо");
            Console.WriteLine("3. Установить отношение родитель-ребенок");
            Console.WriteLine("4. Установить отношение супруги");
            Console.WriteLine("5. Показать ближайших родственников (родители и дети)");
            Console.WriteLine("6. Показать все древо");
            Console.WriteLine("7. Вычислить возраст предка при рождении потомка");
            Console.WriteLine("8. Очистить древо");
            Console.WriteLine("9. Найти общих предков для двух человек");
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
                    AddPerson(service);
                    break;
                case "3":
                    SetParentChild(service);
                    break;
                case "4":
                    SetSpouse(service);
                    break;
                case "5":
                    ShowRelatives(service);
                    break;
                case "6":
                    service.DisplayTree();
                    break;
                case "7":
                    CalculateAncestorAge(service);
                    break;
                case "8":
                    service.ClearTree();
                    Console.WriteLine("Древо очищено.");
                    break;
                case "9":
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
            Console.WriteLine($"Человек создан: ID = {person.Id}");
            // Здесь мы только создали объект в памяти, но не добавили в репозиторий.
            // Можно добавить сразу:
            service.AddPersonToTree(person);
            Console.WriteLine("Человек добавлен в древо.");
        }
        else
        {
            Console.WriteLine("Некорректный ввод.");
        }
    }

    static void AddPerson(IFamilyTreeService service)
    {
        Console.Write("Введите ID человека (Guid): ");
        var idStr = Console.ReadLine();
        if (Guid.TryParse(idStr, out var id))
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
                var person = new Person
                {
                    Id = id,
                    FullName = name,
                    BirthDate = birthDate,
                    Gender = gender
                };
                service.AddPersonToTree(person);
                Console.WriteLine("Человек добавлен в древо.");
            }
            else
            {
                Console.WriteLine("Некорректный ввод данных.");
            }
        }
        else
        {
            Console.WriteLine("Некорректный ID.");
        }
    }

    static void SetParentChild(IFamilyTreeService service)
    {
        Console.Write("Введите ID родителя: ");
        var pidStr = Console.ReadLine();
        Console.Write("Введите ID ребенка: ");
        var cidStr = Console.ReadLine();

        if (Guid.TryParse(pidStr, out var pid) && Guid.TryParse(cidStr, out var cid))
        {
            service.SetParentChildRelationship(pid, cid);
            Console.WriteLine("Отношение родитель-ребенок установлено.");
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

    static void ShowRelatives(IFamilyTreeService service)
    {
        Console.Write("Введите ID человека: ");
        var idStr = Console.ReadLine();
        if (Guid.TryParse(idStr, out var id))
        {
            var parents = service.GetParents(id).ToList();
            var children = service.GetChildren(id).ToList();

            Console.WriteLine("Родители: " + (parents.Any() ? string.Join(", ", parents.Select(p => p.FullName)) : "Нет"));
            Console.WriteLine("Дети: " + (children.Any() ? string.Join(", ", children.Select(p => p.FullName)) : "Нет"));
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
                    Console.WriteLine($"  {c.FullName}");
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
