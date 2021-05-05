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
        public IActionResult CreatePermissionSlip(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var classroom = _context.Classrooms.Find(id);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePermissionSlip(int id,[Bind("Id,Date,Location,Time,Classroom,StudentName,ApprovingParent")]PermissionSlip slip)
        {
            if (ModelState.IsValid)
            {
                var classroom = _context.Classrooms.Find(id);
                var students = _context.Students.Where(x => x.Classroom == classroom.ClassName);
                foreach(var student in students)
                {
                    slip.StudentName = student.StudentName;
                    slip.Classroom = classroom.ClassName;
                    _context.Add(slip);
                    await _context.SaveChangesAsync();
                }
                


            }
            return RedirectToAction("Index");
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
        public IActionResult EmailAllParents()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EmailAllParents(string subject, string message)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _context.Admins.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            MailMessage mailMessage = new MailMessage();
            var parentEmails = _context.Parents.Select(x => x.Email).ToList();
            if (ModelState.IsValid)
            {
                var senderEmail = new MailAddress(admin.Email);
                
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
                foreach(var email in parentEmails)
                {
                    var receiverEmail = new MailAddress(email);
                    using (var mess = new MailMessage(senderEmail, receiverEmail) { Subject = subject, Body = body }) { smtp.Send(mess); }
                }
                
                return View();
            }
            return View();
        }

        public IActionResult EmailThisTeacher(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var teacherUser = _context.Teachers.Find(id);
            return View(teacherUser);
        }
        [HttpPost]
        public IActionResult EmailThisTeacher(int id, string subject, string message)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _context.Admins.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var teacherUser = _context.Teachers.Find(id);
            
                var senderEmail = new MailAddress(admin.Email);
                var receiverEmail = new MailAddress(teacherUser.Email);
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
                using (var mess = new MailMessage(senderEmail, receiverEmail) { Subject = subject, Body = body }) { smtp.Send(mess); }
                
            
            return View("Index");
        }
        public IActionResult EmailAllTeachers()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EmailAllTeachers(string subject, string message)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _context.Admins.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            MailMessage mailMessage = new MailMessage();
            var teacherEmails = _context.Teachers.Select(x => x.Email).ToList();
            if (ModelState.IsValid)
            {
                var senderEmail = new MailAddress(admin.Email);

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
                foreach (var email in teacherEmails)
                {
                    var receiverEmail = new MailAddress(email);
                    using (var mess = new MailMessage(senderEmail, receiverEmail) { Subject = subject, Body = body }) { smtp.Send(mess); }
                }

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
        public IActionResult ViewPosts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var posts = _context.Posts.Where(x => x.ClassId == id);
            return View(posts);
        }
        public IActionResult CreatePost(int? id)
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreatePost(int id, [Bind("PostId,Text,Author,ClassId,Classroom")] Post post)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var admin = _context.Admins.Where(c => c.IdentityUserId == userId).FirstOrDefault();
                var classroom = _context.Classrooms.Find(id);
                post.Author = admin.FirstName + " " + admin.LastName;
                post.ClassId = classroom.ClassId;
                post.Classroom = classroom;
                _context.Add(post);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewPosts");
        }

        public async Task<IActionResult> EditStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            
            return View(student);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(int id, [Bind("StudentId,StudentName,Classroom")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                
                
                return RedirectToAction(nameof(Index));
            }
            // ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", admin.IdentityUserId);
            return View(student);
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

        public async Task<IActionResult> DeleteStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteForm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var form = await _context.PermissionSlips
                .FirstOrDefaultAsync(m => m.Id == id);
            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        public async Task<IActionResult> MoveParent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parent = await _context.Parents.FindAsync(id);
            if (parent == null)
            {
                return NotFound();
            }
            
            return View(parent);
        }

        // POST: Parents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MoveParent(int id, [Bind("Id,FirstName,LastName,PreferredTitle,PhoneNumber,Email,Address,Classroom,IdentityUserId")] Parent model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            var parent = _context.Parents.Find(id);
            if (ModelState.IsValid)
            {
                parent.Address = model.Address;
                parent.Email = model.Email;
                parent.FirstName = model.FirstName;
                parent.Id = model.Id;
                parent.IdentityUserId = model.IdentityUserId;
                parent.LastName = model.LastName;
                parent.PhoneNumber = model.PhoneNumber;
                parent.PreferredTitle = model.PreferredTitle;
                _context.Update(parent);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            
            return View(parent);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var form = await _context.PermissionSlips.FindAsync(id);
            _context.PermissionSlips.Remove(form);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
    }
}
