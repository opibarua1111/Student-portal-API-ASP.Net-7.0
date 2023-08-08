namespace Student_portal.API.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Class { get; set; } 
        public string? Age { get; set; } 
        public string? About { get; set; } 
        public string? Group { get; set; } 
        public string? District { get; set; } 
        public string? Contact { get; set; }
        public string? NoteFilePath { get; set; }
        public string? ImagePath { get; set; }
        public string? VoterIdPath { get; set; }
        public string? CertificatePath { get; set; }
        public string? CvPath { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
