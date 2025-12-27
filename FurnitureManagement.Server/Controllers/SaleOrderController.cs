using FurnitureManagement.Server.Data;
using FurnitureManagement.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SaleOrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SaleOrder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleOrder>>> GetSaleOrders(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null)
        {
            var query = _context.SaleOrder
                .Include(so => so.SaleDetails)
                .ThenInclude(sd => sd.Furniture)
                .Include(so => so.SaleDetails)
                .ThenInclude(sd => sd.Warehouse)
                .AsQueryable();

            // 日期范围筛选
            if (startDate.HasValue)
            {
                query = query.Where(so => so.SaleDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(so => so.SaleDate <= endDate.Value);
            }

            // 状态筛选
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(so => so.Status == status);
            }

            // 多字段模糊搜索
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(so => 
                    so.SaleId.ToString().Contains(search) ||
                    so.CustomerName.Contains(search) ||
                    so.CustomerPhone.Contains(search) ||
                    so.SaleDetails.Any(sd => 
                        sd.Furniture != null && 
                        (sd.Furniture.FurnitureName.Contains(search) ||
                        sd.Furniture.Model.Contains(search))));
            }

            return await query.ToListAsync();
        }

        // GET: api/SaleOrder/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleOrder>> GetSaleOrder(int id)
        {
            var saleOrder = await _context.SaleOrder
                .Include(so => so.SaleDetails)
                .ThenInclude(sd => sd.Furniture)
                .Include(so => so.SaleDetails)
                .ThenInclude(sd => sd.Warehouse)
                .FirstOrDefaultAsync(so => so.SaleId == id);

            if (saleOrder == null)
            {
                return NotFound();
            }

            return saleOrder;
        }

        // POST: api/SaleOrder
        [HttpPost]
        public async Task<ActionResult<SaleOrder>> CreateSaleOrder([FromBody] SaleOrder saleOrder)
        {
            // 计算总金额
            decimal totalAmount = saleOrder.SaleDetails.Sum(sd => sd.TotalPrice);
            saleOrder.TotalAmount = totalAmount;
            // 计算最终金额
            saleOrder.FinalAmount = totalAmount - saleOrder.Discount;

            _context.SaleOrder.Add(saleOrder);
            await _context.SaveChangesAsync();

            // 创建资金流水记录
            var capitalFlow = new CapitalFlow
            {
                FlowDate = saleOrder.SaleDate,
                FlowType = "income",
                Amount = saleOrder.FinalAmount,
                Description = $"销售订单 #{saleOrder.SaleId} 收入",
                ReferenceType = "sale",
                ReferenceId = saleOrder.SaleId,
                CreatedBy = saleOrder.CreatedBy ?? 1, // 默认使用创建订单的用户ID，如果没有则使用1
                CreatedAt = DateTime.Now
            };

            _context.CapitalFlow.Add(capitalFlow);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSaleOrder), new { id = saleOrder.SaleId }, saleOrder);
        }

        // PUT: api/SaleOrder/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSaleOrder(int id, [FromBody] SaleOrder saleOrder)
        {
            if (id != saleOrder.SaleId)
            {
                Console.WriteLine($"UpdateSaleOrder: ID mismatch - URL ID: {id}, SaleOrder.SaleId: {saleOrder.SaleId}");
                return BadRequest();
            }

            // 检查销售明细是否为空
            if (saleOrder.SaleDetails == null || saleOrder.SaleDetails.Count == 0)
            {
                Console.WriteLine($"UpdateSaleOrder: SaleDetails is empty or null");
                return BadRequest("销售明细不能为空");
            }

            // 检查销售明细中的必填字段
            for (int i = 0; i < saleOrder.SaleDetails.Count; i++)
            {
                var detail = saleOrder.SaleDetails[i];
                if (detail.FurnitureId <= 0)
                {
                    Console.WriteLine($"UpdateSaleOrder: SaleDetail[{i}] FurnitureId is invalid: {detail.FurnitureId}");
                    return BadRequest($"第 {i + 1} 行销售明细的家具ID无效");
                }
                if (detail.Quantity <= 0)
                {
                    Console.WriteLine($"UpdateSaleOrder: SaleDetail[{i}] Quantity is invalid: {detail.Quantity}");
                    return BadRequest($"第 {i + 1} 行销售明细的数量无效");
                }
                if (detail.UnitPrice < 0)
                {
                    Console.WriteLine($"UpdateSaleOrder: SaleDetail[{i}] UnitPrice is invalid: {detail.UnitPrice}");
                    return BadRequest($"第 {i + 1} 行销售明细的单价无效");
                }
            }

            // 计算总金额
            decimal totalAmount = saleOrder.SaleDetails.Sum(sd => sd.TotalPrice);
            saleOrder.TotalAmount = totalAmount;
            // 计算最终金额
            saleOrder.FinalAmount = totalAmount - saleOrder.Discount;

            Console.WriteLine($"UpdateSaleOrder: Updating order {id} with {saleOrder.SaleDetails.Count} details, TotalAmount: {totalAmount}, Discount: {saleOrder.Discount}, FinalAmount: {saleOrder.FinalAmount}");

            // 获取数据库中现有的销售订单及其明细
            var existingSaleOrder = await _context.SaleOrder
                .Include(so => so.SaleDetails)
                .FirstOrDefaultAsync(so => so.SaleId == id);

            if (existingSaleOrder == null)
            {
                return NotFound();
            }

            // 记录原状态，用于判断是否需要更新库存
            string originalStatus = existingSaleOrder.Status;
            string newStatus = saleOrder.Status;

            // 更新销售订单的基本信息
            existingSaleOrder.CustomerName = saleOrder.CustomerName;
            existingSaleOrder.CustomerPhone = saleOrder.CustomerPhone;
            existingSaleOrder.SaleDate = saleOrder.SaleDate;
            existingSaleOrder.TotalAmount = saleOrder.TotalAmount;
            existingSaleOrder.Discount = saleOrder.Discount;
            existingSaleOrder.FinalAmount = saleOrder.FinalAmount;
            existingSaleOrder.Status = saleOrder.Status;

            // 处理销售明细：新增、修改、删除
            // 1. 收集客户端传递的明细ID
            var clientDetailIds = saleOrder.SaleDetails.Select(d => d.DetailId).ToList();
            
            // 2. 收集数据库中现有明细ID
            var existingDetailIds = existingSaleOrder.SaleDetails.Select(d => d.DetailId).ToList();

            // 3. 处理删除操作：数据库中有但客户端没有的明细
            foreach (var detail in existingSaleOrder.SaleDetails.ToList())
            {
                if (!clientDetailIds.Contains(detail.DetailId))
                {
                    Console.WriteLine($"UpdateSaleOrder: Deleting detail {detail.DetailId}");
                    existingSaleOrder.SaleDetails.Remove(detail);
                    _context.SaleDetail.Remove(detail);
                }
            }

            // 4. 处理新增和修改操作
            foreach (var detail in saleOrder.SaleDetails)
            {
                // 设置销售订单ID
                detail.SaleId = id;
                
                if (detail.DetailId == 0)
                {
                    // 新增明细
                    Console.WriteLine($"UpdateSaleOrder: Adding new detail for furniture {detail.FurnitureId}");
                    existingSaleOrder.SaleDetails.Add(detail);
                    _context.SaleDetail.Add(detail);
                }
                else
                {
                    // 修改现有明细
                    Console.WriteLine($"UpdateSaleOrder: Updating detail {detail.DetailId}");
                    var existingDetail = existingSaleOrder.SaleDetails.FirstOrDefault(d => d.DetailId == detail.DetailId);
                    if (existingDetail != null)
                    {
                        existingDetail.FurnitureId = detail.FurnitureId;
                        existingDetail.Quantity = detail.Quantity;
                        existingDetail.UnitPrice = detail.UnitPrice;
                        existingDetail.TotalPrice = detail.TotalPrice;
                        existingDetail.WarehouseId = detail.WarehouseId;
                    }
                }
            }

            try
            {
                // 如果状态从非"已完成"变更为"已完成"，更新库存
                if (originalStatus != "completed" && newStatus == "completed")
                {
                    // 使用事务确保库存扣减和订单更新的原子性
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // 检查库存是否充足
                            foreach (var detail in existingSaleOrder.SaleDetails)
                            {
                                var inventory = await _context.Inventory
                                    .FirstOrDefaultAsync(i => i.FurnitureId == detail.FurnitureId && i.WarehouseId == detail.WarehouseId);

                                if (inventory == null || inventory.Quantity < detail.Quantity)
                                {
                                    await transaction.RollbackAsync();
                                    var furniture = await _context.Furniture.FindAsync(detail.FurnitureId);
                                    var warehouse = await _context.Warehouse.FindAsync(detail.WarehouseId);
                                    return BadRequest($"商品 {furniture?.FurnitureName} 在仓库 {warehouse?.WarehouseName} 的库存不足");
                                }
                            }

                            // 更新库存
                            foreach (var detail in existingSaleOrder.SaleDetails)
                            {
                                var inventory = await _context.Inventory
                                    .FirstOrDefaultAsync(i => i.FurnitureId == detail.FurnitureId && i.WarehouseId == detail.WarehouseId);

                                if (inventory != null)
                                {
                                    inventory.Quantity -= detail.Quantity;
                                    inventory.LastUpdated = DateTime.Now;
                                    _context.Entry(inventory).State = EntityState.Modified;
                                }
                            }

                            // 保存销售订单更新
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            Console.WriteLine($"UpdateSaleOrder: Successfully updated order {id} with inventory deduction");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine($"UpdateSaleOrder: Error during inventory update - {ex.Message}");
                            throw;
                        }
                    }
                }
                else
                {
                    // 普通更新，无需处理库存
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"UpdateSaleOrder: Successfully updated order {id} without inventory change");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateSaleOrder: Error saving changes - {ex.Message}");
                return StatusCode(500, "服务器内部错误");
            }

            return NoContent();
        }

        // PUT: api/SaleOrder/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateSaleOrderStatus(int id, [FromBody] string status)
        {
            var saleOrder = await _context.SaleOrder
                .Include(so => so.SaleDetails)
                .FirstOrDefaultAsync(so => so.SaleId == id);

            if (saleOrder == null)
            {
                return NotFound();
            }

            // 如果状态从非"已完成"变更为"已完成"，更新库存
            if (saleOrder.Status != "completed" && status == "completed")
            {
                // 使用事务确保库存扣减和订单状态更新的原子性
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 检查库存是否充足并更新库存
                        foreach (var detail in saleOrder.SaleDetails)
                        {
                            var inventory = await _context.Inventory
                                .FirstOrDefaultAsync(i => i.FurnitureId == detail.FurnitureId && i.WarehouseId == detail.WarehouseId);

                            if (inventory == null || inventory.Quantity < detail.Quantity)
                            {
                                await transaction.RollbackAsync();
                                var furniture = await _context.Furniture.FindAsync(detail.FurnitureId);
                                var warehouse = await _context.Warehouse.FindAsync(detail.WarehouseId);
                                return BadRequest($"商品 {furniture?.FurnitureName} 在仓库 {warehouse?.WarehouseName} 的库存不足");
                            }

                            // 更新库存
                            inventory.Quantity -= detail.Quantity;
                            inventory.LastUpdated = DateTime.Now;
                            _context.Entry(inventory).State = EntityState.Modified;
                        }

                        // 更新订单状态
                        saleOrder.Status = status;
                        _context.Entry(saleOrder).State = EntityState.Modified;

                        // 保存所有更改
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"UpdateSaleOrderStatus: Error during inventory update - {ex.Message}");
                        return StatusCode(500, "服务器内部错误");
                    }
                }
            }
            else
            {
                // 普通状态更新，无需处理库存
                saleOrder.Status = status;
                _context.Entry(saleOrder).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleOrderExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return NoContent();
        }

        // DELETE: api/SaleOrder/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleOrder(int id)
        {
            var saleOrder = await _context.SaleOrder.FindAsync(id);
            if (saleOrder == null)
            {
                return NotFound();
            }

            _context.SaleOrder.Remove(saleOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/SaleOrder/5/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<IEnumerable<SaleDetail>>> GetSaleDetails(int id)
        {
            return await _context.SaleDetail
                .Where(sd => sd.SaleId == id)
                .Include(sd => sd.Furniture)
                .Include(sd => sd.Warehouse)
                .ToListAsync();
        }

        // POST: api/SaleOrder/5/details
        [HttpPost("{id}/details")]
        public async Task<ActionResult<SaleDetail>> AddSaleDetail(int id, [FromBody] SaleDetail saleDetail)
        {
            var saleOrder = await _context.SaleOrder.FindAsync(id);
            if (saleOrder == null)
            {
                return NotFound();
            }

            saleDetail.SaleId = id;
            _context.SaleDetail.Add(saleDetail);
            await _context.SaveChangesAsync();

            // 更新销售订单金额
            await UpdateSaleOrderAmounts(id);

            return CreatedAtAction(nameof(GetSaleDetails), new { id = id }, saleDetail);
        }

        // PUT: api/SaleOrder/details/5
        [HttpPut("details/{detailId}")]
        public async Task<IActionResult> UpdateSaleDetail(int detailId, [FromBody] SaleDetail saleDetail)
        {
            if (detailId != saleDetail.DetailId)
            {
                return BadRequest();
            }

            _context.Entry(saleDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                // 更新销售订单金额
                await UpdateSaleOrderAmounts(saleDetail.SaleId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleDetailExists(detailId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/SaleOrder/details/5
        [HttpDelete("details/{detailId}")]
        public async Task<IActionResult> DeleteSaleDetail(int detailId)
        {
            var saleDetail = await _context.SaleDetail.FindAsync(detailId);
            if (saleDetail == null)
            {
                return NotFound();
            }

            var saleId = saleDetail.SaleId;
            _context.SaleDetail.Remove(saleDetail);
            await _context.SaveChangesAsync();

            // 更新销售订单金额
            await UpdateSaleOrderAmounts(saleId);

            return NoContent();
        }

        // 辅助方法：更新销售订单金额
        private async Task UpdateSaleOrderAmounts(int saleId)
        {
            var saleOrder = await _context.SaleOrder.FindAsync(saleId);
            if (saleOrder != null)
            {
                var details = await _context.SaleDetail.Where(sd => sd.SaleId == saleId).ToListAsync();
                decimal totalAmount = details.Sum(sd => sd.TotalPrice);
                saleOrder.TotalAmount = totalAmount;
                saleOrder.FinalAmount = totalAmount - saleOrder.Discount;
                _context.Entry(saleOrder).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        private bool SaleOrderExists(int id)
        {
            return _context.SaleOrder.Any(e => e.SaleId == id);
        }

        private bool SaleDetailExists(int id)
        {
            return _context.SaleDetail.Any(e => e.DetailId == id);
        }
    }
}