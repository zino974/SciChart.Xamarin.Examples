// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Xamarin.Examples.Demo.iOS.Resources.Layout
{
    [Register ("SingleChartViewLayout")]
    partial class SingleChartViewLayout
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        SciChart.iOS.Charting.SCIChartSurfaceView Surface { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Surface != null) {
                Surface.Dispose ();
                Surface = null;
            }
        }
    }
}