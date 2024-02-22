using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace PseudoFTP.Model.Database;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public ICollection<Profile> Profiles { get; set; }
}

public class UserRepository : Repository<User>
{
    public UserRepository(PrimaryDbContext context) : base(context)
    {
    }
}