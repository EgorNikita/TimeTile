using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Groups.Endpoints.Create;
using TimeTile.API.Subjects.Endpoints.GetById;
using TimeTile.API.Users.Requests;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateCourseEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Title)
                .MustBeValidTitle();

            RuleFor(x => x.SubjectId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.SubjectId)
                        .MustBeValidInstitutionEntityId<CreateCourseEndpoint.Request, Subject>(db, institutionId);
                });

            RuleFor(x => x.TeacherId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.TeacherId)
                        .MustBeValidOptionalInstitutionEntityId<CreateCourseEndpoint.Request, InstitutionMember>(db, institutionId);
                });

            RuleFor(x => x.TermId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.TermId)
                        .MustBeValidInstitutionEntityId<CreateCourseEndpoint.Request, Term>(db, institutionId);
                });

            // Optional
            RuleFor(x => x.StudentIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.StudentIds != null, () =>
                    {
                        RuleFor(x => x.StudentIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<CreateCourseEndpoint.Request, Student>(db, institutionId);
                    });
                });

            // Unique constraint
            RuleFor(c => c)
               .MustAsync(async (request, cancellationToken) =>
               {
                   var title = request.Title.Trim();

                   return !await db.Courses
                       .AsNoTracking()
                       .Where(c => c.InstitutionId == institutionId)
                       .AnyAsync(c =>
                           c.Title == title &&
                           c.SubjectId == request.SubjectId &&
                           c.TeacherId == request.TeacherId &&
                           c.TermId == request.TermId,
                           cancellationToken
                       );
               })
               .WithMessage("Course with such data already exists")
               // Call to the database only in case of successfull validation before
               .When(request =>
               {
                   var validator = new InlineValidator<CreateCourseEndpoint.Request>();

                   validator.RuleFor(x => x.Title).MustBeValidTitle();
                   validator.RuleFor(x => x.SubjectId).MustBeValidId();
                   validator.RuleFor(x => x.TeacherId).MustBeValidId();
                   validator.RuleFor(x => x.TermId).MustBeValidId();

                   var result = validator.Validate(request);
                   return result.IsValid;
               });
        }
    }
}
