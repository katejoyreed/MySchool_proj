using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySchool.Data;
using MySchool.Models;

namespace MySchool.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admins
        public IActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _context.Admins.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            if (admin == null)
            {
                return View("Create");
            }
            var classList = _context.Classrooms.ToList();
            return View(classList);
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,IdentityUserId")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                admin.IdentityUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View("Index");
        }
        //Get Student
        public IActionResult AddStudent(int? id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStudent(int id, [Bind("StudentId,StudentName,Classroom")] Student student)
        {
            var classroom = _context.Classrooms.Where(x => x.ClassId == id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                student.Classroom = classroom.ClassName;
                _context.Add(student);
                _context.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }

        //Get Classroom
        public IActionResult CreateClassroom()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateClassroom([Bind("ClassId,ClassName")] Classroom classroom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classroom);
                _context.SaveChanges();
                
            }
            return RedirectToAction("Index");
        }

        //Get ClassList
        public IActionResult SeeStudents(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classroom = _context.Classrooms.Where(x => x.ClassId == id).FirstOrDefault();
            var students = _context.Students.Where(x => x.Classroom == classroom.ClassName).ToList();
            return View(students);
        }

        //Get EmergencyCard
        public IActionResult SeeEmergencyCard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergencyCard = _context.EmergencyCards.Where(x => x.StudentId == id);
                return View(emergencyCard);
        }
        //Get PermissionSlip
        public IActionResult CreatePermissionSlip()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreatePermissionSlip([Bind("Id,Date,Location,Time,Classroom,StudentName,ApprovingParent")]PermissionSlip slip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(slip);
                _context.SaveChanges();
                
            }
            return View("Index");
        }

        public IActionResult ParentData(string classroom)
        {
            List<string> classrooms = _context.Classrooms.Select(x => x.ClassName).Distinct().ToList();
            classrooms.Insert(0, "All");
            ViewBag.SearchTypes = new SelectList(classrooms);
                if (classroom == "All")
                {
                    var allParents = _context.Parents.ToList();
                    return View(allParents);
                }
                else
                {
                    var classParents = _context.Parents.Where(x => x.Classroom == classroom).ToList();
                    return View(classParents);
                }
        }
        public IActionResult TeacherData(string classroom)
        {
            List<string> classrooms = _context.Classrooms.Select(x => x.ClassName).Distinct().ToList();
            classrooms.Insert(0, "All");
            ViewBag.SearchTypes = new SelectList(classrooms);
            if(classroom == "All")
            {
                var allTeachers = _context.Teachers.ToList();
                return View(allTeachers);
            }
            else 
            {
                var classTeachers = _context.Teachers.Where(x => x.Classroom == classroom).Distinct().ToList();
                return View(classTeachers);
            }
        }
        public IActionResult StudentData(string classroom)
        {
            List<string> classrooms = _context.Classrooms.Select(x => x.ClassName).Distinct().ToList();
            classrooms.Insert(0, "All");
            ViewBag.SearchTypes = new SelectList(classrooms);
            if (classroom == "All")
            {
                var allStudents = _context.Students.ToList();
                return View(allStudents);
            }
            else
            {
                var classStudents = _context.Students.Where(x => x.Classroom == classroom).Distinct().ToList();
                return View(classStudents);
            }
        }
        public IActionResult SendThisParentEmail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var parentUser = _context.Parents.Find(id);
            return View(parentUser);
        }
        [HttpPost]
        public IActionResult SendThisParentEmail(int id, string subject, string message)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _context.Admins.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var parentUser = _context.Parents.Find(id);
            if (ModelState.IsValid)
            {
                var senderEmail = new MailAddress(admin.Email);
                var receiverEmail = new MailAddress(parentUser.Email);
                var password = admin.Password;
                var sub = subject;
                var body = message;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using(var mess = new MailMessage(senderEmail, receiverEmail) { Subject = subject, Body = body }) { smtp.Send(mess); }
                return View();
            }
            return View();
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            //ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", admin.IdentityUserId);
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,IdentityUserId")] Admin admin)
        {
            if (id != admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
           // ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", admin.IdentityUserId);
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
    }
}
