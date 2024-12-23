# GenealogyTreeSolution

## Описание
**GenealogyTreeSolution** – это консольное приложение для работы с генеалогическим древом. Приложение предоставляет функционал для создания людей, установления родственных связей (родители, дети, супруги), отображения дерева в графическом формате (ASCII), а также поиска предков, потомков и общих предков.

### Основные возможности:
- Создание и удаление людей.
- Добавление родителя или потомка для существующего человека.
- Установка супружеских связей.
- Отображение всех потомков и предков заданного человека.
- Вывод ближайших родственников (родителей, детей, супругов).
- Вычисление возраста предка при рождении потомка.
- Поиск общих предков для двух людей.
- Очистка всего генеалогического древа.
- Сохранение данных в базе данных (SQLite) с использованием Entity Framework Core.

---

## Используемые технологии
- **C# .NET 8**: язык и платформа для реализации приложения.
- **.NET CLI**: для сборки, запуска и управления проектами.
- **Entity Framework Core + SQLite**: для хранения данных и работы с базой.
- **SOLID-принципы** и **многоуровневая архитектура**:
  - **Domain**: доменные модели и интерфейсы.
  - **DAL (Data Access Layer)**: доступ к данным, реализация репозитория через EF Core.
  - **BLL (Business Logic Layer)**: бизнес-логика работы с генеалогическим древом.
  - **Presentation**: консольное приложение для взаимодействия с пользователем.
  - **Migrations**: управление схемой базы данных через EF Core.

---

## Общая структура решения
GenealogyTreeSolution/  
├── Domain/  
│   ├── Domain.csproj  
│   ├── Models/  
│   │   └── Person.cs        // Доменная сущность Person  
│   └── Interfaces/  
│       └── IRepository.cs   // Интерфейс репозитория для доступа к данным  
├── DAL/  
│   ├── DAL.csproj  
│   ├── EF/  
│   │   ├── PersonContext.cs // Контекст базы данных для EF Core  
│   │   └── Migrations/      // Миграции EF Core  
│   └── Repositories/  
│       └── EfRepository.cs  // Реализация IRepository с использованием EF  
├── BLL/  
│   ├── BLL.csproj  
│   └── Services/  
│       ├── IFamilyTreeService.cs // Интерфейс бизнес-логики  
│       └── GenealogyService.cs   // Реализация бизнес-логики  
└── Presentation/  
    ├── Presentation.csproj  
    └── Program.cs          // Консольный интерфейс, точка входа  



### Основные проекты:
- **Domain**: Определяет основные модели данных (`Person`) и их свойства, а также интерфейсы (`IRepository`) для абстракции доступа к данным.
- **DAL**: Реализует доступ к базе данных через EF Core, включая репозиторий (`EfRepository`) и контекст (`PersonContext`), а также миграции для управления схемой таблиц.
- **BLL**: Содержит бизнес-логику в виде сервиса (`GenealogyService`), который работает с репозиторием и предоставляет методы для работы с генеалогическим древом.
- **Presentation**: Консольное приложение, где пользователь может добавлять, удалять людей, устанавливать связи, просматривать дерево и выполнять другие операции.

---