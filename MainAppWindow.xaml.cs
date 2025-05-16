using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;

namespace WpfApp1
{
    public partial class MainAppWindow : Window
    {
        public MainAppWindow()
        {
            InitializeComponent();
            Database.Initialize(); // Инициализация базы данных при запуске
            LoadBooksFromDatabase();
        }

        // Метод для загрузки книг из базы и отображения в таблице
        private void LoadBooksFromDatabase()
        {
            try
            {
                var books = Database.GetAllBooks(); // Получаем список книг из БД
                booksGrid.ItemsSource = books;      // Отображаем в DataGrid
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке книг: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Поиск книг по выбранному полю
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string query = txtSearchQuery.Text.ToLower();
            string searchField = ((ComboBoxItem)cbSearchField.SelectedItem)?.Content.ToString();
            if (string.IsNullOrEmpty(searchField))
                return;

            List<Book> allBooks = Database.GetAllBooks();
            List<Book> searchResults = new List<Book>();

            switch (searchField)
            {
                case "По названию":
                    searchResults = allBooks.Where(book => book.Title.ToLower().Contains(query)).ToList();
                    break;
                case "По автору":
                    searchResults = allBooks.Where(book => book.Author.ToLower().Contains(query)).ToList();
                    break;
                case "По жанру":
                    searchResults = allBooks.Where(book => book.Genre.ToLower().Contains(query)).ToList();
                    break;
            }

            booksGrid.ItemsSource = searchResults;
        }

        // Сброс фильтра и обновление списка
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            txtSearchQuery.Text = "";
            cbSearchField.SelectedIndex = 0;
            LoadBooksFromDatabase();
        }

        // Обработчик кнопки бронирования книги
        private void ReserveBook_Click(object sender, RoutedEventArgs e)
        {
            if (booksGrid.SelectedItem is Book selectedBook)
            {
                int days;
                // Проверяем ввод дней бронирования
                if (!int.TryParse(DaysTextBox.Text, out days))
                {
                    MessageBox.Show("Введите корректное число дней.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (days > 7)
                {
                    MessageBox.Show("Нельзя забронировать книгу на срок больше 7 дней.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Попытка забронировать книгу через метод базы данных
                bool success = Database.ReserveBook(selectedBook.Id, days);

                if (success)
                {
                    MessageBox.Show($"Книга успешно забронирована на {days} дней.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBooksFromDatabase(); // обновляем список после бронирования
                }
                else
                {
                    MessageBox.Show("Не удалось забронировать книгу. Возможно, она недоступна.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите книгу для бронирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Открыть личный кабинет пользователя
        private void OpenCabinet_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserId.HasValue)
            {
                Database.RemoveExpiredReservations(); // <<< Добавь это здесь

                int userId = MainWindow.CurrentUserId.Value;
                var reader = Database.GetUserById(userId);
                if (reader != null)
                {
                    var cabinetWindow = new ReaderCabinetWindow(reader);
                    cabinetWindow.Owner = this;
                    cabinetWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Ошибка: не удалось получить информацию о пользователе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Ошибка: пользователь не авторизован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}