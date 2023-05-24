using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Dtos.UserDtos
{
    public class ListUserDtos
    {
        public int id { get; set; }
        public string userCode { get; set; }
        public string userPassword { get; set; }
        public int? userCreated { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? userModified { get; set; }
        public DateTime? dateModified { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime? dOB { get; set; }
        public byte? gender { get; set; }
        public string? address { get; set; }
        public string? university { get; set; }
        public int? yearGraduated { get; set; }
        public string email { get; set; }
        public string? emailPassword { get; set; }
        public string? skype { get; set; }
        public string? skypePassword { get; set; }
        public byte workStatus { get; set; }
        public DateTime dateStartWork { get; set; }
        public DateTime? dateLeave { get; set; }
        public byte? maritalStatus { get; set; }
        public string? reasonResignation { get; set; }
        public string identitycard { get; set; }
        public byte isDeleted { get; set; }
        public string? refreshToken { get; set; }
        public DateTime? refreshTokenExpiryTime { get; set; }
        public string? idGroup { get; set; }
        public string fullName { get; set; }
        public string? avatarLink { get; set; }
        public string  fullNameCreate { get; set; }
        public string fullNameModifier { get; set; }
        public int idBranch { get; set; }
        public string nameBranch { get; set; }
        public string? rank { get; set; }
        public int idUserGitLab { get; set; }
        public string tokenGitLab { get; set; }
    }
}
