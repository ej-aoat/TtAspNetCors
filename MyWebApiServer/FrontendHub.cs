using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MyWebApiServer;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FrontendHub : Hub<IClientHubApi>
{
    public async Task CompleteReadyClient()
    {
        Console.WriteLine("CompleteReadyClient が呼び出されました。");
        await Task.Delay(1);
    }
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("SignalRにクライアント接続が行われました。");
        await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
        await base.OnConnectedAsync();
    }
}

