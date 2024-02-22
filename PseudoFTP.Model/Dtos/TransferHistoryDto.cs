namespace PseudoFTP.Model.Dtos;

public class TransferHistoryDto
{
    public int Id { get; set; }
    public bool Status { get; set; }

    public string Destination { get; set; } = null!;

    public DateTime Started { get; set; }
    public DateTime? Completed { get; set; } = null;

    public string? Message { get; set; } = null;
    public string? Error { get; set; } = null;

    public double Duration { get; set; }
    public bool IsFinished { get; set; }
    public bool IsSuccessful { get; set; }
    public bool IsFailed { get; set; }
}