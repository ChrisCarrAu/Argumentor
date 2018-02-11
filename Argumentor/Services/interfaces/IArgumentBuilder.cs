using ArgumentRes.Models;
using ArgumentRes.Services.implementations;

namespace ArgumentRes.Services.interfaces
{
    interface IArgumentBuilder
    {
        Argumentor Build(string argumentString);
    }
}
