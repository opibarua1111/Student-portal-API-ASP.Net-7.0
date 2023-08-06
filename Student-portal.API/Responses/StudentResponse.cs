using Student_portal.API.Models;
using System.Text.Json.Serialization;

namespace Student_portal.API.Responses
{
    public class StudentResponse: BaseResponse
    {
        public Student Student { get; internal set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Token { get; set; }
        public Guid Id { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string FirstName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LastName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Email { get; set; }
    }
}
