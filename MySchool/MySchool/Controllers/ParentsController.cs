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
        public async Task<IActionResult> Index()
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
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,PreferredTitle,PhoneNumber,Email,Address,IdentityUserId")] Parent parent)
        {
            if (ModelState.IsValid)
            {
                parent.IdentityUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(parent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEmergencyCard([Bind("Id,StudentName,ParentOneName,ParentOneContact,ParentTwoName,ParentTwoContact,ECOneName,ECOneNumber,ECTwoName,ECTwoNumber,DocName,Allergies,StudentId,Student")] EmergencyCard card)
        {
            if (ModelState.IsValid)
            {
                _context.Add(card);
                _context.SaveChangesAsync();
                return View("Index");
            }
            return View("Index");
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

        public IActionResult FillPermissionSlip(int id, [Bind("Id,Date,Location,Time,Classroom,StudentName,ApprovingParent")]PermissionSlip permissionSlip)
        {
            var slip = _context.PermissionSlips.Where(x => x.Id == id).FirstOrDefault();
            permissionSlip.Id = slip.Id;
            permissionSlip.Date = slip.Date;
            permissionSlip.Location = slip.Location;
            permissionSlip.Time = slip.Time;
            permissionSlip.Classroom = slip.Classroom;
            _context.Update(permissionSlip);
            _context.SaveChangesAsync();
            return View(permissionSlip);
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
