// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Iot.Units.Weight;

namespace Iot.Device.HX711
{
    public class HX711 : IDisposable
    {
        private Gain _gain;
        private readonly HX711Settings _settings;
        private GpioController _controller;
        private SemaphoreSlim _readLock = new SemaphoreSlim(1, 1);
        public int TareValue { get; set; }

        public WeightSystem CalibrationWeightSystem => _settings.CalibrationWeightSystem;
        public ImperialUnit CalibrationImperialUnit => _settings.CalibrationImperialUnit;
        public MetricUnit CalibrationMetricUnit => _settings.CalibrationMetricUnit;
        public double CalibrationValue => _settings.CalibrationValue;

        /// <summary>
        /// Gets the Weight Units reading
        /// </summary>
        public Weight Weight => ReadWeight();
        /// <summary>
        /// Gets the ImperialWeight reading. Will still have to do full conversions if calibrated with metric.
        /// </summary>
        public ImperialWeight ImperialWeight => ReadImperialWeight();
        /// <summary>
        /// Gets the MetricWeight reading. Will still have to do full conversions if calibrated with imperial.
        /// </summary>
        public MetricWeight MetricWeight => ReadMetricWeight();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="gain"></param>
        public HX711(HX711Settings settings, int tareValue = 0)
        {
            _settings = settings;
            TareValue = tareValue;
            _controller = new GpioController(settings.PinNumberingScheme);
            _controller.OpenPin(settings.DTPin, PinMode.Input);
            _controller.OpenPin(settings.SCKPin, PinMode.Output);

            //throw away to set gain properly

            _readLock.Wait();
            try
            {
                ReadRaw();
            }
            finally
            {
                _readLock.Release();
            }
        }
        private bool ReadBit()
        {
            _controller.Write(_settings.SCKPin, PinValue.High);
            _controller.Write(_settings.SCKPin, PinValue.Low);
            
            var pin = _controller.Read(_settings.DTPin);
            return  pin == PinValue.High ? true : false;
        }
        private byte ReadByte()
        {
            byte toReturn = new byte();

            for (int i = 0; i < 8; i++)
            {
                byte bit = (byte)(ReadBit() ? 0x01 : 0x00);
                toReturn = (byte)((toReturn << 1) | bit);
            }
            return toReturn;
        }

        private int ReadRaw()
        {
            //wait for data ready
            while(_controller.Read(_settings.DTPin) != PinValue.Low)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(.01));
            }
            List<byte> bytes = new List<byte>();
            int rawVal = 0;
            rawVal = ReadByte();
            rawVal = (rawVal << 8) | ReadByte();
            rawVal = (rawVal << 8) | ReadByte();

            for(int i = 0; i < (int) _gain; i++)
            {
                ReadBit();
            }
            //value comes out as twos complement, reverse that be actual integer value
            rawVal = -(rawVal & 0x800000) + (rawVal & 0x7fffff);
            
