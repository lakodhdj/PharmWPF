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

namespace PharmWPF
{
    /// <summary>
    /// Логика взаимодействия для AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        private Entities _db = new Entities();
        private bool _isEditing;
        private Product _product;
        public AddEditProductWindow(int? id)
        {
            InitializeComponent();

            if (id == null)
            {
                _isEditing = false;
            }
            else
            {
                _isEditing = true;
                _product = _db.Product.Find(id);
            }

            LoadData();
        }

        private void LoadData()
        {
            var types = _db.ProductType.ToList();

            ProductType.ItemsSource = types;
            ProductType.DisplayMemberPath = "Name";
            ProductType.SelectedValuePath = "id";

            if (_isEditing)
                FillData();
        }

        private void FillData()
        {
            ProductName.Text = _product.Name;
            ProductPrice.Text = _product.Price?.ToString();
            ProductDiscount.Text = _product.Discount?.ToString();
            ProductStockQuantity.Text = _product.StockQuantity?.ToString();
            ProductMinStock.Text = _product.MinStock?.ToString();

            ProductDesc.Text = _product.Description;
            ProductComposition.Text = _product.Composition;
            ProductPhoto.Text = _product.Photo;

            IsDiscountActive.IsChecked = _product.isDiscountActive;

            ProductType.SelectedValue = _product.ProductTypeID;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            new ProductWindow().Show();
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (_isEditing == true)
            {
                UpdateProduct();
            }
            else
                CreateProduct();
        }

        private void CreateProduct()
        {
            Product product = new Product();

            product.Name = ProductName.Text;
            product.Price = Convert.ToDecimal(ProductPrice.Text);
            product.Discount = Convert.ToDecimal(ProductDiscount.Text);
            product.StockQuantity = Convert.ToInt32(ProductStockQuantity.Text);
            product.MinStock = Convert.ToInt32(ProductMinStock.Text);

            product.Description = ProductDesc.Text;
            product.Composition = ProductComposition.Text;
            product.Photo = ProductPhoto.Text;

            product.isDiscountActive = IsDiscountActive.IsChecked;

            product.ProductTypeID = (int?)ProductType.SelectedValue;

            _db.Product.Add(product);
            _db.SaveChanges();

            CancelButton_Click(null, null);
        }

        private void UpdateProduct()
        {
            _product.Name = ProductName.Text;
            _product.Price = Convert.ToDecimal(ProductPrice.Text);
            _product.Discount = Convert.ToDecimal(ProductDiscount.Text);
            _product.StockQuantity = Convert.ToInt32(ProductStockQuantity.Text);
            _product.MinStock = Convert.ToInt32(ProductMinStock.Text);

            _product.Description = ProductDesc.Text;
            _product.Composition = ProductComposition.Text;
            _product.Photo = ProductPhoto.Text;

            _product.isDiscountActive = IsDiscountActive.IsChecked;

            _product.ProductTypeID = (int?)ProductType.SelectedValue;

            _db.SaveChanges();

            CancelButton_Click(null, null);
        }

    }
}

