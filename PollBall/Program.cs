using PollBall.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IPollResultsService, PollResultsService>();
builder.Services.AddMvc();
var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var pollResults = services.GetRequiredService<IPollResultsService>();
    app.Use(async (context, next) =>
    {
        if (context.Request.Query.ContainsKey("favourite"))
        {
            string selectedValue = context.Request.Query["favourite"];
            SelectedGame selectedGame = (SelectedGame)Enum.Parse(typeof(SelectedGame), selectedValue, true);
            pollResults.AddVote(selectedGame);

            /*  SortedDictionary<SelectedGame, int> gameVotes = pollResults.GetVoteResult();
              foreach (KeyValuePair<SelectedGame, int> currentVote in gameVotes)
              {
                  await context.Response.WriteAsync($"<div> Game name: {currentVote.Key}. Votes: {currentVote.Value} </div>");
              }*/

            context.Response.Headers.Add("content-type", "text/html");
            await context.Response.WriteAsync("Thank you for submitting the poll. You may look at the poll results <a href='/?submitted=true'>Here</a>");
        }
        else
        {
            await next.Invoke();
        }
    });
}

app.UseStaticFiles();

app.UseRouting();
app.MapDefaultControllerRoute();

app.Run();
