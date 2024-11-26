using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface ISimulateRepository
{
    Task<TermInterestRate> GetMonthsByMonths(int months);
}
