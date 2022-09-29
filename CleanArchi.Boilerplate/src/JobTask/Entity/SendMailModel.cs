using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchi.Boilerplate.JobTask.Entity;

public class SendMailModel
{
    public string Title { get; set; }
    public string Content { get; set; }
    public MailEntity MailInfo { get; set; } = null;
}