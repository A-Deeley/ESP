using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
