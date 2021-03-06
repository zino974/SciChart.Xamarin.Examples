﻿using SciChart.Examples.Demo.Data;
using SciChart.Examples.Demo.Fragments.Base;
using SciChart.iOS.Charting;
using UIKit;
using Xamarin.Examples.Demo.iOS.Helpers;
using Xamarin.Examples.Demo.iOS.Resources.Layout;
using Xamarin.Examples.Demo.iOS.Views.Base;

namespace Xamarin.Examples.Demo.iOS.Views.Examples
{
    [ExampleDefinition("Series Selection")]
    public class SeriesSelectionView : ExampleBaseView
    {
        private readonly SingleChartViewLayout _exampleViewLayout = SingleChartViewLayout.Create();

        public SCIChartSurface Surface;

        public override UIView ExampleView => _exampleViewLayout;

        private const int SeriesPointCount = 50;
        private const int SeriesCount = 80;

        protected override void UpdateFrame()
        {
            _exampleViewLayout.SciChartSurfaceView.Frame = _exampleViewLayout.Frame;
            _exampleViewLayout.SciChartSurfaceView.TranslatesAutoresizingMaskIntoConstraints = true;
        }

        protected override void InitExampleInternal()
        {
            Surface = new SCIChartSurface(_exampleViewLayout.SciChartSurfaceView);
            StyleHelper.SetSurfaceDefaultStyle(Surface);

            var axisStyle = StyleHelper.GetDefaultAxisStyle();
            var xAxis = new SCINumericAxis
            {
                IsXAxis = true,
                AutoRange = SCIAutoRangeMode.Always,
                Style = axisStyle
            };
            var leftAxis = new SCINumericAxis
            {
                AxisAlignment = SCIAxisAlignmentMode.Left,
                AxisId = SCIAxisAlignmentMode.Left.ToString(),
                Style = axisStyle
            };
            var rightAxis = new SCINumericAxis
            {
                AxisAlignment = SCIAxisAlignmentMode.Right,
                AxisId = SCIAxisAlignmentMode.Right.ToString(),
                Style = axisStyle
            };

            Surface.AttachAxis(xAxis, true);
            Surface.AttachAxis(leftAxis, false);
            Surface.AttachAxis(rightAxis, false);

            var initialColor = UIColor.Blue;

            for (var i = 0; i < SeriesCount; i++)
            {
                var alignment = i % 2 == 0 ? SCIAxisAlignmentMode.Left : SCIAxisAlignmentMode.Right;
                var dataSeries = GenerateDataSeries(alignment, i);

                var rs = new SCIFastLineRenderableSeries
                {
                    DataSeries = dataSeries,
                    YAxisId = alignment.ToString(),
                    Style = {LinePen = new SCIPenSolid(initialColor, 2f)}
                };

                // Colors are incremented for visual purposes only
                var newR = initialColor.R() == 255 ? 255 : initialColor.R() + 5;
                var newB = initialColor.B() == 0 ? 0 : initialColor.B() - 2;
                initialColor = UIColor.FromRGB((byte)newR, initialColor.G(), (byte)newB);

                Surface.AttachRenderableSeries(rs);
            }

            //var selectedStrokeStyle = new PenStyle.Builder(Activity).WithColor(Color.White).WithThickness(4f).Build();
            //var selectedPointMarker = new EllipsePointMarker
            //{
            //    Width = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 10, Context.Resources.DisplayMetrics),
            //    Height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 10, Context.Resources.DisplayMetrics),
            //    FillStyle = new SolidBrushStyle(Color.Argb(0xFF, 0xFF, 0x00, 0xDC)),
            //    StrokeStyle = new PenStyle.Builder(Activity).WithColor(Color.White).WithThickness(1f).Build()
            //};

            Surface.ChartModifier = new SCIModifierGroup(new ISCIChartModifierProtocol[]
            {
                //new SeriesSelectionModifier {SelectedSeriesStyle = new SelectedSeriesStyle(selectedStrokeStyle, selectedPointMarker)}
            });

            Surface.InvalidateElement();
        }

        private static IDataSeries GenerateDataSeries(SCIAxisAlignmentMode alignment, int index)
        {
            var dataSeries = new XyDataSeries<double, double> { SeriesName = $"Series {index}" };

            var gradient = alignment == SCIAxisAlignmentMode.Right ? index : -index;
            var start = alignment == SCIAxisAlignmentMode.Right ? 0d : 14000d;

            var straightLine = DataManager.Instance.GetStraightLine(gradient, start, SeriesPointCount);

            dataSeries.Append(straightLine.XData, straightLine.YData);

            return dataSeries;
        }
    }
}