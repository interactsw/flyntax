using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelper;

namespace Flyntax.AvoidVar.Test.WhenVarAvoidable
{
    [TestClass]
    public class WhenTypeHasNativeAlias : TestBase
    {
        // int, string, etc.

        [TestMethod]
        public void Int32()
        {
            ShouldWarn("var x = Environment.TickCount;", "int");
        }

        [TestMethod]
        public void String()
        {
            ShouldWarn("var x = Environment.CommandLine;", "string");
        }
    }
}
