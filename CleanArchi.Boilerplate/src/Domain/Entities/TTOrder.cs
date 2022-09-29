
using SqlSugar;
namespace CleanArchi.Boilerplate.Domain.Entities;
///<summary>
///TTOrder
///</summary>
[SugarTable("TTOrder")]	

public class TTOrder
{
   
	/// <summary>
    /// Date
    /// </summary>
	public DateTime? Date { get; set; }
   
	/// <summary>
    /// ItemId
    /// </summary>
	public string ItemId { get; set; }
   
	/// <summary>
    /// OrderType
    /// </summary>
	public string OrderType { get; set; }
   
	/// <summary>
    /// OrderNumber
    /// </summary>
	public string OrderNumber { get; set; }
   
	/// <summary>
    /// CustomerName
    /// </summary>
	public string CustomerName { get; set; }
   
	/// <summary>
    /// CustomerNumber
    /// </summary>
	public string CustomerNumber { get; set; }
   
	/// <summary>
    /// ShipToNumber
    /// </summary>
	public string ShipToNumber { get; set; }
   
	/// <summary>
    /// ShipToAddress
    /// </summary>
	public string ShipToAddress { get; set; }
   
	/// <summary>
    /// Items
    /// </summary>
	public int? Items { get; set; }
   
	/// <summary>
    /// SKUDescription
    /// </summary>
	public string SKUDescription { get; set; }
   
	/// <summary>
    /// SKUCode
    /// </summary>
	public string SKUCode { get; set; }
   
	/// <summary>
    /// ChangeSKUCode
    /// </summary>
	public string ChangeSKUCode { get; set; }
   
	/// <summary>
    /// OrderQTY
    /// </summary>
	public int? OrderQTY { get; set; }
   
	/// <summary>
    /// PlantID
    /// </summary>
	public string PlantID { get; set; }
   
	/// <summary>
    /// StorageLocation
    /// </summary>
	public string StorageLocation { get; set; }
   
	/// <summary>
    /// Batch
    /// </summary>
	public string Batch { get; set; }
   
	/// <summary>
    /// SAPOrderNumber
    /// </summary>
	public string SAPOrderNumber { get; set; }
   
	/// <summary>
    /// PONumber
    /// </summary>
	public string PONumber { get; set; }
   
	/// <summary>
    /// Barcode
    /// </summary>
	public string Barcode { get; set; }
   
	/// <summary>
    /// RequestedQuantity
    /// </summary>
	public int? RequestedQuantity { get; set; }
   
	/// <summary>
    /// FulFilledQuantity
    /// </summary>
	public int? FulFilledQuantity { get; set; }
   
	/// <summary>
    /// Price
    /// </summary>
	public decimal? Price { get; set; }
   
	/// <summary>
    /// FulFilledPrice
    /// </summary>
	public decimal? FulFilledPrice { get; set; }
 
}
 
