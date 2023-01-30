using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using classmaker_models.Dtos;
using classmaker_models.Entities;
using classmaker_repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace classmaker_api.Controllers
{
	[ApiController]
	[Authorize]
	[Route("[controller]")]
	public class ClassroomController : ControllerBase
	{
		private readonly IClassroomRepository _classroomRepository;

		public ClassroomController(IClassroomRepository classroomRepository)
		{
			_classroomRepository = classroomRepository;
		}
		
		/// <summary>
		/// Get all classrooms
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<List<Classroom>>> GetClassrooms()
		{
			var classrooms = await _classroomRepository.GetClassrooms();

			if (!classrooms.Any())
			{
				return NotFound();
			}

			return Ok(classrooms);
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Result>> AddClassroom([FromBody] Classroom classroom)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			return Ok(await _classroomRepository.AddClassroom(classroom));
		}
		
		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<Result>> DeleteClassroom(int id)
		{
			return Ok(await _classroomRepository.DeleteClassroom(id));
		}
	}
}