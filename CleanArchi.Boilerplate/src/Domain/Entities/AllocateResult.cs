
using SqlSugar;
namespace CleanArchi.Boilerplate.Domain.Entities;
///<summary>
///AllocateResult
///</summary>
[SugarTable("AllocateResult")]	

public class AllocateResult : RootEntityTkey<int> 
{
      
	/// <summary>
    /// Flag
    /// </summary>
	[SugarColumn(Length = 10, IsNullable = true)]
	public string Flag { get; set; }
   
	/// <summary>
    /// SerialNumber
    /// </summary>
	[SugarColumn(IsNullable = true)]
	public int SerialNumber { get; set; }
   
	/// <summary>
    /// Status
    /// </summary>
	[SugarColumn(Length = 20, IsNullable = true)]
	public string Status { get; set; }
   
	/// <summary>
    /// OrderItems
    /// </summary>
	[SugarColumn(IsNullable = true)]
	public int OrderItems { get; set; }
   
	/// <summary>
    /// ExecElapsedTime
    /// </summary>
	[SugarColumn(IsNullable = true)]
	public int ExecElapsedTime { get; set; }
   
	/// <summary>
    /// CompleteTime
    /// </summary>
	[SugarColumn(IsNullable = true)]
	public DateTime CompleteTime { get; set; }
   
	/// <summary>
    /// Allocated
    /// </summary>
	[SugarColumn(IsNullable = true)]
	public int Allocated { get; set; }
   
	/// <summary>
    /// NotAllocated
    /// </summary>
	[SugarColumn(IsNullable = true)]
	public int NotAllocated { get; set; }
 
}
 
