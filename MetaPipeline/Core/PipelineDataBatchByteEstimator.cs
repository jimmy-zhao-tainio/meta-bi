using System.Globalization;
using System.Text;

namespace MetaPipeline;

public static class PipelineDataBatchByteEstimator
{
    public static long EstimatePayloadBytes(PipelineDataBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);

        long total = 0;
        foreach (var row in batch.Rows)
        {
            foreach (var value in row)
            {
                total += EstimatePayloadBytes(value);
            }
        }

        return total;
    }

    private static long EstimatePayloadBytes(object? value)
    {
        return value switch
        {
            null => 0,
            DBNull => 0,
            bool => 1,
            byte => 1,
            sbyte => 1,
            short => 2,
            ushort => 2,
            char character => Encoding.UTF8.GetByteCount([character]),
            int => 4,
            uint => 4,
            float => 4,
            long => 8,
            ulong => 8,
            double => 8,
            DateTime => 8,
            TimeSpan => 8,
            decimal => 16,
            Guid => 16,
            DateTimeOffset => 16,
            string text => Encoding.UTF8.GetByteCount(text),
            byte[] bytes => bytes.LongLength,
            char[] characters => Encoding.UTF8.GetByteCount(characters),
            IFormattable formattable => Encoding.UTF8.GetByteCount(formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty),
            _ => Encoding.UTF8.GetByteCount(value.ToString() ?? string.Empty),
        };
    }
}
