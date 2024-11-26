using Core.Interfaces.Services;
using Core.Request;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost("{Id}/get-token")]
    public async Task<IActionResult> GetToken([FromRoute] int Id)
    {
        return Ok(await _bankService.GetToken(Id));
    }

    [HttpPost("loan/simulate")]
    public async Task<IActionResult> SimulateLoan([FromBody] LoanSimulateRequest request)
    {
        return Ok(await _simulateService.SimulateLoan(request));
    }

    [HttpPost("loan/request")]
    public async Task<IActionResult> LoanApplication([FromBody] LoanApplicationRequest loanApplication)
    {
        return Ok(await _bankService.LoanRequest(loanApplication));
    }

    [HttpGet("loan/{id}")]
    public async Task<IActionResult> LoanDetailed([FromRoute] int id)
    {
        return Ok(await _bankService.DetailedLoan(id));
    }

    [HttpGet("loan/{id}/installments")]
    public async Task<IActionResult> GetInstallments([FromRoute] int id, [FromQuery] string? status)
    {
        return Ok(await _bankService.GetInstallments(id, status));
    }

    [HttpPost("installment/payment")]
    public async Task<IActionResult> InstallmentPayment([FromBody] InstallmentPaymentRequest installmentPayment)
    {
        return Ok(await _bankService.PaidInstallments(installmentPayment));
    }

    [HttpGet("installments/overdue")]
    public async Task<IActionResult> InstallmentsOverdue()
    {
        return Ok(await _bankService.GetInstallmentsOverdue());
    }

    [Authorize(Roles = "admin")]
    [HttpGet("loan/request/{id}/approve")]
    public async Task<IActionResult> ApproveLoan([FromRoute] int id)
    {
        return Ok(await _bankService.ApproveLoan(id));
    }

    [Authorize(Roles = "admin")]
    [HttpPost("loan/request/reject")]
    public async Task<IActionResult> RejectLoan([FromBody] LoanRejectRequest loanReject)
    {
        return Ok(await _bankService.RejectLoan(loanReject));
    }
}
