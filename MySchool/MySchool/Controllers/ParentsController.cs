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
    [Authorize(Roles = "Parent")]
    public class ParentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Parents
        public IActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parent = _context.Parents.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            if (parent == null)
            {
                return View("Create");
            }
            var classPosts = _context.Posts.Where(x => x.Classroom.ClassName == parent.Classroom).ToList();

            return View(classPosts);
        }

        // GET: Parents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parent = await _context.Parents
                .Include(p => p.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parent == null)
            {
                return NotFound();
            }

            return View(parent);
        }

        // GET: Parents/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Parents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,PreferredTitle,PhoneNumber,Email,Address,StudentFirstName,StudentLastName,DOB,IdentityUserId,StudentId")] Parent parent)
        {
            if (ModelState.IsValid)
            {
                parent.IdentityUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var student = _context.Students.Where(x => x.DOB == parent.DOB && x.StudentFirstName == parent.StudentFirstName && x.StudentLastName == parent.StudentLastName).FirstOrDefault();
                parent.Student = student;
                parent.StudentId = student.StudentId;
                parent.Classroom = student.Classroom;
                _context.Add(parent);
                await _context.SaveChangesAsync();
               
            }

            return View("CreateEmergencyCard");
        }
        public IActionResult CreateEmergencyCard()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEmergencyCard([Bind("Id,StudentName,ParentOneName,ParentOneContact,ParentTwoName,ParentTwoContact,ECOneName,ECOneNumber,ECTwoName,ECTwoNumber,DocName,Allergies,StudentId,Student")] EmergencyCard card)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var parent = _context.Parents.Where(c => c.IdentityUserId == userId).FirstOrDefault();
                card.Student = parent.Student;
                card.StudentId = parent.StudentId;
                _context.Add(card);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        //Get Permission Slip
        public IActionResult FillPermissionSlip(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var slip = _context.PermissionSlips.Find(id);
            return View(slip);
        }
        [HttpPost]
        public IActionResult FillPermissionSlip(int id, [Bind("Id,Date,Location,Classroom,StudentFirst,StudentLast,ApprovingParent")] PermissionSlip permissionSlip)
        {
            var slip = _context.PermissionSlips.Where(x => x.Id == id).FirstOrDefault();
            
            permissionSlip.Date = slip.Date;
            permissionSlip.Location = slip.Location;
            permissionSlip.Classroom = slip.Classroom;
            permissionSlip.StudentFirst = slip.StudentFirst;
            permissionSlip.StudentLast = slip.StudentLast;
            _context.Update(permissionSlip);
            _context.SaveChanges();
            return View("ViewPendingForms");
        }
        public IActionResult ViewPermissionSlips()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parent = _context.Parents.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var slips = _context.PermissionSlips.Where(x => x.Classroom == parent.Classroom).ToList();
            
            return View(slips);
        }
        public IActionResult ViewPendingForms()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parent = _context.Parents.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var slips = _context.PermissionSlips.Where(x => x.Classroom == parent.Classroom).ToList();
            foreach (var slip in slips)
            {
                if (slip.ApprovingParent != null)
                {
                    slips.Remove(slip);
                }
            }
            if(slips == null)
            {
                ViewBag.NoForms = "You have no pending forms at this time";
                return View(ViewBag.NoForms);
            }
            return View(slips);
        }
        public IActionResult ViewTeacherAvailability()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parent = _context.Parents.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var availability = _context.SchedulerEvents.Where(x => x.StudentId == parent.StudentId);
            return View(availability);
            
        }
        public IActionResult ScheduleConference()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ScheduleConference([Bind("Id,Date,Text,ParentId,Parent")] Conference conference)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parent = _context.Parents.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            conference.ParentId = parent.Id;
            conference.Parent = parent;
            conference.Text = "Conference with" + parent.FirstName + " " + parent.LastName;
            conference.Id = 0;
            _context.Add(conference);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult ViewEmergencyCard()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parent = _context.Parents.Where(c => c.IdentityUserId == userId).FirstOrDefault();
            var studentId = parent.StudentId;
            var emergencyCard = _context.EmergencyCards.Where(x => x.StudentId == studentId);
            return View(emergencyCard);
        }
        //Get Emergency Card
        public IActionResult UpdateEmergencyCard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var card = _context.EmergencyCards.Find(id);
            return View(card);
        }
        [HttpPost]
        public IActionResult UpdateEmergencyCard(int id, [Bind("Id,StudentName,ParentOneName,ParentOneContact,ParentTwoName,ParentTwoContact,ECOneName,ECOneNumber,ECTwoName,ECTwoNumber,DocName,Allergies,StudentId,Student")] EmergencyCard card)
        {
            if (id != card.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var parent = _context.Parents.Where(c => c.IdentityUserId == userId).FirstOrDefault();
                card.StudentId = parent.StudentId;
                _context.Update(card);
                _context.SaveChanges();
            }
            
            return RedirectToAction("ViewEmergencyCard");
        }
        

        // GET: Parents/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", parent.IdentityUserId);
            return View(parent);
        }

        // POST: Parents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,PreferredTitle,PhoneNumber,Email,Address,IdentityUserId")] Parent parent)
        {
            if (id != parent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParentExists(parent.Id))
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
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", parent.IdentityUserId);
            return View(parent);
        }
        //Get: Comment
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
            var parent = _context.Parents.Where(c => c.IdentityUserId == userId).FirstOrDefault();

            var comment = new Comment() { Id = 0, Author = parent.FirstName + " " + parent.LastName, Text = text, Post = postRep, PostId = postRep.PostId };


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
        public IActionResult ViewComments(int id)
        {
            var comments = _context.Comments.Where(x => x.PostId == id);
            return View(comments);
        }

        // GET: Parents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parent = await _context.Parents
                .Include(p => p.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parent == null)
            {
                return NotFound();
            }

            return View(parent);
        }

        // POST: Parents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            _context.Parents.Remove(parent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParentExists(int id)
        {
            return _context.Parents.Any(e => e.Id == id);
        }
    }
}
