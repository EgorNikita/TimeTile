using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Users.Requests;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.Create
{
    public class CreateInstitutionMemberEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Creates a new InstitutionMember")
                .WithRequestValidation<Request>()
                .DisableAntiforgery();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromForm] Request request,
            TimetileDbContext db,
            IUserService userService,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            // Extract institutionId
            var institutionId = institutionProvider.GetInstitutionId();

            // Save institutionMember
            var institutionMember = await userService.CreateUser<InstitutionMember>(
                request.Avatar,
                request.Firstname,
                request.Lastname,
                request.HomeAddress,
                request.PhoneNumber,
                request.BirthDate,
                institutionId,
                request.RoleId,
                member =>
                {
                    member.PreferredClassroomId = request.PreferredClassroomId;
                    member.WeekWorkHours = request.WeekWorkHours;

                    // Adding relationships to subjects
                    if (request.SubjectsIds is not null)
                    {
                        member.TeacherToSubjects = request.SubjectsIds
                            .Select(subjectId => new TeacherToSubject { SubjectId = subjectId })
                            .ToList();
                    }

                    // Adding relationships to groups
                    if (request.GroupsIds is not null)
                    {
                        member.InstitutionMemberToGroups = request.GroupsIds
                            .Select(groupId => new InstitutionMemberToGroup { GroupId = groupId })
                            .ToList();
                    }
                },
                cancellationToken
            );

            // Return result
            var response = new Response(
                institutionMember.Id,
                institutionMember.Firstname,
                institutionMember.Lastname,
                institutionMember.HomeAddress,
                institutionMember.PhoneNumber,
                institutionMember.BirthDate,
                institutionMember.Login,
                institutionMember.RoleId,
                institutionMember.WeekWorkHours,
                institutionMember.PreferredClassroomId,
                institutionMember.Avatar.FileGuid.ToString()
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{API.Endpoints.Routes.InstitutionMembers}/{institutionMember.Id}", result);
        }

        public record Request : ICreateUserRequest
        {
            public IFormFile? Avatar { get; init; }
            public string Firstname { get; init; } = null!;
            public string Lastname { get; init; } = null!;
            public string HomeAddress { get; init; } = null!;
            public string PhoneNumber { get; init; } = null!;
            public DateOnly BirthDate { get; init; }
            public int RoleId { get; init; }
            public int WeekWorkHours { get; init; }
            public int? PreferredClassroomId { get; init; }
            public List<int>? SubjectsIds { get; init; }
            public List<int>? GroupsIds { get; init; }
        };

        private record Response(
            int Id,
            string Firstname,
            string Lastname,
            string HomeAddress,
            string PhoneNumber,
            DateOnly BirthDate,
            string Login,
            int RoleId,
            int WeekWorkHours,
            int? PreferredClassroomId,
            string AvatarUrl
        );
    }
}
