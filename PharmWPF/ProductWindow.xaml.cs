using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PharmWPF.Statics;
using PharmWPF.ViewModels;
using PharmWPF;
using System.Data.Entity;

namespace PharmWPF
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {

        // Создаем переменные для работы с базой, классом MessageHelper - простые короткие функции для ошибок и предупреждений и
        // ProductViewModels - класс для работы с таблицей Продуктов.
        private Entities _db = new Entities();
        private MessageHelper _mh = new MessageHelper();
        private List<ProductViewModels> _allProducts;

        // Список заранее прописанных вариантов сортировки
        private string[] _sortingTypes = new string[]
        {
            "По умолчанию",
            "По убыванию",
            "По возрастанию"
        };

        // Лист заранее прописанного варианта фильтрации, остальные буду подгружаться из БД
        private List<string> _filteringTypes = new List<string>()
        {
            "Все поставщики"
        };

        // Конструктор окна, создается автоматически, нужно только добавить LoadUI(), LoadProducts(), LoadData();
        public ProductWindow()
        {
            InitializeComponent();
            LoadUI();
            LoadProducts();
            LoadData();

        }

        // Загружаем Админ-панель только для менеджера и админа
        // LoadUI (Загрузка User Interface)
        private void LoadUI()
        {
            User user = CurrentSession.CurrentUser;
            if (user == null || user.RoleID == 3)
            {
                AdminPanel.Visibility = Visibility.Collapsed; // Если пользователь НЕ админ, то панель поиска, сортирвоки скрыта
                CreateButton.Visibility = Visibility.Collapsed; // Если пользователь НЕ админ, то кнопка Создать скрыта
            }
        }

        // Загрузку данных для сортировок и фильтрации
        // Сортировку подключаем автоматически, так как список есть заранее
        // Фильтры подключаем после загрузки данных из БД
        // Также для TextBox FullUserName присваиваем значение - НЕОБЯЗАТЕЛЬНО ДЕЛАТЬ

        private void LoadData()
        {
            SortingComboBox.ItemsSource = _sortingTypes;

            _filteringTypes.AddRange(
                _db.ProductType.Select(pt => pt.Name)
            );

            FilteringComboBox.ItemsSource = _filteringTypes;

            var user = CurrentSession.CurrentUser;
            if (user != null)
            {
                FullUserName.Text = $"{user.LastName} {user.FirstName}";
            }
        }

        // Загрузка продуктов из таблицы Product включаем напрямую связанные таблицы (Provider, Producer etc), чтобы не было null значений
        // Превращаем в список и помещаем в _allProducts, который является источником данных
        private void LoadProducts()
        {
            _allProducts = _db.Product
                .Include(p => p.ProductType)
                .ToList()
                .Select(p => new ProductViewModels(p))
                .ToList();

            ProductList.ItemsSource = _allProducts;
        }

        // При нажатии на кнопку "Выйти" текущий пользователь обнуляется
        // Открывается окно авторизации и закрывается окно продуктов
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.CurrentUser = null;
            new MainWindow().Show();
            Close();
        }

        // При нажатии на кнопку "Создать" открывается окно добавления/редактирования продукта, но с пустыми полями, так как id не передается
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new AddEditProductWindow(null).Show();
            Close();
        }

        // Функция, которая используется тремя элементами Админ-панели
        private void FilterChanged(object sender, EventArgs e) => ApplyFilters();


        private void ApplyFilters()
        {
            var query = _allProducts.AsQueryable();

            // 🔍 Поиск
            string text = SearchingTextBox.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(text)) ||
                    (p.Description != null && p.Description.ToLower().Contains(text)) ||
                    (p.Composition != null && p.Composition.ToLower().Contains(text))
                );
            }

            // 🔽 Фильтрация по типу продукта
            var filter = (FilteringComboBox.SelectedItem as string)?.ToLower();

            if (!string.IsNullOrEmpty(filter) && filter != "все поставщики")
            {
                query = query.Where(p =>
                    p.ProductType != null &&
                    p.ProductType.Name.ToLower() == filter
                );
            }

            // 🔼 Сортировка по количеству
            switch (SortingComboBox.SelectedIndex)
            {
                case 1:
                    query = query.OrderByDescending(p => p.StockQuantity);
                    break;
                case 2:
                    query = query.OrderBy(p => p.StockQuantity);
                    break;
            }

            ProductList.ItemsSource = query.ToList();
        }

        // Кнопка Редактирования
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка прав
            var user = CurrentSession.CurrentUser;
            if (user == null || user.id == 3)
            {
                MessageBox.Show("Нет прав для редактирования", "Доступ запрещён",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Безопасное получение id из Tag
            // При нажатии на кнопку автоматически передается АЙДИ продукта
            try
            {
                int id = (int)((Button)sender).Tag;
                new AddEditProductWindow(id).Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:\n" + ex.Message);
            }
        }

        // Кнопка Удалить
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Забираем по кнопке АЙДИ продукта
            int id = (int)((Button)sender).Tag;
            // Берем текущего юзера
            var user = CurrentSession.CurrentUser;

            // Проверяем пользователя
            if (user == null || user.RoleID == 3)
            {
                _mh.ShowWarning("Нет прав на удаление.");
                return;
            }

            // Окно подтверждения удаления 
            if (MessageBox.Show("Удалить товар?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            // Удаление из БД и из отобржаемого списка продуктов
            try
            {
                var product = _db.Product.FirstOrDefault(p => p.id == id);
                if (product == null) return;

                _db.Product.Remove(product);
                _db.SaveChanges();

                var item = _allProducts.FirstOrDefault(p => p.id == id);
                if (item != null)
                {
                    _allProducts.Remove(item);
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления:\n" + ex.Message);
            }
        }

        // Открытие панели Заказов
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
            Close();
        }
    }
}
