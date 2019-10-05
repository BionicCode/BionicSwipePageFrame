using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BionicUtilities.NetStandard.ViewModel;

namespace TestView
{
  public class ViewModel : BaseViewModel
  {
    public ViewModel()
    {
      this.Pages = new ObservableCollection<IPage>() {new Page() {Title = "First Page", FirstName = "James", LastName = "Bond"}, new Page() {Title = "Second Page", FirstName = "Termi", LastName = "Nator"}};
    }

    public ObservableCollection<IPage> Pages { get; set; }
  }

  public interface IPage
  {
    string Title { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
  }

  public class Page : BaseViewModel, IPage
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

    #endregion
  }
}
