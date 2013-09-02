using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.UnitTests
{
    using System.IO;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using CypherNet.Transaction;

    [TestClass]
    public class ScratchPad
    {
        [TestMethod]
        public void TestExtensionMethodInQueries()
        {
            var val = UriHelper.Combine("Http://mysite", "something");
        }
    }

    class Car
    {
       
    }

    static class CarExtensions
    {
        public static void Drive(this Car c, int speed)
        {
        }
    }
}