            return rawVal;
        }
        /// <summary>
        /// Sets gain level. Will cause a 50ms sleep to clean up garbage data.
        /// </summary>
        /// <param name="gain"></param>
        public void SetGain(Gain gain)
        {
            _readLock.Wait();
            try
            {
                _gain = gain;
                //cycle to next calculation interval so gain is ready and set for next reading
                ReadRaw();
                //Changing gain causes a bunch of garbage data for ~50ms so wait it out here
                Thread.Sleep(50); 
            }
            finally
            {
                _readLock.Release();
            }
        }
        /// <summary>
        /// Sets the Tare value based on the average of reading <paramref name="readings"/> values
        /// </summary>
        /// <param name="readings">Number of readings to average together. Defaults to 10</param>
        /// <returns></returns>
        public int Tare(int readings = 10)
        {
            _readLock.Wait();
            try
            {
                List<int> values = new List<int>(readings);
                for(int i = 0; i < readings; i++)
                {
                    values.Add(ReadRaw());
                }
                TareValue = values.Sum() / readings;
                return TareValue;
            }
            finally
            {
                _readLock.Release();
            }
        }
        /// <summary>
        /// Re-calibrates HX711 using the given weight system, unit, and weight value. Returns calibration factor.
        /// </summary>
        /// <param name="weightSystem"></param>
        /// <param name="calibrationWeight"></param>
        /// <param name="imperialCalibrationUnit">Only used if <paramref name="weightSystem"/> is imperial</param>
        /// <param name="metricCalibrationUnit">Only used if <paramref name="weightSystem"/> is metric</param>
        /// <param name="readings">Number of readings to average together. Defaults to 10</param>
        /// <returns></returns>
        public double Calibrate(WeightSystem weightSystem, double calibrationWeight, ImperialUnit imperialCalibrationUnit = ImperialUnit.Pounds, MetricUnit metricCalibrationUnit = MetricUnit.Grams, int readings = 10)
        {
            _readLock.Wait();
            try
            {
                List<double> values = new List<double>(readings);
                for(int i = 0; i < readings; i++)
                {
                    values.Add((ReadRaw() - TareValue) / calibrationWeight);
                }
                _settings.CalibrationValue = values.Average();

                _settings.CalibrationWeightSystem = weightSystem;
                _settings.CalibrationMetricUnit = metricCalibrationUnit;
                _settings.CalibrationImperialUnit = imperialCalibrationUnit;
                return _settings.CalibrationValue;
            }
            finally
            {
                _readLock.Release();
            }
        }

        private Weight ReadWeight()
        {
            _readLock.Wait();
            try
            {
                int raw = ReadRaw();
                double unitValue = (raw - TareValue) / _settings.CalibrationValue;
                IWeightSystem baseWeight = null;
                if(_settings.CalibrationWeightSystem == WeightSystem.Metric)
                {
                    baseWeight = new MetricWeight(unitValue, _settings.CalibrationMetricUnit);
                }
                else if (_settings.CalibrationWeightSystem == WeightSystem.Imperial)
                {
                    baseWeight = new ImperialWeight(unitValue, _settings.CalibrationImperialUnit);
                }
                else
                {
                    throw new Exception("HX711 was uncalibrated or not calibrated properly and can't convert to a weight!");
                }

                return new Weight(baseWeight);
            }
            finally
            {
                _readLock.Release();
            }
        }


        private ImperialWeight ReadImperialWeight()
        {
            _readLock.Wait();
            try
            {
                int raw = ReadRaw();
                double unitValue = (raw - TareValue) / _settings.CalibrationValue;

                if (_settings.CalibrationWeightSystem == WeightSystem.Imperial)
                {
                    return new ImperialWeight(unitValue, _settings.CalibrationImperialUnit);
                }
                else if (_settings.CalibrationWeightSystem == WeightSystem.Metric)
                {
                    return new ImperialWeight( new MetricWeight(unitValue, _settings.CalibrationMetricUnit));
                }
                else
                {
                    throw new Exception("HX711 was uncalibrated or not calibrated properly and can't convert to a weight!");
                }
            }
            finally
            {
                _readLock.Release();
            }
        }

        private MetricWeight ReadMetricWeight()
        {
            _readLock.Wait();
            try
            {
                int raw = ReadRaw();
                double unitValue = (raw - TareValue) / _settings.CalibrationValue;
                if (_settings.CalibrationWeightSystem == WeightSystem.Metric)
                {
                    return new MetricWeight(unitValue, _settings.CalibrationMetricUnit);
                }
                
                else if (_settings.CalibrationWeightSystem == WeightSystem.Imperial)
                {
                    return new MetricWeight(new ImperialWeight(unitValue, _settings.CalibrationImperialUnit));
                }
                else
                {
                    throw new Exception("HX711 was uncalibrated or not calibrated properly and can't convert to a weight!");
                }
            }
            finally
            {
                _readLock.Release();
            }
        }
        public void Dispose()
        {
        }
    }
}
