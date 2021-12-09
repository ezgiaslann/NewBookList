using BookList.DataAccess.Abstract;
using BookList.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookList.DataAccess.Concrete.EfCore
{
    public class EfCoreOrderDal: EfCoreGenericRepository<Order, BookContext>, IOrderDal
    {
    }
}
