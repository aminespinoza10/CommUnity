using AppVecinos.API.Models;
using AppVecinos.API.Repositories;

namespace AppVecinos.API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private IGenericRepository<Neighbor>? _neighborRepository;
        private IGenericRepository<Fee>? _feeRepository;
        private IGenericRepository<Payment>? _paymentRepository;
        private IGenericRepository<Outcome>? _outcomeRepository;
        private IGenericRepository<Balance>? _balanceRepository;

        public UnitOfWork(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IGenericRepository<Neighbor> NeighborRepository => _neighborRepository ??= new GenericRepository<Neighbor>(_context);

        public IGenericRepository<Fee> FeeRepository => _feeRepository ??= new GenericRepository<Fee>(_context);

        public IGenericRepository<Payment> PaymentRepository => _paymentRepository ??= new GenericRepository<Payment>(_context);

        public IGenericRepository<Outcome> OutcomeRepository => _outcomeRepository ??= new GenericRepository<Outcome>(_context);

        public IGenericRepository<Balance> BalanceRepository => _balanceRepository ??= new GenericRepository<Balance>(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}