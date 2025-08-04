using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Requests;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.Constants;

namespace TimeTile.API.Auth.Authorization
{
    public class PermissionFallbackHandler
    {
        public static void Handle(
            int userId,
            AuthorizationHandlerContext context, 
            PermissionRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;

            if (httpContext is null)
                return;

            var endpoint = httpContext.GetEndpoint();
            var methodInfo = endpoint?.Metadata.GetMetadata<MethodInfo>();

            if (methodInfo != null)
            {
                var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole == GeneralRoles.Student)
                {
                    if (TryToUpdateQueryForStudent(userId, httpContext, methodInfo.GetParameters()))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
                else if (userRole == GeneralRoles.InstitutionMember)
                {
                    if (TryToUpdateQueryForInstitutionMember(userId, httpContext, methodInfo.GetParameters()))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }

                if (TryToUpdateQueryForUser(userId, httpContext, methodInfo.GetParameters()))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            Log.Information($"Permission {requirement.Permission} denied for user: {userId}");
        }

        private static bool TryToUpdateQueryForStudent(
            int userId,
            HttpContext httpContext,
            ParameterInfo[] parameters)
        {
            // Find parameter that implements IFilterByStudentsRequest
            var isRequestFilteredByStudents = parameters.Any(p =>
                typeof(IFilterByStudentsRequest).IsAssignableFrom(p.ParameterType));

            if (isRequestFilteredByStudents)
            {
                httpContext.Request.QueryString = UpdateQueryParameters(userId, QueryParameters.StudentIds, httpContext.Request.Query);
            }

            return isRequestFilteredByStudents;
        }

        private static bool TryToUpdateQueryForInstitutionMember(
            int userId,
            HttpContext httpContext,
            ParameterInfo[] parameters)
        {
            // Find parameter that implements IFilterByInstitutionMembersRequest
            var isRequestFilteredByInstitutionMembers = parameters.Any(p =>
                typeof(IFilterByInstitutionMembersRequest).IsAssignableFrom(p.ParameterType));

            if (isRequestFilteredByInstitutionMembers)
            {
                httpContext.Request.QueryString = UpdateQueryParameters(userId, QueryParameters.InstitutionMemberIds, httpContext.Request.Query);

                return true;
            }

            // Find parameter that implements IFilterByTeachersRequest
            var isRequestFilteredByTeachers = parameters.Any(p =>
                typeof(IFilterByTeachersRequest).IsAssignableFrom(p.ParameterType));

            if (isRequestFilteredByTeachers)
            {
                httpContext.Request.QueryString = UpdateQueryParameters(userId, QueryParameters.TeacherIds, httpContext.Request.Query);
            }

            return isRequestFilteredByTeachers;
        }

        private static bool TryToUpdateQueryForUser(
            int userId,
            HttpContext httpContext,
            ParameterInfo[] parameters)
        {
            // Find parameter that implements IFilterByUsersRequest
            var isRequestFilteredByUsers = parameters.Any(p =>
                typeof(IFilterByUsersRequest).IsAssignableFrom(p.ParameterType));

            if (isRequestFilteredByUsers)
            {
                httpContext.Request.QueryString = UpdateQueryParameters(userId, QueryParameters.UserIds, httpContext.Request.Query);
            }

            return isRequestFilteredByUsers;
        }

        private static QueryString UpdateQueryParameters(int userId, string ignoredParameter, IQueryCollection queryPairs)
        {
            var queryBuilder = new QueryBuilder();

            foreach (var pair in queryPairs)
            {
                // Leave all parameters except an ignored one
                if (!string.Equals(pair.Key, ignoredParameter, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var value in pair.Value)
                    {
                        queryBuilder.Add(pair.Key, value);
                    }
                }
            }

            // Instead of values that passed user we inject the only valid value
            queryBuilder.Add(ignoredParameter, userId.ToString());

            return queryBuilder.ToQueryString();
        }
    }
}
