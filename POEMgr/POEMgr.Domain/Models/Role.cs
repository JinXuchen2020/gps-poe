using POEMgr.Domain.Cores;

namespace POEMgr.Domain.Models
{
    public class Role : AggregateRoot
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDisabled { get; set; }

        private readonly List<User> _user;
        public IReadOnlyList<User> Users => _user;

        public Role()
        {
            _user = new List<User>();
        }

        public Role(string name, string description, bool isDisabled = false)
        {
            Name = name;
            Description = description;
            IsDisabled = isDisabled;

            _user = new List<User>();
        }

        public void AddUser(User user)
        {
            _user.Add(user);
            user.SetRole(this);
        }

        internal void SetUser(User user)
        {
            _user.Add(user);
        }
    }
}
