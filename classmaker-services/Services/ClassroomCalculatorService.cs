using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using classmaker_models.Dtos;
using classmaker_models.Entities;
using classmaker_models.Enums;
using classmaker_repositories;
using Microsoft.Extensions.Logging;

namespace classmaker_services.Services
{
    public interface IClassroomCalculatorService
    {
        /// <summary>
        /// Divide students into separate classes based on gender and perceived difficulty
        /// </summary>
        /// <param name="students"></param>
        /// <returns>List of sorted students with classrooms assigned</returns>
        Task<Result<List<Student>>> CalculateClassrooms(IEnumerable<Student> students);
    }

    public class ClassroomCalculatorService : IClassroomCalculatorService
    {
        private readonly ILogger<ClassroomCalculatorService> _logger;
        private readonly IClassroomRepository _classroomRepository;
        private readonly IStudentRepository _studentRepository;

        public ClassroomCalculatorService(ILogger<ClassroomCalculatorService> logger,
            IClassroomRepository classroomRepository,
            IStudentRepository studentRepository)
        {
            _logger = logger;
            _classroomRepository = classroomRepository;
            _studentRepository = studentRepository;
        }
        
        public async Task<Result<List<Student>>> CalculateClassrooms(IEnumerable<Student> students)
        {
            // TODO: Need to make sure I am saving/updating the students properly
            var result = new Result<List<Student>>();
            var classrooms = await _classroomRepository.GetClassrooms();
            var studentsToSort = students.Where(x => x.LockedInClassroom == false);
            
            // 1. Sort by difficulty
            studentsToSort = SortStudentsByDifficulty(studentsToSort);
            
            // 2. Split into Male/Female 
            var maleStudents = studentsToSort.Where(x => x.Gender == "M").ToList();
            var femaleStudents = studentsToSort.Where(x => x.Gender == "F").ToList();

            // 3. Divide into classrooms and save
            result.Value = DivideStudentsIntoClassrooms(maleStudents, femaleStudents, classrooms);
            await _studentRepository.UpdateStudents(result.Value);
            return result;
        }

        private static List<Student> DivideStudentsIntoClassrooms(List<Student> maleStudents, List<Student> femaleStudents, List<ClassroomDto> classrooms)
        {
            var sortedStudents = new List<Student>();
            
            sortedStudents.AddRange(DivideGenderedStudents(maleStudents, classrooms));
            sortedStudents.AddRange(DivideGenderedStudents(femaleStudents, classrooms));

            return sortedStudents;
        }

        private static IEnumerable<Student> DivideGenderedStudents(List<Student> students,
            List<ClassroomDto> classrooms)
        {
            var sortedStudents = new List<Student>();
            var classroomMaxLength = classrooms.Count - 1;
            var currentClassroomIndex = 0;
            for (var j = students.Count - 1; j >= 0; j--)
            {
                if (currentClassroomIndex > classroomMaxLength)
                {
                    currentClassroomIndex = 0;
                }
                
                var currentStudent = students[j]; 
                currentStudent.Classroom = ClassroomDtoHelper.DtoToClassroomMap(classrooms[currentClassroomIndex]);
                sortedStudents.Add(currentStudent);
                students.RemoveAt(j);
                currentClassroomIndex++;
            }

            return sortedStudents;
        }

        private static List<Student> SortStudentsByDifficulty(IEnumerable<Student> studentsToSort)
        {
            var sortStudentsByDifficulty = studentsToSort.ToList();
            foreach (var student in sortStudentsByDifficulty)
            {
                student.DifficultyRating = 
                    ((int)student.AcademicPerformance * StudentWeighting.AcademicPerformanceWeight) + 
                    ((int)student.Behaviour * StudentWeighting.BehaviourWeight) + 
                    ((int)student.LearningDifficulty * StudentWeighting.LearningDifficultyWeight);
            }

            return sortStudentsByDifficulty.OrderBy(x => x.DifficultyRating).ToList();
        }
    }
}