#!/bin/bash
yum update -y
yum install nginx -y
systemctl enable nginx # habilita inicialização automática
rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm  # add the Microsoft package signing key to your list of trusted keys
yum install dotnet-sdk-6.0 -y  # install .net sdk
yum install git -y
mkdir /src
cd /src
git clone https://github.com/igorgomes96/cipa-api.git
cd "/src/cipa-api/1 - WebApi/Cipa.WebApi"
export DOTNET_CLI_HOME=/tmp
/usr/bin/dotnet publish --configuration Release /p:CustomTransformFileName=aws.transform -o /var/www/cipa
chmod -R 777 /var/www/cipa/Assets
mkdir /var/www/cipa/StaticFiles
chmod -R 777 /var/www/cipa/StaticFiles
rm -rf /etc/systemd/system/cipa.service 
touch /etc/systemd/system/cipa.service # cria o arquivo de definição de serviço
chmod -R 777 /etc/systemd/system/cipa.service
echo "[Unit]" >> /etc/systemd/system/cipa.service
echo "Description=Servico da CIPA" >> /etc/systemd/system/cipa.service
echo " " >> /etc/systemd/system/cipa.service
echo "[Service]" >> /etc/systemd/system/cipa.service
echo "WorkingDirectory=/var/www/cipa" >> /etc/systemd/system/cipa.service
echo "ExecStart=/usr/bin/dotnet /var/www/cipa/Cipa.WebApi.dll" >> /etc/systemd/system/cipa.service
echo "Restart=always" >> /etc/systemd/system/cipa.service
echo "# Restart service after 10 seconds if the dotnet service crashes:" >> /etc/systemd/system/cipa.service
echo "RestartSec=10" >> /etc/systemd/system/cipa.service
echo "KillSignal=SIGINT" >> /etc/systemd/system/cipa.service
echo "SyslogIdentifier=cipa" >> /etc/systemd/system/cipa.service
echo "Environment=ASPNETCORE_ENVIRONMENT=Production" >> /etc/systemd/system/cipa.service
echo "Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false" >> /etc/systemd/system/cipa.service
echo " " >> /etc/systemd/system/cipa.service
echo "# How many seconds to wait for the app to shut down after it receives the initial interrupt signal. " >> /etc/systemd/system/cipa.service
echo "# If the app doesn't shut down in this period, SIGKILL is issued to terminate the app. " >> /etc/systemd/system/cipa.service
echo "# The default timeout for most distributions is 90 seconds." >> /etc/systemd/system/cipa.service
echo "TimeoutStopSec=90" >> /etc/systemd/system/cipa.service
echo " " >> /etc/systemd/system/cipa.service
echo "[Install]" >> /etc/systemd/system/cipa.service
echo "WantedBy=multi-user.target" >> /etc/systemd/system/cipa.service
systemctl enable cipa.service # habilita inicialização automática
systemctl start cipa
chmod -R 777 /etc/nginx/nginx.conf
echo "user nginx;" > /etc/nginx/nginx.conf
echo "worker_processes auto;" >> /etc/nginx/nginx.conf
echo "error_log /var/log/nginx/error.log notice;" >> /etc/nginx/nginx.conf
echo "pid /run/nginx.pid;" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "# Load dynamic modules. See /usr/share/doc/nginx/README.dynamic." >> /etc/nginx/nginx.conf
echo "include /usr/share/nginx/modules/*.conf;" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "events {" >> /etc/nginx/nginx.conf
echo "    worker_connections 1024;" >> /etc/nginx/nginx.conf
echo "}" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "http {" >> /etc/nginx/nginx.conf
echo "    log_format  main  '\$remote_addr - \$remote_user [\$time_local] "\$request" '" >> /etc/nginx/nginx.conf
echo "                      '\$status \$body_bytes_sent "\$http_referer" '" >> /etc/nginx/nginx.conf
echo "                      '"\$http_user_agent" "\$http_x_forwarded_for"';" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "    access_log  /var/log/nginx/access.log  main;" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "    sendfile            on;" >> /etc/nginx/nginx.conf
echo "    tcp_nopush          on;" >> /etc/nginx/nginx.conf
echo "    keepalive_timeout   65;" >> /etc/nginx/nginx.conf
echo "    types_hash_max_size 4096;" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "    include             /etc/nginx/mime.types;" >> /etc/nginx/nginx.conf
echo "    default_type        application/octet-stream;" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "    include /etc/nginx/conf.d/*.conf;" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "    server {" >> /etc/nginx/nginx.conf
echo "        listen       80;" >> /etc/nginx/nginx.conf
echo "        listen       [::]:80;" >> /etc/nginx/nginx.conf
echo "        server_name  [cipa.4uptech.com.br];" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "        location / {" >> /etc/nginx/nginx.conf
echo "            root         /var/www/cipa/wwwroot/;" >> /etc/nginx/nginx.conf
echo "            index        index.html;" >> /etc/nginx/nginx.conf
echo "            try_files \$uri \$uri/ /index.html;" >> /etc/nginx/nginx.conf
echo "        }" >> /etc/nginx/nginx.conf
echo "" >> /etc/nginx/nginx.conf
echo "        location /api/ {" >> /etc/nginx/nginx.conf
echo "            proxy_pass         http://localhost:5000;" >> /etc/nginx/nginx.conf
echo "            proxy_http_version 1.1;" >> /etc/nginx/nginx.conf
echo "            proxy_set_header   Upgrade \$http_upgrade;" >> /etc/nginx/nginx.conf
echo "            proxy_set_header   Connection keep-alive;" >> /etc/nginx/nginx.conf
echo "            proxy_set_header   Host \$host;" >> /etc/nginx/nginx.conf
echo "            proxy_cache_bypass \$http_upgrade;" >> /etc/nginx/nginx.conf
echo "            proxy_set_header   X-Forwarded-For \$proxy_add_x_forwarded_for;" >> /etc/nginx/nginx.conf
echo "            proxy_set_header   X-Forwarded-Proto \$scheme;" >> /etc/nginx/nginx.conf
echo "        }" >> /etc/nginx/nginx.conf
echo "    } " >> /etc/nginx/nginx.conf
echo "}" >> /etc/nginx/nginx.conf
systemctl restart nginx
