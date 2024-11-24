﻿using Core.Interfaces.Services;
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

    [Authorize(Roles = "admin")]
    [HttpGet("loan/request/{loanRequesId}/approve")]
    public async Task<IActionResult> ApproveLoan([FromRoute] int loanRequesId)
    {
        return Ok(await _bankService.ApproveLoan(loanRequesId));
    }

    [Authorize(Roles = "admin")]
    [HttpPost("loan/request/reject")]
    public async Task<IActionResult> RejectLoan([FromBody] LoanRejectRequest loanReject)
    {
        return Ok(await _bankService.RejectLoan(loanReject));
    }
}
