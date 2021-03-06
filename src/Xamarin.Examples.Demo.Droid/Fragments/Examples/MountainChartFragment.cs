﻿using System;
using Android.Graphics;
using Android.Util;
using SciChart.Charting.Model;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Modifiers;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Data.Model;
using SciChart.Drawing.Common;
using SciChart.Examples.Demo.Data;
using SciChart.Examples.Demo.Fragments.Base;
using Xamarin.Examples.Demo.Droid.Fragments.Base;

namespace Xamarin.Examples.Demo.Droid.Fragments.Examples
{
    [ExampleDefinition("Mountain Chart")]
    public class MountainChartFragment : ExampleBaseFragment
    {
        private SciChartSurface Surface => View.FindViewById<SciChartSurface>(Resource.Id.chart);

        public override int ExampleLayoutId => Resource.Layout.Example_Single_Chart_Fragment;

        protected override void InitExample()
        {
            var priceData = DataManager.Instance.GetPriceDataIndu();

            var dataSeries = new XyDataSeries<DateTime, double>();
            dataSeries.Append(priceData.TimeData, priceData.CloseData);

            var xAxis = new DateAxis(Activity) {GrowBy = new DoubleRange(0.1, 0.1)};
            var yAxis = new NumericAxis(Activity) {GrowBy = new DoubleRange(0.1, 0.1)};

            var renderableSeries = new FastMountainRenderableSeries
            {
                DataSeries = dataSeries,
                StrokeStyle = new PenStyle.Builder(Activity).WithColor(Color.Argb(0xAA, 0xFF, 0xC9, 0xA8)).WithThickness(2, ComplexUnitType.Dip).Build(),
                AreaStyle = new LinearGradientBrushStyle(0, 0, 1, 1, Color.Argb(0xAA, 0xFF, 0x8D, 0x42), Color.Argb(0x88, 0x09, 0x0E, 0x11), TileMode.Clamp)
            };

            using (Surface.SuspendUpdates())
            {
                Surface.XAxes.Add(xAxis);
                Surface.YAxes.Add(yAxis);
                Surface.RenderableSeries.Add(renderableSeries);

                Surface.ChartModifiers = new ChartModifierCollection
                {
                    new ZoomPanModifier(),
                    new PinchZoomModifier(),
                    new ZoomExtentsModifier(),
                };
            }
        }
    }
}