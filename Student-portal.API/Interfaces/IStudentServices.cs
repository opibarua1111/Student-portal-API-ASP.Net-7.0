using Microsoft.AspNetCore.Mvc;
using Student_portal.API.Models;
using Student_portal.API.Requests;
using Student_portal.API.Responses;

namespace Student_portal.API.Interfaces
{
    public interface IStudentServices
    {
        Task<StudentResponse> RegisterStudentAsync(AddStudentRequest addStudentRequest);
        Task<StudentResponse> LoginStudentAsync(AddStudentRequest addStudentRequest, Student student);
        Task<StudentResponse> StudentFileUploadAsync( UpdateStudentRequest updateStudentRequest, string fileName, string path);
        Task<StudentResponse> StudentUpdateAsync( Student student, UpdateStudentRequest updateStudentRequest);
        Task<StudentResponse> StudentFileDeleteAsync( Student student, [FromQuery] StudentQueryParameters queryParameters);
        Task<StudentResponse> GetStudentByEmailAsync(string email);
        Task<StudentResponse> GetStudentByIdAsync(Guid id);
        Task<StudentResponse> DeleteAccountAsync(Guid id, Student student);
    }
}
