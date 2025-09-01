using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace ConnectorTool.Storage
{

	/// <summary>
	/// A static class that issues sample commands to a SchemaWrapper to demonstrate
	/// schema and data storage.
	/// </summary>
	/// 
	public static class StorageManager
	{

		#region Create new sample schemas and write data to them

		/// <summary>
		///  Creates a new sample Schema, creates an instance of that Schema (an Entity) in the given element,
		///  sets data on that element's entity, and exports the schema to a given XML file.
		/// </summary>
		/// <returns>A new SchemaWrapper</returns>
		public static SchemaWrapper CreateSet(Element storageElement, Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess, string vendorId, string applicationId, string name, string documentation)
		{

			#region Start a new transaction, and create a new Schema

			if (Schema.Lookup(schemaId) != null)
			{
				throw new Exception("A Schema with this Guid already exists in this document -- another one cannot be created.");
			}
			//Create a new schema.
			SchemaWrapper mySchemaWrapper = SchemaWrapper.NewSchema(schemaId, readAccess, writeAccess, vendorId, applicationId, name, documentation);
			#endregion
			//Create some schema fields.
			SchemaAndData(mySchemaWrapper, storageElement, out Entity storageElementEntityWrite);
			#region Store the main entity in an element, save the Serializeable SchemaWrapper to xml, and finish the transaction

			storageElement.SetEntity(storageElementEntityWrite);
			return mySchemaWrapper;
			#endregion
		}

		#region Helper methods for CreateSetAndExport

		/// <summary>
		/// Adds a simple fields, arrays, maps, subEntities, and arrays and maps of subEntities to a SchemaWrapper and Entity
		/// </summary>
		private static void SchemaAndData(SchemaWrapper mySchemaWrapper, Element storageElement, out Entity storageElementEntityWrite)
		{
			#region Add Fields to the SchemaWrapper

#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
			mySchemaWrapper.AddField<double>(doubleValue, SpecTypeId.Length, null);
			mySchemaWrapper.AddField<bool>(boolValue, SpecTypeId.Custom, null);
			mySchemaWrapper.AddField<string>(string0Name, SpecTypeId.Custom, null);
			mySchemaWrapper.AddField<ElementId>(idValue, SpecTypeId.Custom, null);
#else
			mySchemaWrapper.AddField<double>(doubleValue, UnitType.UT_Length, null);
			mySchemaWrapper.AddField<bool>(boolValue, UnitType.UT_Undefined, null);
			mySchemaWrapper.AddField<string>(string0Name, UnitType.UT_Undefined, null);
			mySchemaWrapper.AddField<ElementId>(idValue, UnitType.UT_Undefined, null);
#endif

			#endregion

			#region Populate the Schema in the SchemaWrapper with data

			mySchemaWrapper.FinishSchema();

			#endregion

			#region Create a new entity to store an instance of schema data

			storageElementEntityWrite = new Entity(mySchemaWrapper.GetSchema());

			#endregion

			#region Get fields and set data in them
			Field fieldDouble = mySchemaWrapper.GetSchema().GetField(doubleValue);
			Field fieldBool = mySchemaWrapper.GetSchema().GetField(boolValue);
			Field fieldString = mySchemaWrapper.GetSchema().GetField(string0Name);

			Field fieldId = mySchemaWrapper.GetSchema().GetField(idValue);

#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
			storageElementEntityWrite.Set(fieldDouble, 100.0, UnitTypeId.Millimeters);
#else
			storageElementEntityWrite.Set(fieldDouble, 100.0, DisplayUnitType.DUT_MILLIMETERS);
#endif
			storageElementEntityWrite.Set(fieldBool, false);
			storageElementEntityWrite.Set(fieldString, "hello");
			storageElementEntityWrite.Set(fieldId, storageElement.Id);

			#endregion
		}
		#endregion
		#endregion

		/// <summary>
		/// Given an element, try to find an entity containing instance data from a given Schema Guid.
		/// </summary>
		/// <param name="storageElement">The element to query</param>
		/// <param name="schemaId">The id of the Schema to query</param>
		public static void LookupAndExtractData(Element storageElement, Guid schemaId, out SchemaWrapper schemaWrapper)
		{
			Schema schemaLookup = Schema.Lookup(schemaId);
			if (schemaLookup == null)
			{
				throw new Exception("Schema not found in current document.");
			}
			schemaWrapper = SchemaWrapper.FromSchema(schemaLookup);

			Entity storageElementEntityRead = storageElement.GetEntity(schemaLookup);
			if (storageElementEntityRead.SchemaGUID != schemaId)
			{
				throw new Exception("SchemaID of found entity does not match the SchemaID passed to GetEntity.");
			}

			if (storageElementEntityRead == null)
			{
				throw new Exception("Entity of given Schema not found.");
			}
		}

		#region Helper methods
		/// <summary>
		/// Create a new pseudorandom Guid
		/// </summary>
		/// <returns></returns>
		public static Guid NewGuid()
		{
			byte[] guidBytes = new byte[16];
			Random randomGuidBytes = new Random(s_counter);
			randomGuidBytes.NextBytes(guidBytes);
			s_counter++;
			return new Guid(guidBytes);
		}
		#endregion

		#region Data

		//A counter field used to assist in creating pseudorandom Guids
		private static int s_counter = System.DateTime.Now.Second;

		//Field names and schema guids used in sample schemas
		readonly private static string doubleValue = "doubleValue";
		readonly private static string boolValue = "boolValue";
		readonly private static string string0Name = "stringValue";
		readonly private static string idValue = "idName";
		#endregion
	}
}
