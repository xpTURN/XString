// Run performance tests: Window > General > Test Runner (PlayMode).

using System;
using System.Globalization;
using System.Text;
using Cysharp.Text;
using NUnit.Framework;
using Unity.PerformanceTesting;
using xpTURN.Text;

namespace xpTURN.Text.Tests
{
    [TestFixture]
    public static class XStringBenchmarks
    {
        const int WarmupCount = 5;
        const int MeasurementCount = 20;
        const int IterationsPerMeasurement = 100000;

        // Single test pattern: string + number (format) + date (format) + number (percent)
        const string Label = "Player_Alpha";
        const double Score = 12345.6789;
        static readonly DateTime At = new DateTime(2026, 2, 24, 14, 30, 0);
        const double Rate = 0.4567;

        const string FormatTemplate = "User: {0} | Score: {1:N2} | At: {2:yyyy-MM-dd HH:mm} | Rate: {3:P1}";

        [Test, Performance]
        public static void XStringIFormat_FourArgs()
        {
            Measure.Method(() =>
            {
                // One heap allocation (returned string)
                _ = XString.Format($"User: {Label} | Score: {Score:N2} | At: {At:yyyy-MM-dd HH:mm} | Rate: {Rate:P1}");
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void ZStringFormat_FourArgs()
        {
            Measure.Method(() =>
            {
                // One heap allocation (returned string)
                _ = ZString.Format(FormatTemplate, Label, Score, At, Rate);
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringFormat_FourArgs()
        {
            Measure.Method(() =>
            {
                // Multiple temporary allocations inside
                _ = string.Format(FormatTemplate, Label, Score, At, Rate);
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringIFormat_FourArgs()
        {
            Measure.Method(() =>
            {
                // Multiple temporary allocations inside
                _ = $"User: {Label} | Score: {Score:N2} | At: {At:yyyy-MM-dd HH:mm} | Rate: {Rate:P1}";
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringBuilder_FourArgs()
        {
            // Reuse the same StringBuilder instance across iterations (created once)
            var sb = new StringBuilder();
            Measure.Method(() =>
            {
                sb.Clear();
                sb.Append("User: ");
                sb.Append(Label);
                sb.Append(" | Score: ");
                sb.AppendFormat("{0:N2}", Score);
                sb.Append(" | At: ");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm}", At);
                sb.Append(" | Rate: ");
                sb.AppendFormat("{0:P1}", Rate);
                _ = sb.ToString();
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringBuilder128_FourArgs()
        {
            // Reuse the same StringBuilder instance across iterations (created once)
            var sb = new StringBuilder(128);
            Measure.Method(() =>
            {
                sb.Clear();
                sb.Append("User: ");
                sb.Append(Label);
                sb.Append(" | Score: ");
                sb.AppendFormat("{0:N2}", Score);
                sb.Append(" | At: ");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm}", At);
                sb.Append(" | Rate: ");
                sb.AppendFormat("{0:P1}", Rate);
                _ = sb.ToString();
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void Utf16ValueStringBuilder_FourArgs()
        {
            Measure.Method(() =>
            {
                // Utf16ValueStringBuilder (buffer reused); final ToString (returned string) => one heap allocation
                using var sb = ZString.CreateStringBuilder();
                sb.Append("User: ");
                sb.Append(Label);
                sb.Append(" | Score: ");
                sb.AppendFormat("{0:N2}", Score);
                sb.Append(" | At: ");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm}", At);
                sb.Append(" | Rate: ");
                sb.AppendFormat("{0:P1}", Rate);
                _ = sb.ToString();
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }
    }
}
