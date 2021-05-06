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
    [Authorize(Roles = "Teacher")]
    public class TeachersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeachersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Teachers
        public IActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = _context.Teachers.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            if (teacher == null)
            {
                return View("Create");
            }
            var classPosts = _context.Posts.Where(x => x.Classroom.ClassName == teacher.Classroom);
            return View(classPosts);
        }
        public IActionResult MyStudents()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = _context.Teachers.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var myStudents = _context.Students.Where(x => x.Classroom == teacher.Classroom);
            return View(myStudents);
        }
        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.IdentityUser)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        public IActionResult CreateComment(int? id)
        {
            var post = _context.Posts.Find(id);
            return View(post);
        }
        [HttpPost]
        public IActionResult CreateComment(int id, [Bind("Id,Author,Text,PostId,Post")] Comment comment)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = _context.Teachers.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var post = _context.Posts.Where(x => x.PostId == id).FirstOrDefault();
            comment.Author = teacher.FirstName + " " + teacher.LastName;
            comment.PostId = post.PostId;
            comment.Post = post;
            _context.Add(post);
            _context.SaveChanges();
            return RedirectToAction("ViewComments");

        }
        public IActionResult ViewComments(int id)
        {
            var comments = _context.Comments.Where(x => x.PostId == id);
            return View(comments);
        }
        // GET: Teachers/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeacherId,FirstName,LastName,Email,Classroom,PhoneNumber,IdentityUserId")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                teacher.IdentityUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(teacher);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }
        //Get Post
        public IActionResult CreatePost()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreatePost([Bind("PostId,Text,Author,ClassId,Classroom")] Post post)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacher = _context.Teachers.Where(c => c.IdentityUserId == userId).FirstOrDefault();
                var classroom = _context.Classrooms.Where(x => x.ClassName == teacher.Classroom).FirstOrDefault();
                post.Author = teacher.FirstName + " " + teacher.LastName;
                post.ClassId = classroom.ClassId;
                post.Classroom = classroom;
                _context.Add(post);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult SetAvailability()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SetAvailability([Bind("Id,Text,StartDate,EndDate,StudentId,Student")] SchedulerEvent schedulerEvent)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = _context.Teachers.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var students = _context.Students.Where(x => x.Classroom == teacher.Classroom).ToList();
            foreach (var  student in students)
            {
                schedulerEvent.Id = 0;
                schedulerEvent.StudentId = student.StudentId;
                schedulerEvent.Student = student;
                SchedulerEvent availabileTime = new SchedulerEvent() { Id = schedulerEvent.Id, Text = schedulerEvent.Text, StartDate = schedulerEvent.StartDate, EndDate = schedulerEvent.StartDate, StudentId = schedulerEvent.StudentId, Student = schedulerEvent.Student };
                _context.Add(availabileTime);
            }
            _context.SaveChanges();
            return RedirectToAction("MyScheduledEvents");
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", teacher.IdentityUserId);
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeacherId,FirstName,LastName,Email,Classroom,PhoneNumber,IdentityUserId")] Teacher teacher)
        {
            if (id != teacher.TeacherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.TeacherId))
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
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", teacher.IdentityUserId);
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.IdentityUser)
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.TeacherId == id);
        }
    }
}
