using demo;
using demo.Models;
using demo.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    public ActionResult<Order> Index()
    {
        var userName = _userManager.GetUserId(User);
        var orders = _context.Orders.Include(o => o.Items).Where(o => o.UserName == userName).Select(o => new Order
        {
            Id = o.Id,
            UserName = o.UserName,
            CreatedAt = o.CreatedAt,
            Items = o.Items,
            TotalAmount = o.Items.Sum(i => i.UnitPrice * i.Quantity)
        }).ToList();


        return View(orders);
    }

    public ActionResult<Order> Detail(int id)
    {
        var order = _context.Orders.Include(o => o.Items).Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefault(o => o.Id == id);
        if (order == null) return NotFound();

        order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        return View(order);
    }

    [HttpPost]
    public async Task<ActionResult> SubmitOrder([FromBody] List<CartItemDto> items)
    {
        if (items == null || !items.Any())
        {
            return BadRequest("購物車是空的。");
        } 

        var userName = _userManager.GetUserId(User);

        var order = new Order
        {
            UserName = userName!,
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
