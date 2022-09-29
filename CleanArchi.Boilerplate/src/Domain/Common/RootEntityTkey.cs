using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace CleanArchi.Boilerplate.Domain.Common;

public class RootEntityTkey<Tkey> where Tkey : IEquatable<Tkey>
{
    /// <summary>
    /// ID
    /// 泛型主键Tkey
    /// </summary>
    [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
    public Tkey Id { get; set; }


}
