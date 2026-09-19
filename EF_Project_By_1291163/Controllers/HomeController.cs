using EF_Project_By_1291163.Models;
using EventManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EF_Project_By_1291163.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string aggregateType)
        {
            var allEvents = await _context.Events.ToListAsync();
            var allRegistrations = await _context.Registrations.ToListAsync();

            ViewBag.TotalEvents = allEvents.Count;

            ViewBag.GrandTotalRevenue = allRegistrations
                .Sum(r => r.TotalPaid);

            ViewBag.AvgTicketPrice = allEvents.Any()
                ? allEvents.Average(e => e.TicketPrice)
                : 0;

            ViewBag.MaxPrice = allEvents.Any()
                ? allEvents.Max(e => e.TicketPrice)
                : 0;

            ViewBag.MinPrice = allEvents.Any()
                ? allEvents.Min(e => e.TicketPrice)
                : 0;

            var eventList = allEvents
                .Select(e => new HomeEventsVM
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    TicketPrice = e.TicketPrice,
                    TotalAttendees = allRegistrations
                        .Count(r => r.EventId == e.EventId),
                    TotalRevenue = allRegistrations
                        .Where(r => r.EventId == e.EventId)
                        .Sum(r => (decimal?)r.TotalPaid) ?? 0
                })
                .ToList();

            if (allEvents.Any() && !string.IsNullOrEmpty(aggregateType))
            {
                switch (aggregateType)
                {
                    case "Max":
                        {
                            var maxPrice = allEvents.Max(e => e.TicketPrice);

                            eventList = eventList
                                .Where(e => e.TicketPrice == maxPrice)
                                .ToList();

                            break;
                        }

                    case "Min":
                        {
                            var minPrice = allEvents.Min(e => e.TicketPrice);

                            eventList = eventList
                                .Where(e => e.TicketPrice == minPrice)
                                .ToList();

                            break;
                        }

                    case "Avg":
                        {
                            var avgPrice = allEvents.Average(e => e.TicketPrice);

                            eventList = eventList
                                .Where(e => e.TicketPrice >= avgPrice)
                                .ToList();

                            break;
                        }
                }
            }

            ViewBag.CurrentFilter = aggregateType;

            return View(eventList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        public IActionResult GetEventSummary(int eventId)
        {
            return ViewComponent("EventSummary", new { eventId = eventId });
        }
    }
}