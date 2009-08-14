using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareNinjas.Core.TestDoubles.Stubs.Test
{
    public class SOneMethodNoArgs : IOneMethodNoArgs
    {
        public Action Run;
        void IOneMethodNoArgs.Run()
        {
            if (Run != null)
            {
                Run();
            }
        }
    }
}
