using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FlipperControl
{
    public enum RotateAxis
    {
        Y = 1,
        X = 2,
    }

    public enum Direction
    {
        FrontToBack,
        BackToFront
    }

    public class FlipperControl : Control
    {
        public List<FrameworkElement> Views
        {
            get { return (List<FrameworkElement>)GetValue(ViewsProperty); }
            set { SetValue(ViewsProperty, value); }
        }

        public static readonly DependencyProperty ViewsProperty =
            DependencyProperty.Register("Views", typeof(List<FrameworkElement>), typeof(FlipperControl),
                new PropertyMetadata(new List<FrameworkElement>()));

        public int DisplayIndex
        {
            get { return (int)GetValue(DisplayIndexProperty); }
            set
            {
                var nextIndex = value;
                if (nextIndex >= Views.Count) nextIndex = 0;
                SetValue(DisplayIndexProperty, nextIndex);
            }
        }

        public static readonly DependencyProperty DisplayIndexProperty =
            DependencyProperty.Register("DisplayIndex", typeof(int), typeof(FlipperControl),
                new PropertyMetadata(0, OnDisplayIndexPropertyChanged));

        public bool AllowTapToFlip
        {
            get { return (bool)GetValue(AllowTapToFlipProperty); }
            set { SetValue(AllowTapToFlipProperty, value); }
        }

        public static readonly DependencyProperty AllowTapToFlipProperty =
            DependencyProperty.Register("AllowTapToFlip", typeof(bool), typeof(FlipperControl), new PropertyMetadata(false));

        public RotateAxis RotationAxis
        {
            get { return (RotateAxis)GetValue(RotaetAxisProperty); }
            set { SetValue(RotaetAxisProperty, value); }
        }

        public static readonly DependencyProperty RotaetAxisProperty =
            DependencyProperty.Register("RotationAxis", typeof(RotateAxis), typeof(FlipperControl),
                new PropertyMetadata(RotateAxis.X));

        public Direction FlipDirection
        {
            get { return (Direction)GetValue(FlipDirectionProperty); }
            set { SetValue(FlipDirectionProperty, value); }
        }

        public static readonly DependencyProperty FlipDirectionProperty =
            DependencyProperty.Register("FlipDirection", typeof(Direction), typeof(FlipperControl), new PropertyMetadata(Direction.FrontToBack));

        private static async void OnDisplayIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlipperControl;
            await control.NextAsync();
        }

        private Grid _rootGrid;
        private Compositor _compositor;
        private int _zindex = 1;

        public FlipperControl()
        {
            DefaultStyleKey = typeof(FlipperControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rootGrid = GetTemplateChild("RootGrid") as Grid;
            _rootGrid.Tapped += _rootGrid_Tapped;
            _rootGrid.SizeChanged += _rootGrid_SizeChanged;
            _compositor = this.GetVisual().Compositor;

            var firstView = Views.LastOrDefault();
            if (firstView != null)
            {
                Canvas.SetZIndex(firstView, _zindex++);
                _rootGrid.Children.Add(firstView);
            }
        }

        private void UpdatePerspective(Visual visual)
        {
            Vector2 sizeList = new Vector2((float)_rootGrid.ActualWidth, (float)_rootGrid.ActualHeight);
            Matrix4x4 perspective = new Matrix4x4(
                        1.0f, 0.0f, 0.0f, 0.0f,
                        0.0f, 1.0f, 0.0f, 0.0f,
                        0.0f, 0.0f, 1.0f, -1.0f / sizeList.X,
                        0.0f, 0.0f, 0.0f, 1.0f);

            visual.TransformMatrix =
                               Matrix4x4.CreateTranslation(-sizeList.X / 2, -sizeList.Y / 2, 0f) *      // Translate to origin
                               perspective *                                                            // Apply perspective at origin
                               Matrix4x4.CreateTranslation(sizeList.X / 2, sizeList.Y / 2, 0f);         // Translate back to original position
        }

        private void _rootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePerspective(_rootGrid.GetVisual());
        }

        private async Task NextAsync()
        {
            if (Views.Count > 1)
            {
                var nextIndex = Views.Count - DisplayIndex - 1;
                var backView = Views[nextIndex];

                _rootGrid.Children.Insert(0, backView);

                if (_rootGrid.Children.Count == 3)
                {
                    _rootGrid.Children.RemoveAt(2);
                }

                var frontView = _rootGrid.Children[_rootGrid.Children.Count - 1] as FrameworkElement;

                await frontView.WaitForNonZeroSizeAsync();
                await backView.WaitForNonZeroSizeAsync();

                var frontViewVisual = _rootGrid.Children[1].GetVisual();
                var backViewVisual = _rootGrid.Children[0].GetVisual();

                backViewVisual.CenterPoint = new Vector3((float)(backView.ActualWidth / 2f), (float)(backView.ActualHeight / 2f), 0f);
                frontViewVisual.CenterPoint = new Vector3((float)(frontView.ActualWidth / 2f), (float)(frontView.ActualHeight / 2f), 0f);

                backViewVisual.RotationAngleInDegrees = 180f;

                backViewVisual.Size = new Vector2((float)backView.ActualWidth / 2, (float)backView.ActualHeight / 2);
                frontViewVisual.Size = new Vector2((float)frontView.ActualWidth / 2, (float)frontView.ActualHeight / 2);

                var linear = _compositor.CreateLinearEasingFunction();
                var delta = GetDeltaDegreeByDirection();

                var frontViewAnimation = _compositor.CreateScalarKeyFrameAnimation();
                frontViewAnimation.InsertKeyFrame(1f, frontViewVisual.RotationAngleInDegrees + delta, linear);
                frontViewAnimation.Duration = TimeSpan.FromMilliseconds(200);

                var backViewAnimation = _compositor.CreateScalarKeyFrameAnimation();
                backViewAnimation.InsertKeyFrame(1f, backViewVisual.RotationAngleInDegrees + delta, linear);
                backViewAnimation.Duration = TimeSpan.FromMilliseconds(200);

                SetRotatioinAxis(frontViewVisual);
                SetRotatioinAxis(backViewVisual);

                var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

                frontViewVisual.StartAnimation("RotationAngleInDegrees", frontViewAnimation);
                backViewVisual.StartAnimation("RotationAngleInDegrees", backViewAnimation);

                batch.Completed += (sender, e) =>
                 {
                     Canvas.SetZIndex(backView, _zindex++);

                     frontViewAnimation.InsertKeyFrame(1f, frontViewVisual.RotationAngleInDegrees + delta, linear);
                     backViewAnimation.InsertKeyFrame(1f, backViewVisual.RotationAngleInDegrees + delta, linear);

                     frontViewVisual.StartAnimation("RotationAngleInDegrees", frontViewAnimation);
                     backViewVisual.StartAnimation("RotationAngleInDegrees", backViewAnimation);
                 };
                batch.End();
            }
        }

        private void SetRotatioinAxis(Visual visual)
        {
            var rotationAxis = (int)RotationAxis;
            visual.RotationAxis = new Vector3((rotationAxis & 2) != 0 ? 1 : 0, (rotationAxis & 1) != 0 ? 1 : 0, 0f);
        }

        private float GetDeltaDegreeByDirection()
        {
            return FlipDirection == Direction.FrontToBack ? 90f : -90f;
        }

        private void _rootGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!AllowTapToFlip) return;

            DisplayIndex++;
        }
    }
}
