using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformWellSync.Models;

public class Well
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public int PlatformId { get; set; }
    public Platform? Platform { get; set; }

    public string? UniqueName { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}