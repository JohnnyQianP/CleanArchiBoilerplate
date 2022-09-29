
using SqlSugar;
namespace CleanArchi.Boilerplate.Domain.Entities;
///<summary>
///CustomerInfoForImport
///</summary>
[SugarTable("CustomerInfoForImport")]	

public class CustomerInfoForImport
{
   
	/// <summary>
    /// PrimaryCustomerCode
    /// </summary>
	public string PrimaryCustomerCode { get; set; }
   
	/// <summary>
    /// AssociateCustomerCode
    /// </summary>
	public int? AssociateCustomerCode { get; set; }
   
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
 
