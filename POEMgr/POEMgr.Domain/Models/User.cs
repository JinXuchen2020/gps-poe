using POEMgr.Domain.Cores;

namespace POEMgr.Domain.Models
{
    public class User : AggregateRoot
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public bool IsDisabled { get; set; }

        private readonly List<Role> _roles;
        public IReadOnlyList<Role> Roles => _roles;

        public User()
        {
            _roles = new List<Role>();
        }

        public User(string name, string email, bool isDisabled = false)
        {
            Name = name;
            Email = email;
            IsDisabled = isDisabled;
        }

        public void AddRole(Role role)
        {
            _roles.Add(role);
            role.SetUser(this);
        }

        internal void SetRole(Role role)
        {
            _roles.Add(role);
        }
    }
}
