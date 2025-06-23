using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Users.Requests;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.Create
{
    public class RequestValidator : BaseCreateUserValidator<CreateInstitutionMemberEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider) 
            : base(db, institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(u => u.RoleId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(u => u.RoleId)
                        .MustBeValidOptionalInstitutionEntityId<CreateInstitutionMemberEndpoint.Request, Role>(db, institutionId);
                });

            RuleFor(u => u.WeekWorkHours)
                .GreaterThan(0)
                .WithMessage("Week work hours should be greater than 0.");

            RuleFor(u => u.PreferredClassroomId)
                .Must(id => id == null || id >= 1)
                .WithMessage("Id must be greater or equal to 1.")
                .DependentRules(() =>
                {
                    RuleFor(u => u.PreferredClassroomId)
                        .MustAsync(async (classroomId, cancellationToken) =>
                        {
                            if (classroomId is null)
                                return true;

                            return await db.Classrooms
                                .Where(g => g.InstitutionId == institutionId)
                                .AnyAsync(g => g.Id == classroomId, cancellationToken);
                        })
                        .WithMessage("Classroom's ID is invalid.");
                });

            RuleFor(u => u.SubjectsIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.SubjectsIds != null, () => {
                        RuleFor(x => x.SubjectsIds!)
                            .MustBeValidInstitutionEntityIdsList<CreateInstitutionMemberEndpoint.Request, Subject>(db, institutionId);
                    });
                });

            RuleFor(x => x.GroupsIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.GroupsIds != null, () => {
                        RuleFor(x => x.GroupsIds!)
                            .MustBeValidInstitutionEntityIdsList<CreateInstitutionMemberEndpoint.Request, Group>(db, institutionId);
                    });
                });
        }
    }
}
