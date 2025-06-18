using FluentValidation;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.Classrooms.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateClassroomEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Title)
                .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
                .WithMessage("Title contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.Title);

            RuleFor(x => x.Capacity)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Capacity should be greater or equal to 1");

            RuleFor(x => x.ClassroomTypeId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("ClassroomTypeId should be greater or equal to 1");
        }
    }
}
