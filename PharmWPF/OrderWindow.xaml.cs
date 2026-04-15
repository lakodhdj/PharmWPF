using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using PharmWPF.ViewModels;

using PharmWPF.Statics; 

namespace PharmWPF
{
    public partial class OrderWindow : Window
    {
        // Предполагается, что сущности в новом проекте называются PM01Entities или аналогично
        private Entities _db = new Entities();
        private List<OrderViewModels> _allOrders;

        private string[] _sortingTypes = new string[]
        {
            "По умолчанию",
            "Сначала новые",
            "Сначала старые"
        };

        private List<string> _filteringTypes = new List<string>()
        {
            "Все статусы"
        };

        public OrderWindow()
        {
            InitializeComponent();
            LoadUI();
            LoadData(); // Загружаем комбобоксы до заказов
            LoadOrders();
        }

        private void FilterChanged(object sender, EventArgs e) => ApplyFilters();

        private void LoadUI()
        {
            // Получаем пользователя из вашей системы сессий
            var user = CurrentSession.CurrentUser;
            if (user == null || user.RoleID == 3) // Предположим 3 - это роль с ограниченным доступом
            {
                AdminPanelOrder.Visibility = Visibility.Collapsed;
            }
            else
            {
                CreateButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadData()
        {
            SortingComboBox.ItemsSource = _sortingTypes;
            SortingComboBox.SelectedIndex = 0;

            // Заполняем фильтрацию статусами из БД
            var statuses = _db.OrderStatus.Select(s => s.Name).ToList();
            _filteringTypes.AddRange(statuses);
            FilteringComboBox.ItemsSource = _filteringTypes;
            FilteringComboBox.SelectedIndex = 0;

            var user = CurrentSession.CurrentUser;
            if (user != null)
            {
                FullUserName.Text = $"{user.FirstName} {user.LastName} ({user.Role.Name})";
            }
        }

        private void LoadOrders()
        {
            // Обновленные Include: убираем PickUpPoint, добавляем Product и OrderStatus
            _allOrders = _db.Order
                .Include(o => o.OrderStatus)
                .Include(o => o.Product)
                .Include(o => o.User)
                .ToList()
                .Select(o => new OrderViewModels(o))
                .ToList();

            OrderList.ItemsSource = _allOrders;
        }

        private void ApplyFilters()
        {
            if (_allOrders == null) return;

            var query = _allOrders.AsQueryable();

            // Поиск по адресу доставки или названию продукта
            string text = SearchingTextBox.Text.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(o =>
                    o.id.ToString().Contains(text) ||
                    (o.DeliveryAddress != null && o.DeliveryAddress.ToLower().Contains(text)) ||
                    (o.Product != null && o.Product.Name.ToLower().Contains(text)));
            }

            // Сортировка по дате
            switch (SortingComboBox.SelectedIndex)
            {
                case 1: // Сначала новые
                    query = query.OrderByDescending(o => o.OrderDate);
                    break;
                case 2: // Сначала старые
                    query = query.OrderBy(o => o.OrderDate);
                    break;
            }

            // Фильтрация по статусу
            string filter = FilteringComboBox.SelectedValue as string;
            if (!string.IsNullOrWhiteSpace(filter) && filter != "Все статусы")
            {
                query = query.Where(o => o.OrderStatus != null && o.OrderStatus.Name == filter);
            }

            OrderList.ItemsSource = query.ToList();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на окно продуктов (убедитесь, что имя класса совпадает)
            new ProductWindow().Show();
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new AddEditOrderWindow().Show();
            Close();
        }
    }
}