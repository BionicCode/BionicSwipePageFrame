using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BionicUtilities.NetStandard.ViewModel;

namespace TestView
{
  internal class ViewModel : BaseViewModel
  {
    public ViewModel()
    {
      this.Pages = new ObservableCollection<IPage>() {new Page() {Title = "First Page", FirstName = "James", LastName = "Bond", Logo = new BitmapImage(new Uri("/TestView;component/rangers_coyotes_hockey.jpg", UriKind.RelativeOrAbsolute)) }, new Page() {Title = "Second Page", FirstName = "Termi", LastName = "Nator", Logo = new BitmapImage(new Uri("/TestView;component/meric-dagli-AThrXc5Gi2s-unsplash-683x1024.jpg", UriKind.RelativeOrAbsolute)) } };
    }

    public ObservableCollection<IPage> Pages { get; set; }
    private Image logo;
    /// <inheritdoc />
    public Image Logo
    {
      get => new Image() { Source = new BitmapImage(new Uri("rangers_coyotes_hockey.jpg", UriKind.RelativeOrAbsolute)), Height = 100, Width = 100};
      set => TrySetValue(value, ref this.logo);
    }
  }

  internal interface IPage
  {
    string Title { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    ImageSource Logo { get; set; }
  }

  internal class Page : BaseViewModel, IPage
  {
    #region Implementation of IPage

    private string title;

    /// <inheritdoc />
    public string Title
    {
      get => this.title;
      set => TrySetValue(value, ref this.title);
    }

    private string firstName;

    /// <inheritdoc />
    public string FirstName { get => this.firstName; set => TrySetValue(value, ref this.firstName); }

    private string lastName;

    /// <inheritdoc />
    public string LastName { get => this.lastName; set => TrySetValue(value, ref this.lastName); }

    private ImageSource logo;
    /// <inheritdoc />
    public ImageSource Logo
    {
      get => this.logo;
      set => TrySetValue(value, ref this.logo);
    }

    #endregion
  }
}
