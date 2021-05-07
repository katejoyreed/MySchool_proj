﻿using System;
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
            var post = _context.Posts.Where(x => x.ClassId == id).FirstOrDefault();
            return View(post);
        }
        [HttpPut]
        public IActionResult CreateComment(string text, int id, [Bind("PostId,Text,Author,Comments,ClassId,Classroom")] Post post)
        {
            if (post.PostId != id)
            {
                return NotFound();
            }
            var postRep = _context.Posts.Where(x => x.PostId == id).FirstOrDefault();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = _context.Teachers.Where(c => c.IdentityUserId == userId).FirstOrDefault();

            var comment = new Comment() { Id = 0, Author = teacher.FirstName + " " + teacher.LastName, Text = text, Post = postRep, PostId = postRep.PostId };
        

            post.PostId = postRep.PostId;
            post.Text = postRep.Text;
            post.Author = postRep.Author;
            post.ClassId = postRep.ClassId;
            post.Classroom = postRep.Classroom;
            _context.Add(comment);
            _context.SaveChanges();
            post.Comments.Add(comment);
            _context.Update(post);
            _context.SaveChanges();
            return RedirectToAction("Index");

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
        public IActionResult CreatePost([Bind("PostId,Text,Author,Comments,ClassId,Classroom")] Post post)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacher = _context.Teachers.Where(c => c.IdentityUserId == userId).FirstOrDefault();
                var classroom = _context.Classrooms.Where(x => x.ClassName == teacher.Classroom).FirstOrDefault();
                post.Author = teacher.FirstName + " " + teacher.LastName;
                post.ClassId = classroom.ClassId;
                post.Classroom = classroom;
                post.Comments = new List<Comment>();
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
            return RedirectToAction("Index");
        }

        public IActionResult MyEvents()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = _context.Teachers.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var parents = _context.Parents.Where(x => x.Classroom == teacher.Classroom).ToList();
            List<Conference> conferences = null;
            foreach (var parent in parents)
            {
                var conference = _context.Conferences.Where(x => x.Parent == parent).FirstOrDefault();
                conferences.Add(conference);
            }
            return View(conferences);
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
