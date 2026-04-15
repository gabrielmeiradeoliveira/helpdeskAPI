using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Helpdesk.Domain.Dto;

[ApiController]
[Route("api/ticket")]
public class TicketController : ControllerBase
{
    private readonly TicketService _ticketService;
    private readonly ILogger<TicketController> _logger;

    public TicketController(
        TicketService ticketService,
        ILogger<TicketController> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    private string GetEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value
            ?? throw new UnauthorizedAccessException();
    }

    private string GetRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value
            ?? throw new UnauthorizedAccessException();
    }

    private bool IsAdminOrEditor()
    {
        var role = GetRole();

        return role == UserRoleEnum.Admin.ToString() ||
               role == UserRoleEnum.Editor.ToString();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
    {
        var email = GetEmail();

        _logger.LogInformation("User {Email} creating ticket", email);

        await _ticketService.CreateTicket(dto, email);

        return StatusCode(201);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTicketDto dto)
    {
        if (!IsAdminOrEditor())
        {
            _logger.LogWarning("Unauthorized update attempt on ticket {Id}", id);
            return Forbid();
        }

        _logger.LogInformation("Updating ticket {Id}", id);

        await _ticketService.UpdateTicket(id, dto);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (GetRole() != UserRoleEnum.Admin.ToString())
        {
            _logger.LogWarning("Unauthorized delete attempt on ticket {Id}", id);
            return Forbid();
        }

        _logger.LogWarning("Deleting ticket {Id}", id);

        await _ticketService.DeleteTicket(id);

        return NoContent();
    }

    [Authorize]
    [HttpPut("{id}/close")]
    public async Task<IActionResult> Close(Guid id)
    {
        if (!IsAdminOrEditor())
        {
            _logger.LogWarning("Unauthorized close attempt on ticket {Id}", id);
            return Forbid();
        }

        _logger.LogInformation("Closing ticket {Id}", id);

        await _ticketService.CloseTicket(id);

        return NoContent();
    }

    [Authorize]
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        if (!IsAdminOrEditor())
        {
            _logger.LogWarning("Unauthorized cancel attempt on ticket {Id}", id);
            return Forbid();
        }

        _logger.LogInformation("Canceling ticket {Id}", id);

        await _ticketService.CancelTicket(id);

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> MyTickets()
    {
        var email = GetEmail();

        _logger.LogInformation("Fetching tickets for user {Email}", email);

        var tickets = await _ticketService.GetMyTickets(email);

        return Ok(tickets);
    }
}