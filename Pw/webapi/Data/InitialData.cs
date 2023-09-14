using PwServer.Models;

namespace webapi.Data
{
    public static class InitialData
    {
        public static void Seed(this webapiContext dbContext)
        {
            if (!dbContext.UserInfoModel.Any())
            {
                dbContext.UserInfoModel.Add(new UserInfoModel
                {
                    Name = "Employee001",
                    Email = "Male",
                    Password = "01-01-1990",
                    Amount = 123
                });

                dbContext.SaveChanges();
            }
        }
    }
}
