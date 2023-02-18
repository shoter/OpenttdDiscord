using FluentValidation;
using FluentValidation.Results;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Validation
{
    public class OttdValidator<TValidator, T>
        where TValidator : AbstractValidator<T>
    {
        private readonly TValidator validator;
        public OttdValidator(TValidator validator)
        {
            this.validator = validator;
        }

        public EitherUnit Validate(T t)
        {
            try
            {
                ValidationResult validationResult = validator.Validate(t);
                if (validationResult.IsValid)
                {
                    return Unit.Default;
                }

                return new ValidationError(validationResult);
            }
            catch (Exception e)
            {
                return new ExceptionError(e);
            }
        }
    }
}
