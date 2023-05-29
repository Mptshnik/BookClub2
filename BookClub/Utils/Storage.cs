using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookClub.Utils
{
    public class Storage
    {
        public Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
        private static Storage instance;

        public void Clear() { Data.Clear(); }

        public static Storage GetInstance()
        {
            if(instance == null)
            {
                instance = new Storage();
            }

            return instance;
        }
    }
}
