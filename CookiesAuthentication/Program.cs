namespace CookiesAuthentication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //Add authendtication handler
            builder.Services.AddAuthentication("MyCookie").AddCookie("MyCookie", options =>
            {
                options.Cookie.Name = "MyCookie";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                //options.ExpireTimeSpan = TimeSpan.FromSeconds(100); //set the cookie expiriation time

            });

            #region Add Authorization
            builder.Services.AddAuthorization(options =>
            {
                //Add a policy called HROnly, so the page 
                options.AddPolicy("HROnly", policy => policy.RequireClaim("Department", "HR"));
            });
            #endregion
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //Add Authentication to pipeline authen->authori
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}