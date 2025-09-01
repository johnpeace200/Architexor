using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System.Reflection;
using System.Diagnostics;

namespace ConnectorTool.Storage
{
	/// <summary>
	/// This class provides a simpler level of access to the
	/// Autodesk.Revit.DB.ExtensibleStorage Schema and SchemaBuilder objects
	/// </summary>
	[Serializable]
	public class SchemaWrapper
	{
		#region Constructors and class Factories
		/// <summary>
		/// For serialization only -- Do not use.
		/// </summary>
		internal SchemaWrapper() { }

		/// <summary>
		/// Creates a new SchemaWrapper from an existing schema.  
		/// </summary>
		/// <param name="schema">A schema to create a wrapper from</param>
		/// <returns>A new SchemaWrapper containing all data in the schema</returns>
		public static SchemaWrapper FromSchema(Schema schema)
		{
			SchemaWrapper swReturn = new SchemaWrapper(schema);

			foreach (Field currentField in schema.ListFields())
			{
				//We need to add call AddField on the SchemaWrapper we are creating for each field in the source schema.
				//Since we do not know the data type of the field yet, we need to get the generic method first and
				//then query the field data types from the field and instantiate a new generic method with those types as parameters.

				//Get the "AddField" method.
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
				MethodInfo addFieldmethod = typeof(SchemaWrapper).GetMethod("AddField", new Type[] { typeof(string), typeof(ForgeTypeId), typeof(SchemaWrapper) });
#else
				MethodInfo addFieldmethod = typeof(SchemaWrapper).GetMethod("AddField", new Type[] { typeof(string), typeof(UnitType), typeof(SchemaWrapper) });
#endif
				Type[] methodGenericParameters;

				//Get the generic type parameters.  The type will either be a single type, an IList<> of a single type, or an IDictionary<> of a key type and a value type.
				if (currentField.ContainerType == ContainerType.Simple)
					methodGenericParameters = new Type[] { currentField.ValueType };
				else if (currentField.ContainerType == ContainerType.Array)
					methodGenericParameters = new Type[] { typeof(IList<int>).GetGenericTypeDefinition().MakeGenericType(new Type[] { currentField.ValueType }) };
				else
					methodGenericParameters = new Type[] { typeof(Dictionary<int, int>).GetGenericTypeDefinition().MakeGenericType(new Type[] { currentField.KeyType, currentField.ValueType }) };

				//Instantiate a new generic method from the "AddField" method we got before with the generic parameters we got
				//from the current field we are querying.
				MethodInfo genericAddFieldMethodInstantiated = addFieldmethod.MakeGenericMethod(methodGenericParameters);
				SchemaWrapper swSub = null;  //Our subSchema is null by default unless the field is of type "Entity," in which case
											 //we will call "FromSchema" again on the field's subSchema.
				if (currentField.ValueType == typeof(Entity))
				{
					Schema subSchema = Schema.Lookup(currentField.SubSchemaGUID);
					swSub = SchemaWrapper.FromSchema(subSchema);
				}
                //Invoke the "AddField" method with the generic parameters from the current field.
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
                genericAddFieldMethodInstantiated.Invoke(swReturn, new object[] { currentField.FieldName, currentField.GetSpecTypeId(), swSub });
#else
				genericAddFieldMethodInstantiated.Invoke(swReturn, new object[] { currentField.FieldName, currentField.UnitType, swSub });
#endif
			}
			//Note that we don't call SchemaWrapper.FinishSchema here. 
			//The Autodesk.Revit.DB.ExtensibleStorage.Schema object already exists and is registered -- we're just populating the outer wrapper.
			return swReturn;
		}


		/// <summary>
		/// Creates a new SchemaWrapper from a Guid
		/// </summary>
		/// <param name="schemaId">The schemaId guid</param>
		/// <returns>A new SchemaWrapper</returns>
		public static SchemaWrapper NewSchema(Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess, string vendorId, string applicationId, string name, string description)
		{
			return new SchemaWrapper(schemaId, readAccess, writeAccess, vendorId, applicationId, name, description);
		}

