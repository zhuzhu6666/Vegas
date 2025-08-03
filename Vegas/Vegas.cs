using System;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None), 
     Cloud("UpperBand", "LowerBand", Opacity = 0.2),
     Cloud("UpperBand2", "LowerBand2", Opacity = 0.2)]
    public class Vegas : Indicator
    {
        [Parameter("EMA1周期", DefaultValue = 21)]
        public int EMA1Period { get; set; }

        [Parameter("EMA2周期", DefaultValue = 144)]
        public int EMA2Period { get; set; }

        [Parameter("EMA3周期", DefaultValue = 169)]
        public int EMA3Period { get; set; }

        [Parameter("EMA4周期", DefaultValue = 576)]
        public int EMA4Period { get; set; }

        [Parameter("EMA5周期", DefaultValue = 676)]
        public int EMA5Period { get; set; }

        [Output("EMA1", LineColor = "Blue", Thickness = 1, LineStyle = LineStyle.Solid)]
        public IndicatorDataSeries EMA1 { get; set; }

        [Output("EMA2", LineColor = "Red", Thickness = 1, LineStyle = LineStyle.Solid)]
        public IndicatorDataSeries EMA2 { get; set; }

        [Output("EMA3", LineColor = "Green", Thickness = 1, LineStyle = LineStyle.Solid)]
        public IndicatorDataSeries EMA3 { get; set; }

        [Output("EMA4", LineColor = "Orange", Thickness = 1, LineStyle = LineStyle.Solid)]
        public IndicatorDataSeries EMA4 { get; set; }

        [Output("EMA5", LineColor = "Purple", Thickness = 1, LineStyle = LineStyle.Solid)]
        public IndicatorDataSeries EMA5 { get; set; }

        [Output("UpperBand", LineColor = "Transparent")]
        public IndicatorDataSeries UpperBand { get; set; }

        [Output("LowerBand", LineColor = "Transparent")]
        public IndicatorDataSeries LowerBand { get; set; }

        [Output("UpperBand2", LineColor = "Transparent")]
        public IndicatorDataSeries UpperBand2 { get; set; }

        [Output("LowerBand2", LineColor = "Transparent")]
        public IndicatorDataSeries LowerBand2 { get; set; }

        [Parameter("显示通道", DefaultValue = true, Group = "通道")]
        public bool ShowTunnel { get; set; }

        [Parameter("通道颜色", DefaultValue = "LightGray", Group = "通道")]
        public string TunnelColor { get; set; }

        private readonly List<ExponentialMovingAverage> _emas = new List<ExponentialMovingAverage>();
        private readonly List<IndicatorDataSeries> _emaOutputs = new List<IndicatorDataSeries>();
        private readonly List<int> _emaPeriods = new List<int>();

        protected override void Initialize()
        {
            _emaPeriods.AddRange(new[] { EMA1Period, EMA2Period, EMA3Period, EMA4Period, EMA5Period });
            _emaOutputs.AddRange(new[] { EMA1, EMA2, EMA3, EMA4, EMA5 });

            for (int i = 0; i < _emaPeriods.Count; i++)
            {
                _emas.Add(Indicators.ExponentialMovingAverage(Bars.ClosePrices, _emaPeriods[i]));
            }
        }

        public override void Calculate(int index)
        {
            // Calculate方法现在只负责计算指标值
            for (int i = 0; i < _emas.Count; i++)
            {
                _emaOutputs[i][index] = _emas[i].Result[index];
            }

            // 计算通道的上下边界
            if (ShowTunnel)
            {
                // 第一个通道: EMA2 和 EMA3
                var ema2Value = EMA2[index];
                var ema3Value = EMA3[index];

                UpperBand[index] = Math.Max(ema2Value, ema3Value);
                LowerBand[index] = Math.Min(ema2Value, ema3Value);

                // 第二个通道: EMA4 和 EMA5
                var ema4Value = EMA4[index];
                var ema5Value = EMA5[index];

                UpperBand2[index] = Math.Max(ema4Value, ema5Value);
                LowerBand2[index] = Math.Min(ema4Value, ema5Value);
            }
            else
            {
                // 如果不显示，则设置为NaN以隐藏
                UpperBand[index] = double.NaN;
                LowerBand[index] = double.NaN;
                UpperBand2[index] = double.NaN;
                LowerBand2[index] = double.NaN;
            }
        }
    }
}