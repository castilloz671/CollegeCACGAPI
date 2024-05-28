using CollegeCACGAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeCACGAPI.Data
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }

        public DbSet<Semester> Semesters { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<MonthlyGrade> MonthlyGrades { get; set;}

        public DbSet<Exam> Exams { get; set; }
    }
}
