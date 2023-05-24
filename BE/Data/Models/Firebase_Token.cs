using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    public class Firebase_Token
    {
        public Firebase_Token() { }

        public Firebase_Token(string userName, string token)
        {
            UserName = userName;
            Token = token;
        }

        public int? Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }

    }
}
