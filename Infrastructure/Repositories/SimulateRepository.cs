﻿using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SimulateRepository : ISimulateRepository
{

    private readonly AppDbContext _context;

    public SimulateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TermInterestRate> GetMonthsByMonths(int Months)
    {
        return await _context.TermInterestRates.FirstOrDefaultAsync(x => x.Months == Months) ??
            throw new Exception("Seleccione un mes valido.");
    }
}