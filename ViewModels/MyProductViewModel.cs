namespace tagr.ViewModels
{
    public class MyProductListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;
    }
        public class SellerDashboardViewModel
        {
            public int TotalProducts { get; set; }

            public int TotalOrders { get; set; }

            public int PendingOrders { get; set; }

            public decimal TotalSales { get; set; }
        }
}