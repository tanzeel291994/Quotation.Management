using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using Quotation.Management.Repositories;
using Quotation.Management.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

/*
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "cors",
                      policy =>
                      {
                          policy.WithOrigins("*", "http://localhost:3000");
                      });
});
*/

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Services
builder.Services.AddScoped<IProductMasterService, ProductMasterService>();
builder.Services.AddScoped<IItemGroupService, ItemGroupService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IOptCodeService, OptCodeService>();
builder.Services.AddScoped<ISeriesService, SeriesService>();
builder.Services.AddScoped<IItemCodeService, ItemCodeService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IMastersService, MastersService>();
builder.Services.AddScoped<IQuotationService, QuotationService>();
#endregion

#region Repositories
builder.Services.AddScoped<IProductMasterRepository<ProductMaster>, ProductMasterRepository>();
builder.Services.AddScoped<IItemGroupRepository<ItemGroupMaster>, ItemGroupRepository>();
builder.Services.AddScoped<IBrandRepository<BrandMaster>, BrandRepository>();
builder.Services.AddScoped<IOptCodeRepository<OptionMaster>, OptCodeRepository>();
builder.Services.AddScoped<ISeriesRepository<SeriesMaster>, SeriesRepository>();
builder.Services.AddScoped<IItemCodeRepository<ItemMaster>, ItemCodeRepository>();
builder.Services.AddScoped<IPricingRepository<PricingMaster>, PricingRepository>();
builder.Services.AddScoped<IMastersRepository, MastersRepository>();
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();
#endregion
 
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
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
app.Run();
