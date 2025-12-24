using System.Net.Http;
using System.Net.Http.Json;
using FurnitureManagement.Client.Models;

namespace FurnitureManagement.Client.Servcie
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5192/api/";

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(60); // 增加超时时间
            
            // 添加默认请求头
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "FurnitureManagement-Client/1.0");
        }

        // 登录
        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("User/login", request);
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"登录请求失败: {ex.Message}");
                return new LoginResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 注册
        public async Task<LoginResponse?> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("User/register", request);
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"注册请求失败: {ex.Message}");
                return new LoginResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 获取所有用户
        public async Task<List<User>?> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("User");
                return await response.Content.ReadFromJsonAsync<List<User>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取用户列表失败: {ex.Message}");
                return null;
            }
        }

        // 根据ID获取用户
        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"User/{id}");
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取用户失败: {ex.Message}");
                return null;
            }
        }

        // 更新用户
        public async Task<ApiResponse?> UpdateUserAsync(int id, User user)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"User/{id}", user);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "更新成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"更新失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新用户失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 删除用户
        public async Task<ApiResponse?> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"User/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "删除成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"删除失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除用户失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 激活用户
        public async Task<ApiResponse?> ActivateUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"User/{id}/activate", null);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "激活成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"激活失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"激活用户失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 禁用用户
        public async Task<ApiResponse?> DeactivateUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"User/{id}/deactivate", null);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "禁用成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"禁用失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"禁用用户失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 修改密码
        public async Task<ApiResponse?> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("User/change-password", request);
                return await response.Content.ReadFromJsonAsync<ApiResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"修改密码失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        #region Category 商品分类相关接口

        // 获取所有商品分类
        public async Task<List<Category>?> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Category");
                return await response.Content.ReadFromJsonAsync<List<Category>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取商品分类列表失败: {ex.Message}");
                return null;
            }
        }

        // 根据ID获取商品分类
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Category/{id}");
                return await response.Content.ReadFromJsonAsync<Category>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取商品分类失败: {ex.Message}");
                return null;
            }
        }

        // 创建商品分类
        public async Task<Category?> CreateCategoryAsync(Category category)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Category", category);
                return await response.Content.ReadFromJsonAsync<Category>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建商品分类失败: {ex.Message}");
                return null;
            }
        }

        // 更新商品分类
        public async Task<ApiResponse?> UpdateCategoryAsync(int id, Category category)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"Category/{id}", category);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "更新成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"更新失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新商品分类失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 删除商品分类
        public async Task<ApiResponse?> DeleteCategoryAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Category/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "删除成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"删除失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除商品分类失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        #endregion

        #region Furniture 商品相关接口

        // 获取所有商品
        public async Task<List<Furniture>?> GetFurnitureAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Furniture");
                return await response.Content.ReadFromJsonAsync<List<Furniture>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取商品列表失败: {ex.Message}");
                return null;
            }
        }

        // 搜索商品
        public async Task<List<Furniture>?> SearchFurnitureAsync(string? query = null, int? categoryId = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(query))
                    queryParams.Add($"query={Uri.EscapeDataString(query)}");
                if (categoryId.HasValue && categoryId.Value > 0)
                    queryParams.Add($"categoryId={categoryId.Value}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var response = await _httpClient.GetAsync($"Furniture/search{queryString}");
                return await response.Content.ReadFromJsonAsync<List<Furniture>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"搜索商品失败: {ex.Message}");
                return null;
            }
        }

        // 根据ID获取商品
        public async Task<Furniture?> GetFurnitureByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Furniture/{id}");
                return await response.Content.ReadFromJsonAsync<Furniture>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取商品失败: {ex.Message}");
                return null;
            }
        }

        // 创建商品
        public async Task<Furniture?> CreateFurnitureAsync(Furniture furniture)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Furniture", furniture);
                return await response.Content.ReadFromJsonAsync<Furniture>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建商品失败: {ex.Message}");
                return null;
            }
        }

        // 更新商品
        public async Task<ApiResponse?> UpdateFurnitureAsync(int id, Furniture furniture)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"Furniture/{id}", furniture);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "更新成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"更新失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新商品失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 删除商品
        public async Task<ApiResponse?> DeleteFurnitureAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Furniture/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "删除成功" };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // 尝试解析服务器返回的错误消息
                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
                        return errorResponse ?? new ApiResponse { Success = false, Message = "删除失败" };
                    }
                    catch
                    {
                        return new ApiResponse { Success = false, Message = "删除失败，请先删除相关数据" };
                    }
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"删除失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除商品失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        #endregion

        #region Purchase 采购相关接口

        // 获取所有采购订单
        public async Task<List<PurchaseOrder>?> GetPurchaseOrdersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Purchase");
                return await response.Content.ReadFromJsonAsync<List<PurchaseOrder>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取采购订单列表失败: {ex.Message}");
                return null;
            }
        }

        // 创建采购订单
        public async Task<PurchaseOrder?> CreatePurchaseOrderAsync(PurchaseOrder order)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Purchase", order);
                return await response.Content.ReadFromJsonAsync<PurchaseOrder>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建采购订单失败: {ex.Message}");
                return null;
            }
        }

        // 完成采购订单
        public async Task<ApiResponse?> CompletePurchaseOrderAsync(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"Purchase/{id}/complete", null);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "采购订单已完成" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = "完成采购订单失败" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"完成采购订单失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 取消采购订单
        public async Task<ApiResponse?> CancelPurchaseOrderAsync(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"Purchase/{id}/cancel", null);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "采购订单已取消" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = "取消采购订单失败" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"取消采购订单失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 按供应商查询采购订单
        public async Task<List<PurchaseOrder>?> GetPurchaseOrdersBySupplierAsync(int supplierId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Purchase/by-supplier/{supplierId}");
                return await response.Content.ReadFromJsonAsync<List<PurchaseOrder>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"按供应商查询采购订单失败: {ex.Message}");
                return null;
            }
        }

        // 按商品查询采购订单
        public async Task<List<PurchaseOrder>?> GetPurchaseOrdersByFurnitureAsync(int furnitureId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Purchase/by-furniture/{furnitureId}");
                return await response.Content.ReadFromJsonAsync<List<PurchaseOrder>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"按商品查询采购订单失败: {ex.Message}");
                return null;
            }
        }

        // 获取采购统计信息
        public async Task<object?> GetPurchaseStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Purchase/statistics");
                return await response.Content.ReadFromJsonAsync<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取采购统计信息失败: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Inventory 库存相关接口

        // 获取所有库存
        public async Task<List<Inventory>?> GetInventoryAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Inventory");
                return await response.Content.ReadFromJsonAsync<List<Inventory>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取库存列表失败: {ex.Message}");
                return null;
            }
        }

        // 根据ID获取库存
        public async Task<Inventory?> GetInventoryByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Inventory/{id}");
                return await response.Content.ReadFromJsonAsync<Inventory>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取库存失败: {ex.Message}");
                return null;
            }
        }

        // 根据商品ID获取库存
        public async Task<List<Inventory>?> GetInventoryByFurnitureIdAsync(int furnitureId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Inventory/by-furniture/{furnitureId}");
                return await response.Content.ReadFromJsonAsync<List<Inventory>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据商品ID获取库存失败: {ex.Message}");
                return null;
            }
        }

        // 根据仓库ID获取库存
        public async Task<List<Inventory>?> GetInventoryByWarehouseIdAsync(int warehouseId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Inventory/by-warehouse/{warehouseId}");
                return await response.Content.ReadFromJsonAsync<List<Inventory>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据仓库ID获取库存失败: {ex.Message}");
                return null;
            }
        }

        // 创建库存
        public async Task<Inventory?> CreateInventoryAsync(Inventory inventory)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Inventory", inventory);
                return await response.Content.ReadFromJsonAsync<Inventory>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建库存失败: {ex.Message}");
                return null;
            }
        }

        // 更新库存
        public async Task<ApiResponse?> UpdateInventoryAsync(int id, Inventory inventory)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"Inventory/{id}", inventory);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "更新成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"更新失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新库存失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 更新库存数量
        public async Task<ApiResponse?> UpdateInventoryQuantityAsync(int id, int quantity)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"Inventory/{id}/update-quantity", quantity);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "更新成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"更新失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新库存数量失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 删除库存
        public async Task<ApiResponse?> DeleteInventoryAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Inventory/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "删除成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"删除失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除库存失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        #endregion

        #region Supplier 供应商相关接口

        // 获取所有供应商
        public async Task<List<Supplier>?> GetSuppliersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Supplier");
                return await response.Content.ReadFromJsonAsync<List<Supplier>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取供应商列表失败: {ex.Message}");
                return null;
            }
        }

        // 搜索供应商
        public async Task<List<Supplier>?> SearchSuppliersAsync(string? query = null)
        {
            try
            {
                var queryString = !string.IsNullOrWhiteSpace(query) ? $"?query={Uri.EscapeDataString(query)}" : "";
                var response = await _httpClient.GetAsync($"Supplier/search{queryString}");
                return await response.Content.ReadFromJsonAsync<List<Supplier>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"搜索供应商失败: {ex.Message}");
                return null;
            }
        }

        // 根据ID获取供应商
        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Supplier/{id}");
                return await response.Content.ReadFromJsonAsync<Supplier>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取供应商失败: {ex.Message}");
                return null;
            }
        }

        // 创建供应商
        public async Task<Supplier?> CreateSupplierAsync(Supplier supplier)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Supplier", supplier);
                return await response.Content.ReadFromJsonAsync<Supplier>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建供应商失败: {ex.Message}");
                return null;
            }
        }

        // 更新供应商
        public async Task<ApiResponse?> UpdateSupplierAsync(int id, Supplier supplier)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"Supplier/{id}", supplier);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "更新成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"更新失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新供应商失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 删除供应商
        public async Task<ApiResponse?> DeleteSupplierAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Supplier/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "删除成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"删除失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除供应商失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        #endregion

        #region Warehouse 仓库相关接口

        // 获取所有仓库
        public async Task<List<Warehouse>?> GetWarehousesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Warehouse");
                return await response.Content.ReadFromJsonAsync<List<Warehouse>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取仓库列表失败: {ex.Message}");
                return null;
            }
        }

        // 搜索仓库
        public async Task<List<Warehouse>?> SearchWarehousesAsync(string? query = null)
        {
            try
            {
                var queryString = !string.IsNullOrWhiteSpace(query) ? $"?query={Uri.EscapeDataString(query)}" : "";
                var response = await _httpClient.GetAsync($"Warehouse/search{queryString}");
                return await response.Content.ReadFromJsonAsync<List<Warehouse>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"搜索仓库失败: {ex.Message}");
                return null;
            }
        }

        // 根据ID获取仓库
        public async Task<Warehouse?> GetWarehouseByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Warehouse/{id}");
                return await response.Content.ReadFromJsonAsync<Warehouse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取仓库失败: {ex.Message}");
                return null;
            }
        }

        // 创建仓库
        public async Task<Warehouse?> CreateWarehouseAsync(Warehouse warehouse)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Warehouse", warehouse);
                return await response.Content.ReadFromJsonAsync<Warehouse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建仓库失败: {ex.Message}");
                return null;
            }
        }

        // 更新仓库
        public async Task<ApiResponse?> UpdateWarehouseAsync(int id, Warehouse warehouse)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"Warehouse/{id}", warehouse);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "更新成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"更新失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新仓库失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 删除仓库
        public async Task<ApiResponse?> DeleteWarehouseAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Warehouse/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "删除成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"删除失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除仓库失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }
        
        #endregion

        #region SaleOrder 销售订单相关接口

        // 获取销售订单列表（支持筛选和搜索）
        public async Task<List<SaleOrder>?> GetSaleOrdersAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? status = null,
            string? search = null)
        {
            try
            {
                var queryParams = new List<string>();
                
                if (startDate.HasValue)
                {
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                }
                if (endDate.HasValue)
                {
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
                }
                if (!string.IsNullOrEmpty(status))
                {
                    queryParams.Add($"status={status}");
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryParams.Add($"search={Uri.EscapeDataString(search)}");
                }
                
                var queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : "";
                var response = await _httpClient.GetAsync($"SaleOrder{queryString}");
                return await response.Content.ReadFromJsonAsync<List<SaleOrder>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取销售订单列表失败: {ex.Message}");
                return null;
            }
        }

        // 根据ID获取销售订单
        public async Task<SaleOrder?> GetSaleOrderByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"SaleOrder/{id}");
                return await response.Content.ReadFromJsonAsync<SaleOrder>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取销售订单失败: {ex.Message}");
                return null;
            }
        }

        // 创建销售订单
        public async Task<SaleOrder?> CreateSaleOrderAsync(SaleOrder saleOrder)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("SaleOrder", saleOrder);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SaleOrder>();
                }
                else
                {
                    // 获取详细的错误信息
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"创建销售订单失败: {response.StatusCode}, 详细信息: {errorMessage}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建销售订单失败: {ex.Message}");
                return null;
            }
        }

        // 更新销售订单
        public async Task<ApiResponse?> UpdateSaleOrderAsync(int id, SaleOrder saleOrder)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"SaleOrder/{id}", saleOrder);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "更新成功" };
                }
                else
                {
                    // 获取详细的错误信息
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return new ApiResponse { Success = false, Message = $"更新失败: {response.StatusCode}, 详细信息: {errorMessage}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新销售订单失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 更新销售订单状态
        public async Task<ApiResponse?> UpdateSaleOrderStatusAsync(int id, string status)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"SaleOrder/{id}/status", status);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "状态更新成功" };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse { Success = false, Message = $"状态更新失败: {errorContent}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新销售订单状态失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 删除销售订单
        public async Task<ApiResponse?> DeleteSaleOrderAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"SaleOrder/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "删除成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"删除失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除销售订单失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 获取销售订单明细
        public async Task<List<SaleDetail>?> GetSaleDetailsAsync(int saleId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"SaleOrder/{saleId}/details");
                return await response.Content.ReadFromJsonAsync<List<SaleDetail>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取销售订单明细失败: {ex.Message}");
                return null;
            }
        }

        // 添加销售明细
        public async Task<SaleDetail?> AddSaleDetailAsync(int saleId, SaleDetail saleDetail)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"SaleOrder/{saleId}/details", saleDetail);
                return await response.Content.ReadFromJsonAsync<SaleDetail>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加销售明细失败: {ex.Message}");
                return null;
            }
        }

        // 更新销售明细
        public async Task<ApiResponse?> UpdateSaleDetailAsync(int detailId, SaleDetail saleDetail)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"SaleOrder/details/{detailId}", saleDetail);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "更新成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"更新失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新销售明细失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        // 删除销售明细
        public async Task<ApiResponse?> DeleteSaleDetailAsync(int detailId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"SaleOrder/details/{detailId}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "删除成功" };
                }
                else
                {
                    return new ApiResponse { Success = false, Message = $"删除失败: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除销售明细失败: {ex.Message}");
                return new ApiResponse { Success = false, Message = "网络请求失败" };
            }
        }

        #endregion
    }
}