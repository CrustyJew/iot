// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Iot.Device.HX711;

namespace Iot.Device.HX711.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new HX711Settings(25, 24);
            var hx711 = new HX711(settings);
            hx711.SetGain(Gain.x64);

            Console.WriteLine($"TARE: {hx711.Tare(50)}");

            Console.ReadLine();

            Console.WriteLine($"Calibration: {hx711.Calibrate(Iot.Units.Weight.WeightSystem.Metric, 100)}");

            while (true) {
                Console.WriteLine($"{hx711.MetricWeight.Grams.ToString("0.00")}g");
                Thread.Sleep(500);
            }

        }
    }
}
