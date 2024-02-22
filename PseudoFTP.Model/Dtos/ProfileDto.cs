namespace PseudoFTP.Model.Dtos;

public class ProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Destination { get; set; } = null!;
}

public class AddProfileDto
{
    public string Name { get; set; } = null!;
    public string Destination { get; set; } = null!;
}
