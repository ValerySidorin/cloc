using Cloc.Hosting;
using Sample.AspNetCore.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCloc(builder.Configuration, configure: options =>
{
    options.ExitOnJobFailed = true;
});
builder.Services.AddClocJob<HelloWorldJob>();
builder.Services.AddClocJob<SampleScopedJob>();
builder.Services.AddClocJob<SampleJob>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
