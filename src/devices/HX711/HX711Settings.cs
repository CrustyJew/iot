using System;
using System.Collections.Generic;
using System.Text;
using Iot.Units.Weight;
using System.Device.Gpio;

namespace Iot.Device.HX711
{
    public class HX711Settings
    {
        public int DTPin { get; private set; }
        public int SCKPin { get; private set; }
        public bool MSB { get; private set; }
        public WeightSystem CalibrationWeightSystem { get; set; }
        public ImperialUnit CalibrationImperialUnit { get; set; }
        public MetricUnit CalibrationMetricUnit { get; set; }
        public double CalibrationValue { get; set; }
        public PinNumberingScheme PinNumberingScheme { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtPin"></param>
        /// <param name="sckPin"></param>
        /// <param name="calibrationWeightSystem">Determines which calibration unit will be used</param>
        /// <param name="calibrationImperialUnit">Only needs entered if weight system is Imperial</param>
        /// <param name="calibrationMetricUnit">Only needs entered if weight system is Metric</param>
        /// <param name="pinNumberingScheme"></param>
        /// <param name="gain"></param>
        /// <param name="msb">Boolean indicating that the HX711 outputs the most significant bit first</param>
        public HX711Settings(int dtPin, int sckPin, WeightSystem calibrationWeightSystem = WeightSystem.Metric, 
            ImperialUnit calibrationImperialUnit = ImperialUnit.Pounds, MetricUnit calibrationMetricUnit = MetricUnit.Grams, 
            double calibrationValue = 0, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical, bool msb = true)
        {
            DTPin = dtPin;
            SCKPin = sckPin;
            MSB = msb;
            CalibrationWeightSystem = calibrationWeightSystem;
            CalibrationImperialUnit = calibrationImperialUnit;
            CalibrationMetricUnit = calibrationMetricUnit;
            CalibrationValue = calibrationValue;
            PinNumberingScheme = pinNumberingScheme;
        }
    }
}
