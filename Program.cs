using System.Net;
using Microsoft.AspNetCore.Mvc;
using ReqDumper;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSystemd();

builder.Services.AddSingleton<FileLogger>();

var app = builder.Build();

// app.Map("/{**path}", async (string path, HttpContext context, [FromServices] FileLogger logger) =>
// {
//     await logger.WriteRequestBodyToFileAsync(context.Request, WebUtility.UrlEncode(path));
// });


app.Map("/{**path}", async (string path, HttpContext context, [FromServices] FileLogger logger) =>
{
    await logger.WriteRequestToFileAsync(context.Request, WebUtility.UrlEncode(path));
});

app.Run();

