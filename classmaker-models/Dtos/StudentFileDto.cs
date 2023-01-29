using classmaker_models.Entities;
using classmaker_models.Enums;

namespace classmaker_models.Dtos
{
    public class StudentFileDto
    {
        public string Firstname { get; set; }
        
        public string Surname { get; set; }
        
        public string Gender { get; set; }
        
        public AcademicPerformance AcademicPerformance { get; set; }
        
        public Behaviour Behaviour { get; set; }
        
        public LearningDifficulty LearningDifficulty { get; set; }

        
    }

    public static class StudentDtoHelper
    {
        /// <summary>
        /// Map StudentDto to Student object
        /// </summary>
        /// <param name="student"></param>
        /// <returns>Student</returns>
        public static Student DtoToStudentMap(StudentFileDto student)
        {
            return new Student
            {
                Firstname = student.Firstname,
                Surname = student.Surname,
                Gender = student.Gender,
                AcademicPerformance = student.AcademicPerformance,
                Behaviour = student.Behaviour,
                LearningDifficulty = student.LearningDifficulty
            };
        }
    }
}