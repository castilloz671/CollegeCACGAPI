using CollegeCACGAPI.Data;
using CollegeCACGAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeCACGAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentContext _context;

        public StudentsController(StudentContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.Semesters)
                .ThenInclude(s => s.Subjects)
                .ThenInclude(s => s.MonthlyGrades)
                .Include(s => s.Semesters)
                .ThenInclude(s => s.Subjects)
                .ThenInclude(s => s.Exams)
                .ToListAsync();

            foreach (var student in students)
            {
                foreach (var semester in student.Semesters)
                {
                    foreach (var subject in semester.Subjects)
                    {
                        subject.CalculateFinalGrade();
                    }
                }
            }

            return students;
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.Semesters)
                .ThenInclude(s => s.Subjects)
                .ThenInclude(s => s.MonthlyGrades)
                .Include(s => s.Semesters)
                .ThenInclude(s => s.Subjects)
                .ThenInclude(s => s.Exams)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            foreach (var semester in student.Semesters)
            {
                foreach (var subject in semester.Subjects)
                {
                    subject.CalculateFinalGrade();
                }
            }

            return student;
        }

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.StudentId }, student);
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.Semesters)
                .ThenInclude(s => s.Subjects)
                .ThenInclude(s => s.MonthlyGrades)
                .Include(s => s.Semesters)
                .ThenInclude(s => s.Subjects)
                .ThenInclude(s => s.Exams)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            foreach (var semester in student.Semesters)
            {
                foreach (var subject in semester.Subjects)
                {
                    if (subject.MonthlyGrades != null)
                    {
                        _context.MonthlyGrades.RemoveRange(subject.MonthlyGrades);
                    }

                    if (subject.Exams != null)
                    {
                        _context.Exams.RemoveRange(subject.Exams);
                    }
                }

                if (semester.Subjects != null)
                {
                    _context.Subjects.RemoveRange(semester.Subjects);
                }
            }

            if (student.Semesters != null)
            {
                _context.Semesters.RemoveRange(student.Semesters);
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{studentId}/monthlyGrades")]
        public async Task<ActionResult<MonthlyGrade>> PostMonthlyGrade(int studentId, MonthlyGrade monthlyGrade)
        {
            if (monthlyGrade == null)
            {
                return BadRequest("Monthly grade data is null");
            }

            var subject = await _context.Subjects.FindAsync(monthlyGrade.SubjectId);
            if (subject == null)
            {
                return NotFound("Subject not found");
            }

            _context.MonthlyGrades.Add(monthlyGrade);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = studentId }, monthlyGrade);
        }

        [HttpPost("{studentId}/exams")]
        public async Task<ActionResult<Exam>> PostExam(int studentId, Exam exam)
        {
            if (exam == null)
            {
                return BadRequest("Exam data is null");
            }

            var subject = await _context.Subjects.FindAsync(exam.SubjectId);
            if (subject == null)
            {
                return NotFound("Subject not found");
            }

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = studentId }, exam);
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
