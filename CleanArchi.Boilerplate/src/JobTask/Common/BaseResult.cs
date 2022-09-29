using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchi.Boilerplate.JobTask.Common;

public class BaseResult
{
    public int Code { get; set; } = 200;
    public string Msg { get; set; }
}
