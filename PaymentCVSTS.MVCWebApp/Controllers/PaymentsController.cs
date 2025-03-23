using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaymentCVSTS.Repositories.Models;
using PaymentCVSTS.Services;

namespace PaymentCVSTS.MVCWebApp.Controllers
{
    [Authorize(Policy = "AdminOnly")] // Chỉ cho phép Admin truy cập
    public class PaymentsController : Controller
    {
        private readonly IPayment _payment;
        private readonly AppointmentService _appointmentService;

        public PaymentsController(IPayment payment, AppointmentService appointmentService)
        {
            _payment = payment;
            _appointmentService = appointmentService;
        }

        // GET: Payments
        public async Task<IActionResult> Index(DateOnly? date, string? status, int? childId, string sortOrder, int page = 1)
        {
            // Set page size
            int pageSize = 7;

            // Set up sorting parameters
            ViewData["AmountSortParam"] = string.IsNullOrEmpty(sortOrder) ? "amount_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["CurrentSort"] = sortOrder ?? "";
            ViewData["CurrentFilter"] = new { date, status, childId };

            var payments = await _payment.GetAll();

            if (date.HasValue || !string.IsNullOrEmpty(status) || childId.HasValue)
            {
                payments = await _payment.Search(date, status, childId);
            }

            // Apply sorting
            payments = sortOrder switch
            {
                "amount_desc" => payments.OrderByDescending(p => p.Amount).ToList(),
                "date" => payments.OrderBy(p => p.PaymentDate).ToList(),
                "date_desc" => payments.OrderByDescending(p => p.PaymentDate).ToList(),
                _ => payments.OrderBy(p => p.Amount).ToList(), // Default sort by amount ascending
            };

            // Calculate pagination values
            int totalItems = payments.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Ensure page is within valid range
            page = Math.Max(1, Math.Min(page, totalPages));

            // Get the correct page of items
            var pagedPayments = payments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Set up pagination info for the view
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["HasPreviousPage"] = page > 1;
            ViewData["HasNextPage"] = page < totalPages;

            var appointments = await _appointmentService.GetAllAsync();
            if (appointments == null)
            {
                appointments = new List<Appointment>();
            }

            ViewData["Appointment"] = new SelectList(appointments, "AppointmentId", "TotalCost");

            return View(pagedPayments);
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var payment = await _payment.GetById(id);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public async Task<IActionResult> Create()
        {
            var appointment = await _appointmentService.GetAllAsync();
            ViewData["AppointmentId"] = new SelectList(appointment, "AppointmentId", "AppointmentId");
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment paymentDetail)
        {
            // Always validate all fields before ModelState.IsValid check
            // This approach ensures all validation errors show at once
            if (paymentDetail.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "The Amount field is required.");
            }

            if (string.IsNullOrEmpty(paymentDetail.PaymentStatus))
            {
                ModelState.AddModelError("PaymentStatus", "The PaymentStatus field is required.");
            }

            if (string.IsNullOrEmpty(paymentDetail.PaymentMethod))
            {
                ModelState.AddModelError("PaymentMethod", "The PaymentMethod field is required.");
            }

            if (paymentDetail.PaymentDate == default)
            {
                ModelState.AddModelError("PaymentDate", "The PaymentDate field is required.");
            }

            if (paymentDetail.AppointmentId <= 0)
            {
                ModelState.AddModelError("AppointmentId", "The AppointmentId field is required.");
            }

            // Then check ModelState.IsValid
            if (ModelState.IsValid)
            {
                await _payment.Create(paymentDetail);
                return RedirectToAction(nameof(Index));
            }

            var appointment = await _appointmentService.GetAllAsync();
            ViewData["AppointmentId"] = new SelectList(appointment, "AppointmentId", "AppointmentId", paymentDetail.AppointmentId);
            return View(paymentDetail);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var payment = await _payment.GetById(id);
            if (payment == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentService.GetAllAsync();
            ViewData["AppointmentId"] = new SelectList(appointment, "AppointmentId", "AppointmentId", payment.AppointmentId);

            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Payment payment)
        {
            // Always validate all fields before ModelState.IsValid check
            // This ensures all validation errors show at once
            if (payment.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "The Amount field is required.");
            }

            if (string.IsNullOrEmpty(payment.PaymentStatus))
            {
                ModelState.AddModelError("PaymentStatus", "The PaymentStatus field is required.");
            }

            if (string.IsNullOrEmpty(payment.PaymentMethod))
            {
                ModelState.AddModelError("PaymentMethod", "The PaymentMethod field is required.");
            }

            if (payment.PaymentDate == default)
            {
                ModelState.AddModelError("PaymentDate", "The PaymentDate field is required.");
            }

            if (payment.AppointmentId <= 0)
            {
                ModelState.AddModelError("AppointmentId", "The AppointmentId field is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _payment.Update(payment);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency exception
                    throw new Exception();
                }
            }

            var appointment = await _appointmentService.GetAllAsync();
            ViewData["AppointmentId"] = new SelectList(appointment, "AppointmentId", "AppointmentId", payment.AppointmentId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var payment = await _payment.GetById(id);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _payment.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}