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
	public partial class AddCategoryDialog : Window
	{
		public string SelectedCategory { get; private set; }

		public AddCategoryDialog(IEnumerable<string> categories)
		{
			InitializeComponent();
			cmbCategory.ItemsSource = categories;
			cmbCategory.SelectedIndex = 0; // Select first item by default
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			SelectedCategory = cmbCategory.SelectedItem as string;
			DialogResult = true;
			Close();
		}
	}
}
