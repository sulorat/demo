using System;
using System.Collections.Generic;

namespace demo.EntityModels;

public partial class Gender
{
    public int Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
