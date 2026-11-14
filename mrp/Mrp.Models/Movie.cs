using System;

namespace Mrp.Models;

public class Movie 
{
    public Movie() : base() {}
    public int ageRestriction {get; set;}
    public int runtime {get; set;}
    public string regisseur {get; set;}
}
