using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class GradeFaker : BaseFaker<Grade>
    {
        // Value constraints
        private const int MIN_VALUE = 1;
        private const int MAX_VALUE = 15;

        // Weight constraints
        private const float MIN_WEIGHT = 0.5f;
        private const int MAX_WEIGHT = 4;
        private const float DEFAULT_WEIGHT_POSSIBILITY = 0.8f;
        private const int DEFAULT_WEIGHT = 1;


        public GradeFaker()
        {
            _faker
                .RuleFor(g => g.Value, f => f.Random.Int(MIN_VALUE, MAX_VALUE))
                .RuleFor(g => g.Weight, f =>
                {
                    if (f.Random.Bool(DEFAULT_WEIGHT_POSSIBILITY))
                    {
                        return DEFAULT_WEIGHT;
                    }

                    return f.Random.Float(MIN_WEIGHT, MAX_WEIGHT);
                });
        }
    }
}
