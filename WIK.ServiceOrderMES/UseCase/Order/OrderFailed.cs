using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES.UseCase
{
    public interface IOrderFailed : Abstraction.IUseCase
    {
        new Task MainLogic(string delimiter, string sourceFile);
    }
    public class OrderFailed : IOrderFailed
    {
        private readonly Repository.IOrderCached _repositoryCached;
        private readonly Repository.MaintenanceTransaction _repositoryMaintenanceTxn;
        public OrderFailed(Repository.IOrderCached repositoryCached, Repository.MaintenanceTransaction repositoryMaintenanceTxn)
        {
            _repositoryCached = repositoryCached;
            _repositoryMaintenanceTxn = repositoryMaintenanceTxn;
        }
        public async Task MainLogic(string delimiter, string sourceFile)
        {
            List<Entity.OrderSaved> listOrderFailed = await _repositoryCached.GetOrderFailed("FAIL_ORDER_*");
            if (listOrderFailed.Count == 0) 
            {
                #if DEBUG
                    Console.WriteLine("There's no Order Failed");
                    return;
                #endif
            }
            foreach (var order in listOrderFailed)
            {
                if (order.Order.ProductionOrder != "")
                {
                    Entity.Order orderFromCached = await _repositoryCached.GetOrder(order.Order.ProductionOrder);
                    if (JsonSerializer.Serialize(orderFromCached) != JsonSerializer.Serialize(order.Order))
                    {
                        #if DEBUG
                        Console.WriteLine($"{order.Order.WorkCenter} - {order.Order.ProductionOrder} - {order.Order.Material} - {order.Qty} - {order.Order.StartTime} - {order.Order.EndTime} - {order.OrderStatus} - {order.OrderType}");
                        #endif
                        bool result = _repositoryMaintenanceTxn.SaveMfgOrder(
                                order.Order.ProductionOrder,
                                "",
                                "",
                                order.Order.Material,
                                "",
                                "",
                                "",
                                order.Qty,
                                null,
                                "",
                                Formatting.IsDate(order.Order.StartTime) == true ? order.Order.StartTime : "",
                                Formatting.IsDate(order.Order.EndTime) == true ? order.Order.EndTime : "",
                                "",
                                order.OrderStatus,
                                order.OrderType,
                                order.Order.WorkCenter);

                        if (!result)
                        {
                            // Save as a Fail Transaction in cached
                            // If Transaction Fail, it must be executed later
                            await _repositoryCached.SaveOrderFailed(order.Order.ProductionOrder, order, TimeSpan.FromDays(AppSettings.CachedExpiration));
                        }
                        else
                        {
                            // Save into Cached
                            await _repositoryCached.SaveOrder(order.Order.ProductionOrder, order.Order, TimeSpan.FromHours(AppSettings.CachedExpiration));
                            // Delete Fail Order from Cached
                            await _repositoryCached.DeleteOrderFail(order.Order.ProductionOrder);
                        }
                    }
                    else
                    {
                        #if DEBUG
                        Console.WriteLine($"{order.Order.ProductionOrder} already exists in cached");
                        #endif
                    }
                }
            }
        }
    }
}
