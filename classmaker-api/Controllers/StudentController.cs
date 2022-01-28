using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using classmaker_models.Entities;
using classmaker_models.QueryModels;
using classmaker_repository.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace classmaker_api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        
        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
    
        /// <summary>
        /// Get single student by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Student</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = await _studentRepository.GetStudent(id);

            if (student == null)
            {
                return NotFound();
            }
            
            return Ok(student);
        }
        
        /// <summary>
        /// Add a student
        /// </summary>
        /// <param name="student"></param>
        /// <returns>Result with errors or success</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> AddStudent([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _studentRepository.AddStudent(student));
        }

        /// <summary>
        /// List all Students
        /// </summary>
        /// <returns>List of all Students</returns>
        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Student>>> GetStudents()
        {
            var students = await _studentRepository.GetStudents();
            
            if (!students.Any())
            {
                return NotFound();
            }
            
            return Ok(students);
        }
        
        /// <summary>
        /// Update an existing student with new values
        /// </summary>
        /// <param name="student"></param>
        /// <returns>Result object indicating success, or containing errors/warnings</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateStudent([FromBody] Student student)
        {
            if (!ModelState.IsValid || student == null)
            {
                return BadRequest();
            }
            
            return Ok(await _studentRepository.UpdateStudent(student));
        }
        
        /// <summary>
        /// Delete an existing student by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Result object indicating success, or containing errors/warnings</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            return Ok(await _studentRepository.DeleteStudent(id));
        }

    }
}