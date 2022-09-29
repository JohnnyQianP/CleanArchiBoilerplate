
using SqlSugar;
namespace CleanArchi.Boilerplate.Domain.Entities;
///<summary>
///OrderList
///</summary>
[SugarTable("OrderList")]	

public class OrderList : RootEntityTkey<long> 
{
      
	/// <summary>
    /// 送货方代码-Associate Customer Code即副客户代码
    /// </summary>
	public int? ShipToNumber { get; set; }
   
	/// <summary>
    /// PONumber
    /// </summary>
	public string PONumber { get; set; }
   
	/// <summary>
    /// 项数
    /// </summary>
	public int? Items { get; set; }
   
	/// <summary>
    /// Barcode
    /// </summary>
	public string Barcode { get; set; }
   
	/// <summary>
    /// SKUCode
    /// </summary>
	public int? SKUCode { get; set; }
   
	/// <summary>
    /// SKUDescription
    /// </summary>
	public string SKUDescription { get; set; }
   
	/// <summary>
    /// ChangeSKUCode
    /// </summary>
	public string ChangeSKUCode { get; set; }
   
	/// <summary>
    /// OrderQTY
    /// </summary>
	public int? OrderQTY { get; set; }
   
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
    /// BatchQTY
    /// </summary>
	public string BatchQTY { get; set; }
   
	/// <summary>
    /// Flag
    /// </summary>
	public string Flag { get; set; }
   
	/// <summary>
    /// OrderType
    /// </summary>
	public string OrderType { get; set; }
   
	/// <summary>
    /// SoldToNumber
    /// </summary>
	public string SoldToNumber { get; set; }
   
	/// <summary>
    /// CustomerName
    /// </summary>
	public string CustomerName { get; set; }
   
	/// <summary>
    /// ShipToAddress
    /// </summary>
	public string ShipToAddress { get; set; }
   
	/// <summary>
    /// Date
    /// </summary>
	public DateTime? Date { get; set; }
   
	/// <summary>
    /// Status
    /// </summary>
	public string Status { get; set; }
   
	/// <summary>
    /// FullfiledOrNot
    /// </summary>
	public string FullfiledOrNot { get; set; }
   
	/// <summary>
    /// AllocationDate
    /// </summary>
	public DateTime? AllocationDate { get; set; }
   
	/// <summary>
    /// AllocateQTY
    /// </summary>
	public int? AllocateQTY { get; set; }
   
	/// <summary>
    /// IsAgingAllocate
    /// </summary>
	public string IsAgingAllocate { get; set; }
   
	/// <summary>
    /// AgingMissingQTY
    /// </summary>
	public int? AgingMissingQTY { get; set; }
   
	/// <summary>
    /// SKULevel
    /// </summary>
	public int? SKULevel { get; set; }
   
	/// <summary>
    /// RequestedQTY
    /// </summary>
	public int? RequestedQTY { get; set; }
   
	/// <summary>
    /// Price
    /// </summary>
	public decimal? Price { get; set; }
   
	/// <summary>
    /// FulFilledQTY
    /// </summary>
	public int? FulFilledQTY { get; set; }
   
	/// <summary>
    /// FulFilledPrice
    /// </summary>
	public decimal? FulFilledPrice { get; set; }
   
	/// <summary>
    /// SerialNumber
    /// </summary>
	[SugarColumn(IsNullable = true)]
	public int SerialNumber { get; set; }
   
	/// <summary>
    /// IsContainsAgingBatch
    /// </summary>
	public string IsContainsAgingBatch { get; set; }
 
}
 
