using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace StatsBB.Model
{
    public class Player
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Name
        {
            get => $"{FirstName} {LastName}".Trim();
            set
            {
                var parts = value?.Split(' ') ?? Array.Empty<string>();
                if (parts.Length > 0) FirstName = parts[0];
                if (parts.Length > 1) LastName = string.Join(" ", parts.Skip(1));
            }
        }

        public int Number { get; set; }
        public bool IsActive { get; set; }
        public bool IsTeamA { get; set; }

        public string DisplayName => Number.ToString();
    }
}
*/