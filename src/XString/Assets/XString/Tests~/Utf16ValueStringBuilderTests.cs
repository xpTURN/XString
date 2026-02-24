using NUnit.Framework;
using Cysharp.Text;

namespace xpTURN.Text.Tests
{
    [TestFixture]
    public static class Utf16ValueStringBuilderTests
    {
        /// <summary>When (string)null is passed, no exception and nothing appended (on some runtimes AsSpan() does not throw on null).</summary>
        [Test]
        public static void AppendString_Null_NoThrow_AppendsNothing()
        {
            using var sb = ZString.CreateStringBuilder(true);
            sb.Append("a");
            sb.Append((string)null);
            sb.Append("b");
            Assert.That(sb.ToString(), Is.EqualTo("ab"));
        }

        [Test]
        public static void AppendT_Null_AppendsNothing()
        {
            using var sb = ZString.CreateStringBuilder(true);
            sb.Append("a");
            sb.Append<string>(null);
            sb.Append<object>(null);
            sb.Append("b");
            Assert.That(sb.ToString(), Is.EqualTo("ab"));
        }

        [Test]
        public static void AppendFormat_NullArg_AppendsNothing()
        {
            using var sb = ZString.CreateStringBuilder(true);
            sb.AppendFormat<string, object, string>("{0}-{1}-{2}", "a", null, "c");
            Assert.That(sb.ToString(), Is.EqualTo("a--c"));
        }

        [Test]
        public static void AppendStringStartIndexCount_NullWithZeroLength_NoThrow()
        {
            using var sb = ZString.CreateStringBuilder(true);
            sb.Append("x");
            sb.Append((string)null, 0, 0);
            Assert.That(sb.ToString(), Is.EqualTo("x"));
        }
    }
}
