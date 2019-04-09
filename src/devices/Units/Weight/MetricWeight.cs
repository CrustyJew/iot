namespace Iot.Units.Weight
{
    public struct MetricWeight : IWeightSystem
    {
        private const double PICO_CONVERSION_FACTOR = 1000000000000.0;
        private const double NANO_CONVERSION_FACTOR = 1000000000.0;
        private const double MICRO_CONVERSION_FACTOR = 1000000.0;
        private const double MILLI_CONVERSION_FACTOR = 1000.0;
        private const double KILO_CONVERSION_FACTOR = 1000.0;
        private const double MEGA_CONVERSION_FACTOR = 1000000.0;

        private const double POUNDS_CONVERSION_FACTOR = 453.59237;

        private double BaseValue
        {
            get { return Grams; }
            set
            {
                _grams = value;
                _picograms = value * PICO_CONVERSION_FACTOR;
                _nanograms = value * NANO_CONVERSION_FACTOR;
                _micrograms = value * MICRO_CONVERSION_FACTOR;
                _milligrams = value * MILLI_CONVERSION_FACTOR;
                _kilograms = value / KILO_CONVERSION_FACTOR;
                _megagrams = value / MEGA_CONVERSION_FACTOR;
            }
        }

        private double _picograms;
        public double Picograms
        {
            get { return _picograms; }
            set
            {
                BaseValue = value / PICO_CONVERSION_FACTOR;
            }
        }
        private double _nanograms;
        public double Nanograms
        {
            get { return _nanograms; }
            set
            {
                BaseValue = value / NANO_CONVERSION_FACTOR;
            }
        }
        private double _micrograms;
        public double Micrograms
        {
            get { return _micrograms; }
            set
            {
                BaseValue = value / MICRO_CONVERSION_FACTOR;
            }
        }
        private double _milligrams;
        public double Milligrams
        {
            get { return _milligrams; }
            set
            {
                BaseValue = value / MILLI_CONVERSION_FACTOR;
            }
        }

        private double _grams;
        public double Grams
        {
            get { return _grams; }
            set
            {
                BaseValue = value;
            }
        }

        private double _kilograms;
        public double Kilograms
        {
            get { return _kilograms; }
            set
            {
                BaseValue = value / KILO_CONVERSION_FACTOR;
            }
        }

        private double _megagrams;
        public double Megagrams
        {
            get { return _megagrams; }
            set
            {
                BaseValue = value / MEGA_CONVERSION_FACTOR;
            }
        }
        public double Tonnes
        {
            get { return _megagrams; }
            set
            {
                BaseValue = value / MEGA_CONVERSION_FACTOR;
            }
        }

        public WeightSystem System => WeightSystem.Metric;

        /// <summary>
        /// New Metric Weight measurement from grams
        /// </summary>
        /// <param name="grams"></param>
        public MetricWeight(double grams)
        {
            _picograms = _nanograms = _micrograms = _milligrams = _grams = _kilograms = _megagrams = 0;

            BaseValue = grams;
        }

        /// <summary>
        /// New Metric Weight measurement from existing weight measurement
        /// </summary>
        /// <param name="weightSystem"></param>
        public MetricWeight(IWeightSystem weightSystem)
        {
            _picograms = _nanograms = _micrograms = _milligrams = _grams = _kilograms = _megagrams = 0;


            if (weightSystem is ImperialWeight iw)
            {
                BaseValue = iw.Pounds / POUNDS_CONVERSION_FACTOR;
            }
            else if (weightSystem is MetricWeight mw)
            {
                _picograms = mw.Picograms;
                _nanograms = mw.Nanograms;
                _micrograms = mw.Micrograms;
                _milligrams = mw.Milligrams;
                _grams = mw.Grams;
                _kilograms = mw.Kilograms;
                _megagrams = mw.Megagrams;
            }
            else
            {
                throw new UnknownConversionException($"Unknown conversion from '{weightSystem.GetType().ToString()}' to 'Imperial'");
            }
        }
        /// <summary>
        /// New Metric Weight measurement from <paramref name="value"/> in <paramref name="unit"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        public MetricWeight(double value, MetricUnit unit)
        {
            _picograms = _nanograms = _micrograms = _milligrams = _grams = _kilograms = _megagrams = 0;

            switch (unit)
            {
                case MetricUnit.Grams:
                    {
                        BaseValue = value;
                        break;
                    }
                case MetricUnit.Kilogram:
                    {
                        Kilograms = value;
                        break;
                    }
                case MetricUnit.Megagram:
                case MetricUnit.Tonne:
                    {
                        Megagrams = value;
                        break;
                    }
                case MetricUnit.Microgram:
                    {
                        Micrograms = value;
                        break;
                    }
                case MetricUnit.Milligram:
                    {
                        Milligrams = value;
                        break;
                    }
                case MetricUnit.Nanogram:
                    {
                        Nanograms = value;
                        break;
                    }
                case MetricUnit.Picogram:
                    {
                        Picograms = value;
                        break;
                    }
                default:
                    {
                        throw new UnknownConversionException($"IoT Library Exception! Someone forgot to add '{unit.ToString()}' to the constructor!");
                    }
            }
        }
    }
}
