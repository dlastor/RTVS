﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.VisualStudio.R.Package.DataInspect {
    /// <summary>
    /// Matrix view user control for two dimensional data
    /// Internally Visual is used directly for performance
    /// </summary>
    internal partial class MatrixView : UserControl {
        private VisualGridScroller _scroller;

        static MatrixView() {
            ForegroundProperty.OverrideMetadata(
                typeof(MatrixView),
                new FrameworkPropertyMetadata(
                    SystemColors.ControlTextBrush,
                    FrameworkPropertyMetadataOptions.Inherits,
                    new PropertyChangedCallback(OnForegroundPropertyChanged)));
        }

        public MatrixView() {
            InitializeComponent();
            DataContext = this;
        }

        internal void Initialize(IGridProvider<string> dataProvider) {
            if (Points != null) {
                Points.PointChanged -= Points_PointChanged;
            }

            Points = new GridPoints(dataProvider.RowCount, dataProvider.ColumnCount, Data.RenderSize);
            Points.PointChanged += Points_PointChanged;

            ColumnHeader.Clear();
            RowHeader.Clear();
            Data.Clear();

            DataProvider = dataProvider;

            _scroller?.StopScroller();
            _scroller = new VisualGridScroller(this);
            Refresh();  // initial refresh

            // reset scroll bar position to zero
            HorizontalScrollBar.Value = HorizontalScrollBar.Minimum;
            VerticalScrollBar.Value = VerticalScrollBar.Minimum;
            SetScrollBar(ScrollDirection.Both);

            CanSort = dataProvider.CanSort;
        }

        public void Refresh() {
            _scroller?.EnqueueCommand(GridUpdateType.Refresh, 0);
        }
        public void UpdateSort() {
            _scroller?.EnqueueCommand(GridUpdateType.Sort, 0);
        }

        internal GridPoints Points { get; set; }

        internal IGridProvider<string> DataProvider { get; set; }

        public bool CanSort {
            get { return (bool)GetValue(CanSortProperty); }
            set { SetValue(CanSortProperty, value); }
        }

        public static readonly DependencyProperty CanSortProperty =
            DependencyProperty.Register("CanSort", typeof(bool), typeof(MatrixView), new PropertyMetadata(false));

        #region Foreground

        private static void OnForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue != e.NewValue) {
                ((MatrixView)d).OnForegroundPropertyChanged((Brush)e.NewValue);
            }
        }

        private void OnForegroundPropertyChanged(Brush foregroundBrush) {
            ColumnHeader.Foreground = foregroundBrush;
            RowHeader.Foreground = foregroundBrush;
            Data.Foreground = foregroundBrush;

            Refresh();
        }

        #endregion

        #region GridLinesBrush

        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register(
                "GridLinesBrush",
                typeof(Brush),
                typeof(MatrixView),
                new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnGridLinesBrushPropertyChanged)));

        public Brush GridLinesBrush {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        private static void OnGridLinesBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue != e.NewValue) {
                ((MatrixView)d).OnGridLinesBrushPropertyChanged((Brush)e.NewValue);
            }
        }

        private void OnGridLinesBrushPropertyChanged(Brush gridLineBrush) {
            Data.SetGridLineBrush(gridLineBrush);

            Refresh();
        }

        #endregion

        #region GridBackground

        public static readonly DependencyProperty GridBackgroundProperty =
            DependencyProperty.Register(
                "GridBackground",
                typeof(Brush),
                typeof(MatrixView),
                new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnGridBackgroundPropertyChanged)));

        public Brush GridBackground {
            get { return (Brush)GetValue(GridBackgroundProperty); }
            set { SetValue(GridBackgroundProperty, value); }
        }

        private static void OnGridBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue != e.NewValue) {
                ((MatrixView)d).OnGridBackgroundPropertyChanged((Brush)e.NewValue);
            }
        }

        private void OnGridBackgroundPropertyChanged(Brush gridBackgroundBrush) {
            Data.Background = gridBackgroundBrush;

            // VisualGrid uses OnRender to paint background color, InvalidateVisual will call it
            Data.InvalidateVisual();

            Refresh();
        }

        #endregion

        #region HeaderLinesBrush

        public static readonly DependencyProperty HeaderLinesBrushProperty =
            DependencyProperty.Register(
                "HeaderLinesBrush",
                typeof(Brush),
                typeof(MatrixView),
                new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnHeaderLinesBrushPropertyChanged)));

        public Brush HeaderLinesBrush {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        private static void OnHeaderLinesBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue != e.NewValue) {
                ((MatrixView)d).OnHeaderLinesBrushPropertyChanged((Brush)e.NewValue);
            }
        }

        private void OnHeaderLinesBrushPropertyChanged(Brush gridLineBrush) {
            ColumnHeader.SetGridLineBrush(gridLineBrush);
            RowHeader.SetGridLineBrush(gridLineBrush);

            Refresh();
        }

        #endregion

        #region HeaderBackground

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register(
                "HeaderBackground",
                typeof(Brush),
                typeof(MatrixView),
                new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnHeaderBackgroundPropertyChanged)));

        public Brush HeaderBackground {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        private static void OnHeaderBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue != e.NewValue) {
                ((MatrixView)d).OnHeaderBackgroundPropertyChanged((Brush)e.NewValue);
            }
        }

        private void OnHeaderBackgroundPropertyChanged(Brush headerBackground) {
            ColumnHeader.Background = headerBackground;
            RowHeader.Background = headerBackground;

            // VisualGrid uses OnRender to paint background color, InvalidateVisual will call it
            ColumnHeader.InvalidateVisual();
            RowHeader.InvalidateVisual();

            Refresh();
        }

        #endregion

        protected override void OnKeyDown(KeyEventArgs e) {
            e.Handled = true;
            if (e.Key == Key.Up) {
                _scroller?.EnqueueCommand(GridUpdateType.LineUp, 1);
            } else if (e.Key == Key.Down) {
                _scroller?.EnqueueCommand(GridUpdateType.LineDown, 1);
            } else if (e.Key == Key.Right) {
                _scroller?.EnqueueCommand(GridUpdateType.LineRight, 1);
            } else if (e.Key == Key.Left) {
                _scroller?.EnqueueCommand(GridUpdateType.LineLeft, 1);
            } else if (e.Key == Key.PageUp) {
                _scroller?.EnqueueCommand(GridUpdateType.PageUp, 1);
            } else if (e.Key == Key.PageDown) {
                _scroller?.EnqueueCommand(GridUpdateType.PageDown, 1);
            } else if (e.Key == Key.Home) {
                _scroller?.EnqueueCommand(GridUpdateType.SetVerticalOffset, 0.0, ThumbTrack.None);
            } else if (e.Key == Key.End) {
                _scroller?.EnqueueCommand(GridUpdateType.SetVerticalOffset, 1.0, ThumbTrack.None);
            } else {
                e.Handled = false;
            }

            if (!e.Handled) {
                base.OnKeyDown(e);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);

            // null if not Initialized yet
            if (_scroller != null) {
                _scroller.EnqueueCommand(GridUpdateType.SizeChange, Data.RenderSize);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            if (_scroller != null && (e.Delta > 0 || e.Delta < 0)) {
                _scroller.EnqueueCommand(GridUpdateType.MouseWheel, e.Delta);
                e.Handled = true;
            }

            if (!e.Handled) {
                base.OnMouseWheel(e);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            Point pt = e.GetPosition(this);

            HitTestResult result = VisualTreeHelper.HitTest(this, pt);
            if (result.VisualHit is TextVisual) {
                var textVisual = (TextVisual)result.VisualHit;
                textVisual.ToggleHighlight();

                e.Handled = true;
            }

            if (!e.Handled) {
                base.OnMouseLeftButtonDown(e);
            }
        }

        private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e) {
            if (_scroller == null) {
                return;
            }

            switch (e.ScrollEventType) {
                // page up/down
                case ScrollEventType.LargeDecrement:
                    _scroller.EnqueueCommand(GridUpdateType.PageUp, 1);
                    break;
                case ScrollEventType.LargeIncrement:
                    _scroller.EnqueueCommand(GridUpdateType.PageDown, 1);
                    break;

                // line up/down
                case ScrollEventType.SmallDecrement:
                    _scroller.EnqueueCommand(GridUpdateType.LineUp, 1);
                    break;
                case ScrollEventType.SmallIncrement:
                    _scroller.EnqueueCommand(GridUpdateType.LineDown, 1);
                    break;

                // scroll to here
                case ScrollEventType.ThumbPosition:
                    _scroller.EnqueueCommand(GridUpdateType.SetVerticalOffset, ComputeVerticalOffset(e), ThumbTrack.None);
                    break;

                // thumb drag
                case ScrollEventType.ThumbTrack:
                    _scroller.EnqueueCommand(GridUpdateType.SetVerticalOffset, ComputeVerticalOffset(e), ThumbTrack.Track);
                    break;
                case ScrollEventType.EndScroll:
                    _scroller.EnqueueCommand(GridUpdateType.SetVerticalOffset, ComputeVerticalOffset(e), ThumbTrack.End);
                    break;

                // home/end (scroll to limit)
                case ScrollEventType.First:
                    _scroller.EnqueueCommand(GridUpdateType.SetVerticalOffset, ComputeVerticalOffset(e), ThumbTrack.None);
                    break;
                case ScrollEventType.Last:
                    _scroller.EnqueueCommand(GridUpdateType.SetVerticalOffset, ComputeVerticalOffset(e), ThumbTrack.None);
                    break;

                default:
                    break;
            }
        }

        private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e) {
            if (_scroller == null) {
                return;
            }

            switch (e.ScrollEventType) {
                // page left/right
                case ScrollEventType.LargeDecrement:
                    _scroller.EnqueueCommand(GridUpdateType.PageLeft, 1);
                    break;
                case ScrollEventType.LargeIncrement:
                    _scroller.EnqueueCommand(GridUpdateType.PageRight, 1);
                    break;

                // line left/right
                case ScrollEventType.SmallDecrement:
                    _scroller.EnqueueCommand(GridUpdateType.LineLeft, 1);
                    break;
                case ScrollEventType.SmallIncrement:
                    _scroller.EnqueueCommand(GridUpdateType.LineRight, 1);
                    break;

                // scroll to here
                case ScrollEventType.ThumbPosition:
                    _scroller.EnqueueCommand(GridUpdateType.SetHorizontalOffset, ComputeHorizontalOffset(e), ThumbTrack.None);
                    break;

                // thumb drag
                case ScrollEventType.ThumbTrack:
                    _scroller.EnqueueCommand(GridUpdateType.SetHorizontalOffset, ComputeHorizontalOffset(e), ThumbTrack.Track);
                    break;
                case ScrollEventType.EndScroll:
                    _scroller.EnqueueCommand(GridUpdateType.SetHorizontalOffset, ComputeHorizontalOffset(e), ThumbTrack.End);
                    break;

                // home/end (scroll to limit)
                case ScrollEventType.First:
                    _scroller.EnqueueCommand(GridUpdateType.SetHorizontalOffset, ComputeHorizontalOffset(e), ThumbTrack.None);
                    break;
                case ScrollEventType.Last:
                    _scroller.EnqueueCommand(GridUpdateType.SetHorizontalOffset, ComputeHorizontalOffset(e), ThumbTrack.None);
                    break;

                default:
                    break;
            }
        }

        private void Points_PointChanged(object sender, PointChangedEventArgs e) {
            SetScrollBar(e.Direction);
        }

        private double ComputeVerticalOffset(ScrollEventArgs e) {
            return e.NewValue / VerticalScrollBar.Maximum;
        }

        private double ComputeHorizontalOffset(ScrollEventArgs e) {
            return e.NewValue / HorizontalScrollBar.Maximum;
        }

        private void SetScrollBar(ScrollDirection direction) {
            if (direction.HasFlag(ScrollDirection.Horizontal)) {
                HorizontalScrollBar.ViewportSize = Points.ViewportWidth;
                HorizontalScrollBar.Maximum = Points.HorizontalExtent - Points.ViewportWidth;
                HorizontalScrollBar.Value = Points.HorizontalOffset;
            }

            if (direction.HasFlag(ScrollDirection.Vertical)) {
                VerticalScrollBar.ViewportSize = Points.ViewportHeight;
                VerticalScrollBar.Maximum = Points.VerticalExtent - Points.ViewportHeight;
                VerticalScrollBar.Value = Points.VerticalOffset;
            }
        }
    }
}
