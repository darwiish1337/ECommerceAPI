namespace SharedKernel.Exceptions;

public class ConflictException(string message) : DomainException(message);
