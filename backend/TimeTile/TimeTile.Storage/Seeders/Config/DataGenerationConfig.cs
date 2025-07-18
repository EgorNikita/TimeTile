using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTile.Storage.Seeders.Config
{
    public class DataGenerationConfig
    {
        public static DataGenerationMode GenerationMode { get; set; } = DataGenerationMode.Random;

        public static void SetMode(DataGenerationMode mode)
        {
            GenerationMode = mode;
        }
    }
}
