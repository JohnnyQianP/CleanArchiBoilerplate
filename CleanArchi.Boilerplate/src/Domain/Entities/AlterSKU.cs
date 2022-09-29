
using SqlSugar;
namespace CleanArchi.Boilerplate.Domain.Entities;
///<summary>
///AlterSKU
///</summary>
[SugarTable("AlterSKU")]	

public class AlterSKU : RootEntityTkey<long> 
{
      
	/// <summary>
    /// Barcode
    /// </summary>
	public string Barcode { get; set; }
   
	/// <summary>
    /// MaterialA
    /// </summary>
	public string MaterialA { get; set; }
   
	/// <summary>
    /// DescriptionA
    /// </summary>
	public string DescriptionA { get; set; }
   
	/// <summary>
    /// MaterialB
    /// </summary>
	public string MaterialB { get; set; }
   
	/// <summary>
    /// DescriptionB
    /// </summary>
	public string DescriptionB { get; set; }
   
	/// <summary>
    /// MaterialC
    /// </summary>
	public string MaterialC { get; set; }
   
	/// <summary>
    /// DescriptionC
    /// </summary>
	public string DescriptionC { get; set; }
   
	/// <summary>
    /// MaterialD
    /// </summary>
	public string MaterialD { get; set; }
   
	/// <summary>
    /// DescriptionD
    /// </summary>
	public string DescriptionD { get; set; }
 
}
 
