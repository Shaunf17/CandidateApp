
using CandidateApp.Services;
using CandidateApp.Services.Interfaces;

namespace CandidateApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Register the repository with the connection string
            builder.Services.AddScoped<ICandidateRepository>(sp => new CandidateRepository(connectionString));
            builder.Services.AddScoped<ISkillRepository>(sp => new SkillRepository(connectionString));

            builder.Services.AddControllers();

            // Configure CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins("http://localhost:60881") // Frontend client url & port
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            // Generative documentation
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Candidate API v1");
                });
            }

            // Enable CORS
            app.UseCors("AllowSpecificOrigins");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
