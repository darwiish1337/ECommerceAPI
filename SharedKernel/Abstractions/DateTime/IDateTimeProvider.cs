namespace SharedKernel.Abstractions.DateTime;

public interface IDateTimeProvider
{
    System.DateTime UtcNow { get; }
}