using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ScanCode.UserControls
{
    /// <inheritdoc />
    /// <summary>
    ///     A control featuring a range of loading indicating animations.
    /// </summary>
    [TemplatePart(Name = TemplateBorderName, Type = typeof(Border))]
    public class LoadingIndicator : Control
    {
        internal const string TemplateBorderName = "PART_Border";

        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(LoadingIndicator), new PropertyMetadata(1d,
                OnSpeedRatioChanged));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(LoadingIndicator), new PropertyMetadata(true,
                OnIsActiveChanged));

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(LoadingIndicatorMode), typeof(LoadingIndicator),
            new PropertyMetadata(default(LoadingIndicatorMode)));

        // ReSharper disable once InconsistentNaming
        protected Border PART_Border;

        static LoadingIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingIndicator),
                new FrameworkPropertyMetadata(typeof(LoadingIndicator)));
        }

        public LoadingIndicator()
        {
        }

        public LoadingIndicatorMode Mode
        {
            get => (LoadingIndicatorMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        /// <summary>
        ///     Get/set the speed ratio of the animation.
        /// </summary>
        public double SpeedRatio
        {
            get => (double)GetValue(SpeedRatioProperty);
            set => SetValue(SpeedRatioProperty, value);
        }

        /// <summary>
        ///     Get/set whether the loading indicator is active.
        /// </summary>
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        private static void OnSpeedRatioChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var li = (LoadingIndicator)o;

            if (li.PART_Border == null || li.IsActive == false)
            {
                return;
            }

            SetStoryBoardSpeedRatio(li.PART_Border, (double)e.NewValue);
        }

        private static void OnIsActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var li = (LoadingIndicator)o;

            if (li.PART_Border == null)
            {
                return;
            }

            if ((bool)e.NewValue == false)
            {
                VisualStateManager.GoToElementState(li.PART_Border, IndicatorVisualStateNames.InactiveState.Name,
                    false);
                li.PART_Border.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            }
            else
            {
                VisualStateManager.GoToElementState(li.PART_Border, IndicatorVisualStateNames.ActiveState.Name, false);

                li.PART_Border.SetCurrentValue(VisibilityProperty, Visibility.Visible);

                SetStoryBoardSpeedRatio(li.PART_Border, li.SpeedRatio);
            }
        }

        private static void SetStoryBoardSpeedRatio(FrameworkElement element, double speedRatio)
        {
            var activeStates = element.GetActiveVisualStates();
            foreach (var activeState in activeStates) activeState.Storyboard.SetSpeedRatio(element, speedRatio);
        }

        /// <inheritdoc />
        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code
        ///     or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Border = (Border)GetTemplateChild(TemplateBorderName);

            if (PART_Border == null)
            {
                return;
            }

            VisualStateManager.GoToElementState(PART_Border,
                IsActive
                    ? IndicatorVisualStateNames.ActiveState.Name
                    : IndicatorVisualStateNames.InactiveState.Name, false);

            SetStoryBoardSpeedRatio(PART_Border, SpeedRatio);

            PART_Border.SetCurrentValue(VisibilityProperty, IsActive ? Visibility.Visible : Visibility.Collapsed);
        }
    }

    internal class IndicatorVisualStateNames : MarkupExtension
    {
        private static IndicatorVisualStateNames _activeState;
        private static IndicatorVisualStateNames _inactiveState;

        public static IndicatorVisualStateNames ActiveState =>
            _activeState ?? (_activeState = new IndicatorVisualStateNames("Active"));

        public static IndicatorVisualStateNames InactiveState =>
            _inactiveState ?? (_inactiveState = new IndicatorVisualStateNames("Inactive"));

        private IndicatorVisualStateNames(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public string Name { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Name;
        }
    }

    public enum LoadingIndicatorMode
    {
        [Description("LoadingIndicatorWaveStyle")]
        Wave,

        [Description("LoadingIndicatorArcsStyle")]
        Arcs,

        [Description("LoadingIndicatorArcsRingStyle")]
        ArcsRing,

        [Description("LoadingIndicatorDoubleBounceStyle")]
        DoubleBounce,

        [Description("LoadingIndicatorFlipPlaneStyle")]
        FlipPlane,

        [Description("LoadingIndicatorPulseStyle")]
        Pulse,

        [Description("LoadingIndicatorRingStyle")]
        Ring,

        [Description("LoadingIndicatorThreeDotsStyle")]
        ThreeDots
    }

    internal static class LoadingIndicatorModeUtility
    {
        public static string GetDescription(this LoadingIndicatorMode value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
        }
    }

    internal class IndicatorVisualStateGroupNames : MarkupExtension
    {
        private static IndicatorVisualStateGroupNames _internalActiveStates;
        private static IndicatorVisualStateGroupNames _sizeStates;

        public static IndicatorVisualStateGroupNames ActiveStates =>
            _internalActiveStates ?? (_internalActiveStates = new IndicatorVisualStateGroupNames("ActiveStates"));

        public static IndicatorVisualStateGroupNames SizeStates =>
            _sizeStates ?? (_sizeStates = new IndicatorVisualStateGroupNames("SizeStates"));

        private IndicatorVisualStateGroupNames(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public string Name { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Name;
        }
    }

    internal static class VisualStateUtilities
    {
        public static IEnumerable<VisualStateGroup> GetActiveVisualStateGroups(this FrameworkElement element) =>
            element.GetVisualStateGroupsByName(IndicatorVisualStateGroupNames.ActiveStates.Name);

        public static IEnumerable<VisualState> GetActiveVisualStates(this FrameworkElement element) => element
            .GetActiveVisualStateGroups().GetAllVisualStatesByName(IndicatorVisualStateNames.ActiveState.Name);

        public static IEnumerable<VisualState> GetAllVisualStatesByName(
            this IEnumerable<VisualStateGroup> visualStateGroups, string name) =>
            visualStateGroups.SelectMany(vsg => vsg.GetVisualStatesByName(name));

        public static IEnumerable<VisualState> GetVisualStatesByName(this VisualStateGroup visualStateGroup,
            string name)
        {
            if (visualStateGroup is null)
            {
                return null;
            }

            var visualStates = visualStateGroup.GetVisualStates();

            return string.IsNullOrWhiteSpace(name) ? visualStates : visualStates?.Where(vs => vs.Name == name);
        }

        public static IEnumerable<VisualStateGroup> GetVisualStateGroupsByName(this FrameworkElement element,
            string name)
        {
            var groups = VisualStateManager.GetVisualStateGroups(element);

            if (groups is null)
            {
                return null;
            }

            IEnumerable<VisualStateGroup> castedVisualStateGroups;

            try
            {
                castedVisualStateGroups = groups.Cast<VisualStateGroup>().ToArray();
                if (!castedVisualStateGroups.Any())
                {
                    return null;
                }
            }
            catch (InvalidCastException)
            {
                return null;
            }

            return string.IsNullOrWhiteSpace(name)
                ? castedVisualStateGroups
                : castedVisualStateGroups.Where(vsg => vsg.Name == name);
        }

        public static IEnumerable<VisualState> GetVisualStates(this VisualStateGroup visualStateGroup)
        {
            if (visualStateGroup is null)
            {
                return null;
            }

            return visualStateGroup.States.Count == 0 ? null : visualStateGroup.States.Cast<VisualState>();
        }
    }
}
