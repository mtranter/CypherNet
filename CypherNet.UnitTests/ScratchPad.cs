using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.UnitTests
{
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ScratchPad
    {
        [TestMethod]
        public void TestExtensionMethodInQueries()
        {
            Expression<Action<Car>> carFunc = c => c.Drive(5);
            var body = carFunc.Body;
            Console.WriteLine(body.NodeType);
            var method = body as MethodCallExpression;
            foreach (var arg in method.Arguments)
            {
                Console.WriteLine(arg.NodeType);
            }
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
