# ==========================================
# Étape 1 : Base ASP.NET Runtime + Chrome + ChromeDriver
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Installer dépendances système + Chrome + ChromeDriver
ENV CHROME_VERSION=141.0.7390.78
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y \
    wget unzip ca-certificates gnupg \
    fonts-liberation libappindicator3-1 libnss3 libxss1 \
    libasound2 libatk-bridge2.0-0 libgtk-3-0 xdg-utils \
    libgbm1 libxshmfence1 libxi6 libgconf-2-4 \
 && wget -q "https://storage.googleapis.com/chrome-for-testing-public/${CHROME_VERSION}/linux64/chrome-linux64.zip" \
 && unzip -q chrome-linux64.zip -d /opt/chrome \
 && ln -s /opt/chrome/chrome-linux64/chrome /usr/bin/google-chrome \
 && wget -q "https://storage.googleapis.com/chrome-for-testing-public/${CHROME_VERSION}/linux64/chromedriver-linux64.zip" \
 && unzip -q chromedriver-linux64.zip -d /tmp \
 && mv /tmp/chromedriver-linux64/chromedriver /usr/local/bin/chromedriver \
 && chmod +x /usr/local/bin/chromedriver \
 && rm -rf /var/lib/apt/lists/* chrome-linux64.zip chromedriver-linux64.zip /tmp/chromedriver-linux64

# Variables d'environnement pour Selenium
ENV PATH="/usr/local/bin:/opt/chrome/chrome-linux64:${PATH}"
ENV CHROME_BIN="/usr/bin/google-chrome"
ENV CHROMEDRIVER_PATH="/usr/local/bin/chromedriver"

# ==========================================
# Étape 2 : Build .NET SDK
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copier les fichiers de projet (pour une restauration plus rapide)
COPY ["front/TesterLab.csproj", "front/"]
COPY ["backend/TesterLab.Application/TesterLab.Applications.csproj", "backend/TesterLab.Application/"]
COPY ["backend/TesterLab.Domain/TesterLab.Domain.csproj", "backend/TesterLab.Domain/"]
COPY ["backend/TesterLab.Infrastructure.Selenium/TesterLab.Infrastructure.Selenium.csproj", "backend/TesterLab.Infrastructure.Selenium/"]
COPY ["backend/TesterLab.Infrastructure/TesterLab.Infrastructure.csproj", "backend/TesterLab.Infrastructure/"]

# Restaurer les dépendances
RUN dotnet restore "front/TesterLab.csproj"

# Copier le reste du code
COPY . .

# Compiler
WORKDIR "/src/front"
RUN dotnet build "TesterLab.csproj" -c Release -o /app/build

# ==========================================
# Étape 3 : Publication
# ==========================================
FROM build AS publish
RUN dotnet publish "TesterLab.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# Étape 4 : Image finale exécutable
# ==========================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TesterLab.dll"]
