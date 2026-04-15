    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
using System.Windows.Media;

    namespace PharmWPF.ViewModels
    {
        public class ProductViewModels
        {
            public ProductViewModels(Product product)
            {
                id = product.id;
                Name = product.Name;
                ProductTypeID = product.ProductTypeID;
                Description = product.Description;
                Composition = product.Composition;
                Price = product.Price;
                Discount = product.Discount;
                isDiscountActive = product.isDiscountActive;
                MinStock = product.MinStock;
                StockQuantity = product.StockQuantity;
                Photo = product.Photo;

                this.ProductType = product.ProductType;


                GetBackground();
                GetPhoto();
            }

            public int id { get; set; }
            public string Name { get; set; }
            public Nullable<int> ProductTypeID { get; set; }
            public string Description { get; set; }
            public string Composition { get; set; }
            public Nullable<decimal> Price { get; set; }
            public Nullable<decimal> Discount { get; set; }
            public Nullable<bool> isDiscountActive { get; set; }
            public Nullable<int> MinStock { get; set; }
            public Nullable<int> StockQuantity { get; set; }
            public string Photo { get; set; }

            public Brush Background { get; set; }

            public virtual ICollection<Order> Order { get; set; }
            public virtual ProductType ProductType { get; set; }

            private void GetBackground()
            {
                if (Discount >= 15)
                {
                    Background = (Brush)new BrushConverter().ConvertFrom("#67BA80");
                    return;
                }
                else if (StockQuantity == 0)
                {
                    Background = Brushes.LightBlue;
                    return;
                }
            }

            private void GetPhoto()
            {
                if (!string.IsNullOrEmpty(Photo))
                    return;
                Photo = "/Res/picture.png";
            }



    }

    }
