using Core.Interfaces.Services;
using Core.Request;
using Microsoft.AspNetCore.Mvc;

namespace BankManagment.Controllers;

public class BankController : BaseApiController
{
    private readonly ISimulateService _simulateService;

    public BankController(ISimulateService simulateService)
    {
        _simulateService = simulateService;
    }

    [HttpPost("simulate-loan")]
    public async Task<IActionResult> SimulateLoan([FromBody] LoanRequest request)
    {
        return Ok(await _simulateService.SimulateLoan(request));
    }
}
