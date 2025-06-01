FROM nikolaik/python-nodejs:latest

RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

RUN dpkg -i packages-microsoft-prod.deb

RUN rm packages-microsoft-prod.deb

RUN apt-get update

RUN apt-get install -y dotnet-sdk-8.0

RUN apt-get install -y dotnet-sdk-9.0

WORKDIR /appsrc
