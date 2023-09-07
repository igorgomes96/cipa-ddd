sudo systemctl stop cipa
cd "/src/cipa-api"
sudo git pull
sudo /usr/bin/dotnet publish --configuration Release /p:CustomTransformFileName=aws.transform -o /var/www/cipa
sudo chmod -R 777 /var/www/cipa/Assets
sudo mkdir /var/www/cipa/StaticFiles
sudo chmod -R 777 /var/www/cipa/StaticFiles
sudo systemctl restart cipa