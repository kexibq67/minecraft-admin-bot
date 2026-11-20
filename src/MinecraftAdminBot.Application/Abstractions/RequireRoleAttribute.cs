using MinecraftAdminBot.Domain.Authorization;

namespace MinecraftAdminBot.Application.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequireRoleAttribute : Attribute
    {
        public UserRole Role { get; }
        public RequireRoleAttribute(UserRole role) => Role = role;
    }
}
