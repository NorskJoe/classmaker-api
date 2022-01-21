using System.ComponentModel.DataAnnotations;

namespace classmaker_models.Entities
{
    public class Classroom
    {
        [Required]
        public int ClassroomId { get; set; }

        public string Name { get; set; }
    }
}