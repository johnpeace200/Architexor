using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ConnectorTool.Object;

namespace ConnectorTool.Storage
{
	public static class StorageData
	{
		/// <summary>
		/// Save data of connector to the given primary element
		/// </summary>
		/// <returns></returns>
		public static bool SetDataInElement(List<TConnector> connectors, ElementId priElemId, Document doc)
		{
			Element priElement = doc.GetElement(priElemId);
			Transaction setDataTrans = new Transaction(priElement.Document);
			setDataTrans.Start("SetDataInPriElement");
			SchemaBuilder schemaBuilder = new SchemaBuilder(new Guid("720080CB-DA99-40DC-9415-E53F280AA1F0"));
			schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
			schemaBuilder.SetWriteAccessLevel(AccessLevel.Vendor);
			schemaBuilder.SetVendorId("ADSK");
			schemaBuilder.SetSchemaName("SetData");

			//Create a field to store an connector data
#if REVIT2024 || REVIT2025
			float elemId = priElemId.Value;
#else
			float elemId = priElemId.IntegerValue;
#endif
			FieldBuilder fieldBuilder = schemaBuilder.AddSimpleField("ConnectorData", typeof(float));
#if REVIT2019 || REVIT2020
			fieldBuilder.SetUnitType(UnitType.UT_Length);
#else
			fieldBuilder.SetSpec(SpecTypeId.Length);
#endif
			fieldBuilder.SetDocumentation("Stored data of TConnector that placed in primary element.");

			Schema schema = schemaBuilder.Finish();	 //register the schema object
			Entity entity = new Entity(schema);
			//Get field from the schema
			Field fieldTConnector = schema.GetField("ConnectorData");
			//Set data for this entity
			entity.Set<List<TConnector>>(fieldTConnector, connectors);
			priElement.SetEntity(entity);	   //Store the entity in element

			if (setDataTrans.Commit() == TransactionStatus.Committed)
			{
				return true;
			}
			else
			{
				setDataTrans.RollBack();
				return false;
			}
				
		}

		/// <summary>
		/// Get data of connector to the given primary element
		/// </summary>
		/// <returns></returns>
		public static List<TConnector> GetDatainElement(ElementId priElemId, Document doc)
		{
			List<TConnector> connectors = new List<TConnector>();

			Element priElement = doc.GetElement(priElemId);
			Transaction getDataTrans = new Transaction(priElement.Document);
			getDataTrans.Start("SetDataInPriElement");
			SchemaBuilder schemaBuilder = new SchemaBuilder(new Guid("720080CB-DA99-40DC-9415-E53F280AA1F0"));
			schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
			schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
			schemaBuilder.SetSchemaName("SetData");

			//Create a field to store an connector data
			FieldBuilder fieldBuilder = schemaBuilder.AddArrayField("ConnectorData", typeof(TConnector));
#if REVIT2019 || REVIT2020
			fieldBuilder.SetUnitType(UnitType.UT_Number);
#else
			fieldBuilder.SetSpec(SpecTypeId.Number);
#endif
			fieldBuilder.SetDocumentation("Stored data of TConnector that placed in primary element.");

			Schema schema = schemaBuilder.Finish();	 //register the schema object
			Entity entity = new Entity(schema);
			//Get field from the schema
			Field fieldTConnector = schema.GetField("ConnectorData");

			//Get the connector data back from the element
			Entity retrieveEntity = priElement.GetEntity(schema);
			connectors = retrieveEntity.Get<List<TConnector>>(schema.GetField("ConnectorData"));

			getDataTrans.Commit();

			return connectors;
		}
	}
}
