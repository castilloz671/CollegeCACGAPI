using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeCACGAPI.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime F_Nacimiento { get; set; }

        public ICollection<Semester> Semesters { get; set; }
    }

    public class Semester
    {
        [Key]
        public int SemesterId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        public ICollection<Subject> Subjects { get; set; }
    }

    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int SemesterId { get; set; }

        [ForeignKey("SemesterId")]
        public Semester Semester { get; set; }

        public ICollection<MonthlyGrade> MonthlyGrades { get; set; }
        public ICollection<Exam> Exams { get; set; }

        [NotMapped]
        public decimal FinalGrade { get; set; }

        [NotMapped]
        public bool IsPassed { get; set; }

        public void CalculateFinalGrade()
        {
            decimal monthlyGradeSum = 0;
            foreach (var monthlyGrade in MonthlyGrades)
            {
                monthlyGradeSum += monthlyGrade.Grade;
            }

            decimal monthlyAverage = MonthlyGrades.Count > 0 ? monthlyGradeSum / MonthlyGrades.Count : 0;
            decimal monthlyWeight = monthlyAverage * 0.7M;

            decimal examsSum = 0;
            foreach (var exam in Exams)
            {
                examsSum += exam.Grade;
            }

            decimal examsAverage = Exams.Count > 0 ? examsSum / Exams.Count : 0;
            decimal examsWeight = examsAverage * 0.3M;

            FinalGrade = monthlyWeight + examsWeight;
            IsPassed = FinalGrade >= 70;
        }
    }

    public class MonthlyGrade
    {
        [Key]
        public int MonthlyGradeId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }

        [Required]
        public int Month {  get; set; }

        [Range(0, 100)]
        public decimal Grade { get; set; }
    }

    public class Exam
    {
        [Key]
        public int ExamId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }

        [Required]
        [StringLength(100)]
        public string ExamName { get; set; }

        [Range(0, 100)]
        public decimal Grade { get; set; }
    }
}