		/// <summary>
		/// Constructor used by class factories
		/// </summary>
		private SchemaWrapper(Guid schemaId, AccessLevel readAccess, AccessLevel writeAccess, string vendorId, string applicationId, string schemaName, string schemaDescription)
		{
			m_SchemaDataWrapper = new SchemaDataWrapper(schemaId, readAccess, writeAccess, vendorId, applicationId, schemaName, schemaDescription);
			SetRevitAssembly();
		}

		private SchemaWrapper(Schema schema) : this(schema.GUID, schema.ReadAccessLevel, schema.WriteAccessLevel, schema.VendorId, schema.ApplicationGUID.ToString(), schema.SchemaName, schema.Documentation)
		{
			this.SetSchema(schema);
		}

        #endregion

        #region Active schema manipulation

        /// <summary>
        /// Adds a new field to the SchemaWrapper
        /// </summary>
        /// <typeparam name="TypeName">Currently supported types:  int, short, float, double, bool, string, ElementId, XYZ, UV, Guid, Entity, IDictionary<>, IList<>.  IDictionary<> does not support floating point types, XYZ, UV, or Entity as Key parameters.</typeparam>
        /// <param name="name">The name of the field</param>
        /// <param name="unit">The unit type of the field.  Defintion required for floating point, XYZ, and UV types</param>
        /// <param name="subSchema">A subSchemaWrapper, if the field is of type Entity</param>
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
        public void AddField<TypeName>(string name, ForgeTypeId unit, SchemaWrapper subSchema)
#else
		public void AddField<TypeName>(string name, UnitType unit, SchemaWrapper subSchema)
#endif
		{
			//	TODO
			//m_SchemaDataWrapper.AddData(name, typeof(TypeName), unit, subSchema);
		}

