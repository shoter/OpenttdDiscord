using FluentValidation.Results;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Validation
{
    public class ValidationError : IError
    {
        public string Reason { get; set; }

        public List<ValidationFailure> Errors { get; }

        public ValidationError(ValidationResult validationResult)
        {
            Reason = validationResult.Errors.First().ErrorMessage;
            Errors = validationResult.Errors;
        }
    }
}
