#!/bin/bash
rm -rf /src/cipa-api/
rm -rf /var/www/orion
cd /src
git clone https://github.com/igorgomes96/cipa-api.git
cd "/src/cipa-api/1 - WebApi/Cipa.WebApi"
export DOTNET_CLI_HOME=/tmp
/usr/bin/dotnet publish --configuration Release /p:CustomTransformFileName=aws.transform -o /var/www/cipa
chmod -R 777 /var/www/cipa/Assets
systemctl restart cipa
systemctl restart nginx