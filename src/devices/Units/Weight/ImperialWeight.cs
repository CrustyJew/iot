namespace Iot.Units.Weight
{
    /// <summary>
    /// Imperial Weight measurement.
    /// </summary>
    public struct ImperialWeight : IWeightSystem
    {
        private const double OUNCES_CONVERSION_FACTOR = 16;
        private const double GRAINS_CONVERSION_FACTOR = 7000;
        private const double STONES_CONVERSION_FACTOR = 14;
        private const double TONS_CONVERSION_FACTOR = 2240;


        private const double GRAMS_CONVERSION_FACTOR = 453.59237;

        private double BaseValue
        {
            get { return Pounds; }
            set
            {
                _pounds = value;
                _ounces = Pounds * OUNCES_CONVERSION_FACTOR;
                _grains = Pounds * GRAINS_CONVERSION_FACTOR;
                _stones = Pounds / STONES_CONVERSION_FACTOR;
                _tons = Pounds / TONS_CONVERSION_FACTOR;
            }
        }

        public WeightSystem System => WeightSystem.Imperial;

        private double _pounds;
        public double Pounds
        {
            get { return _pounds; }
            set
            {
                BaseValue = value;
            }
        }
        private double _ounces;
        public double Ounces
        {
            get { return _ounces; }
            set
            {
                BaseValue = value / OUNCES_CONVERSION_FACTOR;
            }
        }

        private double _grains;
        public double Grains
        {
            get { return _grains; }
            set
            {
                BaseValue = value / GRAINS_CONVERSION_FACTOR;
            }
        }
        private double _stones;
        public double Stones
        {
            get { return _stones; }
            set
            {
                BaseValue = value * STONES_CONVERSION_FACTOR;
            }
        }

        private double _tons;
        public double Tons
        {
            get { return _tons; }
            set
            {
                BaseValue = value * TONS_CONVERSION_FACTOR;
            }
        }

        /// <summary>
        /// New Imperial weight measurement from pounds
        /// </summary>
        /// <param name="pounds"></param>
        public ImperialWeight(double pounds)
        {
            _pounds = _ounces = _grains = _stones = _tons = 0;

            BaseValue = pounds;
        }

        /// <summary>
        /// New Imperial Weight measurement from existing weight measurement
        /// </summary>
        /// <param name="weightSystem"></param>
        public ImperialWeight(IWeightSystem weightSystem)
        {
            _pounds = _ounces = _grains = _stones = _tons = 0;

            if (weightSystem is MetricWeight mw)
            {
                BaseValue = mw.Grams * GRAMS_CONVERSION_FACTOR;
            }
            else if (weightSystem is ImperialWeight iw)
            {
                _pounds = iw.Pounds;
                _ounces = iw.Ounces;
                _grains = iw.Grains;
                _stones = iw.Stones;
                _tons = iw.Tons;
            }
            else
            {
                throw new UnknownConversionException($"Unknown conversion from '{weightSystem.GetType().ToString()}' to 'Imperial'");
            }
        }

        /// <summary>
        /// New Imperial Weight measurement from <paramref name="value"/> in <paramref name="unit"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        public ImperialWeight(double value, ImperialUnit unit)
        {
            _pounds = _ounces = _grains = _stones = _tons = 0;

            switch (unit)
            {
                case ImperialUnit.Pounds:
                    {
                        BaseValue = value;
                        break;
                    }
                case ImperialUnit.Grains:
                    {
                        Grains = value;
                        break;
                    }
                case ImperialUnit.Ounces:
                    {
                        Ounces = value;
                        break;
                    }
                case ImperialUnit.Stones:
                    {
                        Stones = value;
                        break;
                    }
                case ImperialUnit.Tons:
                    {
                        Tons = value;
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
