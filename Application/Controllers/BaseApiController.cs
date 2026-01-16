using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Lib.Shared.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();
    
    protected string GetUserId() => User.FindFirst("sub")?.Value ?? string.Empty;
    
    protected string GetUserEmail() => User.FindFirst("email")?.Value ?? string.Empty;
    
    protected string GetUserRole() => User.FindFirst("custom:role")?.Value ?? string.Empty;
}
