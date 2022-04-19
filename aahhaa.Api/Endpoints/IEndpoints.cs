namespace aahhaa.Api.Endpoints;

public interface IEndpoints
{
    /// <summary>
    /// This is where routes and handlers are defined
    /// </summary>
    /// <param name="routeBuilder">A route builder for the application</param>
    public void Register(IEndpointRouteBuilder routeBuilder);
}