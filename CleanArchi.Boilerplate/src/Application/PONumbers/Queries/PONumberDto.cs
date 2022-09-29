using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchi.Boilerplate.Application.PONumbers.Queries;

public class PONumberDto
{
    public int Id { get; set; }
    public string PONumber { get; set; } 
    public int ShipToNumber { get; set; }

    public string AllowedSL_1 { get; set; }
    public string AllowedSL_2 { get; set; }
    public string AllowedSL_3 { get; set; }
    public string AllowedSL_4 { get; set; }
    public string AllowedSL_5 { get; set; }
    public string ClientExist { get; set; }
    public int DCHL { get; set; }
    public string SplittedSL { get; set; }
    public int Splitted { get; set; }
    public int AllowedSplit { get; set; }
    public int SerialNumber { get; set; }
}
