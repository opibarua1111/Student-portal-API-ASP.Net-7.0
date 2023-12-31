﻿using System.ComponentModel.DataAnnotations;

namespace Student_portal.API.Requests
{
    public class UpdateStudentRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Contact { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public string Age { get; set; }
        [Required]
        public string About { get; set; }
        [Required]
        public string Group { get; set; }
        [Required]
        public string District { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? NoteFile { get; set; }
        public IFormFile? VoterId { get; set; }
        public IFormFile? Certificate { get; set; }
        public IFormFile? Cv { get; set; }
        public string? ImagePath { get; set; }
        public string? NoteFilePath { get; set; }
        public string? VoterIdPath { get; set; }
        public string? CertificatePath { get; set; }
        public string? CvPath { get; set; }
    }
}
