using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.ViewModels;
using System.Data;

namespace EventManagementSystem.Controllers
{
    public class AttendeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AttendeesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        // GET: Attendees
        public async Task<IActionResult> Index()
        {

            var attendees = await _context.Attendees
                .Include(a => a.Registrations)
                .ThenInclude(r => r.Event)
                .ToListAsync();

            return View(attendees);
        }
        [HttpGet]
        public async Task<JsonResult> GetAllEvents()
        {
            var events = await _context.Events
                .Select(e => new { e.EventId, e.EventName, e.TicketPrice })
                .ToListAsync();
            return Json(events);
        }

        // GET: Create
        public IActionResult Create()
        {
            var model = new AttendeeVM();

            model.Registrations.Add(new RegistrationVM());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendeeVM vm)
        {
            if (ModelState.IsValid)
            {

                string uniqueFileName = ProcessUploadedFile(vm);


                DataTable dt = new DataTable();
                dt.Columns.Add("RegistrationId", typeof(int));
                dt.Columns.Add("EventId", typeof(int));
                dt.Columns.Add("TicketCount", typeof(int));
                dt.Columns.Add("RegistrationDate", typeof(DateTime));
                dt.Columns.Add("TotalPaid", typeof(decimal));

                foreach (var item in vm.Registrations)
                {
                    dt.Rows.Add(0, item.EventId, item.TicketCount, DateTime.Now, item.TotalPaid);
                }


                var actionParam = new SqlParameter("@Action", "INSERT");
                var nameParam = new SqlParameter("@FullName", vm.FullName);
                var emailParam = new SqlParameter("@Email", vm.Email);
                var phoneParam = new SqlParameter("@PhoneNumber", vm.PhoneNumber);
                var imgParam = new SqlParameter("@Image", uniqueFileName ?? (object)DBNull.Value);
                var adultParam = new SqlParameter("@IsAdult", vm.IsAdult);

                var tableParam = new SqlParameter("@Registrations", SqlDbType.Structured)
                {
                    TypeName = "dbo.RegistrationTableType",
                    Value = dt
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_ManageAttendeeRegistrations @Action, 0, @FullName, @Email, @PhoneNumber, @Image, @IsAdult, @Registrations",
                    actionParam, nameParam, emailParam, phoneParam, imgParam, adultParam, tableParam);

                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        private string ProcessUploadedFile(AttendeeVM vm)
        {
            string uniqueFileName = "";
            if (vm.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + vm.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    vm.ImageFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();


            var attendee = await _context.Attendees
                .Include(a => a.Registrations)
                .FirstOrDefaultAsync(m => m.AttendeeId == id);

            if (attendee == null) return NotFound();


            var vm = new AttendeeVM
            {
                AttendeeId = attendee.AttendeeId,
                FullName = attendee.FullName,
                Email = attendee.Email,
                PhoneNumber = attendee.PhoneNumber,
                ExistingImage = attendee.Image,
                IsAdult = attendee.IsAdult,
                Registrations = attendee.Registrations.Select(r => {

                    var eventInfo = _context.Events.FirstOrDefault(e => e.EventId == r.EventId);
                    var price = eventInfo?.TicketPrice ?? 0;

                    return new RegistrationVM
                    {
                        RegistrationId = r.RegistrationId,
                        EventId = r.EventId,
                        EventName = eventInfo?.EventName,
                        TicketCount = r.TicketCount,
                        TicketPrice = price

                        
                    };
                }).ToList()
            };


            ViewBag.Events = await _context.Events.ToListAsync();

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AttendeeVM vm)
        {


            if (ModelState.IsValid)
            {
                string? imageName = vm.ExistingImage;
                if (vm.ImageFile != null)
                {
                    imageName = ProcessUploadedFile(vm);
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("RegistrationId", typeof(int));
                dt.Columns.Add("EventId", typeof(int));
                dt.Columns.Add("TicketCount", typeof(int));
                dt.Columns.Add("RegistrationDate", typeof(DateTime));
                dt.Columns.Add("TotalPaid", typeof(decimal));

                if (vm.Registrations != null)
                {
                    foreach (var item in vm.Registrations)
                    {

                        decimal total = (item.TicketCount * item.TicketPrice);
                        dt.Rows.Add(item.RegistrationId, item.EventId, item.TicketCount, DateTime.Now, total);
                    }
                }

                var actionParam = new SqlParameter("@Action", "UPDATE");
                var idParam = new SqlParameter("@AttendeeId", vm.AttendeeId);
                var nameParam = new SqlParameter("@FullName", vm.FullName);
                var emailParam = new SqlParameter("@Email", vm.Email);
                var phoneParam = new SqlParameter("@PhoneNumber", vm.PhoneNumber ?? (object)DBNull.Value);
                var imgParam = new SqlParameter("@Image", imageName);
                var adultParam = new SqlParameter("@IsAdult", vm.IsAdult);

                var tableParam = new SqlParameter("@Registrations", SqlDbType.Structured)
                {
                    TypeName = "dbo.RegistrationTableType",
                    Value = dt
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_ManageAttendeeRegistrations @Action, @AttendeeId, @FullName, @Email, @PhoneNumber, @Image, @IsAdult, @Registrations",
                    actionParam, idParam, nameParam, emailParam, phoneParam, imgParam, adultParam, tableParam);

                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }
        // GET: Attendees/Delete/1
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var attendee = await _context.Attendees
                .Include(a => a.Registrations)
                .ThenInclude(r => r.Event)
                .FirstOrDefaultAsync(a => a.AttendeeId == id);

            if (attendee == null) return NotFound();

            return View(attendee); // confirmation view
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var attendee = await _context.Attendees.AsNoTracking()
                .FirstOrDefaultAsync(a => a.AttendeeId == id);

            if (attendee == null) return RedirectToAction(nameof(Index));


            DataTable dt = new DataTable();
            dt.Columns.Add("RegistrationId", typeof(int));
            dt.Columns.Add("EventId", typeof(int));
            dt.Columns.Add("TicketCount", typeof(int));
            dt.Columns.Add("RegistrationDate", typeof(DateTime));
            dt.Columns.Add("TotalPaid", typeof(decimal));

            var actionParam = new SqlParameter("@Action", "DELETE");
            var idParam = new SqlParameter("@AttendeeId", id);


            var nameParam = new SqlParameter("@FullName", attendee.FullName ?? (object)DBNull.Value);
            var emailParam = new SqlParameter("@Email", attendee.Email ?? (object)DBNull.Value);
            var phoneParam = new SqlParameter("@PhoneNumber", attendee.PhoneNumber ?? (object)DBNull.Value);
            var imgParam = new SqlParameter("@Image", attendee.Image ?? (object)DBNull.Value);
            var adultParam = new SqlParameter("@IsAdult", attendee.IsAdult);

            var tableParam = new SqlParameter("@Registrations", SqlDbType.Structured)
            {
                TypeName = "dbo.RegistrationTableType",
                Value = dt
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_ManageAttendeeRegistrations @Action, @AttendeeId, @FullName, @Email, @PhoneNumber, @Image, @IsAdult, @Registrations",
                actionParam, idParam, nameParam, emailParam, phoneParam, imgParam, adultParam, tableParam);

            if (!string.IsNullOrWhiteSpace(attendee.Image))
            {
                var imgPath = Path.Combine(_environment.WebRootPath, "images", attendee.Image);
                if (System.IO.File.Exists(imgPath))
                    System.IO.File.Delete(imgPath);
            }

            return RedirectToAction(nameof(Index));
        }

    }

}