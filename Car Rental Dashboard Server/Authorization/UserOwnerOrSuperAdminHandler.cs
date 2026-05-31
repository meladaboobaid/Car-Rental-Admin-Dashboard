using Car_Rental_Dashboard_Server.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Car_Rental_Dashboard_Server.Authorization
{
    public class UserOwnerOrSuperAdminHandler : AuthorizationHandler<UserOwnerOrSuperAdminRequirement, int>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnerOrSuperAdminRequirement requirement, int adminID)
        {
            if (context.User.IsInRole("Super_admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out int AuthenticatedId) && AuthenticatedId == adminID)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
