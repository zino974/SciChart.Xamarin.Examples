﻿using System.Timers;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Java.Lang;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Core.Model;
using SciChart.Drawing.Common;
using SciChart.Examples.Demo.Data;
using SciChart.Examples.Demo.Fragments.Base;
using Xamarin.Examples.Demo.Droid.Fragments.Base;
using Math = Java.Lang.Math;
using Random = System.Random;
using Timer = System.Timers.Timer;

namespace Xamarin.Examples.Demo.Droid.Fragments.Examples
{
    [ExampleDefinition("Performance Demo")]
    public class PerformanceDemoFragment : ExampleBaseFragment
    {
        private SciChartSurface Surface => View.FindViewById<SciChartSurface>(Resource.Id.chart);

        public override int ExampleLayoutId => Resource.Layout.Example_Single_Realtime_Chart_Fragment;

        private static readonly int MaxPointCount = CalculateMaxPointCountToDisplay();
        private const int TimerInterval = 10;
        private const int BufferSize = 1000;

        private readonly MovingAverage _maLow = new MovingAverage(200);
        private readonly MovingAverage _maHigh = new MovingAverage(1000);

        private int _xValue = 0;
        private double _yValue = 10;

        private readonly IntegerValues _xValues = new IntegerValues(BufferSize);
        private readonly FloatValues _firstYValues = new FloatValues(BufferSize);
        private readonly FloatValues _secondYValues = new FloatValues(BufferSize);
        private readonly FloatValues _thirdYValues = new FloatValues(BufferSize);

        private readonly XyDataSeries<int, float> _mainSeries = new XyDataSeries<int, float>();
        private readonly XyDataSeries<int, float> _maLowSeries = new XyDataSeries<int, float>();
        private readonly XyDataSeries<int, float> _maHighSeries = new XyDataSeries<int, float>();

        private readonly Random _random = new Random();

        private volatile bool _isRunning = false;
        private Timer _timer;

        protected override void InitExample()
        {
            View.FindViewById<Button>(Resource.Id.start).Click += (sender, args) => Start();
            View.FindViewById<Button>(Resource.Id.pause).Click += (sender, args) => Pause();
            View.FindViewById<Button>(Resource.Id.reset).Click += (sender, args) => Reset();

            var xAxis = new NumericAxis(Activity)
            {
                AutoRange = AutoRange.Always,
                AxisTitle = "X-Axis"
            };

            var yAxis = new NumericAxis(Activity)
            {
                AutoRange = AutoRange.Always,
                AxisTitle = "Y-Axis"
            };

            var rs1 = new FastLineRenderableSeries
            {
                DataSeries = _mainSeries,
                StrokeStyle = new PenStyle.Builder(Activity).WithColor(Color.Argb(0xFF, 0x40, 0x83, 0xB7)).WithThickness(2, ComplexUnitType.Dip).Build()
            };
            var rs2 = new FastLineRenderableSeries
            {
                DataSeries = _maLowSeries,
                StrokeStyle = new PenStyle.Builder(Activity).WithColor(Color.Argb(0xFF, 0xFF, 0xA5, 0x00)).WithThickness(2, ComplexUnitType.Dip).Build()
            };
            var rs3 = new FastLineRenderableSeries
            {
                DataSeries = _maHighSeries,
                StrokeStyle = new PenStyle.Builder(Activity).WithColor(Color.Argb(0xFF, 0xE1, 0x32, 0x19)).WithThickness(2, ComplexUnitType.Dip).Build()
            };

            using (Surface.SuspendUpdates())
            {
                Surface.XAxes.Add(xAxis);
                Surface.YAxes.Add(yAxis);

                Surface.RenderableSeries.Add(rs1);
                Surface.RenderableSeries.Add(rs2);
                Surface.RenderableSeries.Add(rs3);
            }

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
            if(_isRunning)
                Pause();

            using (Surface.SuspendUpdates())
            {
                _mainSeries.Clear();
                _maLowSeries.Clear();
                _maHighSeries.Clear();
            }

            _maLow.Clear();
            _maHigh.Clear();
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            lock (_timer)
            {
                if(GetPointsCount() < MaxPointCount)
                    DoAppendLoop();
                else
                    Pause();
            }
        }

        private int GetPointsCount()
        {
            return _mainSeries.Count + _maLowSeries.Count + _maHighSeries.Count;
        }

        private void DoAppendLoop()
        {
            using (Surface.SuspendUpdates())
            {
                _xValues.Clear();
                _firstYValues.Clear();
                _secondYValues.Clear();
                _thirdYValues.Clear();

                for (var i = 0; i < BufferSize; i++)
                {
                    _xValue++;
                    _yValue += _random.NextDouble() - 0.5;

                    _xValues.Add(_xValue);
                    _firstYValues.Add((float) _yValue);
                    _secondYValues.Add((float) _maLow.Push(_yValue).Current);
                    _thirdYValues.Add((float) _maHigh.Push(_yValue).Current);
                }

                _mainSeries.Append(_xValues, _firstYValues);
                _maLowSeries.Append(_xValues, _secondYValues);
                _maHighSeries.Append(_xValues, _thirdYValues);
            }
        }

        private static int CalculateMaxPointCountToDisplay()
        {
            const int oneMlnPointsRequirement = 8 + 16 + 4 + 8;
            var memorySize = GetMaxMemorySize() - 40;
            var maxPointCount = memorySize/oneMlnPointsRequirement*1000000;

            return (int) Math.Round(maxPointCount/3);
        }

        private static double GetMaxMemorySize()
        {
            return Runtime.GetRuntime().MaxMemory()/1024.0/1024.0;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            Reset();
        }
    }
}