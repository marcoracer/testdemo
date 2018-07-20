using System;
using System.Runtime.Remoting.Messaging;
using Castle.Core.Internal;
using Moq;
using NUnit.Framework;

namespace TestDemo.UnitTests
{
    public interface IStore
    {
        void Save(int result);
    }

    public class StringCalculator
    {
        private readonly IStore _store;


        public StringCalculator(IStore store)
        {
            _store = store;
        }

        public int Add(string input)
        {
            if (input.IsNullOrEmpty()) return 0;

            var numbers = input.Split(',');
            int total = 0;
            foreach (var number in numbers)
            {
                total += TryParseToInteger(number);
            }

            if (_store != null)
            {
                if (IsPrime(total))
                {
                    _store.Save(total);
                }
            }

            return total;
        }

        private bool IsPrime(int number)
        {
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            for (int i = 3; i <= (int) (Math.Sqrt(number)); i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private int TryParseToInteger(string input)
        {
            int dest;
            if (!int.TryParse(input, out dest))
            {
                throw new ArgumentException("Input format was incorrect");
            }

            return dest;
        }
    }

    public class StringCalculator_UnitTests
    {
        private Mock<IStore> _mockStore;

        private StringCalculator GetCalculator()
        {
            _mockStore = new Mock<IStore>();
            return new StringCalculator(_mockStore.Object);
        }

        [Test]
        public void Add_EmptyString_Returns_0()
        {
            StringCalculator calc = GetCalculator();
            int expectedResult = 0;
            int result = calc.Add("");
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("1", 1)]
        [TestCase("2", 2)]
        [TestCase("3", 3)]
        [TestCase("6", 6)]
        [TestCase("9", 9)]
        [TestCase("100", 100)]
        public void Add_SingleNumers_ReturnsResultNumber(string input, int expectedResult)
        {
            StringCalculator calc = GetCalculator();
            int result = calc.Add(input);
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("2,3", 5)]
        [TestCase("1,2,5", 8)]
        [TestCase("100,1", 101)]
        [TestCase("3,8, 10", 21)]
        public void Add_MultipleNumbers_SumOfAllNumbers(string input, int expectedResult)
        {
            StringCalculator calc = GetCalculator();
            int result = calc.Add(input);
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("a,1")]
        [TestCase("abc, ''")]
        [TestCase("qwery")]
        [TestCase("-,/")]
        // [ExpectedException(typeof(ArgumentException))]
        public void Add_InvalidString_ThrowsExeption(string input)
        {
            StringCalculator calc = GetCalculator();
            Assert.Throws<ArgumentException>(() => calc.Add(input));
            //calc.Add(input);

        }

        [TestCase("-1, 5", 4)]
        [TestCase("-1", -1)]
        [TestCase("-1, -5", -6)]
        [TestCase("-1, -5, -20", -26)]
        public void Add_MinusNumbers_AreSummedCorrectly(string input, int expectedResult)
        {
            StringCalculator calc = GetCalculator();
            var result = calc.Add(input);
            Assert.AreEqual(expectedResult, result);

        }

        [TestCase(("2"))]                   // 2
        [TestCase(("5,6"))]                 // 11
        [TestCase(("3,4"))]                 // 7
        [TestCase(("10, 10, 3"))]           // 23
        [TestCase(("40, 5, 5, 3"))]         // 53
        public void Add_ResultIsAPrimeNumber_ResultAreSaved(string input)
        {
            // Mock<IStore> mockstore = new Mock<IStore>();
            // StringCalculator calc = new StringCalculator(mockstore.Object);
            StringCalculator calc = GetCalculator();
            var result = calc.Add(input);
            _mockStore.Verify(m => m.Save(It.IsAny<int>()), Times.Once);
        }

        [TestCase(("4"))]                   // 4
        [TestCase(("5,5"))]                 // 10
        [TestCase(("5,4"))]                 // 9
        [TestCase(("10, 10, 5"))]           // 25
        [TestCase(("40, 5, 5, 3,2"))]       // 54
        public void Add_ResultNOTAPrimeNumber_ResultNOTSaved(string input)
        {
            // Mock<IStore> mockstore = new Mock<IStore>();
            // StringCalculator calc = new StringCalculator(mockstore.Object);
            StringCalculator calc = GetCalculator();
            var result = calc.Add(input);
            _mockStore.Verify(m => m.Save(It.IsAny<int>()), Times.Never);
        }

        // UnitunderTest_Scenario_ExpectedOutcome
    }
}