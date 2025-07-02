namespace SharedKernel.Helpers;

public static class VerificationCodeGenerator
{
    public static string Generate(int length = 6)
    {
        var random = new Random();
        return string.Join("", Enumerable.Range(0, length)
            .Select(_ => random.Next(0, 10)));
    }
}