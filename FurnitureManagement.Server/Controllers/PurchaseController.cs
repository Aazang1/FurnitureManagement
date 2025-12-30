using FurnitureManagement.Server.Data;
using FurnitureManagement.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // 需要添加这个

namespace FurnitureManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Purchase
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrders()
        {
            return await _context.PurchaseOrder
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseDetails)
                    .ThenInclude(pd => pd.Furniture)
                .Include(po => po.PurchaseDetails)
                    .ThenInclude(pd => pd.Warehouse)
                .OrderByDescending(po => po.PurchaseDate)
                .ToListAsync();
        }

        // GET: api/Purchase/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrder(int id)
        {
            var purchaseOrder = await _context.PurchaseOrder
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseDetails)
                    .ThenInclude(pd => pd.Furniture)
                .Include(po => po.PurchaseDetails)
                    .ThenInclude(pd => pd.Warehouse)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return purchaseOrder;
        }

        // POST: api/Purchase
        [HttpPost]
        public async Task<ActionResult<PurchaseOrder>> CreatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. 暂时硬编码默认用户ID为1
                int userId = 1;

                // 2. 验证用户ID=1是否存在
                var userExists = await _context.User.AnyAsync(u => u.UserId == userId);
                if (!userExists)
                {
                    // 如果用户1不存在，尝试查找第一个用户
                    var firstUser = await _context.User.FirstOrDefaultAsync();
                    if (firstUser != null)
                    {
                        userId = firstUser.UserId;
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "系统中没有用户，请先创建用户"
                        });
                    }
                }

                // 3. 设置创建人和创建时间
                purchaseOrder.CreatedBy = userId;
                purchaseOrder.CreatedAt = DateTime.Now;

                // 4. 验证供应商是否存在
                var supplierExists = await _context.Supplier.AnyAsync(s => s.SupplierId == purchaseOrder.SupplierId);
                if (!supplierExists)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "供应商不存在"
                    });
                }

                // 5. 计算采购总金额
                if (purchaseOrder.PurchaseDetails != null && purchaseOrder.PurchaseDetails.Count > 0)
                {
                    purchaseOrder.TotalAmount = purchaseOrder.PurchaseDetails.Sum(pd => pd.Amount);

                    // 验证采购明细
                    foreach (var detail in purchaseOrder.PurchaseDetails)
                    {
                        // 验证商品是否存在
                        var furnitureExists = await _context.Furniture.AnyAsync(f => f.FurnitureId == detail.FurnitureId);
                        if (!furnitureExists)
                        {
                            return BadRequest(new
                            {
                                success = false,
                                message = $"商品ID {detail.FurnitureId} 不存在"
                            });
                        }

                        // 验证仓库是否存在
                        var warehouseExists = await _context.Warehouse.AnyAsync(w => w.WarehouseId == detail.WarehouseId);
                        if (!warehouseExists)
                        {
                            return BadRequest(new
                            {
                                success = false,
                                message = $"仓库ID {detail.WarehouseId} 不存在"
                            });
                        }

                        // 确保计算明细金额
                        if (detail.Amount == 0)
                        {
                            detail.CalculateAmount();
                        }
                    }
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "采购明细不能为空"
                    });
                }

                // 6. 设置默认状态
                if (string.IsNullOrEmpty(purchaseOrder.Status))
                {
                    purchaseOrder.Status = "pending";
                }

                // 7. 保存采购订单
                _context.PurchaseOrder.Add(purchaseOrder);
                await _context.SaveChangesAsync();

                // 8. 创建资金流水记录
                var capitalFlow = new CapitalFlow
                {
                    FlowDate = purchaseOrder.CreatedAt,
                    FlowType = "expense", // 采购是支出
                    Amount = purchaseOrder.TotalAmount, // 使用正数表示支出金额，汇总时自动处理
                    Description = $"采购订单 #{purchaseOrder.PurchaseOrderId} 支出",
                    ReferenceType = "purchase",
                    ReferenceId = purchaseOrder.PurchaseOrderId,
                    CreatedBy = purchaseOrder.CreatedBy,
                    CreatedAt = DateTime.Now
                };

                _context.CapitalFlow.Add(capitalFlow);
                await _context.SaveChangesAsync();

                // 提交事务
                await transaction.CommitAsync();

                // 9. 返回成功响应
                return Ok(new
                {
                    success = true,
                    message = "采购订单创建成功",
                    data = new
                    {
                        purchaseOrder = purchaseOrder,
                        capitalFlow = capitalFlow
                    }
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException is MySqlConnector.MySqlException mysqlEx)
            {
                await transaction.RollbackAsync();

                if (mysqlEx.Message.Contains("foreign key constraint"))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "外键约束失败，请检查相关数据是否存在"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "数据库操作失败: " + mysqlEx.Message
                    });
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new
                {
                    success = false,
                    message = "创建采购订单失败: " + ex.Message
                });
            }
        }

        // PUT: api/Purchase/5/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompletePurchaseOrder(int id)
        {
            var purchaseOrder = await _context.PurchaseOrder
                .Include(po => po.PurchaseDetails)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            // 将订单状态改为completed
            purchaseOrder.Status = "completed";
            _context.Entry(purchaseOrder).State = EntityState.Modified;

            // 更新库存
            if (purchaseOrder.PurchaseDetails != null)
            {
                foreach (var detail in purchaseOrder.PurchaseDetails)
                {
                    // 查找现有的库存记录
                    var inventory = await _context.Inventory
                        .FirstOrDefaultAsync(i => i.FurnitureId == detail.FurnitureId && i.WarehouseId == detail.WarehouseId);

                    if (inventory != null)
                    {
                        // 更新现有库存数量
                        inventory.Quantity += detail.Quantity;
                        inventory.LastUpdated = DateTime.Now;
                        _context.Entry(inventory).State = EntityState.Modified;
                    }
                    else
                    {
                        // 创建新的库存记录
                        var newInventory = new Inventory
                        {
                            FurnitureId = detail.FurnitureId,
                            WarehouseId = detail.WarehouseId,
                            Quantity = detail.Quantity,
                            LastUpdated = DateTime.Now
                        };
                        _context.Inventory.Add(newInventory);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Purchase/5/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelPurchaseOrder(int id)
        {
            var purchaseOrder = await _context.PurchaseOrder.FindAsync(id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            // 将订单状态改为cancelled
            purchaseOrder.Status = "cancelled";
            _context.Entry(purchaseOrder).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Purchase/statistics
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetPurchaseStatistics()
        {
            var totalPurchaseAmount = await _context.PurchaseOrder
                .Where(po => po.Status == "completed")
                .SumAsync(po => po.TotalAmount);

            var totalPurchaseCount = await _context.PurchaseOrder
                .Where(po => po.Status == "completed")
                .CountAsync();

            var totalItemsPurchased = await _context.PurchaseDetail
                .Where(pd => pd.PurchaseOrder.Status == "completed")
                .SumAsync(pd => pd.Quantity);

            return new
            {
                TotalPurchaseAmount = totalPurchaseAmount,
                TotalPurchaseCount = totalPurchaseCount,
                TotalItemsPurchased = totalItemsPurchased
            };
        }

        // GET: api/Purchase/by-supplier/{supplierId}
        [HttpGet("by-supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrdersBySupplier(int supplierId)
        {
            return await _context.PurchaseOrder
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseDetails)
                    .ThenInclude(pd => pd.Furniture)
                .Where(po => po.SupplierId == supplierId)
                .OrderByDescending(po => po.PurchaseDate)
                .ToListAsync();
        }

        // GET: api/Purchase/by-furniture/{furnitureId}
        [HttpGet("by-furniture/{furnitureId}")]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrdersByFurniture(int furnitureId)
        {
            return await _context.PurchaseOrder
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseDetails)
                    .ThenInclude(pd => pd.Furniture)
                .Where(po => po.PurchaseDetails.Any(pd => pd.FurnitureId == furnitureId))
                .OrderByDescending(po => po.PurchaseDate)
                .ToListAsync();
        }
    }
}