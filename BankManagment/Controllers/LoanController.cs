using Core.Interfaces.Services;
using Core.Request;
using Microsoft.AspNetCore.Mvc;

namespace BankManagment.Controllers;

public class LoanController : BaseApiController
{
    private readonly ISimulateService _simulateService;
    private readonly ILoanService _loanService;

    public LoanController(ISimulateService simulateService, ILoanService bankService)
    {
        _simulateService = simulateService;
        _loanService = bankService;
    }

    [HttpPost("simulate")]
    public async Task<IActionResult> SimulateLoan([FromBody] LoanSimulateRequest request)
    {
        return Ok(await _simulateService.SimulateLoan(request));
    }

    [HttpPost("request")]
    public async Task<IActionResult> LoanApplication([FromBody] LoanApplicationRequest loanApplication)
    {
        return Ok(await _loanService.LoanRequest(loanApplication));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> LoanDetailed([FromRoute] int id)
    {
        return Ok(await _loanService.DetailedLoan(id));
    }
}
