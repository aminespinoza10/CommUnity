using AppVecinos.API.Models;
using AppVecinos.API.Repositories;
using System;

namespace AppVecinos.API.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Neighbor> NeighborRepository { get; }
        IGenericRepository<Fee> FeeRepository { get; }
        IGenericRepository<Payment> PaymentRepository { get; }
        IGenericRepository<Outcome> OutcomeRepository { get; }
        IGenericRepository<Balance> BalanceRepository { get; }
        Task SaveAsync();
    }
}