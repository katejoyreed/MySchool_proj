using System;
using System.Collections.Generic;
using System.Linq;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStudent([Bind("StudentId,StudentName,Classroom")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                _context.SaveChangesAsync();
                return View("Index");
            }
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateClassroom([Bind("ClassId,ClassName")] Classroom classroom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classroom);
                _context.SaveChangesAsync();
                return View("Index");
            }
            return View("Index");
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

        public IActionResult CreatePermissionSlip([Bind("Id,Date,Location,Time,Classroom,StudentName,ApprovingParent")]PermissionSlip slip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(slip);
                _context.SaveChangesAsync();
                return View("Index");
            }
            return View("Index");
        }

        public IActionResult ViewData(string cohort, string classroom)
        {
            List<string> cohorts = new List<string> { "Parents", "Teachers", "Students" };
            List<string> classrooms = _context.Classrooms.Select(x => x.ClassName).Distinct().ToList();
            classrooms.Insert(0, "All");
            ViewBag.Cohorts = new SelectList(cohorts);
            ViewBag.SearchTypes = new SelectList(classrooms);
            

            if (cohort == "Parents")
            {
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
            
            else if (cohort == "Teachers")
            {
                if (classroom == "All")
                {
                    var allTeachers = _context.Teachers.ToList();
                    return View(allTeachers);
                }
                else 
                {
                    var classTeachers = _context.Teachers.Where(x => x.Classroom == classroom).ToList();
                    return View(classTeachers);
                }
                
            }
            else if (cohort == "Students")
            {
                if (classroom == "All")
                {
                    var allStudents = _context.Students.ToList();
                    return View(allStudents);
                }
                else 
                {
                    var classStudents = _context.Students.Where(x => x.Classroom == classroom);
                    return View(classStudents);
                }
            }
            else 
            {
                return NotFound();
            }
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
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", admin.IdentityUserId);
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
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", admin.IdentityUserId);
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
