using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace HotelAutomationdotnet
{

    public abstract class Oda
    {
       public int odano;
        public bool h1, h2, h3, h4;

        public Oda(int odano, bool h1, bool h2, bool h3, bool h4)
        {
            this.odano = odano;
            this.h1 = h1;
            this.h2 = h2;
            this.h3 = h3;
            this.h4 = h4;
        }

        public Oda()
        {

        }



    }
}
