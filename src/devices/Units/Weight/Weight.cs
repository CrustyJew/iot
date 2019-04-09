using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Units.Weight
{
    public class Weight
    {
        private ImperialWeight _imperial;
        public ImperialWeight Imperial { get { return _imperial; } set
            {
                _imperial = value;
                _metric = new MetricWeight(value);
            }
        }

        private MetricWeight _metric;
        public MetricWeight Metric { get { return _metric; } set
            {
                _metric = value;
                _imperial = new ImperialWeight(value);
            }
        }

        public Weight(IWeightSystem weight)
        {
            if(weight is MetricWeight mw)
            {
                Metric = mw;
            }
            else if(weight is ImperialWeight iw)
            {
                Imperial = iw;
            }
            else
            {
                throw new UnknownConversionException($"Internal IoT Library Error! Unknown conversion to 'Weight' from '{weight.GetType()}'");
            }
        }
    }
}
