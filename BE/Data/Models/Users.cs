using BE.Data.Dtos.PostDtos;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("Users")]
    public class Users
    {
        public Users()
        {
            UserCommenNavigations = new HashSet<PostComments>();
        }
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
        public int? IdGroup { get; set; }
        public int idBranch { get; set; }
        public string? rank { get; set; }
        //Date: 8/2/2023
        //Modifile: add field fullName in table User
        [NotMapped]
        public string FullName
        {
            get { return $"{lastName} {firstName}"; }
        }
        //Date: 9/3/2023
        //Modifile: add field avatarLink in table User
        public string? avatarLink { get; set; }
        public string? avatarName { get; set; }
        public string? avatarType { get; set; }
        public byte[]? avatarCode { get; set; }
        public int idUserGitLab { get; set; }
        public string tokenGitLab { get; set; }

        public ICollection<PostComments>? UserCommenNavigations { get; set; }
    }
}