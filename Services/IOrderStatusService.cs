using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public interface IOrderStatusService
    {
        Task<IPagedList<OrderStatus>> GetAllOrderStatusAsync(string name = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        Task InsertOrderStatusAsync(OrderStatus orderStatus);
        Task<OrderStatus> GetOrderStatusByIdAsync(int orderStatusId);
        Task UpdateOrderStatusAsync(OrderStatus orderStatus);
        Task DeleteOrderStatusAsync(OrderStatus orderStatus);
        Task<bool> IsOrderStatesNameFoundAsync(string Name,int Id);
        Task<string> GetOrderStatusNameAsync(int statusId);
    }
}
