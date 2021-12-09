using BookList.DataAccess.Abstract;
using BookList.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookList.DataAccess.Concrete.EfCore
{
    public class EfCoreCartDal : EfCoreGenericRepository<Cart, BookContext>, ICartDal
    {
        public override void Update(Cart entity)
        {
            using (var context = new BookContext())
            {
                context.Carts.Update(entity);
                context.SaveChanges();
            }
        }

        public Cart GetByUserId(string userId)
        {
            using (var context = new BookContext())
            {
                return context
                            .Carts
                            .Include(i => i.CartItems)
                            .ThenInclude(i => i.Product)
                            .FirstOrDefault(i => i.UserId == userId);
            }
        }

        public void DeleteFromCart(int cartId, int productId)
        {
            //using (var context = new ShopContext())
            //{
            //    var cmd = @"delete from CartItem where CartId=@p0 And ProductId=@p1";
            //    context.Database.ExecuteSqlCommand(cmd, cartId, productId);
            //}
        }
    }
}
