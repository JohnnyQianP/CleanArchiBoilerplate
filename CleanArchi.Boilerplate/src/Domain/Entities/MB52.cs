
using SqlSugar;
namespace CleanArchi.Boilerplate.Domain.Entities;
///<summary>
///MB52
///</summary>
[SugarTable("MB52")]	

public class MB52 : RootEntityTkey<int> 
{
      
	/// <summary>
    /// 物料-SKUCode
    /// </summary>
	public int? Material { get; set; }
   
	/// <summary>
    /// SKUDescription
    /// </summary>
	public string SKUDescription { get; set; }
   
	/// <summary>
    /// 工厂
    /// </summary>
	public string Plant { get; set; }
   
	/// <summary>
    /// 库位
    /// </summary>
	public string StorageLocation { get; set; }
   
	/// <summary>
    /// 批次
    /// </summary>
	public string Batch { get; set; }
   
	/// <summary>
    /// 库存数
    /// </summary>
	public int? StorageQTY { get; set; }
   
	/// <summary>
    /// 该批次是否临期
    /// </summary>
	public string IsAging { get; set; }
 
}
 
