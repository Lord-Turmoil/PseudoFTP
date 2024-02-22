using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace PseudoFTP.Model.Database;

public class TransferHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    /// <summary>
    ///     True indicates successful transfer, false indicates failure or unfinished.
    /// </summary>
    public bool Status { get; set; } = false;

    [MaxLength(1024)]
    public string Destination { get; set; } = null!;

    public DateTime Started { get; set; }
    public DateTime? Completed { get; set; } = null;

    /// <summary>
    ///     Some message to describe the transfer.
    /// </summary>
    [MaxLength(1024)]
    public string? Message { get; set; } = null;

    public string? Error { get; set; } = null;

    public double Duration => Completed.HasValue ? (Completed.Value - Started).TotalSeconds : -1.0;
    public bool IsFinished => Completed.HasValue;
    public bool IsSuccessful => IsFinished && Status;
    public bool IsFailed => IsFinished && !Status;
}

public class TransferHistoryRepository : Repository<TransferHistory>
{
    public TransferHistoryRepository(PrimaryDbContext context) : base(context)
    {
    }
}