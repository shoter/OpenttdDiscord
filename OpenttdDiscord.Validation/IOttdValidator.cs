namespace OpenttdDiscord.Validation
{
    public interface IOttdValidator<T>
    {
        EitherUnit Validate(T t);
    }
}
