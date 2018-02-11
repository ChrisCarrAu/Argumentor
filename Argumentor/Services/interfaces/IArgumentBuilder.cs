using ArgumentRes.Models;

namespace ArgumentRes.Services.interfaces
{
    interface IArgumentBuilder
    {
        Argumentor Build(string argumentString);
    }
}
