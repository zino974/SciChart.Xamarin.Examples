﻿using System;
using System.Timers;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Data.Model;
using SciChart.Drawing.Common;
using SciChart.Examples.Demo.Fragments.Base;
using Xamarin.Examples.Demo.Droid.Fragments.Base;

namespace Xamarin.Examples.Demo.Droid.Fragments.Examples
{
    [ExampleDefinition("FIFO Chart")]
    public class FifoChartFragment : ExampleBaseFragment
    {
        private const int FifoCapacity = 200;
        private const long TimerInterval = 10;
        private const double OneOverTimeInteval = 1.0/TimerInterval;
        private const double VisibleRangeMax = FifoCapacity*OneOverTimeInteval;
        private const double GrowBy = VisibleRangeMax*0.1;

        public override int ExampleLayoutId => Resource.Layout.Example_Single_Realtime_Chart_Fragment;

        private SciChartSurface Surface => View.FindViewById<SciChartSurface>(Resource.Id.chart);

        private readonly Random _random = new Random();

        private readonly XyDataSeries<double, double> _ds1 = new XyDataSeries<double, double> { FifoCapacityValue = FifoCapacity };
        private readonly XyDataSeries<double, double> _ds2 = new XyDataSeries<double, double> { FifoCapacityValue = FifoCapacity };
        private readonly XyDataSeries<double, double> _ds3 = new XyDataSeries<double, double> { FifoCapacityValue = FifoCapacity };

        private readonly DoubleRange _xVisibleRange = new DoubleRange(-GrowBy, VisibleRangeMax + GrowBy);

        private double _t = 0;
        private volatile bool _isRunning = false;
        private Timer _timer;

        protected override void InitExample()
        {
            View.FindViewById<Button>(Resource.Id.start).Click += (sender, args) => Start();
            View.FindViewById<Button>(Resource.Id.pause).Click += (sender, args) => Pause();
            View.FindViewById<Button>(Resource.Id.reset).Click += (sender, args) => Reset();

            var xAxis = new NumericAxis(Activity)
            {
                VisibleRange = _xVisibleRange,
                AutoRange = AutoRange.Never
            };

            var yAxis = new NumericAxis(Activity)
            {
                GrowBy = new DoubleRange(0.1, 0.1),
                AutoRange = AutoRange.Always
            };
            
            var rs1 = new FastLineRenderableSeries
            {
                DataSeries = _ds1,
                StrokeStyle = new PenStyle.Builder(Activity).WithColor(Color.Argb(0xFF, 0x40, 0x83, 0xB7)).WithThickness(2, ComplexUnitType.Dip).Build()
            };

            var rs2 = new FastLineRenderableSeries
            {
                DataSeries = _ds2,
                StrokeStyle = new PenStyle.Builder(Activity).WithColor(Color.Argb(0xFF, 0xFF, 0xA5, 0x00)).WithThickness(2, ComplexUnitType.Dip).Build()
            };

            var rs3 = new FastLineRenderableSeries
            {
                DataSeries = _ds3,
                StrokeStyle = new PenStyle.Builder(Activity).WithColor(Color.Argb(0xFF, 0xE1, 0x32, 0x19)).WithThickness(2, ComplexUnitType.Dip).Build()
            };

            Surface.XAxes.Add(xAxis);
            Surface.YAxes.Add(yAxis);
            Surface.RenderableSeries.Add(rs1);
            Surface.RenderableSeries.Add(rs2);
            Surface.RenderableSeries.Add(rs3);

            Start();
        }

        private void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _timer = new Timer(TimerInterval);
            _timer.Elapsed += OnTick;
            _timer.AutoReset = true;
            _timer.Start();

            Surface.InvalidateElement();
        }

        private void Pause()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _timer.Stop();
            _timer.Elapsed -= OnTick;
            _timer = null;

            Surface.InvalidateElement();
        }

        private void Reset()
        {
            if (_isRunning)
                Pause();

            using (Surface.SuspendUpdates())
            {
                _ds1.Clear();
                _ds2.Clear();
                _ds3.Clear();
            }
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            lock (_timer)
            {
                var y1 = 3.0 * Math.Sin(((2 * Math.PI) * 1.4) * _t) + _random.NextDouble() * 0.5;
                var y2 = 2.0 * Math.Cos(((2 * Math.PI) * 0.8) * _t) + _random.NextDouble() * 0.5;
                var y3 = 1.0 * Math.Sin(((2 * Math.PI) * 2.2) * _t) + _random.NextDouble() * 0.5;

                _ds1.Append(_t, y1);
                _ds2.Append(_t, y2);
                _ds3.Append(_t, y3);

                _t += OneOverTimeInteval;

                if (_t > VisibleRangeMax)
                    _xVisibleRange.SetMinMax(_xVisibleRange.Min + OneOverTimeInteval, _xVisibleRange.Max + OneOverTimeInteval);
            }
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            Reset();
        }
    }
}