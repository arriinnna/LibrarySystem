using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

namespace WpfApp1
{
    public static class Database
    {   
        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin";

        private static string dbPath = "library.db";
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        public static void Initialize()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Создание таблицы Users (если не существует)
                string sql = @"CREATE TABLE IF NOT EXISTS Users (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Username TEXT NOT NULL UNIQUE,
                                    Password TEXT NOT NULL
                                );";
                new SQLiteCommand(sql, connection).ExecuteNonQuery();

                // Создание таблицы Books (если не существует)
                sql = @"CREATE TABLE IF NOT EXISTS Books (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Title TEXT NOT NULL,
                                    Author TEXT NOT NULL,
                                    Genre TEXT NOT NULL,
                                    AvailableCount INTEGER DEFAULT 1,
                                    TotalReservations INTEGER DEFAULT 0
                                );";
                new SQLiteCommand(sql, connection).ExecuteNonQuery();

                // Создание таблицы Reservations (если не существует)
                sql = @"CREATE TABLE IF NOT EXISTS Reservations (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    BookId INTEGER NOT NULL,
                                    UserId INTEGER NOT NULL,
                                    ReservationDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                                    DueDate DATETIME NOT NULL,
                                    FOREIGN KEY (BookId) REFERENCES Books(Id),
                                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                                );";
                new SQLiteCommand(sql, connection).ExecuteNonQuery();

                // Добавляем тестовые данные, если база данных только что создана
                if (GetUsersCount() == 0)
                {
                    AddTestData(connection);
                }
            }
        }

        private static void AddTestData(SQLiteConnection connection)
        {
            // Тестовый администратор
            string sql = "INSERT INTO Users (Username, Password) VALUES ('admin', 'admin')";
            new SQLiteCommand(sql, connection).ExecuteNonQuery();

            // Тестовые книги
            var books = new List<Book>
            {
                new Book { Title = "Война и мир", Author = "Лев Толстой", Genre = "Классика", AvailableCount = 3 },
                new Book { Title = "Преступление и наказание", Author = "Федор Достоевский", Genre = "Классика", AvailableCount = 2 },
                new Book { Title = "1984", Author = "Джордж Оруэлл", Genre = "Антиутопия", AvailableCount = 5 },
                new Book { Title = "Мастер и Маргарита", Author = "Михаил Булгаков", Genre = "Роман", AvailableCount = 1 }
            };

            foreach (var book in books)
            {
                sql = $"INSERT INTO Books (Title, Author, Genre, AvailableCount) VALUES ('{book.Title}', '{book.Author}', '{book.Genre}', {book.AvailableCount})";
                new SQLiteCommand(sql, connection).ExecuteNonQuery();
            }
        }

        // Методы для работы с пользователями...

        public static bool IsAdmin(string username, string password)
        {
            return username == AdminUsername && password == AdminPassword;
        }

        public static bool RegisterUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                try
                {
                    return command.ExecuteNonQuery() == 1;
                }
                catch (SQLiteException)
                {
                    return false; // Пользователь уже существует
                }
            }
        }

        public static bool ValidateUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT 1 FROM Users WHERE Username = @username AND Password = @password";
                var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                return command.ExecuteScalar() != null;
            }
        }

        public static int? GetUserId(string username, string password)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id FROM Users WHERE Username = @username AND Password = @password";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int id))
                    {
                        return id;
                    }
                    return null;
                }
            }
        }

        public static Reader GetUserById(int userId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id, Username FROM Users WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Reader
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                FirstName = "Имя", // Временно, если нет этих полей
                                LastName = "Фамилия" // Временно, если нет этих полей
                            };
                        }
                        return null;
                    }
                }
            }
        }


        // Методы для работы с книгами

        public static bool AddBook(Book book)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Books (Title, Author, Genre, AvailableCount) VALUES (@Title, @Author, @Genre, @AvailableCount)";
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@Genre", book.Genre);
                    command.Parameters.AddWithValue("@AvailableCount", book.AvailableCount);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static List<Book> GetAllBooks()
        {
            var books = new List<Book>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Books";
                var command = new SQLiteCommand(sql, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Author = reader.GetString(2),
                            Genre = reader.GetString(3),
                            AvailableCount = reader.GetInt32(4),
                            TotalReservations = reader.GetInt32(5)
                        });
                    }
                }
            }

            return books;
        }

        public static int GetUsersCount()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM Users";
                return Convert.ToInt32(new SQLiteCommand(sql, connection).ExecuteScalar());
            }
        }

        public static List<GenreStat> GetGenreStatistics()
        {
            var stats = new List<GenreStat>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Genre, SUM(TotalReservations) as Popularity FROM Books GROUP BY Genre ORDER BY Popularity DESC";
                var command = new SQLiteCommand(sql, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stats.Add(new GenreStat
                        {
                            Genre = reader.GetString(0),
                            Popularity = reader.GetInt32(1)
                        });
                    }
                }
            }

            return stats;
        }

        public static List<AuthorStat> GetAuthorStatistics()
        {
            var stats = new List<AuthorStat>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Author, SUM(TotalReservations) as Popularity FROM Books GROUP BY Author ORDER BY Popularity DESC LIMIT 5";
                var command = new SQLiteCommand(sql, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stats.Add(new AuthorStat
                        {
                            Author = reader.GetString(0),
                            Popularity = reader.GetInt32(1)
                        });
                    }
                }
            }

            return stats;
        }

        public static List<ReservedBookInfo> GetReservedBooksForUser(int userId)
        {
            var reservedBooks = new List<ReservedBookInfo>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT b.Title, b.Author, r.DueDate
                                            FROM Reservations r
                                            JOIN Books b ON r.BookId = b.Id
                                            WHERE r.UserId = @UserId";
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime dueDate = DateTime.Parse(reader["DueDate"].ToString());
                            int daysLeft = (dueDate - DateTime.Now).Days;

                            reservedBooks.Add(new ReservedBookInfo
                            {
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                DueDate = dueDate,
                                DaysLeft = daysLeft >= 0 ? daysLeft : 0 // Корректное вычисление оставшихся дней
                            });
                        }
                    }
                }
            }

            return reservedBooks;
        }

        public static List<ReservedBookInfo> GetBooksNearingReturn(int userId, int days)
        {
            var nearingReturnBooks = new List<ReservedBookInfo>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT b.Title, r.DueDate
                                            FROM Reservations r
                                            JOIN Books b ON r.BookId = b.Id
                                            WHERE r.UserId = @UserId AND r.DueDate BETWEEN DATE('now') AND DATE('now', '+' || @Days || ' days')";
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Days", days);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nearingReturnBooks.Add(new ReservedBookInfo
                            {
                                Title = reader["Title"].ToString(),
                                DueDate = DateTime.Parse(reader["DueDate"].ToString())
                            });
                        }
                    }
                }
            }

            return nearingReturnBooks;
        }

        public static bool ReserveBook(int bookId, int days)
       
        {
            if (!MainWindow.CurrentUserId.HasValue)
            {
                return false; // Пользователь не авторизован
            }

            // Ограничение срока бронирования 7 днями
            if (days > 7)
            {
                throw new ArgumentException("Срок бронирования не может превышать 7 дней.");
            }

            int userId = MainWindow.CurrentUserId.Value;

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Books SET AvailableCount = AvailableCount - 1, TotalReservations = TotalReservations + 1 WHERE Id = @Id AND AvailableCount > 0";
                    command.Parameters.AddWithValue("@Id", bookId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        DateTime dueDate = DateTime.Now.AddDays(days);
                        command.CommandText = "INSERT INTO Reservations (BookId, UserId, DueDate) VALUES (@BookId, @UserId, @DueDate)";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@BookId", bookId);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@DueDate", dueDate);
                        command.ExecuteNonQuery();
                        return true;
                    }
                    return false;
                }
            }
        }
        public static void RemoveExpiredReservations()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Сначала получаем BookId всех просроченных бронирований
                using (var selectCommand = connection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT BookId FROM Reservations WHERE DueDate < DATE('now')";
                    var expiredBookIds = new List<int>();

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            expiredBookIds.Add(reader.GetInt32(0));
                        }
                    }

                    // Увеличиваем AvailableCount для каждой книги
                    foreach (var bookId in expiredBookIds)
                    {
                        using (var updateCommand = connection.CreateCommand())
                        {
                            updateCommand.CommandText = "UPDATE Books SET AvailableCount = AvailableCount + 1 WHERE Id = @BookId";
                            updateCommand.Parameters.AddWithValue("@BookId", bookId);
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                }

                // Удаляем просроченные бронирования
                using (var deleteCommand = connection.CreateCommand())
                {
                    deleteCommand.CommandText = "DELETE FROM Reservations WHERE DueDate < DATE('now')";
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }
        public static bool DeleteBook(int bookId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Books WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", bookId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        public static int GetReservedBooksCountForUser(int userId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Reservations WHERE UserId = @UserId";
                    command.Parameters.AddWithValue("@UserId", userId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int AvailableCount { get; set; }
        public int TotalReservations { get; set; }
    }

    public class GenreStat
    {
        public string Genre { get; set; }
        public int Popularity { get; set; }
    }

    public class AuthorStat
    {
        public string Author { get; set; }
        public int Popularity { get; set; }
    }

    public class Reader
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public int ReservedBooksCount { get; set; }
    }

    public class ReservedBookInfo
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime DueDate { get; set; } // Добавлено для уведомлений
        public int DaysLeft { get; set; }
    }
}