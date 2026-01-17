namespace Application.Ophthalmologists.Queries.GetOphthalmologist;

public class OphthalmologistDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
