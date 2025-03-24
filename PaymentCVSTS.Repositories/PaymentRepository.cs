using Microsoft.EntityFrameworkCore;
using PaymentCVSTS.Repositories.Basic;
using PaymentCVSTS.Repositories.Models;

namespace PaymentCVSTS.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository() { }

        public async Task<List<Payment>> GetAll()
        {
            var payment = await _context.Payments
                .Include(v => v.Appointment)
                .ToListAsync();

            return payment;
        }

        public async Task<Payment> GetByIdAsync(int code)
        {
            var item = await _context.Payments
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(i => i.PaymentId == code);
            return item;
        }

        public async Task<List<Payment>> Search(DateOnly? date, string? status, string? method)
        {
            var query = _context.Payments
                .Include(p => p.Appointment) // Ensure Appointment is always included
                .AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(vr => vr.PaymentDate == date.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(vr => vr.PaymentStatus.Contains(status));
            }

            if (!string.IsNullOrEmpty(method))
            {
                query = query.Where(vr => vr.PaymentMethod.Contains(method));
            }

            return await query.ToListAsync();
        }
    }
}