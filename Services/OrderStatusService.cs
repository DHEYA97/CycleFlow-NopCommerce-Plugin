using DocumentFormat.OpenXml.EMMA;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        #region Fields
        private readonly IRepository<OrderStatus> _orderStatusRepository;
        #endregion
        #region Ctor
        public OrderStatusService(IRepository<OrderStatus> orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }
        #endregion
        #region Method
        public async Task<IPagedList<OrderStatus>> GetAllOrderStatusAsync(string name = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var unit = await _orderStatusRepository.GetAllPagedAsync(query =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                    query = query.Where(v => v.Name.Contains(name));

                query = query.Where(e => !e.Deleted);
                return query;

            }, pageIndex, pageSize);
            return unit;
        }
        public async Task InsertOrderStatusAsync(OrderStatus orderStatus)
        {
            await _orderStatusRepository.InsertAsync(orderStatus);
        }
        public async Task<OrderStatus> GetOrderStatusByIdAsync(int orderStatusId)
        {
            return await _orderStatusRepository.GetByIdAsync(orderStatusId, cache => default);
        }
        public async Task UpdateOrderStatusAsync(OrderStatus orderStatus)
        {
            await _orderStatusRepository.UpdateAsync(orderStatus);
        }
        public async Task DeleteOrderStatusAsync(OrderStatus orderStatus)
        {
            await _orderStatusRepository.DeleteAsync(orderStatus);
        }
        public async Task<bool> IsOrderStatesNameFoundAsync(string Name,int Id)
        {
            if(string.IsNullOrWhiteSpace(Name))
                return false;
            Name = Name.Trim();
            return await _orderStatusRepository.Table.AnyAsync(t => t.Name == Name && t.Id != Id);
        }
        #endregion
        #region Utilite
        #endregion

    }
}
