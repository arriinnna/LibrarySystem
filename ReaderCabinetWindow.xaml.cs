using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfApp1
{
    public partial class ReaderCabinetWindow : Window
    {
        public Reader Reader { get; }
        public string BooksStatus => "Здесь будет информация о выданных книгах";
        public ObservableCollection<string> Notifications { get; } = new ObservableCollection<string>();
        public ObservableCollection<ReservedBookInfo> ReservedBooks { get; } = new ObservableCollection<ReservedBookInfo>();

        public ReaderCabinetWindow(Reader reader)
        {
            InitializeComponent();
            Reader = reader;
            DataContext = this;
            LoadReservedBooks(Reader.Id); // Используем ID из объекта Reader
            CheckForReturnNotifications(Reader.Id); // Используем ID из объекта Reader
        }

        private void LoadReservedBooks(int userId)
        {
            ReservedBooks.Clear();
            var reservedBooksFromDb = Database.GetReservedBooksForUser(userId);
            foreach (var reservedBook in reservedBooksFromDb)
            {
                ReservedBooks.Add(reservedBook);
            }
            Reader.ReservedBooksCount = ReservedBooks.Count;
        }

        private void CheckForReturnNotifications(int userId)
        {
            var nearingDueDateBooks = Database.GetBooksNearingReturn(userId, 3);
            if (nearingDueDateBooks.Any())
            {
                Notifications.Add("Внимание! Следующие книги необходимо вернуть в ближайшие 3 дня:");
                foreach (var book in nearingDueDateBooks)
                {
                    Notifications.Add($"- \"{book.Title}\" (вернуть до {book.DueDate.ToShortDateString()})");
                }
            }
            else
            {
                Notifications.Add("Пока нет уведомлений о возврате книг.");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}