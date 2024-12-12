using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Domain.Models;

namespace DAL.DataAccess
{
    public class JsonDataStorage
    {
        private readonly string _filePath;
        private List<Person> _persons;

        public JsonDataStorage(string filePath)
        {
            _filePath = filePath;
            if (!File.Exists(_filePath))
            {
                _persons = new List<Person>();
                Save();
            }
            else
            {
                Load();
            }
        }

        public List<Person> Persons => _persons;

        public void Save()
        {
            var json = JsonSerializer.Serialize(_persons, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        private void Load()
        {
            var json = File.ReadAllText(_filePath);
            _persons = JsonSerializer.Deserialize<List<Person>>(json) ?? new List<Person>();
        }
    }
}
