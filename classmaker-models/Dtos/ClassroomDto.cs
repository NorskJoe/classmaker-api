using System.Collections.Generic;
using classmaker_models.Entities;

namespace classmaker_models.Dtos
{
	public class ClassroomDto
	{
		public int ClassroomId { get; set; }
		public string Name { get; set; }
		public List<Student> Students { get; set; }
	}
}