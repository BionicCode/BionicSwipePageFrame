using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BionicUtilities.Net.Extensions;
using BionicUtilities.Net.Utility;
using BionicUtilities.NetStandard.ViewModel;

namespace BionicCode.BionicSwipePageFrame
{
  /// <summary>
  /// Customizable page frame that displays a collection of arbitrary data model items according to a provided <see cref="ItemsControl.ItemTemplate"/>. The items are selectable.
  /// </summary>
  /// <remarks>
  /// <see cref="BionicSwipePageFrame"/> derives from <see cref="Selector"/> which is a <see cref="ItemsControl"/>. The <see cref="ItemsControl.ItemsPanel"/> property is ignored.<br/><br/>
  ///
  /// 
  /// If the <see cref="ItemsControl.ItemsSource"/> contains different data types use the <see cref="ItemsControl.ItemTemplateSelector"/> property to assign a <see cref="DataTemplateSelector"/> that maps the appropriate <see cref="DataTemplate"/> to the items. <br/>
  /// The <see cref="BionicSwipePageFrame"/> uses <see cref="BionicSwipePage"/> as container for the data items.<br/>
  /// The control uses semi-virtualization which means item containers are lazy generated once they are requested for display, but never destroyed. Full virtualization support is not implemented yet. <br/><br/>
  /// When <see cref="IsLoopingPagesEnabled"/> is set to <c>True</c>, index values assigned to <see cref="Selector.SelectedIndex"/> property or to the <see cref="ICommandSource.CommandParameter"/> of <see cref="LoadPageFromIndexRoutedCommand"/> are treated special. An index greater than the count of the <see cref="ItemsControl.ItemsSource"/> is coerced to point to the first item in the collection. Similarly an index value lesser than zero will point to the last page item (wrapping). <br/>
  /// When <see cref="IsLoopingPagesEnabled"/> is set to <c>False</c>, index values assigned to <see cref="Selector.SelectedIndex"/> property or to the <see cref="ICommandSource.CommandParameter"/> of <see cref="LoadPageFromIndexRoutedCommand"/> are coerced to valid index values. An index greater than the count of the <see cref="ItemsControl.ItemsSource"/> is coerced to point to the last item in the collection. An index value lesser than zero will point to the first page item (zero index). <br/>
  /// </remarks>
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
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(BionicSwipePage))]
  [TemplatePart(Name = "PART_SelectedPageHost", Type = typeof(ContentPresenter))]
  [TemplatePart(Name = "PART_AnimatedPreviousPageHost", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "PART_AnimatedSelectedPageHost", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "PART_PageHeader", Type = typeof(BionicSwipePageFrameHeader))]
  public class BionicSwipePageFrame : Selector
  {
    #region RoutedCommands

    /// <summary>
    /// Load the next page. Command is parameterless.
    /// </summary>
    public static readonly RoutedUICommand LoadNextPageRoutedCommand = new RoutedUICommand("Load the next page", nameof(BionicSwipePageFrame.LoadNextPageRoutedCommand), typeof(BionicSwipePageFrame));

    /// <summary>
    /// Load the previous page. This Command is parameterless.
    /// </summary>
    public static readonly RoutedUICommand LoadPreviousPageRoutedCommand = new RoutedUICommand("Load the previous page", nameof(BionicSwipePageFrame.LoadPreviousPageRoutedCommand), typeof(BionicSwipePageFrame));

    /// <summary>
    /// Load a specific page by passing in the page index as CommandParameter. <br/>
    /// <see cref="ICommandSource.CommandParameter"/> is the item's index in the <see cref="ItemsControl.ItemsSource"/>.
    /// </summary>
    public static readonly RoutedUICommand LoadPageFromIndexRoutedCommand = new RoutedUICommand("Load a specific page by passing in the corresponding page index as CommandParameter", nameof(BionicSwipePageFrame.LoadPageFromIndexRoutedCommand), typeof(BionicSwipePageFrame));

    /// <summary>
    /// Load a specific page by passing in the item data model as CommandParameter. <br/>
    /// <see cref="ICommandSource.CommandParameter"/> is the item data model from the <see cref="ItemsControl.ItemsSource"/>.
    /// </summary>
    public static readonly RoutedUICommand LoadPageFromItemRoutedCommand = new RoutedUICommand("Load a specific page by passing in the item data model as CommandParameter", nameof(BionicSwipePageFrame.LoadPageFromItemRoutedCommand), typeof(BionicSwipePageFrame));

    #endregion

    #region RoutedEvents


    /// <summary>
    /// The <see cref="RoutedEvent"/> iof the <see cref="PageChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent PreviewPageChangedRoutedEvent = EventManager.RegisterRoutedEvent("PreviewPageChanged",
      RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(BionicSwipePageFrame));

    /// <summary>
    /// The event is raised when the <see cref="SelectedPage"/> and the <see cref="PreviousPage"/> have changed.
    /// </summary>
    public event RoutedEventHandler PreviewPageChanged
    {
      add { AddHandler(BionicSwipePageFrame.PreviewPageChangedRoutedEvent, value); }
      remove { RemoveHandler(BionicSwipePageFrame.PreviewPageChangedRoutedEvent, value); }
    }

    /// <summary>
    /// The <see cref="RoutedEvent"/> iof the <see cref="PageChanged"/> event.
    /// </summary>
    public static readonly RoutedEvent PageChangedRoutedEvent = EventManager.RegisterRoutedEvent("PageChanged",
      RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BionicSwipePageFrame));

    /// <summary>
    /// The event is raised when the <see cref="SelectedPage"/> and the <see cref="PreviousPage"/> have changed.
    /// </summary>
    public event RoutedEventHandler PageChanged
    {
      add { AddHandler(BionicSwipePageFrame.PageChangedRoutedEvent, value); }
      remove { RemoveHandler(BionicSwipePageFrame.PageChangedRoutedEvent, value); }
    }

    #endregion

    #region DependencyProperties

    /// <summary>
    /// The current <see cref="BionicSwipePage"/> instance. DependencyProperty.
    /// </summary>
    public static readonly DependencyProperty SelectedPageProperty = DependencyProperty.Register(
      "SelectedPage",
      typeof(BionicSwipePage),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(default(BionicSwipePage), BionicSwipePageFrame.OnSelectedPageChanged));

    /// <summary>
    /// The current <see cref="BionicSwipePage"/> instance. CLR Property.
    /// </summary>
    public BionicSwipePage SelectedPage
    {
      get => (BionicSwipePage) GetValue(BionicSwipePageFrame.SelectedPageProperty);
      set => SetValue(BionicSwipePageFrame.SelectedPageProperty, value);
    }

    public static readonly DependencyProperty PreviousSelectedIndexProperty = DependencyProperty.Register(
      "PreviousSelectedIndex",
      typeof(int),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(default(int)));

    public int PreviousSelectedIndex
    {
      get => (int) GetValue(BionicSwipePageFrame.PreviousSelectedIndexProperty);
      set => SetValue(BionicSwipePageFrame.PreviousSelectedIndexProperty, value);
    }

    public static readonly DependencyProperty PreviousPageProperty = DependencyProperty.Register(
      "PreviousPage",
      typeof(BionicSwipePage),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(default(BionicSwipePage), BionicSwipePageFrame.OnPreviousPageChanged));

    public BionicSwipePage PreviousPage
    {
      get => (BionicSwipePage) GetValue(BionicSwipePageFrame.PreviousPageProperty);
      set => SetValue(BionicSwipePageFrame.PreviousPageProperty, value);
    }

    public static readonly DependencyProperty IsHeaderVisibleProperty = DependencyProperty.Register(
      "IsHeaderVisible",
      typeof(bool),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(true));

    public bool IsHeaderVisible
    {
      get => (bool) GetValue(BionicSwipePageFrame.IsHeaderVisibleProperty);
      set => SetValue(BionicSwipePageFrame.IsHeaderVisibleProperty, value);
    }

    public static readonly DependencyProperty FrameHeaderStyleProperty = DependencyProperty.Register(
      "FrameHeaderStyle",
      typeof(Style),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(default(Style), BionicSwipePageFrame.OnFrameHeaderStyleChanged));

    public Style FrameHeaderStyle
    {
      get => (Style) GetValue(BionicSwipePageFrame.FrameHeaderStyleProperty);
      set => SetValue(BionicSwipePageFrame.FrameHeaderStyleProperty, value);
    }

    public static readonly DependencyProperty NavigationDirectionProperty = DependencyProperty.Register(
      "NavigationDirection",
      typeof(PageNavigationDirection),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(PageNavigationDirection.Undefined, BionicSwipePageFrame.OnNavigationDirectionChanged));

    public PageNavigationDirection NavigationDirection
    {
      get => (PageNavigationDirection) GetValue(BionicSwipePageFrame.NavigationDirectionProperty);
      set => SetValue(BionicSwipePageFrame.NavigationDirectionProperty, value);
    }

    /// <summary>
    /// DependencyProperty of the <see cref="IsLoopingPagesEnabled"/> property
    /// </summary>
    public static readonly DependencyProperty IsLoopingPagesEnabledProperty = DependencyProperty.Register(
      "IsLoopingPagesEnabled",
      typeof(bool),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(true));

    /// <summary>
    /// When enabled the <see cref="LoadNextPageRoutedCommand"/> and <see cref="LoadPreviousPageRoutedCommand"/> will loop throug the <see cref="ItemsControl.ItemsSource"/>.
    /// </summary>
    public bool IsLoopingPagesEnabled
    {
      get => (bool) GetValue(BionicSwipePageFrame.IsLoopingPagesEnabledProperty);
      set => SetValue(BionicSwipePageFrame.IsLoopingPagesEnabledProperty, value);
    }

    public static readonly DependencyProperty PreviousPageTranslationStartPositionProperty =
      DependencyProperty.Register(
        "PreviousPageTranslationStartPosition",
        typeof(double),
        typeof(BionicSwipePageFrame),
        new PropertyMetadata(default(double)));

    public double PreviousPageTranslationStartPosition
    {
      get => (double) GetValue(BionicSwipePageFrame.PreviousPageTranslationStartPositionProperty);
      set => SetValue(BionicSwipePageFrame.PreviousPageTranslationStartPositionProperty, value);
    }

    public static readonly DependencyProperty PreviousPageTranslationEndPositionProperty = DependencyProperty.Register(
      "PreviousPageTranslationEndPosition",
      typeof(double),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(default(double)));

    public double PreviousPageTranslationEndPosition
    {
      get => (double) GetValue(BionicSwipePageFrame.PreviousPageTranslationEndPositionProperty);
      set => SetValue(BionicSwipePageFrame.PreviousPageTranslationEndPositionProperty, value);
    }

    public static readonly DependencyProperty SelectedPageTranslationStartPositionProperty =
      DependencyProperty.Register(
        "SelectedPageTranslationStartPosition",
        typeof(double),
        typeof(BionicSwipePageFrame),
        new PropertyMetadata(default(double)));

    public double SelectedPageTranslationStartPosition
    {
      get => (double) GetValue(BionicSwipePageFrame.SelectedPageTranslationStartPositionProperty);
      set => SetValue(BionicSwipePageFrame.SelectedPageTranslationStartPositionProperty, value);
    }

    public static readonly DependencyProperty SelectedPageTranslationEndPositionProperty = DependencyProperty.Register(
      "SelectedPageTranslationEndPosition",
      typeof(double),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(default(double)));

    public double SelectedPageTranslationEndPosition
    {
      get => (double) GetValue(BionicSwipePageFrame.SelectedPageTranslationEndPositionProperty);
      set => SetValue(BionicSwipePageFrame.SelectedPageTranslationEndPositionProperty, value);
    }

    /// <summary>
    /// The DependencyProperty for <see cref="TitleMemberPath"/>.
    /// </summary>
    public static readonly DependencyProperty TitleMemberPathProperty = DependencyProperty.Register(
      "TitleMemberPath",
      typeof(string),
      typeof(BionicSwipePageFrame),
      new PropertyMetadata(default(string), BionicSwipePageFrame.OnTitleMemberPathChanged));

    /// <summary>
    /// The property path to the property that holds the page title <c>String</c>. The property path is relative to the data model type. <br/><br/>
    /// E.g. The property path to the <c>PageTitle</c> property of a data model class called ExampleClass would be <c>"PageTitle"</c>. <br/>The property can be nested (e.g. <c>"NestedType.PageTitle"</c>) or reference a collection member (e.g. <c>"NestedType.Items[1].PageTitle"</c>) 
    /// </summary>
    public string TitleMemberPath
    {
      get => (string) GetValue(BionicSwipePageFrame.TitleMemberPathProperty);
      set => SetValue(BionicSwipePageFrame.TitleMemberPathProperty, value);
    }

    #endregion
    
    static BionicSwipePageFrame()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
        typeof(BionicSwipePageFrame),
        new FrameworkPropertyMetadata(typeof(BionicSwipePageFrame)));
      KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(
        typeof(TabControl),
        new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
      Selector.SelectedIndexProperty.OverrideMetadata(typeof(BionicSwipePageFrame), new FrameworkPropertyMetadata(0, BionicSwipePageFrame.OnSelectedIndexChanged, BionicSwipePageFrame.CoerceSelectedIndex));
    }

    public BionicSwipePageFrame()
    {
      this.CommandBindings.Add(
        new CommandBinding(
          BionicSwipePageFrame.LoadNextPageRoutedCommand,
          ((sender, args) => this.SelectedIndex += 1)));
      this.CommandBindings.Add(
        new CommandBinding(
          BionicSwipePageFrame.LoadPreviousPageRoutedCommand,
          ((sender, args) => this.SelectedIndex -= 1)));
      this.CommandBindings.Add(
        new CommandBinding(
          BionicSwipePageFrame.LoadPageFromItemRoutedCommand,
          (sender, args) => this.SelectedItem = args.Parameter, (sender, args) => args.CanExecute = args.Parameter != null && this.Items.Contains(args.Parameter)));
      this.CommandBindings.Add(
        new CommandBinding(
          BionicSwipePageFrame.LoadPageFromIndexRoutedCommand,
          (sender, args) => this.SelectedIndex = args.Parameter is int newSelectedIndex
            ? newSelectedIndex
            : int.Parse((string) args.Parameter),
          (sender, args) => args.CanExecute = args.Parameter is int || int.Parse(args.Parameter as string) is int));

      this.PageChanged += OnPageChanged;
      this.PreviewPageChanged += OnPreviewPageChanged;

      this.Loaded += (e, args) => InitializeSwipeAnimations();
    }

    private static void OnTitleMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as BionicSwipePageFrame).UpdateFrameTitleFromTitleMemberPath();
    }

    /// <summary>
    /// Called when the <see cref="PreviewPageChanged"/> event is received.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPreviewPageChanged(object sender, RoutedEventArgs e)
    {
    }
    
    /// <summary>
    /// Called when the <see cref="PageChanged"/> event is received.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnPageChanged(object sender, RoutedEventArgs e)
    {
    }

    private static object CoerceSelectedIndex(DependencyObject d, object basevalue)
    {
      var _this = d as BionicSwipePageFrame;
      if (!_this.IsInitialized)
      {
        return basevalue;
      }
      
      var originalValue = (int) basevalue;
      int coercedValue = originalValue;

      _this.NavigationDirection = originalValue > _this.SelectedIndex
        ? PageNavigationDirection.Next
        : PageNavigationDirection.Previous;

      if (_this.Items.Count == 0)
      {
        _this.NavigationDirection = PageNavigationDirection.Undefined;
        coercedValue = -1;
      }
      else if (_this.IsLoopingPagesEnabled)
      {
        if (originalValue < 0)
        {
          coercedValue = _this.Items.Count - 1;
        }
        else if (originalValue >= _this.Items.Count)
        {
          coercedValue = 0;
        }
      }
      else 
      {
        if (_this.NavigationDirection == PageNavigationDirection.Previous)
        {
          coercedValue = Math.Max(originalValue, 0);
        }
        else if (_this.NavigationDirection == PageNavigationDirection.Next)
        {
          coercedValue = Math.Min(originalValue, _this.Items.Count - 1);
        }
      }
      return coercedValue;
    }

    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var _this = (d as BionicSwipePageFrame);
      _this.OnSelectedIndexChanged((int) e.OldValue, (int) e.NewValue);
    }

    /// <summary>
    /// Called when the <see cref="Selector.SelectedIndex"/> property changes. <br/>
    /// Executes the swipe animation.
    /// </summary>
    /// <param name="oldValue">Old value of the <see cref="Selector.SelectedIndex"/> property.</param>
    /// <param name="newValue">New value of the <see cref="Selector.SelectedIndex"/> property.</param>
    protected virtual void OnSelectedIndexChanged(int oldValue, int newValue)
    {
      if (!this.IsInitialized)
      {
        return;
      }

      this.PreviousSelectedIndex = oldValue;

      UpdateSelectedPage();
      UpdateLeavingPage();

      // new behavior (DDVSO 208019) - change SelectedContent and focus
      // before raising SelectionChanged.
      bool isKeyboardFocusWithin = this.IsKeyboardFocusWithin;
      if (isKeyboardFocusWithin)
      {
        // If keyboard focus is within the control, make sure it is going to the correct place
        BionicSwipePage item = GetPageContainerAt(this.SelectedIndex);
        item?.Focus();
      }

      HandleSwipeAnimation();
      RaiseEvent(new RoutedEventArgs(BionicSwipePageFrame.PageChangedRoutedEvent, this));
    }

    private void HandleSwipeAnimation()
    {
      InitializeSwipeAnimations();
      this.Storyboard?.Begin();
    }

    private void InitializeSwipeAnimations()
    {
      (this.PART_AnimatedPreviousPageHost.RenderTransform as TranslateTransform).X =
        this.PreviousPageTranslationStartPosition;
      (this.PART_AnimatedSelectedPageHost.RenderTransform as TranslateTransform).X = this
        .SelectedPageTranslationStartPosition;

      var selectedAnimatedPageTranslateAnimation = new DoubleAnimation(
        this.SelectedPageTranslationStartPosition,
        this.SelectedPageTranslationEndPosition,
        new Duration(TimeSpan.FromSeconds(0.4)))
      {
        BeginTime = TimeSpan.Zero,
        EasingFunction = new ExponentialEase() {EasingMode = EasingMode.EaseInOut, Exponent = 6}
      };

      var previousAnimatedPageTranslateAnimation = new DoubleAnimation(
        this.PreviousPageTranslationStartPosition,
        this.PreviousPageTranslationEndPosition,
        new Duration(TimeSpan.FromSeconds(0.4)))
      {
        BeginTime = TimeSpan.Zero,
        EasingFunction = new ExponentialEase() {EasingMode = EasingMode.EaseInOut, Exponent = 6}
      };

      var selectedPageVisibilityAnimation = new ObjectAnimationUsingKeyFrames() {Duration = new Duration(TimeSpan.FromSeconds(0.41)) };
      selectedPageVisibilityAnimation.KeyFrames.Add(
        new DiscreteObjectKeyFrame(
          System.Windows.Visibility.Hidden,
          KeyTime.FromTimeSpan(TimeSpan.Zero)));

      var selectedAnimationPageVisibilityAnimation =
        new ObjectAnimationUsingKeyFrames() {Duration = new Duration(TimeSpan.FromSeconds(0.41))};
      selectedAnimationPageVisibilityAnimation.KeyFrames.Add(
        new DiscreteObjectKeyFrame(
          System.Windows.Visibility.Visible,
          KeyTime.FromTimeSpan(TimeSpan.Zero)));

      var previousAnimatedPageVisibilityAnimation =
        new ObjectAnimationUsingKeyFrames() {Duration = new Duration(TimeSpan.FromSeconds(0.41)) };
      previousAnimatedPageVisibilityAnimation.KeyFrames.Add(
        new DiscreteObjectKeyFrame(
          System.Windows.Visibility.Visible,
          KeyTime.FromTimeSpan(TimeSpan.Zero)));

      Storyboard.SetTarget(selectedPageVisibilityAnimation, this.PART_SelectedPageHost);
      Storyboard.SetTargetProperty(selectedPageVisibilityAnimation, new PropertyPath("Visibility"));
      Storyboard.SetTarget(previousAnimatedPageVisibilityAnimation, this.PART_AnimatedPreviousPageHost);
      Storyboard.SetTargetProperty(previousAnimatedPageVisibilityAnimation, new PropertyPath("Visibility"));
      Storyboard.SetTarget(selectedAnimationPageVisibilityAnimation, this.PART_AnimatedSelectedPageHost);
      Storyboard.SetTargetProperty(selectedAnimationPageVisibilityAnimation, new PropertyPath("Visibility"));

      Storyboard.SetTarget(selectedAnimatedPageTranslateAnimation, this.PART_AnimatedSelectedPageHost);
      Storyboard.SetTargetProperty(
        selectedAnimatedPageTranslateAnimation,
        new PropertyPath("RenderTransform.(TranslateTransform.X)"));
      Storyboard.SetTarget(previousAnimatedPageTranslateAnimation, this.PART_AnimatedPreviousPageHost);
      Storyboard.SetTargetProperty(
        previousAnimatedPageTranslateAnimation,
        new PropertyPath("RenderTransform.(TranslateTransform.X)"));

      this.Storyboard = new Storyboard() {FillBehavior = FillBehavior.Stop};
      this.Storyboard.Children.Add(previousAnimatedPageVisibilityAnimation);
      this.Storyboard.Children.Add(selectedAnimationPageVisibilityAnimation);
      this.Storyboard.Children.Add(selectedPageVisibilityAnimation);
      this.Storyboard.Children.Add(selectedAnimatedPageTranslateAnimation);
      this.Storyboard.Children.Add(previousAnimatedPageTranslateAnimation);
      var beginStoryboard = new BeginStoryboard() {Storyboard = this.Storyboard};
      //this.Storyboard.Completed += (sender, args) => this.Storyboard?.Stop();
      this.Storyboard.Freeze();
    }

    private static void OnSelectedPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as BionicSwipePageFrame).OnSelectedPageChanged((BionicSwipePage) e.OldValue, (BionicSwipePage) e.NewValue);
    }

    /// <summary>
    /// Called when the <see cref="SelectedPage"/> property changes.
    /// </summary>
    /// <param name="oldValue">Old value of the <see cref="SelectedPage"/> property.</param>
    /// <param name="newValue">New value of the <see cref="SelectedPage"/> property.</param>
    protected virtual void OnSelectedPageChanged(BionicSwipePage oldValue, BionicSwipePage newValue)
    {
    }

    private static void OnPreviousPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as BionicSwipePageFrame).OnPreviousPageChanged((BionicSwipePage) e.OldValue, (BionicSwipePage) e.NewValue);
    }

    /// <summary>
    /// Called when the <see cref="PreviousPage"/> property changes.
    /// </summary>
    /// <param name="oldValue">Old value of the <see cref="PreviousPage"/> property.</param>
    /// <param name="newValue">New value of the <see cref="PreviousPage"/> property.</param>
    protected virtual  void OnPreviousPageChanged(BionicSwipePage oldValue, BionicSwipePage newValue)
    {
    }
    
    private static void OnFrameHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var _this = (d as BionicSwipePageFrame);
      var style = (Style) e.NewValue;
      if (style == null || _this.PART_PageHeader == null)
      {
        return;
      }
      _this.PART_PageHeader.Style = style;
    }

    private static void OnNavigationDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var _this = (d as BionicSwipePageFrame);
      _this.OnNavigationDirectionChanged((PageNavigationDirection) e.OldValue, (PageNavigationDirection) e.NewValue);
    }

    /// <summary> 
    /// Called when the <see cref="NavigationDirection"/> property changes.
    /// </summary>
    /// <param name="oldNavigationDirection">The last value of the <see cref="NavigationDirection"/> property.</param>
    /// <param name="newNavigationDirection">The new value of the <see cref="NavigationDirection"/> property.</param>
    protected virtual void OnNavigationDirectionChanged(PageNavigationDirection oldNavigationDirection, PageNavigationDirection newNavigationDirection)
    {
      if (newNavigationDirection == PageNavigationDirection.Undefined)
      {
        return;
      }

      this.PreviousPageTranslationEndPosition = newNavigationDirection == PageNavigationDirection.Next
        ? this.ActualWidth * -1
        : this.ActualWidth;
      this.SelectedPageTranslationStartPosition = newNavigationDirection == PageNavigationDirection.Next
        ? this.ActualWidth
        : this.ActualWidth * -1;
    }

    #region Overrides of FrameworkElement

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.PART_PageHeader = GetTemplateChild("PART_PageHeader") as BionicSwipePageFrameHeader;
      this.PART_PageHeader.Style = this.FrameHeaderStyle;
      this.PART_PageHeader.ApplyTemplate();
      //this.PART_PageHeader.Template.LoadContent();
      if (this.PART_PageHeader.TryFindVisualChildElementByName("PART_Title", out FrameworkElement titleElement))
      {
        this.PART_Title = titleElement as TextBlock;

        UpdateFrameTitleFromTitleMemberPath();
      }
      this.PART_SelectedPageHost = GetTemplateChild("PART_SelectedPageHost") as ContentPresenter;
      this.PART_SelectedPageHost.Visibility = Visibility.Visible;
      this.PART_AnimatedSelectedPageHost = GetTemplateChild("PART_AnimatedSelectedPageHost") as FrameworkElement;
      this.PART_AnimatedSelectedPageHost.Visibility = Visibility.Collapsed;
      this.PART_AnimatedPreviousPageHost = GetTemplateChild("PART_AnimatedPreviousPageHost") as FrameworkElement;
      this.PART_AnimatedPreviousPageHost.Visibility = Visibility.Collapsed;
      this.IsInitialized = true;
    }

    #endregion

    #region Overrides of ItemsControl

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride() => new BionicSwipePage();

    /// <inheritdoc />
    protected override bool IsItemItsOwnContainerOverride(object item) => item is BionicSwipePage;

    /// <summary>
    ///     This virtual method in called when IsInitialized is set to true and it raises an Initialized event
    /// </summary>
    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.SelectedIndex = 0;
      UpdateSelectedPage();
    }

    #endregion

    private void UpdateFrameTitleFromTitleMemberPath()
    {
      if (this.PART_Title == null)
      {
        return;
      }

      if (string.IsNullOrWhiteSpace(this.TitleMemberPath))
      {
        this.PART_Title.Visibility = Visibility.Collapsed;
        return;
      }

      var binding = new Binding
      {
        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(BionicSwipePageFrame), 1),
        Path = new PropertyPath(nameof(this.SelectedItem) + "." + this.TitleMemberPath)
      };
      this.PART_Title.SetBinding(TextBlock.TextProperty, binding);
      this.PART_Title.Visibility = Visibility.Visible;
    }

    private void GenerateContainerForItemAt(int pageIndex)
    {
      if (pageIndex < 0)
      {
        return;
      }

      var generator = this.ItemContainerGenerator as IItemContainerGenerator;
      GeneratorPosition startPosition = generator.GeneratorPositionFromIndex(pageIndex);
      using (generator.StartAt(startPosition, GeneratorDirection.Forward, true))
      {
        DependencyObject itemContainer = generator.GenerateNext(out bool isNewlyGenerated);
        if (isNewlyGenerated)
        {
          generator.PrepareItemContainer(itemContainer);
        }
      }
    }

    private void GenerateContainersForItems()
    {
      if (this.Items.Count < 1)
      {
        return;
      }

      var generator = this.ItemContainerGenerator as IItemContainerGenerator;

      GeneratorPosition startPosition = generator.GeneratorPositionFromIndex(0);
      using (generator.StartAt(startPosition, GeneratorDirection.Forward, true))
      {
        for (int i = 0; i < this.Items.Count; i++)
        {
          DependencyObject itemContainer = generator.GenerateNext(out bool isNewlyGenerated);
          if (isNewlyGenerated)
          {
            generator.PrepareItemContainer(itemContainer);
          }
        }
      }
    }

    private void UpdateLeavingPage()
    {
      GenerateContainerForItemAt(this.PreviousSelectedIndex);
      this.PreviousPage = GetPageContainerAt(this.PreviousSelectedIndex);
    }

    private void UpdateSelectedPage()
    {
      GenerateContainerForItemAt(this.SelectedIndex);
      this.SelectedPage = GetPageContainerAt(this.SelectedIndex);
    }

    private BionicSwipePage GetPageContainerAt(int pageIndex)
    {
      // Check if the selected item is a TabItem
      var pageContainer = this.SelectedItem as BionicSwipePage;
      if (pageContainer == null)
      {
        // It is a data item, get its TabItem container
        pageContainer = this.ItemContainerGenerator.ContainerFromIndex(pageIndex) as BionicSwipePage;

        // Due to event leapfrogging, we may have the wrong container.
        // If so, re-fetch the right container using a more expensive method.
        // (BTW, the previous line will cause a debug assert in this case)  [Dev10 452711]
        if (pageContainer == null ||
            !ItemsControl.Equals(this.SelectedItem, this.ItemContainerGenerator.ItemFromContainer(pageContainer)))
        {
          pageContainer = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as BionicSwipePage;
        }
      }

      return pageContainer;
    }

    private BionicSwipePageFrameHeader PART_PageHeader { get; set; }
    private FrameworkElement PART_AnimatedPreviousPageHost { get; set; }

    private FrameworkElement PART_AnimatedSelectedPageHost { get; set; }

    private TextBlock PART_Title { get; set; }

    private ContentPresenter PART_SelectedPageHost { get; set; }
    private new bool IsInitialized { get; set; }
    private Storyboard Storyboard { get; set; }
  }
}