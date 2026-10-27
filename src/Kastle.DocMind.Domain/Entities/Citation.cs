using System.Text.Json.Serialization;

namespace Kastle.DocMind.Domain.Entities;

public class Citation
{
    [JsonPropertyName("marker")]
    public int Marker { get; set; }

    public Guid DocumentId { get; set; }

    public Guid ChunkId { get; set; }

    [JsonPropertyName("filename")]
    public string? FileName { get; set; }

    public string? Section { get; set; }
}