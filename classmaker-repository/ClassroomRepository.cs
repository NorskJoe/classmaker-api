using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using classmaker_models;
using classmaker_models.Dtos;
using classmaker_models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace classmaker_repositories
{
	/// <summary>
	/// Access to classroom objects in the db
	/// </summary>
	public interface IClassroomRepository
	{
		/// <summary>
		/// Get all classrooms and the students they contain
		/// </summary>
		/// <returns>List of classrooms with students</returns>
		Task<List<Classroom>> GetClassrooms();

		/// <summary>
		/// Add a classroom
		/// </summary>
		/// <param name="classroom"></param>
		/// <returns>Result object with success or error</returns>
		Task<Result> AddClassroom(Classroom classroom);

		/// <summary>
		/// Delete a classroom with given id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>Result object with success or error</returns>
		Task<Result> DeleteClassroom(int id);
	}

	public class ClassroomRepository : IClassroomRepository
	{
		private readonly EntityContext _context;
		private readonly ILogger<ClassroomRepository> _logger;

		public ClassroomRepository(EntityContext context, ILogger<ClassroomRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<List<Classroom>> GetClassrooms()
		{
			return await _context.Classrooms.ToListAsync();
		}

		public async Task<Result> AddClassroom(Classroom classroom)
		{
			var result = new Result();

			try
			{
				_context.Classrooms.Add(classroom);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError("An unexpected error occurred: {Message}", ex.Message);
				result.AddError(ex.Message);
			}

			return result;
		}

		public async Task<Result> DeleteClassroom(int id)
		{
			var result = new Result();
			var classroom = new Classroom {ClassroomId = id};

			try
			{
				_context.Classrooms.Remove(classroom);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError("An unexpected error occurred: {Message}", ex.Message);
				result.AddError(ex.Message);
			}

			return result;
		}
	}
}