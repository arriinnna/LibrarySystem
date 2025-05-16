using System;
using System.Windows;

namespace WpfApp1
{
    public partial class AddBookWindow : Window
    {
        public AddBookWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtAuthor.Text) ||
                string.IsNullOrWhiteSpace(txtGenre.Text) ||
                !int.TryParse(txtAvailableCount.Text, out int availableCount) ||
                availableCount < 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newBook = new Book
            {
                Title = txtTitle.Text,
                Author = txtAuthor.Text,
                Genre = txtGenre.Text,
                AvailableCount = availableCount
            };

            if (Database.AddBook(newBook))
            {
                MessageBox.Show($"Книга \"{newBook.Title}\" успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true; // Закрываем окно с результатом "успех"
            }
            else
            {
                MessageBox.Show("Произошла ошибка при добавлении книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false; // Закрываем окно с результатом "неудача"
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закрываем окно с результатом "отмена"
        }
    }
}