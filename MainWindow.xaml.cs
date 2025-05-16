using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public static int? CurrentUserId { get; private set; } // Статическое свойство для хранения ID текущего пользователя

        public MainWindow()
        {
            InitializeComponent();
            Database.Initialize(); // Инициализация базы данных при запуске
            CurrentUserId = null; // Сбрасываем ID пользователя при открытии окна логина
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            if (Database.IsAdmin(username, password))
            {
                MessageBox.Show("Вход как администратор");
                var adminWindow = new AdminWindow();
                adminWindow.Show();
                this.Close();
            }
            else
            {
                int? userId = Database.GetUserId(username, password); // Получаем ID пользователя
                if (userId.HasValue)
                {
                    CurrentUserId = userId.Value; // Сохраняем ID текущего пользователя
                    MessageBox.Show("Авторизация успешна!");
                    var mainApp = new MainAppWindow();
                    mainApp.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            if (Database.RegisterUser(username, password))
            {
                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.");
            }
            else
            {
                MessageBox.Show("Пользователь с таким логином уже существует");
            }
        }
    }
}