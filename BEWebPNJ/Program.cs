using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Firestore;
using Grpc.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BEWebPNJ.Services;

var builder = WebApplication.CreateBuilder(args);

var credentialPath = Path.Combine(AppContext.BaseDirectory, "firebase_key.json");

if (!File.Exists(credentialPath))
{
    throw new FileNotFoundException("Firebase config file not found.", credentialPath);
}

// 🔥 Kiểm tra xem FirebaseApp đã khởi tạo chưa (tránh lỗi trùng lặp khi chạy lại app) 
if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(credentialPath)
    });
}

// ✅ Tạo FirestoreDb với credentials từ file JSON
var googleCredential = GoogleCredential.FromFile(credentialPath);
var firestoreDb = FirestoreDb.Create("pnjstore-66a4d", new FirestoreClientBuilder
{
    ChannelCredentials = googleCredential.ToChannelCredentials()
}.Build());

builder.Services.AddSingleton(firestoreDb);
builder.Services.AddScoped<AdminTitleService>();
builder.Services.AddScoped<AdvertisementService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<CollectionProductService>();
builder.Services.AddScoped<CouponService>();
builder.Services.AddScoped<EvaluationService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<RevenueService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<PaypalService>();

builder.Services.AddHttpClient<UserService>();
builder.Services.AddHttpClient<PurchasedService>();
builder.Services.AddHttpClient<ShoppingCartService>();
builder.Services.AddHttpClient<AddAddressService>();
builder.Services.AddHttpClient<FavouriteProductService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Cấu hình xác thực Firebase JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/pnjstore-66a4d";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/pnjstore-66a4d",
            ValidateAudience = true,
            ValidAudience = "pnjstore-66a4d",
            ValidateLifetime = true
        };
    });


// ✅ CORS - Cho phép Vue.js frontend kết nối
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueClient", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVueClient");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();