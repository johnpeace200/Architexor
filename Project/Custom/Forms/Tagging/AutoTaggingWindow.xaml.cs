using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Architexor.Request;
using Architexor.Controllers;

namespace Architexor.Forms
{
	/// <summary>
	/// Interaction logic for AutoTaggingWindow.xaml
	/// </summary>
	public partial class AutoTaggingWindow : Window, IExternal
	{
		protected ArchitexorRequestHandler m_Handler;
		protected ExternalEvent m_ExEvent;
		private bool _isDisposed = false;

		public ArchitexorRequestHandler Handler { get => m_Handler; }

		public ArchitexorRequestId LastRequestId { get; set; } = ArchitexorRequestId.None;

		public AutoTaggingWindow()
		{
			InitializeComponent();
		}

		public AutoTaggingWindow(ArchitexorRequestId reqId, UIApplication uiApp)
		{
			InitializeComponent();

			//	A new handler to handle request posting by the dialog
			m_Handler = new ArchitexorRequestHandler(Architexor.Custom.Application.thisApp, reqId, uiApp);

			//	External Event for the dialog to use (to post requests)
			m_ExEvent = ExternalEvent.Create(m_Handler);

			WakeUp();
		}

		#region IExternal Implementation

		private void EnableCommands(bool status)
		{

		}

		public ArchitexorRequestId GetRequestId()
		{
			if (m_Handler == null)
			{
				return ArchitexorRequestId.None;
			}
			return m_Handler.RequestId;
		}

		public void MakeRequest(ArchitexorRequestId request)
		{
			Serialize();

			LastRequestId = request;

			m_Handler.Request.Make(request);
			m_ExEvent.Raise();
			DozeOff();
		}

		public void DozeOff()
		{
			EnableCommands(false);
		}

		public void WakeUp(bool bFinish = false)
		{
			if (bFinish)
			{
				Close();
				return;
			}
			EnableCommands(true);

			Activate();
		}

		public void IClose()
		{
			Close();
		}

		public bool IVisible()
		{
			return Visibility == System.Windows.Visibility.Visible;
		}

		public bool IIsDisposed()
		{
			return _isDisposed;
		}

		public void IShow()
		{
			Show();
		}
		#endregion

		#region Event Handler

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Tagging controller = (Tagging)m_Handler.Instance;
			controller.Initialize();

			ElementId curViewId = null;
			if(controller.GetDocument().ActiveView != null)
			{
				curViewId = controller.GetDocument().ActiveView.Id;
			}

			string curViewName = null;
			foreach (ViewPlan view in controller.AllViews)
			{
				if (view.Id == curViewId)
				{
					curViewName = view.ViewType.ToString() + " : " + view.Name;
				}
			}

			if (!string.IsNullOrEmpty(curViewName))
			{
				lstViews.ItemsSource = new List<string>() { curViewName };
			}

			// We support tags(Independenttag, RoomTag), Walls, Stairs, FamilyInstances for now.
			lstOverlapConsiderCategories.Items.Add("Walls");
			lstOverlapConsiderCategories.Items.Add("Stairs");
			lstOverlapConsiderCategories.Items.Add("FamilyInstances");
			lstOverlapConsiderCategories.Items.Add("Tags");
			// Select all as default
			lstOverlapConsiderCategories.SelectedItems.Add("Walls");
			lstOverlapConsiderCategories.SelectedItems.Add("Stairs");
			lstOverlapConsiderCategories.SelectedItems.Add("FamilyInstances");
			lstOverlapConsiderCategories.SelectedItems.Add("Tags");

			// Initialize Type for Category
			foreach(FamilySymbol fs in Custom.Helpers.Tagging.GetTagTypes(controller.GetDocument(), BuiltInCategory.OST_Walls))
			{
				cmbWallTagType.Items.Add(fs.Name);
			}
			foreach(FamilySymbol fs in Custom.Helpers.Tagging.GetTagTypes(controller.GetDocument(), BuiltInCategory.OST_Doors))
			{
				cmbDoorTagType.Items.Add(fs.Name);
			}
			foreach(FamilySymbol fs in Custom.Helpers.Tagging.GetTagTypes(controller.GetDocument(), BuiltInCategory.OST_Windows))
			{
				cmbWindowTagType.Items.Add(fs.Name);
			}
			foreach(FamilySymbol fs in Custom.Helpers.Tagging.GetTagTypes(controller.GetDocument(), BuiltInCategory.OST_Rooms))
			{
				cmbRoomTagType.Items.Add(fs.Name);
			}

