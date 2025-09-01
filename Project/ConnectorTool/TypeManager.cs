using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;

namespace ConnectorTool
{
	/// <summary>
	/// data manager take charge of FamilySymbol object in current document
	/// </summary>
	public class TypeManager
	{
		// list of FamilySymbol objects
		private readonly List<FamilySymbol> m_Symbols;

		/// <summary>
		/// size of FamilySymbol objects in current Revit document
		/// </summary>
		public int Size { get => m_Symbols.Count; }
		
		/// <summary>
		/// constructor
		/// </summary>
		public TypeManager()
		{
			m_Symbols = new List<FamilySymbol>();
		}

		/// <summary>
		/// get list of Family object's names in current Revit document
		/// </summary>
		public List<Family> Families {
			get
			{
				List<Family> families = new List<Family>();
				foreach(FamilySymbol fs in m_Symbols)
				{
					if (families.Find(x => x.Id == fs.Family.Id) == null)
						families.Add(fs.Family);
				}
				return families;
			}
		}

		/// <summary>
		/// get list of FamilySymbol objects in current Revit document
		/// </summary>
		public List<FamilySymbol> Symbols { get => m_Symbols; }

		/// <summary>
		/// add one FamilySymbol object to the lists
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public bool AddSymbol(FamilySymbol symbol)
		{
			//if(m_Symbols.Find(x => x.Name == symbol.Name) == null)
			//	Please consider if the same name can be exist
			//	if this is only for duplication check, we can use Id field?
			if (m_Symbols.Find(x => x.Id == symbol.Id) == null)
			{
				m_Symbols.Add(symbol);
				return true;
			}
			return false;
		}
	}
}
