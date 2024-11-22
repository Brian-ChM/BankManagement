using Core.Interfaces.Services;
using Core.Request;
using Microsoft.AspNetCore.Mvc;

namespace BankManagment.Controllers;

public class BankController : BaseApiController
{
    private readonly ISimulateService _simulateService;
    private readonly IBankService _bankService;

    public BankController(ISimulateService simulateService, IBankService bankService)
    {
        _simulateService = simulateService;
        _bankService = bankService;
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
}
