using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessAPI.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}
