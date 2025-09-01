using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;

namespace ConnectorTool
{
	/// <summary>
	/// data manager take charge of FamilySymbol object in current document
	/// </summary>
	public class ConnectorTypeManager
	{
		//map list pairs Family object and its Name 
		private Dictionary<string, Family> m_familyMaps;
		// map list pairs FamilySymbol object and its Name 
		private Dictionary<string, FamilySymbol> m_symbolMaps;
		// list of FamilySymbol objects
		private List<Family> m_families;
		// list of FamilySymbol objects
		private List<FamilySymbol> m_symbols;

		/// <summary>
		/// size of FamilySymbol objects in current Revit document
		/// </summary>
		public int Size { get => m_symbolMaps.Count; }

		/// <summary>
		/// constructor
		/// </summary>
		public ConnectorTypeManager()
		{
			m_familyMaps = new Dictionary<string, Family>();
			m_symbolMaps = new Dictionary<string, FamilySymbol>();
			m_families = new List<Family>();
			m_symbols = new List<FamilySymbol>();
		}

		/// <summary>
		/// get list of Family object's names in current Revit document
		/// </summary>
		public ReadOnlyCollection<Family> ConnectionFamilies { get => new ReadOnlyCollection<Family>(m_families); }

		/// <summary>
		/// get list of FamilySymbol objects in current Revit document
		/// </summary>
		public ReadOnlyCollection<FamilySymbol> ConnectionSymbols { get => new ReadOnlyCollection<FamilySymbol>(m_symbols); }

		/// <summary>
		/// add one FamilyName to the lists
		/// </summary>
		/// <param name="connectionSymbol"></param>
		/// <returns></returns>
		public bool AddFamily(Family connectionFamily)
		{
			if (ContainsFamilyName(connectionFamily.Name))
			{
				return false;
			}
			m_familyMaps.Add(connectionFamily.Name, connectionFamily);
			m_families.Add(connectionFamily);
			return true;
		}

		/// <summary>
		/// add one FamilySymbol object to the lists
		/// </summary>
		/// <param name="connectionSymbol"></param>
		/// <returns></returns>
		public bool AddSymbol(FamilySymbol connectionSymbol)
		{
			if (ContainsSymbolName(connectionSymbol.Name))
			{
				return false;
			}
			m_symbolMaps.Add(connectionSymbol.Name, connectionSymbol);
			m_symbols.Add(connectionSymbol);
			return true;
		}

		/// <summary>
		/// inquire whether the FamilySymbol's Name already exists in the list
		/// </summary>
		/// <param name="symbolName"></param>
		/// <returns></returns>
		public bool ContainsFamilyName(string familyName)
		{
			return m_familyMaps.ContainsKey(familyName);
		}

		/// <summary>
		/// inquire whether the FamilySymbol's Name already exists in the list
		/// </summary>
		/// <param name="symbolName"></param>
		/// <returns></returns>
		public bool ContainsSymbolName(string symbolName)
		{
			return m_symbolMaps.ContainsKey(symbolName);
		}
	}
}
