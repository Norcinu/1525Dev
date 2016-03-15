using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows;

namespace PDTUtils.CustomCharting
{
    class StevesPie : PieSeries
    {
        protected static readonly DependencyProperty ActualLegendItemStyleProperty = DependencyProperty.Register(DataPointSeries.ActualLegendItemStyleName, typeof(Style), typeof(DataPointSeries), null);
        protected Style ActualLegendItemStyle
        {
            get
            {
                return (base.GetValue(ActualLegendItemStyleProperty) as Style);
            }
            set
            {
                base.SetValue(ActualLegendItemStyleProperty, value);
            }
        }
    }
}
