using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Student_portal.API.Data;
using Student_portal.API.Interfaces;
using Student_portal.API.Models;
using Student_portal.API.Requests;
using Student_portal.API.Responses;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Student_portal.API.Services
{
    public class StudentServices : IStudentServices
    {
        private IConfiguration _configuration;
        public readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public StudentServices(IConfiguration configuration, ApplicationDBContext context, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _context = context;
            _environment = environment;
        }
        public async Task<StudentResponse> GetStudentByIdAsync(Guid id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student != null)
            {
                return new StudentResponse { Success = true, Student = student };
            }
            return new StudentResponse { Success = false, Message = "Didn't find student in this Id", Response = "false" };
        }
        public async Task<StudentResponse> GetStudentByEmailAsync(string email)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student != null)
            {
                return new StudentResponse { Success = true, Student = student };
            }
            return new StudentResponse { Success = false, Message = "this email didn't find!", Response = "false" };
        }

        public async Task<StudentResponse> RegisterStudentAsync(AddStudentRequest addStudentRequest)
        {
            CreatePasswordHash(addStudentRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var newStudent = new Student()
            {
                Id = Guid.NewGuid(),
                FirstName = addStudentRequest.FirstName,
                LastName = addStudentRequest.LastName,
                Email = addStudentRequest.Email,
                Contact = addStudentRequest.Contact,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };
            await _context.Students.AddAsync(newStudent);
            await _context.SaveChangesAsync();

            return new StudentResponse { Success = true, Message = "Registration successfull! Please Login." };
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<StudentResponse> LoginStudentAsync(LoginStudentRequest loginStudentRequest, Student student)
        {
            if (!VerifyPasswordHash(loginStudentRequest.Password, student.PasswordHash, student.PasswordSalt))
            {
                return new StudentResponse {Success= false, Message = "Wrong password!", Response = "false" };
            }

            var token = GenerateToken();
            return new StudentResponse { Success= true, Response = "true", Id = student.Id , Token = token, FirstName = student.FirstName, LastName = student.LastName, Email = student.Email };
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                    expires: DateTime.Now.AddDays(3),
                    signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<StudentResponse> StudentFileUploadAsync(UpdateStudentRequest updateStudentRequest, string fileName, string path)
        {
            var uniqueFileName = GetUniqueFileName(fileName);

            var uploads = Path.Combine(_environment.WebRootPath, path);

            var filePath = Path.Combine(uploads, uniqueFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (path == "CV")
            {
                await updateStudentRequest.Cv.CopyToAsync(new FileStream(filePath, FileMode.Create));

                var newFilePath = Path.Combine(path, uniqueFileName);
                updateStudentRequest.CvPath = newFilePath;
                return new StudentResponse { Success = true };
            }
            else if (path == "Image")
            {
                await updateStudentRequest.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                var newFilePath = Path.Combine(path, uniqueFileName);
                updateStudentRequest.ImagePath = newFilePath;
                return new StudentResponse { Success = true };
            }
            else if(path == "NoteFile")
            {
                await updateStudentRequest.NoteFile.CopyToAsync(new FileStream(filePath, FileMode.Create));
                var newFilePath = Path.Combine(path, uniqueFileName);
                updateStudentRequest.NoteFilePath = newFilePath;
                return new StudentResponse { Success = true };
            }
            else if (path == "VoterId")
            {
                await updateStudentRequest.VoterId.CopyToAsync(new FileStream(filePath, FileMode.Create));
                var newFilePath = Path.Combine(path, uniqueFileName);
                updateStudentRequest.VoterIdPath = newFilePath;
                return new StudentResponse { Success = true };
            } 
            else if (path == "Certificate")
            {
                await updateStudentRequest.Certificate.CopyToAsync(new FileStream(filePath, FileMode.Create));
                var newFilePath = Path.Combine(path, uniqueFileName);
                updateStudentRequest.CertificatePath = newFilePath;
                return new StudentResponse { Success = true };
            }

            return new StudentResponse {Success = false, Message = "Something went wrong!" };
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return string.Concat(Path.GetFileNameWithoutExtension(fileName)
                                , "_"
                                , Guid.NewGuid().ToString().AsSpan(0, 4)
                                , Path.GetExtension(fileName));
        }

        public async Task<StudentResponse> StudentUpdateAsync(Student student, UpdateStudentRequest updateStudentRequest)
        {
            student.FirstName = updateStudentRequest.FirstName;
            student.LastName = updateStudentRequest.LastName;
            student.Email = updateStudentRequest.Email;
            student.Contact = updateStudentRequest.Contact;
            student.Class = updateStudentRequest.Class;
            student.Age = updateStudentRequest.Age;
            student.About = updateStudentRequest.About;
            student.Group = updateStudentRequest.Group;
            student.District = updateStudentRequest.District;
            student.CvPath = updateStudentRequest.CvPath;
            student.ImagePath = updateStudentRequest.ImagePath;
            student.NoteFilePath = updateStudentRequest.NoteFilePath;
            student.CertificatePath = updateStudentRequest.CertificatePath;
            student.VoterIdPath = updateStudentRequest.VoterIdPath;
            student.NoteFilePath = updateStudentRequest.NoteFilePath;
            student.NoteFilePath = updateStudentRequest.NoteFilePath;
            student.NoteFilePath = updateStudentRequest.NoteFilePath;
            await _context.SaveChangesAsync();
            return new StudentResponse { Success = true, Message = "Student info update successfully", Response = "true" };
        }

        public async Task<StudentResponse> StudentFileDeleteAsync(Student student, [FromQuery] StudentQueryParameters queryParameters)
        {
            if (queryParameters.DeleteFileName.ToLower() == "cv")
            {
                if (string.IsNullOrWhiteSpace(student.CvPath))
                {
                    return new StudentResponse { Success = false, Message = "Your CV already null", Response = "false" };
                }
                student.CvPath = null;
                await _context.SaveChangesAsync();
                return new StudentResponse { Success = true, Message = "Your CV Deleted SuccessFully", Response = "true" };
            }
            else if (queryParameters.DeleteFileName.ToLower() == "image")
            {
                if (string.IsNullOrWhiteSpace(student.ImagePath))
                {
                    return new StudentResponse { Success = false, Message = "Your NoteFile already null", Response = "false" };
                }
                student.ImagePath = null;
                await _context.SaveChangesAsync();
                return new StudentResponse { Success = true, Message = "Your image Deleted SuccessFully", Response = "true" };
            }
            else if (queryParameters.DeleteFileName.ToLower() == "notefile")
            {
                if (string.IsNullOrWhiteSpace(student.NoteFilePath))
                {
                    return new StudentResponse { Success = false, Message = "Your NoteFile already null", Response = "false" };
                }
                student.NoteFilePath = null;
                await _context.SaveChangesAsync();
                return new StudentResponse { Success = true, Message = "Your note file Deleted SuccessFully", Response = "true" };
            }
            else if (queryParameters.DeleteFileName.ToLower() == "voterid")
            {
                if (string.IsNullOrWhiteSpace(student.VoterIdPath))
                {
                    return new StudentResponse { Success = false, Message = "Your VoterId already null", Response = "false" };
                }
                student.VoterIdPath = null;
                await _context.SaveChangesAsync();
                return new StudentResponse { Success = true, Message = "Your voterId Deleted SuccessFully", Response = "true" };
            }
            else if (queryParameters.DeleteFileName.ToLower() == "certificate")
            {
                if (string.IsNullOrWhiteSpace(student.CertificatePath))
                {
                    return new StudentResponse { Success = false, Message = "Your Certificate already null", Response = "false" };
                }
                student.CertificatePath = null;
                await _context.SaveChangesAsync();
                return new StudentResponse { Success = true, Message = "Your certificate Deleted SuccessFully", Response = "true" };
            }
            else if (queryParameters.DeleteFileName.ToLower() == "all")
            {
                if (string.IsNullOrWhiteSpace(student.CertificatePath) &&
                    string.IsNullOrWhiteSpace(student.VoterIdPath) &&
                    string.IsNullOrWhiteSpace(student.NoteFilePath) &&
                    string.IsNullOrWhiteSpace(student.CvPath)
                    )
                {
                    return new StudentResponse { Success = false, Message = "Your All file already null", Response = "false" };
                }
                student.CertificatePath = null;
                student.VoterIdPath = null;
                student.NoteFilePath = null;
                student.CvPath = null;
                await _context.SaveChangesAsync();
                return new StudentResponse { Success = true, Message = "Your All file Deleted SuccessFully", Response = "true" };
            }
            return new StudentResponse { Success = false, Message = "Your filename didn't match", Response = "false" };
        }

        public async Task<StudentResponse> DeleteAccountAsync(Guid id, Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return new StudentResponse { Success = true, Message = "Account deleted Successfull", Response = "true" };
        }
    }
}
