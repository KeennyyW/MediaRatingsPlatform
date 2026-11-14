using System;

namespace Mrp.Models;

public class MediaEntry
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string MediaType { get; set; }
    public int ReleaseYear { get; set; }
    public int AgeRestriction { get; set; }
    public int CreatorId { get; set; }
    public List<string> Genres { get; set; } = new List<string>();
         
}
