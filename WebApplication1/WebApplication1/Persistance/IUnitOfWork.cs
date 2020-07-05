using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Persistance
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
