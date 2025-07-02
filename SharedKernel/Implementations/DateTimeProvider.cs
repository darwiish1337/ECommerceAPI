using SharedKernel.Abstractions.DateTime;

namespace SharedKernel.Implementations;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}