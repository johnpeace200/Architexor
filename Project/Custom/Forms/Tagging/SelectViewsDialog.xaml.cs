using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Architexor.Forms
{
	/// <summary>
	/// Interaction logic for AddCategoryDialog.xaml
	/// </summary>
	public partial class SelectViewDialog : Window
	{
		public List<string> SelectedViews { get; private set; }

		public SelectViewDialog(IEnumerable<string> views, IEnumerable<string> checkedViews)
		{
			InitializeComponent();

			lstViews.ItemsSource = views;

			// Pre-select views
			foreach (var view in checkedViews)
			{
				if (lstViews.Items.Contains(view))
				{
					lstViews.SelectedItems.Add(view);
				}
			}
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			SelectedViews = lstViews.SelectedItems.Cast<string>().ToList();
			DialogResult = true;
			Close();
		}

		private void btnCheckAll_Click(object sender, RoutedEventArgs e)
		{
			lstViews.SelectAll();
    }

		private void btnCheckNone_Click(object sender, RoutedEventArgs e)
		{
			lstViews.UnselectAll();
		}
	}
}
