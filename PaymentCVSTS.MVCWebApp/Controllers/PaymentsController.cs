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
        public async Task<IActionResult> Index(DateOnly? date, string? status, int? childId)
        {
            var payments = await _payment.GetAll();

            if (date.HasValue || !string.IsNullOrEmpty(status) || childId.HasValue)
            {
                payments = await _payment.Search(date, status, childId);
            }

            var appointments = await _appointmentService.GetAllAsync();
            if (appointments == null)
            {
                appointments = new List<Appointment>();
            }

            ViewData["Appointment"] = new SelectList(appointments, "AppointmentId", "TotalCost");

            return View(payments);
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
        public async Task<IActionResult> CreateAsync()
        {
            var appointment = await _appointmentService.GetAllAsync();
            ViewData["AppointmentID"] = new SelectList(appointment, "AppointmentId", "AppointmentId");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment paymentDetail)
        {

            if (ModelState.IsValid)
            {
                await _payment.Create(paymentDetail);
                return RedirectToAction(nameof(Index));


            }

            var payments = await _payment.GetAll();
            ViewData["PaymentId"] = new SelectList(payments, "PaymentId", "AppointmentId");

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
            var payments = await _payment.GetAll();
            var appointment = await _appointmentService.GetAllAsync();
            ViewData["PaymentId"] = new SelectList(payments, "PaymentId", "AppointmentId");
            ViewData["AppointmentId"] = new SelectList(appointment, "AppointmentId", "AppointmentId");

            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Payment payment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _payment.Update(payment);

                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new Exception();
                }
                return RedirectToAction(nameof(Index));
            }
            var payments = await _payment.GetAll();
            ViewData["PaymentId"] = new SelectList(payments, "PaymentId", "AppointmentId");
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
