using tagr.Exceptions;
using tagr.Mapping;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.UnitOfWork;
using tagr.ViewModels;

namespace tagr.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OrderListItemViewModel>> GetAllAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllWithDetailsAsync();
            return orders.ToListItemViewModels();
        }
        public async Task<List<OrderListItemViewModel>> GetByCustomerIdAsync(string customerId)
        {
            var orders = await _unitOfWork.Orders.GetByCustomerIdAsync(customerId);
            return orders.ToListItemViewModels();
        }
        public async Task<OrderDetailsViewModel> GetDetailsAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(id) ?? throw new NotFoundException(nameof(Order), id); ;
            return order.ToDetailsViewModel();
        }
        public async Task<string> GetOwnerIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Order), id); ;
            return order.CustomerId;
        }
        public async Task<int> CreateAsync(OrderCreateViewModel model, string customerId)
        {
            if (!model.Items.Any())
                throw new BusinessRuleException("Order must contain at least one item.");

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                PhoneNumber = model.PhoneNumber,
                ShippingAddress = model.ShippingAddress,
            };

            decimal totalAmount = 0;

            foreach (var item in model.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId) ?? throw new NotFoundException(nameof(Product), item.ProductId);
                if (product.StockQuantity < item.Quantity)
                    throw new BusinessRuleException($"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}.");

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                product.StockQuantity -= item.Quantity;
                totalAmount += orderItem.Quantity * orderItem.UnitPrice;

                order.OrderItems.Add(orderItem);
            }

            order.TotalAmount = totalAmount;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return order.Id;
        }
        public async Task UpdateStatusAsync(OrderStatusUpdateViewModel model)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(model.Id) ?? throw new NotFoundException(nameof(Order), model.Id);

            order.Status = model.Status;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
