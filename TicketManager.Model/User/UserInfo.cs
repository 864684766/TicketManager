using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.Model.User
{
    [Table("users")]
    public class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("sex")]
        public string? Sex { get; set; }

        [Column("age")]
        public int? Age { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("idCard")]
        public string? idCard { get; set; }

        [Column("icon")]
        public string? icon { get; set; }

        [Column("userPwd")]
        public string? userPwd { get; set; }

        [Column("account")]
        public string? account { get; set; }
        
    }
}
