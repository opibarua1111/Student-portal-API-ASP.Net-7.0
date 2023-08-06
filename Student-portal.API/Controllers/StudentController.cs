using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_portal.API.Data;
using Student_portal.API.Interfaces;
using Student_portal.API.Models;
using Student_portal.API.Requests;
using Student_portal.API.Responses;

namespace Student_portal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentServices _studentService;
        public StudentController(IStudentServices studentService)
        {
            _studentService = studentService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<Student>> Register(AddStudentRequest addStudentRequest)
        {
            try
            {
                var studentEmailResponse  = await _studentService.GetStudentByEmailAsync(addStudentRequest.Email);

                if (!studentEmailResponse.Success)
                {
                    var studentResponse = await _studentService.RegisterStudentAsync(addStudentRequest);
                    if (studentResponse.Success)
                    {
                        return Ok(studentResponse);
                    }
                    return BadRequest(studentResponse);
                }
                return Ok("this email register before!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login(AddStudentRequest addStudentRequest)
        {
            try
            {
                var studentEmailResponse = await _studentService.GetStudentByEmailAsync(addStudentRequest.Email);

                if (studentEmailResponse.Success)
                {
                    var studentResponse = await _studentService.LoginStudentAsync(addStudentRequest, studentEmailResponse.Student);
                    if (!studentResponse.Success)
                    {
                        return BadRequest(studentResponse.Message);
                    }
                    if (studentResponse.Success)
                    {
                        return Ok(studentResponse);
                    }
                    return BadRequest("Something went wrong!");
                }
                return Ok("this email didn't find!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> GetStudent([FromRoute] Guid id)
        {
            try
            {
                var studentIdResponse = await _studentService.GetStudentByIdAsync(id);
                if (studentIdResponse.Success)
                {
                    return Ok(studentIdResponse.Student);
                }
                return Ok("This student id didn't find!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> UpdateStudent([FromRoute] Guid id, [FromForm] UpdateStudentRequest updateStudentRequest)
        {
            try
            {
                var studentIdResponse = await _studentService.GetStudentByIdAsync(id);

                if (studentIdResponse.Success)
                {
                    if(updateStudentRequest.Cv != null)
                    {
                        var studentFileResponse = await _studentService.StudentFileUploadAsync(updateStudentRequest, updateStudentRequest.Cv.FileName, "CV");
                    }
                    if(updateStudentRequest.NoteFile != null)
                    {
                        var studentFileResponse = await _studentService.StudentFileUploadAsync(updateStudentRequest, updateStudentRequest.NoteFile.FileName, "NoteFile");
                    }
                    if(updateStudentRequest.VoterId != null)
                    {
                        var studentFileResponse = await _studentService.StudentFileUploadAsync(updateStudentRequest, updateStudentRequest.VoterId.FileName, "VoterId");
                    }
                    if(updateStudentRequest.Certificate != null)
                    {
                        var studentFileResponse = await _studentService.StudentFileUploadAsync(updateStudentRequest, updateStudentRequest.Certificate.FileName, "Certificate");
                    }
                    var studentResponse = await _studentService.StudentUpdateAsync(studentIdResponse.Student, updateStudentRequest);
                    if (studentResponse.Success)
                    {
                        return Ok(studentResponse.Student);
                    }
                    return BadRequest("Something went wrong!");
                }
                return Ok("This student id didn't find!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteFile/{id:guid}")]
        [Authorize]
        public async Task<ActionResult> DeleteFile([FromRoute] Guid id, [FromQuery] StudentQueryParameters queryParameters) 
        {
            try
            {
                var studentIdResponse = await _studentService.GetStudentByIdAsync(id);
                if (studentIdResponse.Success)
                {
                    if (queryParameters.DeleteFileName != null)
                    {
                        var studentResponse = await _studentService.StudentFileDeleteAsync(studentIdResponse.Student, queryParameters);
                        if (studentResponse.Success)
                        {
                            return Ok(studentResponse.Message);
                        }
                        else
                        {
                            return Ok(studentResponse.Message);
                        }
                    }
                    return Ok("Please provide delete file name");
                }
                return Ok("This student id didn't find!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteAccount/{id:guid}")]
        [Authorize]
        public async Task<ActionResult> DeleteAccount([FromRoute] Guid id)
        {
            try
            {
                var studentIdResponse = await _studentService.GetStudentByIdAsync(id);
                if (studentIdResponse.Success)
                {
                    var studentResponse = await _studentService.DeleteAccountAsync(id, studentIdResponse.Student);
                    if (studentResponse.Success)
                    {
                        return Ok(studentResponse.Message);
                    }
                    return BadRequest("Something went wrong");
                }
                return BadRequest(studentIdResponse.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
