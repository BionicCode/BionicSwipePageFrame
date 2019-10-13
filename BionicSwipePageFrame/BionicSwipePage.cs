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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BionicCode.BionicSwipePageFrame
{
  /// <summary>
  /// Container element that represents a selectable item in a <see cref="BionicSwipePageFrame"/>.
  /// </summary>
  /// <example>
  /// <code>
  /// &lt;BionicSwipePageFrame x:Name="PageFrame" Height="500" &gt;
  ///   &lt;BionicSwipePage&gt;First XAML created page&lt;/BionicSwipePage&gt;
  ///   &lt;BionicSwipePage&gt;Second XAML created page&lt;/BionicSwipePage&gt;
  ///   &lt;BionicSwipePage&gt;Third XAML created page&lt;/BionicSwipePage&gt;
  ///   &lt;BionicSwipePage&gt;Fourth XAML created page&lt;/BionicSwipePage&gt;
  /// &lt;/BionicSwipePageFrame&gt;
  /// </code>
  /// </example>
  public class BionicSwipePage : ContentControl
  {
    static BionicSwipePage()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(BionicSwipePage), new FrameworkPropertyMetadata(typeof(BionicSwipePage)));
    }
  }
}
