# BrewBoxApi

Drink order application

dotnet ef migrations add InitialSet --startup-project ../BrewBoxApi.Presentation
dotnet ef database update --startup-project ../BrewBoxApi.Presentation

dotnet publish -c Release -o ./publish

cd publish
zip -r ../brewboxapi.zip .

az webapp deployment source config-zip --resource-group SvwDesignRG --name brewbox.azurewebsites.net --src ../brewboxapi.zip

dotnet publish -c Release -o ../Publish -r linux-x64 --no-self-contained

# Generate migrations

dotnet tool install --global dotnet-ef
dotnet ef --version

cd BrewBoxApi.Presentation
dotnet ef migrations add InitialCreate --project BrewBoxApi.Infrastructure --startup-project BrewBoxApi.Presentation --context BrewBoxApi.Infrastructure.ApplicationDbContext
