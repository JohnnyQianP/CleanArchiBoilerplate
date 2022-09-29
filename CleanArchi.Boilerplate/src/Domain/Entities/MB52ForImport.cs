
using SqlSugar;
namespace CleanArchi.Boilerplate.Domain.Entities;
///<summary>
///MB52ForImport
///</summary>
[SugarTable("MB52ForImport")]	

public class MB52ForImport
{
   
	/// <summary>
    /// Material
    /// </summary>
	public string Material { get; set; }
   
	/// <summary>
    /// SKUDescription
    /// </summary>
	public string SKUDescription { get; set; }
   
	/// <summary>
    /// Plant
    /// </summary>
	public string Plant { get; set; }
   
	/// <summary>
    /// StorageLocation
    /// </summary>
	public string StorageLocation { get; set; }
   
	/// <summary>
    /// Batch
    /// </summary>
	public string Batch { get; set; }
   
	/// <summary>
    /// StorageQTY
    /// </summary>
	public int? StorageQTY { get; set; }
 
}
 
