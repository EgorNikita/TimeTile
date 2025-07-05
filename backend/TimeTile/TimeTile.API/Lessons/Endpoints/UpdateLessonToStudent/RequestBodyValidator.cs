using FluentValidation;
using System.Text.Json;

namespace TimeTile.API.Lessons.Endpoints.UpdateLessonToStudent
{
    public class RequestBodyValidator : AbstractValidator<UpdateLessonToStudentEndpoint.RequestBody>
    {
        public RequestBodyValidator()
        {
            RuleFor(x => x)
                .Must(request =>
                    !request.LeftAt.HasValue ||
                    !request.CameAt.HasValue ||
                    request.CameAt.Value < request.LeftAt.Value)
                .WithMessage("CameAt should be less than LeftAt");

            When(x => x.ClassworkGrade.HasValue, () =>
            {
                RuleFor(x => x.ClassworkGrade.Value!.Weight)
                    .GreaterThan(0)
                    .WithMessage("Weight should be greater than zero");

                RuleFor(x => (int)x.ClassworkGrade.Value!.Value)
                    .GreaterThan(0)
                    .WithMessage("Value of grade should be greater than zero");
            });

            When(x => x.HomeworkGrade.HasValue, () =>
            {
                RuleFor(x => x.HomeworkGrade.Value!.Weight)
                    .GreaterThan(0)
                    .WithMessage("Weight should be greater than zero");

                RuleFor(x => (int)x.HomeworkGrade.Value!.Value)
                    .GreaterThan(0)
                    .WithMessage("Value of grade should be greater than zero");
            });
        }
    }
}
