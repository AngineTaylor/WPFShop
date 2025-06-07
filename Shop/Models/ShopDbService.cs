using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Services
{
    public class ShopDbService
    {
        private readonly AppDbContext _context;

        public ShopDbService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}