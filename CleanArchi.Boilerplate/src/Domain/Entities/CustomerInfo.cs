
using SqlSugar;
namespace CleanArchi.Boilerplate.Domain.Entities;
///<summary>
///CustomerInfo
///</summary>
[SugarTable("CustomerInfo")]	

public class CustomerInfo : RootEntityTkey<int> 
{
   
	/// <summary>
    /// PrimaryCustomerCode
    /// </summary>
	public string PrimaryCustomerCode { get; set; }
      
	/// <summary>
    /// Plant
    /// </summary>
	public string Plant { get; set; }
   
	/// <summary>
    /// DCHL
    /// </summary>
	public int? DCHL { get; set; }
   
	/// <summary>
    /// AllowSplitSLnumber
    /// </summary>
	public int? AllowSplitSLnumber { get; set; }
   
	/// <summary>
    /// AllowedSL1
    /// </summary>
	public string AllowedSL1 { get; set; }
   
	/// <summary>
    /// AllowedSL2
    /// </summary>
	public string AllowedSL2 { get; set; }
   
	/// <summary>
    /// AllowedSL3
    /// </summary>
	public string AllowedSL3 { get; set; }
   
	/// <summary>
    /// AllowedSL4
    /// </summary>
	public string AllowedSL4 { get; set; }
   
	/// <summary>
    /// AllowedSL5
    /// </summary>
	public string AllowedSL5 { get; set; }
 
}
 
