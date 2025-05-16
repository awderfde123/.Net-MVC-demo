using demo;
using demo.Models;
using demo.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class OrderController : Controller
{
    private readonly DemoDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public OrderController(DemoDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrder([FromBody] List<CartItemDto> items)
    {
        if (items == null || !items.Any())
        {
            return BadRequest("購物車是空的。");
        } 

        var userId = _userManager.GetUserId(User);

        var order = new Order
        {
            UserId = userId!,
            CreatedAt = DateTime.UtcNow,
            Items = items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Qty,
                UnitPrice = i.Price
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "訂單已建立成功", orderId = order.Id });
    }
}
