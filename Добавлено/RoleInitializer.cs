using Microsoft.AspNetCore.Identity;

public static class RoleInitializer
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roleNames = { "Candidate", "Administrator", "HrManager" };

        foreach (var roleName in roleNames)
        {
            bool roleExist = await roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                // create the roles and seed them to the database
                IdentityRole<Guid> role = new(roleName);
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        Console.WriteLine($"Error creating role {roleName}: {error.Description}");

                }
                else
                {
                    Console.WriteLine($"Role {roleName} created successfully.");
                }
            }
            else
                Console.WriteLine($"Role {roleName} already exists.");


        }
    }
}