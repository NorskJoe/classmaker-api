using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using classmaker_models.Dtos;
using classmaker_repositories;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace classmaker_services.Services
{
    public interface IFileUploadService
    {
        /// <summary>
        /// Upload a csv file from a stream.  Parse students and save them to the database
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<Result> UploadFile(Stream file);
    }
    
    public class FileUploadService : IFileUploadService
    {
        private readonly ILogger<FileUploadService> _logger;
        private readonly IStudentRepository _studentRepository;

        public FileUploadService(ILogger<FileUploadService> logger,
            IStudentRepository studentRepository)
        {
            _logger = logger;
            _studentRepository = studentRepository;
        }
        
        public async Task<Result> UploadFile(Stream file)
        {
            var result = new Result();
            try
            {
                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var studentRecords = csv.GetRecords<StudentFileDto>();
                    // Save students to DB
                    var students = studentRecords.Select(StudentDtoHelper.DtoToStudentMap);
                    await _studentRepository.AddStudents(students);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in file upload: {e.Message}");
                result.AddError($"Error in file upload: {e.Message}");
            }

            return result;
        }
    }
}