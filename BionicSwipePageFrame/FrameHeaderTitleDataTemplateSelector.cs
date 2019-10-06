using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BionicCode.BionicSwipePageFrame
{
  internal class FrameHeaderTitleDataTemplateSelector : DataTemplateSelector
  {
    public DataTemplate TextTemplate { get; set; }
    public DataTemplate ObjectTemplate { get; set; }
    public DataTemplate ImageSourceTemplate { get; set; }

    #region Overrides of DataTemplateSelector

    /// <inheritdoc />
    public override DataTemplate SelectTemplate(object item, DependencyObject container) => item is string ? this.TextTemplate : item is ImageSource ? this.ImageSourceTemplate : this.ObjectTemplate;

    #endregion
  }
}
