using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmWPF.ViewModels
{
    public class OrderViewModels
    {
        public OrderViewModels(Order order)
        {
            id = order.id;
            ProductID = order.ProductID;
            DeliveryAddress = order.DeliveryAddress;
            UserFIOID = order.UserFIOID;
            UserRoleID = order.UserRoleID;
            UserLoginID = order.UserLoginID;
            Quantity = order.Quantity;
            OrderDate = order.OrderDate;
            OrderDelivery = order.OrderDelivery;
            OrderStatusID = order.OrderStatusID;
            Quantity = order.Quantity;
            PriceINOrder = order.PriceINOrder;
            OrderTotal = order.OrderTotal;
            ProductTypeID = order.ProductTypeID;



            this.Product = order.Product;
            this.ProductType = order.ProductType;
            this.User = order.User; // Связь через UserFIOID
            this.User1 = order.User1; // Связь через UserLoginID
            this.OrderStatus = order.OrderStatus;
            this.Role = order.Role;
        }
        public int id { get; set; }
        public string DeliveryAddress { get; set; }
        public Nullable<int> UserFIOID { get; set; }
        public Nullable<int> UserLoginID { get; set; }
        public Nullable<int> UserRoleID { get; set; }
        public Nullable<int> OrderStatusID { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.DateTime> OrderDelivery { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> ProductTypeID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> PriceINOrder { get; set; }
        public Nullable<decimal> OrderTotal { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductType ProductType { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
