
using SqlSugar;
namespace CleanArchi.Boilerplate.Domain.Entities;
///<summary>
///PONumber
///</summary>
[SugarTable("PONumber")]	

public class PONumberTest : RootEntityTkey<int> 
{
      
	/// <summary>
    /// PONumber
    /// </summary>
	public string PONumber { get; set; }
   
	/// <summary>
    /// ShipToNumber
    /// </summary>
	public int? ShipToNumber { get; set; }
   
	/// <summary>
    /// AllowedSL_1
    /// </summary>
	public string AllowedSL_1 { get; set; }
   
	/// <summary>
    /// AllowedSL_2
    /// </summary>
	public string AllowedSL_2 { get; set; }
   
	/// <summary>
    /// AllowedSL_3
    /// </summary>
	public string AllowedSL_3 { get; set; }
   
	/// <summary>
    /// AllowedSL_4
    /// </summary>
	public string AllowedSL_4 { get; set; }
   
	/// <summary>
    /// AllowedSL_5
    /// </summary>
	public string AllowedSL_5 { get; set; }
   
	/// <summary>
    /// ClientExist
    /// </summary>
	public string ClientExist { get; set; }
   
	/// <summary>
    /// DCHL
    /// </summary>
	public int? DCHL { get; set; }
   
	/// <summary>
    /// 订单已分配库位（举例：FG01,FG02,FG03）
    /// </summary>
	public string SplittedSL { get; set; }
   
	/// <summary>
    /// 已拆分库位数
    /// </summary>
	public int? Splitted { get; set; }
   
	/// <summary>
    /// 允许拆分库位数
    /// </summary>
	public int? AllowedSplit { get; set; }
   
	/// <summary>
    /// SerialNumber
    /// </summary>
	[SugarColumn(IsNullable = true)]
	public int SerialNumber { get; set; }
 
}
 
