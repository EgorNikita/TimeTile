using FluentValidation;

namespace TimeTile.API.Courses.Endpoints.UpdateCourseToStudent
{
    public class RequestBodyValidator : AbstractValidator<UpdateCourseToStudentEndpoint.RequestBody>
    {
        public RequestBodyValidator()
        {
            RuleFor(x => x)
                .Must(request => request.HasExam != false || request.Grade == null)
                .WithMessage("ExamGradeId must be null or omitted if HasExam is false");

            RuleFor(x => x.PositionX)
                .Must(posX => posX == null || posX >= 0)
                .WithMessage("PositionX cannot be negative");

            RuleFor(x => x.PositionY)
                .Must(posY => posY == null || posY >= 0)
                .WithMessage("PositionY cannot be negative");

            When(x => x.Grade != null, () =>
            {
                RuleFor(x => x.Grade!.Weight)
                    .GreaterThan(0)
                    .WithMessage("Weight should be greater than zero");

                RuleFor(x => (int) x.Grade!.Value)
                    .GreaterThan(0)
                    .WithMessage("Value of grade should be greater than zero");
            });

        }
    }
}
