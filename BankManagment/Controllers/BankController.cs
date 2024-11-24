using Core.Interfaces.Services;
using Core.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankManagment.Controllers;

public class BankController : BaseApiController
{
    private readonly ISimulateService _simulateService;
    private readonly IBankService _bankService;
    private readonly IAuthService _authService;

    public BankController(ISimulateService simulateService,  IBankService bankService, IAuthService authService)
    {
        _simulateService = simulateService;
        _bankService = bankService;
        _authService = authService;
    }
    [HttpPost("{Id}/get-token")]
    public async Task<IActionResult> GetToken([FromRoute] int Id)
    {
        var CustomerDto = await _bankService.GetToken(Id);
        return Ok(_authService.CreateToken(CustomerDto));
    }

    [HttpPost("simulate")]
    public async Task<IActionResult> SimulateLoan([FromBody] LoanSimulateRequest request)
    {
        return Ok(await _simulateService.SimulateLoan(request));
    }

    [HttpPost("loan-request")]
    public async Task<IActionResult> LoanApplication(LoanApplicationRequest loanApplication)
    {
        return Ok(await _bankService.LoanRequest(loanApplication));
    }

    [Authorize(Roles = "admin")]
    [HttpGet("{loanRequesId}/approve")]
    public async Task<IActionResult> ApproveLoan([FromRoute] int loanRequesId)
    {
        return Ok(await _bankService.ApproveLoan(loanRequesId));
    }
}
