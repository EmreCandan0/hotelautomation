using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelAutomationdotnet
{
    public class LuxOda : Oda
    {
        // Constructor that calls the base constructor
        public LuxOda(int odano, bool h1, bool h2, bool h3, bool h4)
            : base(odano, h1, h2, h3, h4)
        {
        }

        // Optionally, you can add additional functionality specific to LuxOda here
    }
}
