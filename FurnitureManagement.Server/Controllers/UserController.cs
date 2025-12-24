using FurnitureManagement.Server.Data;
using FurnitureManagement.Server.Models;
using FurnitureManagement.Server.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            // 查找用户
            var user = await _context.User.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return Ok(new LoginResponse { Success = false, Message = "用户名或密码错误" });
            }

            // 验证密码
            if (!PasswordHelper.VerifyPassword(request.Password, user.Password))
            {
                return Ok(new LoginResponse { Success = false, Message = "用户名或密码错误" });
            }

            // 检查用户状态
            if (user.Status != "active")
            {
                return Ok(new LoginResponse { Success = false, Message = "用户已被禁用" });
            }

            // 更新最后登录时间
            user.LastLogin = DateTime.Now;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new LoginResponse { Success = true, Message = "登录成功", User = user });
        }

        // POST: api/User/register
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
        {
            // 检查用户名是否已存在
            if (await _context.User.AnyAsync(u => u.Username == request.Username))
            {
                return Ok(new LoginResponse { Success = false, Message = "用户名已存在" });
            }

            // 检查邮箱是否已存在
            if (await _context.User.AnyAsync(u => u.Email == request.Email))
            {
                return Ok(new LoginResponse { Success = false, Message = "邮箱已存在" });
            }

            // 创建新用户
            var user = new User
            {
                Username = request.Username,
                Password = PasswordHelper.EncryptPassword(request.Password),
                RealName = request.Username, // 默认真实姓名为用户名
                Email = request.Email,
                Phone = request.Phone,
                Role = request.Role,
                Status = "active",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new LoginResponse { Success = true, Message = "注册成功", User = user });
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            // 检查用户名是否已被其他用户使用
            if (await _context.User.AnyAsync(u => u.Username == user.Username && u.UserId != id))
            {
                return Ok(new { Success = false, Message = "用户名已存在" });
            }

            // 检查邮箱是否已被其他用户使用
            if (await _context.User.AnyAsync(u => u.Email == user.Email && u.UserId != id))
            {
                return Ok(new { Success = false, Message = "邮箱已存在" });
            }

            user.UpdatedAt = DateTime.Now;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Success = true, Message = "用户更新成功" });
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "用户删除成功" });
        }

        // PATCH: api/User/5/activate
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Status = "active";
            user.UpdatedAt = DateTime.Now;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "用户已激活" });
        }

        // PATCH: api/User/5/deactivate
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Status = "inactive";
            user.UpdatedAt = DateTime.Now;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "用户已禁用" });
        }

        // POST: api/User/change-password
        [HttpPost("change-password")]
        public async Task<ActionResult<dynamic>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // 查找用户
            var user = await _context.User.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { Success = false, Message = "用户不存在" });
            }

            // 验证原密码
            if (!PasswordHelper.VerifyPassword(request.OldPassword, user.Password))
            {
                return Ok(new { Success = false, Message = "原密码错误" });
            }

            // 验证新密码不能与原密码相同
            if (request.OldPassword == request.NewPassword)
            {
                return Ok(new { Success = false, Message = "新密码不能与原密码相同" });
            }

            // 验证新密码长度
            if (request.NewPassword.Length < 6)
            {
                return Ok(new { Success = false, Message = "新密码长度不能少于6位" });
            }

            // 更新密码
            user.Password = PasswordHelper.EncryptPassword(request.NewPassword);
            user.UpdatedAt = DateTime.Now;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "密码修改成功" });
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}