using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using Microsoft.Extensions.DependencyInjection;

namespace RegisterPage.model
{
    public class verify
    {
        public static bool Check(IServiceProvider serviceProvider, string username, string password)
        {
            using (var context = new RegisterPageContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<RegisterPageContext>>()))
            {
                var user = context.register
                    .SingleOrDefault(u => u.username == username && u.password == password);

                return user != null;
            }
        }

    }
}
