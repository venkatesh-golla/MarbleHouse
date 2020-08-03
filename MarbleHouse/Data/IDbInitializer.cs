using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarbleHouse.Data
{
    public interface IDbInitializer
    {
       public void Initialize();
    }
}
