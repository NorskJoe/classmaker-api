using System.ComponentModel.DataAnnotations;
using classmaker_models.Enums;

namespace classmaker_models.Entities
{
    public class Student
    {
        [Required] 
        public int StudentId { get; set; }
        
        [Required]
        public string Firstname { get; set; }
        
        [Required]
        public string Surname { get; set; }
        
        [Required]
        public string Gender { get; set; }
        
        [Range(1, 5)]
        public AcademicPerformance AcademicPerformance { get; set; }
        
        [Range(1, 5)]
        public Behaviour Behaviour { get; set; }
        
        [Range(1, 3)]
        public LearningDifficulty LearningDifficulty { get; set; }
    }
}