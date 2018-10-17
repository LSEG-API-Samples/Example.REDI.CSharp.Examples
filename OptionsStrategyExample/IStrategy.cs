using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REDI.Csharp.Examples.ComplexOptionsTrade
{
    interface IStrategy
    {
        bool VerifyArguments();
        void SetOptions(Options options);
        void SendOrder();
    }
}
