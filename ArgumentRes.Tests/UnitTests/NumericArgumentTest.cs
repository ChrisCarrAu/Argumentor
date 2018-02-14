using ArgumentRes.Attributes;
using ArgumentRes.Services.implementations;
using Xunit;

namespace ArgumentRes.Tests.UnitTests
{
    public class NumericArgumentTest
    {
        internal class Arguments_IntegerValue
        {
            [Switch(Key = "i", Description = "Integer value")]
            public int IntegerValue { get; set; }

            [Switch(Key = "l", Description = "Long value")]
            public long LongValue { get; set; }

            [Switch(Key = "dec", Description = "Decimal value")]
            public decimal DecimalValue { get; set; }

            [Switch(Key = "d", Description = "Double value")]
            public double DoubleValue { get; set; }

            [Switch(Key = "f", Description = "Float value")]
            public float FloatValue { get; set; }
        }

        [Fact]
        public void OnParse_IntegerValue_ParsesCorrectly()
        {
            var argumentor = new Argumentor<Arguments_IntegerValue>();
            var arguments = argumentor.Parse(new string[] { "-i", "123" });

            Assert.Equal(123, arguments.IntegerValue);

        }

        [Fact]
        public void OnParse_LongValue_ParsesCorrectly()
        {
            var longValue = ((long) int.MaxValue) + 1;
            var argumentor = new Argumentor<Arguments_IntegerValue>();
            var arguments = argumentor.Parse(new string[] { "-l", longValue.ToString() });

            Assert.Equal(longValue, arguments.LongValue);

        }

        [Fact]
        public void OnParse_DecimalValue_ParsesCorrectly()
        {
            var argumentor = new Argumentor<Arguments_IntegerValue>();
            var arguments = argumentor.Parse(new string[] { "-dec", "123.45" });

            Assert.Equal(123.45m, arguments.DecimalValue);

        }

        [Fact]
        public void OnParse_DoubleValue_ParsesCorrectly()
        {
            var doubleValue = 1234567890.123;
            var argumentor = new Argumentor<Arguments_IntegerValue>();
            var arguments = argumentor.Parse(new string[] { "-d", doubleValue.ToString() });

            Assert.Equal(doubleValue, arguments.DoubleValue);

        }

        [Fact]
        public void OnParse_FloatValue_ParsesCorrectly()
        {
            var floatValue = 1234.567;
            var argumentor = new Argumentor<Arguments_IntegerValue>();
            var arguments = argumentor.Parse(new string[] { "-f", floatValue.ToString() });

            // Check float using range
            Assert.InRange(arguments.FloatValue, 1234.567f, 1235.568f);

        }
    }
}