		/// <summary>
		/// Create a new Autodesk.Revit.DB.ExtensibleStorage.SchemaBuilder and Schema from
		/// the data in the SchemaWrapper.
		/// </summary>
		public void FinishSchema()
		{

			//Create the Autodesk.Revit.DB.ExtensibleStorage.SchemaBuilder that actually builds the schema
			m_SchemaBuilder = new SchemaBuilder(new Guid(m_SchemaDataWrapper.SchemaId));

			//We will add a new field to our schema from each FieldData object in our SchemaWrapper
			foreach (FieldData currentFieldData in m_SchemaDataWrapper.DataList)
			{
				//If the current field's type is a supported system type (int, string, etc...),
				//we can instantiate it with Type.GetType().  If the current field's type is a supported Revit API type
				//(XYZ, elementID, etc...), we need to call GetType from the RevitAPI.dll assembly object.
				Type fieldType = null;
				try
				{
					fieldType = Type.GetType(currentFieldData.Type, true, true);
				}

				catch (Exception ex)  //Get the type from the Revit API assembly if it is not a system type.
				{
					Debug.WriteLine(ex.ToString());
					try
					{
						//Get the field from the Revit API assembly.
						fieldType = m_Assembly.GetType(currentFieldData.Type);
					}
					catch (Exception exx)
					{
						Debug.WriteLine("Error getting type: " + exx.ToString());
						throw;
					}
				}

				//	Now that we have the data type of field we need to add, we need to call either
				//	SchemaBuilder.AddSimpleField, AddArrayField, or AddMapField.
				FieldBuilder currentFieldBuilder = null;
				Guid subSchemaId = Guid.Empty;
				Type[] genericParams = null;


				if (currentFieldData.SubSchema != null)
					subSchemaId = new Guid(currentFieldData.SubSchema.Data.SchemaId);

				//If our data type is a generic, it is an IList<> or an IDictionary<>, so it's an array or map type
				if (fieldType.IsGenericType)
				{
					Type tGeneric = fieldType.GetGenericTypeDefinition();  //tGeneric will be either an IList<> or an IDictionary<>.

					//Create an IList<> or an IDictionary<> to compare against tGeneric.
					Type iDictionaryType = typeof(IDictionary<int, int>).GetGenericTypeDefinition();
					Type iListType = typeof(IList<int>).GetGenericTypeDefinition();

					genericParams = fieldType.GetGenericArguments();  //Get the actual data type(s) stored in the field's IList<> or IDictionary<>
					if (tGeneric == iDictionaryType)
						//Pass the key and value type of our dictionary type to AddMapField.
						currentFieldBuilder = m_SchemaBuilder.AddMapField(currentFieldData.Name, genericParams[0], genericParams[1]);
					else if (tGeneric == iListType)
						//Pass the value type of our list type to AddArrayField.
						currentFieldBuilder = m_SchemaBuilder.AddArrayField(currentFieldData.Name, genericParams[0]);
					else
						throw new Exception("Generic type is neither IList<> nor IDictionary<>, cannot process.");
				}
				else
					//The simpler case -- just add field using a name and a System.Type.
					currentFieldBuilder = m_SchemaBuilder.AddSimpleField(currentFieldData.Name, fieldType);

				if (  //if we're dealing with an Entity as a simple field, map field, or list field and need to do recursion...
					(fieldType == (typeof(Entity)))
					||
				   (fieldType.IsGenericType && ((genericParams[0] == (typeof(Entity))) || ((genericParams.Length > 1) && (genericParams[1] == typeof(Entity)))))
					)
				{
					currentFieldBuilder.SetSubSchemaGUID(subSchemaId);  //Set the SubSchemaID if our field
					currentFieldData.SubSchema.FinishSchema();  //Recursively create the schema for the subSchema.
				}

#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
                if (currentFieldData.Unit != SpecTypeId.Custom)
					currentFieldBuilder.SetSpec(currentFieldData.Unit);
#else
				if (currentFieldData.Unit != UnitType.UT_Undefined)
					currentFieldBuilder.SetUnitType(currentFieldData.Unit);
#endif
			}

			//Set all the top level data in the schema we are generating.
			m_SchemaBuilder.SetReadAccessLevel(this.Data.ReadAccess);
			m_SchemaBuilder.SetWriteAccessLevel(this.Data.WriteAccess);
			m_SchemaBuilder.SetVendorId(this.Data.VendorId);
			m_SchemaBuilder.SetApplicationGUID(new Guid(this.Data.ApplicationId));
			m_SchemaBuilder.SetDocumentation(this.Data.Documentation);
			m_SchemaBuilder.SetSchemaName(this.Data.Name);


			//Actually finish creating the Autodesk.Revit.DB.ExtensibleStorage.Schema.
			m_Schema = m_SchemaBuilder.Finish();
		}

		public override string ToString()
		{
			StringBuilder strBuilder = new StringBuilder();
			strBuilder.AppendLine("--Start Schema--  " + " Name: " + this.Data.Name + ", Description: " + this.Data.Documentation + ", Id: " + this.Data.SchemaId + ", ReadAccess: " + this.Data.ReadAccess.ToString() + ", WriteAccess: " + this.Data.WriteAccess.ToString());
			foreach (FieldData fd in this.Data.DataList)
			{
				strBuilder.AppendLine(fd.ToString());
			}
			strBuilder.AppendLine("--End Schema--");
			return strBuilder.ToString();
		}

#endregion

#region Helper Methods

		/// <summary>
		/// Returns a string representation of all data in an Entity
		/// </summary>
		/// <param name="entity">The entity to query</param>
		/// <returns>All entity data as a string</returns>
		public string GetSchemaEntityData(Entity entity)
		{
			StringBuilder swBuilder = new StringBuilder();
			DumpAllSchemaEntityData<Entity>(entity, entity.Schema, swBuilder);
			return swBuilder.ToString();
		}

