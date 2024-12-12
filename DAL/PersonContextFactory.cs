using DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DAL
{
    public class PersonContextFactory : IDesignTimeDbContextFactory<PersonContext>
    {
        public PersonContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PersonContext>();
            // Задаем строку подключения для дизайн-тайма.
            var connectionString = "Data Source=people.db";
            optionsBuilder.UseSqlite(connectionString);

            return new PersonContext(connectionString);
        }
    }
}
