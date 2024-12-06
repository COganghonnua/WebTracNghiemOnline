using Microsoft.EntityFrameworkCore;
using WebTracNghiemOnline.Access;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction> GetTransactionByIdAsync(string transactionId);
        Task UpdateTransactionStatusAsync(string transactionId, string status);
    }

    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> GetTransactionByIdAsync(string transactionId)
        {
            return await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }


        public async Task UpdateTransactionStatusAsync(string transactionId, string status)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == transactionId);
            if (transaction != null)
            {
                transaction.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }

}
