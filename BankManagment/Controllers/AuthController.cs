using BankManagment.Controllers;
using Core.Interfaces.Services;
using Core.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankManagement.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly ILoanService _loanService;

    public AuthController(IAuthService authService, ILoanService loanService)
    {
        _authService = authService;
        _loanService = loanService;
    }

    [HttpPost("{Id}/token")]
    public async Task<IActionResult> GetToken([FromRoute] int Id)
    {
        return Ok(await _authService.CreateToken(Id));
    }

    [Authorize(Roles = "admin")]
    [HttpGet("loan-request/{id}/approve")]
    public async Task<IActionResult> ApproveLoan([FromRoute] int id)
    {
        return Ok(await _loanService.ApproveLoan(id));
    }

    [Authorize(Roles = "admin")]
    [HttpPost("loan-request/reject")]
    public async Task<IActionResult> RejectLoan([FromBody] LoanRejectRequest loanReject)
    {
        return Ok(await _loanService.RejectLoan(loanReject));
    }
}