			// Add Wall Functions
			new List<string>
			{
				"Interior",
				"Exterior",
				"Foundation",
				"Retaining",
				"Soffit",
				"Coreshaft"
			}.ForEach(funcName => lstWallFunction.Items.Add(funcName));

			// Add EndTypes
			new List<string>
			{
				"Auto",
				"AttachedEnd",
				"FreeEnd"
			}.ForEach(endType => {
				cmbWallEndType.Items.Add(endType);
				cmbDoorEndType.Items.Add(endType);
				cmbWindowEndType.Items.Add(endType);
			});

			// Add Head Directions
			new List<string>
			{
				"Auto",
				"AtCenter",
				"InsideRoom",
				"FacingExterior",
				"FacingInterior",
				"Corner"
			}.ForEach(dir => {
				cmbWallHeadingDirection.Items.Add(dir);
				cmbDoorHeadingDirection.Items.Add(dir);
				cmbWindowHeadingDirection.Items.Add(dir);
			});

			// Add Orientations
			new List<string>
			{
				"Auto",
				"Horizontal",
				"Vertical"
			}.ForEach(orientation => {
				cmbWallOrientation.Items.Add(orientation);
				cmbDoorOrientation.Items.Add(orientation);
				cmbWindowOrientation.Items.Add(orientation);
				cmbRoomOrientation.Items.Add(orientation);
			});

			AddTaggingCategory("Doors");
			AddTaggingCategory("Windows");
			lstCategories.SelectedIndex = 0;
		}

		private void Serialize()
		{
			Tagging controller = (Tagging)m_Handler.Instance;

			List<ViewPlan> views = new List<ViewPlan>();
			List<ViewPlan> allViews = controller.AllViews;
			foreach (string item in lstViews.Items)
			{
				views.Add(allViews.Find(x => item == x.ViewType.ToString() + " : " + x.Name));
			}
			controller.SetSelectedViews(views);

			Document doc = controller.GetDocument();
			Categories categories = doc.Settings.Categories;

			controller.OverlapConsiderCategories.Clear();
			foreach(var catName in lstOverlapConsiderCategories.SelectedItems)
			{
				if (!controller.OverlapConsiderCategories.Contains(catName.ToString()))
				{
					controller.OverlapConsiderCategories.Add(catName.ToString());
				}
			}

			controller.ConsiderLink = chkConsiderLinked.IsChecked.Value;
			controller.AvoidOverlap = chkAvoidOverlap.IsChecked.Value;
			controller.MultiHosting = chkMultiTagging.IsChecked.Value;
			//controller.MergeLeaders = chkMultiLeaders.IsChecked.Value;

			try
			{
				// Tagging Options
				List<Custom.Helpers.TagHostingOption> options = new List<Custom.Helpers.TagHostingOption>();
				foreach (string item in lstCategories.Items)
				{
					Custom.Helpers.TagHostingOption option = new Custom.Helpers.TagHostingOption();
					option.Category = categories.get_Item(item);

					switch (item)
					{
						case "Walls":
							option.TagTypeName = cmbWallTagType.SelectedItem.ToString();
							option.SetEndType(cmbWallEndType.SelectedItem.ToString());
							option.SetHeadingDirection(cmbWallHeadingDirection.SelectedItem.ToString());
							option.SetOrientation(cmbWallOrientation.SelectedItem.ToString());
							option.MaxOffset = Convert.ToDouble(txtWallMaxOffset.Text);
							option.LeaderVisible = chkWallShowLeader.IsChecked.Value;
							option.MinWallLength = Convert.ToDouble(txtMinWallLength.Text);
							option.WallFunctions.Clear();
							foreach (var wallFunction in lstWallFunction.SelectedItems)
							{
								switch (wallFunction.ToString())
								{
									case "Interior":
										option.WallFunctions.Add(WallFunction.Interior);
										break;
									case "Exterior":
										option.WallFunctions.Add(WallFunction.Exterior);
										break;
									case "Foundation":
										option.WallFunctions.Add(WallFunction.Foundation);
										break;
									case "Retaining":
										option.WallFunctions.Add(WallFunction.Retaining);
										break;
									case "Soffit":
										option.WallFunctions.Add(WallFunction.Soffit);
										break;
									case "Coreshaft":
										option.WallFunctions.Add(WallFunction.Coreshaft);
										break;
									default:
										break;
								}
							}
							break;
						case "Doors":
							option.TagTypeName = cmbDoorTagType.SelectedItem.ToString();
							option.SetEndType(cmbDoorEndType.SelectedItem.ToString());
							option.SetHeadingDirection(cmbDoorHeadingDirection.SelectedItem.ToString());
							option.SetOrientation(cmbDoorOrientation.SelectedItem.ToString());
							option.MaxOffset = Convert.ToDouble(txtDoorMaxOffset.Text);
							option.LeaderVisible = chkDoorShowLeader.IsChecked.Value;
							break;
						case "Windows":
							option.TagTypeName = cmbWindowTagType.SelectedItem.ToString();
							option.SetEndType(cmbWindowEndType.SelectedItem.ToString());
							option.SetHeadingDirection(cmbWindowHeadingDirection.SelectedItem.ToString());
							option.SetOrientation(cmbWindowOrientation.SelectedItem.ToString());
							option.MaxOffset = Convert.ToDouble(txtWindowMaxOffset.Text);
							option.LeaderVisible = chkWindowShowLeader.IsChecked.Value;
							break;
						case "Rooms":
							option.TagTypeName = cmbRoomTagType.SelectedItem.ToString();
							option.SetOrientation(cmbRoomOrientation.SelectedItem.ToString());
							option.MaxOffset = 0;
							option.LeaderVisible = chkRoomShowLeader.IsChecked.Value;
							break;
						default:
							break;
					}

					option.Refine();
					options.Add(option);
				}

				controller.TagHostingOptions = options;
			}
			catch
			{
				System.Windows.MessageBox.Show("Please input the parameters correctly.");
			}

		}

