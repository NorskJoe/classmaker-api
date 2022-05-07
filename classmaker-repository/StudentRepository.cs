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
    /// Access to student objects in the db
    /// </summary>
    public interface IStudentRepository
    {
        /// <summary>
        /// Get single student by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Student</returns>
        Task<Student?> GetStudent(int id);
        
        /// <summary>
        /// List all Students
        /// </summary>
        /// <returns>List of all Students</returns>
        Task<List<Student>> GetStudents();

        /// <summary>
        /// Add a student
        /// </summary>
        /// <param name="student"></param>
        /// <returns>Result with errors or success</returns>
        Task<Result> AddStudent(Student student);
        
        /// <summary>
        /// Delete an existing student by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Result object indicating success, or containing errors/warnings</returns>
        Task<Result> DeleteStudent(int id);
        
        /// <summary>
        /// Update an existing student with new values
        /// </summary>
        /// <param name="student"></param>
        /// <returns>Result object indicating success, or containing errors/warnings</returns>
        Task<Result> UpdateStudent(Student student);
    }

    public class StudentRepository : IStudentRepository
    {
        private readonly EntityContext _context;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(EntityContext context, ILogger<StudentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<Student?> GetStudent(int id)
        {
            return await _context.Students
                .Where(x => x.StudentId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Student>> GetStudents() =>
            await _context.Students.ToListAsync();

        public async Task<Result> AddStudent(Student student)
        {
            var result = new Result();

            try
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
                result.AddError(ex.Message);
            }

            return result;
        }

        public async Task<Result> DeleteStudent(int id)
        {
            var result = new Result();
            var student = new Student { StudentId = id };

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
                result.AddError(ex.Message);
            }

            return result;
        }

        public async Task<Result> UpdateStudent(Student student)
        {
            var result = new Result();
            try
            {
                _context.Students.Update(student);
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