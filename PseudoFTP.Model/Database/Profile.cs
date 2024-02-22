using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace PseudoFTP.Model.Database;

/// <summary>
///     For pre-configured profiles.
/// </summary>
public class Profile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    /// <summary>
    ///     Name of the profile. Must be unique.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The destination path.
    /// </summary>
    public string Destination { get; set; } = null!;
}

public class ProfileRepository : Repository<Profile>
{
    public ProfileRepository(PrimaryDbContext context) : base(context)
    {
    }
}