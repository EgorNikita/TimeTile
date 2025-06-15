using FluentValidation;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.ClassroomTypes.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateClassroomTypeEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Description)
                .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
                .WithMessage("Description contains invalid characters.")
                .ApplyRegexPattern(Core.Common.Regex.RegexPatterns.Pattern.Description);
        }
    }
}
