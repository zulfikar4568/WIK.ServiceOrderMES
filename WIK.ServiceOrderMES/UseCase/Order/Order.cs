using Camstar.WCF.ObjectStack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES.UseCase
{
    public interface IOrder : Abstraction.IUseCase
    {
        new Task MainLogic(string delimiter, string sourceFile);
    }

    public class OrderStatusInfo
    {
        public int Counter { get; set; }
        public string Status { get; set; }
    }
    public class Order : IOrder
    {
        private readonly Repository.IOrderCsv _repositoryCsv;
        private readonly Repository.IOrderCached _repositoryCached;
        private readonly Repository.MaintenanceTransaction _repositoryMaintenanceTxn;
        public Order(Repository.IOrderCsv repositoryCsv, Repository.IOrderCached repositoryCached, Repository.MaintenanceTransaction repositoryMaintenanceTxn)
        {
            _repositoryCsv = repositoryCsv;
            _repositoryCached = repositoryCached;
            _repositoryMaintenanceTxn = repositoryMaintenanceTxn;
        }
        public async Task MainLogic(string delimiter, string sourceFile)
        {
            List<Entity.Order> orders = _repositoryCsv.Reading(delimiter, sourceFile);
            foreach (var order in orders)
            {
                order.ProductionOrder = order.ProductionOrder.TrimStart('0');
                order.Material = order.Material.TrimStart('0');

                if (ConvertToDoubleCommaDecimal(order.TargetQty, out double number))
                {
                    string OrderType = FilterOrdertype(order.OrderType);

                    if (!AppSettings.FilterMfgLines.Contains(order.WorkCenter))
                    {
                        EventLogUtil.LogEvent($"{order.WorkCenter} MfgLine or WorkCenter {order.WorkCenter} skipped, cause there's not on the filter list!", System.Diagnostics.EventLogEntryType.Warning, 3);
                        continue;
                    }

                    OrderStatusInfo orderStatusInfo = FilterOrderStatus(order.SystemStatus);

                    // If theres more System Status or null, will skipped, the value counter must be 1
                    if (orderStatusInfo.Counter != 1)
                    {
                        EventLogUtil.LogEvent($"{order.SystemStatus} SystemStatus skipped, cause there's no on the filter list or container more than on the list!", System.Diagnostics.EventLogEntryType.Warning, 3);
                        continue;
                    }

                    // Save Order Status
                    if (orderStatusInfo.Status != "")
                    {
                        if (await _repositoryCached.GetString(orderStatusInfo.Status) is null)
                        {
                            if (_repositoryMaintenanceTxn.SaveOrderStatus(orderStatusInfo.Status, isOrderStateEnum.Open)) await _repositoryCached.SaveString(orderStatusInfo.Status, TimeSpan.FromHours(AppSettings.CachedExpiration));
                        }
                    }

                    // Save Order type
                    if (OrderType != "")
                    {
                        if (await _repositoryCached.GetString(OrderType) is null)
                        {
                            if (_repositoryMaintenanceTxn.SaveOrderType(OrderType)) await _repositoryCached.SaveString(OrderType, TimeSpan.FromHours(AppSettings.CachedExpiration));
                        }
                    }

                    // Save Work Center
                    if (order.WorkCenter != "")
                    {
                        if (await _repositoryCached.GetString(order.WorkCenter) is null)
                        {
                            if (_repositoryMaintenanceTxn.SaveMfgLine(order.WorkCenter)) await _repositoryCached.SaveString(order.WorkCenter, TimeSpan.FromHours(AppSettings.CachedExpiration));
                        }
                    } else
                    {
                        continue;
                    }

                    if (order.Material != "")
                    {
                        if (await _repositoryCached.GetString(order.Material) is null)
                        {
                            if (_repositoryMaintenanceTxn.ProductExists(order.Material))
                            {
                                await _repositoryCached.SaveString(order.Material, TimeSpan.FromHours(AppSettings.CachedExpiration));
                            } else
                            {
                                EventLogUtil.LogEvent($"Material {order.Material} doesn't exists!", System.Diagnostics.EventLogEntryType.Warning, 3);
                                continue;
                            }
                        }
                    } else
                    {
                        continue;
                    }

                    if (order.ProductionOrder != "")
                    {
                        Entity.Order orderFromCached = await _repositoryCached.GetOrder(order.ProductionOrder);
                        if (JsonSerializer.Serialize(orderFromCached) != JsonSerializer.Serialize(order))
                        {
                            #if DEBUG
                                Console.WriteLine($"{order.WorkCenter} - {order.ProductionOrder} - {order.Material} - {number} - {order.StartTime} - {order.EndTime} - {orderStatusInfo.Status} - {OrderType}");
                            #endif
                            bool result = _repositoryMaintenanceTxn.SaveMfgOrder(
                                    order.ProductionOrder,
                                    "",
                                    "",
                                    order.Material,
                                    "",
                                    "",
                                    "",
                                    number,
                                    null,
                                    "",
                                    Formatting.IsDate(order.StartTime) == true ? order.StartTime : "",
                                    Formatting.IsDate(order.EndTime) == true ? order.EndTime : "",
                                    "",
                                    orderStatusInfo.Status,
                                    OrderType,
                                    order.WorkCenter);

                            if (!result)
                            {
                                // Save as a Fail Transaction in cached
                                // If Transaction Fail, it must be executed later
                                await _repositoryCached.SaveOrder($"FAIL_ORDER{order.ProductionOrder}", order, TimeSpan.FromHours(AppSettings.CachedExpiration));
                            } else
                            {
                                // Save into Cached
                                await _repositoryCached.SaveOrder(order.ProductionOrder, order, TimeSpan.FromHours(AppSettings.CachedExpiration));
                            }
                        } else
                        {
                            #if DEBUG
                                Console.WriteLine($"{order.ProductionOrder} already exists in cached");
                            #endif
                        }
                    }
                }
            }
            EventLogUtil.LogEvent("Finish Looping All the rows!", System.Diagnostics.EventLogEntryType.Information, 3);
        }
        private OrderStatusInfo FilterOrderStatus(string orderStatus)
        {
            int counter = 0;
            string orderStatusChecked = "";
            string[] orderStatusrray = orderStatus.Split(' ');
            if (orderStatusrray.Length > 0)
            {
                foreach (var item in orderStatusrray)
                {
                    if (AppSettings.FilterOrderStatus.Contains(item))
                    {
                        orderStatusChecked = item;
                        counter++;
                    }
                }
            }
            return new OrderStatusInfo() { Counter = counter, Status = orderStatusChecked };
        }
        private string FilterOrdertype(string orderType)
        {
            string orderTypeChecked = "";
            if (AppSettings.FilterOrderTypes.Length > 0)
            {
                foreach (var filterOrderType in AppSettings.FilterOrderTypes)
                {
                    if (orderType.Contains(filterOrderType))
                    {
                        orderTypeChecked = orderType;
                        break;
                    }
                }
            }
            return orderTypeChecked;
        }
        private bool ConvertToDoubleCommaDecimal(string value, out double result)
        {
            CultureInfo provider = new CultureInfo("en-US");
            NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            bool status = double.TryParse(value, styles, provider, out result);
            return status;
        }
    }
}