		protected override void OnClosed(EventArgs e)
		{
			m_ExEvent.Dispose();
			m_ExEvent = null;
			m_Handler = null;

			base.OnClosed(e);
		}

		private void btnSelectViews_Click(object sender, RoutedEventArgs e)
		{
			Tagging controller = (Tagging)m_Handler.Instance;

			var views = controller.AllViews.Select(view => view.ViewType.ToString() + " : " + view.Name).ToList();
			// Get the currently selected views from the list
			var checkedViews = lstViews.Items.Cast<string>().ToList();

			var dialog = new SelectViewDialog(views, checkedViews) 
			{
				Owner = this
			};
			bool? result = dialog.ShowDialog();
			if (result == true)
			{
				var selViews = dialog.SelectedViews;
				lstViews.ItemsSource = selViews;
			}
		}

		private void btnAddCategory_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new AddCategoryDialog(new List<string>() { "Walls", "Doors", "Windows", "Rooms" })
			{ 
				Owner = this
			};
			bool? result = dialog.ShowDialog();
			if (result == true)
			{
				string selectedCategory = dialog.SelectedCategory;

				bool bExist = false;
				foreach (var item in lstCategories.Items)
				{
					if((item as string) == selectedCategory)
					{
						bExist = true;
					}
				}

				if (!bExist)
				{
					AddTaggingCategory(selectedCategory);
					lstCategories.SelectedIndex = lstCategories.Items.Count - 1;
				}
			}
		}

		private void btnUpCategory_Click(object sender, RoutedEventArgs e)
		{
			int index = lstCategories.SelectedIndex;
			if(index > 0)
			{
				var item = lstCategories.Items[index];

				// Remove and insert above
				lstCategories.Items.RemoveAt(index);
				lstCategories.Items.Insert(index - 1,item);

				// Reselect moved item
				lstCategories.SelectedIndex = index - 1;
			}
		}

		private void btnDownCategory_Click(object sender, RoutedEventArgs e)
		{
			int index = lstCategories.SelectedIndex;
			int count = lstCategories.Items.Count;
			if (index >= 0 && index < count - 1)
			{
				var item = lstCategories.Items[index];

				// Remove and insert above
				lstCategories.Items.RemoveAt(index);
				lstCategories.Items.Insert(index + 1, item);

				// Reselect moved item
				lstCategories.SelectedIndex = index + 1;
			}

		}

		private void btnRemoveCategory_Click(object sender, RoutedEventArgs e)
		{
			int count = lstCategories.Items.Count;
			if (count == 0) return;

			// Remove the selected item
			lstCategories.Items.RemoveAt(lstCategories.SelectedIndex);

			// Select the first last item if any remains
			if (lstCategories.Items.Count > 0)
			{
				lstCategories.SelectedIndex = 0;
			}

			if(lstCategories.Items.Count == 0)
			{
				UpdateTaggingContainer();
			}
		}

		private void lstCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var category = lstCategories.SelectedItem as string;

			UpdateTaggingContainer(category);
		}

		private void AddTaggingCategory(string category)
		{
			// Add category to the list
			lstCategories.Items.Add(category);

			switch (category)
			{
				case "Walls":
					cmbWallTagType.SelectedIndex = 0;
					cmbWallEndType.SelectedItem = "Auto";
					cmbWallHeadingDirection.SelectedItem = "Auto";
					cmbWallOrientation.SelectedItem = "Auto";
					txtWallMaxOffset.Text = "10";
					txtMinWallLength.Text = "10";
					chkWallShowLeader.IsChecked = true;
					lstWallFunction.SelectedItems.Clear();
					lstWallFunction.SelectedItems.Add("Exterior");
					break;
				case "Doors":
					cmbDoorTagType.SelectedIndex = 0;
					cmbDoorEndType.SelectedItem = "Auto";
					cmbDoorHeadingDirection.SelectedItem = "Auto";
					cmbDoorOrientation.SelectedItem = "Auto";
					txtDoorMaxOffset.Text = "0.5";
					chkDoorShowLeader.IsChecked = false;
					break;
				case "Windows":
					cmbWindowTagType.SelectedIndex = 0;
					cmbWindowEndType.SelectedItem = "Auto";
					cmbWindowHeadingDirection.SelectedItem = "Auto";
					cmbWindowOrientation.SelectedItem = "Auto";
					txtWindowMaxOffset.Text = "0.5";
					chkWindowShowLeader.IsChecked = false;
					break;
				case "Rooms":
					cmbRoomTagType.SelectedIndex = 0;
					cmbRoomOrientation.SelectedItem = "Auto";
					chkRoomShowLeader.IsChecked = false;
					break;
				default:
					break;
			}
		}

		private void UpdateTaggingContainer(string category = "")
		{
			// Hide all tag category containers
			gridTagWall.Visibility = System.Windows.Visibility.Collapsed;
			gridTagDoor.Visibility = System.Windows.Visibility.Collapsed;
			gridTagWindow.Visibility = System.Windows.Visibility.Collapsed;
			gridTagRoom.Visibility = System.Windows.Visibility.Collapsed;

			switch (category)
			{
				case "Walls":
					gridTagWall.Visibility = System.Windows.Visibility.Visible;
					break;
				case "Doors":
					gridTagDoor.Visibility = System.Windows.Visibility.Visible;
					break;
				case "Windows":
					gridTagWindow.Visibility = System.Windows.Visibility.Visible;
					break;
				case "Rooms":
					gridTagRoom.Visibility = System.Windows.Visibility.Visible;
					break;
				default:
					break;

			}
		}

		private void btnInitRectCache_Click(object sender, RoutedEventArgs e)
		{
			MakeRequest(ArchitexorRequestId.InitRectCache);
		}

		private void btnShowCache_Click(object sender, RoutedEventArgs e)
		{
			MakeRequest(ArchitexorRequestId.ShowCache);
		}

		private void btnGenerateTags_Click(object sender, RoutedEventArgs e)
		{
			MakeRequest(ArchitexorRequestId.GenerateTags);
		}
		
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		#endregion

	}
}
