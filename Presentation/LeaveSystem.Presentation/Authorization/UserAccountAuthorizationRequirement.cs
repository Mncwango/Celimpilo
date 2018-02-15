using Microsoft.AspNetCore.Authorization;

namespace LeaveSystem.Presentation.Authorization
{
    public class UserAccountAuthorizationRequirement: IAuthorizationRequirement
    {
        public UserAccountAuthorizationRequirement(string operationName)
        {
            this.OperationName = operationName;
        }
        public string OperationName { get; private set; }
    }
}