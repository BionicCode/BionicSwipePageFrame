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

namespace BionicCode.BionicSwipePageFrame
{
  /// <summary>
  /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
  ///
  /// Step 1a) Using this custom control in a XAML file that exists in the current project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:BionicCode.BionicSwipePageFrame"
  ///
  ///
  /// Step 1b) Using this custom control in a XAML file that exists in a different project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:BionicCode.BionicSwipePageFrame;assembly=BionicCode.BionicSwipePageFrame"
  ///
  /// You will also need to add a project reference from the project where the XAML file lives
  /// to this project and Rebuild to avoid compilation errors:
  ///
  ///     Right click on the target project in the Solution Explorer and
  ///     "Add Reference"->"Projects"->[Browse to and select this project]
  ///
  ///
  /// Step 2)
  /// Go ahead and use your control in the XAML file.
  ///
  ///     <MyNamespace:BionicSwipePageFrameHeader/>
  ///
  /// </summary>
  [TemplatePart(Name = "PART_Title", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "PART_PageHeaderTitleHostPanel", Type = typeof(Panel))]
  public class BionicSwipePageFrameHeader : Control
  {
    public static System.Windows.ResourceKey ButtonStyleKey { get; }
    
    [System.ComponentModel.TypeConverter(typeof(System.Windows.LengthConverter))]
    public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(
      "IconHeight",
      typeof(double),
      typeof(BionicSwipePageFrameHeader),
      new PropertyMetadata(double.NaN));

    [System.ComponentModel.TypeConverter(typeof(System.Windows.LengthConverter))]

    public double IconHeight { get { return (double) GetValue(BionicSwipePageFrameHeader.IconHeightProperty); } set { SetValue(BionicSwipePageFrameHeader.IconHeightProperty, value); } }

    public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(
      "IconWidth",
      typeof(double),
      typeof(BionicSwipePageFrameHeader),
      new PropertyMetadata(double.NaN));

    [System.ComponentModel.TypeConverter(typeof(System.Windows.LengthConverter))]
    public double IconWidth { get { return (double) GetValue(BionicSwipePageFrameHeader.IconWidthProperty); } set { SetValue(BionicSwipePageFrameHeader.IconWidthProperty, value); } }

    static BionicSwipePageFrameHeader()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(BionicSwipePageFrameHeader), new FrameworkPropertyMetadata(typeof(BionicSwipePageFrameHeader)));

      BionicSwipePageFrameHeader.ButtonStyleKey = new ComponentResourceKey(typeof(BionicSwipePageFrameHeader), "ButtonStyle");
    }

    public BionicSwipePageFrameHeader()
    {
    }

    #region Overrides of FrameworkElement

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.PART_Title = GetTemplateChild("PART_Title") as FrameworkElement;
      this.PART_PageHeaderHostPanel = GetTemplateChild("PART_PageHeaderTitleHostPanel") as Panel;
    }

    internal Panel PART_PageHeaderHostPanel { get; set; }

    internal FrameworkElement PART_Title { get; set; }

    #endregion
  }
}
