using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Cysharp.Text;
using xpTURN.Text;

namespace xpTURN.Text.Tests
{
    [TestFixture]
    public class XStringTests
    {
        /// <summary>Runs the action under the given culture and restores the previous culture when done.</summary>
        private static void RunUnderCulture(string cultureName, Action action)
        {
            var prev = Thread.CurrentThread.CurrentCulture;
            var prevUi = Thread.CurrentThread.CurrentUICulture;
            try
            {
                var ci = CultureInfo.GetCultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
                action();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = prev;
                Thread.CurrentThread.CurrentUICulture = prevUi;
            }
        }
        [Test]
        public void Format_Interpolated_LiteralOnly_ReturnsLiteral()
        {
            var result = XString.Format($"Hello");
            Assert.That(result, Is.EqualTo("Hello"));
        }

        [Test]
        public void Format_Interpolated_EmptyLiteral_ReturnsEmpty()
        {
            var result = XString.Format($"");
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Format_Interpolated_SingleInt_EmbedsValue()
        {
            var result = XString.Format($"value={42}");
            Assert.That(result, Is.EqualTo("value=42"));
        }

        [Test]
        public void Format_Interpolated_SingleString_EmbedsValue()
        {
            var result = XString.Format($"name={"Alice"}");
            Assert.That(result, Is.EqualTo("name=Alice"));
        }

        [Test]
        public void Format_Interpolated_NullReference_EmbedsEmpty()
        {
            string nil = null;
            var result = XString.Format($"x={nil}");
            Assert.That(result, Is.EqualTo("x="));
        }

        // --- Interpolated string: null handling by variable type ---

        [Test]
        public void Format_Interpolated_Null_String_EmbedsEmpty()
        {
            string a = null;
            var result = XString.Format($"prefix_{a}_suffix");
            Assert.That(result, Is.EqualTo("prefix__suffix"));
        }

        [Test]
        public void Format_Interpolated_Null_Object_EmbedsEmpty()
        {
            object a = null;
            var result = XString.Format($"[{a}]");
            Assert.That(result, Is.EqualTo("[]"));
        }

        [Test]
        public void Format_Interpolated_Null_NullableInt_EmbedsEmpty()
        {
            int? a = null;
            var result = XString.Format($"n={a}");
            Assert.That(result, Is.EqualTo("n="));
        }

        [Test]
        public void Format_Interpolated_Null_NullableDouble_EmbedsEmpty()
        {
            double? a = null;
            var result = XString.Format($"x={a}");
            Assert.That(result, Is.EqualTo("x="));
        }

        [Test]
        public void Format_Interpolated_Null_NullableWithFormat_EmbedsEmpty()
        {
            double? a = null;
            var result = XString.Format($"{a:N2}");
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Format_Interpolated_Null_ReferenceTypeWithFormat_EmbedsEmpty()
        {
            string a = null;
            var result = XString.Format($"{a:X}");
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Format_Interpolated_Null_MixedWithValues_EmbedsEmptyForNullsOnly()
        {
            string s = null;
            int? n = null;
            int ok = 1;
            var result = XString.Format($"s=[{s}] n=[{n}] ok=[{ok}]");
            Assert.That(result, Is.EqualTo("s=[] n=[] ok=[1]"));
        }

        [Test]
        public void Format_Interpolated_MultipleValues_Concatenates()
        {
            var result = XString.Format($"a={1} b={2} c={3}");
            Assert.That(result, Is.EqualTo("a=1 b=2 c=3"));
        }

        [Test]
        public void Format_Interpolated_Unicode_PreservesCharacters()
        {
            var result = XString.Format($"한글={"테스트"}");
            Assert.That(result, Is.EqualTo("한글=테스트"));
        }

        [Test]
        public void Format_Interpolated_WithFormat_IFormattable_UsesFormat()
        {
            var result = XString.Format($"{1234:N0}");
            Assert.That(result, Is.EqualTo(1234.ToString("N0")));
        }

        [Test]
        public void Format_Interpolated_NumberFormat_RespectsCurrentCulture_enUS()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"{1234.56:N2}");
                Assert.That(result, Is.EqualTo("1,234.56"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberFormat_RespectsCurrentCulture_deDE()
        {
            RunUnderCulture("de-DE", () =>
            {
                var result = XString.Format($"{1234.56:N2}");
                Assert.That(result, Is.EqualTo("1.234,56"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberFormat_RespectsCurrentCulture_koKR()
        {
            RunUnderCulture("ko-KR", () =>
            {
                var result = XString.Format($"{1234:N0}");
                Assert.That(result, Is.EqualTo("1,234"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberFormat_RespectsCurrentCulture_jaJP()
        {
            RunUnderCulture("ja-JP", () =>
            {
                var result = XString.Format($"{1234:N0}");
                Assert.That(result, Is.EqualTo("1,234"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberToString_RespectsCurrentCulture_DecimalSeparator()
        {
            RunUnderCulture("de-DE", () =>
            {
                // double.ToString() uses CurrentCulture (e.g. comma as decimal separator in de-DE)
                var result = XString.Format($"value={1.5}");
                Assert.That(result, Is.EqualTo("value=1,5"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberToString_RespectsCurrentCulture_Invariant()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"value={1.5}");
                Assert.That(result, Is.EqualTo("value=1.5"));
            });
        }

        [Test]
        public void Format_FormatString_OneArg_AppliesFormat()
        {
            var result = XString.Format("{0}", 42);
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Format_FormatString_TwoArgs_AppliesFormat()
        {
            var result = XString.Format("{0} + {1}", "a", "b");
            Assert.That(result, Is.EqualTo("a + b"));
        }

        [Test]
        public void Format_FormatString_ThreeArgs_AppliesFormat()
        {
            var result = XString.Format("{0},{1},{2}", 1, 2, 3);
            Assert.That(result, Is.EqualTo("1,2,3"));
        }

        [Test]
        public void Format_FormatString_FourArgs_AppliesFormat()
        {
            var result = XString.Format("{0}-{1}-{2}-{3}", "A", "B", "C", "D");
            Assert.That(result, Is.EqualTo("A-B-C-D"));
        }

        [Test]
        public void Join_CharSeparator_JoinsValues()
        {
            var result = XString.Join(',', "a", "b", "c");
            Assert.That(result, Is.EqualTo("a,b,c"));
        }

        [Test]
        public void Join_StringSeparator_JoinsValues()
        {
            var result = XString.Join(" | ", "x", "y", "z");
            Assert.That(result, Is.EqualTo("x | y | z"));
        }

        [Test]
        public void Join_IntArray_JoinsAsStrings()
        {
            var result = XString.Join(",", 1, 2, 3);
            Assert.That(result, Is.EqualTo("1,2,3"));
        }

        [Test]
        public void Concat_Values_Concatenates()
        {
            var result = XString.Concat("a", "b", "c");
            Assert.That(result, Is.EqualTo("abc"));
        }

        [Test]
        public void Concat_IntValues_ConcatenatesToStrings()
        {
            var result = XString.Concat(1, 2, 3);
            Assert.That(result, Is.EqualTo("123"));
        }

        [Test]
        public void CreateStringBuilder_AppendAndToString_ReturnsContent()
        {
            using var sb = XString.CreateStringBuilder();
            sb.Append("Hello");
            sb.Append("World");
            Assert.That(sb.ToString(), Is.EqualTo("HelloWorld"));
        }

        [Test]
        public void CreateUtf8StringBuilder_AppendAndToString_ReturnsContent()
        {
            using var sb = XString.CreateUtf8StringBuilder();
            sb.Append("Test");
            Assert.That(sb.ToString(), Is.EqualTo("Test"));
        }

        /// <summary>Helper that throws from ToString() to verify exception propagation in the interpolated handler.</summary>
        private class ThrowsInToString
        {
            public override string ToString() => throw new InvalidOperationException("ToString failed.");
        }

        [Test]
        public void Format_Interpolated_WhenAppendThrows_GetStringRethrows()
        {
            var bad = new ThrowsInToString();
            var ex = Assert.Throws<InvalidOperationException>(() => XString.Format($"x={bad}"));
            Assert.That(ex.Message, Is.EqualTo("ToString failed."));
        }
    }
}
