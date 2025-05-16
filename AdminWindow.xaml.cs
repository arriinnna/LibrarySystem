using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1
{
    public partial class AdminWindow : Window
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int UsersCount { get; set; }
        public List<Book> AllBooks { get; set; }
        public List<GenreStat> PopularGenres { get; set; }
        public List<AuthorStat> PopularAuthors { get; set; }

        public AdminWindow()
        {
            InitializeComponent();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            AllBooks = Database.GetAllBooks();
            TotalBooks = AllBooks.Count;
            AvailableBooks = AllBooks.Sum(b => b.AvailableCount);
            UsersCount = Database.GetUsersCount();
            PopularGenres = Database.GetGenreStatistics();
            PopularAuthors = Database.GetAuthorStatistics();
            booksGrid.ItemsSource = AllBooks;
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            var addBookWindow = new AddBookWindow();
            if (addBookWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Book bookToDelete)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить книгу \"{bookToDelete.Title}\"?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = Database.DeleteBook(bookToDelete.Id);
                    if (success)
                    {
                        MessageBox.Show($"Книга \"{bookToDelete.Title}\" успешно удалена.", "Удалено", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить книгу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}