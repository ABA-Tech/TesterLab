# Étape 1 : Image de base runtime .NET 7
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# --- Installer Chrome & ChromeDriver dans l'image finale ---
# (on le fait dès maintenant pour que tout soit prêt pour Selenium)
RUN apt-get update && apt-get install -y wget unzip \
    fonts-liberation libappindicator3-1 libnss3 libxss1 libasound2 \
    libatk-bridge2.0-0 libgtk-3-0 xdg-utils \
    && wget https://storage.googleapis.com/chrome-for-testing-public/140.0.7339.20700/linux64/chrome-linux64.zip \
    && unzip chrome-linux64.zip -d /opt/chrome \
    && ln -s /opt/chrome/chrome-linux64/chrome /usr/bin/google-chrome \
    && wget https://storage.googleapis.com/chrome-for-testing-public/140.0.7339.20700/linux64/chromedriver-linux64.zip \
    && unzip chromedriver-linux64.zip -d /usr/local/bin/ \
    && chmod +x /usr/local/bin/chromedriver \
    && rm -rf /var/lib/apt/lists/* chrome-linux64.zip chromedriver-linux64.zip

# Étape 2 : Image de build .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copier les fichiers de projet (pour restaurer les dépendances plus vite)
COPY ["front/TesterLab.csproj", "front/"]
COPY ["backend/TesterLab.Application/TesterLab.Applications.csproj", "backend/TesterLab.Application/"]
COPY ["backend/TesterLab.Domain/TesterLab.Domain.csproj", "backend/TesterLab.Domain/"]
COPY ["backend/TesterLab.Infrastructure.Selenium/TesterLab.Infrastructure.Selenium.csproj", "backend/TesterLab.Infrastructure.Selenium/"]
COPY ["backend/TesterLab.Infrastructure/TesterLab.Infrastructure.csproj", "backend/TesterLab.Infrastructure/"]

# Restaurer les dépendances
RUN dotnet restore "front/TesterLab.csproj"

# Copier tout le reste
COPY . .

# Construire l'application
WORKDIR "/src/front"
RUN dotnet build "TesterLab.csproj" -c Release -o /app/build

# Étape 3 : Publication
FROM build AS publish
RUN dotnet publish "TesterLab.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Étape 4 : Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Chrome + ChromeDriver sont déjà installés depuis la première étape
ENV PATH="${PATH}:/usr/local/bin"

# Démarre l'application
ENTRYPOINT ["dotnet", "TesterLab.dll"]
