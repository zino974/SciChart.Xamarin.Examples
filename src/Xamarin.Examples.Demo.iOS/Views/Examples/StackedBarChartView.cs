﻿using SciChart.Examples.Demo.Fragments.Base;
using SciChart.iOS.Charting;
using UIKit;
using Xamarin.Examples.Demo.iOS.Helpers;
using Xamarin.Examples.Demo.iOS.Resources.Layout;
using Xamarin.Examples.Demo.iOS.Views.Base;

namespace Xamarin.Examples.Demo.iOS.Views.Examples
{
    [ExampleDefinition("Stacked Bar Chart")]
    public class StackedBarChartView :ExampleBaseView
    {
        private readonly SingleChartViewLayout _exampleViewLayout = SingleChartViewLayout.Create();

        public SCIChartSurface Surface;

        public override UIView ExampleView => _exampleViewLayout;

        protected override void UpdateFrame()
        {
            _exampleViewLayout.SciChartSurfaceView.Frame = _exampleViewLayout.Frame;
            _exampleViewLayout.SciChartSurfaceView.TranslatesAutoresizingMaskIntoConstraints = true;
        }

        protected override void InitExampleInternal()
        {
            Surface = new SCIChartSurface(_exampleViewLayout.SciChartSurfaceView);
            StyleHelper.SetSurfaceDefaultStyle(Surface);

            var xAxis = new SCINumericAxis {IsXAxis = true, AxisAlignment = SCIAxisAlignmentMode.Left};
            var yAxis = new SCINumericAxis {AxisAlignment = SCIAxisAlignmentMode.Bottom, FlipCoordinates = true};

            var yValues1 = new[] {0.0, 0.1, 0.2, 0.4, 0.8, 1.1, 1.5, 2.4, 4.6, 8.1, 11.7, 14.4, 16.0, 13.7, 10.1, 6.4, 3.5, 2.5, 5.4, 6.4, 7.1, 8.0, 9.0};
            var yValues2 = new[] {2.0, 10.1, 10.2, 10.4, 10.8, 1.1, 11.5, 3.4, 4.6, 0.1, 1.7, 14.4, 16.0, 13.7, 10.1, 6.4, 3.5, 2.5, 1.4, 0.4, 10.1, 0.0, 0.0};
            var yValues3 = new[] {20.0, 4.1, 4.2, 10.4, 10.8, 1.1, 11.5, 3.4, 4.6, 5.1, 5.7, 14.4, 16.0, 13.7, 10.1, 6.4, 3.5, 2.5, 1.4, 10.4, 8.1, 10.0, 15.0};

            var ds1 = new XyDataSeries<double, double> {SeriesName = "data 1"};
            var ds2 = new XyDataSeries<double, double> {SeriesName = "data 2"};
            var ds3 = new XyDataSeries<double, double> {SeriesName = "data 3"};

            for (var i = 0; i < yValues1.Length; i++)
            {
                ds1.Append(i, yValues1[i]);
                ds2.Append(i, yValues2[i]);
                ds3.Append(i, yValues3[i]);
            }

            //TODO finish after porting StackedSeries to Xamarin from iOS
            var series1 = GetRenderableSeries(ds1, UIColor.FromRGB(0x56, 0x78, 0x93), UIColor.FromRGB(0x56, 0x78, 0x93), UIColor.FromRGB(0x3D, 0x55, 0x68));
            var series2 = GetRenderableSeries(ds2, UIColor.FromRGB(0xAC, 0xBC, 0xCA), UIColor.FromRGB(0xAC, 0xBC, 0xCA), UIColor.FromRGB(0x43, 0x9A, 0xAF));
            var series3 = GetRenderableSeries(ds3, UIColor.FromRGB(0xDB, 0xE0, 0xE1), UIColor.FromRGB(0xDB, 0xE0, 0xE1), UIColor.FromRGB(0xB6, 0xC1, 0xC3));

            var columnsCollection = new SCIStackedVerticalColumnGroupSeries();
            columnsCollection.AddSeries(series1);
            columnsCollection.AddSeries(series2);
            columnsCollection.AddSeries(series3);

            Surface.AttachAxis(xAxis, true);
            Surface.AttachAxis(yAxis, false);
            Surface.AttachRenderableSeries(columnsCollection);

            Surface.ChartModifier = new SCIModifierGroup(new ISCIChartModifierProtocol[]
            {
                new SCICursorModifier(),
                new SCIZoomExtentsModifier()
            });

            Surface.InvalidateElement();
        }

        private SCIStackedColumnRenderableSeries GetRenderableSeries(IDataSeries dataSeries, UIColor strokeColor, UIColor fillColorStart, UIColor fillColorEnd)
        {
            return new SCIStackedColumnRenderableSeries
            {
                DataSeries = dataSeries,
                Style = new SCIColumnSeriesStyle
                {
                    DataPointWidth = 0.8,
                    BorderPen = new SCIPenSolid(strokeColor, 1f),
                    FillBrush = new SCIBrushLinearGradient(fillColorStart, fillColorEnd, SCILinearGradientDirection.Vertical)
                }
            };
        }
    }
}