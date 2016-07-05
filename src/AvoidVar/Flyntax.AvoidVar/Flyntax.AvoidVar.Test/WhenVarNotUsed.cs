using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyntax.AvoidVar.Test
{
    [TestClass]
    public class WhenVarNotUsed : TestBase
    {
        [TestMethod]
        public void NoDeclaration()
        {
            VerifyCSharpDiagnostic(WrapStatement(""));

        }

        [TestMethod]
        public void TypedDeclaration()
        {
            VerifyCSharpDiagnostic(WrapStatement("int x = Environment.TickCount;"));
        }
    }
}