		/// <summary>
		/// Recursively gets all data from an Entity and appends it in string format to a StringBuilder.
		/// </summary>
		/// <typeparam name="EntityType">A type parameter that will always be set to type "Entity" -- used to get around some type reflection limitations in .NET</typeparam>
		/// <param name="storageEntity">The entity to query</param>
		/// <param name="schema">The schema of the Entity</param>
		/// <param name="strBuilder">The StringBuilder to append entity data to</param>
		private void DumpAllSchemaEntityData<EntityType>(EntityType storageEntity, Schema schema, StringBuilder strBuilder)
		{
			strBuilder.AppendLine("Schema/Entity Name: " + "" + ", Description: " + schema.Documentation + ", Id: " + schema.GUID.ToString() + ", Read Access: " + schema.ReadAccessLevel.ToString() + ", Write Access: " + schema.WriteAccessLevel.ToString());
			foreach (Field currentField in schema.ListFields())
			{

				//Here, we call GetFieldDataAsString on this class, the SchemaWrapper class.  However, we must
				//call it with specific generic parameters that are only known at runtime, so we must use reflection to 
				//dynamically create a method with parameters from the current field we want to extract data from


				ParameterModifier[] pmodifiers = new ParameterModifier[0]; //a Dummy parameter needed for GetMethod() call, empty

				//Get the method.
				MethodInfo getFieldDataAsStringMethod = typeof(SchemaWrapper).GetMethod("GetFieldDataAsString", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, System.Type.DefaultBinder, new Type[] { typeof(Field), typeof(Entity), typeof(StringBuilder) }, pmodifiers);

				//Determine if our field type is a simple field, an array field, or a map field.  Then, create an array
				//of type parameters corresponding to the key and value types of the field.

				//Note that GetFieldDatAsString takes two generic parameters, a key type and field(value) type.
				//The 'key' type is only used for dictionary/map types, so pass an int type as a placeholder when
				//processing simple and array fields.
				Type[] methodGenericParameters;
				if (currentField.ContainerType == ContainerType.Simple)
					methodGenericParameters = new Type[] { typeof(int), currentField.ValueType };   //dummy int types for non dictionary type
				else if (currentField.ContainerType == ContainerType.Array)
					methodGenericParameters = new Type[] { typeof(int), currentField.ValueType };  //dummy int types for non dictionary type
				else
					methodGenericParameters = new Type[] { currentField.KeyType, currentField.ValueType };

				//Instantiate a generic version of "GetFieldDataAsString" with the type parameters we just got from our field.
				MethodInfo genericGetFieldDataAsStringmethodInstantiated = getFieldDataAsStringMethod.MakeGenericMethod(methodGenericParameters);
				//Call that method to get the data out of that field.
				genericGetFieldDataAsStringmethodInstantiated.Invoke(this, new object[] { currentField, storageEntity, strBuilder });

			}
			strBuilder.AppendLine("---------------------------------------------------------");


		}

		/// <summary>
		/// Creates an instance of the RevitAPI.DLL assembly so we can query type information
		/// from it
		/// </summary>
		private void SetRevitAssembly()
		{
			m_Assembly = System.Reflection.Assembly.GetAssembly(typeof(XYZ));
		}

#endregion

#region Properties

		/// <summary>
		/// Gets the Autodesk.Revit.DB.ExtensibleStorage schema that the wrapper owns.
		/// </summary>
		/// <returns>An Autodesk.Revit.DB.ExtensibleStorage.Schema</returns>
		public Schema GetSchema() { return m_Schema; }

		/// <summary>
		/// Sets the Autodesk.Revit.DB.ExtensibleStorage schema that the wrapper owns.
		/// </summary>
		/// <param name="schema">An Autodesk.Revit.DB.ExtensibleStorage.Schema</param>
		public void SetSchema(Schema schema) { m_Schema = schema; }


		/// <summary>
		/// Gets and set the SchemaDataWrapper of the SchemaWrapper.  "Set" is for serialization use only.
		/// </summary>
		public SchemaDataWrapper Data
		{
			get { return m_SchemaDataWrapper; }
			set { m_SchemaDataWrapper = value; }
		}

#endregion

#region Data
		private SchemaDataWrapper m_SchemaDataWrapper;

		[NonSerialized]
		private Schema m_Schema;

		[NonSerialized]
		private SchemaBuilder m_SchemaBuilder;

		[NonSerialized]
		private System.Reflection.Assembly m_Assembly;

#endregion

	}
}
