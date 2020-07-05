using System.Threading.Tasks;

namespace UserApp.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserAppDbContext _context;

        public UnitOfWork(UserAppDbContext context)
        {
            this._context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}