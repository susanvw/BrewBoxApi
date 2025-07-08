# BrewBoxApi

Drink order application

dotnet ef migrations add InitialSet --startup-project ../BrewBoxApi.Presentation
dotnet ef database update --startup-project ../BrewBoxApi.Presentation

dotnet publish -c Release -o ./publish

cd publish
zip -r ../brewboxapi.zip .

az webapp deployment source config-zip --resource-group SvwDesignRG --name brewbox.azurewebsites.net --src ../brewboxapi.zip
