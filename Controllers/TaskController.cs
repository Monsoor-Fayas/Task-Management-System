using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    //[Route("task")]
    [Authorize]
    public class TaskController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private DateTime _TaskDate;
        public TaskController(ApplicationDbContext context,
            IServiceProvider serviceProvider,
            UserManager<IdentityUser> userManager) : base(serviceProvider, userManager)
        {
            _context = context;
        }



        // GET: TaskDataModels
        public IActionResult Index()
        {
            return View();
        }

        // GET: TaskDataModels/Details/5
        public async Task<IActionResult> GetTaskList(GetTaskParams getTaskParams)
        {
            try
            {
                List<TaskDataModel> taskList = new();

                if (_context.TaskDataModel != null)
                {
                    if (getTaskParams == null || getTaskParams.TaskDate == DateTime.MinValue)
                    {
                        _TaskDate = DateTime.Parse(HttpContext.Session.GetString("TaskDate")).Date;
                    }
                    else
                    {
                        _TaskDate = getTaskParams.TaskDate;
                    }

                    HttpContext.Session.SetString("TaskDate", _TaskDate.Date.ToString());
                    taskList = await _context.TaskDataModel
                        .Where(x => x.CreatedUser == LoggedInUserId && x.TaskDate.Date == _TaskDate)
                        .ToListAsync();
                }

                return View("List", taskList);

            }
            catch (Exception ex)
            {
                Problem(ex.Message);
            }

            return RedirectToAction("Index");
        }

        // GET: TaskDataModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TaskDataModel == null)
            {
                return NotFound();
            }

            var taskDataModel = await _context.TaskDataModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskDataModel == null)
            {
                return NotFound();
            }

            return View(taskDataModel);
        }

        // GET: TaskDataModels/Create
        public IActionResult Create()
        {
            TaskDataModel taskDataModel = new();
            if (HttpContext != null && HttpContext.Session != null)
            {
                taskDataModel.TaskDate = DateTime.Parse(HttpContext.Session.GetString("TaskDate")).Date;
            }
            return View(taskDataModel);
        }

        // POST: TaskDataModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TaskDescription,TaskDate")] TaskDataModel taskDataModel)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext != null && HttpContext.Session != null)
                {
                    taskDataModel.TaskDate = DateTime.Parse(HttpContext.Session.GetString("TaskDate")).Date;
                }
                taskDataModel.CreatedUser = LoggedInUserId;
                taskDataModel.CreatedTime = DateTime.UtcNow;

                _context.Add(taskDataModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetTaskList));
            }
            return View(taskDataModel);
        }

        // GET: TaskDataModels/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (_context.TaskDataModel == null)
            {
                return NotFound();
            }

            var taskDataModel = GetTaskDataModelById(id);
            if (taskDataModel == null)
            {
                return NotFound();
            }

            return View(taskDataModel);
        }

        // POST: TaskDataModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TaskDescription,TaskDate,CreatedUser,CreatedTime")] TaskDataModel taskDataModel)
        {
            if (id != taskDataModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (HttpContext != null && HttpContext.Session != null)
                    {
                        taskDataModel.TaskDate = DateTime.Parse(HttpContext.Session.GetString("TaskDate")).Date;
                    }

                    TaskDataModel currTask = GetTaskDataModelById(id);
                    if (currTask != null)
                    {
                        currTask.TaskDescription = taskDataModel.TaskDescription;
                        _context.Update(currTask);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(GetTaskList));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskDataModelExists(taskDataModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(taskDataModel);
        }

        // GET: TaskDataModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TaskDataModel == null)
            {
                return NotFound();
            }

            var taskDataModel = await _context.TaskDataModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskDataModel == null)
            {
                return NotFound();
            }

            return View(taskDataModel);
        }

        // POST: TaskDataModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TaskDataModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TaskDataModel'  is null.");
            }
            var taskDataModel = await _context.TaskDataModel.FindAsync(id);
            if (taskDataModel != null)
            {
                _context.TaskDataModel.Remove(taskDataModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetTaskList));
        }

        private bool TaskDataModelExists(int id)
        {
            return (_context.TaskDataModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private TaskDataModel GetTaskDataModelById(int id)
        {
            return _context.TaskDataModel?.FirstOrDefault(x => x.Id == id);
        }
    }
}
