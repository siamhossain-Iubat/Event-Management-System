using EventManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.ViewComponents
{
    public class EventSummaryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public EventSummaryViewComponent(ApplicationDbContext context) => _context = context;

        public async Task<IViewComponentResult> InvokeAsync(int eventId)
        {
            var summary = await _context.Registrations
                .Where(r => r.EventId == eventId)
                .ToListAsync();
            return View(summary);
        }
    }
}
