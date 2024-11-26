using BankManagment.Controllers;
using Core.Interfaces.Services;
using Core.Request;
using Microsoft.AspNetCore.Mvc;

namespace BankManagement.Controllers;

public class InstallmentsController: BaseApiController
{
    private readonly IInstallmentService _installmentService;

    public InstallmentsController(IInstallmentService installmentService)
    {
        _installmentService = installmentService;
    }

    [HttpGet("loan/{id}")]
    public async Task<IActionResult> GetInstallments([FromRoute] int id, [FromQuery] string? status)
    {
        return Ok(await _installmentService.GetInstallments(id, status));
    }

    [HttpPost("payment")]
    public async Task<IActionResult> InstallmentPayment([FromBody] InstallmentPaymentRequest installmentPayment)
    {
        return Ok(await _installmentService.PaidInstallments(installmentPayment));
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> InstallmentsOverdue()
    {
        return Ok(await _installmentService.GetInstallmentsOverdue());
    }
}
