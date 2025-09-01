using System;
using System.Text;
using Autodesk.Revit.DB;

namespace ConnectorTool.Storage
{
	/// <summary>
	/// /// A class to store schema field information
	/// </summary>
	[Serializable]
	public class FieldData
	{
        #region Constructors

        /// <summary>
        /// Create a new FieldData object
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="typeIn">The AssemblyQualifiedName of the Field's data type</param>
        /// <param name="unit">The unit type of the Field (set to UT_Undefined for non-floating point types</param>
        /// <param name="subSchema">The SchemaWrapper of the field's subSchema, if the field is of type "Entity"</param>
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
        public FieldData(string name, string typeIn, ForgeTypeId unit, SchemaWrapper subSchema)
#else
		public FieldData(string name, string typeIn, UnitType unit, SchemaWrapper subSchema) 
#endif
		{ 
			m_Name = name; 
			m_Type = typeIn;
			m_Unit = unit;
			m_SubSchema = subSchema;
		}
#endregion

#region Other helper functions
	    public override string ToString()
	    {
			  StringBuilder strBuilder = new StringBuilder();
			  strBuilder.Append("   Field: ");
			  strBuilder.Append(Name);
			  strBuilder.Append(", ");
			  strBuilder.Append(Type);
			  strBuilder.Append(", ");
			  strBuilder.Append(Unit.ToString());

			  if (SubSchema != null)
			  {
				  strBuilder.Append(Environment.NewLine + "   " + SubSchema.ToString());
			  }
			  return strBuilder.ToString();
		}
#endregion

#region Properties
		/// <summary>
		/// The name of a schema field
		/// </summary>
		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		/// <summary>
		/// The string representation of a schema field type (e.g. System.Int32)
		/// </summary>
		public string Type
		{
			get { return m_Type; }
			set { m_Type = value; }
		}

        /// <summary>
        /// The Unit type of the field
        /// </summary>
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
        public ForgeTypeId Unit
#else
		public UnitType Unit
#endif
		{
			get { return m_Unit; }
			set { m_Unit = value; }
		}

		/// <summary>
		/// The SchemaWrapper of the field's sub-Schema, if is of type "Entity"
		/// </summary>
		public SchemaWrapper SubSchema
		{
			get { return m_SubSchema; }
			set { m_SubSchema = value; }
		}
#endregion

#region Data
		private SchemaWrapper m_SubSchema;
		private string m_Name;
		private string m_Type;
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
		private ForgeTypeId m_Unit;
#else
		private UnitType m_Unit;
#endif
		#endregion

	}
}
