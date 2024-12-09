using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
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
            var order = await _orderStatusRepository.GetAllPagedAsync(query =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                    query = query.Where(v => v.Name.Contains(name));

                query = query.Where(e => !e.Deleted);
                return query;

            }, pageIndex, pageSize);
            return order;
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
        public async Task<IQueryable<OrderStatus>> SearchOrderStatesByNameAsync(string orderStatusName)
        {
            var query = _orderStatusRepository.Table;
            query = query.Where(c => !c.Deleted && c.IsActive);

            if (!string.IsNullOrEmpty(orderStatusName))
                query = query.Where(o => o.Name.Contains(orderStatusName));

            return query;
        }
        public virtual async Task<string> GetOrderStatusNameAsync(int statusId)
        {
            return await _orderStatusRepository.Table
                           .Where(x => x.Id == statusId)
                           .Select(x => x.Name)
                           .FirstOrDefaultAsync() ?? string.Empty;
        }
        #endregion
        #region Utilite
        #endregion

    }
}
