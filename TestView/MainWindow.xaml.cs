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
using BionicCode.BionicSwipePageFrame;

namespace TestView
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  internal partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      (this.PageFrame.DataContext as ViewModel).Pages.Add(new Page() { Title = $"Page #{this.PageFrame.Items.Count + 1}" });
      var item = new Page() { Title = $"Page #{this.PageFrame.Items.Count + 1}" };
      (this.PageFrame.DataContext as ViewModel).Pages.Add(item);
      this.Page = item;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      (this.PageFrame.DataContext as ViewModel).Pages.Add(new Page() {Title = $"Page #{this.PageFrame.Items.Count + 1}"});
      //this.PageFrame.SelectedIndex += 1;
    }

    public Page Page { get; set; }
  }
}
