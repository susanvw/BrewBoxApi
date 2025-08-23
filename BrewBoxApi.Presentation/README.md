# INSTALL SQL
1. docker pull mcr.microsoft.com/mssql/server

2. docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Password@1' \
       -p 1433:1433 -d \
       mcr.microsoft.com/mssql/server:latest

-e 'ACCEPT_EULA=Y' accepts the End-User Licensing Agreement. 
-e 'SA_PASSWORD=<password>' sets the password for the sa user. 
-p 1433:1433 maps the container's port 1433 to your Mac's port 1433. 
-d runs the container in detached mode (in the background). 
mcr.microsoft.com/mssql/server:latest specifies the image to use. 

